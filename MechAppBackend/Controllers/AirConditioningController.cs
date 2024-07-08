using MechAppBackend.Data;
using MechAppBackend.features;
using MechAppBackend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MySql.Data.MySqlClient;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/airConditioning")]
    [ApiController]
    public class AirConditioningController : ControllerBase
    {
        MechAppContext _context;
        logs logsController;
        CheckCookieToken cookieToken;
        CheckRoles roles;
        airConditioning airConditioningController;

        public AirConditioningController(MechAppContext context)
        {
            _context = context;
            logsController = new logs(context);
            cookieToken = new CheckCookieToken(context);
            roles = new CheckRoles(context);
            airConditioningController = new airConditioning(context);
        }

        /*
         * Method: GetAirConditioningEntries
         * Description: This method retrieves air conditioning entries from the database based on the specified filters such as date range, status, department IDs, name, and type.
         *              It performs authentication and authorization checks, validates and refreshes session tokens, and logs the operation.
         * Parameters:
         *    - DateTime? dateFrom: The start date for filtering the entries. If null, filtering by start date is not applied.
         *    - DateTime? dateTo: The end date for filtering the entries. If null, filtering by end date is not applied.
         *    - string? status: A comma-separated list of status IDs for filtering the entries. If null or empty, no filtering by status is applied.
         *    - string? departmentIds: A comma-separated list of department IDs for filtering the entries. If null or empty, no filtering by department is applied.
         *    - string? name: A string for filtering the entries by name. If null or empty, no filtering by name is applied.
         *    - string? type: A comma-separated list of type IDs for filtering the entries. If null or empty, no filtering by type is applied.
         *    - int currentPage: The current page number for pagination.
         *    - int pageSize: The number of entries to display per page.
         * Returns: IActionResult: A JSON result containing the filtered list of air conditioning entries or an error message if the operation fails.
         * 
         * Process:
         *    1. Checks for the session token in the cookies. If absent, returns a "no_auth" result.
         *    2. Validates the session token. If invalid, returns a "no_auth" result.
         *    3. Refreshes the cookie expiration.
         *    4. Checks if the user is authorized to view air conditioning reports. If not, returns a "no_permission" result.
         *    5. Calculates the offset for pagination.
         *    6. Parses the status, departmentIds, and type parameters into lists of integers.
         *    7. Logs the operation of retrieving the entries.
         *    8. Calls the airConditioningController.GetEntries method to fetch the entries based on the filters and pagination parameters.
         *    9. Returns the entries in a JSON result with the status "done".
         *    10. Catches any MySqlException, logs the exception, and returns an error result.
         */
        [HttpGet("entries")]
        public IActionResult GetAirConditioningEntries(DateTime? dateFrom, DateTime? dateTo, string? status, string? departmentIds, string? name, string? type, 
            int currentPage, int pageSize)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Validate session token
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Refresh the cookie expiration
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "airConditioningReports", "view"))
                return new JsonResult(new { result = "no_permission" });

            int offset = ((currentPage - 1) * pageSize);

            List<int> statuses = (!string.IsNullOrEmpty(status)) ? status.Split(',').Select(id => Convert.ToInt32(id)).ToList() : new List<int>();
            List<int> depIds = (!string.IsNullOrEmpty(departmentIds)) ? departmentIds.Split(',').Select(id => Convert.ToInt32(id)).ToList() : new List<int>();
            List<int> types = (!string.IsNullOrEmpty(type)) ? type.Split(',').Select(id => Convert.ToInt32(id)).ToList() : new List<int>();


            try
            {
                logsController.AddLog(_cookieValue, "Pobranie listy wpisów ewidencji środków klimatyzacji");

                var entries = airConditioningController.GetEntries(dateFrom, dateTo, statuses, depIds, name, types, pageSize, offset);

                return new JsonResult(new
                {
                    result = "done",
                    entries = entries
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "AirConditioningController", "GetAirConditioningEntries", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /*
         * Method: GetAirConditioningReports
         * Description: This method retrieves air conditioning reports from the database based on specified filters such as date range, type, department IDs, and pagination parameters.
         *              It performs authentication and authorization checks, validates and refreshes session tokens, and logs the operation.
         * Parameters:
         *    - DateTime? dateFrom: The start date for filtering the reports. If null, filtering by start date is not applied.
         *    - DateTime? dateTo: The end date for filtering the reports. If null, filtering by end date is not applied.
         *    - string? type: A comma-separated list of report types for filtering. If null or empty, no filtering by type is applied.
         *    - string? departmentIds: A comma-separated list of department IDs for filtering the reports. If null or empty, no filtering by department is applied.
         *    - int currentPage: The current page number for pagination.
         *    - int pageSize: The number of reports per page for pagination.
         * Returns: IActionResult: A JSON result containing the filtered list of air conditioning reports or an error message if the operation fails.
         * 
         * Process:
         *    1. Checks for the session token in the cookies. If absent, returns a "no_auth" result.
         *    2. Validates the session token. If invalid, returns a "no_auth" result.
         *    3. Refreshes the cookie expiration.
         *    4. Checks if the user is authorized to view air conditioning reports. If not, returns a "no_permission" result.
         *    5. Calculates the offset for pagination.
         *    6. Parses the departmentIds and type parameters into lists of integers.
         *    7. Logs the operation of retrieving the reports.
         *    8. Calls the airConditioningController.GetReports method to fetch the reports based on the filters and pagination parameters.
         *    9. Returns the reports in a JSON result with the status "done".
         *    10. Catches any MySqlException, logs the exception, and returns an error result.
         */
        [HttpGet("reports")]
        public IActionResult GetAirConditioningReports(DateTime? dateFrom, DateTime? dateTo, string? type, string? departmentIds, int currentPage, int pageSize)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Validate session token
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Refresh the cookie expiration
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "airConditioningReports", "view"))
                return new JsonResult(new { result = "no_permission" });

            int offset = ((currentPage - 1) * pageSize);

            List<int> depIds = (!string.IsNullOrEmpty(departmentIds)) ? departmentIds.Split(',').Select(id => Convert.ToInt32(id)).ToList() : new List<int>();
            List<int> types = (!string.IsNullOrEmpty(type)) ? type.Split(',').Select(id => Convert.ToInt32(id)).ToList() : new List<int>();

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie listy raportów ewidencji środków klimatyzacji");

                var reports = airConditioningController.GetReports(dateFrom, dateTo, types, depIds, pageSize, offset);

                return new JsonResult(new
                {
                    result = "done",
                    reports = reports
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "AirConditioningController", "GetAirConditioningReports", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString() });
        }
        /*
         * Method: GetAirConditioningEntryDetails
         * Description: This method retrieves the details of a specific air conditioning entry from the database based on the provided entry ID.
         *              It performs authentication and authorization checks, validates and refreshes session tokens, and logs the operation.
         * Parameters:
         *    - int id: The ID of the air conditioning entry to be retrieved. Defaults to -1 if not provided.
         * Returns: IActionResult: A JSON result containing the details of the air conditioning entry or an error message if the operation fails.
         * 
         * Process:
         *    1. Checks for the session token in the cookies. If absent, returns a "no_auth" result.
         *    2. Validates the session token. If invalid, returns a "no_auth" result.
         *    3. Refreshes the cookie expiration.
         *    4. Checks if the user is authorized to view air conditioning entry details. If not, returns a "no_permission" result.
         *    5. Logs the operation of retrieving the entry details.
         *    6. Calls the airConditioningController.GetAirConditioningEntryDetails method to fetch the entry details based on the provided ID.
         *    7. If the entry ID is invalid, returns an "error" result.
         *    8. Returns the entry details in a JSON result with the status "done".
         *    9. Catches any MySqlException, logs the exception, and returns an error result.
         */
        [HttpGet("entries/details")]
        public IActionResult GetAirConditioningEntryDetails(int id = -1)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Validate session token
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Refresh the cookie expiration
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "airConditioningReports", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie szczegółów wpisu ewidencji środków klimatyzacji");

                var entry = airConditioningController.GetAirConditioningEntryDetails(id);

                if (entry.id == -1)
                    return new JsonResult(new { result = "error" });

                return new JsonResult(new
                {
                    result = "done",
                    entry = entry
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "AirConditioningController", "GetAirConditioningEntryDetails", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /*
         * Method: GetAirConditioningReportDetails
         * Description: This method retrieves the details of a specific air conditioning report from the database based on the provided report ID.
         *              It performs authentication and authorization checks, validates and refreshes session tokens, and logs the operation.
         * Parameters:
         *    - int id: The ID of the air conditioning report to be retrieved. Defaults to -1 if not provided.
         * Returns: IActionResult: A JSON result containing the details of the air conditioning report or an error message if the operation fails.
         * 
         * Process:
         *    1. Checks for the session token in the cookies. If absent, returns a "no_auth" result.
         *    2. Validates the session token. If invalid, returns a "no_auth" result.
         *    3. Refreshes the cookie expiration.
         *    4. Checks if the user is authorized to view air conditioning report details. If not, returns a "no_permission" result.
         *    5. Logs the operation of retrieving the report details.
         *    6. Calls the airConditioningController.GetReportDetails method to fetch the report details based on the provided ID.
         *    7. If the report ID is invalid, returns an "error" result.
         *    8. Returns the report details in a JSON result with the status "done".
         *    9. Catches any MySqlException, logs the exception, and returns an error result.
         */
        [HttpGet("reports/details")]
        public IActionResult GetAirConditioningReportDetails(int id = -1)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Validate session token
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Refresh the cookie expiration
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "airConditioningReports", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie szczegółów raportu ewidencji środków klimatyzacji");

                var report = airConditioningController.GetReportDetails(id);

                if (report.id == -1)
                    return new JsonResult(new { result = "error" });

                return new JsonResult(new
                {
                    result = "done",
                    report = report
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "AirConditioningController", "GetAirConditioningReportDetails", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /*
         * Method: AddAirConditioningEntry
         * Description: This method handles the HTTP POST request to add a new entry to the air conditioning records.
         *              It performs authentication and authorization checks, validates and refreshes session tokens, and logs the operation.
         * Parameters:
         *    - airConditioningAddEditEntries entry: The data structure containing the details of the entry to be added.
         * Returns: IActionResult: A JSON result indicating the outcome of the operation.
         * 
         * Process:
         *    1. Checks for the session token in the cookies. If absent, returns a "no_auth" result.
         *    2. Validates the session token. If invalid, returns a "no_auth" result.
         *    3. Refreshes the cookie expiration.
         *    4. Checks if the user is authorized to add entries to the air conditioning records. If not, returns a "no_permission" result.
         *    5. Logs the operation of adding a new entry.
         *    6. Calls the airConditioningController.AddEntry method to add the entry based on the provided data.
         *    7. Captures the result of the operation and constructs a JSON response.
         *    8. Catches any MySqlException, logs the exception, and returns an error result.
         */
        [HttpPost("entries")]
        public IActionResult AddAirConditioningEntry([FromBody]airConditioningAddEditEntries entry)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Validate session token
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Refresh the cookie expiration
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "airConditioningReports", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Dodanie nowego wpisu do ewidencji środków klimatyzacji");

                string result = airConditioningController.AddEntry(entry);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "AirConditioningController", "AddAirConditioningEntry", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString()});
        }

        /*
         * Method: GenerateAirConditioningReport
         * Description: Handles the HTTP POST request to generate an air conditioning inventory report based on provided criteria.
         *              Performs authentication and authorization checks, validates and refreshes session tokens, and logs the operation.
         * Parameters:
         *    - airConditioningGenerateReports report: Data structure containing criteria for generating the report.
         * Returns: IActionResult: JSON result indicating the outcome of the report generation operation.
         * 
         * Process:
         *    1. Checks for the session token in cookies. If absent, returns a "no_auth" result.
         *    2. Validates the session token. If invalid, returns a "no_auth" result.
         *    3. Refreshes the cookie expiration.
         *    4. Checks if the user is authorized to generate reports. If not, returns a "no_permission" result.
         *    5. Logs the operation of generating an air conditioning report.
         *    6. Calls the airConditioningController.GenerateReport method to generate the report based on provided criteria.
         *    7. Captures the result of the operation and constructs a JSON response.
         *    8. Catches any MySqlException, logs the exception, and returns an error result.
         */
        [HttpPost("reports")]
        public IActionResult GenerateAirConditioningReport([FromBody]airConditioningGenerateReports report)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Validate session token
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Refresh the cookie expiration
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "airConditioningReports", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Wygenerowanie raportu ewidencji środków klimatyzacji");

                string result = airConditioningController.GenerateReport(report);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "AirConditioningController", "GenerateAirConditioningReport", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /*
         * Method: EditAirConditioningEntry
         * Description: This method handles the HTTP PUT request to edit an existing entry in the air conditioning records.
         *              It performs authentication and authorization checks, validates and refreshes session tokens, and logs the operation.
         * Parameters:
         *    - airConditioningAddEditEntries entry: The data structure containing the details of the entry to be edited.
         * Returns: IActionResult: A JSON result indicating the outcome of the operation.
         * 
         * Process:
         *    1. Checks for the session token in the cookies. If absent, returns a "no_auth" result.
         *    2. Validates the session token. If invalid, returns a "no_auth" result.
         *    3. Refreshes the cookie expiration.
         *    4. Checks if the user is authorized to edit entries in the air conditioning records. If not, returns a "no_permission" result.
         *    5. Logs the operation of editing an existing entry.
         *    6. Calls the airConditioningController.EditEntry method to update the entry based on the provided data.
         *    7. Captures the result of the operation and constructs a JSON response.
         *    8. Catches any MySqlException, logs the exception, and returns an error result.
         */
        [HttpPut("entries")]
        public IActionResult EditAirConditioningEntry([FromBody]airConditioningAddEditEntries entry)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Validate session token
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Refresh the cookie expiration
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "airConditioningReports", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Edycja wpisu ewidencji środków klimatyzacji");

                string result = airConditioningController.EditEntry(entry);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "AirConditioningController", "EditAirConditioningEntry", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString()});
        }

        /*
         * Method: DeleteAirConditioningEntry
         * Description: This method handles the HTTP DELETE request to delete an existing entry from the air conditioning records.
         *              It performs authentication and authorization checks, validates and refreshes session tokens, and logs the operation.
         * Parameters:
         *    - int id: The ID of the entry to be deleted.
         * Returns: IActionResult: A JSON result indicating the outcome of the operation.
         * 
         * Process:
         *    1. Checks for the session token in the cookies. If absent, returns a "no_auth" result.
         *    2. Validates the session token. If invalid, returns a "no_auth" result.
         *    3. Refreshes the cookie expiration.
         *    4. Checks if the user is authorized to delete entries in the air conditioning records. If not, returns a "no_permission" result.
         *    5. Logs the operation of deleting an existing entry.
         *    6. Calls the airConditioningController.DeleteEntry method to remove the entry from the database based on the provided ID.
         *    7. Captures the result of the operation and constructs a JSON response.
         *    8. Catches any MySqlException, logs the exception, and returns an error result.
         */
        [HttpDelete("entry")]
        public IActionResult DeleteAirConditioningEntry(int id = -1)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Validate session token
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Refresh the cookie expiration
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "airConditioningReports", "delete"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Usunięcie wpisu ewidencji środków klimatyzacji");

                string result = airConditioningController.DeleteEntry(id);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "AirConditioningController", "DeleteAirConditioningEntry", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /*
         * Method: DeleteAirConditioningEntries
         * Description: This method handles the HTTP DELETE request to delete multiple entries from the air conditioning records.
         *              It performs authentication and authorization checks, validates and refreshes session tokens, and logs the operation.
         * Parameters:
         *    - string idsList: A comma-separated list of IDs of entries to be deleted.
         * Returns: IActionResult: A JSON result indicating the outcome of the operation.
         * 
         * Process:
         *    1. Checks for the session token in the cookies. If absent, returns a "no_auth" result.
         *    2. Validates the session token. If invalid, returns a "no_auth" result.
         *    3. Refreshes the cookie expiration.
         *    4. Checks if the user is authorized to delete entries in the air conditioning records. If not, returns a "no_permission" result.
         *    5. Logs the operation of deleting multiple entries.
         *    6. Converts the comma-separated list of IDs into a list of integers.
         *    7. Calls the airConditioningController.DeleteEntries method to remove the entries from the database based on the provided IDs.
         *    8. Captures the result of the operation and constructs a JSON response.
         *    9. Catches any MySqlException, logs the exception, and returns an error result.
         */
        [HttpDelete("entries")]
        public IActionResult DeleteAirConditioningEntries(string idsList)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Validate session token
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Refresh the cookie expiration
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "airConditioningReports", "delete"))
                return new JsonResult(new { result = "no_permission" });

            List<int> ids  = (!string.IsNullOrEmpty(idsList)) ? idsList.Split(',').Select(id => Convert.ToInt32(id)).ToList() : new List<int>();

            try
            {
                logsController.AddLog(_cookieValue, "Usunięcie wpisów ewidencji środka klimatyzacji");

                string result = airConditioningController.DeleteEntries(ids);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "AirConditioningController", "DeleteAirConditioningEntries", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }
    }
}
