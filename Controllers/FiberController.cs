using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FiberConnectionClient.Models;
using Microsoft.AspNetCore.Mvc;
using JWT;
using Newtonsoft.Json;
using System.Text;

namespace FiberConnectionClient.Controllers
{
    public class FiberController : Controller
    {
        [HttpGet]
        public IActionResult UpdatePlan()
        {
            string AdminExp = HttpContext.Request.Cookies["AdminExpiry"];
            string AdminToken = HttpContext.Request.Cookies["AdminToken"];
            if (Convert.ToDateTime(AdminExp) < DateTime.Now)
            {
                return RedirectToAction("AdminLogin", "Admin");

            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePlan(FiberPlan fp)
        {
            string AdminToken = HttpContext.Request.Cookies["AdminToken"];
            using (var client =new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);
                StringContent addfp = new StringContent(JsonConvert.SerializeObject(fp), Encoding.UTF8, "application/json");
                var res = await client.PostAsync("https://fiberplanapiteam3.azurewebsites.net/api/Fiber", addfp);
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction("AdminControlPlanDetails");
                }
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AdminControlPlanDetails()
        {
            string AdminExp = HttpContext.Request.Cookies["AdminExpiry"];
            string AdminToken = HttpContext.Request.Cookies["AdminToken"];
            if (Convert.ToDateTime(AdminExp) < DateTime.Now)
            {
                return RedirectToAction("AdminLogin", "Admin");

            }
            List<FiberPlan> fpp = new List<FiberPlan>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);
                var res = await client.GetAsync("https://fiberplanapiteam3.azurewebsites.net/api/Fiber");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    fpp = JsonConvert.DeserializeObject<List<FiberPlan>>(result);
                }
                return View(fpp);
            }
        }
        [HttpGet]
        public async Task<IActionResult> PlanDetails()
        {
            List<FiberPlan> fpp = new List<FiberPlan>();
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync("https://fiberplanapiteam3.azurewebsites.net/api/Fiber");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    fpp = JsonConvert.DeserializeObject<List<FiberPlan>>(result);

                }
                return View(fpp);
            }

        }
        [HttpGet]
        public async Task<IActionResult> AdminControlEditPlan(int id)
        {
            string AdminExp = HttpContext.Request.Cookies["AdminExpiry"];
            string AdminToken = HttpContext.Request.Cookies["AdminToken"];
            if (Convert.ToDateTime(AdminExp) < DateTime.Now)
            {
                return RedirectToAction("AdminLogin", "Admin");

            }
            FiberPlan fp = new FiberPlan();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);
                var res = await client.GetAsync("https://fiberplanapiteam3.azurewebsites.net/api/Fiber/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    fp = JsonConvert.DeserializeObject<FiberPlan>(result);

                }
                return View(fp);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AdminControlEditPlan(int id, FiberPlan fp)
        {
            string AdminExp = HttpContext.Request.Cookies["AdminExpiry"];
            if (Convert.ToDateTime(AdminExp) < DateTime.Now)
            {
                return RedirectToAction("AdminLogin", "Admin");

            }
            string AdminToken = HttpContext.Request.Cookies["AdminToken"];
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);
                StringContent editfp = new StringContent(JsonConvert.SerializeObject(fp), Encoding.UTF8, "application/json");
                var res = await client.PutAsync("https://fiberplanapiteam3.azurewebsites.net/api/Fiber?id=" + id, editfp);
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction("AdminControlPlanDetails");
                }
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AdminControlDeletePlan(int id)
        {

            string AdminToken = HttpContext.Request.Cookies["AdminToken"];
            string AdminExp = HttpContext.Request.Cookies["AdminExpiry"];
            if (Convert.ToDateTime(AdminExp) < DateTime.Now)
            {
                return RedirectToAction("AdminLogin", "Admin");

            }
            FiberPlan fp = new FiberPlan();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);
                var res = await client.GetAsync("https://fiberplanapiteam3.azurewebsites.net/api/Fiber/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    fp = JsonConvert.DeserializeObject<FiberPlan>(result);

                }
                return View(fp);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AdminControlDeletePlan(int id,FiberPlan fp)
        {

                string AdminToken = HttpContext.Request.Cookies["AdminToken"];
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);
                    var res = await client.DeleteAsync("https://fiberplanapiteam3.azurewebsites.net/api/Fiber?id=" + id);
                    if (res.IsSuccessStatusCode)
                    {
                        return RedirectToAction("AdminControlPlanDetails");
                    }
                }
                return RedirectToAction("AdminControlPlanDetails");
        }

    }
}
