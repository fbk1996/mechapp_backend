using MechAppBackend.Data;
using MechAppBackend.features;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        MechAppContext _context;
        CheckCookieToken cookieToken;
        logs logsController;
        CheckRoles roles;

        public RolesController (MechAppContext context)
        {
            _context = context;
            cookieToken = new CheckCookieToken(context);
            logsController = new logs(context);
            roles = new CheckRoles(context);
        }

        /// <summary>
        /// Retrieve User Roles
        /// Fetches a list of user roles from the database, including the count of members in each role.
        /// The method returns the roles and their member counts in a structured JSON format.
        ///
        /// Responses:
        /// - "done": Successfully retrieved the list of roles along with their member counts.
        /// - "error": Returned in case of a database exception during the operation.
        /// <param name="_pageSize">The number of logs to return per page.</param>
        /// <param name="_currentPage">The current page number.</param>
        /// </summary>
        /// <returns>JsonResult containing a list of roles and their member counts, or an error message</returns>
        [HttpGet]
        public IActionResult GetRoles(int _pageSize, int _currentPage)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;

            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });

            DateTime expireCookie = endCookieDate.GetEndCookieDate();

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "roles", "view"))
                return new JsonResult(new { result = "no_permission" });

            int offset = ((_currentPage - 1) * _pageSize);

            try
            {
                // Query the database to get a list of roles along with their member counts
                var roles = _context.Roles
                    .Where(r => r.Name.ToLower() != "root")
                    .Skip(offset)
                    .Take(_pageSize)
                    .Select(item => new
                    {
                        id = item.Id,
                        name = item.Name,
                        membersCount = _context.UsersRoles.Count(ur => ur.RoleId == item.Id),
                        count = _context.Roles.Count()
                    }).ToList();

                logsController.AddLog(_cookieValue, "Pobranie listy ról");

                // Return the roles and their details in JSON format
                return new JsonResult(new
                {
                    result = "done",
                    roles = roles
                });
            }
            catch (MySqlException ex)
            {
                // In case of a database exception, log the error and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "RolesController", "GetRoles", ex);
            }

            // Return the error result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Retrieve Role Details
        /// Fetches details of a specific user role based on its ID, including the role name and permissions.
        /// Permissions are structured into categories with boolean values indicating if a permission is granted.
        ///
        /// Parameters:
        /// - id: The ID of the role to retrieve details for.
        ///
        /// Responses:
        /// - "done": Successfully retrieved the role details including its permissions.
        /// - "error": Returned if the role does not exist or in case of a database exception during the operation.
        /// </summary>
        /// <param name="id">The ID of the role.</param>
        /// <returns>JsonResult containing role details and permissions, or an error message</returns>
        [HttpGet("details/{id}")]
        public IActionResult GetRoleDetails(int id)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;

            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });

            DateTime expireCookie = endCookieDate.GetEndCookieDate();

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "roles", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Retrieve the role based on the provided ID
                var role = _context.Roles.FirstOrDefault(r => r.Id == id);
                // Return error if the role is not found
                if (role == null) return new JsonResult(new { result = "error" });
                // Initialize permission structure
                var permissionStructure = new Dictionary<string, Dictionary<string, bool>>
                {
                    {"profile", new Dictionary<string, bool> { {"edit", false}, { "passChange", false}, {"emailChange", false }, {"delAccount", false} } },
                    {"departments", new Dictionary<string, bool> { {"view", false}, {"edit", false}, {"add", false}, {"delete", false}, {"links", false} } },
                    {"workPosition", new Dictionary<string, bool> {{"view", false}, {"edit", false}, {"add", false}, {"delete", false} } },
                    {"airConditioningReports", new Dictionary<string, bool> {{"view", false}, {"edit", false}, {"add", false}, {"delete", false} } },
                    {"users", new Dictionary<string, bool> { { "view", false }, { "edit", false}, { "add", false}, {"delete", false}, {"links", false} } },
                    {"roles", new Dictionary<string, bool> {{"view", false}, { "edit", false}, { "add", false }, {"delete", false} } },
                    {"requests", new Dictionary<string, bool> {{"view", false}, { "edit", false}, { "add", false}, { "delete", false} } },
                    {"schedule", new Dictionary<string, bool> {{"view", false}, { "edit", false}, { "add", false}, { "delete", false} } },
                    {"timetable", new Dictionary<string, bool> {{"view", false}, { "edit", false}, { "add", false}, { "delete", false} } },
                    {"orders", new Dictionary<string, bool> {{"view", false}, { "edit", false}, { "add", false}, { "delete", false} } },
                    {"clients", new Dictionary<string, bool> {{"view", false}, { "edit", false}, { "add", false}, { "delete", false} } },
                    {"services", new Dictionary<string, bool> {{"view", false}, { "edit", false }, { "add", false}, { "delete", false} } },
                    {"warehouse", new Dictionary<string, bool> {{"view", false}, { "edit", false}, { "add", false}, { "delete", false} } },
                    {"demands", new Dictionary<string, bool> {{"view", false}, { "edit", false}, { "add", false}, { "delete", false} } },
                    {"prices", new Dictionary<string, bool> {{"view", false}, { "edit", false} } },
                    {"tickets", new Dictionary<string, bool> {{"view", false}, { "add", false} } },
                    {"logs", new Dictionary<string, bool> {{"view", false}} }
                };
                // Deserialize role permissions from the database
                var permissions = JsonConvert.DeserializeObject<List<string>>(role.Permissions);
                // Update permission structure based on role's permissions
                foreach (var permission in permissions)
                {
                    string[] parts = permission.Split('_');

                    if (parts.Length == 2 && permissionStructure.ContainsKey(parts[0]))
                    {
                        permissionStructure[parts[0]][parts[1]] = true;
                    }
                }

                logsController.AddLog(_cookieValue, $"Pobranie szczegółów roli {role.Name}");

                // Return role details and permissions in JSON format
                return new JsonResult(new
                {
                    result = "done",
                    id = role.Id,
                    name = role.Name,
                    permissions = permissionStructure
                });
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "RolesController", "GetRoleDetails", ex);
            }
            // Return the error result
            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// Add New Role
        /// Adds a new role to the database with specified name and permissions.
        /// Validates the role name for length and uniqueness, and ensures that permissions are provided.
        ///
        /// Parameters:
        /// - addRoleOb role: An object containing the new role's name and permissions.
        ///   - role.name: The name of the new role.
        ///   - role.permissions: A list of permissions associated with the role.
        ///
        /// Responses:
        /// - "role_created": Indicates successful creation of the role.
        /// - "no_name": Returned if the role name is empty.
        /// - "no_permissions": Returned if no permissions are provided for the role.
        /// - "name_too_long": Returned if the role name exceeds the maximum length limit.
        /// - "exists": Returned if a role with the same name already exists.
        /// - "error": Returned in case of a database exception during the operation.
        /// </summary>
        /// <param name="role">The addRoleOb object containing role creation information.</param>
        /// <returns>JsonResult indicating the outcome of the role creation operation</returns>
        [HttpPost("add")]
        public IActionResult AddRole([FromBody] addRoleOb role)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;

            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });

            DateTime expireCookie = endCookieDate.GetEndCookieDate();

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            // Validate role name
            if (string.IsNullOrEmpty(role.name))
                return new JsonResult(new { result = "no_name" });

            // Validate presence of permissions
            if (role.permissions.Count == 0)
                return new JsonResult(new { result = "no_permissions" });

            // Validate length of role name
            if (!Validators.CheckLength(role.name, 50))
                return new JsonResult(new { result = "name_too_long" });

            if (!roles.isAuthorized(_cookieValue, "roles", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Check if a role with the same name already exists
                var roledb = _context.Roles.FirstOrDefault(r => r.Name.ToLower() == role.name.Trim().ToLower());

                if (roledb != null)
                    return new JsonResult(new { result = "exists" });

                // Create and add the new role to the database
                Role newRole = new Role
                {
                    Name = role.name.Trim(),
                    Permissions = JsonConvert.SerializeObject(role.permissions)
                };

                logsController.AddLog(_cookieValue, $"Dodanie roli {role.name}");

                _context.Roles.Add(newRole);
                _context.SaveChanges();

                // Indicate successful role creation
                resultBuilder.Append("role_created");
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "RolesController", "AddRole", ex);
            }

            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Edit an Existing Role
        /// Allows editing the permissions of an existing role in the database.
        /// The method validates the presence of the role ID and permissions list, then updates the role's permissions.
        ///
        /// Parameters:
        /// - editRoleOb role: An object containing the role ID and the new list of permissions.
        ///   - role.id: The ID of the role to be edited.
        ///   - role.permissions: The new list of permissions for the role.
        ///
        /// Responses:
        /// - "role_edited": Indicates the role was successfully edited.
        /// - "no_permissions": Returned if no permissions are provided for the role.
        /// - "error": Returned if the role ID is invalid, the role does not exist, or in case of a database exception during the operation.
        /// </summary>
        /// <param name="role">The editRoleOb object containing role edit information.</param>
        /// <returns>JsonResult indicating the outcome of the role edit operation</returns>
        [HttpPost("edit")]
        public IActionResult EditRole([FromBody] editRoleOb role)
        {
            StringBuilder resultBuilder = new StringBuilder();

            // Validate the presence of permissions
            if (role.permissions.Count == 0)
                return new JsonResult(new { result = "no_permissions" });

            string _cookieValue = string.Empty;

            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });

            DateTime expireCookie = endCookieDate.GetEndCookieDate();

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            // Validate the role ID
            if (role.id == null || role.id == -1)
                return new JsonResult(new { result = "error" });

            if (!roles.isAuthorized(_cookieValue, "roles", "edit"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                // Retrieve the role from the database
                var roleDb = _context.Roles.FirstOrDefault(r => r.Id == role.id);

                // Return error if the role does not exist
                if (roleDb == null)
                    return new JsonResult(new { result = "error" });

                // Update the role's permissions
                roleDb.Permissions = JsonConvert.SerializeObject(role.permissions);
                _context.SaveChanges();
                logsController.AddLog(_cookieValue, $"Edycja roli {roleDb.Name}");
                // Indicate successful role editing
                resultBuilder.Append("role_edited");
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "RolesController", "EditRole", ex);
            }

            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Delete a Role
        /// Removes a role from the database based on its ID. 
        /// This operation also removes all associations of users with this role.
        /// Validates the role ID before attempting deletion.
        ///
        /// Parameters:
        /// - id: The ID of the role to be deleted.
        ///
        /// Responses:
        /// - "role_deleted": Indicates the role was successfully deleted.
        /// - "error": Returned if the role ID is invalid or in case of a database exception during the operation.
        /// </summary>
        /// <param name="id">The ID of the role to delete.</param>
        /// <returns>JsonResult indicating the outcome of the role deletion operation</returns>
        [HttpDelete("delete/role/{id}")]
        public IActionResult DeleteRole(int id)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;

            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });

            DateTime expireCookie = endCookieDate.GetEndCookieDate();

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            if (!roles.isAuthorized(_cookieValue, "roles", "delete"))
                return new JsonResult(new { result = "no_permission" });

            // Validate the role ID
            if (id == null || id == -1)
                return new JsonResult(new { result = "error" });

            try
            {
                logsController.AddLog(_cookieValue, $"Usunięcie roli {_context.Roles.Where(r => r.Id == id).Select(r => r.Name).FirstOrDefault()}");

                // Remove the role and its associations with users from the database
                _context.Roles.RemoveRange(_context.Roles.Where(r => r.Id == id));
                _context.UsersRoles.RemoveRange(_context.UsersRoles.Where(ur => ur.RoleId == id));
                _context.SaveChanges();

                // Indicate successful role deletion
                resultBuilder.Append("role_deleted");
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "RolesController", "DeleteRole", ex);
            }

            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Delete Multiple Roles
        /// Removes multiple roles from the database based on a list of their IDs. 
        /// Also removes all associations of these roles with users. Validates the list of IDs before proceeding.
        ///
        /// Parameters:
        /// - ids: A list of IDs of the roles to be deleted.
        ///
        /// Responses:
        /// - "roles_deleted": Indicates that the roles were successfully deleted.
        /// - "error": Returned if the list of IDs is empty or in case of a database exception during the operation.
        /// </summary>
        /// <param name="ids">The list of role IDs to delete.</param>
        /// <returns>JsonResult indicating the outcome of the role deletion operation</returns>
        [HttpDelete("delete/roles/{idsList}")]
        public IActionResult DeleteRoles(string idsList)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;

            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            if (!cookieToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });

            DateTime expireCookie = endCookieDate.GetEndCookieDate();

            CookieOptions cookieOptions = new CookieOptions
            {
                Expires = expireCookie
            };

            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            List<int> ids = idsList.Split(',').Select(id => Convert.ToInt32(id)).ToList();
            // Validate the list of IDs
            if (ids.Count == 0)
                return new JsonResult(new { result = "error" });

            if (!roles.isAuthorized(_cookieValue, "roles", "delete"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, $"Usunięcie ról: {_context.Roles.Where(r => ids.Contains((int)r.Id)).Select(r => r.Name).ToString()}");

                // Remove the roles and their associations with users from the database
                _context.Roles.RemoveRange(_context.Roles.Where(r => ids.Contains(Convert.ToInt32(r.Id))));
                _context.UsersRoles.RemoveRange(_context.UsersRoles.Where(ur => ids.Contains(Convert.ToInt32(ur.Id))));
                _context.SaveChanges();

                // Indicate successful deletion of roles
                resultBuilder.Append("roles_deleted");
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "RolesController", "DeleteRoles", ex);
            }

            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

    }

    public class addRoleOb
    {
        public string? name { get; set; }
        public List<string>? permissions { get; set; }
    }

    public class editRoleOb
    {
        public int? id { get; set; }
        public List<string>? permissions { get; set; }
    }
}
