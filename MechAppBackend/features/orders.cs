using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using MySql.Data.MySqlClient;

namespace MechAppBackend.features
{
    public class orders
    {
        private MechAppContext _context;

        public orders(MechAppContext context)
        {
            _context = context;
        }

        public List<orderOb> getOrders()
        {
            try
            {
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
                            enginePower = v.EnginePower,
                            engineCapacity = v.EngineCapacity,
                            fuelType = v.FuelType
                        }),
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
                        }),
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
                        }),
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
                        })
                }).ToList();

                return orders;
            }
            catch (MySqlException ex)
            {
                Logger.SendException("Mechapp", "orders", "GetOrders", ex);
                return new List<orderOb>();
            }
        }

        public List<orderOb> GetClientOrders(int _status, int _clientID)
        {
            try
            {
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
                            enginePower = v.EnginePower,
                            engineCapacity = v.EngineCapacity,
                            fuelType = v.FuelType
                        }),
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
                        }),
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
                        }),
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
                        })
                }).ToList();

                var fetchedOrders = new List<orderOb>();

                if (_status == 5)
                    fetchedOrders = orders.Where(o => o.status == 5).ToList();
                else
                    fetchedOrders = orders.Where(o => o.status != 5).ToList();

                return fetchedOrders;
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "orders", "GetClientOrders", ex);
                return new List<orderOb>();
            }
        }

        public orderOb GetOrderDetail(int _orderID)
        {
            try
            {
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
                            enginePower = v.EnginePower,
                            engineCapacity = v.EngineCapacity,
                            fuelType = v.FuelType
                        }),
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
                        }),
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
                        }),
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
                        })
                    });

                return order;
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "orders", "GetOrderDetail", ex);
                return new orderOb();
            }
        }

        public string AddOrder(addOrderOb order)
        {
            if (order.vehicleID == null || order.vehicleID == -1)
                return "error";
            if (order.clientId == null || order.clientId == -1)
                return "error";
            if (order.departmentId == null || order.departmentId == -1)
                return "error";

            if (string.IsNullOrEmpty(order.clientDiagnose))
                return "no_client_diagnose";

            try
            {
                Order newOrder = new Order
                {
                    ClientId = order.clientId,
                    VehicleId = order.vehicleID,
                    DepartmentId = order.departmentId,
                    ClientDiagnose = order.clientDiagnose,
                    Status = 0,
                    StartDate = order.startDate
                };

                _context.Orders.Add(newOrder);
                _context.SaveChanges();

                foreach (var item in order.images)
                {
                    _context.OrdersImages.Add(new OrdersImage
                    {
                        Image = item.image
                    });
                }

                _context.SaveChanges();

                return "order_added";
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "orders", "AddOrder", ex);
                return "error";
            }
        }

        public string ChangeOrderStatus(int _orderID, int _status)
        {
            if (_orderID == null || _orderID == -1)
                return "error";
            if (_status == null)
                return "error";

            try
            {
                var order = _context.Orders.FirstOrDefault(o => o.Id == _orderID);

                if (order == null)
                    return "error";

                order.Status = (short)_status;

                if (_status == 4)
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == order.ClientId);

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
                if (_status == 5)
                {
                    DateTime endDate = DateTime.Now;

                    order.EndDate = endDate;
                }

                _context.SaveChanges();

                return "status_changed";
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "orders", "ChangeOrderStatus", ex);
                return "error";
            }
        }

        public string AddEstimate(addEditEstimateOb estimate)
        {
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
                var newEstimate = new Estimate
                {
                    OrderId = estimate.orderID,
                    TotalPartsPrice = estimate.totalPartsPrice,
                    TotalServicesPrice = estimate.totalServicesPrice,
                    TotalPrice = estimate.totalPrice
                };

                _context.Estimates.Add(newEstimate);
                _context.SaveChanges();

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

                _context.SaveChanges();

                return "estimate_added";
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "orders", "AddOrder", ex);
                return "error";
            }
        }

        public string EditEstimate(addEditEstimateOb estimate)
        {
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
                var estimateOb = _context.Estimates.FirstOrDefault(e => e.Id == estimate.id);

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

                _context.SaveChanges();

                return "estimate_edited";
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "orders", "EditEstimate", ex);
                return "error";
            }
        }

        public string AddChecklist(addEditChecklistOb checklist)
        {
            if (checklist.orderID == null)
                return "error";

            try
            {
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

                _context.SaveChanges();

                return "checklist_added";
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "orders", "AddChecklist", ex);
                return "error";
            }
        }

        public string EditChecklist(addEditChecklistOb checklist)
        {
            if (checklist.id == null)
                return "error";
            if (checklist.orderID == null)
                return "error";

            try
            {
                var checklistDb = _context.CheckLists.FirstOrDefault(c => c.Id == checklist.id);

                if (checklistDb == null)
                    return "error";

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

                _context.SaveChanges();

                return "checklist_edited";
            }
            catch (MySqlException ex)
            {
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
        public List<estimatePartOb>? estimateParts { get; set; }
        public List<estimateServiceOb>? estimateServices { get; set; }
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

    public class estimateServiceOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public float? rwhAmount { get; set; }
        public decimal? grossUnitPrice { get; set; }
        public decimal? totalPrice { get; set;}
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
