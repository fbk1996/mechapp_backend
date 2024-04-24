using MechAppBackend.Data;
using MechAppBackend.features;
using MechAppBackend.Helpers;
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

        public ProfileController(MechAppContext context)
        {
            _context = context;
            cookieToken = new CheckCookieToken(context);
            vehicleController = new vehicles(context);
        }

        [HttpGet]
        public IActionResult GetUserData()
        {
            StringBuilder resultBuilder = new StringBuilder();


            string _cookieValue = string.Empty;

            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });

            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            try
            {
                var token = _context.UsersTokens.FirstOrDefault(t => t.Token == _cookieValue);

                if (token == null)
                    return new JsonResult(new { result = "error" });

                var user = _context.Users.FirstOrDefault(u => u.Id == token.UserId);

                if (user == null)
                    return new JsonResult(new { result = "error" });

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

        [HttpGet("vehicles")]
        public IActionResult GetUserVehicles()
        {
            StringBuilder resultBuilder = new StringBuilder();


            string _cookieValue = string.Empty;

            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });

            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            try
            {
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

                return new JsonResult(new
                {
                    result = "done",
                    vehicles = vehicles
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ProfileController", "GetUserVehicles", ex);
            }

            return new JsonResult(new { result = "error" });
        }

        [HttpPost("vehicle/add")]
        public IActionResult AddVehicle([FromBody]AddVehicleOb vehicle)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;

            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });

            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            try
            {
                var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.Token == _cookieValue);

                if (sessionToken == null)
                    return new JsonResult(new { result = "error" });

                var user = _context.Users.FirstOrDefault(u => u.Id == sessionToken.UserId);

                if (user == null)
                    return new JsonResult(new { result = "error" });

                string _addResult = vehicleController.AddVehicle(vehicle.Producer, vehicle.Model, vehicle.ProduceDate,
                    vehicle.Mileage, vehicle.Vin, vehicle.EngineNumber, vehicle.RegistrationNumber, vehicle.EnginePower, vehicle.EngineCapacity,
                    vehicle.fuelType, Convert.ToInt32(user.Id));

                resultBuilder.Append(_addResult);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("Mechapp", "ProfileController", "AddVehicle", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString() });
        }

        [HttpPost("vehicles/edit")]
        public IActionResult EditVehicle([FromBody]EditVehicleOb vehicle)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;

            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });

            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            try
            {
                string _editResult = vehicleController.EditVehicle(vehicle.id, vehicle.ProduceDate, vehicle.Mileage, vehicle.EngineNumber,
                    vehicle.RegistrationNumber, vehicle.EnginePower, vehicle.EngineCapacity);

                resultBuilder.Append(_editResult);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "ProfileController", "EditVehicle", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        [HttpDelete("vehicles/delete/{id}")]
        public IActionResult DeleteVehicle(int id)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;

            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });

            DateTime expireCookie = DateTime.Now.AddHours(2);

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            try
            {
                string _deleteResult = vehicleController.DeleteVehicle(id);

                resultBuilder.Append(_deleteResult);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("Mechapp", "ProfileController", "DeleteVehicle", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString() });
        }
    }
}
