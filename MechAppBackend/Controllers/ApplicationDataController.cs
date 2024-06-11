using MechAppBackend.AppSettings;
using MechAppBackend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MechAppBackend.Controllers
{
    [Route("api/appdata")]
    [ApiController]
    public class ApplicationDataController : ControllerBase
    {
        /// <summary>
        /// HTTP GET endpoint for retrieving application data.
        /// </summary>
        /// <returns>JSON result containing the application data such as logos, company name, address, postcode, city, phone, and emails.</returns>
        [HttpGet]
        public IActionResult GetApplicationData()
        {
            return new JsonResult(new
            {
                loginLogo = appdata.logo2Url,
                appLogo = appdata.logoUrl,
                companyName = appdata.companyName,
                companyAddress = appdata.companyAddress,
                companyPostcode = appdata.companyPostcode,
                companyCity = appdata.companyCity,
                companyPhone = appdata.companyPhone,
                companyEmail = appdata.companyEmail,
                companyUserDataEmail = appdata.companyUserDataEmail
            });
        }
    }
}
