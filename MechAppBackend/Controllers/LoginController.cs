using MechAppBackend.AppSettings;
using MechAppBackend.Conns;
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
        CheckCookieToken checkToken;

        public LoginController(MechAppContext context)
        {
            _context = context;
            checkToken = new CheckCookieToken(context);
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

                var subscriptionCheckDate = DateTime.Now;

                if (subscriptionCheckDate < appdata.endDate)
                    return new JsonResult(new { result = "no_subscription_active" });

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

                DateTime expireDate = endCookieDate.GetEndCookieDate();
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

                _context.SaveChanges();
                //prepare cookie options
                CookieOptions cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = expireDate,
                    Path = "/",
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

        /// <summary>
        /// Check User Authentication Token
        /// Validates the user's authentication by checking the session token stored in cookies.
        /// If the token is valid and not expired, the method implicitly indicates successful authentication.
        /// 
        /// Responses:
        /// - "no_auth": Returned if no session token is found in cookies, or if the token is invalid or expired.
        /// - "error": Returned in case of an exception during the operation.
        /// </summary>
        /// <returns>JsonResult indicating the outcome of the authentication check</returns>
        [HttpGet("/api/checkAuth")]
        public IActionResult CheckToken()
        {
            StringBuilder resultBuilder = new StringBuilder();

            try
            {
                // Variable to hold the cookie value
                string _cookieValue = string.Empty;

                // Check if there's a 'sessionToken' cookie in the request
                if (Request.Cookies["sessionToken"] != null)
                {
                    // Retrieve the token value from the cookie
                    _cookieValue = Request.Cookies["sessionToken"];
                }
                else
                {
                    // If no cookie is present, return a 'no_auth' response
                    return new JsonResult(new { result = "no_auth" });
                }

                // Use the checkCookie method to validate the token
                if (!checkToken.checkCookie(_cookieValue))
                {
                    // If token validation fails, return a 'no_auth' response
                    return new JsonResult(new { result = "no_auth" });
                }

                var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.Token == _cookieValue);

                if (sessionToken == null) return new JsonResult(new { result = "no_auth" });

                var user = _context.Users.FirstOrDefault(u => u.Id == sessionToken.UserId);

                if (user == null) return new JsonResult(new { result = "no_auth" });

                DateTime expireCookie = endCookieDate.GetEndCookieDate();

                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = expireCookie
                };

                Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

                return new JsonResult(new
                {
                    result = "authenticated",
                    approle = user.AppRole
                });
                // If token is valid, continue (implicit success case)
            }
            catch (Exception ex)
            {
                // In case of an exception, clear the result builder and append 'error'
                resultBuilder.Clear().Append("error");
                // Log the exception for further analysis
                Logger.SendNormalException("MechApp", "LoginController", "CheckToken", ex);
            }

            // If everything is successful, return an empty result, indicating success
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Send Verification Code for Password Reset
        /// Generates and sends a verification code to the user's email or phone for password reset purposes.
        /// The method first checks if the user with the given email exists, then generates a code, 
        /// and sends it either via email or SMS depending on the user's provided contact details.
        ///
        /// Parameters:
        /// - email: The email address of the user who is attempting to reset their password.
        ///
        /// Responses:
        /// - "user_not_found": Returned if no user is associated with the provided email address.
        /// - "send_by_phone": Indicates the verification code was sent by phone.
        /// - "send_by_email": Indicates the verification code was sent by email.
        /// - "error": Returned in case of a database exception or sending failure.
        /// </summary>
        /// <param name="email">The email address of the user</param>
        /// <returns>JsonResult indicating the outcome of the operation</returns>
        [HttpGet("/api/resetPassword/{email}")]
        public IActionResult SendVeryficationCode(string email)
        {
            StringBuilder resultBuilder = new StringBuilder();

            try
            {

                // Retrieve the user based on the email address
                var user = _context.Users.FirstOrDefault(u => u.Email == email);

                // If no user found, return 'user_not_found'
                if (user == null) return new JsonResult(new { result = "user_not_found" });

                // Generate a verification code
                var code = generators.generateValidationCode(10);

                // Set the expiration date for the code
                DateTime expireDate = DateTime.Now.AddMinutes(30);

                // Create a new validation code object
                ValidationCode newValCode = new ValidationCode
                {
                    UserId = user.Id,
                    Code = code,
                    Expire = expireDate
                };

                // Add the validation code to the context and save changes
                _context.ValidationCodes.Add(newValCode);
                _context.SaveChanges();

                // Check if the user has a phone number and send SMS
                if (!string.IsNullOrEmpty(user.Phone) && !string.IsNullOrEmpty(connections.smsApiToken))
                {
                    string _userPhone = user.Phone.Replace("48", "").Replace("+", "");

                    smssender.SendSms($"Kod weryfikujacy zmiane hasla: {code}", _userPhone);

                    resultBuilder.Append("send_by_phone");
                }
                else // Otherwise, send the verification code via email
                {
                    Sender.SendValidationCode("Kod weryfikacyjny", user.Name, user.Lastname, code, email);
                    resultBuilder.Append("send_by_email");
                }
            }
            catch (MySqlException ex)
            {
                // In case of an exception, log the error and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "LoginController", "SendVeryficationCode", ex);
            }

            // Return the operation result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
        /// Verify Password Reset Code
        /// Verifies the validity of a password reset code provided by the user. It checks whether the code exists,
        /// is not expired, and then removes it from the database upon successful verification.
        ///
        /// Parameters:
        /// - code: The verification code to be validated for password reset.
        ///
        /// Responses:
        /// - "code_verified": Indicates successful verification of the code.
        /// - "no_code": Returned if the code parameter is empty or the code does not exist.
        /// - "code_expired": Returned if the code has expired.
        /// - "error": Returned in case of a database exception during the operation.
        /// </summary>
        /// <param name="code">The verification code string to verify.</param>
        /// <returns>JsonResult indicating the outcome of the verification process</returns>
        [HttpGet("/api/resetPassword/verifyCode/{code}")]
        public IActionResult VerifyCode(string code)
        {
            StringBuilder resultBuilder = new StringBuilder();

            // Check if the code is provided
            if (string.IsNullOrEmpty(code)) return new JsonResult(new { result = "no_code" });

            try
            {
                // Retrieve the verification code from the database
                var veryficationCode = _context.ValidationCodes.FirstOrDefault(vc => vc.Code == code);

                // If the code is not found, return 'no_code'
                if (veryficationCode == null) return new JsonResult(new { result = "no_code" });

                DateTime now = DateTime.Now;

                // Check if the code has expired
                if (now > veryficationCode.Expire) return new JsonResult(new { result = "code_expired" });

                // Remove the verification code from the database

                // Append 'code_verified' to the result builder
                resultBuilder.Append("code_verified");
            }
            catch (MySqlException ex)
            {
                // In case of a database exception, log the error and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("Mechapp", "LoginController", "VerifyCode", ex);
            }

            // Return the final result
            return new JsonResult(new { result = resultBuilder.ToString() });
        }

        /// <summary>
/// Reset User Password
/// Allows a user to reset their password. It checks the new password against password policies,
/// and if valid, updates the user's password in the database.
///
/// Parameters:
/// - resetPasswordOb reset: An object containing the new password, repeated password, and user email.
///   - reset.password: The new password.
///   - reset.resetPassword: The repeated new password for confirmation.
///   - reset.email: The email address of the user resetting their password.
///
/// Responses:
/// - "password_changed": Indicates successful password change.
/// - "no_password": Returned if any of the required fields (password, repeat password, email) are empty.
/// - "no_repeat": Returned if the password and repeat password do not match.
/// - "password_policy_not_match": Returned if the new password does not meet the password policy.
/// - "user_not_found": Returned if no user is associated with the provided email address.
/// - "error": Returned in case of a database exception during the operation.
/// </summary>
/// <param name="reset">The resetPasswordOb object containing reset information.</param>
/// <returns>JsonResult indicating the outcome of the password reset operation</returns>
    [HttpPost("/api/resetPassword")]
    public IActionResult ResetPassword([FromBody] resetPasswordOb reset)
    {
        StringBuilder resultBuilder = new StringBuilder();

        // Validate the provided information
        if (string.IsNullOrEmpty(reset.password) || string.IsNullOrEmpty(reset.repeatPassword) || 
            string.IsNullOrEmpty(reset.email) || string.IsNullOrEmpty(reset.token)) 
            return new JsonResult(new { result = "no_data" });

        if (reset.password != reset.repeatPassword) 
            return new JsonResult(new { result = "no_repeat" });

        if (!Validators.CheckPasswordPolicy(reset.password)) 
            return new JsonResult(new { result = "password_policy_not_match" });

        try
        {
            var veryficationCode = _context.ValidationCodes.FirstOrDefault(vc => vc.Code == reset.token);

            if (veryficationCode == null) return new JsonResult(new { result = "no_veryfication_code" });

            _context.ValidationCodes.Remove(veryficationCode);
            // Retrieve the user based on the provided email
            var user = _context.Users.FirstOrDefault(u => u.Email == reset.email);

            // If no user found, return 'user_not_found'
            if (user == null) return new JsonResult(new { result = "user_not_found" });

            // Generate a new salt and combine it with the new password
            string _salt = generators.generatePassword(10);
            string _combinedPassword = reset.password + _salt;

            // Update the user's password
            user.Password = hashes.GenerateSHA512Hash(_combinedPassword);
            _context.SaveChanges();

            // Indicate successful password change
            resultBuilder.Append("password_changed");
        }
        catch (MySqlException ex)
        {
            // In case of a database exception, log the error and return 'error'
            resultBuilder.Clear().Append("error");
            Logger.SendException("MechApp", "LoginController", "ResetPassword", ex);
        }

        // Return the final result
        return new JsonResult(new {result =  resultBuilder.ToString()});
    }

        /// <summary>
        /// Change Password on First Login
        /// Allows a user to change their password upon first login. 
        /// Validates the session token from cookies and ensures the password adheres to defined policies.
        ///
        /// Parameters:
        /// - firstPasswordOb pass: An object containing the new password and its repetition.
        ///   - pass.password: The new password.
        ///   - pass.repeatPassword: The repetition of the new password for confirmation.
        ///
        /// Responses:
        /// - "password_changed": Indicates the password was successfully changed.
        /// - "no_auth": Returned if there is no authentication token or if the token is invalid.
        /// - "no_data": Returned if required password data is missing.
        /// - "no_password_policy": Returned if the new password does not meet the password policy.
        /// - "password_not_matching": Returned if the password and its repetition do not match.
        /// - "error": Returned in case of a database exception during the operation or if user/token is not found.
        /// </summary>
        /// <param name="pass">The firstPasswordOb object containing new password information.</param>
        /// <returns>JsonResult indicating the outcome of the password change operation</returns>
        [HttpPost("/api/firstlogin")]
        public IActionResult ChangeFirstLoginPassword([FromBody] firstPasswordOb pass)
        {
            StringBuilder resultBuilder = new StringBuilder();

            string _cookieValue = string.Empty;

            // Validate the session token from cookies
            if (Request.Cookies["sessionToken"] != null)
            {
                _cookieValue = Request.Cookies["sessionToken"];
            }
            else return new JsonResult(new { result = "no_auth" });

            // Check if the token is valid
            if (!checkToken.checkCookie(_cookieValue)) return new JsonResult(new { result = "no_auth" });

            // Extend cookie expiration
            DateTime expireCookie = endCookieDate.GetEndCookieDate();
            CookieOptions cookieOptions = new CookieOptions { Expires = expireCookie };
            Response.Cookies.Append("sessionToken", _cookieValue, cookieOptions);

            // Validate password data
            if (string.IsNullOrEmpty(pass.password) || string.IsNullOrEmpty(pass.repeatPassword))
                return new JsonResult("no_data");

            if (!Validators.CheckPasswordPolicy(pass.password))
                return new JsonResult(new { result = "no_password_policy" });

            if (!Validators.isPasswordMatching(pass.password, pass.repeatPassword))
                return new JsonResult(new { result = "password_not_matching" });

            try
            {
                // Generate new password and salt
                string _salt = generators.generatePassword(10);
                string _combinedPassword = pass.password + _salt;

                // Find the user by session token
                var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.Token == _cookieValue);
                if (sessionToken == null)
                    return new JsonResult(new { result = "error" });

                var user = _context.Users.FirstOrDefault(u => u.Id == sessionToken.UserId);
                if (user == null)
                    return new JsonResult(new { result = "error" });

                // Update the user's password
                user.Password = hashes.GenerateSHA512Hash(_combinedPassword);
                user.Salt = _salt;
                user.IsFirstLogin = 0;
                _context.SaveChanges();

                // Indicate successful password change
                resultBuilder.Append("password_changed");
            }
            catch (MySqlException ex)
            {
                // Log exception and return 'error'
                resultBuilder.Clear().Append("error");
                Logger.SendException("MechApp", "LoginController", "ChangeFirstLoginPassword", ex);
            }

            return new JsonResult(new { result = resultBuilder.ToString() });
        }


    }
    public class loginOb //user login object
    {
        public string? email { get; set; }
        public string? password { get; set; }
    }

    public class resetPasswordOb
    {
        public string? token { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? repeatPassword { get; set; }
    }

    public class firstPasswordOb
    {
        public string? password { get; set; }
        public string? repeatPassword { get; set; }
    }
}
