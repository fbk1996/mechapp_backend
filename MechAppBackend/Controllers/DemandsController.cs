using MechAppBackend.Data;
using MechAppBackend.features;
using MechAppBackend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/demands")]
    [ApiController]
    public class DemandsController : ControllerBase
    {
        MechAppContext _context;
        logs logsController;
        CheckCookieToken cookieToken;
        CheckRoles roles;
        demands demandsController;

        public DemandsController(MechAppContext context)
        {
            _context = context;
            logsController = new logs(context);
            roles = new CheckRoles(context);
            cookieToken = new CheckCookieToken(context);
            demandsController = new demands(context);
        }

        /*
         * Method: GetDemands
         * Description: This method handles the HTTP GET request to retrieve a list of demands based on provided filters and pagination parameters.
         *              It performs authentication and authorization checks, validates and refreshes session tokens, and logs the operation.
         * Parameters:
         *    - DateTime? dateFrom: The start date for filtering demands.
         *    - DateTime? dateTo: The end date for filtering demands.
         *    - string? status: A comma-separated list of statuses to filter demands.
         *    - string? departmentIds: A comma-separated list of department IDs to filter demands.
         *    - int currentPage: The current page number for pagination.
         *    - int pageSize: The number of demands to display per page.
         * Returns: IActionResult: A JSON result indicating the outcome of the operation along with the list of demands.
         * 
         * Process:
         *    1. Checks for the session token in the cookies. If absent, returns a "no_auth" result.
         *    2. Validates the session token. If invalid, returns a "no_auth" result.
         *    3. Refreshes the cookie expiration.
         *    4. Checks if the user is authorized to view demands. If not, returns a "no_permission" result.
         *    5. Logs the operation of retrieving the list of demands.
         *    6. Parses the status and departmentIds into lists of integers.
         *    7. Calculates the offset for pagination.
         *    8. Calls the demandsController.GetDemands method to retrieve the demands based on the provided filters and pagination parameters.
         *    9. Returns a JSON response with the result status and the list of demands.
         *   10. Catches any MySqlException, logs the exception, and returns an error result.
         */
        [HttpGet]
        public IActionResult GetDemands(DateTime? dateFrom, DateTime? dateTo, string? status, string? departmentIds, int currentPage, int pageSize)
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

            if (!roles.isAuthorized(_cookieValue, "demands", "view"))
                return new JsonResult(new { result = "no_permission" });

            int offset = ((currentPage - 1) * pageSize);

            List<int> statuses = (!string.IsNullOrEmpty(status)) ? status.Split(',').Select(id => Convert.ToInt32(id)).ToList() : new List<int>();
            List<int> depIds = (!string.IsNullOrEmpty(departmentIds)) ? departmentIds.Split(',').Select(id => Convert.ToInt32(id)).ToList(): new List<int>();

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie listy zapotrzebowań");

                var demands = demandsController.GetDemands(dateFrom, dateTo, statuses, depIds, pageSize, offset);

                return new JsonResult(new
                {
                    result = "done",
                    demands = demands
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DemandsController", "GetDemands", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /**
         * Method: GetDemandDetails
         * Description: Handles the HTTP GET request to fetch the details of a specific demand based on its ID.
         *              Performs authentication and authorization checks, validates and refreshes session tokens, 
         *              and logs the operation.
         * Parameters:
         *    - int id: The ID of the demand to fetch details for. Defaults to -1 if not provided.
         * Returns: IActionResult: A JSON result indicating the outcome of the operation.
         * 
         * Process:
         *    1. Checks for the session token in the cookies. If absent, returns a "no_auth" result.
         *    2. Validates the session token. If invalid, returns a "no_auth" result.
         *    3. Refreshes the cookie expiration.
         *    4. Checks if the user is authorized to view demands. If not, returns a "no_permission" result.
         *    5. Logs the operation of fetching demand details.
         *    6. Calls the demandsController.GetDemandDetails method to fetch the details of the demand.
         *    7. Checks if the fetched demand has a valid ID. If invalid, returns an "error" result.
         *    8. Constructs a JSON response with the demand details and returns it.
         *    9. Catches any MySqlException, logs the exception, and returns an error result.
         */
        [HttpGet("details")]
        public IActionResult GetDemandDetails(int id = -1)
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

            if (!roles.isAuthorized(_cookieValue, "demands", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie szczegółów zapotrzebowania");

                var demand = demandsController.GetDemandDetails(id);

                if (demand.id == -1)
                    return new JsonResult(new { result = "error" });

                return new JsonResult(new
                {
                    result = "done",
                    demand = demand
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DemandsController", "GetDemandDetails", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Method: SearchInWarehouse
        /// Description: Handles the HTTP GET request to search for items in the warehouse based on EAN code and department ID.
        ///              It performs authentication and authorization checks, validates and refreshes session tokens, and logs the operation.
        /// Parameters:
        ///    - ean (string, optional): The EAN code of the item to search for. If null or empty, search is based only on department ID.
        ///    - departmentId (int, optional): The ID of the department to search in. Defaults to -1.
        /// Returns: IActionResult: A JSON result indicating the outcome of the search operation, including the list of found items.
        /// 
        /// Process:
        ///    1. Checks for the session token in the cookies. If absent, returns a "no_auth" result.
        ///    2. Validates the session token. If invalid, returns a "no_auth" result.
        ///    3. Refreshes the cookie expiration.
        ///    4. Checks if the user is authorized to view demands. If not, returns a "no_permission" result.
        ///    5. Logs the search operation.
        ///    6. Calls the demandsController.SearchItemsInWarehouse method to search for items based on provided criteria.
        ///    7. Captures the result of the search operation and constructs a JSON response.
        ///    8. Catches any MySqlException, logs the exception, and returns an error result.
        /// </summary>
        [HttpGet("search")]
        public IActionResult SearchInWarehouse(string? ean, int departmentId = -1)
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

            if (!roles.isAuthorized(_cookieValue, "demands", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Poszukanie zasobu w magazynie");

                var searchItems = demandsController.SearchItemsInWarehouse(ean, departmentId);

                return new JsonResult(new
                {
                    result = "done",
                    search = searchItems
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DemandsController", "SearchInWarehouse", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString()});
        }

        /*
         * Method: AddDemand
         * Description: This method handles the HTTP POST request to add a new demand entry.
         * It performs authentication and authorization checks, validates and refreshes session tokens, logs the operation, and adds the new demand entry.
         * Parameters:
         * AddEditDemandOb demand: The data structure containing the details of the demand to be added.
         * Returns: IActionResult: A JSON result indicating the outcome of the operation.
         * Process:
         * Checks for the session token in the cookies. If absent, returns a "no_auth" result.
         * Validates the session token. If invalid, returns a "no_auth" result.
         * Refreshes the cookie expiration.
         * Checks if the user is authorized to add demands. If not, returns a "no_permission" result.
         * Logs the operation of adding a new demand.
         * Calls the demandsController.AddDemand method to add the demand based on the provided data.
         * Captures the result of the operation and constructs a JSON response.
         * Catches any MySqlException, logs the exception, and returns an error result.
         */
        [HttpPost]
        public IActionResult AddDemand([FromBody]AddEditDemandOb demand)
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

            if (!roles.isAuthorized(_cookieValue, "demands", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Dodanie nowego zapotrzebowania");

                string result = demandsController.AddDemand(demand);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DemandsController", "AddDemand", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Edits an existing demand in the system.
        /// </summary>
        /// <param name="demand">The demand object containing the updated details.</param>
        /// <returns>
        /// A JSON result indicating the outcome of the operation.
        /// Possible results include:
        /// - "demand_edited" if the demand is successfully edited.
        /// - "no_items" if the demand has no items.
        /// - "no_auth" if the user is not authenticated.
        /// - "no_permission" if the user lacks the necessary permissions.
        /// - "error" if an error occurs during the operation.
        /// </returns>
        /// <remarks>
        /// This endpoint requires a valid session token and appropriate permissions.
        /// </remarks>
        [HttpPut]
        public IActionResult EditDemand([FromBody]AddEditDemandOb demand)
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

            if (!roles.isAuthorized(_cookieValue, "demands", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Edycja zapotrzebowania");

                string result = demandsController.EditDemand(demand);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DemandsController", "EditDemand", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Changes the status of a demand in the system.
        /// </summary>
        /// <param name="status">The status object containing the demand ID and the new status value.</param>
        /// <returns>
        /// A JSON result indicating the outcome of the operation.
        /// Possible results include:
        /// - "status_changed" if the demand status is successfully updated.
        /// - "no_demand" if the demand with the specified ID does not exist.
        /// - "no_status" if the status value is missing or invalid.
        /// - "no_auth" if the user is not authenticated.
        /// - "no_permission" if the user lacks the necessary permissions.
        /// - "error" if an error occurs during the operation.
        /// </returns>
        /// <remarks>
        /// This endpoint requires a valid session token and appropriate permissions.
        /// </remarks>
        [HttpPut("status")]
        public IActionResult ChangeDemandStatus([FromBody]editDemandStatus status)
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

            if (!roles.isAuthorized(_cookieValue, "demands", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Zmiana statusu zapotrzebowania");

                string result = demandsController.ChangeDemandStatus(status);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DemandsController", "ChangeDemandStatus", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// Deletes a demand from the system based on the provided demand ID.
        /// </summary>
        /// <param name="id">The ID of the demand to be deleted.</param>
        /// <returns>
        /// A JSON result indicating the outcome of the operation.
        /// Possible results include:
        /// - "demand_deleted" if the demand is successfully deleted.
        /// - "no_auth" if the user is not authenticated.
        /// - "no_permission" if the user lacks the necessary permissions.
        /// - "error" if an error occurs during the operation.
        /// </returns>
        /// <remarks>
        /// This endpoint requires a valid session token and appropriate permissions.
        /// </remarks>
        [HttpDelete("demand")]
        public IActionResult DeleteDemand(int id = -1)
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

            if (!roles.isAuthorized(_cookieValue, "demands", "delete"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Usunięcie zapotrzebowania");

                string result = demandsController.DeleteDemand(id);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear() .Append("error");
                Logger.SendException("MechApp", "DemandsController", "DeleteDemand", ex);
            }

            return new JsonResult(new {result = resultBuilder .ToString() });
        }

        /// <summary>
        /// Deletes multiple demands from the system based on the provided list of demand IDs.
        /// </summary>
        /// <param name="idsList">Comma-separated list of demand IDs to be deleted.</param>
        /// <returns>
        /// A JSON result indicating the outcome of the operation.
        /// Possible results include:
        /// - "demands_deleted" if all demands are successfully deleted.
        /// - "no_auth" if the user is not authenticated.
        /// - "no_permission" if the user lacks the necessary permissions.
        /// - "error" if an error occurs during the operation.
        /// </returns>
        /// <remarks>
        /// This endpoint requires a valid session token and appropriate permissions.
        /// </remarks>
        [HttpDelete("demands")]
        public IActionResult DeleteDemands(string idsList)
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

            if (!roles.isAuthorized(_cookieValue, "demands", "delete"))
                return new JsonResult(new { result = "no_permission" });

            List<int> ids = (!string.IsNullOrEmpty(idsList)) ? idsList.Split(',').Select(id =>  Convert.ToInt32(id)).ToList() : new List<int>();

            try
            {
                logsController.AddLog(_cookieValue, "Usunięcie wybranych zapotrzebowań");

                string result = demandsController.DeleteDemands(ids);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DemandsController", "DeleteDemands", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString() });
        }
    }
}
