using MechAppBackend.AppSettings;
using MechAppBackend.Conns;
using System.Net.Mail;
using System.Net;
using Google.Protobuf;

namespace MechAppBackend.Helpers
{
    public class Sender
    {
        private static SmtpClient smtpclient()
        {
            return new SmtpClient()
            {
                Host = connections.smtpHost,
                Port = connections.smtpPort,
                Credentials = new NetworkCredential(connections.smtpUser, connections.smtpPassword),
                EnableSsl = true
            };
        }

        public static async void SendAddEmployeeEmail(string _title, string _name, string _lastname, string _email, string _password)
        {
            try
            {

                if (string.IsNullOrEmpty(connections.smtpHost) || string.IsNullOrEmpty(connections.smtpUser) || string.IsNullOrEmpty(connections.smtpPassword)) return;

                string _message = EmailTemplates.AddEmployeeTemplate;
                string _mesTitle = EmailTemplates.AddEmployeeTitle;

                _message = _message.Replace("#title", _title).Replace("#name", _name).Replace("#lastname", _lastname).Replace("#login", _email).Replace("#password", _password);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(connections.smtpUser);
                    mail.To.Add(_email);
                    mail.Subject = _mesTitle;
                    mail.Body = _message;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = smtpclient())
                    {
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("mechapp", "SMTP SENDER", "ADD EMPLOYEE", ex);
            }
        }

        public static async void SendExistingAccountAddEmployeeEmail(string _title, string _name, string _lastname, string _email)
        {
            try
            {
                if (string.IsNullOrEmpty(connections.smtpHost) || string.IsNullOrEmpty(connections.smtpUser) || string.IsNullOrEmpty(connections.smtpPassword)) return;

                string _message = EmailTemplates.GreetExistingAccountEmployee;
                string _mesTitle = EmailTemplates.AddEmployeeTitle;

                _message = _message.Replace("#title", _title).Replace("#name", _name).Replace("#lastname", _lastname);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(connections.smtpUser);
                    mail.To.Add(_email);
                    mail.Subject = _mesTitle;
                    mail.Body = _message;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = smtpclient())
                    {
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("mechapp", "SMTP SENDER", "SEND EXIST", ex);
            }
        }

        public static async void SendValidationCode(string _title, string _name, string _lastname, string _code, string _email)
        {
            try
            {
                if (string.IsNullOrEmpty(connections.smtpHost) || string.IsNullOrEmpty(connections.smtpUser) || string.IsNullOrEmpty(connections.smtpPassword)) return;

                string _message = EmailTemplates.validationCodeMailMessage;
                string _mesTitle = EmailTemplates.validationCodeMailTitle;

                _message = _message.Replace("#title", _title).Replace("#name", _name).Replace("#lastname", _lastname).Replace("#token", _code);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(connections.smtpUser);
                    mail.To.Add(_email);
                    mail.Subject = _mesTitle;
                    mail.Body = _message;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = smtpclient())
                    {
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("mechapp", "SMTP SENDER", "SEND VALIDATION CODE", ex);
            }
        }

        public static async void SendContactMessage(string _title, string _name, string _lastname, string _mess, string _email, string _phone)
        {
            try
            {
                if (string.IsNullOrEmpty(connections.smtpHost) || string.IsNullOrEmpty(connections.smtpUser) || string.IsNullOrEmpty(connections.smtpPassword)) return;

                string _message = EmailTemplates.ContactMessage;
                string _mesTitle = EmailTemplates.ContactMessageTitle;

                _message = _message.Replace("#title", _title).Replace("#name", _name).Replace("#lastname", _lastname).Replace("#phone", _phone).Replace("#email", _email).Replace("#message", _mess);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(connections.smtpUser);
                    mail.To.Add(appdata.companyEmail);
                    mail.Subject = _mesTitle;
                    mail.Body = _message;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = smtpclient())
                    {
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("mechapp", "SMTP SENDER", "SEND CONTACT MESSAGE", ex);
            }
        }

        public static async void SendCreateEstimateMessage(string _title, string _name, string _lastname, string _orderNumber, string _vehicleProducer, string _vehicleModel,
            string _vehicleRegisterNumber, string _parts, string _services, string _totalPartsPrice, string _totalServicesPrice, string _totalPrice, string _email)
        {
            try
            {
                if (string.IsNullOrEmpty(connections.smtpHost) || string.IsNullOrEmpty(connections.smtpUser) || string.IsNullOrEmpty(connections.smtpPassword)) return;

                string _message = EmailTemplates.AddEstimateMessage;
                string _mesTitle = EmailTemplates.AddEstimateTitle;

                _message = _message.Replace("#title", _title).Replace("#name", _name).Replace("#lastname", _lastname).Replace("#orderNumber", _orderNumber)
                    .Replace("#vehicleProducer", _vehicleProducer).Replace("#vehicleModel", _vehicleModel).Replace("#registerNumber", _vehicleRegisterNumber)
                    .Replace("#parts", _parts).Replace("#services", _services).Replace("#totalPartsPrice", _totalPartsPrice).Replace("#totalServicesPrice", _totalServicesPrice)
                    .Replace("#totalPrice", _totalPrice);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(connections.smtpUser);
                    mail.To.Add(_email);
                    mail.Subject = _mesTitle;
                    mail.Body = _message;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = smtpclient())
                    {
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("mechapp", "SMTP SENDER", "SEND ADD ESTIMATE MESSAGE", ex);
            }
        }

