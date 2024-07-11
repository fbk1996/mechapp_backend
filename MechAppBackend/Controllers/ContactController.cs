using MechAppBackend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MechAppBackend.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        /// <summary>
        /// Sends a contact email with the provided message details.
        /// </summary>
        /// <param name="message">The contact message containing name, lastname, email, phone, and the message content.</param>
        /// <returns>
        /// A JSON result indicating the outcome of the operation.
        /// Possible results include:
        /// - "message_sended" if the email is successfully sent.
        /// - "no_name" if the name is missing.
        /// - "no_lastname" if the lastname is missing.
        /// - "no_email" if the email is missing.
        /// - "no_message" if the message content is missing.
        /// - "error" if an error occurs during the operation.
        /// </returns>
        /// <remarks>
        /// This endpoint does not require authentication.
        /// </remarks>
        [HttpPost]
        public IActionResult SendEmail([FromBody]mailMessage message)
        {
            StringBuilder resultBuilder = new StringBuilder();

            if (string.IsNullOrEmpty(message.name))
                return new JsonResult(new { result = "no_name" });
            if (string.IsNullOrEmpty(message.lastname))
                return new JsonResult(new { result = "no_lastname" });
            if (string.IsNullOrEmpty(message.email))
                return new JsonResult(new { result = "no_email" });
            if (string.IsNullOrEmpty(message.message))
                return new JsonResult(new { result = "no_message" });

            try
            {
                Sender.SendContactMessage("Wiadomość - kontakt", message.name, message.lastname, message.message, message.email, message.phone);
                Sender.SendMailConfirmationtEmail("Wiadomość - kontakt", message.name, message.lastname, message.email, message.message);
                resultBuilder.Append("message_sended");
            }
            catch (Exception ex)
            {
                resultBuilder.Clear().Append("error");
                Logger.SendNormalException("MechApp", "ContactController", "SendEmail", ex);
            }

            return new JsonResult(new {result =  resultBuilder.ToString()});
        }
    }

    public class mailMessage
    {
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? message { get; set; }
    }
}
