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
        public async Task<IActionResult> CustomerLogin(Customer c)
        {
            using (var client = new HttpClient())
            {
                StringContent c_content = new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json");
                var response =await client.PostAsync("https://authorizationapiteam3.azurewebsites.net/api/Authorization/CustomerLogin", c_content);
                var response1 =await client.PostAsync("https://customerapiteam3.azurewebsites.net/api/Customer/Login", c_content);
                if (response.IsSuccessStatusCode)
                {
                    if (response1.IsSuccessStatusCode)
                    {
                        Token = response.Content.ReadAsStringAsync().Result;
                        HttpContext.Response.Cookies.Append("Token", Token);

                        string c1 = response1.Content.ReadAsStringAsync().Result;
                        Customer CustomerDetails = JsonConvert.DeserializeObject<Customer>(c1);

                        HttpContext.Response.Cookies.Append("Username", CustomerDetails.CustomerName);
                        HttpContext.Response.Cookies.Append("UserId", CustomerDetails.CustomerId.ToString());

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
            string Token = HttpContext.Request.Cookies["Token"];
            string Exp = HttpContext.Request.Cookies["Expiry"];
            if (Convert.ToDateTime(Exp) < DateTime.Now)
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            Customer c = new Customer();
            int id =Convert.ToInt32( HttpContext.Request.Cookies["UserId"]);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var res = await client.GetAsync("https://customerapiteam3.azurewebsites.net/api/Customer/" + id);
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
            string Token = HttpContext.Request.Cookies["Token"];
            string Exp = HttpContext.Request.Cookies["Expiry"];
            if (Convert.ToDateTime(Exp) < DateTime.Now)
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            Customer c = new Customer();
            int id = Convert.ToInt32(HttpContext.Request.Cookies["UserId"]);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var res = await client.GetAsync("https://customerapiteam3.azurewebsites.net/api/Customer/" + id);
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
            string Token = HttpContext.Request.Cookies["Token"];
            string Exp = HttpContext.Request.Cookies["Expiry"];
            if (Convert.ToDateTime(Exp) < DateTime.Now)
            {
                return RedirectToAction("CustomerLogin", "Customer");
            }
            int id = Convert.ToInt32(HttpContext.Request.Cookies["UserId"]);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                StringContent editcon = new StringContent(JsonConvert.SerializeObject(c),Encoding.UTF8,"application/json");
                var res =await client.PutAsync("https://customerapiteam3.azurewebsites.net/api/Customer?id=" + id,editcon);
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
            string AdminExp = HttpContext.Request.Cookies["AdminExpiry"];
            if (Convert.ToDateTime(AdminExp) < DateTime.Now)
            {
                return RedirectToAction("AdminLogin","Admin");
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var res = await client.GetAsync("https://customerapiteam3.azurewebsites.net/api/Customer");
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
                Response.Cookies.Delete("Expiry");
            }
            HttpContext.Session.Clear();
            return RedirectToAction("PlanDetails","Fiber");
        }
        [HttpGet]
        public IActionResult RegisterCustomer()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterCustomer(Customer c)
        {
            using (var client = new HttpClient())
            {
                StringContent addcus = new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json");
                var res = await client.PostAsync("https://customerapiteam3.azurewebsites.net/api/Customer", addcus);
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction("Profile");
                }
                return View();
            }
        }
    }
}
