using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using MySql.Data.MySqlClient;

namespace MechAppBackend.features
{
    public class warehouse
    {
        MechAppContext _context;

        public warehouse(MechAppContext context)
        {
            _context = context;
        }

        public List<warehouseItemOb> GetWarehouseItems(string name, List<long?> depids, int offset, int pageSize)
        {
            List<warehouseItemOb> warehouseItems = new List<warehouseItemOb>();

            try
            {
                IQueryable<Warehouse> query = _context.Warehouses;

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(w => w.Name.ToLower().Contains(name.ToLower()));

                if (depids.Count > 0)
                    query = query.Where(w => depids.Contains(w.DepartmentId));

                warehouseItems = query
                    .Skip(offset)
                    .Take(pageSize)
                    .Select(w => new warehouseItemOb
                    {
                        id = (int)w.Id,
                        name = w.Name,
                        ean = w.Ean,
                        amount = w.Amount,
                        unitPrice = w.UnitPrice
                    }).ToList();

                return warehouseItems;
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "warehouse", "GetWareouseItems", ex);
                warehouseItems.Clear();
                return warehouseItems;
            }
        }

        public warehouseItemDetailsOb GetWarehouseItemDetails(int? id, string? ean)
        {
            try
            {
                IQueryable<Warehouse> query = _context.Warehouses;
                
                if (id != null || id != -1) 
                    query = query.Where(w => w.Id ==  id);

                if (!string.IsNullOrEmpty(ean)) 
                    query = query.Where(w => w.Ean == ean);

                warehouseItemDetailsOb item = (warehouseItemDetailsOb)query.Select(w => new warehouseItemDetailsOb
                {
                    id = (int)w.Id,
                    name = w.Name,
                    ean = w.Ean,
                    amount = w.Amount,
                    unitPrice = w.UnitPrice,
                    stand = w.Stand,
                    standPlace = w.PlaceNumber
                }).FirstOrDefault();

                return item;
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "warehouse", "GetWarehouseItemDetails", ex);
                return new warehouseItemDetailsOb { id = -1 };
            }
        }

        public string AddWarehouseItem(warehouseItemDetailsOb item)
        {
            if (string.IsNullOrEmpty(item.name))
                return "no_name";
            if (string.IsNullOrEmpty(item.ean))
                return "no_ean";
            if (item.amount == null || item.amount == -1)
                return "no_amount";
            if (item.unitPrice == null || item.unitPrice == -1)
                return "no_unit_price";

            try
            {
                var checkItem = _context.Warehouses.FirstOrDefault(w => w.Name.ToLower() == item.name.Trim().ToLower() && w.Ean == item.ean.Trim());

                if (checkItem != null)
                    return "exist";

                _context.Warehouses.Add(new Warehouse
                {
                    Name = item.name.Trim(),
                    Ean = item.ean.Trim(),
                    Amount = item.amount,
                    UnitPrice = item.unitPrice,
                    Stand = item.stand,
                    PlaceNumber = item.standPlace
                });

                _context.SaveChanges();

                return "item_added";
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "warehouse", "AddWarehouseItem", ex);
                return "error";
            }
        }

        public string EditWarehouseItem(warehouseItemDetailsOb item)
        {
            if (item.id == null || item.id == -1)
                return "no_id";
            if (string.IsNullOrEmpty(item.name))
                return "no_name";
            if (item.amount == null || item.amount == -1)
                return "no_amount";
            if (item.unitPrice == null || item.unitPrice == -1)
                return "no_unit_price";

            try
            {
                var itemDb = _context.Warehouses.FirstOrDefault(w => w.Id == item.id);

                if (itemDb == null)
                    return "error";

                itemDb.Name = item.name;
                itemDb.Amount = item.amount;
                itemDb.UnitPrice = item.unitPrice;
                itemDb.Stand = item.stand;
                itemDb.PlaceNumber = item.standPlace;

                _context.SaveChanges();

                return "item_edited";
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "warehouse", "EditWarehouseItem", ex);
                return "error";
            }
        }

        public string DeleteWarehouseItem(int id = -1)
        {
            if (id == null || id == -1)
                return "error";

            try
            {
                _context.Warehouses.RemoveRange(_context.Warehouses.Where(w => w.Id == id));
                _context.SaveChanges();

                return "item_deleted";
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "warehouse", "DeleteWarehouseItem", ex);
                return "error";
            }
        }

        public string DeleteWarehouseItems(List<int> ids)
        {
            if (ids.Count == 0)
                return "error";

            try
            {
                _context.Warehouses.RemoveRange(_context.Warehouses.Where(w => ids.Contains((int)w.Id)));
                _context.SaveChanges();

                return "items_deleted";
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "warehouse", "DeleteWarehouseItems", ex);
                return "error";
            }
        }
    }

    public class warehouseItemOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? ean { get; set; }
        public int? amount { get; set; }
        public decimal? unitPrice { get; set; }
    }

    public class warehouseItemDetailsOb
    {
        public int? id { get; set; } = -1;
        public string? name { get; set; }
        public string? ean { get; set; }
        public int? amount { get; set; } = -1;
        public decimal? unitPrice { get; set; } = -1;
        public string? stand { get; set; }
        public string? standPlace { get; set; }
    }
}
