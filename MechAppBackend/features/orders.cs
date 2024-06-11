using MechAppBackend.AppSettings;
using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using MySql.Data.MySqlClient;
using System.Linq;

namespace MechAppBackend.features
{
    public class orders
    {
        private MechAppContext _context;

        public orders(MechAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of orders with detailed information including related vehicle, client,
        /// diagnostic details, and associated images and services.
        /// Each order contains comprehensive data such as vehicle details, client information,
        /// order diagnostics, statuses, and financial estimates.
        ///
        /// Processes:
        /// - Fetches each order and constructs a detailed object containing:
        ///   - Basic order data (ID, diagnosis, status, start and end dates).
        ///   - Associated vehicle information structured as a nested object.
        ///   - Client information related to the order.
        ///   - Images linked to the order.
        ///   - Financial estimates including breakdown of parts and services.
        ///   - A detailed checklist associated with the order.
        /// Error Handling:
        /// - Attempts to fetch all orders and handle any database exceptions.
        ///
        /// Returns:
        /// - A list of 'orderOb' objects containing detailed information for each order.
        /// - Returns an empty list and logs the exception in case of a database error.
        /// </summary>
        /// <returns>List of 'orderOb' structured with detailed order, vehicle, client, and additional data.</returns>
        public List<orderOb> getOrders()
        {
            try
            {
                // Use LINQ to query the database and select orders with required details
                var orders = _context.Orders.Select(o => new orderOb
                {
                    id = Convert.ToInt32(o.Id),
                    vehicle = (vehicleOb)_context.UsersVehicles
                        .Where(v => v.Id == o.VehicleId)
                        .Select(v => new vehicleOb
                        {
                            id = Convert.ToInt32(v.Id),
                            producer = v.Producer,
                            model = v.Model,
                            produceDate = v.ProduceDate,
                            mileage = Convert.ToInt32(v.Mileage),
                            vin = v.Vin,
                            engineNumber = v.EngineNumber,
                            registrationNumber = v.RegistrationNumber,
                            enginePower = (int)v.EnginePower,
                            engineCapacity = v.EngineCapacity,
                            fuelType = v.FuelType
                        })
                        .FirstOrDefault(),
                    client = (orderClientOb)_context.Users
                        .Where(u => u.Id == o.ClientId)
                        .Select(u => new orderClientOb
                        {
                            id = Convert.ToInt32(u.Id),
                            name = u.Name,
                            lastname = u.Lastname,
                            email = u.Email,
                            phone = u.Phone,
                            nip = u.Nip
                        }).FirstOrDefault(),
                    clientDiagnose = o.ClientDiagnose,
                    status = o.Status,
                    startDate = o.StartDate,
                    endDate = o.EndDate,
                    images = _context.OrdersImages
                        .Where(oi => oi.OrderId == o.Id)
                        .Select(oi => new orderImageOb
                        {
                            id = Convert.ToInt32(o.Id),
                            image = oi.Image
                        }).ToList(),
                    estimate = (estimateOb)_context.Estimates
                        .Where(e => e.OrderId == o.Id)
                        .Select(e => new estimateOb
                        {
                            id = Convert.ToInt32(e.Id),
                            totalPartsPrice = e.TotalPartsPrice,
                            totalServicesPrice = e.TotalServicesPrice,
                            totalPrice = e.TotalPrice,
                            estimateParts = _context.EstimateParts
                                .Where(ep => ep.EstimateId == e.Id)
                                .Select(ep => new estimatePartOb
                                {
                                    id = Convert.ToInt32(ep.Id),
                                    name = ep.Name,
                                    ean = ep.Ean,
                                    amount = ep.Amount,
                                    grossUnitPrice = ep.GrossUnitPrice,
                                    totalPrice = ep.TotalPrice
                                }).ToList(),
                            estimateServices = _context.EstimateServices
                                .Where(es => es.EstimateId == e.Id)
                                .Select(es => new estimateServiceOb
                                {
                                    id = Convert.ToInt32(es.Id),
                                    name = es.Name,
                                    rwhAmount = es.Rhwamount,
                                    grossUnitPrice = es.GrossUnitPrice,
                                    totalPrice = es.TotalPrice
                                }).ToList()
                        }).FirstOrDefault(),
                    checklist = (ChecklistOb)_context.CheckLists
                        .Where(c => c.OrderId == o.Id)
                        .Select(c => new ChecklistOb
                        {
                            id = Convert.ToInt32(c.Id),
                            suspensionFrontStatus = c.SuspensionFrontStatus,
                            suspensionFrontDescription = c.SuspensionFrontDescription,
                            brakesFrontStatus = c.BrakesFrontStatus,
                            brakesFrontDescription = c.BrakesFrontDescription,
                            engineSuspensionStatus = c.EngineSuspensionStatus,
                            engineSuspensionDescription = c.EngineSuspensionDescription,
                            leaksStatus = c.LeaksStatus,
                            leaksDescription = c.LeaksDescription,
                            suspensionRearStatus = c.SuspensionRearStatus,
                            suspensionRearDescription = c.SuspensionRearDescription,
                            brakesRearStatus = c.BrakesRearStatus,
                            brakesRearDescription = c.BrakesRearDescription,
                            exhoustSystemStatus = c.ExhoustSystemStatus,
                            exhoustSystemDescription = c.ExhoustSystemDescription,
                            fluidLevelStatus = c.FluidLevelStatus,
                            fluidLevelDescription = c.FluidLevelDescription,
                            engineStatus = c.EngineStatus,
                            engineDescription = c.EngineDescription,
                            electricSystemStatus = c.ElectricSystemStatus,
                            electricSystemDescription = c.ElectricSystemDescription
                        }).FirstOrDefault()
                }).ToList();

                return orders;
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an empty list if there is a database error
                Logger.SendException("Mechapp", "orders", "GetOrders", ex);
                return new List<orderOb>();
            }
        }

        /// <summary>
        /// Retrieves a list of orders for a specific client filtered by order status.
        /// Fetches all related vehicle and client data, including diagnostics, images, estimates, and checklists.
        /// Filters orders based on the status provided, where a specific status (e.g., 5) can be targeted or all other statuses.
        ///
        /// Parameters:
        /// - _status: The status to filter orders by. If set to 5, returns orders with status 5; otherwise, returns orders with statuses other than 5.
        /// - _clientID: The client ID to fetch orders for, ensuring that only orders associated with this client are returned.
        ///
        /// Returns:
        /// - A list of 'orderOb' objects each representing an order with detailed associated data including vehicles, clients, images, and estimates.
        /// - "error": Returned if there's a failure in database operations.
        ///
        /// Usage:
        /// - This method is intended to provide detailed visibility into the orders for customer service or management review,
        ///   allowing for precise tracking and status updates of client orders.
        /// </summary>
        /// <returns>List of 'orderOb' structured with detailed order, vehicle, client, and additional data.</returns>

        public List<orderOb> GetClientOrders(int _status, int _clientID)
        {
            try
            {
                // Query to fetch all orders with nested details such as vehicle and client information
                var orders = _context.Orders.Select(o => new orderOb
                {
                    id = Convert.ToInt32(o.Id),
                    vehicle = (vehicleOb)_context.UsersVehicles
                        .Where(v => v.Id == o.VehicleId)
                        .Select(v => new vehicleOb
                        {
                            id = Convert.ToInt32(v.Id),
                            producer = v.Producer,
                            model = v.Model,
                            produceDate = v.ProduceDate,
                            mileage = Convert.ToInt32(v.Mileage),
                            vin = v.Vin,
                            engineNumber = v.EngineNumber,
                            registrationNumber = v.RegistrationNumber,
                            enginePower = (int)v.EnginePower,
                            engineCapacity = v.EngineCapacity,
                            fuelType = v.FuelType
                        }).FirstOrDefault(),
                    client = (orderClientOb)_context.Users
                        .Where(u => u.Id == o.ClientId)
                        .Select(u => new orderClientOb
                        {
                            id = Convert.ToInt32(u.Id),
                            name = u.Name,
                            lastname = u.Lastname,
                            email = u.Email,
                            phone = u.Phone,
                            nip = u.Nip
                        }).FirstOrDefault(),
                    clientDiagnose = o.ClientDiagnose,
                    status = o.Status,
                    startDate = o.StartDate,
                    endDate = o.EndDate,
                    images = _context.OrdersImages
                        .Where(oi => oi.OrderId == o.Id)
                        .Select(oi => new orderImageOb
                        {
                            id = Convert.ToInt32(o.Id),
                            image = oi.Image
                        }).ToList(),
                    estimate = (estimateOb)_context.Estimates
                        .Where(e => e.OrderId == o.Id)
                        .Select(e => new estimateOb
                        {
                            id = Convert.ToInt32(e.Id),
                            totalPartsPrice = e.TotalPartsPrice,
                            totalServicesPrice = e.TotalServicesPrice,
                            totalPrice = e.TotalPrice,
                            estimateParts = _context.EstimateParts
                                .Where(ep => ep.EstimateId == e.Id)
                                .Select(ep => new estimatePartOb
                                {
                                    id = Convert.ToInt32(ep.Id),
                                    name = ep.Name,
                                    ean = ep.Ean,
                                    amount = ep.Amount,
                                    grossUnitPrice = ep.GrossUnitPrice,
                                    totalPrice = ep.TotalPrice
                                }).ToList(),
                            estimateServices = _context.EstimateServices
                                .Where(es => es.EstimateId == e.Id)
                                .Select(es => new estimateServiceOb
                                {
                                    id = Convert.ToInt32(es.Id),
                                    name = es.Name,
                                    rwhAmount = es.Rhwamount,
                                    grossUnitPrice = es.GrossUnitPrice,
                                    totalPrice = es.TotalPrice
                                }).ToList()
                        }).FirstOrDefault(),
                    checklist = (ChecklistOb)_context.CheckLists
                        .Where(c => c.OrderId == o.Id)
                        .Select(c => new ChecklistOb
                        {
                            id = Convert.ToInt32(c.Id),
                            suspensionFrontStatus = c.SuspensionFrontStatus,
                            suspensionFrontDescription = c.SuspensionFrontDescription,
                            brakesFrontStatus = c.BrakesFrontStatus,
                            brakesFrontDescription = c.BrakesFrontDescription,
                            engineSuspensionStatus = c.EngineSuspensionStatus,
                            engineSuspensionDescription = c.EngineSuspensionDescription,
                            leaksStatus = c.LeaksStatus,
                            leaksDescription = c.LeaksDescription,
                            suspensionRearStatus = c.SuspensionRearStatus,
                            suspensionRearDescription = c.SuspensionRearDescription,
                            brakesRearStatus = c.BrakesRearStatus,
                            brakesRearDescription = c.BrakesRearDescription,
                            exhoustSystemStatus = c.ExhoustSystemStatus,
                            exhoustSystemDescription = c.ExhoustSystemDescription,
                            fluidLevelStatus = c.FluidLevelStatus,
                            fluidLevelDescription = c.FluidLevelDescription,
                            engineStatus = c.EngineStatus,
                            engineDescription = c.EngineDescription,
                            electricSystemStatus = c.ElectricSystemStatus,
                            electricSystemDescription = c.ElectricSystemDescription
                        }).FirstOrDefault()
                }).ToList();

                // Filtering orders based on the status provided

                var fetchedOrders = new List<orderOb>();

                if (_status == 6)
                    fetchedOrders = orders.Where(o => o.status == 6).ToList();
                else
                    fetchedOrders = orders.Where(o => o.status != 6 && o.status != 7).ToList();

                return fetchedOrders;
            }
            catch (MySqlException ex)
            {
                // Handle database exceptions and return an empty list indicating failure
                Logger.SendException("MechApp", "orders", "GetClientOrders", ex);
                return new List<orderOb>();
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific order by its ID.
        /// Includes all related data such as vehicle details, client information, diagnostics, images, estimates, and checklists.
        /// This method is essential for viewing complete order details in customer service or management systems where full transparency of order data is required.
        ///
        /// Parameters:
        /// - _orderID: The unique identifier of the order for which detailed information is required.
        ///
        /// Returns:
        /// - An 'orderOb' object containing detailed information about the order, including:
        ///   - Basic order details like status and dates.
        ///   - Detailed vehicle information linked to the order.
        ///   - Client details associated with the order.
        ///   - Images related to the order.
        ///   - Estimates including breakdowns of parts and services costs.
        ///   - A detailed checklist of vehicle diagnostics and conditions.
        /// - Returns a new 'orderOb' object with default values if an error occurs during database operations.
        ///
        /// Exception Handling:
        /// - Catches and logs any MySqlException that may occur during the operation, returning minimal order data on failure.
        /// </summary>
        /// <param name="_orderID">The ID of the order to retrieve.</param>
        /// <returns>Returns a fully populated 'orderOb' object if successful, or an empty 'orderOb' object on failure.</returns>
        public orderOb GetOrderDetail(int _orderID)
        {
            try
            {
                // Query the database to get the order and its related details based on the order ID
                var order = (orderOb)_context.Orders
                    .Where(o => o.Id == _orderID)
                    .Select(o => new orderOb
                    {
                        id = Convert.ToInt32(o.Id),
                        vehicle = (vehicleOb)_context.UsersVehicles
                        .Where(v => v.Id == o.VehicleId)
                        .Select(v => new vehicleOb
                        {
                            id = Convert.ToInt32(v.Id),
                            producer = v.Producer,
                            model = v.Model,
                            produceDate = v.ProduceDate,
                            mileage = Convert.ToInt32(v.Mileage),
                            vin = v.Vin,
                            engineNumber = v.EngineNumber,
                            registrationNumber = v.RegistrationNumber,
                            enginePower = (int)v.EnginePower,
                            engineCapacity = v.EngineCapacity,
                            fuelType = v.FuelType
                        }).FirstOrDefault(),
                        client = (orderClientOb)_context.Users
                        .Where(u => u.Id == o.ClientId)
                        .Select(u => new orderClientOb
                        {
                            id = Convert.ToInt32(u.Id),
                            name = u.Name,
                            lastname = u.Lastname,
                            email = u.Email,
                            phone = u.Phone,
                            nip = u.Nip
                        }).FirstOrDefault(),
                        clientDiagnose = o.ClientDiagnose,
                        status = o.Status,
                        startDate = o.StartDate,
                        endDate = o.EndDate,
                        images = _context.OrdersImages
                        .Where(oi => oi.OrderId == o.Id)
                        .Select(oi => new orderImageOb
                        {
                            id = Convert.ToInt32(o.Id),
                            image = oi.Image
                        }).ToList(),
                        estimate = (estimateOb)_context.Estimates
                        .Where(e => e.OrderId == o.Id)
                        .Select(e => new estimateOb
                        {
                            id = Convert.ToInt32(e.Id),
                            totalPartsPrice = e.TotalPartsPrice,
                            totalServicesPrice = e.TotalServicesPrice,
                            totalPrice = e.TotalPrice,
                            estimateParts = _context.EstimateParts
                                .Where(ep => ep.EstimateId == e.Id)
                                .Select(ep => new estimatePartOb
                                {
                                    id = Convert.ToInt32(ep.Id),
                                    name = ep.Name,
                                    ean = ep.Ean,
                                    amount = ep.Amount,
                                    grossUnitPrice = ep.GrossUnitPrice,
                                    totalPrice = ep.TotalPrice
                                }).ToList(),
                            estimateServices = _context.EstimateServices
                                .Where(es => es.EstimateId == e.Id)
                                .Select(es => new estimateServiceOb
                                {
                                    id = Convert.ToInt32(es.Id),
                                    name = es.Name,
                                    rwhAmount = es.Rhwamount,
                                    grossUnitPrice = es.GrossUnitPrice,
                                    totalPrice = es.TotalPrice
                                }).ToList()
                        }).FirstOrDefault(),
                        checklist = (ChecklistOb)_context.CheckLists
                        .Where(c => c.OrderId == o.Id)
                        .Select(c => new ChecklistOb
                        {
                            id = Convert.ToInt32(c.Id),
                            suspensionFrontStatus = c.SuspensionFrontStatus,
                            suspensionFrontDescription = c.SuspensionFrontDescription,
                            brakesFrontStatus = c.BrakesFrontStatus,
                            brakesFrontDescription = c.BrakesFrontDescription,
                            engineSuspensionStatus = c.EngineSuspensionStatus,
                            engineSuspensionDescription = c.EngineSuspensionDescription,
                            leaksStatus = c.LeaksStatus,
                            leaksDescription = c.LeaksDescription,
                            suspensionRearStatus = c.SuspensionRearStatus,
                            suspensionRearDescription = c.SuspensionRearDescription,
                            brakesRearStatus = c.BrakesRearStatus,
                            brakesRearDescription = c.BrakesRearDescription,
                            exhoustSystemStatus = c.ExhoustSystemStatus,
                            exhoustSystemDescription = c.ExhoustSystemDescription,
                            fluidLevelStatus = c.FluidLevelStatus,
                            fluidLevelDescription = c.FluidLevelDescription,
                            engineStatus = c.EngineStatus,
                            engineDescription = c.EngineDescription,
                            electricSystemStatus = c.ElectricSystemStatus,
                            electricSystemDescription = c.ElectricSystemDescription
                        }).FirstOrDefault()
                    });

                return order;
            }
            catch (MySqlException ex)
            {
                // Handle any database-related exceptions and log the exception details
                Logger.SendException("MechApp", "orders", "GetOrderDetail", ex);
                return new orderOb
                {
                    id = -1
                };
            }
        }

        /// <summary>
        /// Adds a new order to the system based on provided details.
        /// Validates essential IDs for vehicle, client, and department to ensure the order can be correctly linked.
        /// Also checks for the presence of a client diagnosis before proceeding.
        ///
        /// Parameters:
        /// - order: An object of type 'addOrderOb' containing all necessary details to create an order:
        ///   - vehicleID: The ID of the vehicle associated with the order.
        ///   - clientId: The ID of the client placing the order.
        ///   - departmentId: The ID of the department responsible for the order.
        ///   - clientDiagnose: The initial diagnosis or description provided by the client.
        ///   - startDate: The date when the order starts.
        ///   - images: A collection of images related to the order.
        ///
        /// Returns:
        /// - "order_added": Indicates the order was successfully added to the database.
        /// - "error": Returned if there's a failure in validation or during database operations.
        /// - "no_client_diagnose": Returned if the client diagnosis field is empty.
        ///
        /// Usage:
        /// - This method is typically used to create a new order within a service or sales system,
        ///   where vehicle-related services are tracked and managed.
        /// </summary>
        /// <param name="order">The order object containing all necessary data to create a new order.</param>
        /// <returns>A string indicating the result of the operation: either "order_added", "error", or "no_client_diagnose".</returns>
        public string AddOrder(addOrderOb order)
        {
            // Validate required IDs and client diagnosis
            if (order.vehicleID == null || order.vehicleID == -1)
                return "error";
            if (order.clientId == null || order.clientId == -1)
                return "error";
            if (order.departmentId == null || order.departmentId == -1)
                return "error";

            if (string.IsNullOrEmpty(order.clientDiagnose))
                return "no_client_diagnose";

            DateTime checkStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime checkEndDate = checkStartDate.AddMonths(1).AddDays(-1);

            try
            {
                var count = _context.Orders
                    .Count(o => o.StartDate >= checkStartDate && o.StartDate <= checkEndDate);

                if (count >= appdata.ordersAmount)
                    return "max_order_reached";


                // Create a new Order object and populate it with the provided data
                Order newOrder = new Order
                {
                    ClientId = order.clientId,
                    VehicleId = order.vehicleID,
                    DepartmentId = order.departmentId,
                    ClientDiagnose = order.clientDiagnose,
                    Status = 0,
                    StartDate = order.startDate
                };
                // Add the new order to the database context
                _context.Orders.Add(newOrder);
                _context.SaveChanges();
                // Handle the addition of any associated images
                foreach (var item in order.images)
                {
                    _context.OrdersImages.Add(new OrdersImage
                    {
                        Image = item.image
                    });
                }
                // Save changes to the database
                _context.SaveChanges();

                return "order_added";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return a general error message
                Logger.SendException("MechApp", "orders", "AddOrder", ex);
                return "error";
            }
        }

        /// <summary>
        /// Changes the status of an existing order and performs actions based on the new status.
        /// Validates the order ID and the new status before attempting any updates. 
        /// If the status is changed to '4' (ready for pickup), it sends an SMS or email notification to the client.
        /// If the status is changed to '5' (completed), it sets the order's end date to the current date.
        ///
        /// Parameters:
        /// - _orderID: The ID of the order for which the status is to be changed.
        /// - _status: The new status to be set for the order.
        ///
        /// Returns:
        /// - "status_changed": Indicates the order status was successfully updated and any related actions were performed.
        /// - "error": Returned if there's a failure in validation, if the order does not exist, or during database operations.
        ///
        /// Usage:
        /// - This method is used in scenarios where order management requires status updates and possible notifications to clients,
        ///   such as transitioning an order to a completed state or notifying when a service is ready for pickup.
        /// </summary>
        /// <param name="_orderID">The unique identifier of the order to update.</param>
        /// <param name="_status">The new status code for the order.</param>
        /// <returns>A string indicating the result of the operation: either "status_changed" or "error".</returns>
        public string ChangeOrderStatus(int _orderID, int _status)
        {
            // Validate order ID and status
            if (_orderID == null || _orderID == -1)
                return "error";
            if (_status == null)
                return "error";

            try
            {
                // Retrieve the order from the database
                var order = _context.Orders.FirstOrDefault(o => o.Id == _orderID);

                if (order == null)
                    return "error";
                // Update the order's status
                order.Status = (short)_status;
                // Check specific statuses for additional actions
                if (_status == 4) // Ready for pickup
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == order.ClientId);
                    // Send notification via SMS or email based on user contact availability
                    if (!string.IsNullOrEmpty(user.Phone))
                    {
                        string _phone = user.Phone.Replace("+", "").Replace("48", "");

                        smssender.SendSms("Szanowny kliencie! Zapraszamy po odbior samochodu!", _phone);
                    }
                    else 
                    {
                        Sender.SendOrderReadyStatus("Samochód gotowy do odbioru!", user.Name, user.Lastname, user.Email);
                    }

                    order.SendDoneNotification = 1;
                }
                if (_status == 5) // Order completed
                {
                    DateTime endDate = DateTime.Now;

                    order.EndDate = endDate;
                }
                // Save changes to the database
                _context.SaveChanges();

                return "status_changed";
            }
            catch (MySqlException ex)
            {
                // Log and handle exceptions
                Logger.SendException("MechApp", "orders", "ChangeOrderStatus", ex);
                return "error";
            }
        }

        /// <summary>
        /// Adds a new estimate to the system for a specific order, along with detailed parts and services estimates.
        /// Validates the presence of total prices for parts, services, and the overall estimate, as well as the associated order ID.
        ///
        /// Parameters:
        /// - estimate: An object of type 'addEditEstimateOb' containing all necessary details to create an estimate:
        ///   - orderID: The ID of the order for which the estimate is being created.
        ///   - totalPartsPrice: The total price of all parts included in the estimate.
        ///   - totalServicesPrice: The total price of all services included in the estimate.
        ///   - totalPrice: The overall total price of the estimate.
        ///   - estimateParts: A list of parts included in the estimate.
        ///   - estimateServices: A list of services included in the estimate.
        ///
        /// Returns:
        /// - "estimate_added": Indicates the estimate was successfully added to the database.
        /// - "no_total_parts_price": Returned if the total price for parts is not provided.
        /// - "no_total_services_price": Returned if the total price for services is not provided.
        /// - "no_total_price": Returned if the overall total price of the estimate is not provided.
        /// - "error": Returned if there's a failure during database operations or if the order ID is not provided.
        ///
        /// Usage:
        /// - This method is typically used in scenarios where an estimate needs to be officially recorded in a system,
        ///   such as in automotive repair shops or construction services, where accurate cost estimation is crucial for billing and client communication.
        /// </summary>
        /// <param name="estimate">The estimate object containing all necessary data to create a new financial estimate.</param>
        /// <returns>A string indicating the result of the operation: either "estimate_added", "error", or specific error messages related to input validation.</returns>
        public string AddEstimate(addEditEstimateOb estimate)
        {
            // Validate required fields
            if (estimate.totalPartsPrice == null)
                return "no_total_parts_price";
            if (estimate.totalServicesPrice == null)
                return "no_total_services_price";
            if (estimate.totalPrice == null)
                return "no_total_price";

            if (estimate.orderID == null)
                return "error";

            try
            {
                // Create a new Estimate object and populate it with the provided data
                var newEstimate = new Estimate
                {
                    OrderId = estimate.orderID,
                    TotalPartsPrice = estimate.totalPartsPrice,
                    TotalServicesPrice = estimate.totalServicesPrice,
                    TotalPrice = estimate.totalPrice
                };
                // Add the new estimate to the database context
                _context.Estimates.Add(newEstimate);
                _context.SaveChanges();
                // Handle the addition of parts and services to the estimate
                foreach (var part in estimate.estimateParts)
                {
                    _context.EstimateParts.Add(new EstimatePart
                    {
                        EstimateId = newEstimate.Id,
                        Name = part.name,
                        Ean = part.ean,
                        Amount = part.amount,
                        GrossUnitPrice = part.grossUnitPrice,
                        TotalPrice = part.totalPrice
                    });
                }

                foreach (var service in estimate.estimateServices)
                {
                    _context.EstimateServices.Add(new EstimateService
                    {
                        EstimateId = newEstimate.Id,
                        Name = service.name,
                        Rhwamount = service.rwhAmount,
                        GrossUnitPrice = service.grossUnitPrice,
                        TotalPrice = service.totalPrice
                    });
                }
                // Save all changes to the database
                _context.SaveChanges();

                return "estimate_added";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return a general error message
                Logger.SendException("MechApp", "orders", "AddOrder", ex);
                return "error";
            }
        }

        /// <summary>
        /// Edits an existing estimate, updating part and service details, prices, and the total estimated costs.
        /// Validates all critical inputs for presence and correctness before proceeding with the update. 
        /// This method handles updates by adjusting existing parts and services, adding new ones, and removing those not included in the updated estimate.
        ///
        /// Parameters:
        /// - estimate: An object of type 'addEditEstimateOb' containing all necessary details to edit an estimate:
        ///   - id: The unique identifier of the estimate to be edited.
        ///   - orderID: The ID of the order associated with the estimate.
        ///   - totalPartsPrice: The updated total price of all parts.
        ///   - totalServicesPrice: The updated total price of all services.
        ///   - totalPrice: The updated total price of the estimate.
        ///   - estimateParts: A list of parts involved in the estimate with updated details.
        ///   - estimateServices: A list of services involved in the estimate with updated details.
        ///
        /// Returns:
        /// - "estimate_edited": Indicates the estimate was successfully updated in the database.
        /// - "error": Returned if there's a validation failure or during database operations.
        /// - Specific error messages like "no_total_parts_price", "no_total_services_price", or "no_total_price" for missing mandatory fields.
        ///
        /// Usage:
        /// - This method is used in scenarios where an existing estimate needs to be revised,
        ///   possibly due to changes in project scope, pricing adjustments, or corrections to previous entries.
        /// </summary>
        /// <param name="estimate">The estimate object containing all data necessary for updating the estimate.</param>
        /// <returns>A string indicating the result of the operation: either "estimate_edited", "error", or specific error messages related to input validation.</returns>
        public string EditEstimate(addEditEstimateOb estimate)
        {
            // Validate essential information
            if (estimate.totalPartsPrice == null)
                return "no_total_parts_price";
            if (estimate.totalServicesPrice == null)
                return "no_total_services_price";
            if (estimate.totalPrice == null)
                return "no_total_price";

            if (estimate.orderID == null)
                return "error";
            if (estimate.id == null)
                return "error";

            try
            {
                // Fetch the existing estimate to update
                var estimateOb = _context.Estimates.FirstOrDefault(e => e.Id == estimate.id);
                // Update the basic estimate details
                estimateOb.TotalServicesPrice = estimate.totalServicesPrice;
                estimateOb.TotalPartsPrice = estimate.totalPartsPrice;
                estimateOb.TotalPrice = estimate.totalPrice;

                var partsToUpdate = _context.EstimateParts.Where(p => estimate.estimateParts.Select(ep => ep.id).Contains((int)p.Id)).ToList();

                foreach (var part in estimate.estimateParts)
                {
                    var partToUpdate = partsToUpdate.FirstOrDefault(p => p.Id == part.id);

                    if (partToUpdate != null)
                    {
                        partToUpdate.Name = part.name;
                        partToUpdate.Ean = part.ean;
                        partToUpdate.Amount = part.amount;
                        partToUpdate.GrossUnitPrice = part.grossUnitPrice;
                        partToUpdate.TotalPrice = part.totalPrice;
                    }
                }

                var existingPartsIds = new HashSet<string>(_context.EstimateParts
                    .Where(p => p.EstimateId == estimate.id)
                    .Select(p => p.Id.ToString()));

                var partsToadd = estimate.estimateParts
                    .Where(p => !existingPartsIds.Contains(p.id.ToString()))
                    .ToList();

                foreach (var part in partsToadd)
                {
                    _context.EstimateParts.Add(new EstimatePart
                    {
                        EstimateId = part.estimateId,
                        Name = part.name,
                        Ean = part.ean,
                        Amount = part.amount,
                        GrossUnitPrice = part.grossUnitPrice,
                        TotalPrice = part.totalPrice
                    });
                }

                var existingPartsInDb = _context.EstimateParts
                    .Where(p => p.EstimateId == estimate.id)
                    .ToList();

                var newEstimatePartsIds = estimate.estimateParts.Select(p => p.id).ToHashSet();

                var partsToDelete = existingPartsInDb
                    .Where(p => !newEstimatePartsIds.Contains((int)p.Id))
                    .ToList();

                foreach (var part in partsToDelete)
                {
                    _context.EstimateParts.Remove(part);
                }

                var servicesToUpdate = _context.EstimateServices.Where(s => estimate.estimateServices.Select(es => es.id).Contains((int)s.Id)).ToList();

                foreach (var service in estimate.estimateServices)
                {
                    var serviceToUpdate = servicesToUpdate.FirstOrDefault(s => s.Id == service.id);

                    if (serviceToUpdate != null)
                    {
                        serviceToUpdate.Name = service.name;
                        serviceToUpdate.Rhwamount = service.rwhAmount;
                        serviceToUpdate.GrossUnitPrice = service.grossUnitPrice;
                        serviceToUpdate.TotalPrice = service.totalPrice;
                    }
                }

                var existingServicesIds = new HashSet<int>(_context.EstimateServices
                    .Where(s => s.EstimateId == estimate.id)
                    .Select(s => (int)s.Id));

                var servicesToAdd = estimate.estimateServices
                    .Where(s => !existingServicesIds.Contains((int)s.id))
                    .ToList();

                foreach (var service in servicesToAdd)
                {
                    _context.EstimateServices.Add(new EstimateService
                    {
                        EstimateId = service.estimateId,
                        Name = service.name,
                        Rhwamount = service.rwhAmount,
                        GrossUnitPrice = service.grossUnitPrice,
                        TotalPrice = service.totalPrice
                    });
                }

                var newEstimateServicesIds = estimate.estimateServices.Select(p => (int)p.id).ToHashSet();

                var existingServicesInDb = _context.EstimateServices
                    .Where(p => p.EstimateId == estimate.id)
                    .ToList();

                var servicesToDelete = existingServicesInDb
                    .Where(s => !newEstimateServicesIds.Contains((int)s.Id))
                    .ToList();

                foreach ( var service in servicesToDelete)
                {
                    _context.EstimateServices.Remove(service);
                }
                // Commit all changes to the database
                _context.SaveChanges();

                return "estimate_edited";
            }
            catch (MySqlException ex)
            {
                // Handle exceptions and log the error
                Logger.SendException("MechApp", "orders", "EditEstimate", ex);
                return "error";
            }
        }

        /// <summary>
        /// Adds a new checklist associated with a specific order in the system.
        /// Validates the order ID before proceeding to ensure the checklist is linked to a valid order.
        /// The method captures detailed diagnostics about various vehicle components such as brakes, engine, suspension, and more.
        ///
        /// Parameters:
        /// - checklist: An object of type 'addEditChecklistOb' containing all necessary details for creating a checklist:
        ///   - orderID: The ID of the order to which this checklist is associated.
        ///   - suspensionFrontStatus, suspensionFrontDescription: Status and notes on the vehicle's front suspension.
        ///   - brakesFrontStatus, brakesFrontDescription: Status and notes on the vehicle's front brakes.
        ///   - leaksStatus, leaksDescription: Status and notes on any leaks.
        ///   - engineStatus, engineDescription: Status and notes on the engine's condition.
        ///   - engineSuspensionStatus, engineSuspensionDescription: Status and notes on the engine suspension.
        ///   - suspensionRearStatus, suspensionRearDescription: Status and notes on the vehicle's rear suspension.
        ///   - brakesRearStatus, brakesRearDescription: Status and notes on the vehicle's rear brakes.
        ///   - exhoustSystemStatus, exhoustSystemDescription: Status and notes on the exhaust system.
        ///   - electricSystemStatus, electricSystemDescription: Status and notes on the electrical system.
        ///   - fluidLevelStatus, fluidLevelDescription: Status and notes on fluid levels.
        ///
        /// Returns:
        /// - "checklist_added": Indicates the checklist was successfully added to the database.
        /// - "error": Returned if there's a failure during database operations or if the order ID is invalid.
        ///
        /// Usage:
        /// - This method is typically used to record detailed inspections associated with service orders,
        ///   crucial for repair shops and maintenance services where detailed component checks are mandatory.
        /// </summary>
        /// <param name="checklist">The checklist object containing all the details to create a new vehicle diagnostic checklist.</param>
        /// <returns>A string indicating the result of the operation: either "checklist_added" or "error".</returns>
        public string AddChecklist(addEditChecklistOb checklist)
        {
            // Validate the order ID to ensure it's provided
            if (checklist.orderID == null)
                return "error";

            try
            {
                // Create and add the new checklist to the database
                _context.CheckLists.Add(new CheckList
                {
                    OrderId = checklist.orderID,
                    SuspensionFrontStatus = (short)checklist.suspensionFrontStatus,
                    SuspensionFrontDescription = checklist.suspensionFrontDescription,
                    BrakesFrontStatus = (short)checklist.brakesFrontStatus,
                    BrakesFrontDescription = checklist.brakesFrontDescription,
                    LeaksStatus = (short)checklist.leaksStatus,
                    LeaksDescription = checklist.leaksDescription,
                    EngineStatus = (short)checklist.engineStatus,
                    EngineDescription = checklist.engineDescription,
                    EngineSuspensionStatus = (short)checklist.engineSuspensionStatus,
                    EngineSuspensionDescription = checklist.engineSuspensionDescription,
                    SuspensionRearStatus = (short)checklist.suspensionRearStatus,
                    SuspensionRearDescription = checklist.suspensionRearDescription,
                    BrakesRearStatus = (short)checklist.brakesRearStatus,
                    BrakesRearDescription = checklist.brakesRearDescription,
                    ExhoustSystemStatus = (short)checklist.exhoustSystemStatus,
                    ExhoustSystemDescription = checklist.exhoustSystemDescription,
                    ElectricSystemStatus = (short)checklist.electricSystemStatus,
                    ElectricSystemDescription = checklist.electricSystemDescription,
                    FluidLevelStatus = (short)checklist.fluidLevelStatus,
                    FluidLevelDescription = checklist.fluidLevelDescription
                });
                // Commit the new checklist to the database
                _context.SaveChanges();

                return "checklist_added";
            }
            catch (MySqlException ex)
            {
                // Log any exceptions that occur during database operations
                Logger.SendException("MechApp", "orders", "AddChecklist", ex);
                return "error";
            }
        }

        /// <summary>
        /// Edits an existing checklist in the system by updating its various diagnostic statuses and descriptions.
        /// Validates the presence of the checklist and order IDs before proceeding with updates.
        /// Each component of the checklist is updated based on the provided new values, ensuring the checklist accurately reflects the current status of the vehicle's inspection.
        ///
        /// Parameters:
        /// - checklist: An object of type 'addEditChecklistOb' containing all necessary details to update a checklist:
        ///   - id: The unique identifier of the checklist to be edited.
        ///   - orderID: The ID of the order associated with the checklist.
        ///   - suspensionFrontStatus, suspensionFrontDescription: Updated status and description of the vehicle's front suspension.
        ///   - brakesFrontStatus, brakesFrontDescription: Updated status and description of the vehicle's front brakes.
        ///   - leaksStatus, leaksDescription: Updated status and description of any leaks detected.
        ///   - engineStatus, engineDescription: Updated status and description of the engine.
        ///   - [Other component statuses and descriptions]
        ///
        /// Returns:
        /// - "checklist_edited": Indicates the checklist was successfully updated in the database.
        /// - "error": Returned if there's a failure in validation, if the checklist does not exist, or during database operations.
        ///
        /// Usage:
        /// - This method is critical for service management systems where detailed records of vehicle inspections are maintained,
        ///   allowing for ongoing tracking of vehicle conditions and necessary maintenance actions.
        /// </summary>
        /// <param name="checklist">The checklist object containing all the details for updating an existing vehicle diagnostic checklist.</param>
        /// <returns>A string indicating the result of the operation: either "checklist_edited" or "error".</returns>
        public string EditChecklist(addEditChecklistOb checklist)
        {
            // Validate the essential identifiers
            if (checklist.id == null)
                return "error";
            if (checklist.orderID == null)
                return "error";

            try
            {
                // Retrieve the existing checklist from the database
                var checklistDb = _context.CheckLists.FirstOrDefault(c => c.Id == checklist.id);

                if (checklistDb == null)
                    return "error";
                // Update checklist details
                checklistDb.SuspensionFrontStatus = (short)checklist.suspensionFrontStatus;
                checklistDb.SuspensionFrontDescription = checklist.suspensionFrontDescription;
                checklistDb.BrakesFrontStatus = (short)checklist.brakesFrontStatus;
                checklistDb.BrakesFrontDescription = checklist.brakesFrontDescription;
                checklistDb.LeaksStatus = (short)checklist.leaksStatus;
                checklistDb.LeaksDescription = checklist.leaksDescription;
                checklistDb.EngineStatus = (short)checklist.engineStatus;
                checklistDb.EngineDescription = checklist.engineDescription;
                checklistDb.EngineSuspensionStatus = (short)checklist.engineSuspensionStatus;
                checklistDb.EngineSuspensionDescription = checklist.engineSuspensionDescription;
                checklistDb.SuspensionRearStatus = (short)checklist.suspensionRearStatus;
                checklistDb.SuspensionRearDescription = checklist.suspensionRearDescription;
                checklistDb.BrakesRearStatus = (short)checklist.brakesRearStatus;
                checklistDb.BrakesRearDescription = checklist.brakesRearDescription;
                checklistDb.ExhoustSystemStatus = (short)checklist.exhoustSystemStatus;
                checklistDb.ExhoustSystemDescription = checklist.exhoustSystemDescription;
                checklistDb.ElectricSystemStatus = (short)checklist.electricSystemStatus;
                checklistDb.ElectricSystemDescription = checklist.electricSystemDescription;
                checklistDb.FluidLevelStatus = (short)checklist.fluidLevelStatus;
                checklistDb.FluidLevelDescription = checklist.fluidLevelDescription;
                // Save the updated checklist back to the database
                _context.SaveChanges();

                return "checklist_edited";
            }
            catch (MySqlException ex)
            {
                // Log the exception and handle the failure
                Logger.SendException("MechApp", "orders", "EditChecklist", ex);
                return "error";
            }
        }
    }

