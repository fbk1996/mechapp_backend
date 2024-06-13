using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using MechAppBackend.Security;
using MechAppBackend.features;

namespace MechAppBackend.Controllers
{
    [Route("api/departments")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        MechAppContext _context;
        CheckCookieToken cookieToken;
        departments departmentsController;
        logs logsController;

        public DepartmentsController(MechAppContext context)
        {
            _context = context;
            cookieToken = new CheckCookieToken(context);
            departmentsController = new departments(context);
            logsController = new logs(context);
        }

        /// <summary>
        /// Retrieve Departments Information
        /// Fetches a list of departments from the database, including detailed information about each department such as name, 
        /// description, address, postcode, city, and the count of members in each department.
        /// The method returns this information in a structured JSON format.
        ///
        /// Responses:
        /// - "done": Successfully retrieved the list of departments along with their detailed information.
        /// - "error": Returned in case of a database exception during the operation.
        /// <param name="_pageSize">The number of logs to return per page.</param>
        /// <param name="_currentPage">The current page number.</param>
        /// </summary>
        /// <returns>JsonResult containing a list of departments and their details, or an error message</returns>
        [HttpGet]
        public IActionResult GetDepartments(int _pageSize, int _currentPage)
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

            int offset = ((_currentPage - 1) * _pageSize);

            try
            {
                // Query the database to get a list of departments along with their detailed information
                var departmentsList = _context.Departments
                    .Skip(offset)
                    .Take(_pageSize)
                    .Select(d => new departmentOb
                    {
                        id = Convert.ToInt32(d.Id),
                        name = d.Name,
                        description = d.Description,
                        address = d.Address,
                        postcode = d.Postcode,
                        city = d.City
                    }).ToList();

                logsController.AddLog(_cookieValue, "Pobranie listy oddziałów");

                // Return the departments and their details in JSON format
                return new JsonResult(new
                {
                    result = "done",
                    departments = departmentsList
                });
            }
            catch (MySqlException ex)
            {
                // In case of a database exception, log the error and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DepartmentsController", "GetDepartments", ex);
            }

            // Return the error result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Retrieve Detailed Information for a Specific Department
        /// Fetches detailed information about a department based on its ID. Validates the ID and checks if the department exists.
        /// If successful, returns details like name, address, postcode, and city of the department.
        ///
        /// Parameters:
        /// - id: The ID of the department for which detailed information is requested.
        ///
        /// Responses:
        /// - "done": Successfully retrieved the department details.
        /// - "not_found": Returned if no department is associated with the provided ID.
        /// - "error": Returned if the ID is invalid or in case of a database exception during the operation.
        /// </summary>
        /// <param name="id">The ID of the department.</param>
        /// <returns>JsonResult containing department details or an error message</returns>
        [HttpGet("details/{id}")]
        public IActionResult GetDepartmentDetails(int id)
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

            // Validate the department ID
            if (id == null || id == -1)
                return new JsonResult(new { result = "error" });

            try
            {
                // Retrieve the department based on the provided ID
                var department = _context.Departments.FirstOrDefault(d => d.Id == id);

                // Return 'not_found' if the department does not exist
                if (department == null)
                    return new JsonResult(new { result = "not_found" });

                logsController.AddLog(_cookieValue, $"Pobranie szczegółów oddziału {department.Name}");

                // Return the department details in JSON format
                return new JsonResult(new
                {
                    result = "done",
                    id = department.Id,
                    name = department.Name,
                    address = department.Address,
                    postcode = department.Postcode,
                    city = department.City
                });
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DepartmentsController", "GetDepartmentDetails", ex);
            }

            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Retrieve Users Associated with a Department
        /// Fetches a list of users along with their basic information and indicates whether they are attached to a specific department.
        /// Validates the department ID before fetching the data.
        ///
        /// Parameters:
        /// - id: The ID of the department to retrieve users for.
        ///
        /// Responses:
        /// - "done": Successfully retrieved the list of users associated with the specified department.
        /// - "error": Returned if the department ID is invalid or in case of a database exception during the operation.
        /// </summary>
        /// <param name="id">The ID of the department.</param>
        /// <returns>JsonResult containing a list of users associated with the department, or an error message</returns>
        [HttpGet("details/users/{id}")]
        public IActionResult GetDepartmentUsers(int id)
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

            // Validate the department ID
            if (id == null || id == -1)
                return new JsonResult(new { result = "error" });

            try
            {
                // Retrieve the list of users with their attachment status to the department
                var users = _context.Users
                    .Select(user => new
                    {
                        id = user.Id,
                        name = user.Name,
                        lastname = user.Lastname,
                        isAttachedToDepartment = _context.UsersDepartments
                                                        .Any(ud => ud.UserId == user.Id && ud.DepartmentId == id)
                    }).ToList();

                logsController.AddLog(_cookieValue, $"Pobranie listy użytkowników należących do oddziału {_context.Departments.Where(d => d.Id == id).Select(d => d.Name).FirstOrDefault()}");

                // Return the list of users
                return new JsonResult(new
                {
                    result = "done",
                    users = users
                });
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DepartmentsController", "GetDepartmentsUsers", ex);
            }

            // Return the error result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Add a New Department
        /// Creates a new department in the database with specified details. 
        /// Validates the presence of all required fields: name, description, address, postcode, and city. 
        /// Checks for the uniqueness of the department name before creation.
        ///
        /// Parameters:
        /// - addDepartmentOb dep: An object containing the new department's details.
        ///   - dep.name: The name of the department.
        ///   - dep.description: The description of the department.
        ///   - dep.address: The address of the department.
        ///   - dep.postcode: The postcode of the department.
        ///   - dep.city: The city where the department is located.
        ///
        /// Responses:
        /// - "department_created": Indicates the department was successfully created.
        /// - "no_name", "no_description", "no_address", "no_postcode", "no_city": Returned if any required field is missing.
        /// - "exists": Returned if a department with the same name already exists.
        /// - "error": Returned in case of a database exception during the operation.
        /// </summary>
        /// <param name="dep">The addDepartmentOb object containing department creation information.</param>
        /// <returns>JsonResult indicating the outcome of the department creation operation</returns>
        [HttpPost("add")]
        public IActionResult AddDepartment([FromBody] addDepartmentOb dep)
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
            if (string.IsNullOrEmpty(dep.name))
                return new JsonResult(new { result = "no_name" });
            if (string.IsNullOrEmpty(dep.description))
                return new JsonResult(new { result = "no_description" });
            if (string.IsNullOrEmpty(dep.address))
                return new JsonResult(new { result = "no_address" });
            if (string.IsNullOrEmpty(dep.postcode))
                return new JsonResult(new { result = "no_postcode" });
            if (string.IsNullOrEmpty(dep.city))
                return new JsonResult(new { result = "no_city" });

            try
            {
                // Check for existing department with the same name
                var department = _context.Departments.FirstOrDefault(d => d.Name.ToLower() == dep.name.Trim().ToLower());

                if (department != null)
                    return new JsonResult(new { result = "exists" });

                // Create and add the new department to the database
                Department newDepartment = new Department
                {
                    Name = dep.name.Trim(),
                    Description = dep.description.Trim(),
                    Address = dep.address.Trim(),
                    Postcode = dep.postcode.Trim(),
                    City = dep.city.Trim()
                };

                logsController.AddLog(_cookieValue, $"Dodanie oddziału {dep.name}");

                _context.Departments.Add(newDepartment);
                _context.SaveChanges();

                // Indicate successful department creation
                resultBuilder.Append("department_created");
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DepartmentsController", "AddDepartment", ex);
            }

            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Edit an Existing Department
        /// Updates the details of an existing department in the database.
        /// Validates the presence of the department ID and checks if all required fields (name, description, address, postcode, city) are provided.
        /// Ensures the department exists before attempting to edit.
        ///
        /// Parameters:
        /// - editDepartmentOb dep: An object containing the updated details of the department.
        ///   - dep.id: The ID of the department to be edited.
        ///   - dep.name: The updated name of the department.
        ///   - dep.description: The updated description of the department.
        ///   - dep.address: The updated address of the department.
        ///   - dep.postcode: The updated postcode of the department.
        ///   - dep.city: The updated city where the department is located.
        ///
        /// Responses:
        /// - "department_edited": Indicates the department was successfully edited.
        /// - "error": Returned if the department ID is invalid, the department does not exist, or in case of a database exception during the operation.
        /// - "no_name", "no_description", "no_address", "no_postcode", "no_city": Returned if any required field is missing.
        /// </summary>
        /// <param name="dep">The editDepartmentOb object containing department edit information.</param>
        /// <returns>JsonResult indicating the outcome of the department edit operation</returns>
        [HttpPost("edit")]
        public IActionResult EditDepartment([FromBody] editDepartmentOb dep)
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

            // Validate the department ID
            if (dep.id == null || dep.id == -1)
                return new JsonResult(new { result = "error" });

            // Validate required fields
            if (string.IsNullOrEmpty(dep.name))
                return new JsonResult(new { result = "no_name" });
            if (string.IsNullOrEmpty(dep.description))
                return new JsonResult(new { result = "no_description" });
            if (string.IsNullOrEmpty(dep.address))
                return new JsonResult(new { result = "no_address" });
            if (string.IsNullOrEmpty(dep.postcode))
                return new JsonResult(new { result = "no_postcode" });
            if (string.IsNullOrEmpty(dep.city))
                return new JsonResult(new { result = "no_city" });

            try
            {
                // Retrieve the department to be edited
                var department = _context.Departments.FirstOrDefault(d => d.Id == dep.id);

                // Return error if the department does not exist
                if (department == null) return new JsonResult(new { result = "error" });

                // Update department details
                department.Name = dep.name.Trim();
                department.Description = dep.description.Trim();
                department.Address = dep.address.Trim();
                department.City = dep.city.Trim();
                department.Postcode = dep.postcode.Trim();

                logsController.AddLog(_cookieValue, $"Edycja oddziału {dep.name}");

                // Save changes to the database
                _context.SaveChanges();

                // Indicate successful department edit
                resultBuilder.Append("department_edited");
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DepartmentsController", "EditDepartment", ex);
            }

            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Update Department User Links
        /// Modifies the user associations for a specific department. It adds new users to the department and removes existing users that are not in the provided list.
        /// Validates the department ID and processes the list of user IDs to be linked to the department.
        ///
        /// Parameters:
        /// - departmentLinksOb links: An object containing the department ID and the list of user IDs.
        ///   - links.id: The ID of the department to update links for.
        ///   - links.userIds: A list of user IDs to be associated with the department.
        ///
        /// Responses:
        /// - "links_changed": Indicates the user links for the department were successfully updated.
        /// - "error": Returned if the department ID is invalid or in case of a database exception during the operation.
        /// </summary>
        /// <param name="links">The departmentLinksOb object containing linking information.</param>
        /// <returns>JsonResult indicating the outcome of the link update operation</returns>
        [HttpPost("links")]
        public IActionResult DepartmentLinks([FromBody] departmentLinksOb links)
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

            // Validate the department ID
            if (links.id == null || links.id == -1)
                return new JsonResult(new { result = "error" });

            try
            {
                // Get the current users in the department
                var currentUsersInDepartments = _context.UsersDepartments
                    .Where(ud => ud.DepartmentId == links.id)
                    .Select(ud => (int?)ud.UserId)
                    .ToList();

                // Prepare the list of user IDs for linking
                var userIdsasNullable = links.userIds.Select(id => (int?)id).ToList();

                // Determine which users to add
                var usersToAdd = userIdsasNullable.Except(currentUsersInDepartments).ToList();

                // Add new user links
                foreach (var user in usersToAdd)
                {
                    _context.UsersDepartments.Add(new UsersDepartment
                    {
                        UserId = user,
                        DepartmentId = links.id
                    });
                }

                // Determine which users to remove
                var usersToRemove = currentUsersInDepartments.Where(ud => !userIdsasNullable.Contains(ud)).ToList();

                // Remove old user links
                foreach (var user in usersToRemove)
                {
                    _context.UsersDepartments.Remove(_context.UsersDepartments.FirstOrDefault(ud => ud.UserId == user && ud.DepartmentId == links.id));
                }
                logsController.AddLog(_cookieValue, "Dodanie użytkowników do oddziału");
                // Save changes to the database
                _context.SaveChanges();

                // Indicate successful update of links
                return new JsonResult(new { result = "links_changed" });
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DepartmentsController", "DepartmentLinks", ex);
            }

            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }
        /// <summary>
        /// Add a New User
        /// Creates a new user in the system with the provided details, including assigning them to a department and roles.
        /// Validates necessary fields like names, email, and roles. Also checks the phone number format and email format.
        ///
        /// Parameters:
        /// - departmentsAddUserOb user: An object containing the new user's details.
        ///   - user.names: Full name of the user.
        ///   - user.email: Email address of the user.
        ///   - user.roles: List of roles to be assigned to the user.
        ///   - user.phone: (Optional) Phone number of the user.
        ///   - user.color: (Optional) Color associated with the user.
        ///   - Additional fields like NIP, postcode, city, etc.
        ///
        /// Responses:
        /// - "user_created": Indicates the user was successfully created or updated.
        /// - "no_names", "no_email", "no_roles", "phone_format", "bad_email_format", "exists": Returned if there are validation errors in the input.
        /// - "error": Returned in case of a database exception during the operation.
        /// </summary>
        /// <param name="user">The departmentsAddUserOb object containing new user information.</param>
        /// <returns>JsonResult indicating the outcome of the user creation operation</returns>
        [HttpPost("users")]
        public IActionResult AddNewUser([FromBody] departmentsAddUserOb user)
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
                return new JsonResult(new {result =  "no_names" });
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
                // Check if user already exists
                var userdb = _context.Users.FirstOrDefault(u => u.Email == user.email.Trim());
                // Handling existing or new user
                if (userdb != null)
                {
                    if (userdb.AppRole == "Employee")
                        return new JsonResult(new { result = "exists" });
                    // Update the existing user if they are an client
                    if (userdb.IsDeleted != 1)
                    {
                        userdb.Name = _name;
                        userdb.Lastname = _lastname;
                        userdb.Email = user.email.Trim();
                        userdb.Nip = user.nip.Trim();
                        userdb.Phone = user.phone.Trim();
                        userdb.Postcode = user.postcode.Trim();
                        userdb.City = user.city.Trim();
                        userdb.Address = user.address.Trim();
                        userdb.Color = user.color;
                        userdb.AppRole = "Employee";

                        _context.UsersDepartments.Add(new UsersDepartment
                        {
                            UserId = userdb.Id,
                            DepartmentId = user.departmentID
                        });
                    }
                    else
                    {
                        userdb.IsDeleted = 0;
                    }
                    logsController.AddLog(_cookieValue, $"Dodanie konta użytkownika {userdb.Name} {userdb.Lastname} do oddziału!");
                    _context.SaveChanges();


                    Sender.SendExistingAccountAddEmployeeEmail("Mechapp - Witamy w zespole", userdb.Name, userdb.Lastname, userdb.Email);
                }
                else
                {
                    // Create a new user if they don't exist
                    string password = generators.generatePassword(15);
                    string salt = generators.generatePassword(10);
                    string _combinedPassword = password + salt;

                    Models.User newUser = new Models.User
                    {
                        Name = _name,
                        Lastname = _lastname,
                        Email = user.email.Trim(),
                        Password = hashes.GenerateSHA512Hash(_combinedPassword),
                        Salt = salt,
                        Nip = user.nip.Trim(),
                        Phone = user.phone.Trim(),
                        Postcode = user.postcode.Trim(),
                        City = user.city.Trim(),
                        Address = user.address.Trim(),
                        Color = user.color.Trim(),
                        AppRole = "Employee",
                        IsFirstLogin = 1
                    };

                    logsController.AddLog(_cookieValue, $"Dodanie konta użytkownika {newUser.Name} {newUser.Lastname} do oddziału!");

                    _context.Users.Add(newUser);
                    _context.SaveChanges();

                    _context.UsersDepartments.Add(new UsersDepartment
                    {
                        UserId = newUser.Id,
                        DepartmentId = user.departmentID
                    });

                    _context.SaveChanges();

                    Sender.SendAddEmployeeEmail("MechApp - Witamy w zespole", newUser.Name, newUser.Lastname, newUser.Email, password);
                }
                // Indicate successful user creation or update
                resultBuilder.Append("user_created");
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DepartmentsController", "DepartmentAddUser", ex);
            }
            // Return the final result
            return new JsonResult(new {result =  resultBuilder.ToString()});
        }

        /// <summary>
        /// Delete a Department
        /// Removes a department and its associations from the database based on the provided ID.
        /// Validates the department ID before attempting the deletion.
        ///
        /// Parameters:
        /// - id: The ID of the department to be deleted.
        ///
        /// Responses:
        /// - "department_deleted": Indicates the department was successfully deleted.
        /// - "error": Returned if the department ID is invalid or in case of a database exception during the operation.
        /// </summary>
        /// <param name="id">The ID of the department to delete.</param>
        /// <returns>JsonResult indicating the outcome of the department deletion operation</returns>
        [HttpDelete("delete/department/{id}")]
        public IActionResult DeleteDepartment(int id)
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

            // Validate the department ID
            if (id == null || id == -1)
                return new JsonResult(new { result = "error" });

            try
            {
                logsController.AddLog(_cookieValue, $"Usunięcie oddziału: {_context.Departments.Where(d => d.Id == id).Select(d => d.Name).FirstOrDefault()}");
                // Remove the department and its user associations from the database
                _context.Departments.RemoveRange(_context.Departments.Where(d => d.Id == id));
                _context.UsersDepartments.RemoveRange(_context.UsersDepartments.Where(ud => ud.DepartmentId == id));
                _context.SaveChanges();

                // Indicate successful department deletion
                resultBuilder.Append("department_deleted");
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DepartmentsControllers", "DeleteDepartment", ex);
            }

            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Delete Multiple Departments
        /// Removes multiple departments from the database based on a list of their IDs. 
        /// This operation also removes all associations of these departments with users. 
        /// Parses and validates the list of IDs before proceeding.
        ///
        /// Parameters:
        /// - idsList: A comma-separated string of department IDs to be deleted.
        ///
        /// Responses:
        /// - "departments_deleted": Indicates that the departments were successfully deleted.
        /// - "error": Returned if there is an issue with parsing the IDs, if any of the departments do not exist, or in case of a database exception during the operation.
        /// </summary>
        /// <param name="idsList">A string containing the comma-separated list of department IDs.</param>
        /// <returns>JsonResult indicating the outcome of the departments deletion operation</returns>
        [HttpDelete("delete/departments/{idsList}")]
        public IActionResult DeleteDepartments(string idsList)
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

            // Parse the ID list and convert to integers
            List<int> ids = idsList.Split(',').Select(id => Convert.ToInt32(id)).ToList();

            try
            {
                logsController.AddLog(_cookieValue, $"Usunięcie oddziałów: {_context.Departments.Where(d => ids.Contains((int)d.Id)).Select(d => d.Name).ToString()}");

                // Remove the departments and their user associations from the database
                _context.Departments.RemoveRange(_context.Departments.Where(d => ids.Contains(Convert.ToInt32(d.Id))));
                _context.UsersDepartments.RemoveRange(_context.UsersDepartments.Where(ud => ids.Contains(Convert.ToInt32(ud.DepartmentId))));
                _context.SaveChanges();

                // Indicate successful deletion of departments
                resultBuilder.Append("departments_deleted");
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "DepartmentsController", "DeleteDepartments", ex);
            }

            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }



        public class addDepartmentOb
        {
            public string? name { get; set; }
            public string? description { get; set; }
            public string? address { get; set; }
            public string? postcode { get; set; }
            public string? city { get; set; }
        }

        public class editDepartmentOb
        {
            public int? id { get; set; }
            public string? name { get; set; }
            public string? description { get; set; }
            public string? address { get; set; }
            public string? postcode { get; set; }
            public string? city { get; set; }
        }

        public class departmentLinksOb
        {
            public int? id { get; set; }
            public List<int>? userIds { get; set; }
        }

        public class departmentsAddUserOb 
        { 
            public int? departmentID { get; set; }
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
    }
}
