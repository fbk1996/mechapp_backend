using MechAppBackend.Data;
using MechAppBackend.features;
using MechAppBackend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        MechAppContext _context;
        MechappClientsContext _clientContext;
        MechappApContext _apContext;

        logs logsController;
        CheckCookieToken cookieToken;
        CheckRoles roles;
        tickets ticketsController;

        public TicketsController(MechAppContext context, MechappClientsContext clientContext, MechappApContext apContext)
        {
            _context = context;
            _clientContext = clientContext;
            _apContext = apContext;
            logsController = new logs(context);
            cookieToken = new CheckCookieToken(context);
            roles = new CheckRoles(context);
            ticketsController = new tickets(context, clientContext, apContext);
        }

        /// <summary>
        /// Retrieves a paginated list of support tickets based on the provided filters.
        /// </summary>
        /// <param name="dateFrom">Optional filter for the start date of tickets.</param>
        /// <param name="dateTo">Optional filter for the end date of tickets.</param>
        /// <param name="title">Optional filter for the title of tickets.</param>
        /// <param name="pageSize">The number of tickets to retrieve per page.</param>
        /// <param name="currentPage">The current page number for pagination.</param>
        /// <returns>
        /// A JSON result containing the outcome of the operation and the list of tickets if successful.
        /// Possible results include:
        /// - "done" if the operation is successful.
        /// - "no_auth" if the user is not authenticated.
        /// - "no_permission" if the user lacks the necessary permissions.
        /// - "error" if an error occurs during the operation.
        /// </returns>
        /// <remarks>
        /// This endpoint requires the user to be authenticated and authorized to view tickets.
        /// </remarks>
        [HttpGet]
        public IActionResult GetTickets(DateTime? dateFrom, DateTime? dateTo, string? title, int pageSize, int currentPage)
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

            if (!roles.isAuthorized(_cookieValue, "tickets", "view"))
                return new JsonResult(new { result = "no_permission" });

            int offset = ((currentPage - 1) * pageSize);

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie listy zgłoszeń pomocy technicznej");

                var tickets = ticketsController.GetTickets(dateFrom, dateTo, title, pageSize, offset);

                return new JsonResult(new
                {
                    result = "done",
                    tickets = tickets
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "TicketsController", "GetTickets", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString() });
        }

        /// <summary>
        /// Retrieves detailed information for a specific support ticket.
        /// </summary>
        /// <param name="id">The ID of the ticket to retrieve details for. Defaults to -1.</param>
        /// <returns>
        /// A JSON result containing the ticket details if successful, or an error message indicating the issue:
        /// - "no_auth": No authentication token present.
        /// - "no_permission": User does not have permission to view tickets.
        /// - "error": An error occurred while retrieving the ticket details.
        /// </returns>
        /// <remarks>
        /// This method checks for a session token in the request cookies and validates it. 
        /// If the token is valid and the user has the appropriate permissions, 
        /// the method retrieves the ticket details from the database and returns them in a JSON format.
        /// </remarks>
        [HttpGet("details")]
        public IActionResult GetTicketDetails(int id = -1)
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

            if (!roles.isAuthorized(_cookieValue, "tickets", "view"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Pobranie szczegółów zgłoszenia pomocy technicznej");

                var ticket = ticketsController.GetTicketDetails(id);

                if (ticket.id == -1)
                    return new JsonResult(new { result = "error" });

                return new JsonResult(new
                {
                    result = "done",
                    ticket = ticket
                });
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "TicketsController", "GetTicketDetails", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Adds a new support ticket to the system.
        /// </summary>
        /// <param name="ticket">The details of the ticket to be added.</param>
        /// <returns>A JSON result indicating the outcome of the operation.</returns>
        /// <remarks>
        /// This endpoint requires authentication and appropriate permissions.
        /// If the session token is missing or invalid, the response will indicate "no_auth".
        /// If the user does not have the necessary permissions, the response will indicate "no_permission".
        /// In case of a database error, the response will indicate "error".
        /// </remarks>
        [HttpPost]
        public IActionResult AddTicket([FromBody]AddTicketOb ticket)
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

            if (!roles.isAuthorized(_cookieValue, "tickets", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Dodanie nowego zgłoszenia pomocy technicznej");

                string result = ticketsController.AddTicket(ticket, _cookieValue);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "TicketsController", "AddTicket", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Endpoint for adding a new message to an existing technical support ticket.
        /// </summary>
        /// <remarks>
        /// This endpoint allows authenticated users with appropriate permissions to add a new message
        /// to an existing technical support ticket. The session token is validated to ensure user authentication
        /// and authorization before processing the request.
        /// </remarks>
        /// <param name="message">The message object containing details of the message to be added.</param>
        /// <returns>
        /// JSON result indicating the outcome of the operation:
        /// - "no_auth" if the user is not authenticated.
        /// - "no_permission" if the user does not have permission to add messages to tickets.
        /// - "error" if an error occurred during processing.
        /// - Otherwise, a result indicating successful addition of the message.
        /// </returns>
        [HttpPost("message")]
        public IActionResult AddMessage([FromBody]AddTicketMessageOb message)
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

            if (!roles.isAuthorized(_cookieValue, "tickets", "add"))
                return new JsonResult(new { result = "no_permission" });

            try
            {
                logsController.AddLog(_cookieValue, "Dodanie nowego komentarza do zgłoszenia pomocy technicznej");

                string result = ticketsController.AddMessage(message, _cookieValue);

                resultBuilder.Append(result);
            }
            catch (MySqlException ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "TicketsController", "AddMessage", ex);
            }

            return new JsonResult(new {result = resultBuilder.ToString() });
        }
    }
}
