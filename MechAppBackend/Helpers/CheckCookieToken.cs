using MechAppBackend.Data;

namespace MechAppBackend.Helpers
{
    public class CheckCookieToken
    {
        MechAppContext _context;

        public CheckCookieToken(MechAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Check User Session Token
        /// Verifies the validity of a given session token. It checks if the token exists and whether it has expired.
        /// If the token is valid and not expired, the expiration time is extended by two hours.
        ///
        /// Parameters:
        /// - token: The session token to be verified.
        ///
        /// Responses:
        /// - true: If the token is valid and the expiration time has been successfully extended.
        /// - false: If the token is invalid, does not exist, or has expired.
        /// </summary>
        /// <param name="token">The session token string to verify.</param>
        /// <returns>A boolean indicating whether the token is valid and expiration was extended.</returns>
        public bool checkCookie(string token)
        {
            bool result = false;

            // Retrieve the session token from the database
            var sessionToken = _context.UsersTokens.FirstOrDefault(st => st.Token == token);

            // Return false if the token does not exist
            if (sessionToken == null) return false;

            DateTime now = DateTime.Now;

            // Check if the token has expired
            if (now > sessionToken.Expire) return false;

            // Extend the expiration date of the token by two hours
            DateTime expireDate = endCookieDate.GetEndCookieDate();
            sessionToken.Expire = expireDate;

            // Save the updated token information in the database
            _context.SaveChanges();

            // Set result to true as the token is valid and expiration has been extended
            result = true;

            // Return the final result
            return result;
        }
    }
}
