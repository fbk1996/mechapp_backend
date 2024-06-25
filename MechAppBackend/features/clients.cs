using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using MechAppBackend.Security;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace MechAppBackend.features
{
    public class clients
    {
        MechAppContext _context;

        public clients(MechAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a paginated list of clients based on the provided search criteria.
        /// </summary>
        /// <param name="name">Optional search term for filtering clients by their name or last name. If null or empty, no name-based filtering is applied.</param>
        /// <param name="pageSize">The number of clients to return in a single page.</param>
        /// <param name="offset">The offset from the beginning of the list of clients to start returning results from, used for pagination.</param>
        /// <returns>
        /// A list of client objects that match the search criteria, limited by the specified pageSize and starting from the given offset.
        /// Each client object includes the client's ID, name, last name, company name, NIP, and a flag indicating if they are a loyal customer.
        /// If a database error occurs, an empty list is returned and the exception is logged.
        /// </returns>
        /// <remarks>
        /// This method allows for searching and paginating the list of clients who have the role "Client".
        /// It supports filtering by a search term that is matched against the client's name or last name.
        /// </remarks>
        public List<clientOb> GetClients(string? name, int pageSize, int offset)
        {
            try
            {
                IQueryable<User> query = _context.Users;
                // Apply name-based filtering if a name is provided.
                if (!string.IsNullOrEmpty(name))
                    query = query.Where(u => u.Name.ToLower().Contains(name.ToLower().Trim()) || u.Lastname.ToLower().Contains(name.ToLower().Trim()));
                // Filter by clients only.
                query = query.Where(u => u.AppRole == "Client");
                //filter if not deleted
                query = query.Where(u => u.IsDeleted == 0);
                // Retrieve a paginated list of clients.
                var clients = query
                    .AsNoTracking()
                    .Skip(offset)
                    .Take(pageSize)
                    .Select(u => new clientOb
                    {
                        id = (int)u.Id,
                        name = u.Name,
                        lastname = u.Lastname,
                        companyName = u.CompanyName,
                        nip = u.Nip,
                        isLoyalCustomer = u.IsLoyalCustomer,
                    }).ToList();

                return clients;
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an empty list in case of a database error.
                Logger.SendException("MechApp", "clients", "GetClients", ex);
                return new List<clientOb>();
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific client by their ID.
        /// </summary>
        /// <param name="id">The ID of the client for whom details are being requested. If -1, a default object indicating an invalid ID is returned.</param>
        /// <returns>
        /// An object containing detailed information about the client, including their ID, email, name, last name, NIP, phone number, postcode, city, address, icon, company name, and a flag indicating if they are a loyal customer.
        /// If the client does not exist or a database error occurs, an object with an ID of -1 is returned and the exception is logged.
        /// </returns>
        /// <remarks>
        /// This method is used to fetch all relevant details for a client, which can be used for displaying on a client profile page or for editing client information.
        /// </remarks>
        public clientDetailsOb GetClientDetails(int id = -1)
        {
            if (id == -1)
                return new clientDetailsOb { id = -1 }; // Return a default object for invalid ID.

            try
            {
                // Query the database for the client with the specified ID.
                var user = _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == id)
                    .Select(u => new clientDetailsOb
                    {
                        id = (int)u.Id,
                        email = u.Email,
                        name = u.Name,
                        lastname = u.Lastname,
                        nip = u.Nip,
                        phone = u.Phone,
                        postcode = u.Postcode,
                        city = u.City,
                        address = u.Address,
                        icon = u.Icon,
                        companyName = u.CompanyName,
                        isLoyalCustomer = u.IsLoyalCustomer
                    }).FirstOrDefault();

                return user ?? new clientDetailsOb { id = -1}; // Return the found client or a default object if not found.
            }
            catch (MySqlException ex)
            {
                // Log the exception and return a default object in case of a database error.
                Logger.SendException("MechApp", "clients", "GetClientDetails", ex);
                return new clientDetailsOb { id = -1 };
            }
        }

        /// <summary>
        /// Adds a new client to the database based on the provided client information.
        /// </summary>
        /// <param name="client">An object containing the new client's information, including name, last name, email, phone, and other relevant details.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "no_name" if the client's name is missing,
        /// - "no_lastname" if the client's last name is missing,
        /// - "no_email" if the client's email is missing,
        /// - "bad_phone_format" if the provided phone number does not meet the expected format,
        /// - "bad_email_format" if the provided email does not meet the expected format,
        /// - "exist" if a client with the same email already exists in the database,
        /// - "user_activated" if a previously deleted client with the same email is reactivated,
        /// - "client_created" if the new client is successfully added to the database,
        /// - "error" if a database error occurs during the operation.
        /// </returns>
        /// <remarks>
        /// This method validates the provided client information, checks for existing clients with the same email, and adds a new client to the database. It also handles reactivation of previously deleted clients and sends a welcome email to the new client.
        /// </remarks>
        public string AddClient(AddEditClientOb client)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(client.name))
                return "no_name";
            if (string.IsNullOrEmpty(client.lastname))
                return "no_lastname";
            if (string.IsNullOrEmpty(client.email))
                return "no_email";
            // Validate phone and email formats
            if (!string.IsNullOrEmpty(client.phone) && Validators.CheckPhone(client.phone, client.country))
                return "bad_phone_format";
            if (!string.IsNullOrEmpty(client.email) && !Validators.ValidateEmail(client.email))
                return "bad_email_format";

            try
            {
                // Check if a user with the same email already exists
                var checkUser = _context.Users.FirstOrDefault(u => u.Email == client.email.Trim());

                if (checkUser != null)
                {
                    // If exists, check if it was a deleted client and reactivate it
                    if (checkUser.AppRole == "Client" && checkUser.IsDeleted == 1)
                    {
                        checkUser.IsDeleted = 0;
                        _context.SaveChanges();
                        Sender.SendAddDeletedClientEmail("Klient - ponowne powitanie!", client.name, client.lastname, client.email);
                        return "user_activated";
                    }
                    return "exist";
                }
                // Generate salt and password for the new client
                string _salt = generators.generatePassword(10);
                string _password = generators.generatePassword(15);
                string _combinedPassword = _password + _salt;
                // Add the new client to the database
                _context.Users.Add(new User
                {
                    Email = client.email.Trim(),
                    Name = client.name,
                    Lastname = client.lastname,
                    Nip = client.nip,
                    Phone = client.phone,
                    Postcode = client.postcode,
                    City = client.city,
                    Address = client.address,
                    IsLoyalCustomer = (short)client.isLoyalCustomer,
                    CompanyName = client.companyName,
                    IsFirstLogin = 1,
                    IsDeleted = 0,
                    AppRole = "Client",
                    Password = hashes.GenerateSHA512Hash(_combinedPassword),
                    Salt = _salt
                });

                _context.SaveChanges();
                // Send a welcome email to the new client
                Sender.SendAddClientEmail("Nowe konto Klienta!", client.name, client.lastname, client.email, _password);

                return "client_created";
            }
            catch (MySqlException ex)
            {
                // Log any database errors and return an error message
                Logger.SendException("MechApp", "clients", "AddClient", ex);
                return "error";
            }
        }

        /// <summary>
        /// Edits an existing client's information in the database based on the provided client object.
        /// </summary>
        /// <param name="client">An object containing the updated information for the client, including their ID, name, last name, email, and other relevant details.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the client ID is invalid, required fields are missing, or a database error occurs,
        /// - "no_name" if the client's name is missing,
        /// - "no_lastname" if the client's last name is missing,
        /// - "no_email" if the client's email is missing,
        /// - "bad_phone_format" if the provided phone number does not meet the expected format,
        /// - "bad_email_format" if the provided email does not meet the expected format,
        /// - "email_exist" if the new email provided already exists for another client,
        /// - "client_edited" if the client's information is successfully updated.
        /// </returns>
        /// <remarks>
        /// This method checks for the presence of required fields, validates the format of the phone number and email, checks for email uniqueness, and updates the client's information in the database.
        /// </remarks>
        public string EditClient(AddEditClientOb client)
        {
            // Validate required fields and formats
            if (client.id == -1)
                return "error";
            if (string.IsNullOrEmpty(client.name))
                return "no_name";
            if (string.IsNullOrEmpty(client.lastname))
                return "no_lastname";
            if (string.IsNullOrEmpty(client.email))
                return "no_email";
            if (string.IsNullOrEmpty(client.oldEmail))
                return "error";

            if (!string.IsNullOrEmpty(client.phone) && Validators.CheckPhone(client.phone, client.country))
                return "bad_phone_format";
            if (!string.IsNullOrEmpty(client.email) && !Validators.ValidateEmail(client.email))
                return "bad_email_format";

            try
            {
                // Check if the client exists in the database
                var clientDb = _context.Users.FirstOrDefault(u => u.Id == client.id);

                if (clientDb == null)
                    return "error";

                var checkEmail = _context.Users.FirstOrDefault(u => u.Email == client.email);
                // Check if the new email is unique
                if (checkEmail != null && client.email != client.oldEmail)
                    return "email_exist";
                // Update client information
                clientDb.Name = client.name;
                clientDb.Lastname = client.lastname;
                clientDb.CompanyName = client.companyName;
                clientDb.Email = client.email;
                clientDb.Phone = client.phone;
                clientDb.Postcode = client.postcode;
                clientDb.City = client.city;
                clientDb.Address = client.address;
                clientDb.Nip = client.nip;
                clientDb.IsLoyalCustomer = (short)client.isLoyalCustomer;
                // Save changes to the database
                _context.SaveChanges();
                return "client_edited";
            }
            catch (MySqlException ex)
            {
                // Log any database errors and return an error message
                Logger.SendException("MechApp", "clients", "EditClient", ex);
                return "error";
            }
        }

        /// <summary>
        /// Marks a client as deleted in the database based on the provided client ID.
        /// </summary>
        /// <param name="id">The ID of the client to be marked as deleted. If -1, an error is returned.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the client ID is invalid or a database error occurs,
        /// - "client_deleted" if the client is successfully marked as deleted.
        /// </returns>
        /// <remarks>
        /// This method sets the IsDeleted flag for the specified client and all vehicles owned by the client to 1, effectively marking them as deleted without actually removing them from the database. This allows for data recovery and audit trails.
        /// </remarks>
        public string DeleteClient(int id = -1)
        {
            if (id == -1)
                return "error"; // Return error if the client ID is invalid.

            try
            {
                // Retrieve all vehicles owned by the client.
                var usersVehicles = _context.UsersVehicles.Where(v => v.Owner == id).ToList();
                // Mark each vehicle as deleted.
                foreach (var vehicle in usersVehicles)
                {
                    vehicle.IsDeleted = 1;
                }
                // Retrieve the client from the database.
                var client = _context.Users.FirstOrDefault(u => u.Id == id);

                if (client == null)
                    return "error"; // Return error if the client does not exist.

                client.IsDeleted = 1; // Mark the client as deleted.

                _context.SaveChanges(); // Save changes to the database.

                return "client_deleted"; // Return success message.
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "clients", "DeleteClient", ex); // Log the exception and return an error message in case of a database error.
                return "error";
            }
        }

        /// <summary>
        /// Marks multiple clients as deleted in the database based on the provided list of client IDs.
        /// </summary>
        /// <param name="ids">A list of IDs of the clients to be marked as deleted.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the list of IDs is empty or a database error occurs,
        /// - "clients_deleted" if all specified clients are successfully marked as deleted.
        /// </returns>
        /// <remarks>
        /// This method sets the IsDeleted flag for each specified client and all vehicles owned by those clients to 1, effectively marking them as deleted without actually removing them from the database. This allows for data recovery and audit trails.
        /// </remarks>
        public string DeleteClients(List<int> ids)
        {
            if (ids.Count == 0)
                return "error"; // Return error if the list of IDs is empty

            try
            {
                // Retrieve all vehicles owned by the specified clients.
                var usersVehicles = _context.UsersVehicles.Where(v => ids.Contains((int)v.Id)).ToList();

                // Mark each vehicle as deleted.
                foreach (var vehicle in usersVehicles)
                {
                    vehicle.IsDeleted = 1;
                }

                // Retrieve the specified clients from the database.
                var clients = _context.Users.Where(u => ids.Contains((int)u.Id)).ToList();
                // Mark each client as deleted.
                foreach (var client in clients)
                {
                    client.IsDeleted = 1;
                }
                // Save changes to the database.
                _context.SaveChanges();

                return "clients_deleted";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error message in case of a database error.
                Logger.SendException("MechApp", "clients", "DeleteClients", ex);
                return "error";
            }
        }
    }

    public class clientOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? companyName { get; set; }
        public string? nip { get; set; }
        public int? isLoyalCustomer { get; set; }
    }

    public class clientDetailsOb
    {
        public int? id { get; set; } = -1;
        public string? email { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? nip { get; set; }
        public string? phone { get; set; }
        public string? postcode { get; set; }
        public string? city { get; set; }
        public string? address { get; set; }
        public int? isLoyalCustomer { get; set; }
        public string? icon { get; set; }
        public string? companyName { get; set; }
    }

    public class AddEditClientOb
    {
        public int? id { get; set; } = -1;
        public string? email { get; set; }
        public string? oldEmail { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? nip { get; set; }
        public string? phone { get; set; }
        public string? postcode { get; set; }
        public string? city { get; set; }
        public string? address { get; set; }
        public int? isLoyalCustomer { get; set; }
        public string? companyName { get; set; }
        public string? country { get; set; }
    }
}
