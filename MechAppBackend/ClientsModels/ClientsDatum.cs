using System;
using System.Collections.Generic;

namespace MechAppBackend.ClientsModels
{
    public partial class ClientsDatum
    {
        public string? Subscription { get; set; }
        public string? Dbhost { get; set; }
        public string? Dbdatabase { get; set; }
        public string? Dbuser { get; set; }
        public string? Dbpassword { get; set; }
        public string? SmtpHost { get; set; }
        public string? SmtpPort { get; set; }
        public string? SmtpUser { get; set; }
        public string? SmtpPassword { get; set; }
        public string? AppPrefix { get; set; }
        public string? SmsApiToken { get; set; }
        public string? SmsApiSender { get; set; }
        public string? LogoUrl { get; set; }
        public string? Logo2Url { get; set; }
        public string? LoginUrl { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyPostcode { get; set; }
        public string? CompanyCity { get; set; }
        public string? CompanyPhone { get; set; }
        public string? CompanyEmail { get; set; }
        public string? SecureKey { get; set; }
        public long? EmployeeAmount { get; set; }
        public long? OrdersAmount { get; set; }
        public DateTime? DateEnd { get; set; }
        public long Id { get; set; }
        public string? EmailLogoUrl { get; set; }
        public string? CompanyUserDataEmail { get; set; }
    }
}
