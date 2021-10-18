using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FiberConnectionClient.Models;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Web;

namespace FiberConnectionClient.Controllers
{
    public class EmployeeController : Controller
    {
        string EmpToken = "";

        IConfiguration _config;

        private IJsonSerializer _serializer = new JsonNetSerializer();
        private IDateTimeProvider _provider = new UtcDateTimeProvider();
        private IBase64UrlEncoder _urlEncoder = new JwtBase64UrlEncoder();
        private IJwtAlgorithm _algorithm = new HMACSHA256Algorithm();
        public EmployeeController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        public IActionResult EmployeeLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EmployeeLogin(Employee e)
        {
            using (var client = new HttpClient())
            {
                StringContent e_content = new StringContent(JsonConvert.SerializeObject(e), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://authorizationapiteam3.azurewebsites.net/api/Authorization/EmployeeLogin", e_content);
                var response1 = await client.PostAsync("https://employeeapi3.azurewebsites.net/api/Employee/Login", e_content);
                if (response.IsSuccessStatusCode)
                {
                    if (response1.IsSuccessStatusCode)
                    {
                        EmpToken = response.Content.ReadAsStringAsync().Result;
                        HttpContext.Response.Cookies.Append("EmpToken", EmpToken);

                        string e1 = response1.Content.ReadAsStringAsync().Result;
                        Employee EmployeeDetails = JsonConvert.DeserializeObject<Employee>(e1);

                        HttpContext.Session.SetString("EmpName", EmployeeDetails.Name);
                        HttpContext.Session.SetInt32("EmpId", EmployeeDetails.EmployeeId);
                        HttpContext.Session.SetString("EmpPhone", EmployeeDetails.PhoneNumber);

                        IJwtValidator _validator = new JwtValidator(_serializer, _provider);
                        IJwtDecoder decoder = new JwtDecoder(_serializer, _validator, _urlEncoder, _algorithm);
                        var EmptokenExp = decoder.DecodeToObject<JwtTokenExp>(EmpToken);
                        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(EmptokenExp.exp);
                        DateTime EmptimeExp = dateTimeOffset.LocalDateTime;

                        HttpContext.Response.Cookies.Append("EmpExpiry", EmptimeExp.ToString());
                        return RedirectToAction("AllStatus", "Status");
                    }
                    else
                    {
                        return View();
                    }
                }
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            string EmpToken = HttpContext.Request.Cookies["EmpToken"];
            string EmpExp = HttpContext.Request.Cookies["EmpExpiry"];
            if (Convert.ToDateTime(EmpExp) < DateTime.Now)
            { 
                return RedirectToAction("EmployeeLogin", "Employee");
            }
            Employee e = new Employee();
            int id = (int)HttpContext.Session.GetInt32("EmpId");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EmpToken);
                var res = await client.GetAsync("https://employeeapi3.azurewebsites.net/api/Employee/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    e = JsonConvert.DeserializeObject<Employee>(result);
                }
            }
            return View(e);
        }
        [HttpGet]
        public async Task<IActionResult> ProfileEdit()
        {
            string EmpToken = HttpContext.Request.Cookies["EmpToken"];
            string EmpExp = HttpContext.Request.Cookies["EmpExpiry"];
            if (Convert.ToDateTime(EmpExp) < DateTime.Now)
            {
                return RedirectToAction("EmployeeLogin", "Employee");
            }
            Employee e = new Employee();
            int id = (int)HttpContext.Session.GetInt32("EmpId");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EmpToken);
                var res = await client.GetAsync("https://employeeapi3.azurewebsites.net/api/Employee/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    e = JsonConvert.DeserializeObject<Employee>(result);

                }
            }
            return View(e);
        }
        [HttpPost]
        public async Task<IActionResult> ProfileEdit(Employee e)
        {
            string EmpToken = HttpContext.Request.Cookies["EmpToken"];
            string EmpExp = HttpContext.Request.Cookies["EmpExpiry"];
            if (Convert.ToDateTime(EmpExp) < DateTime.Now)
            {
                return RedirectToAction("EmployeeLogin", "Employee");
            }
            int id = (int)HttpContext.Session.GetInt32("EmpId");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EmpToken);
                StringContent editemp = new StringContent(JsonConvert.SerializeObject(e), Encoding.UTF8, "application/json");
                var res = await client.PutAsync("https://employeeapi3.azurewebsites.net/api/Employee?id=" + id, editemp);
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction("Profile");
                }
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> EmployeeList()
        {
            string AdminExp = HttpContext.Request.Cookies["AdminExpiry"];
            if (Convert.ToDateTime(AdminExp) < DateTime.Now)
            {
                return RedirectToAction("AdminLogin","Admin");
            }
            List<Employee> e = new List<Employee>();
            string AdminToken = HttpContext.Request.Cookies["AdminToken"];
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AdminToken);
                var res = await client.GetAsync("https://employeeapi3.azurewebsites.net/api/Employee");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    e = JsonConvert.DeserializeObject<List<Employee>>(result);
                }
            }
            return View(e);
        }
        public IActionResult Logout()
        {
            if (Request.Cookies["EmpToken"] != null)
            {
                Response.Cookies.Delete("EmpToken");
                Response.Cookies.Delete("EmpExpiry");
            }
            HttpContext.Session.Clear();
            return RedirectToAction("PlanDetails", "Fiber");
        }
        [HttpGet]
        public IActionResult RegisterEmployee()
        {
            string AdminExp = HttpContext.Request.Cookies["AdminExpiry"];
            if (Convert.ToDateTime(AdminExp) < DateTime.Now)
            {
                return RedirectToAction("AdminLogin", "Admin");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterEmployee(Employee e)
        {
            using (var client = new HttpClient())
            {
                StringContent addemp = new StringContent(JsonConvert.SerializeObject(e), Encoding.UTF8, "application/json");
                var res = await client.PostAsync("https://employeeapi3.azurewebsites.net/api/Employee", addemp);
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction("EmployeeList","Employee");
                }
                return View();
            }
        }
    }
}
