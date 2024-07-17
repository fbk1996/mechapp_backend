using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace MechAppBackend.features
{
    public class demands
    {
        MechAppContext _context;
        warehouse warehouseController;
        
        public demands(MechAppContext context)
        {
            _context = context;
            warehouseController = new warehouse(context);
        }

        /// <summary>
        /// Retrieves a list of demands based on the specified filter criteria and pagination parameters.
        /// </summary>
        /// <param name="dateFrom">The start date for filtering demands. If null, no filtering is applied for the start date.</param>
        /// <param name="dateTo">The end date for filtering demands. If null, no filtering is applied for the end date.</param>
        /// <param name="statuses">A list of statuses to filter demands. If null or empty, no filtering is applied for statuses.</param>
        /// <param name="departmentIds">A list of department IDs to filter demands. If null or empty, no filtering is applied for departments.</param>
        /// <param name="pageSize">The number of demands to retrieve.</param>
        /// <param name="offset">The number of demands to skip for pagination.</param>
        /// <returns>A list of demandOb objects that match the specified filter criteria.</returns>
        public List<demandOb> GetDemands(DateTime? dateFrom, DateTime? dateTo, List<int>? statuses, List<int>? departmentIds, int pageSize, int offset)
        {
            // Start with the base query for demands
            IQueryable<Demand> query = _context.Demands;
            // Apply date range filtering
            if (dateFrom != null && dateTo == null)
                query = query.Where(d => d.Date >= dateFrom); // Filter demands from dateFrom onwards
            else if (dateFrom == null && dateTo != null)
                query = query.Where(d => d.Date <= dateTo); // Filter demands up to dateTo
            else if (dateFrom != null && dateTo != null)
                query = query.Where(d => d.Date >= dateFrom && d.Date <= dateTo); // Filter demands within the date range from dateFrom to dateTo

            if (statuses.Count != 0)
                query = query.Where(d => statuses.Contains((int)d.Status)); // Apply status filtering if statuses list is not empty
            if (departmentIds.Count != 0)
                query = query.Where(d => departmentIds.Contains((int)d.DepartmentId));  // Apply department filtering if departmentIds list is not empty
            // Filter out deleted demands
            query = query.Where(d => d.IsDeleted == 0);

            try
            {
                // Execute the query with pagination and project results into demandOb objects
                var demands = query
                    .AsNoTracking() // Optimize for read-only queries
                    .Skip(offset)
                    .Take(pageSize)
                    .Select(d => new demandOb
                    {
                        id = (int)d.Id,
                        user = new demandUserOb
                        {
                            id = (int)d.User.Id,
                            name = d.User.Name,
                            lastname = d.User.Lastname
                        },
                        department = new demandDepartmentOb
                        {
                            id = (int)d.Department.Id,
                            name = d.Department.Name
                        },
                        date = d.Date,
                        status = d.Status,
                        count = query.Count()
                    }).ToList();

                return demands;
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an empty list if an error occurs
                Logger.SendException("MechApp", "demands", "GetDemands", ex);
                return new List<demandOb>();
            }
        }

        /// <summary>
        /// Retrieves the details of a specific demand by its ID.
        /// </summary>
        /// <param name="id">The ID of the demand to retrieve. If the default value (-1) is provided, a demandDetailOb with id -1 is returned.</param>
        /// <returns>A demandDetailOb object containing the details of the demand, or a demandDetailOb with id -1 if the demand is not found or an error occurs.</returns>
        public demandDetailOb GetDemandDetails(int id = -1)
        {
            // If the id is the default value (-1), return a demandDetailOb with id -1
            if (id == -1)
                return new demandDetailOb { id = -1 };

            try
            {
                // Query the database to find the demand with the specified id
                var demand = (demandDetailOb)_context.Demands
                    .Where(d => d.Id == id)
                    .AsNoTracking()
                    .Select(d => new demandDetailOb
                    {
                        id = (int)d.Id,
                        user = new demandUserOb
                        {
                            id = (int)d.User.Id,
                            name = d.User.Name,
                            lastname = d.User.Lastname
                        },
                        department = new demandDepartmentOb
                        {
                            id = (int)d.Department.Id,
                            name = d.Department.Name
                        },
                        date = d.Date,
                        status = d.Status,
                        items = d.DemandsItems
                            .Where(di => di.DemandId == d.Id)
                            .Select(di => new demandItemOb
                            {
                                id = (int)di.Id,
                                name = di.Name,
                                ean = di.Ean,
                                grossUnitPrice = di.GrossUnitPrice,
                                amount = di.Amount,
                                status = di.Status
                            }).ToList()
                    }).FirstOrDefault();
                // If no demand is found, return a demandDetailOb with id -1
                return demand ?? new demandDetailOb { id = -1 };
            }
            catch (MySqlException ex)
            {
                // Log the exception and return a demandDetailOb with id -1 if an error occurs
                Logger.SendException("MechApp", "demands", "GetDemands", ex);
                return new demandDetailOb { id = -1 };
            }
        }

        /// <summary>
        /// Searches for items in the warehouse based on the provided EAN and department ID.
        /// </summary>
        /// <param name="ean">The EAN code to search for. If null, no EAN filtering is applied.</param>
        /// <param name="departmentId">The ID of the department to filter items. If the default value (-1) is provided, an empty list is returned.</param>
        /// <returns>A list of demandItemOb objects that match the search criteria, or an empty list if no items are found or an error occurs.</returns>
        public List<demandItemOb> SearchItemsInWarehouse(string? ean, int departmentId = -1)
        {
            // If the departmentId is the default value (-1), return an empty list
            if (departmentId == -1)
                return new List<demandItemOb>();

            try
            {
                // Start with the base query for warehouses
                IQueryable<Warehouse> query = _context.Warehouses;
                // Apply EAN filtering if an EAN is provided
                if (ean != null)
                    query = query.Where(w => w.Ean.ToLower().Contains(ean.ToLower().Trim()));
                // Apply department ID filtering
                query = query.Where(w => w.DepartmentId == departmentId);
                // Project the query results into a list of demandItemOb objects
                var items = query
                    .Select(w => new demandItemOb
                    {
                        name = w.Name,
                        ean = w.Ean,
                        grossUnitPrice = w.UnitPrice
                    }).ToList();

                return items;
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an empty list if an error occurs
                Logger.SendException("MechApp", "demands", "SearchItemsInWarehouse", ex);
                return new List<demandItemOb>();
            }
        }

        /// <summary>
        /// Adds a new demand based on the provided demand object.
        /// </summary>
        /// <param name="demand">An AddEditDemandOb object containing the details of the demand to be added.</param>
        /// <returns>A string indicating the result of the operation:
        /// "no_user" if the userId is invalid,
        /// "no_department" if the departmentId is invalid,
        /// "no_date" if the date is null,
        /// "no_status" if the status is invalid,
        /// "no_items" if there are no items in the demand,
        /// "demand_added" if the demand was successfully added,
        /// "error" if an exception occurred.</returns>
        public string AddDemand(AddEditDemandOb demand)
        {
            // Validate input parameters
            if (demand.userId == -1)
                return "no_user";
            if (demand.departmentId == -1)
                return "no_department";
            if (demand.date == null)
                return "no_date";
            if (demand.status == -1)
                return "no_status";
            if (demand.items.Count == 0)
                return "no_items";

            try
            {
                // Create and add a new demand entity to the context
                var newDemand = _context.Demands.Add(new Demand
                {
                    UserId = demand.userId,
                    DepartmentId = demand.departmentId,
                    Date = demand.date,
                    Status = (short)demand.status,
                    IsDeleted = 0
                });
                // Save changes to the context to generate the demand ID
                _context.SaveChanges();
                // Add each item in the demand to the context
                foreach (var item in demand.items)
                {
                    _context.DemandsItems.Add(new DemandsItem
                    {
                        DemandId = newDemand.Entity.Id, // Use the generated demand ID
                        Name = item.name,
                        Ean = item.ean,
                        GrossUnitPrice = item.grossUnitPrice,
                        Amount = item.amount,
                        Status = (short)item.status
                    });
                }
                // Save changes to the context to persist the items
                _context.SaveChanges();

                return "demand_added";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error string if an exception occurs
                Logger.SendException("MechApp", "demands", "AddDemand", ex);
                return "error";
            }
        }

        /// <summary>
        /// Changes the status of a specified demand and updates the warehouse inventory if the status is "Completed".
        /// </summary>
        /// <param name="status">An editDemandStatus object containing the ID of the demand and the new status to be set.</param>
        /// <returns>A string indicating the result of the operation:
        /// "status_changed" if the status was successfully changed,
        /// "error" if the demand ID or status is invalid, the demand is not found, or an exception occurs.</returns>
        public string ChangeDemandStatus(editDemandStatus status)
        {
            // Validate input parameters
            if (status.id == -1)
                return "error";
            if (status.status == -1)
                return "error";

            try
            {
                // Retrieve the demand entity based on the provided ID
                var demand = _context.Demands.FirstOrDefault(d => d.Id == status.id);
                // If the demand is not found, return an error
                if (demand == null)
                    return "error";
                // Update the status of the demand
                demand.Status = (short)status.status;
                // Save changes to the context
                _context.SaveChanges();
                // If the new status is "Zrealizowane" (status 4), update the warehouse inventory
                if (status.status == 4)
                {
                    // Retrieve the items associated with the demand
                    var demandItems = _context.DemandsItems
                        .Where(di => di.DemandId == status.id).ToList();
                    // Iterate over each demand item
                    foreach (var demandItem in demandItems)
                    {
                        // Find the corresponding warehouse item
                        var warehouseItem = _context.Warehouses.FirstOrDefault(w => w.Ean == demandItem.Ean && w.DepartmentId == demand.DepartmentId);
                        // Only update warehouse if the demand item status is "Approved" (status 1)
                        if (demandItem.Status != 1) continue;

                        if (warehouseItem == null) // Add new item to warehouse if it doesn't exist
                        {
                            _context.Warehouses.Add(new Warehouse
                            {
                                DepartmentId = demand.DepartmentId,
                                Name = demandItem.Name,
                                Ean = demandItem.Ean,
                                Amount = demandItem.Amount,
                                UnitPrice = demandItem.GrossUnitPrice
                            });
                        }
                        else  // Update the amount of the existing warehouse item
                            warehouseItem.Amount += demandItem.Amount;
                    }

                    _context.SaveChanges();
                }
                // Save changes to the context to persist warehouse updates
                return "status_changed";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error string if an exception occurs
                Logger.SendException("MechApp", "demands", "ChangeDemandStatus", ex);
                return "error";
            }
        }

        /// <summary>
        /// Edits an existing demand based on the provided demand object.
        /// </summary>
        /// <param name="demand">An AddEditDemandOb object containing the details of the demand to be edited.</param>
        /// <returns>A string indicating the result of the operation:
        /// "demand_edited" if the demand was successfully edited,
        /// "error" if the demand ID is invalid, an error occurs, or if required fields are missing.</returns>
        public string EditDemand(AddEditDemandOb demand)
        {
            // Validate input parameters
            if (demand.id == -1)
                return "error";
            if (demand.items.Count == 0)
                return "no_items";

            try
            {
                // Get the current items associated with the demand from the database
                var currentItemsInDemands = _context.DemandsItems
                    .Where(di => di.DemandId == demand.id)
                    .Select(di => (int?)di.Id)
                    .ToList();
                // Get the list of item IDs from the provided demand object
                var itemIdsAsNullable = demand.items.Select(id => (int?)id.id).ToList();
                // Determine which items need to be added
                var itemsToAdd = itemIdsAsNullable.Except(currentItemsInDemands).ToList();
                // Add new items to the demand
                foreach (var item in itemsToAdd)
                {
                    var reqItem = demand.items.FirstOrDefault(i => i.id == item);

                    if (reqItem == null) continue;

                    _context.DemandsItems.Add(new DemandsItem
                    {
                        DemandId = demand.id,
                        Name = reqItem.name,
                        Ean = reqItem.ean,
                        Amount = reqItem.amount,
                        GrossUnitPrice = reqItem.grossUnitPrice,
                        Status = (short)reqItem.status
                    });
                }
                // Determine which items need to be removed
                var itemsToRemove = currentItemsInDemands.Where(di => !itemIdsAsNullable.Contains(di)).ToList();
                // Remove items from the demand
                foreach (var item in itemsToRemove)
                {
                    _context.DemandsItems.Remove(_context.DemandsItems.FirstOrDefault(di => di.Id == item));
                }
                // Determine which items need to be edited
                var itemsToEdit = itemIdsAsNullable.Except(itemsToAdd).ToList();
                // Edit existing items in the demand
                foreach (var item in itemsToEdit)
                {
                    var reqItem = demand.items.FirstOrDefault(i => i.id == item);

                    if (reqItem == null) continue;

                    var dbItem = _context.DemandsItems.FirstOrDefault(di => di.Id == item);

                    if (dbItem == null) continue;

                    dbItem.Name = reqItem.name;
                    dbItem.Ean = reqItem.ean;
                    dbItem.Amount = reqItem.amount;
                    dbItem.GrossUnitPrice = reqItem.grossUnitPrice;
                    dbItem.Status = (short)reqItem.status;
                }
                // Save changes to the context to persist the modifications
                _context.SaveChanges();

                return "demand_edited";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error string if an exception occurs
                Logger.SendException("MechApp", "demands", "EditDemand", ex);
                return "error";
            }
        }

        /// <summary>
        /// Marks a demand as deleted by setting its IsDeleted property to 1.
        /// </summary>
        /// <param name="id">The ID of the demand to be marked as deleted.</param>
        /// <returns>A string indicating the result of the operation:
        /// "demand_deleted" if the demand was successfully marked as deleted,
        /// "error" if the demand ID is invalid, the demand is not found, or an exception occurs.</returns>
        public string DeleteDemand(int id = -1)
        {
            // Validate input parameter
            if (id == -1)
                return "error";

            try
            {
                // Retrieve the demand entity based on the provided ID
                var demand = _context.Demands.FirstOrDefault(d => d.Id == id);
                // If the demand is not found, return an error
                if (demand == null)
                    return "error";
                // Mark the demand as deleted by setting the IsDeleted property to 1
                demand.IsDeleted = 1;
                // Save changes to the context to persist the modification
                _context.SaveChanges();

                return "demand_deleted";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error string if an exception occurs
                Logger.SendException("MechApp", "demands", "DeleteDemand", ex);
                return "error";
            }
        }

        /// <summary>
        /// Marks multiple demands as deleted by setting their IsDeleted property to 1.
        /// </summary>
        /// <param name="ids">A list of IDs of the demands to be marked as deleted.</param>
        /// <returns>A string indicating the result of the operation:
        /// "demands_deleted" if the demands were successfully marked as deleted,
        /// "error" if the list of IDs is empty, no demands are found, or an exception occurs.</returns>
        public string DeleteDemands(List<int> ids)
        {
            // Validate input parameter
            if (ids.Count == 0)
                return "error";

            try
            {
                // Retrieve the demands based on the provided list of IDs
                var demands = _context.Demands
                    .Where(d => ids.Contains((int)d.Id)).ToList();
                // If no demands are found, return an error
                if (demands.Count == 0)
                    return "error";
                // Mark each demand as deleted by setting the IsDeleted property to 1
                foreach (var demand in demands)
                {
                    demand.IsDeleted = 1;
                }
                // Save changes to the context to persist the modifications
                _context.SaveChanges();

                return "demands_deleted";
            }
            catch (MySqlException ex)
            {
                // Log the exception and return an error string if an exception occurs
                Logger.SendException("MechApp", "demands", "DeleteDemands", ex);
                return "error";
            }
        }
    }

    public class demandOb
    {
        public int? id { get; set; }
        public demandUserOb? user { get; set; }
        public demandDepartmentOb? department { get; set; }
        public DateTime? date { get; set; }
        public int? status { get; set; }
        public int? count { get; set; }
    }

    public class demandDetailOb
    {
        public int? id { get; set; }
        public demandUserOb? user { get; set; }
        public demandDepartmentOb? department { get; set; }
        public DateTime? date { get; set; }
        public int? status { get; set; }
        public List<demandItemOb>? items { get; set; }
    }

    public class AddEditDemandOb
    {
        public int? id { get; set; } = -1;
        public int? userId { get; set; } = -1;
        public int? departmentId { get; set; } = -1;
        public DateTime? date { get; set; }
        public int? status { get; set; } = -1;
        public List<demandItemOb>? items { get; set; }
    }

    public class editDemandStatus
    {
        public int? id { get; set; } = -1;
        public int? status { get; set; } = -1;
    }

    public class demandItemOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? ean { get; set; }
        public decimal? grossUnitPrice { get; set; }
        public int? amount { get; set; }
        public int? status { get; set; }
    }

    public class demandUserOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
    }

    public class demandDepartmentOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
    }
}
