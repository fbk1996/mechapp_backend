using MechAppBackend.Data;
using MechAppBackend.features;
using MechAppBackend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        MechAppContext _context;
        logs logsController;
        CheckCookieToken cookieToken;
        CheckRoles roles;
        orders ordersController;
        vehicles vehiclesController;
        clients clientsController;

        public OrdersController(MechAppContext context)
        {
            _context = context;
            logsController = new logs(context);
            cookieToken = new CheckCookieToken(context);
            roles = new CheckRoles(context);
            ordersController = new orders(context);
            vehiclesController = new vehicles(context);
            clientsController = new clients(context);
        }

        /// <summary>
        /// HTTP GET endpoint for retrieving a list of orders within a specified date range, optionally filtered by name and department IDs.
        /// </summary>
        /// <param name="from">The start date of the date range for which orders are to be retrieved.</param>
        /// <param name="to">The end date of the date range for which orders are to be retrieved.</param>
        /// <param name="name">Optional parameter for filtering orders by name.</param>
        /// <param name="departmentIds">Optional comma-separated list of department IDs for filtering orders.</param>
        /// <param name="pageSize">The number of orders to retrieve per page.</param>
        /// <param name="currentPage">The current page number for pagination.</param>
        /// <returns>JSON result containing the operation result status and the list of orders.</returns>
        [HttpGet]
        public IActionResult GetOrders(DateTime from, DateTime to, string? name, string? departmentIds, int pageSize, int currentPage)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "view"))
                return new JsonResult(new { result = "no_permission" });

            List<long> depIds = !string.IsNullOrEmpty(departmentIds) ? departmentIds.Split(',').Select(id => Convert.ToInt64(id)).ToList() : new List<long>();

            int offset = ((currentPage - 1) * pageSize);

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie listy zleceń");

                var orders = ordersController.getOrders(from, to, name, depIds, pageSize, offset);

                return new JsonResult(new
                {
                    result = "done",
                    orders = orders
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "GetOrders", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP GET endpoint for retrieving the details of a specific order identified by its ID.
        /// </summary>
        /// <param name="id">The ID of the order for which details are requested.</param>
        /// <returns>JSON result containing the operation result status and the order details if successful.</returns>
        [HttpGet("details")]
        public IActionResult GetOrderDetails(int id)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                var order = ordersController.GetOrderDetail(id);

                if (order.id == -1)
                    return new JsonResult(new { result = "error" });

                DateTime orderStartDate = (DateTime)order.startDate;

                logsController.AddLog(_cookieValue, $"Pobranie szczegółów zlecenia: {order.id}/{orderStartDate.ToString("yyyy")}");

                return new JsonResult(new
                {
                    result = "done",
                    order = order
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "GetOrderDetails", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP GET endpoint for searching parts for an estimate based on optional criteria such as part ID, EAN, name, and department ID.
        /// </summary>
        /// <param name="id">Optional parameter for filtering parts by their ID.</param>
        /// <param name="ean">Optional parameter for filtering parts by their EAN code.</param>
        /// <param name="name">Optional parameter for filtering parts by their name.</param>
        /// <param name="departmentID">Required parameter for specifying the department ID to filter parts.</param>
        /// <returns>JSON result containing the operation result status and a list of parts matching the search criteria.</returns>
        [HttpGet("estimate/search/parts")]
        public IActionResult SearchEstimateParts(int? id, string? ean, string? name, int departmentID)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie listy wyszukiwanych części");
                var parts = ordersController.searchParts(id, ean, name, departmentID);

                return new JsonResult(new
                {
                    result = "done",
                    parts = parts
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "SearchEstimateParts", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP GET endpoint for searching services for an estimate based on optional criteria such as service ID and name.
        /// </summary>
        /// <param name="id">Optional parameter for filtering services by their ID.</param>
        /// <param name="name">Optional parameter for filtering services by their name.</param>
        /// <returns>JSON result containing the operation result status and a list of services matching the search criteria.</returns>
        [HttpGet("estimate/search/services")]
        public IActionResult SearchEstimateServices(int? id, string? name)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "view") && !roles.isAuthorized(_cookieValue, "services", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie listy wyszukiwanych usług");

                var services = ordersController.searchServices(id, name);

                return new JsonResult(new
                {
                    result = "done",
                    services = services
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "SearchEstimateServices", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP GET endpoint for searching users based on an optional name parameter. This method is typically used when creating an order to find users.
        /// </summary>
        /// <param name="name">Optional parameter for filtering users by name. If not provided, all users may be returned.</param>
        /// <returns>JSON result containing the operation result status and a list of users matching the search criteria.</returns>
        [HttpGet("search/users")]
        public IActionResult OrderSearchUsers(string? name)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Wyszukanie użytkowników przy tworzeniu zlecenia");

                var users = ordersController.searchUsers(name);

                return new JsonResult(new
                {
                    result = "done",
                    users = users
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "OrdersSearchUsers", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString()});
        }

        /// <summary>
        /// HTTP GET endpoint for retrieving complaints associated with a specific order identified by its ID.
        /// This method checks for a valid session token in cookies, validates it, and refreshes the cookie expiration.
        /// It also checks if the user has the necessary permissions to view complaints.
        /// If the orderId is not provided or is invalid, it returns an error.
        /// On success, it returns the complaint details for the specified order.
        /// </summary>
        /// <param name="orderId">The ID of the order for which complaints are requested. Default value is -1.</param>
        /// <returns>JSON result containing the operation result status and complaint details if successful, or an error message.</returns>
        [HttpGet("complaints")]
        public IActionResult GetOrderComplaint(int orderId = -1)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "view"))
                return new JsonResult(new { result = "no_permission" });

            if (orderId == -1)
                return new JsonResult(new { result = "error" });

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie reklamacji do zlecenia");

                var complaint = ordersController.GetComplaint(orderId);

                if (complaint.id == -1)
                    return new JsonResult(new { result = "error" });

                return new JsonResult(new
                {
                    result = "done",
                    complaint = complaint
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "GetOrderComplaints", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP GET endpoint for retrieving a list of vehicles owned by a specific user, identified by their user ID.
        /// This method performs several checks including session token validation, cookie expiration refresh, and user authorization.
        /// If the user ID is not provided or is invalid, it returns an error.
        /// On success, it returns a list of vehicles owned by the user.
        /// </summary>
        /// <param name="id">The ID of the user whose vehicles are being requested. Default value is -1.</param>
        /// <returns>JSON result containing the operation result status and a list of vehicles if successful, or an error message.</returns>
        [HttpGet("vehicles")]
        public IActionResult GetUserVehicles(int id = -1)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "view"))
                return new JsonResult(new { result = "no_permission" });

            if (id == -1)
                return new JsonResult(new { result = "error" });

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie listy pojazdów klienta!");

                var vehicles = _context.UsersVehicles
                    .Where(v => v.Owner == id)
                    .Select(v => new vehicleOb
                    {
                        id = (int)v.Id,
                        producer = v.Producer,
                        model = v.Model,
                        produceDate = v.ProduceDate,
                        mileage = (int)v.Mileage,
                        vin = v.Vin,
                        engineNumber = v.EngineNumber,
                        registrationNumber = v.RegistrationNumber,
                        enginePower = v.EnginePower,
                        engineCapacity = v.EngineCapacity,
                        fuelType = v.FuelType
                    }).ToList();

                return new JsonResult(new
                {
                    result = "done",
                    vehicles = vehicles
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "GetUserVehicles", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP GET endpoint for retrieving detailed information about a specific vehicle owned by a user, identified by the vehicle's ID.
        /// This method performs several operations including:
        /// - Checking for a session token in cookies and validating it.
        /// - Refreshing the cookie expiration.
        /// - Checking if the user has the necessary permissions to view vehicle details.
        /// - Logging the action of retrieving vehicle details.
        /// If the vehicle ID is not provided or is invalid, or if any checks fail (e.g., authentication, authorization), it returns an appropriate error.
        /// On success, it returns detailed information about the specified vehicle.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle for which details are being requested.</param>
        /// <returns>JSON result containing the operation result status and detailed information about the vehicle if successful, or an error message.</returns>
        [HttpGet("vehicles/details")]
        public IActionResult GetUserVehicleDetails(int vehicleId)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie szczegółów pojazdu klienta!");

                vehicleOb vehicle = (vehicleOb)_context.UsersVehicles
                    .Where(v => v.Id == vehicleId)
                    .Select(v => new vehicleOb
                    {
                        id = (int)v.Id,
                        producer = v.Producer,
                        model = v.Model,
                        produceDate = v.ProduceDate,
                        mileage = (int)v.Mileage,
                        vin = v.Vin,
                        engineNumber = v.EngineNumber,
                        registrationNumber = v.RegistrationNumber,
                        enginePower = v.EnginePower,
                        engineCapacity = v.EngineCapacity,
                        fuelType = v.FuelType
                    }).FirstOrDefault();

                if (vehicle == null)
                    return new JsonResult(new { result = "error" });

                return new JsonResult(new
                {
                    result = "done",
                    vehicle = vehicle
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "GetUserVehicleDetails", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString()});
        }

        /// <summary>
        /// HTTP GET endpoint for retrieving detailed information about a specific client identified by their client ID.
        /// This method performs several operations including:
        /// - Checking for a session token in cookies and validating it.
        /// - Refreshing the cookie expiration.
        /// - Checking if the user has the necessary permissions to view client details.
        /// - Logging the action of retrieving client details.
        /// If the client ID is not provided or is invalid, or if any checks fail (e.g., authentication, authorization), it returns an appropriate error.
        /// On success, it returns detailed information about the specified client.
        /// </summary>
        /// <param name="id">The ID of the client for which details are being requested. Default value is -1.</param>
        /// <returns>JSON result containing the operation result status and detailed information about the client if successful, or an error message.</returns>
        [HttpGet("users/details")]
        public IActionResult GetClientDetails(int id = -1)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "view") && !roles.isAuthorized(_cookieValue, "clients", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie szczegółów klienta");

                var client = clientsController.GetClientDetails(id);

                if (client.id == -1)
                    return new JsonResult(new { result = "error" });

                return new JsonResult(new
                {
                    result = "done",
                    client = client
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "GetClientDetails", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP POST endpoint for adding a new order to the system. This method performs several operations including:
        /// - Checking for a session token in cookies and validating it.
        /// - Refreshing the cookie expiration.
        /// - Checking if the user has the necessary permissions to add an order.
        /// - Logging the action of adding a new order.
        /// - Attempting to add the order through the orders controller.
        /// If the session token is missing or invalid, or if the user lacks permission, it returns an appropriate error.
        /// On success, it returns the result of the order addition attempt.
        /// In case of a database error, it logs the exception and returns an error message.
        /// </summary>
        /// <param name="order">The order object to be added, received in the request body.</param>
        /// <returns>JSON result containing the operation result status.</returns>
        [HttpPost("add")]
        public IActionResult AddOrder([FromBody]addOrderOb order)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Dodanie nowego zlecenia!");

                string result = ordersController.AddOrder(order);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "AddOrder", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString()});
        }

        /// <summary>
        /// HTTP POST endpoint for adding an estimate to an order. This method performs several operations including:
        /// - Checking for a session token in cookies and validating it.
        /// - Refreshing the cookie expiration.
        /// - Checking if the user has the necessary permissions to add an estimate.
        /// - Logging the action of adding an estimate to an order.
        /// If the session token is missing or invalid, or if the user lacks permission, it returns an appropriate error.
        /// On success, it attempts to add the estimate through the orders controller and returns the result of the operation.
        /// In case of a database error, it logs the exception and returns an error message.
        /// </summary>
        /// <param name="estimate">The estimate object to be added, received in the request body.</param>
        /// <returns>JSON result containing the operation result status.</returns>
        [HttpPost("estimate")]
        public IActionResult AddEstimate([FromBody] addEditEstimateOb estimate)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Dodanie kosztorysu do zlecenia");

                string result = ordersController.AddEstimate(estimate);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "AddEstimate", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP POST endpoint for adding a checklist to an order. This method performs several operations including:
        /// - Checking for a session token in cookies and validating it.
        /// - Refreshing the cookie expiration.
        /// - Checking if the user has the necessary permissions to add a checklist.
        /// - Logging the action of adding a checklist to an order.
        /// If the session token is missing or invalid, or if the user lacks permission, it returns an appropriate error.
        /// On success, it attempts to add the checklist through the orders controller and returns the result of the operation.
        /// In case of a database error, it logs the exception and returns an error message.
        /// </summary>
        /// <param name="checklist">The checklist object to be added, received in the request body.</param>
        /// <returns>JSON result containing the operation result status.</returns>
        [HttpPost("checklist")]
        public IActionResult AddChecklist([FromBody] addEditChecklistOb checklist)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Dodanie checklisty do zlecenia");

                string result = ordersController.AddChecklist(checklist);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "AddChecklist", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP POST endpoint for adding a complaint to an order. This method performs several operations including:
        /// - Checking for a session token in cookies. If the token is missing, it returns a "no_auth" result.
        /// - Validating the session token. If the validation fails, it returns a "no_auth" result.
        /// - Refreshing the cookie expiration to extend the user's session.
        /// - Checking if the user has the necessary permissions to add a complaint. If not, it returns a "no_permission" result.
        /// - Attempting to add the complaint to the database. If successful, it logs the action and returns the result of the operation.
        /// - Catching any database errors (e.g., from MySQL) during the operation, logging the exception, and returning an "error" result.
        /// </summary>
        /// <param name="complaint">The complaint object to be added, received in the request body.</param>
        /// <returns>JSON result containing the operation result status. Possible results include "no_auth", "no_permission", or "error".</returns>
        [HttpPost("complaint")]
        public IActionResult AddComplaint([FromBody]addOrderComplaintOb complaint)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Dodanie reklamacji do zlecenia!");

                string result = ordersController.AddComplaint(complaint);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "AddComplaint", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP POST endpoint for adding a new client to the system. This method performs several operations including:
        /// - Checking for a session token in cookies. If the token is missing, it returns a "no_auth" result.
        /// - Validating the session token. If the validation fails, it returns a "no_auth" result again.
        /// - Refreshing the cookie expiration to extend the user's session.
        /// - Checking if the user has the necessary permissions to add a client. If not, it returns a "no_permission" result.
        /// - Attempting to add the client to the database. If successful, it logs the action and returns the result of the operation.
        /// - Catching any database errors (e.g., from MySQL) during the operation, logging the exception, and returning an "error" result.
        /// </summary>
        /// <param name="client">The client object to be added, received in the request body.</param>
        /// <returns>JSON result containing the operation result status. Possible results include "no_auth", "no_permission", or "error".</returns>
        [HttpPost("users")]
        public IActionResult AddClient([FromBody] AddEditClientOb client)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "add") && !roles.isAuthorized(_cookieValue, "clients", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Dodanie nowego klienta");

                string result = clientsController.AddClient(client);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "AddClient", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP POST endpoint for adding a new vehicle to the system. This method performs several operations including:
        /// - Checking for a session token in cookies. If the token is missing, it returns a "no_auth" result.
        /// - Validating the session token. If the validation fails, it returns a "no_auth" result again.
        /// - Refreshing the cookie expiration to extend the user's session.
        /// - Checking if the user has the necessary permissions to add a vehicle. If not, it returns a "no_permission" result.
        /// - Attempting to add the vehicle to the database. If successful, it logs the action and returns the result of the operation.
        /// - Catching any database errors (e.g., from MySQL) during the operation, logging the exception, and returning an "error" result.
        /// </summary>
        /// <param name="vehicle">The vehicle object to be added, received in the request body.</param>
        /// <returns>JSON result containing the operation result status. Possible results include "no_auth", "no_permission", or "error".</returns>
        [HttpPost("vehicles")]
        public IActionResult AddVehicle([FromBody]AddVehicleOb vehicle)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "add") && !roles.isAuthorized(_cookieValue, "clients", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Dodanie nowego pojazdu dla klienta");

                string result = vehiclesController.AddVehicle(vehicle.Producer, vehicle.Model, vehicle.ProduceDate, vehicle.Mileage, vehicle.Vin, vehicle.EngineNumber,
                    vehicle.RegistrationNumber, vehicle.EnginePower, vehicle.EngineCapacity, vehicle.fuelType, vehicle.Owner);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "AddVehicle", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP PUT endpoint for changing the status of an order. This method performs several operations including:
        /// - Checking for a session token in cookies. If the token is missing, it returns a "no_auth" result.
        /// - Validating the session token. If the validation fails, it returns a "no_auth" result again.
        /// - Refreshing the cookie expiration to extend the user's session.
        /// - Checking if the user has the necessary permissions to edit an order status. If not, it returns a "no_permission" result.
        /// - Attempting to change the order status in the database. If successful, it logs the action and returns the result of the operation.
        /// - Catching any database errors (e.g., from MySQL) during the operation, logging the exception, and returning an "error" result.
        /// </summary>
        /// <param name="orderId">The ID of the order whose status is to be changed.</param>
        /// <param name="status">The new status to be applied to the order.</param>
        /// <returns>JSON result containing the operation result status. Possible results include "no_auth", "no_permission", or "error".</returns>
        [HttpPut("status")]
        public IActionResult ChangeOrderStatus(int orderId = - 1, int status = -1)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Zmiana statusu zlecenia!");

                string result = ordersController.ChangeOrderStatus(orderId, status);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "ChangeOrderStatus", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP PUT endpoint for editing an estimate associated with an order. This method performs several operations including:
        /// - Checking for a session token in cookies. If the token is missing, it returns a "no_auth" result.
        /// - Validating the session token. If the validation fails, it returns a "no_auth" result again.
        /// - Refreshing the cookie expiration to extend the user's session.
        /// - Checking if the user has the necessary permissions to edit an estimate. If not, it returns a "no_permission" result.
        /// - Attempting to edit the estimate in the database. If successful, it logs the action and returns the result of the operation.
        /// - Catching any database errors (e.g., from MySQL) during the operation, logging the exception, and returning an "error" result.
        /// </summary>
        /// <param name="estimate">The estimate object to be edited, received in the request body.</param>
        /// <returns>JSON result containing the operation result status. Possible results include "no_auth", "no_permission", or "error".</returns>
        [HttpPut("estimate")]
        public IActionResult EditEstimate([FromBody]addEditEstimateOb estimate)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Edycja kosztorysu do zlecenia");

                string result = ordersController.EditEstimate(estimate);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "EditEstimate", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP PUT endpoint for editing a checklist associated with an order. This method performs several operations including:
        /// - Checking for a session token in cookies. If the token is missing, it returns a "no_auth" result.
        /// - Validating the session token. If the validation fails, it returns a "no_auth" result again.
        /// - Refreshing the cookie expiration to extend the user's session.
        /// - Checking if the user has the necessary permissions to edit a checklist. If not, it returns a "no_permission" result.
        /// - Attempting to edit the checklist in the database. If successful, it logs the action and returns the result of the operation.
        /// - Catching any database errors (e.g., from MySQL) during the operation, logging the exception, and returning an "error" result.
        /// </summary>
        /// <param name="checklist">The checklist object to be edited, received in the request body.</param>
        /// <returns>JSON result containing the operation result status. Possible results include "no_auth", "no_permission", or "error".</returns>
        [HttpPut("checklist")]
        public IActionResult EditChecklist([FromBody] addEditChecklistOb checklist)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Edycja checklisty zlecenia");

                string result = ordersController.EditChecklist(checklist);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "ChangeChecklist", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// Rozpoczyna proces rozpatrywania zgłoszenia reklamacyjnego. Metoda wykonuje kilka operacji, w tym:
        /// - Sprawdza obecność tokenu sesji w ciasteczkach. Jeśli token jest nieobecny, zwraca wynik "no_auth".
        /// - Waliduje token sesji. Jeśli walidacja się nie powiedzie, ponownie zwraca wynik "no_auth".
        /// - Odświeża wygaśnięcie ciasteczka, aby przedłużyć sesję użytkownika.
        /// - Sprawdza, czy użytkownik ma niezbędne uprawnienia do edycji zgłoszeń reklamacyjnych. Jeśli nie, zwraca wynik "no_permission".
        /// - Próbuje rozpocząć proces rozpatrywania reklamacji w bazie danych. W przypadku powodzenia, rejestruje działanie i zwraca wynik operacji.
        /// - Łapie wszelkie błędy bazy danych (np. z MySQL) podczas operacji, rejestruje wyjątek i zwraca wynik "error".
        /// </summary>
        /// <param name="complaintId">ID zgłoszenia reklamacyjnego, które ma zostać rozpatrzone.</param>
        /// <returns>JSON zawierający status wyniku operacji. Możliwe wyniki to "no_auth", "no_permission" lub "error".</returns>
        [HttpPut("complaint/start")]
        public IActionResult StartComplaint(int complaintId = -1)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Rozpoczęcie rozpatrywania zgłoszenia reklamacyjnego");

                string result = ordersController.StartProcessComplaint(complaintId);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "StartComplaint", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP PUT endpoint for resolving complaint submissions. This method performs several operations including:
        /// - Checking for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" result.
        /// - Validating the session token. If the validation fails, it returns a "no_auth" result again.
        /// - Refreshing the cookie expiration to extend the user's session.
        /// - Checking if the user has the necessary permissions to resolve complaint submissions. If not, it returns a "no_permission" result.
        /// - Attempting to finalize the complaint resolution process in the database. If successful, it logs the action and returns the result of the operation.
        /// - Catching any database errors (e.g., from MySQL) during the operation, logging the exception, and returning an "error" result.
        /// </summary>
        /// <param name="complaint">The complaint submission object to be resolved, received in the request body.</param>
        /// <returns>JSON containing the status of the operation result. Possible results include "no_auth", "no_permission", or "error".</returns>
        [HttpPut("complaint/resolve")]
        public IActionResult ResolveComplaint([FromBody]submitOrderComplaintOb complaint)
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

            if (!roles.isAuthorized(_cookieValue, "orders", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Zakończenie rozpatrywania zgłoszenia reklamacji!");

                string result = ordersController.SubmitComplaint(complaint);

                resultBuilder.Append(complaint);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "OrdersController", "ResolveComplaint", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString()});
        }
    }
}
