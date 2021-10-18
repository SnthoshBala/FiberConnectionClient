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
using Microsoft.AspNetCore.Http;

namespace FiberConnectionClient.Controllers
{
    public class StatusController : Controller
    {
        public async Task<IActionResult> StatusList(int id)
        {
            string Token = HttpContext.Request.Cookies["Token"];
            if (string.IsNullOrEmpty(Token))
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            id = Convert.ToInt32(HttpContext.Request.Cookies["UserId"]);
            List<Status> s= new List<Status>();
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync("https://statusapiteam3.azurewebsites.net/api/Status/AllStatus/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    s = JsonConvert.DeserializeObject<List<Status>>(result);

                }
                return View(s);
            }
        }
        [HttpGet]
        public async Task<IActionResult> StatusDetails(int id)
        {

            string Token = HttpContext.Request.Cookies["Token"];
            if (string.IsNullOrEmpty(Token))
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            Status s = new Status();
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync("https://statusapiteam3.azurewebsites.net/api/Status/StatusById/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    s = JsonConvert.DeserializeObject<Status>(result);
                }
                return View(s);
            }
        }
        [HttpGet]
        public async Task<IActionResult> StatusCancel(int id)
        {
            string Token = HttpContext.Request.Cookies["Token"];
            if (string.IsNullOrEmpty(Token))
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            Status s = new Status();
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync("https://statusapiteam3.azurewebsites.net/api/Status/StatusById/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    s = JsonConvert.DeserializeObject<Status>(result);
                }
                return View(s);
            }
        }
        [HttpPost]
        public async Task<IActionResult> StatusCancel(int id,Status s)
        {
            string Token = HttpContext.Request.Cookies["Token"];
            if (string.IsNullOrEmpty(Token))
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var res = await client.DeleteAsync("https://statusapiteam3.azurewebsites.net/api/Status?id=" + id);
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction("StatusList");
                }
            }
            return RedirectToAction("StatusList");
        }
        [HttpGet]
        public async Task<IActionResult> StatusEdit(int id)
        {
            string EmpToken = HttpContext.Request.Cookies["EmpToken"];
            if (string.IsNullOrEmpty(EmpToken))
            {
                return RedirectToAction("EmployeeLogin", "Employee");
            }
            Status s = new Status();
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync("https://statusapiteam3.azurewebsites.net/api/Status/StatusById/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    s = JsonConvert.DeserializeObject<Status>(result);
                    s.EmployeeName = HttpContext.Session.GetString("EmpName");
                    s.EmployeePhonenumber = HttpContext.Session.GetString("EmpPhone");
                    s.Status1 = "On Process";
                }
                return View(s);
            }
        }
        [HttpPost]
        public async Task<IActionResult> StatusEdit(Status s,int id)
        {
            string EmpToken = HttpContext.Request.Cookies["EmpToken"];
            if (string.IsNullOrEmpty(EmpToken))
            {
                return RedirectToAction("EmployeeLogin", "Employee");
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EmpToken);
                StringContent edits = new StringContent(JsonConvert.SerializeObject(s), Encoding.UTF8, "application/json");
                var res = await client.PutAsync("https://statusapiteam3.azurewebsites.net/api/Status?id=" + id, edits);
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction("AllStatus");
                }
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AllStatus()
        {
            string EmpToken = HttpContext.Request.Cookies["EmpToken"];
            if (string.IsNullOrEmpty(EmpToken))
            {
                return RedirectToAction("EmployeeLogin", "Employee");
            }
            List<Status> s = new List<Status>();
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync("https://statusapiteam3.azurewebsites.net/api/Status/AllStatus");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    s = JsonConvert.DeserializeObject<List<Status>>(result);
                }
                return View(s);
            }
        }
    }
}
