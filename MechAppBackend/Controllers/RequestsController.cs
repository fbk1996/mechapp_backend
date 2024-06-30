using MechAppBackend.Data;
using MechAppBackend.features;
using MechAppBackend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/requests")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        MechAppContext _context;
        logs logsController;
        CheckCookieToken cookieToken;
        CheckRoles roles;
        requests requestsController;

        public RequestsController(MechAppContext context)
        {
            _context = context;
            logsController = new logs(context);
            cookieToken = new CheckCookieToken(context);
            roles = new CheckRoles(context);
            requestsController = new requests(context);
        }

        /// <summary>
        /// Retrieves a paginated list of requests based on optional search criteria, page size, and current page number.
        /// This method performs several key operations:
        /// - Checks for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" response.
        /// - Validates the session token. If the validation fails, it returns a "no_auth" response again.
        /// - Refreshes the cookie expiration to extend the user's session by updating the cookie with a new expiration time.
        /// - Checks if the user has the necessary permissions to view requests. If not, it returns a "no_permission" response.
        /// - Calculates the offset for pagination based on the current page number and page size.
        /// - Calls the requestsController to retrieve a list of requests based on the provided search criteria, page size, and offset.
        /// - Returns a JSON result containing the operation outcome ("done" for success or "error" for failure) and the list of requests if successful.
        /// </summary>
        /// <param name="name">Optional search criteria to filter requests by name.</param>
        /// <param name="pageSize">The number of requests to return per page.</param>
        /// <param name="currentPage">The current page number for pagination.</param>
        /// <returns>A JSON result indicating the outcome of the operation and containing the list of requests if successful.</returns>
        [HttpGet]
        public IActionResult GetRequests(string? name, int pageSize, int currentPage)
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

            if (!roles.isAuthorized(_cookieValue, "requests", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                int offset = ((currentPage - 1) * pageSize);

                logsController.AddLog(_cookieValue, "Pobranie listy wniosków urlopowych");

                var requests = requestsController.GetRequests(name, pageSize, offset);

                return new JsonResult(new
                {
                    result = "done",
                    requests = requests
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "RequestsController", "GetRequests", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString()});
        }

        /// <summary>
        /// Retrieves the details of a specific request based on the provided request ID.
        /// This method performs several key operations:
        /// - Checks for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" response.
        /// - Validates the session token. If the validation fails, it returns a "no_auth" response again.
        /// - Refreshes the cookie expiration to extend the user's session by updating the cookie with a new expiration time.
        /// - Checks if the user has the necessary permissions to view request details. If not, it returns a "no_permission" response.
        /// - Attempts to retrieve the details of the request from the database using the requestsController. If the request ID is invalid (-1), it returns an "error" response.
        /// - Logs the action of retrieving request details.
        /// - Returns a JSON result containing the operation outcome ("done" for success or "error" for failure) and the request details if successful.
        /// </summary>
        /// <param name="id">The ID of the request for which details are being retrieved. Default value is -1.</param>
        /// <returns>A JSON result indicating the outcome of the operation and containing the request details if successful.</returns>
        [HttpGet("details")]
        public IActionResult GetRequestDetails(int id = -1)
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

            if (!roles.isAuthorized(_cookieValue, "clients", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie szczegółów wniosku urlopowego");

                var request = requestsController.GetRequestsDetails(id);

                if (request.id == -1)
                    return new JsonResult(new { result = "error" });

                return new JsonResult(new
                {
                    result = "done",
                    request = request
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "RequestsController", "GetRequestDetails", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Retrieves the printing details of a specific request based on the provided request ID.
        /// This method performs several key operations:
        /// - Checks for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" response.
        /// - Validates the session token. If the validation fails, it returns a "no_auth" response again.
        /// - Refreshes the cookie expiration to extend the user's session by updating the cookie with a new expiration time.
        /// - Checks if the user has the necessary permissions to view printing details of the request. If not, it returns a "no_permission" response.
        /// - Attempts to retrieve the printing details of the request from the database using the requestsController. If the request ID is invalid (-1), it returns an "error" response.
        /// - Logs the action of retrieving printing details of the request.
        /// - Returns a JSON result containing the operation outcome ("done" for success or "error" for failure) and the printing details of the request if successful.
        /// </summary>
        /// <param name="id">The ID of the request for which printing details are being retrieved. Default value is -1.</param>
        /// <returns>A JSON result indicating the outcome of the operation and containing the printing details of the request if successful.</returns>
        [HttpGet("print")]
        public IActionResult GetPrintDetails(int id = -1)
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

            if (!roles.isAuthorized(_cookieValue, "clients", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie danych do wydrukowania wniosku urlopowego");

                var request = requestsController.GetRequestPrint(id);

                if (request.id == -1)
                    return new JsonResult(new { result = "error" });

                return new JsonResult(new
                {
                    result = "done",
                    request = request
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "RequestsController", "GetPrintDetails", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Adds a new request based on the provided request object.
        /// This method performs several key operations:
        /// - Checks for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" response.
        /// - Validates the session token. If the validation fails, it returns a "no_auth" response again.
        /// - Refreshes the cookie expiration to extend the user's session by updating the cookie with a new expiration time.
        /// - Checks if the user has the necessary permissions to add a new request. If not, it returns a "no_permission" response.
        /// - Attempts to add the new request to the database using the requestsController. If successful, logs the action of adding a new request.
        /// - Returns a JSON result containing the operation outcome ("done" for success, "error" for failure).
        /// </summary>
        /// <param name="request">The request object containing the details of the new request to be added.</param>
        /// <returns>A JSON result indicating the outcome of the operation.</returns>
        [HttpPost]
        public IActionResult AddRequest([FromBody]AddRequestOb request)
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

            if (!roles.isAuthorized(_cookieValue, "clients", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Dodanie wniosku urlopowego");

                string result = requestsController.AddRequest(request);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "RequestsController", "AddRequest", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// Adds files to a specific request based on the provided request object.
        /// This method performs several key operations:
        /// - Checks for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" response.
        /// - Validates the session token. If the validation fails, it returns a "no_auth" response again.
        /// - Refreshes the cookie expiration to extend the user's session by updating the cookie with a new expiration time.
        /// - Checks if the user has the necessary permissions to add files to a request. If not, it returns a "no_permission" response.
        /// - Attempts to add the files to the request in the database using the requestsController. If successful, logs the action of adding files to the request.
        /// - Returns a JSON result containing the operation outcome ("done" for success, "error" for failure).
        /// </summary>
        /// <param name="request">The request object containing the details of the request to which files are being added.</param>
        /// <returns>A JSON result indicating the outcome of the operation.</returns>
        [HttpPost("files")]
        public IActionResult AddFilesToRequest([FromBody]EditRequestOb request)
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

            if (!roles.isAuthorized(_cookieValue, "clients", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Dodanie plików do wniosku urlopowego");

                string result = requestsController.AddFilesToRequests(request);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "RequestsController", "AddFilesToRequest", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString() } );
        }

        /// <summary>
        /// Changes the status of a specific request based on the provided status object.
        /// This method performs several key operations:
        /// - Checks for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" response.
        /// - Validates the session token. If the validation fails, it returns a "no_auth" response again.
        /// - Refreshes the cookie expiration to extend the user's session by updating the cookie with a new expiration time.
        /// - Retrieves the request from the database using the provided request ID. If the request is not found, it returns an "error" response.
        /// - Checks if the user has the necessary permissions to change the request status. If not, it returns a "no_permission" response.
        /// - Prevents users from setting a decision on their own requests to avoid conflicts of interest.
        /// - Logs the action of changing the request status.
        /// - Attempts to change the status of the request in the database using the requestsController. If successful, returns the operation outcome ("done" for success, "error" for failure).
        /// </summary>
        /// <param name="status">The status object containing the ID of the request and the new status to be set.</param>
        /// <returns>A JSON result indicating the outcome of the operation.</returns>
        [HttpPut]
        public IActionResult ChangeRequestStatus([FromBody]ChangeRequestStatusOb status)
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

            try
            {
                var request = _context.AbsenceRequests.FirstOrDefault(r => r.Id == status.id);

                if (request == null)
                    return new JsonResult(new { result = "error" });

                var user = _context.UsersTokens.FirstOrDefault(st => st.Token == _cookieValue);

                if (user == null)
                    return new JsonResult(new { result = "error" });

                if (!roles.isAuthorized(_cookieValue, "clients", "edit") && request.UserId != user.UserId)
                    return new JsonResult(new { result = "no_permission" });

                if (request.Status == 1 && request.UserId == user.UserId || request.Status == 2 && request.UserId == user.UserId)
                    return new JsonResult(new { result = "decision_set" });

                if (status.status == 1 && request.UserId == user.UserId)
                    return new JsonResult(new { result = "can_not_accept_own_request" });

                logsController.AddLog(_cookieValue, "Zmiana statusu wniosku urlopowego");

                string result = requestsController.ChangeOrderStatus(status);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "RequestsController", "ChangeRequestStatus", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Deletes a file from a specific request based on the provided file ID.
        /// This method performs several key operations:
        /// - Checks for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" response.
        /// - Validates the session token. If the validation fails, it returns a "no_auth" response again.
        /// - Refreshes the cookie expiration to extend the user's session by updating the cookie with a new expiration time.
        /// - Checks if the user has the necessary permissions to delete a file from a request. If not, it returns a "no_permission" response.
        /// - Attempts to delete the file from the request in the database using the requestsController. If successful, logs the action of deleting a file from the request.
        /// - Returns a JSON result containing the operation outcome ("done" for success, "error" for failure).
        /// </summary>
        /// <param name="id">The ID of the file to be deleted. Default value is -1.</param>
        /// <returns>A JSON result indicating the outcome of the operation.</returns>
        [HttpDelete("files")]
        public IActionResult DeleteFile(int id = -1)
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

            if (!roles.isAuthorized(_cookieValue, "clients", "delete"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Usunięcie pliku z wniosku urlopowego");

                string result = requestsController.DeleteFile(id);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "RequestsController", "DeleteFile", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }
    }
}
