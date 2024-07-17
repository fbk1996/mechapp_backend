using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MySql.Data.MySqlClient;

namespace MechAppBackend.features
{
    public class departments
    {
        private MechAppContext _context;

        public departments(MechAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of all departments.
        /// </summary>
        /// <returns>A list of department objects, each containing the ID, name, description, address, postcode, and city of a department.</returns>
        public List<departmentOb> GetDepartments()
        {
            try
            {
                // Query the database for all departments
                var departmentsList = _context.Departments
                    .Select(d => new departmentOb
                    {
                        id = Convert.ToInt32(d.Id),
                        name = d.Name,
                        description = d.Description,
                        address = d.Address,
                        postcode = d.Postcode,
                        city = d.City,
                        count = _context.Departments.Count()
                    }).ToList();
                // Return the list of departments
                return departmentsList;
            }
            catch (MySqlException ex)
            {
                // If an error occurs, log it and return an empty list
                Logger.SendException("MechApp", "departments", "GetDepartments", ex);
                return new List<departmentOb>();
            }
        }
    }

    public class departmentOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public string? address { get; set; }
        public string? postcode { get; set; }
        public string? city { get; set; }
        public int? count { get; set; }
    }
}
