using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using MySql.Data.MySqlClient;

namespace MechAppBackend.features
{
    public class services
    {
        private MechAppContext _context;

        public services(MechAppContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieves a list of all services.
        /// </summary>
        /// <returns>A list of service objects, each containing the ID, name, duration, price, and active status of a service.</returns>
        public List<serviceOb> GetServices()
        {
            try
            {
                // Query the database for all services
                var serviceList = _context.Services
                    .Select(s => new serviceOb
                    {
                        id = Convert.ToInt32(s.Id),
                        name = s.Name,
                        duration = s.Duration,
                        price = s.Price,
                        isActive = s.IsActive
                    }).ToList();
                // Return the list of services
                return serviceList;
            }
            catch (MySqlException ex)
            {
                // Return the list of services
                Logger.SendException("MechApp", "services", "GetServices", ex);
                return new List<serviceOb>();
            }
        }

        /// <summary>
        /// Retrieves a service by its ID.
        /// </summary>
        /// <param name="id">The ID of the service to be retrieved.</param>
        /// <returns>A service object containing the ID, name, duration, price, and active status of the service, or an object with an ID of -1 if the service does not exist.</returns>
        public serviceOb GetServiceById(int id)
        {
            try
            {
                // Query the database for the service
                var service = (serviceOb)_context.Services.Where(s => s.Id == id);
                // If the service doesn't exist, return an object with an ID of -1
                if (service == null)
                    return new serviceOb
                    {
                        id = -1
                    };
                // Return the service
                return service;
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an object with an ID of -1
                Logger.SendException("MechApp", "services", "GetServiceById", ex);
                return new serviceOb
                {
                    id = -1
                };
            }
        }

        /// <summary>
        /// Adds a new service.
        /// </summary>
        /// <param name="service">The object containing the name, duration, price, and active status of the service to be added.</param>
        /// <returns>A string indicating the success or failure of the operation.</returns>
        public string AddService(addServiceOb service)
        {
            // Validate the service data
            if (string.IsNullOrEmpty(service.name))
                return "no_name";
            if (service.duration == null)
                return "no_duration";
            if (service.price == null)
                return "no_price";
            if (service.isActive == null)
                return "no_isActive";
            // Check if a service with the same name already exists
            try
            {
                var checkName = _context.Services.FirstOrDefault(s => s.Name.ToLower() == service.name.Trim().ToLower());

                if (checkName != null)
                    return "exists";
                // Add the new service to the database
                _context.Services.Add(new Service
                {
                    Name = service.name.Trim(),
                    Duration = service.duration,
                    Price = service.price,
                    IsActive = (short)service.isActive
                });
                // Save the changes to the database
                _context.SaveChanges();

                return "service_added";
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an error response
                Logger.SendException("Mechapp", "services", "AddService", ex);
                return "error";
            }
        }

        /// <summary>
        /// Edits an existing service.
        /// </summary>
        /// <param name="service">The object containing the ID, name, duration, price, and active status of the service to be edited.</param>
        /// <returns>A string indicating the success or failure of the operation.</returns>
        public string EditService(serviceOb service)
        {
            // Validate the service data
            if (service.id == null)
                return "no_id";
            if (string.IsNullOrEmpty(service.name))
                return "no_name";
            if (service.duration == null)
                return "no_duration";
            if (service.price == null)
                return "no_price";
            if (service.isActive == null)
                return "no_isActive";

            try
            {
                // Get the service from the database
                var serviceDb = _context.Services.FirstOrDefault(s => s.Id == service.id);
                // If the service doesn't exist, return an error response
                if (serviceDb == null)
                    return "error";
                // Update the service data
                serviceDb.Name = service.name.Trim();
                serviceDb.Duration = service.duration;
                serviceDb.Price = service.price;
                serviceDb.IsActive = (short)service.isActive;
                // Save the changes to the database
                _context.SaveChanges();

                return "service_edited";
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an error response
                Logger.SendException("MechApp", "services", "EditService", ex);
                return "error";
            }
        }

        /// <summary>
        /// Deletes an existing service.
        /// </summary>
        /// <param name="id">The ID of the service to be deleted.</param>
        /// <returns>A string indicating the success or failure of the operation.</returns>
        public string DeleteService(int id)
        {
            // Validate the service ID
            if (id == null)
                return "no_id";

            try
            {
                // Get the service from the database
                var service = _context.Services.FirstOrDefault(s => s.Id == id);
                // If the service doesn't exist, return an error response
                if (service == null)
                    return "error";
                // Remove the service from the database
                _context.Services.Remove(service);
                // Save the changes to the database
                _context.SaveChanges();

                return "service_deleted";
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an error response
                Logger.SendException("MechApp", "services", "DeleteService", ex);
                return "error";
            }
        }

        /// <summary>
        /// Deletes multiple existing services.
        /// </summary>
        /// <param name="ids">A list of IDs of the services to be deleted.</param>
        /// <returns>A string indicating the success or failure of the operation.</returns>
        public string DeleteServices(List<int> ids)
        {
            // Validate the service IDs
            if (ids.Count == 0)
                return "no_ids";

            try
            {
                // Remove the services from the database
                _context.Services.RemoveRange(_context.Services.Where(s => ids.Contains((int)s.Id)));
                // Save the changes to the database
                _context.SaveChanges();

                return "services_deleted";
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an error response
                Logger.SendException("MechApp", "services", "DeleteServices", ex);
                return "error";
            }
        }
    }

    public class serviceOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public float? duration { get; set; }
        public decimal? price { get; set; }
        public int? isActive { get; set; }
    }

    public class addServiceOb
    {
        public string? name { get; set; }
        public float? duration { get; set; }
        public decimal? price { get; set; }
        public int? isActive { get; set; }
    }
}
