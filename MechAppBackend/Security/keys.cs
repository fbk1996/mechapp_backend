namespace MechAppBackend.Security
{
    public class keys
    {
        private static string msqkey = "e+0YO0bl/3g+2vr/zdVMapwxvdXUb+IqfPqmgNPX+Ag=";

        public static string GetMysqlKey()
        {
            return msqkey;
        }

        public static void SetMysqlKey(string _key)
        {
            msqkey = _key;
        }
    }
}
