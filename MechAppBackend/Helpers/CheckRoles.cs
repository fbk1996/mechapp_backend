using MechAppBackend.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace MechAppBackend.Helpers
{
    public class CheckRoles
    {
        MechAppContext _context;

        public CheckRoles(MechAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// The isAuthorized method checks if a user is authorized to perform a certain action on a certain page.
        /// </summary>
        /// <param name="_cookieValue">The user's cookie value</param>
        /// <param name="_page">The page where the user is trying to perform the action</param>
        /// <param name="_type">The type of action the user is trying to perform</param>
        /// <returns>Returns true if the user is authorized to perform the action, otherwise false</returns>
        public bool isAuthorized(string _cookieValue, string _page, string _type)
        {
            try
            {
                // Get the user's session token
                var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.Token == _cookieValue);
                // If the session token does not exist, return false
                if (sessionToken == null) return false;
                // Get the user's roles
                var usersRoles = _context.UsersRoles
                    .Where(ur => ur.UserId == sessionToken.UserId)
                    .Select(ur => ur.RoleId).ToList();
                // Create a permission structure
                var permissionStructure = new Dictionary<string, Dictionary<string, bool>>
                {
                    {"profile", new Dictionary<string, bool> { {"edit", false}, { "passChange", false}, {"emailChange", false }, { "phoneChange", false }, { "delAccount", false} } },
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
                // For each of the user's roles
                foreach (var role in usersRoles)
                {
                    // Get the role object
                    var roleObject = _context.Roles.FirstOrDefault(r => r.Id == role);
                    // If the role object exists
                    if (roleObject != null)
                    {
                        // Deserialize the role's permissions
                        var permissions = JsonConvert.DeserializeObject<List<string>>(roleObject.Permissions);
                        // For each permission
                        foreach (var permission in permissions)
                        {
                            // Split the permission into parts
                            string[] parts = permission.Split('_');
                            // If the permission consists of two parts and the permission structure contains the first part
                            if (parts.Length == 2 && permissionStructure.ContainsKey(parts[0]))
                            {
                                // Set the second part of the permission to true
                                permissionStructure[parts[0]][parts[1]] = true;
                            }
                        }
                    }
                }
                // Return the permission for the page and type
                return permissionStructure[_page][_type];
            }
            catch (MySqlException ex)
            {
                // Log the exception and return false
                Logger.SendException("MechApp", "CheckRoles", "isAuthorized", ex);
                return false;
            }
        }

        public sidebarPermissionsOb getSidebarPermissions(string _cookieValue)
        {
            try
            {
                // Get the user's session token
                var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.Token == _cookieValue);
                // If the session token does not exist, return false
                if (sessionToken == null) return new sidebarPermissionsOb
                {
                    departments = false,
                    services = false,
                    airConditioning = false,
                    users = false,
                    roles = false,
                    requests = false,
                    schedule = false,
                    timetable = false,
                    orders = false,
                    clients = false,
                    warehouse = false,
                    demands = false,
                    prices = false,
                    tickets = false,
                    logs = false
                };   
                // Get the user's roles
                var usersRoles = _context.UsersRoles
                    .Where(ur => ur.UserId == sessionToken.UserId)
                    .Select(ur => ur.RoleId).ToList();
                // Create a permission structure
                var permissionStructure = new Dictionary<string, Dictionary<string, bool>>
                {
                    {"profile", new Dictionary<string, bool> { {"edit", false}, { "passChange", false}, {"emailChange", false }, { "phoneChange", false }, { "delAccount", false} } },
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
                    {"tickets", new Dictionary<string, bool> {{"view", false}, { "add", false} } },
                    {"prices", new Dictionary<string, bool> {{"view", false}, { "edit", false} } },
                    {"logs", new Dictionary<string, bool> {{"view", false}} }
                };
                // For each of the user's roles
                foreach (var role in usersRoles)
                {
                    // Get the role object
                    var roleObject = _context.Roles.FirstOrDefault(r => r.Id == role);
                    // If the role object exists
                    if (roleObject != null)
                    {
                        // Deserialize the role's permissions
                        var permissions = JsonConvert.DeserializeObject<List<string>>(roleObject.Permissions);
                        // For each permission
                        foreach (var permission in permissions)
                        {
                            // Split the permission into parts
                            string[] parts = permission.Split('_');
                            // If the permission consists of two parts and the permission structure contains the first part
                            if (parts.Length == 2 && permissionStructure.ContainsKey(parts[0]))
                            {
                                // Set the second part of the permission to true
                                permissionStructure[parts[0]][parts[1]] = true;
                            }
                        }
                    }
                }
                // Return the permission for the page and type
                return new sidebarPermissionsOb
                {
                    departments = permissionStructure["departments"]["view"],
                    services = permissionStructure["services"]["view"],
                    airConditioning = permissionStructure["airConditioningReports"]["view"],
                    users = permissionStructure["users"]["view"],
                    roles = permissionStructure["roles"]["view"],
                    requests = permissionStructure["requests"]["view"],
                    schedule = permissionStructure["schedule"]["view"],
                    timetable = permissionStructure["timetable"]["view"],
                    orders = permissionStructure["orders"]["view"],
                    clients = permissionStructure["clients"]["view"],
                    warehouse = permissionStructure["warehouse"]["view"],
                    demands = permissionStructure["demands"]["view"],
                    prices = permissionStructure["prices"]["view"],
                    tickets = permissionStructure["tickets"]["view"],
                    logs = permissionStructure["logs"]["view"]
                };
            }
            catch (MySqlException ex)
            {
                // Log the exception and return false
                Logger.SendException("MechApp", "CheckRoles", "isAuthorized", ex);
                return new sidebarPermissionsOb
                {
                    departments = false,
                    services = false,
                    airConditioning = false,
                    users = false,
                    roles = false,
                    requests = false,
                    schedule = false,
                    timetable = false,
                    orders = false,
                    clients = false,
                    warehouse = false,
                    demands = false,
                    prices = false,
                    tickets = false,
                    logs = false
                };
            }
        }
    }

    public class sidebarPermissionsOb
    {
        public bool departments { get; set; }
        public bool services { get; set; }
        public bool airConditioning { get; set; }
        public bool users { get; set; }
        public bool roles { get; set; }
        public bool requests { get; set; }
        public bool schedule { get; set; }
        public bool timetable { get; set; }
        public bool orders { get; set; }
        public bool clients { get; set; }
        public bool warehouse { get; set; }
        public bool demands { get; set; }
        public bool prices { get; set; }
        public bool tickets { get; set; }
        public bool logs { get; set; }
    }
}
