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
                    {"adminPanel", new Dictionary<string, bool> {{"view", false}, { "edit", false} } },
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
    }
}
