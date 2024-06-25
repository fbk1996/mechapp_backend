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
