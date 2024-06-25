using MechAppBackend.Data;
using MechAppBackend.features;
using MechAppBackend.Helpers;
using MechAppBackend.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        MechAppContext _context;
        CheckCookieToken cookieToken;
        vehicles vehicleController;
        orders ordersController;
        logs logsController;
        CheckRoles roles;

        public ProfileController(MechAppContext context)
        {
            _context = context;
            cookieToken = new CheckCookieToken(context);
            vehicleController = new vehicles(context);
            ordersController = new orders(context);
            logsController = new logs(context);
            roles = new CheckRoles(context);
        }

        /// <summary>
        /// Retrieves the personal data of a user based on a session token stored in cookies.
        /// This method checks for the presence of a session token, validates it, and then fetches user details from the database.
        /// It also extends the session token expiration to maintain user session continuity.
        ///
        /// Flow:
        /// 1. Check for the presence of the "sessionToken" cookie.
        /// 2. Validate the token using the cookieToken service.
        /// 3. Refresh the token's expiration time in the cookie.
        /// 4. Fetch user details associated with the token from the database.
        /// 5. Return user details if successful; otherwise, return an error.
        ///
        /// Returns:
        /// - A JSON result with user details if the token is valid and the user exists.
        /// - "no_auth": Returned if the session token is missing or invalid.
        /// - "error": Returned if the token does not correspond to any user or if a database error occurs.
        ///
        /// Usage:
        /// - This method is typically used in scenarios where user data needs to be fetched from a client-side application
        ///   that relies on session continuity and user authentication, such as personal profile pages.
        /// </summary>
        /// <returns>A JsonResult containing either the user's data or an error/result code indicating the operation's outcome.</returns>
        [HttpGet]
        public IActionResult GetUserData()
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

            if (!roles.isAuthorized(_cookieValue, "profile", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Fetch user data from database using the session token
                var token = _context.UsersTokens.FirstOrDefault(t => t.Token == _cookieValue);

                if (token == null)
                    return new JsonResult(new { result = "error" });

                var user = _context.Users.FirstOrDefault(u => u.Id == token.UserId);

                if (user == null)
                    return new JsonResult(new { result = "error" });

                logsController.AddLog(_cookieValue, $"Pobranie danych użytkownika {user.Name} {user.Lastname}");

                // Return the user data if everything is valid
                return new JsonResult(new
                {
                    result = "done",
                    name = user.Name,
                    lastname = user.Lastname,
                    email = user.Email,
                    nip = user.Nip,
                    phone = user.Phone,
                    postcode = user.Postcode,
                    city = user.City,
                    address = user.Address,
                    image = user.Icon
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ProfileController", "GetUserData", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// Retrieves a list of vehicles associated with a user, identified through a session token.
        /// This method checks for the presence of a "sessionToken" cookie, validates it, and uses it to query the user's vehicles that are not marked as deleted.
        /// It extends the session token expiration to maintain user session continuity and returns the vehicles if the session is authenticated.
        ///
        /// Flow:
        /// 1. Check for the presence of the "sessionToken" cookie.
        /// 2. Validate the session token.
        /// 3. Refresh the token's expiration time in the cookie.
        /// 4. Fetch the vehicles owned by the user associated with the token.
        /// 5. Return the vehicles if successful; otherwise, return an error.
        ///
        /// Returns:
        /// - A JSON result containing a list of the user's vehicles if the token is valid and the user has vehicles.
        /// - "no_auth": Returned if the session token is missing or invalid.
        /// - "error": Returned if there's a failure in finding the token, accessing the vehicles, or during database operations.
        ///
        /// Usage:
        /// - This endpoint is typically used to provide users with a view of all their registered vehicles in a client-side application,
        ///   particularly useful in automotive service platforms or user profile sections where vehicle management is required.
        /// </summary>
        /// <returns>A JsonResult containing either a list of vehicles or an error/result code indicating the outcome of the operation.</returns>
        [HttpGet("vehicles")]
        public IActionResult GetUserVehicles()
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

            if (!roles.isAuthorized(_cookieValue, "profile", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Fetch user data from database using the session token
                var token = _context.UsersTokens.FirstOrDefault(t => t.Token == _cookieValue);

                if (token == null)
                    return new JsonResult(new { result = "error" });

                var vehicles = _context.UsersVehicles
                    .Where(v => v.Owner == token.UserId && v.IsDeleted != 1)
                    .Select(v => new
                    {
                        producer = v.Producer,
                        model = v.Model,
                        produceDate = v.ProduceDate,
                        mileage = v.Mileage,
                        vin = v.Vin,
                        engineNumber = v.EngineNumber,
                        registrationNumber = v.RegistrationNumber,
                        enginePower = v.EnginePower,
                        engineCapacity = v.EngineCapacity,
                        fuelType = v.FuelType
                    })
                    .ToList();

                logsController.AddLog(_cookieValue, $"Pobranie listy pojazdów użytkownika {_context.Users.Where(u => u.Id == token.UserId).Select(u => u.Name).FirstOrDefault()} " +
                    $"{_context.Users.Where(u => u.Id == token.UserId).Select(u => u.Lastname).FirstOrDefault()}");

                // Return the list of vehicles if found
                return new JsonResult(new
                {
                    result = "done",
                    vehicles = vehicles
                });
            }
            catch (MySqlException ex)
            {
                // Log any exceptions that occur during database operations
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ProfileController", "GetUserVehicles", ex);
            }

            return new JsonResult(new { result = "error" });
        }

        /// <summary>
        /// Retrieves a list of orders for the authenticated user based on a specified order status.
        /// This method validates the presence of a session token, checks its validity, and then queries orders based on the provided status.
        /// The session token expiration is also extended to maintain user session continuity.
        ///
        /// Flow:
        /// 1. Check for the presence of the "sessionToken" cookie.
        /// 2. Validate the session token.
        /// 3. Refresh the token's expiration time in the cookie.
        /// 4. Fetch the orders for the authenticated user that match the specified status.
        /// 5. Return the orders if successful; otherwise, return an error.
        ///
        /// Returns:
        /// - A JSON result containing a list of orders if the token is valid and orders exist for the given status.
        /// - "no_auth": Returned if the session token is missing or invalid.
        /// - "error": Returned if there's a failure in finding the token, accessing the orders, or during database operations.
        ///
        /// Usage:
        /// - This endpoint is typically used to provide users with a view of their orders based on their status in a client-side application,
        ///   such as managing pending, active, or completed orders in a service or sales platform.
        /// </summary>
        /// <param name="_status">The status of the orders to retrieve, which filters orders based on their current state.</param>
        /// <returns>A JsonResult containing either a list of orders or an error/result code indicating the operation's outcome.</returns>
        [HttpGet("orders/{_status}")]
        public IActionResult GetClientsOrders(int _status)
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

            if (!roles.isAuthorized(_cookieValue, "profile", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                //Fetch user data from database using the session token
                var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.Token == _cookieValue);

                if (sessionToken == null)
                    return new JsonResult(new { result = "error" });
                //Retrieve orders matching the status for the user
                var orders = ordersController.GetClientOrders(_status, (int)sessionToken.UserId);

                logsController.AddLog(_cookieValue, $"Pobranie listy zleceń użytkownika {_context.Users.Where(u => u.Id == sessionToken.UserId).Select(u => u.Name).FirstOrDefault()} " +
                    $"{_context.Users.Where(u => u.Id == sessionToken.UserId).Select(u => u.Lastname).FirstOrDefault()}");

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
                Logger.SendException("MechApp", "ProfileController", "GetClientOrders", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Retrieves a specific order complaint based on the provided order ID.
        /// </summary>
        /// <param name="orderId">The ID of the order for which the complaint is being requested. If -1, indicates an error or invalid request.</param>
        /// <returns>A JSON result indicating the outcome of the request. This can be a success with the complaint details, an error, or a no_auth response if authentication fails.</returns>
        [HttpGet("order/complaints")]
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
        /// Retrieves detailed information for a specific order based on its ID. The method validates the user's session token,
        /// extends the session, and queries the order details.
        /// 
        /// Flow:
        /// 1. Validates the presence and validity of the "sessionToken" cookie.
        /// 2. Refreshes the token's expiration time in the cookie.
        /// 3. Queries the order details by ID using the authenticated user's session.
        /// 4. Returns the order details if successful; otherwise, handles errors appropriately.
        ///
        /// Returns:
        /// - A JSON result containing detailed information about the order if the session is valid and the order exists.
        /// - "no_auth": Returned if the session token is missing or invalid.
        /// - "error": Returned if there's an issue finding the order, if the order ID is invalid, or during database operations.
        ///
        /// Usage:
        /// - This endpoint is typically used to provide detailed information about an order to the client-side application,
        ///   such as in scenarios where a detailed view of an order is required for review or management.
        /// </summary>
        /// <param name="id">The unique identifier of the order to retrieve details for.</param>
        /// <returns>A JsonResult containing either the order details or an error/result code indicating the operation's outcome.</returns>
        [HttpGet("order/{id}")]
        public IActionResult GetOrderId(int id)
        {
            StringBuilder resultBuilder = new StringBuilder();
            // Validate the order ID
            if (id == null)
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

            if (!roles.isAuthorized(_cookieValue, "profile", "edit"))
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
                Logger.SendException("MechApp", "ProfileController", "GetOrderDetails", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Adds a new vehicle to the user's profile based on the provided vehicle details. This method checks for a valid session token,
        /// confirms user authentication, and proceeds to add the vehicle data to the database.
        /// 
        /// Flow:
        /// 1. Validate the presence and authenticity of the "sessionToken" cookie.
        /// 2. Refresh the token's expiration time to maintain session continuity.
        /// 3. Retrieve the user associated with the session token.
        /// 4. Add vehicle information to the database associated with the user.
        /// 5. Return the outcome of the vehicle addition operation.
        ///
        /// Parameters:
        /// - vehicle: An object of type 'AddVehicleOb' which contains all necessary vehicle information:
        ///   - Producer: The manufacturer of the vehicle.
        ///   - Model: The model of the vehicle.
        ///   - ProduceDate: The production date of the vehicle.
        ///   - Mileage: The current mileage of the vehicle.
        ///   - Vin: The Vehicle Identification Number.
        ///   - EngineNumber: The engine number of the vehicle.
        ///   - RegistrationNumber: The registration number of the vehicle.
        ///   - EnginePower: The power output of the vehicle's engine.
        ///   - EngineCapacity: The engine capacity of the vehicle.
        ///   - FuelType: The type of fuel the vehicle uses.
        ///   - UserId: The ID of the user to whom the vehicle will be registered.
        ///
        /// Returns:
        /// - A JSON result indicating success or failure of the vehicle addition process.
        /// - "no_auth": Returned if the session token is missing or invalid.
        /// - "error": Returned if there's an issue accessing the database, finding the user, or during the addition process.
        ///
        /// Usage:
        /// - This endpoint is typically used in a client-side application where users need to add new vehicles to their profiles,
        ///   particularly useful for applications dealing with vehicle management or user-owned assets tracking.
        /// </summary>
        /// <param name="vehicle">The vehicle object containing all details necessary for registration.</param>
        /// <returns>A JsonResult indicating the result of the operation.</returns>
        [HttpPost("vehicle/add")]
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

            if (!roles.isAuthorized(_cookieValue, "profile", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Retrieve user and add vehicle
                var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.Token == _cookieValue);

                if (sessionToken == null)
                    return new JsonResult(new { result = "error" });

                var user = _context.Users.FirstOrDefault(u => u.Id == sessionToken.UserId);

                if (user == null)
                    return new JsonResult(new { result = "error" });

                string _addResult = vehicleController.AddVehicle(vehicle.Producer, vehicle.Model, vehicle.ProduceDate,
                    vehicle.Mileage, vehicle.Vin, vehicle.EngineNumber, vehicle.RegistrationNumber, vehicle.EnginePower, vehicle.EngineCapacity,
                    vehicle.fuelType, Convert.ToInt32(user.Id));

                logsController.AddLog(_cookieValue, $"Dodanie pojazdu użytkownika {user.Name} {user.Lastname}");

                resultBuilder.Append(_addResult);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("Mechapp", "ProfileController", "AddVehicle", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP POST endpoint for adding a complaint to an order.
        /// </summary>
        /// <param name="complaint">The complaint object containing details about the complaint to be added.</param>
        /// <returns>JSON result containing the operation result status.</returns>
        [HttpPost("complaint")]
        public IActionResult AddComplaint([FromBody] addOrderComplaintOb complaint)
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
        /// Edits the details of an existing vehicle in the system.
        /// </summary>
        /// <param name="vehicle">The vehicle object containing the details to be updated.</param>
        /// <returns>A JSON result indicating the success or failure of the operation.</returns>
        [HttpPost("vehicles/edit")]
        public IActionResult EditVehicle([FromBody]EditVehicleOb vehicle)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check if the session token cookie exists
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Check if the session token is valid
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Extend the session token's expiry time
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };
            // Update the session token cookie
            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "profile", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Attempt to edit the vehicle
                string _editResult = vehicleController.EditVehicle(vehicle.id, vehicle.ProduceDate, vehicle.Mileage, vehicle.EngineNumber,
                    vehicle.RegistrationNumber, vehicle.EnginePower, vehicle.EngineCapacity);

                var user = _context.Users.FirstOrDefault(u => u.Id == _context.UsersVehicles.Where(v => v.Id == vehicle.id).Select(v => v.Owner).FirstOrDefault());

                if (user == null)
                    return new JsonResult(new { result = "error" });

                logsController.AddLog(_cookieValue, $"Edycja pojazdu użytkownika {user.Name} {user.Lastname}");

                resultBuilder.Append(_editResult);
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an error response
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ProfileController", "EditVehicle", ex);
            }
            // Return the result of the operation
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Edits the details of the user's profile.
        /// </summary>
        /// <param name="edit">The object containing the new details for the user's profile.</param>
        /// <returns>A JSON result indicating the success or failure of the operation.</returns>
        [HttpPost("edit")]
        public IActionResult EditProfile([FromBody] editProfileob edit)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check if the session token cookie exists
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" }); // If not, return an unauthorized response
            // Check if the session token is valid
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Extend the session token's expiry time
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };
            // Update the session token cookie
            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);
            // Validate the phone number format
            if (!string.IsNullOrEmpty(edit.phone) && !Validators.CheckPhone(edit.phone, edit.country.ToUpper()))
                return new JsonResult(new { result = "phone_format" });

            if (!roles.isAuthorized(_cookieValue, "profile", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Get the session token from the database
                var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.Token ==  _cookieValue);
                // If the session token doesn't exist, return an error response
                if (sessionToken == null)
                    return new JsonResult("error");
                // Get the user associated with the session token
                var user = _context.Users.FirstOrDefault(u => u.Id == sessionToken.UserId);
                // If the user doesn't exist, return an error response
                if (user == null)
                    return new JsonResult(new { result = "error" });
                // Update the user's profile details
                user.Nip = edit.nip;
                user.Phone = edit.phone;
                user.Postcode = edit.postcode;
                user.City = edit.city;
                user.Address = edit.address;
                user.Icon = edit.image;
                // Save the changes to the database

                logsController.AddLog(_cookieValue, $"Edycja profilu użytkownika {user.Name} {user.Lastname}");

                _context.SaveChanges();

                resultBuilder.Append("profile_edited");
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an error response
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ProfileController", "EditProfile", ex);
            }
            // Return the result of the operation
            return new JsonResult(new {result =  resultBuilder.ToString()});
        }

        /// <summary>
        /// Accepts an order by changing its status to 2.
        /// </summary>
        /// <param name="id">The ID of the order to be accepted.</param>
        /// <returns>A JSON result indicating the success or failure of the operation.</returns>
        [HttpPost("order/accept/{id}")]
        public IActionResult AcceptOrder(int id)
        {
            StringBuilder resultBuilder = new StringBuilder();
            // Check if the session token cookie exists
            string _cookieValue = string.Empty;

            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Check if the session token is valid
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Extend the session token's expiry time
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };
            // Update the session token cookie
            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "profile", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Attempt to change the order status to 2 (accepted)
                string chStatus = ordersController.ChangeOrderStatus(id, 2);

                var order = _context.Orders.FirstOrDefault(o => o.Id == id);

                if (order == null)
                    return new JsonResult(new { result = "error" });

                DateTime logsStartDate = (DateTime)order.StartDate;

                logsController.AddLog(_cookieValue, $"Akceptacja kosztorysu zlecenia {order.Id}/{logsStartDate.ToString("yyyy")}");

                resultBuilder.Append(chStatus);
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an error response
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ProfileController", "AcceptOrder", ex);
            }
            // Return the result of the operation
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Refuses an order by changing its status to 3.
        /// </summary>
        /// <param name="id">The ID of the order to be refused.</param>
        /// <returns>A JSON result indicating the success or failure of the operation.</returns>
        [HttpPost("order/refuse/{id}")]
        public IActionResult RefuseOrder(int id)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check if the session token cookie exists
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Check if the session token is valid
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });

            DateTime expireCookie = DateTime.Now.AddHours(2);
            // Extend the session token's expiry time
            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };
            // Update the session token cookie
            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "profile", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Attempt to change the order status to 3 (refused)
                string chStatus = ordersController.ChangeOrderStatus(id, 3);

                var order = _context.Orders.FirstOrDefault(o => o.Id == id);

                if (order == null)
                    return new JsonResult(new { result = "error" });

                DateTime logsStartDate = (DateTime)order.StartDate;

                logsController.AddLog(_cookieValue, $"Odrzucenie kosztorysu zlecenia {order.Id}/{logsStartDate.ToString("yyyy")}");

                resultBuilder.Append(chStatus);
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an error response
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ProfileController", "RefuseOrder", ex);
            }
            // Return the result of the operation
            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// Changes the user's email.
        /// </summary>
        /// <param name="email">The object containing the old, new, and repeated new email.</param>
        /// <returns>A JSON result indicating the success or failure of the operation.</returns>
        [HttpPost("email")]
        public IActionResult ChangeEmail([FromBody] profileChangeEmailOb email)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check if the session token cookie exists
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Check if the session token is valid
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Extend the session token's expiry time
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };
            // Update the session token cookie
            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);
            // Validate the email format and check if the new email matches the repeated new email
            if (string.IsNullOrEmpty(email.oldEmail))
                return new JsonResult(new { result = "no_old_email" });
            if (string.IsNullOrEmpty(email.newEmail))
                return new JsonResult(new { result = "no_new_email" });
            if (string.IsNullOrEmpty(email.repeatEmail))
                return new JsonResult(new { result = "no_repeat_email" });

            if (!Validators.ValidateEmail(email.newEmail))
                return new JsonResult(new { result = "email_format" });

            if (email.newEmail != email.repeatEmail)
                return new JsonResult(new { result = "email_not_match" });

            if (!roles.isAuthorized(_cookieValue, "profile", "emailChange"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Get the session token from the database
                var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.Token == _cookieValue);
                // If the session token doesn't exist, return an error response
                if (sessionToken == null)
                    return new JsonResult(new { result = "error" });
                // Check if the new email already exists in the database
                var chEmail = _context.Users.FirstOrDefault(u => u.Email == email.newEmail && u.Id != sessionToken.UserId);
                // If the new email already exists and it's not the same as the old email, return an error response
                if (chEmail != null && email.oldEmail != email.newEmail)
                    return new JsonResult(new { result = "email_exists" });
                // Get the user associated with the session token
                var user = _context.Users.FirstOrDefault(u => u.Id == sessionToken.UserId);
                // If the user doesn't exist, return an error response
                if (user == null)
                    return new JsonResult(new { result = "error" });
                // Update the user's email
                user.Email = email.newEmail;

                logsController.AddLog(_cookieValue, $"Zmiana adresu email użytkownika {user.Name} {user.Lastname}");

                // Save the changes to the database
                _context.SaveChanges();

                resultBuilder.Append("email_changed");
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an error response
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ProfileController", "EditEmail", ex);
            }
            // Return the result of the operation
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Changes the user's password.
        /// </summary>
        /// <param name="pass">The object containing the new and repeated new password.</param>
        /// <returns>A JSON result indicating the success or failure of the operation.</returns>
        [HttpPost("password")]
        public IActionResult ChangePassword(profileChangePasswordOb pass)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check if the session token cookie exists
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Check if the session token is valid
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Extend the session token's expiry time
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };
            // Update the session token cookie
            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);
            // Validate the password format and check if the new password matches the repeated new password
            if (string.IsNullOrEmpty(pass.password))
                return new JsonResult(new { result = "no_password" });
            if (string.IsNullOrEmpty(pass.repeatpassword))
                return new JsonResult(new { result = "no_repeat_password" });

            if (!Validators.CheckPasswordPolicy(pass.password))
                return new JsonResult(new { result = "password_policy_not_match" });

            if (pass.password != pass.repeatpassword)
                return new JsonResult(new { result = "password_not_matching" });

            if (!roles.isAuthorized(_cookieValue, "profile", "passChange"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Get the session token from the database
                var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.Token == _cookieValue);
                // If the session token doesn't exist, return an error response
                if (sessionToken == null)
                    return new JsonResult(new { result = "error" });
                // Get the user associated with the session token
                var user = _context.Users.FirstOrDefault(u => u.Id == sessionToken.UserId);
                // If the user doesn't exist, return an error response
                if (user == null)
                    return new JsonResult(new { result = "error" });
                // Generate a new salt
                string salt = generators.generatePassword(15);
                // Combine the new password with the salt
                string combinedPassword = pass.password + salt;
                // Hash the combined password and salt
                user.Password = hashes.GenerateSHA512Hash(combinedPassword);
                user.Salt = salt;

                logsController.AddLog(_cookieValue, $"Zmiana hasła użytkownika {user.Name} {user.Lastname}");
                // Save the changes to the database
                _context.SaveChanges();

                resultBuilder.Append("password_changed");
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an error response
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ProfileController", "ChangePassword", ex);
            }
            // Return the result of the operation
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Changes the user's phone number.
        /// </summary>
        /// <param name="phone">The object containing the new phone number and the country code for format validation.</param>
        /// <returns>A JSON result indicating the success or failure of the operation.</returns>
        [HttpPost("phone")]
        public IActionResult EditPhone([FromBody] profileChangePhoneOb phone)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check if the session token cookie exists
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Check if the session token is valid
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Extend the session token's expiry time
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };
            // Update the session token cookie
            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);
            // Validate the phone number format
            if (string.IsNullOrEmpty(phone.phone))
                return new JsonResult(new { result = "no_phone" });

            if (!Validators.CheckPhone(phone.phone, phone.country))
                return new JsonResult(new { result = "phone_format" });

            if (!roles.isAuthorized(_cookieValue, "profile", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Get the session token from the database
                var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.Token == _cookieValue);
                // If the session token doesn't exist, return an error response
                if (sessionToken == null)
                    return new JsonResult(new { result = "error" });
                // Get the user associated with the session token
                var user = _context.Users.FirstOrDefault(u => u.Id == sessionToken.UserId);
                // If the user doesn't exist, return an error response
                if (user == null)
                    return new JsonResult(new { result = "error" });
                // Update the user's phone number
                user.Phone = phone.phone;

                logsController.AddLog(_cookieValue, $"Zmiana numeru telefonu użytkownika {user.Name} {user.Lastname}");
                // Save the changes to the database
                _context.SaveChanges();

                resultBuilder.Append("phone_changed");
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an error response
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ProfileController", "EditPhone", ex);
            }
            // Return the result of the operation
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Deletes the user's account.
        /// </summary>
        /// <returns>A JSON result indicating the success or failure of the operation.</returns>
        [HttpDelete]
        public IActionResult DeleteUser()
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check if the session token cookie exists
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Check if the session token is valid
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Extend the session token's expiry time
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };
            // Update the session token cookie
            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "profile", "delAccount"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Get the session token from the database
                var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.Token == _cookieValue);
                // If the session token doesn't exist, return an error response
                if (sessionToken == null)
                    return new JsonResult(new { result = "error" });
                // Get the user associated with the session token
                var user = _context.Users.FirstOrDefault(u => u.Id == sessionToken.UserId);
                // If the user doesn't exist, return an error response
                if (user == null)
                    return new JsonResult(new { result = "error" });
                // Remove the user's departments, roles, and tokens from the database
                _context.UsersDepartments.RemoveRange(_context.UsersDepartments.Where(ud => ud.UserId == user.Id));
                _context.UsersRoles.RemoveRange(_context.UsersRoles.Where(ur => ur.UserId == user.Id));
                _context.UsersTokens.RemoveRange(_context.UsersTokens.Where(ur => ur.UserId == user.Id));
                // Mark the user as deleted
                user.IsDeleted = 1;

                logsController.AddLog(_cookieValue, $"Usunięcie konta użytkownika {user.Name} {user.Lastname}");
                // Save the changes to the database
                _context.SaveChanges();
                // If an error occurs, log it and return an error response
                resultBuilder.Append("user_deleted");
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an error response
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ProfileController", "DeleteUser", ex);
            }
            // Return the result of the operation
            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// Deletes a vehicle associated with the user's account.
        /// </summary>
        /// <param name="id">The ID of the vehicle to be deleted.</param>
        /// <returns>A JSON result indicating the success or failure of the operation.</returns>
        [HttpDelete("vehicles/delete/{id}")]
        public IActionResult DeleteVehicle(int id)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;
            // Check if the session token cookie exists
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });
            // Check if the session token is valid
            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });
            // Extend the session token's expiry time
            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };
            // Update the session token cookie
            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "profile", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == _context.UsersVehicles.Where(v => v.Id == id).Select(v => v.Owner).FirstOrDefault());

                if (user == null)
                    return new JsonResult(new { result = "error" });

                // Attempt to delete the vehicle
                string _deleteResult = vehicleController.DeleteVehicle(id);

                logsController.AddLog(_cookieValue, $"Usunięcie pojazdu użytkownika {user.Name} {user.Lastname}");

                resultBuilder.Append(_deleteResult);
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an error response
                resultBuilder.Clear().Append("error");
                Logger.SendException("Mechapp", "ProfileController", "DeleteVehicle", ex);
            }
            // Return the result of the operation
            return new JsonResult(new {result = resultBuilder.ToString() });
        }

       
    }

    public class profileChangePasswordOb
    {
        public string? password { get; set; }
        public string? repeatpassword { get; set; }
    }

    public class profileChangeEmailOb
    {
        public string? oldEmail { get; set; }
        public string? newEmail { get; set; }
        public string? repeatEmail { get; set; }
    }

    public class profileChangePhoneOb
    {
        public string? phone { get; set; }
        public string? country { get; set; }
    }

    public class editProfileob
    {
        public string? nip { get; set; }
        public string? phone { get; set; }
        public string? country { get; set; }
        public string? postcode { get; set; }
        public string? city { get; set; }
        public string? address { get; set; }
        public string? image { get; set; }
    }
}
