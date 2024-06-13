using MechAppBackend.AppSettings;
using MechAppBackend.Data;
using MechAppBackend.features;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using OpenSearch.Client;
using System;
using System.Linq;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        MechAppContext _context;
        CheckCookieToken cookieToken;
        logs logsControlller;

        public UsersController (MechAppContext context)
        {
            _context = context;
            cookieToken = new CheckCookieToken(context);
            logsControlller = new logs(context);
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
        /// <param name="_pageSize">The number of logs to return per page.</param>
        /// <param name="_currentPage">The current page number.</param>
        /// <returns>JsonResult containing the list of filtered users or an error message</returns>
        [HttpGet]
        public IActionResult GetUsers(string? name, string? departmentIds, string? rolesIds, int _pageSize, int _currentPage)
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

            // Parse department and role IDs
            List<long>? depIds = !string.IsNullOrEmpty(departmentIds) ? departmentIds.Split(',').Select(id => Convert.ToInt64(id)).ToList() : new List<long>();
            List<long>? rolIds = !string.IsNullOrEmpty(rolesIds) ? rolesIds.Split(',').Select(id => Convert.ToInt64(id)).ToList() : new List<long>();

            int offset = ((_currentPage - 1) * _pageSize);

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

                query = query.Where(u => u.AppRole == "Employee" && u.IsDeleted != 1);
                // Get filtered users
                users = query.ToList();

                var usersData = users.Skip(offset).Take(_pageSize).Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    lastname = u.Lastname,
                    email = u.Email,
                    phone = u.Phone,
                    departments = users.Where(u => u.UsersDepartments.Any(ur => ur.Id == u.Id)).ToList(),
                }).ToList();

                logsControlller.AddLog(_cookieValue, "Pobranie listy użytkowników");

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

                logsControlller.AddLog(_cookieValue, $"Pobranie szczegółów użytkownika {user.Name} {user.Lastname}");

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
                    roles = _context.Roles.Where(r => r.Name.ToLower() != "root").Select(r => new
                    {
                        id = r.Id,
                        name = r.Name,
                        isAttachedToUser = _context.UsersRoles.Any(ur => ur.UserId == id && ur.RoleId == r.Id)
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

        /// <summary>
        /// Get User Department Links
        /// Retrieves information about the departments linked to a specific user based on their ID.
        /// Includes whether the user is attached to each department.
        /// Validates the user ID before fetching the data.
        ///
        /// Parameters:
        /// - id: The ID of the user to retrieve department links for.
        ///
        /// Responses:
        /// - "done": Successfully retrieved the list of departments with indication of user attachment.
        /// - "error": Returned if the user ID is invalid, the user does not exist, or in case of a database exception during the operation.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>JsonResult containing a list of departments with user attachment info or an error message</returns>
        [HttpGet("details/links/{id}")]
        public IActionResult GetUserLinks(int id)
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

            // Validate the user ID
            if (id == null || id == -1)
                return new JsonResult(new { result = "error" });

            try
            {
                // Find the specific user 
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                    return new JsonResult(new { result = "error" });

                // Retrieve and map department links
                var departments = _context.Departments.Select(d => new
                {
                    id = d.Id,
                    name = d.Name,
                    isAttachedToUser = _context.UsersDepartments.Any(ud => ud.UserId == id && ud.DepartmentId == d.Id)
                });

                logsControlller.AddLog(_cookieValue, $"Pobranie powizań użytkownika {user.Name} {user.Lastname}");

                // Return the department links information
                return new JsonResult(new
                {
                    result = "done",
                    departments = departments
                });
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "UsersController", "GetUserLinks", ex);
            }

            // Return the error result if any issue occurs
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Get User Data by Email
        /// Retrieves detailed information about a user based on their email address. 
        /// Validates the email address before fetching the data. 
        /// Includes personal and role information if the user is found.
        ///
        /// Parameters:
        /// - email: The email address of the user to retrieve data for.
        ///
        /// Responses:
        /// - "done": Successfully retrieved the detailed information of the user.
        /// - "error": Returned if the email address is empty, the user does not exist, or in case of a database exception during the operation.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>JsonResult containing detailed information about the user or an error message</returns>
        [HttpGet("details/email/{email}")]
        public IActionResult GetUserDataViaEmail(string email)
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

            // Validate the email address
            if (string.IsNullOrEmpty(email))
                return new JsonResult(new { result = "error" });

            try
            {
                // Find the user by email
                var user = _context.Users.FirstOrDefault(u => u.Email == email);

                if (user == null)
                    return new JsonResult(new { result = "error" });

                logsControlller.AddLog(_cookieValue, $"Pobranie szczegółów użytkownika {user.Name} {user.Lastname}");
                // Return detailed information of the user
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
                    color = user.Color,
                    roles = _context.Roles.Select(r => new
                    {
                        id = r.Id,
                        name = r.Name,
                        isAttachedToUser = _context.UsersRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == user.Id)
                    })
                });
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "UsersController", "GetUserDataViaEmail", ex);
            }
            // Return the error result if any issue occurs
            return new JsonResult(new {result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Add a New User
        /// Creates a new user in the system or updates an existing one if the user already exists. 
        /// Validates required fields like names, email, roles, phone number, and email format. 
        /// Sets default color if not provided and checks for unique email addresses.
        ///
        /// Parameters:
        /// - addUsersOb user: An object containing the new user's details.
        ///   - user.names: Full name of the user.
        ///   - user.email: Email address of the user.
        ///   - user.roles: List of role IDs assigned to the user.
        ///   - user.phone: (Optional) Phone number of the user.
        ///   - user.color: (Optional) Preferred color for the user.
        ///   - Additional fields like NIP, postcode, city, etc.
        ///
        /// Responses:
        /// - "user_created": Indicates the user was successfully created or updated.
        /// - Validation errors like "no_names", "no_email", "no_roles", "bad_phone_format", "bad_email_format", "exists".
        /// - "error": Returned in case of a database exception during the operation.
        /// </summary>
        /// <param name="user">The addUsersOb object containing new user information.</param>
        /// <returns>JsonResult indicating the outcome of the user creation or update operation</returns>
        [HttpPost("add")]
        public IActionResult AddUser([FromBody] addUsersOb user)
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

            // Validate required fields
            if (string.IsNullOrEmpty(user.names))
                return new JsonResult(new { result = "no_names" });
            if (string.IsNullOrEmpty(user.email))
                return new JsonResult(new { result = "no_email" });
            if (user.roles.Count == 0)
                return new JsonResult(new { result = "no_roles" });
            // Validate phone number format
            if (!string.IsNullOrEmpty(user.phone) && Validators.CheckPhone(user.phone, user.country.ToUpper()))
                return new JsonResult(new { result = "bad_phone_format" });
            // Set default color if not provided
            string _color = "#000000";

            if (!string.IsNullOrEmpty(user.color)) _color = user.color;
            // Validate email format
            if (!string.IsNullOrEmpty(user.email) && !Validators.ValidateEmail(user.email))
                return new JsonResult(new { result = "bad_email_format" });

            try
            {
                var count = _context.Users
                    .Count(c => c.AppRole == "Employee");

                if (count >= appdata.employeeAmount)
                    return new JsonResult(new { result = "max_employee_reached" });

                // Split names and assign appropriately
                string[] namesList = user.names.Trim().Split(' ');
                string _name = string.Empty;
                string _lastname = string.Empty;
                if (namesList.Length > 2)
                {
                    for (int i = 0; i < namesList.Length - 1; i++)
                    {
                        _name += namesList[i] + " ";
                    }
                    _name = _name.Trim();
                    _lastname = namesList[namesList.Length - 1];
                }
                else
                {
                    _name = namesList[0];
                    _lastname = namesList[1];
                }
                // Logic to create or update the user
                var userdb = _context.Users.FirstOrDefault(u => u.Email == user.email);

                if (userdb != null)
                {
                    if (userdb.AppRole == "Employee")
                        return new JsonResult(new { result = "exists" });

                    if (userdb.IsDeleted == 1)
                    {
                        userdb.IsDeleted = 0;
                    }
                    else
                    {
                        userdb.Name = _name;
                        userdb.Lastname = _lastname;
                        userdb.Email = user.email;
                        userdb.Nip = user.nip;
                        userdb.Phone = user.phone;
                        userdb.Postcode = user.postcode;
                        userdb.City = user.city;
                        userdb.Address = user.address;
                        userdb.Color = user.color;
                        userdb.AppRole = "Employee";
                    }

                    Sender.SendExistingAccountAddEmployeeEmail("Witamy w zespole!", _name, _lastname, user.email);
                }
                else
                {
                    string _salt = generators.generatePassword(10);
                    string _password = generators.generatePassword(15);
                    string _combinedPassword = _password + _salt;

                    _context.Users.Add(new User
                    {
                        Name = _name,
                        Lastname = _lastname,
                        Email = user.email,
                        Nip = user.nip,
                        Phone = user.phone,
                        Postcode = user.postcode,
                        City = user.city,
                        Address = user.address,
                        Color = user.color,
                        AppRole = "Employee",
                        IsFirstLogin = 1
                    });

                    Sender.SendAddEmployeeEmail("Witamy w zespole!", _name, _lastname, user.email, _password);
                }

                _context.SaveChanges();
                // Logic to add roles to the user
                if (_context.UsersRoles.Any(ur => ur.UserId == userdb.Id))
                {
                    var existingRolesConnections = _context.UsersRoles
                        .Where(ur => ur.UserId == userdb.Id)
                        .Select(ur => (int?)ur.RoleId)
                        .ToList();

                    var rolesIdsAsNullable = user.roles.Select(id => (int?)id).ToList();

                    var rolesToAdd = rolesIdsAsNullable.Except(existingRolesConnections).ToList();

                    foreach (var role in rolesToAdd)
                    {
                        _context.UsersRoles.Add(new UsersRole
                        {
                            UserId = userdb.Id,
                            RoleId = role
                        });
                    }

                    var rolesToRemove = existingRolesConnections.Where(ur => !rolesIdsAsNullable.Contains(ur)).ToList();

                    foreach (var role in rolesToRemove)
                    {
                        _context.UsersRoles.Remove(_context.UsersRoles.FirstOrDefault(ur => ur.UserId == userdb.Id && ur.RoleId == role));
                    }
                }
                else
                {
                    foreach (var role in user.roles)
                    {
                        _context.UsersRoles.Add(new UsersRole
                        {
                            RoleId = role,
                            UserId = userdb.Id
                        });
                    }
                }

                _context.SaveChanges();

                logsControlller.AddLog(_cookieValue, $"Dodanie użytkownika {user.names}");

                resultBuilder.Append("user_created");
            }
            catch (MySqlException ex) // Log exception and return 'error'
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "UsersControllers", "AddUser", ex);
            }

            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Edit an Existing User
        /// Updates an existing user in the system with the provided details. 
        /// Validates the user ID and required fields such as names, email, roles, and optionally phone number. 
        /// Checks for unique email addresses and validates the format.
        ///
        /// Parameters:
        /// - editUsersOb user: An object containing the user's updated details.
        ///   - user.id: The ID of the user to be edited.
        ///   - user.names: Updated full name of the user.
        ///   - user.email: Updated email address of the user.
        ///   - user.oldEmail: Previous email address of the user for verification.
        ///   - user.roles: List of updated role IDs assigned to the user.
        ///   - user.phone: (Optional) Updated phone number of the user.
        ///   - user.color: (Optional) Updated preferred color for the user.
        ///   - Additional fields like NIP, postcode, city, etc.
        ///
        /// Responses:
        /// - "user_edited": Indicates the user was successfully edited.
        /// - "error", "no_names", "no_email", "no_old_email", "no_roles", "bad_phone_format", "bad_email_format", "email_exists": 
        ///   Returned if there are validation errors in the input.
        /// </summary>
        /// <param name="user">The editUsersOb object containing user edit information.</param>
        /// <returns>JsonResult indicating the outcome of the user edit operation</returns>
        [HttpPost("edit")]
        public IActionResult EditUser([FromBody] editUsersOb user)
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

            if (user.id == null || user.id == -1)
                return new JsonResult(new { result = "error" });

            // Validate required fields
            if (string.IsNullOrEmpty(user.names))
                return new JsonResult(new { result = "no_names" });
            if (string.IsNullOrEmpty(user.email))
                return new JsonResult(new { result = "no_email" });
            if (string.IsNullOrEmpty(user.oldEmail))
                return new JsonResult(new { result = "no_old_email" });
            if (user.roles.Count == 0)
                return new JsonResult(new { result = "no_roles" });
            // Validate phone number format
            if (!string.IsNullOrEmpty(user.phone) && Validators.CheckPhone(user.phone, user.country.ToUpper()))
                return new JsonResult(new { result = "bad_phone_format" });
            // Set default color if not provided
            string _color = "#000000";

            if (!string.IsNullOrEmpty(user.color)) _color = user.color;
            // Validate email format
            if (!string.IsNullOrEmpty(user.email) && !Validators.ValidateEmail(user.email))
                return new JsonResult(new { result = "bad_email_format" });

            try
            {
                // Find the user to be edited
                var userdb = _context.Users.FirstOrDefault(u => u.Id == user.id);

                if (userdb == null)
                    return new JsonResult(new { result = "error" });
                // Logic to update user information
                string[] namesList = user.names.Trim().Split(' ');
                string _name = string.Empty;
                string _lastname = string.Empty;
                if (namesList.Length > 2)
                {
                    for (int i = 0; i < namesList.Length - 1; i++)
                    {
                        _name += namesList[i] + " ";
                    }
                    _name = _name.Trim();
                    _lastname = namesList[namesList.Length - 1];
                }
                else
                {
                    _name = namesList[0];
                    _lastname = namesList[1];
                }

                var checkEmail = _context.Users.FirstOrDefault(u => u.Email == user.email);

                if (checkEmail != null && user.oldEmail != user.email)
                    return new JsonResult(new { result = "email_exists" });

                userdb.Name = _name;
                userdb.Lastname = _lastname;
                userdb.Email = user.email;
                userdb.Nip = user.nip;
                userdb.Phone = user.phone;
                userdb.Postcode = user.postcode;
                userdb.City = user.city;
                userdb.Address = user.address;
                userdb.Color = user.color;
                // Logic to handle roles connections
                var existingRolesConnections = _context.UsersRoles
                    .Where(ur => ur.UserId == userdb.Id)
                    .Select(ur => (int?)ur.RoleId)
                    .ToList();


                var rolesIdsAsNullable = user.roles.Select(id => (int?)id).ToList();

                var rolesToAdd = rolesIdsAsNullable.Except(existingRolesConnections).ToList();

                foreach(var role in rolesToAdd)
                {
                    _context.UsersRoles.Add(new UsersRole
                    {
                        UserId = userdb.Id,
                        RoleId = role
                    });
                }

                var rolesToRemove = existingRolesConnections.Where(ur => !rolesIdsAsNullable.Contains(ur)).ToList();

                foreach(var role in rolesToRemove)
                {
                    _context.UsersRoles.Remove(_context.UsersRoles.FirstOrDefault(ur => ur.UserId == userdb.Id && ur.RoleId == role));
                }

                logsControlller.AddLog(_cookieValue, $"Edycja użytkownika {userdb.Name} {userdb.Lastname}");
                _context.SaveChanges();

                resultBuilder.Append("user_edited");
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "UsersController", "EditUser", ex);
            }
            // Return the final result
            return new JsonResult(new {result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Add or Remove Department Links for a User
        /// Updates the department associations for a specific user based on the provided department IDs. 
        /// Adds new department links and removes existing ones that are not in the provided list.
        /// Validates the user ID before processing the department links.
        ///
        /// Parameters:
        /// - linksUserOb links: An object containing the user ID and a list of department IDs for associations.
        ///   - links.id: The ID of the user to update department links for.
        ///   - links.departments: A list of department IDs to be associated with the user.
        ///
        /// Responses:
        /// - "departments_added": Indicates that the department links for the user were successfully updated.
        /// - "error": Returned if the user ID is invalid or in case of a database exception during the operation.
        /// </summary>
        /// <param name="links">The linksUserOb object containing department linking information.</param>
        /// <returns>JsonResult indicating the outcome of the department link update operation</returns>
        [HttpPost("details/links")]
        public IActionResult AddRemoveDepartmentLinks([FromBody] linksUserOb links)
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

            if (links.id == null || links.id == -1)
                return new JsonResult(new { result = "error" });
            // Validate the user ID
            try
            {
                // Retrieve current department links for the user
                var currentDepartmentsInUsers = _context.UsersDepartments
                    .Where(ud => ud.UserId == links.id)
                    .Select(ud => (int?)ud.DepartmentId)
                    .ToList();
                // Process department IDs for linking
                var departmentIdsAsNullable = links.departments.Select(id => (int?)id).ToList();
                // Determine which departments to add
                var departmentsToAdd = departmentIdsAsNullable.Except(currentDepartmentsInUsers).ToList();

                foreach (var department in departmentsToAdd)
                {
                    _context.UsersDepartments.Add(new UsersDepartment
                    {
                        UserId = links.id,
                        DepartmentId = department
                    });
                }
                // Determine which departments to remove
                var departmentsToDelete = currentDepartmentsInUsers.Where(ud => !departmentIdsAsNullable.Contains(ud)).ToList();

                foreach(var department in departmentsToDelete)
                {
                    _context.UsersDepartments.Remove(_context.UsersDepartments.FirstOrDefault(ud => ud.DepartmentId == department && ud.UserId == links.id));
                }
                // Save changes to the database
                _context.SaveChanges();

                logsControlller.AddLog(_cookieValue, $"Edycja powiązań użytkownika {_context.Users.Where(u => u.Id == links.id).Select(u => u.Name).FirstOrDefault()} " +
                    $"{_context.Users.Where(u => u.Id == links.id).Select(u => u.Lastname).FirstOrDefault()}");
                // Indicate successful update of department links
                resultBuilder.Append("departments_added");
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "UsersController", "AddRemoveDepartmentLinks", ex);
            }
            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Delete a User
        /// Soft deletes a user from the system based on their ID. 
        /// Marks the user as deleted rather than removing them from the database.
        /// Also removes the user's role and department associations.
        /// Validates the user ID before attempting the deletion.
        ///
        /// Parameters:
        /// - id: The ID of the user to be deleted.
        ///
        /// Responses:
        /// - "user_deleted": Indicates the user was successfully marked as deleted.
        /// - "error": Returned if the user ID is invalid, the user does not exist, or in case of a database exception during the operation.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>JsonResult indicating the outcome of the user deletion operation</returns>
        [HttpDelete("delete/user/{id}")]
        public IActionResult DeleteUser(int id)
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

            // Validate the user ID
            if (id == null || id == -1)
                return new JsonResult(new { result = "error" });

            try
            {
                // Find the specific user
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                    return new JsonResult(new { result = "error" });

                // Mark the user as deleted
                user.IsDeleted = 1;
                logsControlller.AddLog(_cookieValue, $"Usunięcie użytkownika {user.Name} {user.Lastname}");
                // Remove user's role and department associations
                _context.UsersRoles.RemoveRange(_context.UsersRoles.Where(ur => ur.UserId == id));
                _context.UsersDepartments.RemoveRange(_context.UsersDepartments.Where(ur => ur.UserId == id));

                // Save changes to the database
                _context.SaveChanges();

                // Indicate successful user deletion
                resultBuilder.Append("user_deleted");
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "UsersController", "DeleteUser", ex);
            }

            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }


        /// <summary>
        /// Delete Multiple Users
        /// Soft deletes multiple users from the system based on a list of their IDs. 
        /// Marks the users as deleted rather than removing them completely from the database.
        /// Also removes the users' roles and department associations.
        /// Validates the list of user IDs before proceeding.
        ///
        /// Parameters:
        /// - idsList: A comma-separated string of user IDs to be deleted.
        ///
        /// Responses:
        /// - "users_deleted": Indicates that the users were successfully marked as deleted.
        /// - "error": Returned if there are any issues with parsing the IDs, if any of the users do not exist, 
        ///   or in case of a database exception during the operation.
        /// </summary>
        /// <param name="idsList">A string containing the comma-separated list of user IDs.</param>
        /// <returns>JsonResult indicating the outcome of the users deletion operation</returns>
        [HttpDelete("delete/users/{idsList}")]
        public IActionResult DeleteUsers(string idsList)
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

            // Parse the list of IDs and convert to integers
            List<int> ids = idsList.Split(',').Select(id => Convert.ToInt32(id)).ToList();

            try
            {
                // Retrieve the list of users to be deleted
                var usersList = _context.Users.Where(u => ids.Contains(Convert.ToInt32(u.Id))).ToList();

                // Mark each user as deleted
                foreach (var user in usersList)
                {
                    user.IsDeleted = 1;
                }
                logsControlller.AddLog(_cookieValue, "Usunięcie użytkowników");
                // Remove users' roles and department associations
                _context.UsersRoles.RemoveRange(_context.UsersRoles.Where(ur => ids.Contains(Convert.ToInt32(ur.UserId))));
                _context.UsersDepartments.RemoveRange(_context.UsersDepartments.Where(ud => ids.Contains(Convert.ToInt32(ud.UserId))));

                // Save changes to the database
                _context.SaveChanges();

                // Indicate successful deletion of users
                resultBuilder.Append("users_deleted");
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "UsersController", "DeleteUsers", ex);
            }

            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

    }

    public class addUsersOb
    {
        public string? names { get; set; }
        public string? email { get; set; }
        public string? nip { get; set; }
        public string? phone { get; set; }
        public string? country { get; set; }
        public string? postcode { get; set; }
        public string? city { get; set; }
        public string? address { get; set; }
        public string? color { get; set; }
        public List<int>? roles { get; set; }
    }

    public class editUsersOb
    {
        public int? id { get; set; }
        public string? names { get; set; }
        public string? email { get; set; }
        public string? oldEmail { get; set; }
        public string? nip { get; set; }
        public string? phone { get; set; }
        public string? country { get; set; }
        public string? postcode { get; set; }
        public string? city { get; set; }
        public string? address { get; set; }
        public string? color { get; set; }
        public List<int>? roles { get; set; }
    }

    public class linksUserOb
    {
        public int? id { get; set; }
        public List<int>? departments { get; set; }
    }
}
