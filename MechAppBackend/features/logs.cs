using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MySql.Data.MySqlClient;

namespace MechAppBackend.features
{
    public class logs
    {
        MechAppContext _context;

        public logs(MechAppContext context)
        {
            _context = context;
        }
        /// <summary>
        /// The GetLogs method retrieves all logs from the database.
        /// </summary>
        /// <returns>A list of logs, each represented as a logsOb object</returns>
        public List<logsOb> GetLogs(int _pageSize, int offset)
        {
            try
            {
                // Query the Logs table and select each log as a logsOb object
                var logsDb = _context.Logs
                    .Skip(offset)
                    .Take(_pageSize)
                    .Select(l => new logsOb
                    {
                        id = Convert.ToInt32(l.Id),
                        user = (logsUserOb)_context.Users
                            .Where(u => u.Id == l.UserId)
                            .Select(u => new logsUserOb
                            {
                                id = (int)u.Id,
                                name = u.Name,
                                lastname = u.Lastname
                            }).FirstOrDefault(),
                        date = l.Date,
                        action = l.Action,
                        count = _context.Logs.Count()
                    }).ToList();
                // Return the list of logs
                return logsDb;
            }
            catch (MySqlException ex)
            {
                // If a MySQL exception occurs, log the exception and return an empty list
                Logger.SendException("MechApp", "logs", "GetLogs", ex);
                return new List<logsOb>();
            }
        }

        /// <summary>
        /// The AddLog method adds a new log to the database.
        /// </summary>
        /// <param name="_cookieToken">The user's cookie value</param>
        /// <param name="_action">The action performed by the user</param>
        /// <param name="_description">The description of the action</param>
        /// <returns>Returns "log_added" if the log was added successfully, otherwise "error"</returns>
        public string AddLog(string _cookieToken, string _action)
        {
            try
            {
                // Get the user's session token
                var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.Token == _cookieToken);
                // If the session token does not exist, return "error"
                if (sessionToken == null)
                    return "error";
                // Get the application settings
                var appSettings = _context.AppSettings.FirstOrDefault(aps => aps.Id == 1);
                // If the application settings do not exist, return "error"
                if (appSettings == null)
                    return "error";
                // Get the current date and time
                DateTime startLog = DateTime.Now;
                // Create a new log
                _context.Logs.Add(new Models.Log
                {
                    UserId = sessionToken.UserId,
                    Date = startLog,
                    Action = _action
                });
                // Calculate the date for log retention
                var deleteRetention = DateTime.Now.AddDays(-(int)appSettings.LogsRetention);
                // Remove logs older than the retention date
                _context.Logs.RemoveRange(_context.Logs.Where(l => l.Date <= deleteRetention));
                // Save changes to the database
                _context.SaveChanges();
                // Return "log_added" to indicate that the log was added successfully
                return "log_added";
            }
            catch (MySqlException ex)
            {
                // If a MySQL exception occurs, log the exception and return "error"
                Logger.SendException("MechApp", "logs", "AddLog", ex);
                return "error";
            }
        }
    }

    public class logsOb
    {
        public int? id { get; set; }
        public logsUserOb? user { get; set; }
        public DateTime? date { get; set; }
        public string? action { get; set; }
        public string? description { get; set; }
        public int? count { get; set; }
    }

    public class logsUserOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
    }
}