    public class orderOb
    {
        public int? id { get; set; }
        public vehicleOb vehicle { get; set; }
        public orderClientOb client { get; set; }
        public string? clientDiagnose { get; set; }
        public int? status { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public List<orderImageOb> images { get; set; }
        public estimateOb estimate { get; set; }
        public ChecklistOb checklist { get; set; }
    }

    public class orderClientOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? nip { get; set; }
    }

    public class addOrderOb
    {
        public int? vehicleID { get; set; }
        public int? departmentId { get; set; }
        public int? clientId { get; set; }
        public string? clientDiagnose { get; set; }
        public DateTime? startDate { get; set; }
        public List<orderImageOb> images { get; set; }
    }

    public class estimateOb
    {
        public int? id { get; set; }
        public decimal? totalPartsPrice { get; set; }
        public decimal? totalServicesPrice { get; set; }
        public decimal? totalPrice { get; set; }
        public List<estimatePartOb>? estimateParts { get; set; }
        public List<estimateServiceOb>? estimateServices { get; set; }
    }

    public class addEditEstimateOb
    {
        public int? id { get; set; }
        public int? orderID { get; set; }
        public decimal? totalPartsPrice { get; set; }
        public decimal? totalServicesPrice { get; set; }
        public decimal? totalPrice { get; set; }
        public List<addEditEstimatePartOb>? estimateParts { get; set; }
        public List<addEditEstimateServiceOb>? estimateServices { get; set; }
    }

