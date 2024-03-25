using MechAppBackend.Conns;
using MechAppBackend.Data;
using MechAppBackend.Helpers;
using MechAppBackend.Security;
using MySql.Data.MySqlClient;

namespace MechAppBackend.AppSettings
{
    public class GetAppData
    {
        MechappClientsContext _context = new MechappClientsContext();


        public void GetAppDataMethod()
        {
            try
            {
                var clientData = _context.ClientsData.FirstOrDefault(c => c.Subscription == appdata.subscription);

                if (clientData == null) return;

                connections.dbhost = clientData.Dbhost;
                connections.dbdatabase = clientData.Dbdatabase;
                connections.dbuser = clientData.Dbuser;
                connections.dbpassword = clientData.Dbpassword;
                connections.smtpHost = clientData.SmtpHost;
                connections.smtpPort = Convert.ToInt32(clientData.SmtpPort);
                connections.smtpUser = clientData.SmtpUser;
                connections.smtpPassword = clientData.SmtpPassword;
                connections.appPrefix = clientData.AppPrefix;
                connections.smsApiToken = clientData.SmsApiToken;
                connections.smsApiSender = clientData.SmsApiSender;
                appdata.logoUrl = clientData.LogoUrl;
                appdata.logo2Url = clientData.Logo2Url;
                appdata.loginUrl = clientData.LoginUrl;
                appdata.companyName = clientData.CompanyName;
                appdata.companyAddress = clientData.CompanyAddress;
                appdata.companyPostcode = clientData.CompanyPostcode;
                appdata.companyCity = clientData.CompanyCity;
                appdata.companyPhone = clientData.CompanyPhone;
                appdata.companyEmail = clientData.CompanyEmail;
                keys.SetMysqlKey(clientData.SecureKey);
                appdata.employeeAmount = clientData.EmployeeAmount;
                appdata.ordersAmount = clientData.OrdersAmount;
                appdata.endDate = clientData.DateEnd;
                appdata.emailLogoUrl = clientData.EmailLogoUrl;
                appdata.companyUserDataEmail = clientData.CompanyUserDataEmail;
            }
            catch (MySqlException ex)
            {
                Logger.SendException("Mechapp", "GetAppData", "GetAppDataMethod", ex);
            }
        }
    }
}