        public static async void SendEditEstimateMessage(string _title, string _name, string _lastname, string _orderNumber, string _vehicleProducer, string _vehicleModel,
            string _vehicleRegisterNumber, string _parts, string _services, string _totalPartsPrice, string _totalServicesPrice, string _totalPrice, string _email)
        {
            try
            {
                if (string.IsNullOrEmpty(connections.smtpHost) || string.IsNullOrEmpty(connections.smtpUser) || string.IsNullOrEmpty(connections.smtpPassword)) return;

                string _message = EmailTemplates.EditEstimateMessage;
                string _mesTitle = EmailTemplates.EditEstimateMessageTitle;

                _message = _message.Replace("#title", _title).Replace("#name", _name).Replace("#lastname", _lastname).Replace("#orderNumber", _orderNumber)
                    .Replace("#vehicleProducer", _vehicleProducer).Replace("#vehicleModel", _vehicleModel).Replace("#registerNumber", _vehicleRegisterNumber)
                    .Replace("#parts", _parts).Replace("#services", _services).Replace("#totalPartsPrice", _totalPartsPrice).Replace("#totalServicesPrice", _totalServicesPrice)
                    .Replace("#totalPrice", _totalPrice);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(connections.smtpUser);
                    mail.To.Add(_email);
                    mail.Subject = _mesTitle;
                    mail.Body = _message;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = smtpclient())
                    {
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("mechapp", "SMTP SENDER", "SEND EDIT ESTIMATE MESSAGE", ex);
            }
        }

        public static async void SendOrderReadyStatus(string _title, string _name, string _lastname, string _email)
        {
            try
            {
                if (string.IsNullOrEmpty(connections.smtpHost) || string.IsNullOrEmpty(connections.smtpUser) || string.IsNullOrEmpty(connections.smtpPassword)) return;

                string _message = EmailTemplates.OrderStatusReadyMessage;
                string _mesTitle = EmailTemplates.OrderStatusReadyMessageTitle;

                _message = _message.Replace("#title", _title).Replace("#name", _name).Replace("#lastname", _lastname);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(connections.smtpUser);
                    mail.To.Add(_email);
                    mail.Subject = _mesTitle;
                    mail.Body = _message;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = smtpclient())
                    {
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("mechapp", "SMTP SENDER", "SEND CONTACT MESSAGE", ex);
            }
        }

        public static async void SendAddClientEmail(string _title, string _name, string _lastname, string _email, string _password)
        {
            try
            {
                if (string.IsNullOrEmpty(connections.smtpHost) || string.IsNullOrEmpty(connections.smtpUser) || string.IsNullOrEmpty(connections.smtpPassword)) return;

                string _message = EmailTemplates.SendAddClientMessage;
                string _mesTitle = EmailTemplates.SendAddClientMessageTitle;

                _message = _message.Replace("#title", _title).Replace("#name", _name).Replace("#lastname", _lastname).Replace("#login", _email)
                    .Replace("#password", _password);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(connections.smtpUser);
                    mail.To.Add(_email);
                    mail.Subject = _mesTitle;
                    mail.Body = _message;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = smtpclient())
                    {
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("mechapp", "SMTP SENDER", "ADD CLIENT", ex);
            }
        }

        public static async void SendMailConfirmationtEmail(string _title, string _name, string _lastname, string _email, string _mess)
        {
            try
            {
                if (string.IsNullOrEmpty(connections.smtpHost) || string.IsNullOrEmpty(connections.smtpUser) || string.IsNullOrEmpty(connections.smtpPassword)) return;

                string _message = EmailTemplates.SendContactConfirmationMessage;
                string _mesTitle = EmailTemplates.SendContactConfirmationTitle;

                _message = _message.Replace("#title", _title).Replace("#name", _name).Replace("#lastname", _lastname)
                    .Replace("#mess", _mess);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(connections.smtpUser);
                    mail.To.Add(_email);
                    mail.Subject = _mesTitle;
                    mail.Body = _message;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = smtpclient())
                    {
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("mechapp", "SMTP SENDER", "ADD CLIENT", ex);
            }
        }

        public static async void SendStartComplaintEmail(string _title, string _name, string _lastname, string _orderId, string _email)
        {
            if (string.IsNullOrEmpty(connections.smtpHost) || string.IsNullOrEmpty(connections.smtpUser) || string.IsNullOrEmpty(connections.smtpPassword)) return;

            string title = EmailTemplates.SendComplaintStartTitle;
            string message = EmailTemplates.SendComplaintStartMessage;

            message = message.Replace("#title", _title).Replace("#name", _name).Replace("#lastname", _lastname).Replace("#orderID", _orderId);

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(connections.smtpUser);
                mail.To.Add(_email);
                mail.Subject = title;
                mail.Body = message;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = smtpclient())
                {
                    await smtp.SendMailAsync(mail);
                }
            }
        }

        public static async void SendDecisionComplaintEmail(string _title, string _name, string _lastname, string _orderId, string _decision, string _description, string _email)
        {
            if (string.IsNullOrEmpty(connections.smtpHost) || string.IsNullOrEmpty(connections.smtpUser) || string.IsNullOrEmpty(connections.smtpPassword)) return;

            string title = EmailTemplates.SendComplaintDecisionTitle;
            string message = EmailTemplates.SendComplaintStartMessage;

            message = message.Replace("#title", _title).Replace("#name", _name).Replace("#lastname", _lastname).Replace("#orderID", _orderId).Replace("#decision", _decision).Replace("#description", _description);
            title = title.Replace("#decision", _decision);

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(connections.smtpUser);
                mail.To.Add(_email);
                mail.Subject = title;
                mail.Body = message;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = smtpclient())
                {
                    await smtp.SendMailAsync(mail);
                }
            }
        }
    }
}
