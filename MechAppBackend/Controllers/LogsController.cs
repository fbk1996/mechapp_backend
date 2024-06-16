using MechAppBackend.AppSettings;
using MechAppBackend.Data;
using MechAppBackend.features;
using MechAppBackend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        MechAppContext _context;
        logs logsController;
        CheckCookieToken cookieToken;
        CheckRoles roles;

        public LogsController(MechAppContext context)
        {
            _context = context;
            logsController = new logs(context);
            cookieToken = new CheckCookieToken(context);
            roles = new CheckRoles(context);
        }
        /// <summary>
        /// HTTP GET endpoint for retrieving logs with pagination.
        /// </summary>
        /// <param name="_pageSize">The number of logs to return per page.</param>
        /// <param name="_currentPage">The current page number.</param>
        /// <returns>JSON result containing the result status and a list of logs.</returns>
        [HttpGet]
        public IActionResult GetLogs(int _pageSize, int _currentPage)
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

            if (!roles.isAuthorized(_cookieValue, "logs", "view"))
                return new JsonResult(new { result = "no_permission" });

            int offset = ((_currentPage - 1) * _pageSize);

            try
            {
                List<logsOb> logs = logsController.GetLogs(_pageSize, offset);

                return new JsonResult(new
                {
                    result = "done",
                    logs = logs
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "LogsController", "GetLogs", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString() });
        }

        /// <summary>
        /// HTTP POST endpoint for editing the log retention period.
        /// </summary>
        /// <param name="_retention">The new retention period for logs in days.</param>
        /// <returns>JSON result containing the result status</returns>
        [HttpPost]
        public IActionResult EditRetentionPeriod(int _retention)
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

            if (!roles.isAuthorized(_cookieValue, "logs", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                if (_retention < 1)
                    return new JsonResult(new { result = "too_small_retention" });

                var appSettings = _context.AppSettings.FirstOrDefault(a => a.Id ==  _retention);

                if (appSettings == null)
                    return new JsonResult(new { result = "error" });

                appSettings.LogsRetention = _retention;
                _context.SaveChanges();

                resultBuilder.Append("done");
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "LogsController", "EditRetentionPeriod", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }
    }
}
