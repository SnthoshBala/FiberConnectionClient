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
    public class CustomerController : Controller
    {
        string Token = "";

        IConfiguration _config;

        private IJsonSerializer _serializer = new JsonNetSerializer();
        private IDateTimeProvider _provider = new UtcDateTimeProvider();
        private IBase64UrlEncoder _urlEncoder = new JwtBase64UrlEncoder();
        private IJwtAlgorithm _algorithm = new HMACSHA256Algorithm();
        public CustomerController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        public IActionResult CustomerLogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CustomerLogin(Customer c)
        {
            using (var client = new HttpClient())
            {
                StringContent c_content = new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json");
                var response = client.PostAsync("https://localhost:44378/api/Authorization/CustomerLogin", c_content).Result;
                var response1 = client.PostAsync("https://localhost:44320/api/Customer/Login", c_content).Result;
                if (response.IsSuccessStatusCode)
                {
                    if (response1.IsSuccessStatusCode)
                    {
                        Token = response.Content.ReadAsStringAsync().Result;
                        HttpContext.Response.Cookies.Append("Token", Token);

                        string c1 = response1.Content.ReadAsStringAsync().Result;
                        Customer CustomerDetails = JsonConvert.DeserializeObject<Customer>(c1);

                        HttpContext.Session.SetString("Username", CustomerDetails.CustomerName);
                        HttpContext.Session.SetInt32("UserId", CustomerDetails.CustomerId);

                        IJwtValidator _validator = new JwtValidator(_serializer, _provider);
                        IJwtDecoder decoder = new JwtDecoder(_serializer, _validator, _urlEncoder, _algorithm);
                        var tokenExp = decoder.DecodeToObject<JwtTokenExp>(Token);
                        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(tokenExp.exp);
                        DateTime timeExp = dateTimeOffset.LocalDateTime;

                        HttpContext.Response.Cookies.Append("Expiry", timeExp.ToString());
                        return RedirectToAction("Index", "Home");
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
            Customer c = new Customer();
            int id = (int)HttpContext.Session.GetInt32("UserId");
            string Token = HttpContext.Request.Cookies["Token"];
            if (string.IsNullOrEmpty(Token))
            {
                return RedirectToAction("Login", "Login");
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var res = await client.GetAsync("https://localhost:44320/api/Customer/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    c = JsonConvert.DeserializeObject<Customer>(result);

                }
            }
            return View(c);
        }
        [HttpGet]
        public async Task<IActionResult> ProfileEdit()
        {
            Customer c = new Customer();
            int id = (int)HttpContext.Session.GetInt32("UserId");
            string Token = HttpContext.Request.Cookies["Token"];
            if (string.IsNullOrEmpty(Token))
            {
                return RedirectToAction("CustomerLogin");
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var res = await client.GetAsync("https://localhost:44320/api/Customer/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    c = JsonConvert.DeserializeObject<Customer>(result);

                }
            }
            return View(c);
        }
        [HttpPost]
        public async Task<IActionResult> ProfileEdit(Customer c)
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            string Token = HttpContext.Request.Cookies["Token"];
            if (string.IsNullOrEmpty(Token))
            {
                return RedirectToAction("CustomerLogin");
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                StringContent editcon = new StringContent(JsonConvert.SerializeObject(c),Encoding.UTF8,"application/json");
                var res =await client.PutAsync("https://localhost:44320/api/Customer?id="+id,editcon);
                if(res.IsSuccessStatusCode)
                {
                    return RedirectToAction("Profile");
                }
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> CustomerList()
        {
            List<Customer> c = new List<Customer>();
            string Token = HttpContext.Request.Cookies["Token"];
            if (string.IsNullOrEmpty(Token))
            {
                return RedirectToAction("AdminLogin", "Admin");
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var res = await client.GetAsync("https://localhost:44320/api/Customer");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    c = JsonConvert.DeserializeObject<List<Customer>>(result);
                }
            }
            return View(c);
        }
        public IActionResult Logout()
        {
            if (Request.Cookies["Token"] != null)
            {
                Response.Cookies.Delete("Token");
            }
            return View("Plan","FiberPlan");
        }
    }
}
