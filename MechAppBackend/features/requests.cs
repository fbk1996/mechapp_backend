using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace MechAppBackend.features
{
    public class requests
    {
        MechAppContext _context;

        public requests(MechAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a paginated list of absence requests based on the optional search criteria and pagination settings.
        /// </summary>
        /// <param name="name">Optional. The name or last name to filter the absence requests by. If provided, only requests for users whose name or last name contains this value will be returned.</param>
        /// <param name="pageSize">The number of requests to return per page.</param>
        /// <param name="offset">The offset from the start of the dataset to begin returning requests. Used for pagination.</param>
        /// <returns>A list of absence request objects that match the search criteria and pagination settings. If an error occurs, an empty list is returned.</returns>
        /// <remarks>
        /// This method allows for filtering absence requests by user name or last name and supports pagination through the pageSize and offset parameters. It is useful for displaying a subset of absence requests in a UI.
        /// </remarks>
        public List<requestOb> GetRequests(string? name, int pageSize, int offset)
        {
            try
            {
                // Start with all absence requests
                IQueryable<AbsenceRequest> query = _context.AbsenceRequests;
                // Filter by name or last name if provided
                if (!string.IsNullOrEmpty(name))
                    query = query.Where(r => r.User.Name.ToLower().Contains(name.ToLower().Trim()) || r.User.Lastname.ToLower().Contains(name.ToLower().Trim()));
                // Apply pagination and project to DTO
                List<requestOb> requests = query
                    .AsNoTracking()
                    .Skip(offset)
                    .Take(pageSize)
                    .Select(r => new requestOb
                    {
                        id = (int)r.Id,
                        user = new requestUserOb
                        {
                            id = (int)r.UserId,
                            name = r.User.Name,
                            lastname = r.User.Lastname
                        },
                        dateStart = r.DateStart,
                        dateEnd = r.DateEnd,
                        status = r.Status,
                        absenceType = r.AbsenceType,
                        count = query.Count()
                    }).ToList();

                return requests;
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an empty list in case of a database error
                Logger.SendException("MechApp", "requests", "GetRequests", ex);
                return new List<requestOb>();
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific absence request by its ID.
        /// </summary>
        /// <param name="id">The ID of the absence request for which details are being retrieved.</param>
        /// <returns>
        /// An object containing detailed information about the specified absence request, including user details, request dates, status, absence type, description, and associated files.
        /// If the request is not found or an error occurs, an object with an ID of -1 is returned.
        /// </returns>
        /// <remarks>
        /// This method is used to fetch all relevant details for a specific absence request, including information about the user who made the request and any files associated with the request.
        /// It is particularly useful for displaying detailed information about an absence request in a user interface.
        /// </remarks>
        public requestDetailsOb GetRequestsDetails(int id)
        {
            try
            {
                // Query the database for the specified absence request and project the result into a requestDetailsOb object
                requestDetailsOb request = (requestDetailsOb)_context.AbsenceRequests
                    .AsNoTracking()
                    .Where(r => r.Id == id)
                    .Select(r => new requestDetailsOb
                    {
                        id = (int)r.Id,
                        user = new requestUserOb
                        {
                            id = (int)r.UserId,
                            name = r.User.Name,
                            lastname = r.User.Lastname
                        },
                        dateStart = r.DateStart,
                        dateEnd = r.DateEnd,
                        status = r.Status,
                        absenceType = r.AbsenceType,
                        submitDescription = r.SubmitDescription,
                        files = _context.AbsenceRequestsFiles
                            .Where(rf => rf.RequestId == r.Id)
                            .Select(rf => new requestFilesOb
                            {
                                id = (int)rf.Id,
                                file = rf.File
                            }).ToList()
                    }).FirstOrDefault();

                return request ?? new requestDetailsOb { id = -1 }; // Return the result or a default object if not found
            }
            catch (MySqlException ex)
            {
                // Log the exception and return a default object in case of a database error
                Logger.SendException("MechApp", "requests", "GetRequestsDetails", ex);
                return new requestDetailsOb { id = -1 };
            }
        }

        /// <summary>
        /// Generates a detailed print-ready object for a specific absence request, including business days calculation.
        /// </summary>
        /// <param name="id">The ID of the absence request. If -1, a default object with an ID of -1 is returned.</param>
        /// <returns>
        /// A detailed object containing information about the absence request suitable for printing, including calculated business days excluding holidays.
        /// If the request is not found or an error occurs, an object with an ID of -1 is returned.
        /// </returns>
        /// <remarks>
        /// This method is particularly useful for generating detailed reports or printouts for absence requests, including the calculation of business days between the start and end dates of the absence, excluding holidays.
        /// </remarks>
        public requestDetailsPrintob GetRequestPrint(int id = -1)
        {
            if (id == -1)
                return new requestDetailsPrintob { id = -1 };

            try
            {
                // Calculate the list of holidays
                List<DateTime> holidaysDays = CalculateDates.holidayDays();
                // Query the database for the specified absence request and project the result into a requestDetailsPrintob object
                requestDetailsPrintob request = (requestDetailsPrintob)_context.AbsenceRequests
                    .Where(r => r.Id == id)
                    .Select(r => new requestDetailsPrintob
                    {
                        id = (int)r.Id,
                        user = new requestUserOb
                        {
                            id = (int)r.UserId,
                            name = r.User.Name,
                            lastname = r.User.Lastname
                        },
                        createDate = r.CreateDate,
                        dateStart = r.DateStart,
                        dateEnd = r.DateEnd,
                        status = r.Status,
                        absenceType = r.AbsenceType,
                        submitDescription = r.SubmitDescription,
                        // Calculate business days excluding holidays
                        businessDays = CalculateDates.BusinessDaysUntil((DateTime)r.DateStart, (DateTime)r.DateEnd, holidaysDays)
                    }).FirstOrDefault();

                return request ?? new requestDetailsPrintob { id = -1 }; // Return the result or a default object if not found
            }
            catch (MySqlException ex)
            {
                // Log the exception and return a default object in case of a database error
                Logger.SendException("MechApp", "requests", "GetRequestPrint", ex);
                return new requestDetailsPrintob { id = -1 };
            }
        }

        /// <summary>
        /// Adds a new absence request to the database.
        /// </summary>
        /// <param name="request">An object containing the details of the absence request to be added.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "no_id" if the user ID is not provided,
        /// - "no_date_start" if the start date of the absence is not provided,
        /// - "no_date_end" if the end date of the absence is not provided,
        /// - "no_absence_type" if the type of absence is not provided,
        /// - "exists" if an overlapping request for the same user already exists,
        /// - "request_added" if the request is successfully added,
        /// - "error" if a database error occurs.
        /// </returns>
        /// <remarks>
        /// This method checks for mandatory fields, validates that no overlapping absence request exists for the same user, and then adds a new absence request to the database.
        /// </remarks>
        public string AddRequest(AddRequestOb request)
        {
            // Validate input parameters
            if (request.userID == -1)
                return "no_id";
            if (request.dateStart == null)
                return "no_date_start";
            if (request.dateEnd == null)
                return "no_date_end";
            if (string.IsNullOrEmpty(request.absenceType))
                return "no_absence_type";

            try
            {
                // Check for existing overlapping requests
                var checkRequest = _context.AbsenceRequests.FirstOrDefault(r => request.dateStart >= r.DateStart && request.dateEnd <= r.DateEnd && r.UserId == request.userID);

                if (checkRequest != null)
                    return "exists";

                DateTime createDate = DateTime.Now;
                // Create and add the new absence request
                _context.AbsenceRequests.Add(new AbsenceRequest
                {
                    UserId = request.userID,
                    DateStart = request.dateStart,
                    DateEnd = request.dateEnd,
                    AbsenceType = request.absenceType,
                    CreateDate = createDate,
                    Status = 0
                });
                // Save changes to the database
                _context.SaveChanges();

                return "request_added";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error message
                Logger.SendException("MechApp", "requests", "AddRequest", ex);
                return "error";
            }
        }

        /// <summary>
        /// Adds files to an existing absence request in the database.
        /// </summary>
        /// <param name="request">An object containing the ID of the absence request and a list of files to be added.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the request ID is not provided or a database error occurs,
        /// - "files_added" if the files are successfully added to the request.
        /// </returns>
        /// <remarks>
        /// This method allows for the addition of multiple files to an existing absence request by updating the database with the new file information.
        /// </remarks>
        public string AddFilesToRequests(EditRequestOb request)
        {
            // Validate if the request ID is provided
            if (request.id == -1)
                return "error";

            try
            {
                // Iterate through each file in the request and add it to the database
                foreach (var file in request.files)
                {
                    _context.AbsenceRequestsFiles.Add(new AbsenceRequestsFile
                    {
                        RequestId = request.id,
                        File = file.file
                    });
                }
                // Save the changes to the database
                _context.SaveChanges();

                return "files_added";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error indication
                Logger.SendException("MechApp", "requests", "AddFilesToRequests", ex);
                return "error";
            }
        }

        /// <summary>
        /// Deletes a file associated with an absence request from the database.
        /// </summary>
        /// <param name="fileID">The ID of the file to be deleted. If -1, an error is returned.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the file ID is not provided or a database error occurs,
        /// - "file_deleted" if the file is successfully deleted.
        /// </returns>
        /// <remarks>
        /// This method removes a file from an absence request by deleting its record from the database. It is useful for managing the files associated with absence requests.
        /// </remarks>
        public string DeleteFile(int fileID = -1)
        {
            // Validate if the file ID is provided
            if (fileID == -1)
                return "error";

            try
            {
                // Find the file by ID and remove it from the database
                _context.AbsenceRequestsFiles.RemoveRange(_context.AbsenceRequestsFiles.FirstOrDefault(rf => rf.Id == fileID));
                // Save the changes to the database
                _context.SaveChanges();
                // Indicate success
                return "file_deleted";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error indication
                Logger.SendException("MechApp", "requests", "DeleteFile", ex);
                return "error";
            }
        }

        /// <summary>
        /// Changes the status of an absence request in the database.
        /// </summary>
        /// <param name="status">An object containing the ID of the request and the new status to be set.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "no_id" if the request ID is not provided,
        /// - "no_status" if the new status is not provided,
        /// - "no_submit_description" if the status is set to 2 (rejected) but no description is provided,
        /// - "error" in case of a database error or if the request does not exist,
        /// - "status_changed" if the status change is successful.
        /// </returns>
        /// <remarks>
        /// This method allows for changing the status of an absence request, which is useful for managing the lifecycle of requests.
        /// </remarks>
        public string ChangeOrderStatus(ChangeRequestStatusOb status)
        {
            // Validate input parameters
            if (status.id == -1)
                return "no_id";
            if (status.status == -1)
                return "no_status";
            if (status.status == 2 && !string.IsNullOrEmpty(status.submitDescription))
                return "no_submit_description"; // No description provided for a rejected request

            try
            {
                // Find the request by ID
                var request = _context.AbsenceRequests.FirstOrDefault(r => r.Id == status.id);

                if (request == null)
                    return "error";
                // Update the request status and submit description
                request.Status = (short)status.status;
                request.SubmitDescription = status.submitDescription;
                // Save changes to the database
                _context.SaveChanges();

                return "status_changed";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error message
                Logger.SendException("MechApp", "requests", "ChangeOrderStatus", ex);
                return "error";
            }
        }
    }

    public class requestOb
    {
        public int? id { get; set; }
        public requestUserOb? user { get; set; }
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        public int? status { get; set; }
        public string? absenceType { get; set; }
        public int? count { get; set; }
    }

    public class requestDetailsOb
    {
        public int? id { get; set; } = -1;
        public requestUserOb? user { get; set; }
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        public int? status { get; set; }
        public string? absenceType { get; set; }
        public string? submitDescription { get; set; }
        public List<requestFilesOb>? files { get; set; }
    }

    public class requestDetailsPrintob
    {
        public int? id { get; set; } = -1;
        public requestUserOb? user { get; set; }
        public DateTime? createDate { get; set; }
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        public int? status { get; set; }
        public string? absenceType { get; set; }
        public string? submitDescription { get; set; }
        public int? businessDays { get; set; }
    }

    public class AddRequestOb 
    {
        public int? userID { get; set; }
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        public string? absenceType { get; set; }
    }

    public class EditRequestOb
    {
        public int? id { get; set; } = -1;
        public List<requestFilesOb>? files { get; set; }
    }

    public class ChangeRequestStatusOb
    {
        public int? id { get; set; } = -1;
        public int? status { get; set; } = -1;
        public string? submitDescription { get; set; }
    }

    public class requestUserOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
    }

    public class requestFilesOb
    {
        public int? id { get; set; } = -1;
        public string? file { get; set; }
    }
}
