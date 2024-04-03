using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        MechAppContext _context;
        CheckCookieToken cookieToken;

        public UsersController (MechAppContext context)
        {
            _context = context;
            cookieToken = new CheckCookieToken(context);
        }

        /// <summary>
        /// Get Users
        /// Retrieves a list of users from the database based on the provided filters: name, department IDs, and role IDs.
        /// Filters users by name (if provided), and filters by their department and role associations.
        ///
        /// Parameters:
        /// - name: (Optional) A string to filter users by their name.
        /// - departmentIds: (Optional) A comma-separated string of department IDs to filter users.
        /// - rolesIds: (Optional) A comma-separated string of role IDs to filter users.
        ///
        /// Responses:
        /// - "done": Successfully retrieved the filtered list of users.
        /// - "error": Returned in case of a database exception during the operation.
        /// </summary>
        /// <param name="name">The filter string for user names.</param>
        /// <param name="departmentIds">The comma-separated list of department IDs.</param>
        /// <param name="rolesIds">The comma-separated list of role IDs.</param>
        /// <returns>JsonResult containing the list of filtered users or an error message</returns>
        [HttpGet]
        public IActionResult GetUsers(string? name, string? departmentIds, string? rolesIds)
        {
            StringBuilder resultBuilder = new StringBuilder();

            // Parse department and role IDs
            List<long>? depIds = !string.IsNullOrEmpty(departmentIds) ? departmentIds.Split(',').Select(id => Convert.ToInt64(id)).ToList() : new List<long>();
            List<long>? rolIds = !string.IsNullOrEmpty(rolesIds) ? rolesIds.Split(',').Select(id => Convert.ToInt64(id)).ToList() : new List<long>();

            try
            {
                List<User> users = new List<User>();

                // Query initialization
                IQueryable<User> query = _context.Users;

                // Filter by name if provided
                if (!string.IsNullOrEmpty(name))
                    query = query.Where(u => u.Name.ToLower().Contains(name.ToLower()));

                // Filter by department IDs if provided
                if (depIds.Count > 0)
                    query = query.Where(u => u.UsersDepartments.Any(ud => depIds.Contains(Convert.ToInt64(ud.DepartmentId))));

                // Filter by role IDs if provided
                if (rolIds.Count > 0)
                    query = query.Where(u => u.UsersRoles.Any(ur => rolIds.Contains(Convert.ToInt64(ur.RoleId))));

                query = query.Where(u => u.AppRole == "Employee");
                // Get filtered users
                users = query.ToList();

                var usersData = users.Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    lastname = u.Lastname,
                    email = u.Email,
                    phone = u.Phone,
                    departments = users.Where(u => u.UsersDepartments.Any(ur => ur.Id == u.Id)).ToList(),
                }).ToList();

                // Return the users list
                return new JsonResult(new
                {
                    result = "done",
                    users = usersData
                });
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "UsersController", "GetUsers", ex);
            }

            // Return the error result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Get User Details
        /// Retrieves detailed information about a specific user based on their ID. 
        /// Includes user's personal information, role associations, and other relevant details.
        /// Validates the user ID before fetching the data.
        ///
        /// Parameters:
        /// - id: The ID of the user to retrieve details for.
        ///
        /// Responses:
        /// - "done": Successfully retrieved the detailed information of the user.
        /// - "error": Returned if the user ID is invalid, the user does not exist, or in case of a database exception during the operation.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>JsonResult containing detailed information about the user or an error message</returns>
        [HttpGet("details/{id}")]
        public IActionResult GetUserDetails(int id)
        {
            StringBuilder resultBuilder = new StringBuilder();

            // Validate the user ID
            if (id == null || id == -1)
                return new JsonResult(new { result = "error" });

            try
            {
                // Retrieve the list of all roles
                var roles = _context.Roles.Select(r => new
                {
                    id = r.Id,
                    name = r.Name
                }).ToList();

                // Find the specific user
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                    return new JsonResult(new { result = "error" });

                // Return the detailed information of the user
                return new JsonResult(new
                {
                    result = "done",
                    id = user.Id,
                    name = user.Name,
                    lastname = user.Lastname,
                    email = user.Email,
                    nip = user.Nip,
                    phone = user.Phone,
                    postcode = user.Postcode,
                    city = user.City,
                    address = user.Address,
                    icon = user.Icon,
                    color = user.Color,
                    roles = roles,
                    userRoles = _context.Roles.Where(r => r.UsersRoles.Any(ur => ur.UserId == id)).Select(r => new
                    {
                        id = r.Id,
                        name = r.Name
                    })
                });
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "UsersController", "GetUserDetails", ex);
            }

            // Return the error result if any issue occurs
            return new JsonResult(new { result = "error" });
        }

    }
}
