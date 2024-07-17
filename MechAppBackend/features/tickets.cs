using MechAppBackend.AppSettings;
using MechAppBackend.ClientsModels;
using MechAppBackend.Data;
using MechAppBackend.Helpers;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace MechAppBackend.features
{
    public class tickets
    {
        MechAppContext _context;
        MechappClientsContext _clientContext;
        MechappApContext _apContext;

        public tickets(MechAppContext context, MechappClientsContext clientContext, MechappApContext apContext)
        {
            _context = context;
            _clientContext = clientContext;
            _apContext = apContext;
        }

        /// <summary>
        /// Retrieves a list of tickets based on the specified criteria.
        /// </summary>
        /// <param name="dateFrom">The start date for filtering tickets. If null, no start date filter is applied.</param>
        /// <param name="dateTo">The end date for filtering tickets. If null, no end date filter is applied.</param>
        /// <param name="title">The title or part of the title to filter tickets. If null or empty, no title filter is applied.</param>
        /// <param name="pageSize">The number of tickets to retrieve per page.</param>
        /// <param name="offset">The number of tickets to skip before starting to retrieve the tickets.</param>
        /// <returns>A list of tickets that match the specified criteria. Returns an empty list if an error occurs.</returns>
        public List<ticketsOb> GetTickets(DateTime? dateFrom, DateTime? dateTo, string? title, int pageSize, int offset)
        {
            try
            {
                // Initialize the query to retrieve tickets from the context
                IQueryable<Ticket> query = _clientContext.Tickets;
                // Apply date filtering based on the provided dateFrom and dateTo parameters
                if (dateFrom != null && dateTo == null)
                    query = query.Where(t => t.Date >= dateFrom);
                else if (dateFrom == null && dateTo != null)
                    query = query.Where(t => t.Date <= dateTo);
                else if (dateFrom != null && dateTo != null)
                    query = query.Where(t => t.Date >= dateFrom && t.Date <= dateTo);
                // Apply title filtering if the title parameter is not null or empty
                if (!string.IsNullOrEmpty(title))
                    query = query.Where(t => t.Title.ToLower().Contains(title.ToLower().Trim()));
                // Filter tickets by the current subscription
                query = query.Where(t => t.SubscriptionId == appdata.subscription);
                // Retrieve the tickets, apply ordering, paging, and project to ticketsOb objects
                var tickets = query
                    .AsNoTracking() // Disable tracking for read-only query
                    .OrderByDescending(t => t.Date) // Order by date descending
                    .Skip(offset)
                    .Take(pageSize)
                    .Select(t => new ticketsOb
                    {
                        id = (int)t.Id,
                        client = new ticketsUserOb
                        {
                            id = (int)t.ClientId,
                            name = t.ClientName,
                            lastname = t.ClientLastname,
                            email = t.ClientEmail,
                            phone = t.ClientPhone
                        },
                        date = t.Date,
                        title = t.Title,
                        status = (int)t.Status,
                        count = query.Count()
                    }).ToList();

                foreach (var ticket in tickets)
                {
                    var ownerId = _clientContext.Tickets.Where(t => t.Id == ticket.id).Select(t => t.OwnerId).FirstOrDefault();

                    var ownerData = (ticketsUserOb)_apContext.Users.Where(u => u.Id == ownerId)
                        .Select(u => new ticketsUserOb
                        {
                            id = (int)u.Id,
                            name = u.Login,
                            email = u.Email,
                            phone = u.Phone
                        }).FirstOrDefault();

                    ticket.owner = ownerData;
                }
                
                return tickets;
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an empty list if an error occurs
                Logger.SendException("MechApp", "tickets", "GetTickets", ex);
                return new List<ticketsOb>();
            }
        }

        /// <summary>
        /// Retrieves the details of a specific ticket based on the provided ticket ID.
        /// </summary>
        /// <param name="id">The ID of the ticket to retrieve details for. Defaults to -1 if not provided.</param>
        /// <returns>A ticketsDetailsOb object containing the details of the ticket, or a default object with id set to -1 if not found or an error occurs.</returns>
        public ticketsDetailsOb GetTicketDetails(int id = -1)
        {
            // Validate input parameter
            if (id == -1)
                return new ticketsDetailsOb { id = -1 };

            try
            {
                // Retrieve the ticket details based on the provided ID
                var ticket = (ticketsDetailsOb)_clientContext.Tickets
                    .AsNoTracking() // Disable tracking for read-only query
                    .Where(t => t.Id == id)
                    .Select(t => new ticketsDetailsOb
                    {
                        id = (int)t.Id,
                        client = new ticketsUserOb
                        {
                            id = (int)t.ClientId,
                            name = t.ClientName,
                            lastname = t.ClientLastname,
                            email = t.ClientEmail,
                            phone = t.ClientPhone
                        },
                        title = t.Title,
                        date = t.Date,
                        status = (int)t.Status,
                        messages = _clientContext.TicketsMessages
                            .AsNoTracking()
                            .Where(tm => tm.TicketId == t.Id)
                            .Select(tm => new TicketsMessageOb
                            {
                                id = (int)tm.Id,
                                message = tm.Message,
                                user = new ticketsUserOb
                                {
                                    name = tm.User
                                },
                                files = _clientContext.TicketsFiles
                                    .AsNoTracking()
                                    .Where(tf => tf.MessageId == tm.Id)
                                    .Select(tf => new TicketsFileOb
                                    {
                                        id = (int)tf.Id,
                                        file = tf.File,
                                        fileName = tf.Name
                                    }).ToList()
                            }).ToList()
                    }).FirstOrDefault();


                    var ownerId = _clientContext.Tickets.Where(t => t.Id == ticket.id).Select(t => t.OwnerId).FirstOrDefault();

                    var ownerData = (ticketsUserOb)_apContext.Users.Where(u => u.Id == ownerId)
                        .Select(u => new ticketsUserOb
                        {
                            id = (int)u.Id,
                            name = u.Login,
                            email = u.Email,
                            phone = u.Phone
                        }).FirstOrDefault();

                    ticket.owner = ownerData;


                // Return the ticket details, or a default object with id set to -1 if not found
                return ticket ?? new ticketsDetailsOb { id = -1 };
            }
            catch (MySqlException ex)
            {
                // Log the exception and return a default object with id set to -1 if an error occurs
                Logger.SendException("MechApp", "tickets", "GetTicketDetails", ex);
                return new ticketsDetailsOb { id = -1 };
            }
        }

        /// <summary>
        /// Adds a new ticket with an associated initial message and optional files.
        /// </summary>
        /// <param name="ticket">The ticket details including title and message.</param>
        /// <param name="_cookieToken">The cookie token used to authenticate the user.</param>
        /// <returns>A string indicating the result of the operation: "ticket_added", "no_title", "no_message", or "error".</returns>
        public string AddTicket(AddTicketOb ticket, string _cookieToken)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(ticket.title))
                return "no_title";
            if (string.IsNullOrEmpty(ticket.message.message))
                return "no_message";

            try
            {
                // Retrieve the user associated with the provided cookie token
                var user = _context.Users.FirstOrDefault(u => u.Id == _context.UsersTokens.Where(st => st.Token == _cookieToken).Select(st => st.UserId).FirstOrDefault());

                DateTime now = DateTime.Now;

                if (user == null)
                    return "error";
                // Add a new ticket to the database
                var newTicket = _clientContext.Tickets.Add(new Ticket
                {
                    Title = ticket.title,
                    ClientId = user.Id,
                    ClientName = user.Name,
                    ClientLastname = user.Lastname,
                    ClientEmail = user.Email,
                    ClientPhone = user.Phone,
                    Date = now,
                    SubscriptionId = appdata.subscription,
                    Status = 0
                });

                _clientContext.SaveChanges();
                // Add the initial message associated with the new ticket
                var newMessage = _clientContext.TicketsMessages.Add(new TicketsMessage
                {
                    TicketId = newTicket.Entity.Id,
                    User = $"{user.Name.ToLower()}.{user.Lastname.ToLower()}",
                    Message = ticket.message.message,
                    IsNotificationSend = 0
                });

                _clientContext.SaveChanges();
                // Add any files associated with the initial message
                foreach (var file in ticket.message.files)
                {
                    _clientContext.TicketsFiles.Add(new TicketsFile
                    {
                        MessageId = newMessage.Entity.Id,
                        File = file.file,
                        Name = file.fileName
                    });
                }

                _clientContext.SaveChanges();

                return "ticket_added";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return "error" if an exception occurs
                Logger.SendException("MechApp", "tickets", "AddTickets", ex);
                return "error";
            }
        }

        /// <summary>
        /// Adds a new message to an existing ticket with optional files.
        /// </summary>
        /// <param name="message">The message details including ticket ID, message content, and any associated files.</param>
        /// <param name="_cookieValue">The cookie value used to authenticate the user.</param>
        /// <returns>A string indicating the result of the operation: "message_added", "no_ticket_id", "no_message", or "error".</returns>
        public string AddMessage(AddTicketMessageOb message, string _cookieValue)
        {
            // Validate input parameters
            if (message.ticketId == -1)
                return "no_ticket_id";
            if (string.IsNullOrEmpty(message.message))
                return "no_message";

            try
            {
                // Retrieve the user associated with the provided cookie value
                var user = _context.Users.FirstOrDefault(u => u.Id == _context.UsersTokens.Where(st => st.Token == _cookieValue).Select(st => st.UserId).FirstOrDefault());

                if (user == null)
                    return "error";
                // Add a new message to the specified ticket
                var newMessage = _clientContext.TicketsMessages.Add(new TicketsMessage
                {
                    TicketId = message.ticketId,
                    Message = message.message,
                    User = $"{user.Name.ToLower()}.{user.Lastname.ToLower()}",
                    IsNotificationSend = 0
                });

                _clientContext.SaveChanges();
                // Add any files associated with the new message
                foreach (var file in message.files)
                {
                    _clientContext.TicketsFiles.Add(new TicketsFile
                    {
                        MessageId = newMessage.Entity.Id,
                        Name = file.fileName,
                        File = file.file
                    });
                }

                _clientContext.SaveChanges();

                return "message_added";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return "error" if an exception occurs
                Logger.SendException("MechApp", "tickets", "AddMessage", ex);
                return "error";
            }
        }
    }

    public class ticketsOb
    {
        public int? id { get; set; }
        public ticketsUserOb? client { get; set; }
        public ticketsUserOb? owner { get; set; }
        public string? title { get; set; }
        public DateTime? date { get; set; }
        public int? status { get; set; }
        public int? count { get; set; }
    }

    public class ticketsDetailsOb
    {
        public int? id { get; set; }
        public ticketsUserOb? client { get; set; }
        public ticketsUserOb? owner { get; set; }
        public string? title { get; set; }
        public DateTime? date { get; set; }
        public int? status { get; set; }
        public List<TicketsMessageOb>? messages { get; set; }
    }

    public class AddTicketOb
    {
        public string? title { get; set; }
        public TicketsMessageOb? message { get; set; }
    }

    public class AddTicketMessageOb
    {
        public int? ticketId { get; set; }
        public string? message { get; set; }
        public List<TicketsFileOb>? files { get; set; }
    }

    public class ticketsUserOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
    }

    public class TicketsMessageOb
    {
        public int? id { get; set; }
        public string? message { get; set; }
        public ticketsUserOb? user { get; set; }
        public List<TicketsFileOb>? files { get; set; }
    }
    
    public class TicketsFileOb 
    {
        public int? id { get; set; }
        public string? file { get; set; }
        public string? fileName { get; set; }
    }
}
