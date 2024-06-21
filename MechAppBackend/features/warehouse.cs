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

        /// <summary>
        /// Pobiera listę przedmiotów z magazynu na podstawie podanych kryteriów.
        /// </summary>
        /// <param name="name">Nazwa przedmiotu do wyszukania.</param>
        /// <param name="depids">Lista identyfikatorów działów, z których mają być pobrane przedmioty.</param>
        /// <param name="offset">Przesunięcie (dla paginacji).</param>
        /// <param name="pageSize">Rozmiar strony (dla paginacji).</param>
        /// <returns>Listę przedmiotów spełniających kryteria wyszukiwania.</returns>
        public List<warehouseItemOb> GetWarehouseItems(string name, List<long> depids, int offset, int pageSize)
        {
            List<warehouseItemOb> warehouseItems = new List<warehouseItemOb>();

            try
            {
                IQueryable<Warehouse> query = _context.Warehouses;

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(w => w.Name.ToLower().Contains(name.ToLower()));

                if (depids.Count > 0)
                    query = query.Where(w => depids.Contains((long)w.DepartmentId));

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

        /// <summary>
        /// Pobiera szczegóły przedmiotu z magazynu na podstawie identyfikatora lub kodu EAN.
        /// </summary>
        /// <param name="id">Identyfikator przedmiotu.</param>
        /// <param name="ean">Kod EAN przedmiotu.</param>
        /// <returns>Szczegóły przedmiotu z magazynu.</returns>
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

        /// <summary>
        /// Dodaje nowy przedmiot do magazynu.
        /// </summary>
        /// <param name="item">Obiekt zawierający szczegóły przedmiotu do dodania.</param>
        /// <returns>
        /// Zwraca ciąg znaków informujący o wyniku operacji:
        /// - "no_name" jeśli brakuje nazwy przedmiotu,
        /// - "no_ean" jeśli brakuje kodu EAN przedmiotu,
        /// - "no_amount" jeśli brakuje ilości przedmiotu lub jest ona równa -1,
        /// - "no_unit_price" jeśli brakuje ceny jednostkowej przedmiotu lub jest ona równa -1,
        /// - "exist" jeśli przedmiot o podanej nazwie i kodzie EAN już istnieje,
        /// - "item_added" jeśli przedmiot został pomyślnie dodany,
        /// - "error" w przypadku wystąpienia błędu podczas dodawania przedmiotu.
        /// </returns>
        /// <remarks>
        /// Metoda sprawdza, czy podane informacje o przedmiocie są kompletne i czy przedmiot o takiej samej nazwie
        /// i kodzie EAN już istnieje w magazynie. Jeśli wszystkie warunki są spełnione, przedmiot jest dodawany do bazy danych.
        /// W przypadku wystąpienia błędu, metoda zwraca informację o błędzie.
        /// </remarks>
        public string AddWarehouseItem(AddEditWarehouseItemDetailsOb item)
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
                var checkItem = _context.Warehouses.FirstOrDefault(w => w.Name.ToLower() == item.name.Trim().ToLower() && w.Ean == item.ean.Trim() && w.DepartmentId == item.departmentId);

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

        /// <summary>
        /// Dodaje listę nowych przedmiotów do magazynu.
        /// </summary>
        /// <param name="items">Lista obiektów zawierających szczegóły przedmiotów do dodania.</param>
        /// <returns>
        /// Zwraca ciąg znaków informujący o wyniku operacji:
        /// - "error" jeśli lista przedmiotów jest pusta,
        /// - "no_name" jeśli brakuje nazwy któregokolwiek z przedmiotów,
        /// - "no_ean" jeśli brakuje kodu EAN któregokolwiek z przedmiotów,
        /// - "no_amount" jeśli brakuje ilości któregokolwiek z przedmiotów lub jest ona równa -1,
        /// - "no_unit_price" jeśli brakuje ceny jednostkowej któregokolwiek z przedmiotów lub jest ona równa -1,
        /// - "items_added" jeśli wszystkie przedmioty zostały pomyślnie dodane,
        /// - "error" w przypadku wystąpienia błędu podczas dodawania przedmiotów.
        /// </returns>
        /// <remarks>
        /// Metoda sprawdza, czy podane informacje o wszystkich przedmiotach są kompletne. Następnie dodaje każdy z przedmiotów
        /// do bazy danych. W przypadku wystąpienia błędu, metoda zwraca informację o błędzie.
        /// </remarks>
        public string AddWarehouseItems(List<warehouseItemDetailsOb> items, int departmentID)
        {
            if (items.Count == 0)
                return "error";

            if (items.Any(i => string.IsNullOrEmpty(i.name)))
                return "no_name";
            if (items.Any(i => string.IsNullOrEmpty(i.ean)))
                return "no_ean";
            if (items.Any(i => i.amount == -1))
                return "no_amount";
            if (items.Any(i => i.unitPrice == -1))
                return "no_unit_price";

            try
            {
                for (var i = 0; i < items.Count; i++)
                {
                    if (_context.Warehouses.Any(w => w.Ean == items[i].ean.Trim()))
                    {
                        var itemdb = _context.Warehouses.FirstOrDefault(w => w.Ean == items[i].ean.Trim() && w.DepartmentId == departmentID);

                        if (itemdb == null)
                            continue;

                        itemdb.Name = items[i].name;
                        itemdb.Ean = items[i].ean.Trim();
                        itemdb.Amount = items[i].amount;
                        itemdb.UnitPrice = items[i].unitPrice;
                        itemdb.Stand = items[i].stand;
                        itemdb.PlaceNumber = items[i].standPlace;
                    }
                    else
                    {
                        _context.Warehouses.Add(new Warehouse
                        {
                            Name = items[i].name,
                            Ean = items[i].ean,
                            Amount = items[i].amount,
                            UnitPrice = items[i].unitPrice,
                            Stand = items[i].stand,
                            PlaceNumber = items[i].standPlace
                        });
                    }
                }

                _context.SaveChanges();

                return "items_added";
            }
            catch (MySqlException ex)
            {
                Logger.SendException("MechApp", "warehouse", "AddWarehouseItems", ex);
                return "error";
            }
        }

        /// <summary>
        /// Edytuje szczegóły istniejącego przedmiotu w magazynie.
        /// </summary>
        /// <param name="item">Obiekt zawierający zaktualizowane szczegóły przedmiotu, w tym jego identyfikator.</param>
        /// <returns>
        /// Zwraca ciąg znaków informujący o wyniku operacji:
        /// - "no_id" jeśli brakuje identyfikatora przedmiotu lub jest on równy -1,
        /// - "no_name" jeśli brakuje nazwy przedmiotu,
        /// - "no_amount" jeśli brakuje ilości przedmiotu lub jest ona równa -1,
        /// - "no_unit_price" jeśli brakuje ceny jednostkowej przedmiotu lub jest ona równa -1,
        /// - "item_edited" jeśli przedmiot został pomyślnie zaktualizowany,
        /// - "error" w przypadku wystąpienia błędu podczas aktualizacji przedmiotu.
        /// </returns>
        /// <remarks>
        /// Metoda sprawdza, czy podane informacje o przedmiocie są kompletne i czy przedmiot o podanym identyfikatorze istnieje w magazynie.
        /// Jeśli wszystkie warunki są spełnione, szczegóły przedmiotu są aktualizowane w bazie danych.
        /// W przypadku wystąpienia błędu, metoda zwraca informację o błędzie.
        /// </remarks>
        public string EditWarehouseItem(AddEditWarehouseItemDetailsOb item)
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

        /// <summary>
        /// Usuwa przedmiot z magazynu na podstawie jego identyfikatora.
        /// </summary>
        /// <param name="id">Identyfikator przedmiotu do usunięcia.</param>
        /// <returns>
        /// Zwraca ciąg znaków informujący o wyniku operacji:
        /// - "error" jeśli identyfikator jest nieprawidłowy lub równy -1,
        /// - "item_deleted" jeśli przedmiot został pomyślnie usunięty,
        /// - "error" w przypadku wystąpienia błędu podczas usuwania przedmiotu.
        /// </returns>
        /// <remarks>
        /// Metoda sprawdza, czy podany identyfikator przedmiotu jest prawidłowy, a następnie usuwa przedmiot z bazy danych.
        /// W przypadku wystąpienia błędu, metoda zwraca informację o błędzie.
        /// </remarks>

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

        /// <summary>
        /// Usuwa z magazynu przedmioty na podstawie listy ich identyfikatorów.
        /// </summary>
        /// <param name="ids">Lista identyfikatorów przedmiotów do usunięcia.</param>
        /// <returns>
        /// Zwraca ciąg znaków informujący o wyniku operacji:
        /// - "error" jeśli lista identyfikatorów jest pusta,
        /// - "items_deleted" jeśli przedmioty zostały pomyślnie usunięte,
        /// - "error" w przypadku wystąpienia błędu podczas usuwania przedmiotów.
        /// </returns>
        /// <remarks>
        /// Metoda sprawdza, czy lista identyfikatorów nie jest pusta, a następnie usuwa przedmioty o podanych identyfikatorach z bazy danych.
        /// W przypadku wystąpienia błędu, metoda zwraca informację o błędzie.
        /// </remarks>
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

    public class AddEditWarehouseItemDetailsOb
    {
        public int? id { get; set; } = -1;
        public string? name { get; set; }
        public string? ean { get; set; }
        public int? amount { get; set; } = -1;
        public decimal? unitPrice { get; set; } = -1;
        public string? stand { get; set; }
        public string? standPlace { get; set; }
        public int? departmentId { get; set; }
    }

    public class SearchWarehouseItemDetailsOb
    {
        public int? id { get; set; } = -1;
        public string? name { get; set; }
        public string? ean { get; set; }
        public int? amount { get; set; } = -1;
        public decimal? unitPrice { get; set; } = -1;
        public string? stand { get; set; }
        public string? standPlace { get; set; }
        public int? departmentId { get; set; }
        public string? submitFrom { get; set; }
    }
}
