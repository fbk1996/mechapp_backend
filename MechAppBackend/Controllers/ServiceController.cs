using MechAppBackend.Data;
using MechAppBackend.features;
using MechAppBackend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/services")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        MechAppContext _context;
        services serviceController;
        CheckCookieToken cookieToken;

        public ServiceController(MechAppContext context)
        {
            _context = context;
            cookieToken = new CheckCookieToken(context);
            serviceController = new services(context);
        }

        /// <summary>
        /// HTTP GET endpoint for getting all services.
        /// </summary>
        /// <returns>JSON result containing the result status and services</returns>
        [HttpGet]
        public IActionResult GetServices()
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
                // Get all services
                var services = serviceController.GetServices();
                // Return the services in a JSON result
                return new JsonResult(new
                {
                    result = "done",
                    services = services
                });
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error result
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ServiceController", "GetServices", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP GET endpoint for getting the details of a specific service.
        /// </summary>
        /// <param name="id">The ID of the service to be retrieved.</param>
        /// <returns>JSON result containing the result status and service details</returns>
        [HttpGet("details/{id}")]
        public IActionResult GetServiceDetail(int id)
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
                //get service details
                var service = serviceController.GetServiceById(id);

                if (service.id == -1)
                    return new JsonResult(new { result = "error" });
                //return service ob
                return new JsonResult(new
                {
                    result = "done",
                    service = service
                });
            }
            catch (MySqlException ex)
            {
                //log error and return error message
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ServiceController", "GetServiceDetail", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP POST endpoint for adding a new service.
        /// </summary>
        /// <param name="service">The object containing the name, duration, price, and active status of the service to be added.</param>
        /// <returns>JSON result containing the result status</returns>
        [HttpPost("add")]
        public IActionResult AddService(addServiceOb service)
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
                //attempt to add service
                string result = serviceController.AddService(service);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                //log error and return error message
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ServiceController", "AddService", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString()});
        }

        /// <summary>
        /// HTTP POST endpoint for editing an existing service.
        /// </summary>
        /// <param name="service">The object containing the ID, name, duration, price, and active status of the service to be edited.</param>
        /// <returns>JSON result containing the result status</returns>
        [HttpPost("edit")]
        public IActionResult EditService(serviceOb service)
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
                // Attempt to edit the service
                string result = serviceController.EditService(service);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error result
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ServiceController", "EditService", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString()});
        }

        /// <summary>
        /// HTTP DELETE endpoint for deleting a specific service.
        /// </summary>
        /// <param name="id">The ID of the service to be deleted.</param>
        /// <returns>JSON result containing the result status</returns>
        [HttpDelete("delete/service/{id}")]
        public IActionResult DeleteService(int id)
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
                // Attempt to delete the service
                string result = serviceController.DeleteService(id);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error result
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ServiceController", "DeleteService", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP DELETE endpoint for deleting multiple services.
        /// </summary>
        /// <param name="idsList">A comma-separated list of IDs of the services to be deleted.</param>
        /// <returns>JSON result containing the result status</returns>
        [HttpDelete("delete/services/{idsList}")]
        public IActionResult DeleteServices(string idsList)
        {
            StringBuilder resultBuilder = new StringBuilder();
            // Convert the comma-separated list of IDs into a list of integers
            List<int> ids = idsList.Split(',').Select(id => Convert.ToInt32(id)).ToList();

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
                // Attempt to delete the services
                string result = serviceController.DeleteServices(ids);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error result
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ServiceController", "DeleteServices", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString()});
        }
    }
}
