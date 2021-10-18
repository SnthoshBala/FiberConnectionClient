using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiberConnectionClient.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FiberConnectionClient.Models;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace FiberConnectionClient.Controllers
{
    public class BillingController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> RequestBilling(int id)
        {
            string Token = HttpContext.Request.Cookies["Token"];
            string Exp = HttpContext.Request.Cookies["Expiry"];
            if (Convert.ToDateTime(Exp) < DateTime.Now)
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            Billing b = new Billing();
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync("https://billingapiteam3.azurewebsites.net/api/Billing/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    b = JsonConvert.DeserializeObject<Billing>(result);

                }
                return View(b);
            }
        }
        [HttpPost]
        public async Task<IActionResult> RequestBilling(int id,Billing b)
        {
            string Token = HttpContext.Request.Cookies["Token"];
            string Exp = HttpContext.Request.Cookies["Expiry"];
            if (Convert.ToDateTime(Exp) < DateTime.Now)
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            StringContent b_content = new StringContent(JsonConvert.SerializeObject(b), Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var res = await client.PutAsync("https://billingapiteam3.azurewebsites.net/api/Billing?=" + id, b_content);
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction("PlanDetails", "Fiber");
                }
                return RedirectToAction("PlanDetails", "Fiber");
            }
        }

        [Route("BillingDetails/RequestBill/{id:int}")]
        public async Task<IActionResult> RequestBill(Billing b, int id)
        {
            string Token = HttpContext.Request.Cookies["Token"];
            string Exp = HttpContext.Request.Cookies["Expiry"];
            if (Convert.ToDateTime(Exp) < DateTime.Now)
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            int c_id =Convert.ToInt32(HttpContext.Request.Cookies["UserId"]);
            int reqid =0;
            StringContent b_content = new StringContent(JsonConvert.SerializeObject(b), Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                var res = await client.PostAsync("https://billingapiteam3.azurewebsites.net/api/Billing?id=" + id+"&c_id="+c_id, b_content);
                if (res.IsSuccessStatusCode)
                    {
                    var result = res.Content.ReadAsStringAsync().Result;
                    reqid = JsonConvert.DeserializeObject<int>(result);
                    return RedirectToAction("RequestBilling", new { id = reqid });
                }
                return RedirectToAction("PlanDetails","Fiber");
            }
        }
    }
}
