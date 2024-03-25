using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MechAppBackend.Models;
using MechAppBackend.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        MechAppContext _context;

        public LoginController (MechAppContext context)
        {
            _context = context;
        }
        /*
         * Api method for login
         * Check if user provided login data and then compare if it matched data in database
         * 
         * inputs:
         * "email" => user email
         * "password" => user provided password
         * 
         * outputs: 
         * no_login_data => user dont provide email or password
         * bad_login => user not found or bad password
         * error => method break on mysql operations
         * loggedIn => user is logged in also in this response is given with few attributes:
                isFirstLogin => does user have to change his password
                isDeleted => is user deleted
                appRole => is User Client or Employee

         * if user is authenticated backend create a session cookie that is send within the response
         */
        [HttpPost] 
        public IActionResult Login([FromBody] loginOb login)
        {
            StringBuilder resultBuilder = new StringBuilder();
            //check if user provided an input
            if (string.IsNullOrEmpty(login.email)) return new JsonResult(new { result = "no_login_data" });
            if (string.IsNullOrEmpty(login.password)) return new JsonResult(new { result = "no_login_data" });

            try
            {   //get user from database
                var user = _context.Users.FirstOrDefault(u => u.Email == login.email);

                if (user == null) return new JsonResult(new { result = "bad_login" });
                //create combined password
                string _combinedPassword = login.password + user.Salt;

                var loggedIn = false;

                if (hashes.GenerateSHA512Hash(_combinedPassword) != user.Password) return new JsonResult(new { result = "bad_login" });
                //declare variables
                bool _isFirstLogin = (user.IsFirstLogin == 1) ? true : false;

                bool _isDeleted = (user.IsDeleted == 1) ? true : false;

                string _appRole = user.AppRole;

                var token = generators.generatePassword(10);
                //check if session token exist
                var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.UserId == user.Id);

                DateTime expireDate = DateTime.Now.AddHours(2);
                //if session token exist update variable in database
                if (sessionToken != null)
                {
                    sessionToken.Token = token;
                    sessionToken.Expire = expireDate;
                }
                else //if not exist create new record in database
                {
                    UsersToken newToken = new UsersToken
                    {
                        UserId = user.Id,
                        Token = token,
                        Expire = expireDate
                    };

                    _context.UsersTokens.Add(newToken);
                }
                //prepare cookie options
                CookieOptions cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = expireDate,
                    Secure = true,
                    SameSite = SameSiteMode.None
                };

                Response.Cookies.Append("sessionToken", token, cookieOptions);
                //return successfull user login message
                return new JsonResult(new
                {
                    result = "loggedIn",
                    isFirstLogin = _isFirstLogin,
                    isDeleted = _isDeleted,
                    appRole = _appRole
                });
            }
            catch (MySqlException ex) //on any error with mysql commands
            {
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "LoginController", "Login", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() }); //return error response
        }
    }

    public class loginOb //user login object
    {
        public string? email { get; set; }
        public string? password { get; set; }
    }
}
