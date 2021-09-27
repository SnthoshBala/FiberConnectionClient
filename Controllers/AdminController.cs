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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FiberConnectionClient.Controllers
{
    public class AdminController : Controller
    {
        string AdminToken = "";

        IConfiguration _config;

        private IJsonSerializer _serializer = new JsonNetSerializer();
        private IDateTimeProvider _provider = new UtcDateTimeProvider();
        private IBase64UrlEncoder _urlEncoder = new JwtBase64UrlEncoder();
        private IJwtAlgorithm _algorithm = new HMACSHA256Algorithm();
        public AdminController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        public IActionResult AdminControl()
        {
            string AdminToken = HttpContext.Request.Cookies["AdminToken"];
            if (string.IsNullOrEmpty(AdminToken))
            {
                return RedirectToAction("AdminLogin");
            }
            return View();
        }
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AdminLogin(Admin a)
        {
            using (var client = new HttpClient())
            {
                StringContent a_content = new StringContent(JsonConvert.SerializeObject(a), Encoding.UTF8, "application/json");
                var response = client.PostAsync("https://localhost:44378/api/Authorization/AdminLogin", a_content).Result;
               if (response.IsSuccessStatusCode)
                {
                        AdminToken = response.Content.ReadAsStringAsync().Result;
                        HttpContext.Response.Cookies.Append("AdminToken", AdminToken);

                        IJwtValidator _validator = new JwtValidator(_serializer, _provider);
                        IJwtDecoder decoder = new JwtDecoder(_serializer, _validator, _urlEncoder, _algorithm);
                        var tokenExp = decoder.DecodeToObject<JwtTokenExp>(AdminToken);
                        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(tokenExp.exp);
                        DateTime timeExp = dateTimeOffset.LocalDateTime;

                        HttpContext.Response.Cookies.Append("Expiry", timeExp.ToString());
                        return RedirectToAction("AdminControl");
                    }
                }
                return View();
            }
        public IActionResult Logout()
        {
            if (Request.Cookies["AdminToken"] != null)
            {
                Response.Cookies.Delete("AdminToken");
            }
            return RedirectToAction("AdminControl");
        }
        }
    }
