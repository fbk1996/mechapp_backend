using MechAppBackend.Data;
using MechAppBackend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/pagepermview")]
    [ApiController]
    public class PagePermissionController : ControllerBase
    {
        MechAppContext _context;
        CheckCookieToken cookieToken;
        CheckRoles roles;

        public PagePermissionController(MechAppContext context)
        {
            _context = context;
            cookieToken = new CheckCookieToken(context);
            roles = new CheckRoles(context);
        }

        /// <summary>
        /// HTTP GET endpoint for checking the view permission of a specific page.
        /// </summary>
        /// <param name="_page">The name of the page to check the view permission for.</param>
        /// <returns>JSON result containing the result status</returns>
        [HttpGet]
        public IActionResult CheckViewPermission(string _page)
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

            try
            {
                // Check if the user is authorized to view the page
                if (roles.isAuthorized(_cookieValue, _page, "view"))
                    return new JsonResult(new { result = "permission_granted" });
                else
                    return new JsonResult(new { result = "permission_denied" });
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error result
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "PagePermissionController", "CheckView Permission", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }
    }
}
