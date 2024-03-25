namespace MechAppBackend.Helpers
{
    public class generators
    {
        private static string alphabet = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpRrSsTtUuWwVvXxYyZz1234567890!@#$%&";

        private static string verCodeAlphabet = "0123456789";

        public static string generatePassword(int _lenght)
        {
            char[] newPassword = new char[_lenght];

            Random rnd = new Random();

            for (int i = 0; i < newPassword.Length; i++)
            {
                newPassword[i] = alphabet[rnd.Next(0, alphabet.Length)];
            }

            return new string(newPassword);
        }

        public static string generateValidationCode(int _lenght)
        {
            char[] newPassword = new char[_lenght];

            Random rnd = new Random();

            for (int i = 0; i < newPassword.Length; i++)
            {
                newPassword[i] = verCodeAlphabet[rnd.Next(0, verCodeAlphabet.Length)];
            }

            return new string(newPassword);
        }
    }
}