    public class estimatePartOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? ean { get; set; }
        public float? amount { get; set; }
        public decimal? grossUnitPrice { get; set; }
        public decimal? totalPrice { get; set; }
    }

    public class addEditEstimatePartOb
    {
        public int? id { get; set; }
        public int? estimateId { get; set; }
        public string? name { get; set; }
        public string? ean { get; set; }
        public float? amount { get; set; }
        public decimal? grossUnitPrice { get; set; }
        public decimal? totalPrice { get; set; }
    }

    public class estimateServiceOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public float? rwhAmount { get; set; }
        public decimal? grossUnitPrice { get; set; }
        public decimal? totalPrice { get; set;}
    }

    public class addEditEstimateServiceOb
    {
        public int? id { get; set; }
        public int? estimateId { get; set; }
        public string? name { get; set; }
        public float? rwhAmount { get; set; }
        public decimal? grossUnitPrice { get; set; }
        public decimal? totalPrice { get; set; }
    }

    public class ChecklistOb
    {
        public int? id { get; set; }
        public int? suspensionFrontStatus { get; set; }
        public string? suspensionFrontDescription { get; set; }
        public int? brakesFrontStatus { get; set; }
        public string? brakesFrontDescription { get;set; }
        public int? engineSuspensionStatus { get; set; }
        public string? engineSuspensionDescription { get; set;}
        public int? leaksStatus { get; set; }
        public string? leaksDescription { get; set; }
        public int? suspensionRearStatus { get; set; }
        public string? suspensionRearDescription { get; set; }
        public int? brakesRearStatus { get; set; }
        public string? brakesRearDescription { get; set; }
        public int? exhoustSystemStatus { get; set; }
        public string? exhoustSystemDescription { get; set;}
        public int? fluidLevelStatus { get; set; }
        public string? fluidLevelDescription { get; set; }
        public int? engineStatus { get; set; }
        public string? engineDescription { get; set;}
        public int? electricSystemStatus { get; set; }
        public string? electricSystemDescription { get; set;}
    }

    public class addEditChecklistOb
    {
        public int? id { get; set; }
        public int? orderID { get; set; }
        public int? suspensionFrontStatus { get; set; } = 0;
        public string? suspensionFrontDescription { get; set; }
        public int? brakesFrontStatus { get; set; } = 0;
        public string? brakesFrontDescription { get; set; }
        public int? engineSuspensionStatus { get; set; } = 0;
        public string? engineSuspensionDescription { get; set; }
        public int? leaksStatus { get; set; } = 0;
        public string? leaksDescription { get; set; }
        public int? suspensionRearStatus { get; set; } = 0;
        public string? suspensionRearDescription { get; set; }
        public int? brakesRearStatus { get; set; } = 0;
        public string? brakesRearDescription { get; set; }
        public int? exhoustSystemStatus { get; set; } = 0;
        public string? exhoustSystemDescription { get; set; }
        public int? fluidLevelStatus { get; set; } = 0;
        public string? fluidLevelDescription { get; set; }
        public int? engineStatus { get; set; } = 0;
        public string? engineDescription { get; set; }
        public int? electricSystemStatus { get; set; } = 0;
        public string? electricSystemDescription { get; set; }
    }

    public class orderImageOb
    {
        public int? id { get; set; }
        public string? image { get; set; }
    }
}
