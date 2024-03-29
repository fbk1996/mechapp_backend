using PhoneNumbers;
using System.Text.RegularExpressions;

namespace MechAppBackend.Helpers
{
    public class Validators
    {
        public static bool CheckLength(string _data, int _length)
        {
            if (_data.Length > _length) return false;
            else return true;
        }

        public static bool CheckPhone(string _phone, string CountryCode)
        {
            CountryCode = CountryCode.ToUpper();

            PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();

            try
            {
                var parsedNumber = phoneNumberUtil.Parse(_phone, CountryCode);

                return phoneNumberUtil.IsValidNumber(parsedNumber);
            }
            catch
            {
                return false;
            }
        }

        public static bool checkTemplate(string _data, string _template)
        {
            bool result = true;

            for (int i = 0; i < _template.Length; i++)
            {
                if (i > _data.Length - 1) { result = false; break; }

                if (_template[i] != '0' && _template[i] != _data[i]) { result = false; break; }

                if (_template[i] == '0' && !char.IsNumber(_data[i])) { result = false; break; }
            }

            return result;
        }

        public static bool CheckForSQLVulneralibities(string _userInput)
        {
            // List of SQL keywords to check against
            string[] sqlKeywords = { "alter", "select", "insert", "update", "delete", "drop", "truncate", "create", "grant", "revoke" };

            // Convert user input to lowercase for case-insensitive matching
            string userInputLower = _userInput.ToLower();

            // Check if any SQL keywords are present in the user input
            foreach (string keyword in sqlKeywords)
            {
                if (userInputLower.Contains(keyword))
                {
                    // SQL statement found
                    return true;
                }
            }

            // No SQL statements found
            return false;
        }

        public static bool CheckPasswordPolicy(string password)
        {
            int minLength = 8;
            int maxLength = 30;
            bool hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
            bool hasNumber = Regex.IsMatch(password, @"\d");
            bool hasSpecialChar = Regex.IsMatch(password, @"[!@#$%^&*()_+\-=[\]{};':""\\|,.<>/?]");

            // Check if the password meets the policy criteria
            if (password.Length >= minLength &&
                password.Length <= maxLength &&
                hasUpperCase &&
                hasNumber &&
                hasSpecialChar)
            {
                return true; // Password meets the policy
            }
            else
            {
                return false; // Password does not meet the policy
            }
        }

        public static bool isContainsErrorCharacters(string _input)
        {
            string[] sqlKeywords = { "|", "<", ">", "$" };

            foreach (string keyword in sqlKeywords)
            {
                if (_input.Contains(keyword))
                {
                    // SQL statement found
                    return true;
                }
            }

            // No SQL statements found
            return false;
        }

        public static bool isPasswordMatching(string password, string repeatPassword)
        {
            return password == repeatPassword;
        }

        public static bool ValidateEmail(string inputText)
        {
            var mailFormat = @"^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,6})+$";
            return Regex.IsMatch(inputText, mailFormat);
        }
    }
}
