using MechAppBackend.Data;
using MechAppBackend.features;
using MechAppBackend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        MechAppContext _context;
        logs logsController;
        CheckCookieToken cookieToken;
        CheckRoles roles;
        clients clientsController;
        orders ordersController;
        vehicles vehiclesController;

        public ClientsController (MechAppContext context)
        {
            _context = context;
            logsController = new logs(context);
            cookieToken = new CheckCookieToken(context);
            roles = new CheckRoles(context);
            clientsController = new clients(context);
            ordersController = new orders(context);
            vehiclesController = new vehicles(context);
        }

        /// <summary>
        /// Retrieves a list of clients with optional filtering by name.
        /// Requires authentication and permissions to view clients.
        /// </summary>
        /// <param name="_name">Optional parameter for filtering clients by name.</param>
        /// <param name="pageSize">The size of the result page.</param>
        /// <param name="currentPage">The current page number of the results.</param>
        /// <returns>Json with the operation result and list of clients or an error.</returns>
        [HttpGet]
        public IActionResult GetClients(string? _name, int pageSize, int currentPage)
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

            int offset = ((currentPage - 1) * pageSize);

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie listy użytkowników");

                var clients = clientsController.GetClients(_name, pageSize, offset);

                return new JsonResult(new
                {
                    result = "done",
                    clients = clients
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ClientsController", "GetClients", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString()});
        }
        /// <summary>
        /// Retrieves detailed information about a specific client based on their ID.
        /// This method performs several operations including:
        /// - Checking for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" result.
        /// - Validating the session token. If the validation fails, it returns a "no_auth" result again.
        /// - Refreshing the cookie expiration to extend the user's session.
        /// - Checking if the user has the necessary permissions to view client details. If not, it returns a "no_permission" result.
        /// - Attempting to retrieve detailed client information from the database. If successful, it returns the client details.
        /// - Catching any database errors (e.g., from MySQL) during the operation, logging the exception, and returning an "error" result.
        /// </summary>
        /// <param name="id">The ID of the client for whom details are being retrieved.</param>
        /// <returns>JSON containing the operation result and client details or an error.</returns>
        [HttpGet("details")]
        public IActionResult GetClientDetails(int id)
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
                Logger.SendException("MechApp", "ClientsController", "GetClientDetails", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Retrieves a list of vehicles owned by a specific client identified by their ID.
        /// This method performs several operations including:
        /// - Checking for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" result.
        /// - Validating the session token. If the validation fails, it returns a "no_auth" result again.
        /// - Refreshing the cookie expiration to extend the user's session.
        /// - Checking if the user has the necessary permissions to view client vehicles. If not, it returns a "no_permission" result.
        /// - Attempting to retrieve a list of vehicles from the database based on the client's ID. If successful, it returns the list of vehicles.
        /// - Catching any database errors (e.g., from MySQL) during the operation, logging the exception, and returning an "error" result.
        /// </summary>
        /// <param name="id">The ID of the client whose vehicles are being retrieved.</param>
        /// <returns>JSON containing the operation result and list of vehicles or an error.</returns>
        [HttpGet("vehicles")]
        public IActionResult GetClientVehicles(int id)
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
                logsController.AddLog(_cookieValue, "Pobranie listy pojazdów klienta");

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
                Logger.SendException("MechApp", "ClientsController", "GetClientVehicles", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString()});
        }

        /// <summary>
        /// Retrieves detailed information about a specific vehicle owned by a client, identified by the vehicle's ID.
        /// This method performs several operations including:
        /// - Checking for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" result.
        /// - Validating the session token. If the validation fails, it returns a "no_auth" result again.
        /// - Refreshing the cookie expiration to extend the user's session.
        /// - Checking if the user has the necessary permissions to view vehicle details. If not, it returns a "no_permission" result.
        /// - Attempting to retrieve detailed vehicle information from the database based on the vehicle's ID. If successful, it returns the vehicle details.
        /// - Catching any database errors (e.g., from MySQL) during the operation, logging the exception, and returning an "error" result.
        /// </summary>
        /// <param name="vehicleId">The ID of the vehicle for which details are being retrieved.</param>
        /// <returns>JSON containing the operation result and vehicle details or an error.</returns>
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

            if (!roles.isAuthorized(_cookieValue, "clients", "view"))
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
                Logger.SendException("MechApp", "ClientsController", "GetUserVehicleDetails", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Retrieves a list of orders for a specific client based on the client's ID and the order status.
        /// This method includes several key operations:
        /// - Checking for a session token in cookies. If absent, returns a "no_auth" response.
        /// - Validating the session token. If validation fails, returns a "no_auth" response.
        /// - Refreshing the cookie expiration to maintain the user's session.
        /// - Checking if the user has the necessary permissions to view client orders or order details. If not, returns a "no_permission" response.
        /// - Attempting to retrieve orders matching the specified status for the client. If successful, returns the list of orders.
        /// - Catching and logging any database errors (e.g., from MySQL) that occur during the operation, and returning an "error" response.
        /// </summary>
        /// <param name="_status">The status of the orders to retrieve.</param>
        /// <param name="id">The ID of the client whose orders are being retrieved.</param>
        /// <returns>JSON containing the operation result and list of orders or an error.</returns>
        [HttpGet("orders")]
        public IActionResult GetClientsOrders(int _status, int id)
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

            if (!roles.isAuthorized(_cookieValue, "clients", "view") && !roles.isAuthorized(_cookieValue, "orders", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                //Retrieve orders matching the status for the user
                var orders = ordersController.GetClientOrders(_status, (int)id);

                logsController.AddLog(_cookieValue, $"Pobranie listy zleceń użytkownika");

                return new JsonResult(new
                {
                    result = "done",
                    orders = orders
                });
            }
            catch (MySqlException ex)
            {
                // Log any exceptions that occur during database operations
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ClientsController", "GetClientOrders", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Retrieves detailed information about a specific order based on the order ID.
        /// The method performs the following operations:
        /// - Validates the provided order ID. If the ID is -1, it returns an error.
        /// - Checks for a session token in the request cookies. If absent, returns a "no_auth" response.
        /// - Validates the session token using the CheckCookieToken service. If validation fails, returns a "no_auth" response.
        /// - Refreshes the session token expiration by updating the cookie with a new expiration time.
        /// - Checks if the user has authorization to view client or order details. If not, returns a "no_permission" response.
        /// - Attempts to fetch the order details from the database using the OrdersController. If the order ID is invalid, returns an error.
        /// - Logs the action of retrieving order details using the LogsController.
        /// - Returns the detailed information of the order in a JSON response.
        /// - Catches and logs any MySQL exceptions that occur during database operations, returning an "error" response.
        /// </summary>
        /// <param name="id">The ID of the order for which details are being retrieved. Default value is -1.</param>
        /// <returns>JSON result containing the operation outcome and order details, or an error message.</returns>
        [HttpGet("orders/details")]
        public IActionResult GetOrderDetails(int id = -1)
        {
            StringBuilder resultBuilder = new StringBuilder();
            // Validate the order ID
            if (id == -1)
                return new JsonResult("error");

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Validate session token
            DateTime expireCookie = DateTime.Now.AddHours(2);
            // Refresh the cookie expiration
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "clients", "view") && !roles.isAuthorized(_cookieValue, "orders", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Fetch order details from the controller based on order ID
                var order = ordersController.GetOrderDetail(id);

                if (order.id == -1)
                    return new JsonResult(new { result = "error" });
                DateTime logsOrderDate = (DateTime)order.startDate;
                logsController.AddLog(_cookieValue, $"Pobranie szczegółów zlecenia o nr {order.id}/{logsOrderDate.ToString("yyyy")}");

                // Return the detailed information of the order
                return new JsonResult(new
                {
                    result = "done",
                    order = order
                });
            }
            catch (MySqlException ex)
            {
                // Log any exceptions that occur during database operations
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ClientsController", "GetOrderDetails", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Retrieves detailed information about a specific complaint associated with an order, identified by the complaint ID.
        /// The method performs the following operations:
        /// - Validates the provided complaint ID. If the ID is -1, it returns an error.
        /// - Checks for a session token in the request cookies. If absent, returns a "no_auth" response.
        /// - Validates the session token using the CheckCookieToken service. If validation fails, returns a "no_auth" response.
        /// - Refreshes the session token expiration by updating the cookie with a new expiration time.
        /// - Checks if the user has authorization to view complaints. If not, returns a "no_permission" response.
        /// - Attempts to fetch the complaint details from the database using the OrdersController. If the complaint ID is invalid, returns an error.
        /// - Logs the action of retrieving complaint details using the LogsController.
        /// - Returns the detailed information of the complaint in a JSON response.
        /// - Catches and logs any MySQL exceptions that occur during database operations, returning an "error" response.
        /// </summary>
        /// <param name="id">The ID of the complaint for which details are being retrieved. Default value is -1.</param>
        /// <returns>JSON result containing the operation outcome and complaint details, or an error message.</returns>
        [HttpGet("orders/complaint")]
        public IActionResult GetComplaintDetails(int id = -1)
        {
            StringBuilder resultBuilder = new StringBuilder();

            if (id == -1)
                return new JsonResult(new { result = "error" });

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Validate session token
            DateTime expireCookie = DateTime.Now.AddHours(2);
            // Refresh the cookie expiration
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "clients", "view") && !roles.isAuthorized(_cookieValue, "orders", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie szczegółów reklamacji do zlecenia");

                var complaint = ordersController.GetComplaint(id);

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
                Logger.SendException("MechApp", "ClientsController", "GetComplaintDetails", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Adds a new client to the system. This method performs several key operations:
        /// - Checks for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" response.
        /// - Validates the session token. If the validation fails, it returns a "no_auth" response again.
        /// - Refreshes the cookie expiration to extend the user's session by updating the cookie with a new expiration time.
        /// - Checks if the user has the necessary permissions to add a client. If not, it returns a "no_permission" response.
        /// - Attempts to add the client to the database using the clientsController. Logs the action of adding a new client.
        /// - Catches and logs any MySQL exceptions that occur during the operation, returning an "error" response.
        /// </summary>
        /// <param name="client">The client object to be added. This object includes client details such as name, email, etc.</param>
        /// <returns>A JSON result indicating the outcome of the operation ("done" for success, "no_auth" for authentication failures, "no_permission" for authorization failures, or "error" for other errors).</returns>
        [HttpPost]
        public IActionResult AddClient([FromBody]AddEditClientOb client)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Validate session token
            DateTime expireCookie = DateTime.Now.AddHours(2);
            // Refresh the cookie expiration
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "clients", "add"))
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
                Logger.SendException("MechApp", "ClientsController", "AddClient", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// Adds a new vehicle to the system for a specific client. This method performs several key operations:
        /// - Checks for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" response.
        /// - Validates the session token. If the validation fails, it returns a "no_auth" response again.
        /// - Refreshes the cookie expiration to extend the user's session by updating the cookie with a new expiration time.
        /// - Checks if the user has the necessary permissions to add a vehicle. If not, it returns a "no_permission" response.
        /// - Attempts to add the vehicle to the database using the vehiclesController. Logs the action of adding a new vehicle.
        /// - Catches and logs any MySQL exceptions that occur during the operation, returning an "error" response.
        /// </summary>
        /// <param name="vehicle">The vehicle object to be added. This object includes vehicle details such as producer, model, etc.</param>
        /// <returns>A JSON result indicating the outcome of the operation ("done" for success, "no_auth" for authentication failures, "no_permission" for authorization failures, or "error" for other errors).</returns>
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

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Validate session token
            DateTime expireCookie = DateTime.Now.AddHours(2);
            // Refresh the cookie expiration
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "clients", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Dodanie pojazdu klienta");

                string result = vehiclesController.AddVehicle(vehicle.Producer, vehicle.Model, vehicle.ProduceDate, vehicle.Mileage, vehicle.Vin,
                    vehicle.EngineNumber, vehicle.RegistrationNumber, vehicle.EnginePower, vehicle.EngineCapacity, vehicle.fuelType, vehicle.Owner);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ClientsController", "AddVehicles", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Edits an existing client's information in the system. This method performs several key operations:
        /// - Checks for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" response.
        /// - Validates the session token. If the validation fails, it returns a "no_auth" response again.
        /// - Refreshes the cookie expiration to extend the user's session by updating the cookie with a new expiration time.
        /// - Checks if the user has the necessary permissions to edit client information. If not, it returns a "no_permission" response.
        /// - Attempts to edit the client's information in the database using the clientsController. Logs the action of editing a client.
        /// - Catches and logs any MySQL exceptions that occur during the operation, returning an "error" response.
        /// </summary>
        /// <param name="client">The client object containing the updated information. This object includes details such as name, email, etc.</param>
        /// <returns>A JSON result indicating the outcome of the operation ("done" for success, "no_auth" for authentication failures, "no_permission" for authorization failures, or "error" for other errors).</returns>
        [HttpPut]
        public IActionResult EditClient([FromBody]AddEditClientOb client)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Validate session token
            DateTime expireCookie = DateTime.Now.AddHours(2);
            // Refresh the cookie expiration
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "clients", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Edycja klienta");

                string result = clientsController.EditClient(client);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ClientsController", "EditClient", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// Edits the details of an existing vehicle in the system. This method performs several key operations:
        /// - Checks for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" response.
        /// - Validates the session token. If the validation fails, it returns a "no_auth" response again.
        /// - Refreshes the cookie expiration to extend the user's session by updating the cookie with a new expiration time.
        /// - Checks if the user has the necessary permissions to edit vehicle details. If not, it returns a "no_permission" response.
        /// - Attempts to edit the vehicle's details in the database using the vehiclesController. Logs the action of editing a vehicle.
        /// - Catches and logs any MySQL exceptions that occur during the operation, returning an "error" response.
        /// </summary>
        /// <param name="vehicle">The vehicle object containing the updated details. This object includes details such as produce date, mileage, engine number, etc.</param>
        /// <returns>A JSON result indicating the outcome of the operation ("done" for success, "no_auth" for authentication failures, "no_permission" for authorization failures, or "error" for other errors).</returns>
        [HttpPut("vehicles")]
        public IActionResult EditVehicle([FromBody]EditVehicleOb vehicle)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Validate session token
            DateTime expireCookie = DateTime.Now.AddHours(2);
            // Refresh the cookie expiration
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "clients", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Edycja pojazdu klienta");

                string result = vehiclesController.EditVehicle(vehicle.id, vehicle.ProduceDate, vehicle.Mileage, vehicle.EngineNumber, vehicle.RegistrationNumber,
                    vehicle.EnginePower, vehicle.EngineCapacity);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ClientsController", "EditVehicle", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Deletes a client from the system based on the provided client ID. This method performs several key operations:
        /// - Checks for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" response.
        /// - Validates the session token. If the validation fails, it returns a "no_auth" response again.
        /// - Refreshes the cookie expiration to extend the user's session by updating the cookie with a new expiration time.
        /// - Checks if the user has the necessary permissions to delete a client. If not, it returns a "no_permission" response.
        /// - Attempts to delete the client from the database using the clientsController. Logs the action of deleting a client.
        /// - Catches and logs any MySQL exceptions that occur during the operation, returning an "error" response.
        /// </summary>
        /// <param name="id">The ID of the client to be deleted.</param>
        /// <returns>A JSON result indicating the outcome of the operation ("done" for success, "no_auth" for authentication failures, "no_permission"
        [HttpDelete("client")]
        public IActionResult DeleteClient(int id)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Validate session token
            DateTime expireCookie = DateTime.Now.AddHours(2);
            // Refresh the cookie expiration
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "clients", "delete"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Usunięcie konta klienta");

                string result = clientsController.DeleteClient(id);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ClientsController", "DeleteClient", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Deletes multiple clients from the system based on a list of client IDs. This method performs several key operations:
        /// - Checks for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" response.
        /// - Validates the session token. If the validation fails, it returns a "no_auth" response again.
        /// - Refreshes the cookie expiration to extend the user's session by updating the cookie with a new expiration time.
        /// - Checks if the user has the necessary permissions to delete clients. If not, it returns a "no_permission" response.
        /// - Validates the list of client IDs to ensure it is not empty. If it is, returns an "error" response.
        /// - Converts the list of client IDs from a string to a list of integers.
        /// - Attempts to delete the clients from the database using the clientsController. Logs the action of deleting clients.
        /// - Catches and logs any MySQL exceptions that occur during the operation, returning an "error" response.
        /// </summary>
        /// <param name="idsList">A comma-separated string containing the IDs of the clients to be deleted.</param>
        /// <returns>A JSON result indicating the outcome of the operation ("done" for success, "no_auth" for authentication failures, "no_permission" for authorization failures, or "error" for other errors).</returns>
        [HttpDelete("clients")]
        public IActionResult DeleteClients(string idsList)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Validate session token
            DateTime expireCookie = DateTime.Now.AddHours(2);
            // Refresh the cookie expiration
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "clients", "delete"))
                return new JsonResult(new { result = "no_permission" });

            if (string.IsNullOrEmpty(idsList))
                return new JsonResult(new { result = "error" });

            List<int> ids = !string.IsNullOrEmpty(idsList) ? idsList.Split(',').Select(id => Convert.ToInt32(id)).ToList() : new List<int>();

            try
            {
                logsController.AddLog(_cookieValue, "Usunięcie klientów");

                string result = clientsController.DeleteClients(ids);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ClientsController", "DeleteClients", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// Deletes a vehicle from the system based on the provided vehicle ID. This method performs several key operations:
        /// - Checks for the presence of a session token in cookies. If the token is missing, it returns a "no_auth" response.
        /// - Validates the session token. If the validation fails, it returns a "no_auth" response again.
        /// - Refreshes the cookie expiration to extend the user's session by updating the cookie with a new expiration time.
        /// - Checks if the user has the necessary permissions to delete a vehicle. If not, it returns a "no_permission" response.
        /// - Validates the vehicle ID. If the ID is -1, it returns an "error" response.
        /// - Attempts to delete the vehicle from the database using the vehiclesController. Logs the action of deleting a vehicle.
        /// - Catches and logs any MySQL exceptions that occur during the operation, returning an "error" response.
        /// </summary>
        /// <param name="id">The ID of the vehicle to be deleted. Default value is -1.</param>
        /// <returns>A JSON result indicating the outcome of the operation ("done" for success, "no_auth" for authentication failures, "no_permission" for authorization failures, or "error" for other errors).</returns>
        [HttpDelete("vehicles")]
        public IActionResult DeleteVehicle(int id = -1)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check for session token in cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Validate session token
            DateTime expireCookie = DateTime.Now.AddHours(2);
            // Refresh the cookie expiration
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "clients", "delete"))
                return new JsonResult(new { result = "no_permission" });

            if (id == -1)
                return new JsonResult(new { result = "error" });

            try
            {
                logsController.AddLog(_cookieValue, "Usunięcie pojazdu klienta");

                string result = vehiclesController.DeleteVehicle(id);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ClientsController", "DeleteVehicle", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }
    }
}
