using MechAppBackend.AppSettings;
using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using MechAppBackend.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//to do usuniecia kiedys
namespace MechAppBackend.Controllers
{
    [Route("api/addnewuser")]
    [ApiController]
    public class PrepareNewUser : ControllerBase
    {
        MechAppContext _context;

        public PrepareNewUser(MechAppContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] newUser userOb)
        {
            string _salt = generators.generatePassword(10);

            string _combinedPassword = userOb.password + _salt;

            string _hashedPassword = hashes.GenerateSHA512Hash(_combinedPassword);

            User newUser = new User
            {
                Email = userOb.email,
                Password = _hashedPassword,
                Name = userOb.name,
                Lastname = userOb.lastname,
                Salt = _salt,
                AppRole = "Employee",
                IsDeleted = 0,
                IsLoyalCustomer = 1,
                IsFirstLogin = 0
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return new JsonResult(new { result = "done" });
        }
    }

    public class newUser
    {
        public string? email { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? password { get; set; }
    }
}
