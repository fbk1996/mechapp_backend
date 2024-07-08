using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace MechAppBackend.features
{
    public class airConditioning
    {
        MechAppContext _context;

        public airConditioning(MechAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of air conditioning service entries based on various filters.
        /// </summary>
        /// <param name="dateFrom">The start date for filtering entries.</param>
        /// <param name="dateTo">The end date for filtering entries.</param>
        /// <param name="status">A list of statuses to filter entries.</param>
        /// <param name="departmentID">A list of department IDs to filter entries.</param>
        /// <param name="name">A name to filter entries by client's name or lastname.</param>
        /// <param name="type">The type of air conditioning service to filter entries.</param>
        /// <param name="pageSize">The number of entries to return.</param>
        /// <param name="offset">The offset from where to start returning entries.</param>
        /// <returns>A list of filtered air conditioning service entries.</returns>
        public List<airConditioningEntries> GetEntries(DateTime? dateFrom, DateTime? dateTo, List<int>? status, List<int>? departmentID, string? name, List<int>? type, int pageSize, int offset)
        {

            try
            {
                IQueryable<AirConditioningEntry> query = _context.AirConditioningEntries;
                
                // Apply filters
                if (dateFrom != null && dateTo != null)
                    query = query.Where(a => a.Date >= dateFrom && a.Date <= dateTo);
                if (status.Count != 0)
                    query = query.Where(a => status.Contains((int)a.Status));
                if (departmentID.Count != 0)
                    query = query.Where(a => departmentID.Contains((int)a.DepartmentId));
                if (!string.IsNullOrEmpty(name))
                    query = query.Where(a => a.Client.Name.ToLower().Contains(name.ToLower().Trim()) || a.Client.Lastname.ToLower().Contains(name.ToLower().Trim()));
                if (type.Count != 0)
                    query = query.Where(a => type.Contains((int)a.Type));
                

                query = query.Where(a => a.IsDeleted != 1);
                // Execute query and project results
                var entries = query
                    .AsNoTracking()
                    .Skip(offset)
                    .Take(pageSize)
                    .Select(a => new airConditioningEntries
                    {
                        id = (int)a.Id,
                        client = new airConditioningClient
                        {
                            id = (int)a.Client.Id,
                            name = a.Client.Name,
                            lastname = a.Client.Lastname
                        },
                        vehicle = new airConditioningVehicle
                        {
                            id = (int)a.Vehicle.Id,
                            producer = a.Vehicle.Producer,
                            model = a.Vehicle.Model,
                            registrationNumber = a.Vehicle.RegistrationNumber
                        },
                        date = a.Date,
                        status = a.Status,
                        type = a.Type
                    }).ToList();

                return entries;
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "airConditioning", "GetEntries", ex);
                return new List<airConditioningEntries>();
            }
        }

        /// <summary>
        /// Retrieves a list of air conditioning service reports based on the year and type.
        /// </summary>
        /// <param name="date">The year to filter reports.</param>
        /// <param name="type">The type of air conditioning service to filter reports.</param>
        /// <param name="pageSize">The number of reports to return.</param>
        /// <param name="offset">The offset from where to start returning reports.</param>
        /// <returns>A list of filtered air conditioning service reports.</returns>
        public List<airConditioningReports> GetReports(DateTime? dateFrom, DateTime? dateTo, List<int>? type, List<int>? departmentIds, int pageSize, int offset)
        {
            try
            {
                IQueryable<AirConditioningReport> query = _context.AirConditioningReports;
                // Apply filters
                if (dateFrom != null && dateTo == null)
                {
                    query = query.Where(a => a.DateFrom >= dateFrom);
                }
                else if (dateFrom == null && dateTo != null)
                    query = query.Where(a => a.DateTo <= dateTo);
                else if (dateFrom != null && dateTo != null)
                    query = query.Where(a => a.DateFrom >= dateFrom && a.DateTo <= dateTo);

                if (type.Count != 0)
                    query = query.Where(a => type.Contains((int)a.Type));

                if (departmentIds.Count != 0)
                    query = query.Where(a => departmentIds.Contains((int)a.DepartmentId));
                // Execute query and project results
                var reports = query
                    .AsNoTracking()
                    .Skip(offset)
                    .Take(pageSize)
                    .Select(a => new airConditioningReports
                    {
                        id = (int)a.Id,
                        date = Convert.ToDateTime(a.DateFrom).Year.ToString(),
                        amountEnd = a.AmountEnd,
                        type = a.Type,
                        department = new airConditioningReportsDepartment
                        {
                            id = (int)a.Department.Id,
                            name = a.Department.Name
                        }
                    }).ToList();

                return reports;
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "airConditioning", "GetReports", ex);
                return new List<airConditioningReports>();
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific air conditioning service entry.
        /// </summary>
        /// <param name="id">The ID of the air conditioning service entry to retrieve details for. If -1, indicates no specific entry was requested.</param>
        /// <returns>
        /// An instance of airConditioningEntriesDetails containing detailed information about the requested air conditioning service entry.
        /// If no entry is found or an invalid ID is provided, returns an instance with an ID of -1.
        /// </returns>
        public airConditioningEntriesDetails GetAirConditioningEntryDetails(int id = -1)
        {
            // Check if the provided ID is valid
            if (id == -1)
                return new airConditioningEntriesDetails { id = -1 };

            try
            {
                // Query the database for the specified air conditioning entry
                var entry = (airConditioningEntriesDetails)_context.AirConditioningEntries
                    .AsNoTracking()
                    .Where(a => a.Id == id)
                    .Select(a => new airConditioningEntriesDetails
                    {
                        id = (int)a.Id,
                        client = new airConditioningClient
                        {
                            id = (int)a.Client.Id,
                            name = a.Client.Name,
                            lastname = a.Client.Lastname
                        },
                        vehicle = new airConditioningVehicle
                        {
                            id = (int)a.Vehicle.Id,
                            producer = a.Vehicle.Producer,
                            model = a.Vehicle.Model,
                            registrationNumber = a.Vehicle.RegistrationNumber
                        },
                        date = a.Date,
                        status = a.Status,
                        type = a.Type,
                        amountGained = a.AmountGained,
                        amountGived = a.AmountGived
                    }).FirstOrDefault();

                return entry ?? new airConditioningEntriesDetails { id = -1 }; // Return the found entry or a default instance if not found
            }
            catch (MySqlException ex)
            {
                // Handle any exceptions that occur during the database query
                Logger.SendException("MechApp", "airConditioning", "GetAirConditioningEntryDetails", ex);
                return new airConditioningEntriesDetails { id = -1 };
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific air conditioning service report.
        /// </summary>
        /// <param name="id">The ID of the air conditioning service report to retrieve details for. If -1, indicates no specific report was requested.</param>
        /// <returns>
        /// An instance of airConditioningReportsDetails containing detailed information about the requested air conditioning service report.
        /// If no report is found or an invalid ID is provided, returns an instance with an ID of -1.
        /// </returns>
        public airConditioningReportsDetails GetReportDetails(int id = -1)
        {
            // Check if the provided ID is valid
            if (id == -1)
                return new airConditioningReportsDetails { id = -1 };

            try
            {
                // Query the database for the specified air conditioning report
                var report = (airConditioningReportsDetails)_context.AirConditioningReports
                    .AsNoTracking()
                    .Where(a => a.Id == id)
                    .Select(a => new airConditioningReportsDetails
                    {
                        id = (int)a.Id,
                        date = Convert.ToDateTime(a.DateFrom).Year.ToString(),
                        amountEnd = a.AmountEnd,
                        type = a.Type,
                        entries = _context.AirConditioningEntries
                            .Where(ae => ae.Date >= a.DateFrom && ae.Date <= a.DateTo && ae.DepartmentId == a.DepartmentId)
                            .Select(ae => new airConditioningEntriesDetails
                            {
                                id = (int)ae.Id,
                                client = new airConditioningClient
                                {
                                    id = (int)ae.Client.Id,
                                    name = ae.Client.Name,
                                    lastname = ae.Client.Lastname
                                },
                                vehicle = new airConditioningVehicle
                                {
                                    id = (int)ae.Vehicle.Id,
                                    producer = ae.Vehicle.Producer,
                                    model = ae.Vehicle.Model,
                                    registrationNumber = ae.Vehicle.RegistrationNumber
                                },
                                date = ae.Date,
                                status = ae.Status,
                                type = ae.Type,
                                amountGained = ae.AmountGained,
                                amountGived = ae.AmountGived,
                            }).ToList(),
                        department = new airConditioningReportsDepartment
                        {
                            id = (int)a.Department.Id,
                            name = a.Department.Name
                        }
                    }).FirstOrDefault();

                return report ?? new airConditioningReportsDetails { id = -1 }; // Return the found report or a default instance if not found
            }
            catch (MySqlException ex)
            {
                // Handle any exceptions that occur during the database query
                Logger.SendException("MechApp", "airConditioning", "GetReportDetails", ex);
                return new airConditioningReportsDetails { id = -1 };
            }
        }

        /// <summary>
        /// Adds a new air conditioning service entry to the database.
        /// </summary>
        /// <param name="entry">The air conditioning service entry details to add.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public string AddEntry(airConditioningAddEditEntries entry)
        {
            // Validate the provided entry details
            if (entry.departmentID == -1)
                return "no_department";
            if (entry.clientID == null && entry.status == 2)
                return "no_client";
            if (entry.vehicleID == null && entry.status == 2)
                return "no_vehicle";
            if (entry.status == -1)
                return "no_status";
            if (entry.type == -1)
                return "no_type";
            if (entry.amountGived == null && entry.status != 0)
                return "no_amount_gived";

            try
            {
                // Set the current date and time
                DateTime addDate = DateTime.Now;
                // Create a new AirConditioningEntry object and add it to the database
                _context.AirConditioningEntries.Add(new AirConditioningEntry
                {
                    DepartmentId = entry.departmentID,
                    ClientId = entry.clientID,
                    VehicleId = entry.vehicleID,
                    Status = (short)entry.status,
                    Type = (short)entry.type,
                    AmountGained = (entry.amountGained != null) ? entry.amountGained : 0,
                    AmountGived = entry.amountGived,
                    Date = addDate,
                    IsDeleted = 0
                });
                // Save changes to the database
                _context.SaveChanges();
                // Return a success message
                return "entry_added";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error message
                Logger.SendException("MechApp", "airConditioning", "AddEntries", ex);
                return "error";
            }
        }

        /// <summary>
        /// Edits an existing air conditioning service entry in the database.
        /// </summary>
        /// <param name="entry">The details of the air conditioning service entry to be edited.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public string EditEntry(airConditioningAddEditEntries entry)
        {
            // Validate the provided entry ID 
            if (entry.id == -1)
                return "error";

            try
            {
                // Retrieve the existing entry from the database
                var entryDb = _context.AirConditioningEntries.FirstOrDefault(a => a.Id == entry.id);
                // Check if the entry exists
                if (entryDb == null)
                    return "error";
                // Update the entry with the provided details
                entryDb.AmountGained = (entry.amountGained != null) ? entry.amountGained : 0;
                entryDb.AmountGived = entry.amountGived;
                // Save changes to the database
                _context.SaveChanges();
                // Return a success message
                return "entry_edited";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error message
                Logger.SendException("MechApp", "airConditioning", "EditEntry", ex);
                return "error";
            }
        }

        /// <summary>
        /// Generates a report for air conditioning services within a specified year and for a specific type and department.
        /// </summary>
        /// <param name="report">The details required to generate the report, including the year, type, and department ID.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public string GenerateReport(airConditioningGenerateReports report)
        {
            // Check if the report date or type is not provided correctly
            if (report.date == null)
                return "no_date";
            if (report.type == -1)
                return "no_type";
            if (report.departmentID == -1)
                return "no_department_id";

            try
            {
                // Calculate the start and end dates for the report based on the provided year
                var startDate = new DateTime(report.date.Value.Year, 1, 1);
                var endDate = startDate.AddYears(1).AddTicks(-1);
                // Check if a report for the given criteria already exists
                var checkReport = _context.AirConditioningReports.FirstOrDefault(a => a.DateFrom == startDate && a.DateTo == endDate && a.DepartmentId == report.departmentID && a.Type == report.type);

                if (checkReport != null)
                    return "exists";
                // Retrieve entries that match the report criteria
                var entries = _context.AirConditioningEntries
                    .Where(a => a.Date >= startDate && a.Date <= endDate && a.Type == report.type && a.DepartmentId == report.departmentID)
                    .Select(a => new airConditioningReportsEntries
                    {
                        id = (int)a.Id,
                        amountGained = a.AmountGained,
                        amountGiven = a.AmountGived,
                        status = a.Status
                    }).ToList();

                float amountEnd = 0;
                // Calculate the final amount for the report
                foreach (var entry in entries)
                {
                    if (entry.status == 0) // Status 0: Add amount gained
                        amountEnd += (float)entry.amountGained;
                    else if (entry.status == 1) // Status 1: Subtract amount given
                        amountEnd -= (float)entry.amountGiven;
                    else // Other statuses: Subtract the difference between given and gained
                    {
                        var amount = (float)entry.amountGiven - (float)entry.amountGained;
                        amountEnd -= amount;
                    }
                }
                // Add the new report to the database
                _context.AirConditioningReports.Add(new AirConditioningReport
                {
                    DateFrom = startDate,
                    DateTo = endDate,
                    AmountEnd = amountEnd,
                    Type = (short)report.type,
                    DepartmentId = report.departmentID
                });
                // Save changes to the database
                _context.SaveChanges();
                // Return a success message
                return "report_added";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error message
                Logger.SendException("MechApp", "airConditioning", "GenerateReport", ex);
                return "error";
            }
        }

        /// <summary>
        /// Handles the deletion of an air conditioning service entry from the database.
        /// </summary>
        /// <param name="id">The ID of the air conditioning service entry to delete. If -1, indicates no specific entry was selected for deletion.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public string DeleteEntry(int id = -1)
        {
            // Check if a valid ID was provided
            if (id == -1)
                return "error";

            try
            {
                // Attempt to find the entry in the database using the provided ID
                var entry = _context.AirConditioningEntries.FirstOrDefault(a => a.Id == id);
                // Mark the entry as deleted
                entry.IsDeleted = 1;
                // Save the changes to the database
                _context.SaveChanges();
                // Return a success message indicating the entry has been deleted
                return "entry_deleted";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error message if an exception occurs
                Logger.SendException("MechApp", "airConditioning", "DeleteEntry", ex);
                return "error";
            }
        }

        /// <summary>
        /// Deletes multiple air conditioning service entries based on their IDs.
        /// </summary>
        /// <param name="ids">A list of IDs of the air conditioning service entries to be deleted.</param>
        /// <returns>A string indicating the result of the operation.</returns>
        public string DeleteEntries(List<int> ids)
        {
            // Check if the list of IDs is empty
            if (ids.Count == 0)
                return "error"; // Return an error message if the list is empty

            try
            {
                // Retrieve the entries from the database that match the provided IDs
                var entries = _context.AirConditioningEntries
                    .Where(a => ids.Contains((int)a.Id))
                    .ToList();
                // Mark each retrieved entry as deleted
                foreach (var entry in entries)
                {
                    entry.IsDeleted = 1;
                }
                // Save the changes to the database
                _context.SaveChanges();
                // Return a success message indicating the entries have been deleted
                return "entries_deleted";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error message if an exception occurs
                Logger.SendException("MechApp", "airConditioning", "DeleteEntries", ex);
                return "error";
            }
        }
    }

    public class airConditioningEntries
    {
        public int? id { get; set; }
        public airConditioningClient? client { get; set; }
        public airConditioningVehicle? vehicle { get; set; }
        public DateTime? date { get; set; }
        public int? status { get; set; }
        public int? type { get; set; }
    }

    public class airConditioningEntriesDetails
    {
        public int? id { get; set; }
        public airConditioningClient? client { get; set; }
        public airConditioningVehicle? vehicle { get; set; }
        public DateTime? date { get; set; }
        public int? status { get; set; }
        public int? type { get; set; }
        public float? amountGained { get; set; }
        public float? amountGived { get; set; }
    }

    public class airConditioningAddEditEntries
    {
        public int? id { get; set; } = -1;
        public int? departmentID { get; set; } = -1;
        public int? clientID { get; set; }
        public int? vehicleID { get; set; }
        public int? status { get; set; } = -1;
        public int? type { get; set; } = -1;
        public float? amountGained { get; set; } = -1;
        public float? amountGived { get; set; } = -1;
    }

    public class airConditioningReports
    {
        public int? id { get; set; }
        public string? date { get; set; }
        public float? amountEnd { get; set; }
        public int? type { get; set; }
        public airConditioningReportsDepartment? department { get; set; }
    }

    public class airConditioningReportsDepartment
    {
        public int? id { get; set; }
        public string? name { get; set; }
    }

    public class airConditioningReportsDetails
    {
        public int? id { get; set; }
        public string? date { get; set; }
        public float? amountEnd { get; set; }
        public int? type { get; set; }
        public List<airConditioningEntriesDetails>? entries { get; set; }
        public airConditioningReportsDepartment? department { get; set; }
    }

    public class airConditioningReportsEntries
    {
        public int? id { get; set; } = -1;
        public float? amountGained { get; set; }
        public float? amountGiven { get; set; }
        public int? status { get; set; }
    }

    public class airConditioningGenerateReports
    {
        public DateTime? date { get; set; }
        public int? type { get; set; } = -1;
        public int? departmentID { get; set; } = -1;
    }

    public class airConditioningClient
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
    }

    public class airConditioningVehicle
    {
        public int? id { get; set; }
        public string? producer { get; set; }
        public string? model { get; set; }
        public string? registrationNumber { get; set; }
    }
}
