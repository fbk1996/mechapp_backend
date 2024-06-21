using MechAppBackend.Data;
using MechAppBackend.features;
using MechAppBackend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        MechAppContext _context;
        logs logsController;
        CheckCookieToken cookieToken;
        CheckRoles roles;
        warehouse warehouseController;

        public WarehouseController(MechAppContext context)
        {
            _context = context;
            logsController = new logs(context);
            cookieToken = new CheckCookieToken(context);
            roles = new CheckRoles(context);
            warehouseController = new warehouse(context);
        }

        /// <summary>
        /// HTTP GET endpoint for retrieving a list of warehouse items based on optional filters and pagination.
        /// </summary>
        /// <param name="name">Optional filter by item name.</param>
        /// <param name="departmentIds">Optional filter by department IDs, comma-separated.</param>
        /// <param name="currentPage">The current page number for pagination.</param>
        /// <param name="pageSize">The number of items per page for pagination.</param>
        /// <returns>JSON result containing the operation result status and a list of filtered warehouse items.</returns>
        [HttpGet]
        public IActionResult GetWarehouseItems(string? name, string? departmentIds, int currentPage, int pageSize)
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

            if (!roles.isAuthorized(_cookieValue, "warehouse", "view"))
                return new JsonResult(new { result = "no_permission" });

            List<long> depIDs = !string.IsNullOrEmpty(departmentIds) ? departmentIds.Split(',').Select(id => Convert.ToInt64(id)).ToList() : new List<long>();

            int offset = ((currentPage - 1) * pageSize);

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie przedmiotów z magazynu");

                List<warehouseItemOb> items = warehouseController.GetWarehouseItems(name, depIDs, offset, pageSize);

                return new JsonResult(new
                {
                    result = "done",
                    items = items
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "WarehouseController", "GetWarehouseItems", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString()});
        }

        /// <summary>
        /// HTTP GET endpoint for retrieving detailed information about a specific warehouse item identified by ID or EAN.
        /// </summary>
        /// <param name="id">Optional parameter for the item's ID. If provided, it takes precedence over EAN.</param>
        /// <param name="ean">Optional parameter for the item's EAN (European Article Number).</param>
        /// <returns>JSON result containing the operation result status and detailed information about the warehouse item.</returns>
        [HttpGet("details")]
        public IActionResult GetItemDetails(int? id, string? ean)
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

            if (!roles.isAuthorized(_cookieValue, "warehouse", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                warehouseItemDetailsOb item = warehouseController.GetWarehouseItemDetails(id, ean);

                if (item == null)
                    return new JsonResult(new { result = "error" });
                if (item.id == -1)
                    return new JsonResult(new { result = "error" });

                logsController.AddLog(_cookieValue, $"Pobranie szczegółów przedmiotu: {item.name}");

                return new JsonResult(new
                {
                    result = "done",
                    item = item
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "WarehouseController", "GetItemDetails", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP POST endpoint for adding a new warehouse item.
        /// </summary>
        /// <param name="item">The warehouse item details object to be added.</param>
        /// <returns>JSON result containing the operation result status.</returns>
        [HttpPost("add")]
        public IActionResult AddWarehouseItem([FromBody]AddEditWarehouseItemDetailsOb item)
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

            if (!roles.isAuthorized(_cookieValue, "warehouse", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                string result = warehouseController.AddWarehouseItem(item);
                logsController.AddLog(_cookieValue, $"Dodanie przedmiotu: {item.name}");
                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "WarehouseController", "AddWarehouseItem", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP POST endpoint for editing an existing warehouse item.
        /// </summary>
        /// <param name="item">The warehouse item details object to be edited.</param>
        /// <returns>JSON result containing the operation result status.</returns>
        [HttpPost("edit")]
        public IActionResult EditWarehouseItem([FromBody]AddEditWarehouseItemDetailsOb item)
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

            if (!roles.isAuthorized(_cookieValue, "warehouse", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                string result = warehouseController.EditWarehouseItem(item);

                logsController.AddLog(_cookieValue, $"Edycja przedmiotu: {item.name}");

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "WarehouseController", "EditWarehouseItem", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString()});
        }

        /// <summary>
        /// HTTP DELETE endpoint for removing a specific warehouse item identified by its ID.
        /// </summary>
        /// <param name="id">The ID of the warehouse item to be deleted.</param>
        /// <returns>JSON result containing the operation result status.</returns>
        [HttpDelete("item")]
        public IActionResult DeleteWarehouseItem(int id)
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

            if (!roles.isAuthorized(_cookieValue, "warehouse", "delete"))
                return new JsonResult(new { result = "no_permission" });
            
            try
            {
                logsController.AddLog(_cookieValue, $"Usunięcie przedmiotu: {_context.Warehouses.Where(w => w.Id == id).Select(w => w.Name).FirstOrDefault()}");

                string result = warehouseController.DeleteWarehouseItem(id);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "WarehouseController", "DeleteWarehouseItem", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP DELETE endpoint for removing multiple warehouse items identified by a list of IDs.
        /// </summary>
        /// <param name="idsList">A comma-separated list of IDs of the warehouse items to be deleted.</param>
        /// <returns>JSON result containing the operation result status.</returns>
        [HttpDelete("items")]
        public IActionResult DeleteWarehouseItems(string idsList)
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

            if (!roles.isAuthorized(_cookieValue, "warehouse", "delete"))
                return new JsonResult(new { result = "no_permission" });

            if (string.IsNullOrEmpty(idsList))
                return new JsonResult(new { result = "error" });

            List<int> ids = idsList.Split(',').Select(id => Convert.ToInt32(id)).ToList();

            try
            {
                logsController.AddLog(_cookieValue, $"Usunięcie przedmiotów: {_context.Warehouses.Where(w => ids.Contains((int)w.Id)).Select(w => w.Name)}");

                string result = warehouseController.DeleteWarehouseItems(ids);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "WarehouseController", "DeleteWarehouseItems", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString() });
        }
    }
}
