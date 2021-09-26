using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        public IActionResult Index()
        {
            return View();
        }
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
                if(response.IsSuccessStatusCode)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        Token = response.Content.ReadAsStringAsync().Result;
                        HttpContext.Response.Cookies.Append("Token", Token);

                        string c1 = response1.Content.ReadAsStringAsync().Result;
                        Customer CustomerDetails = JsonConvert.DeserializeObject<Customer>(c1);
                        
                        HttpContext.Session.SetString("Username",CustomerDetails.CustomerName);
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

    }
}
