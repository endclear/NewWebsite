using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestJWT.Models;
using TestJWT.Services;
using Microsoft.AspNetCore.Session;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace TestJWT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILoginRequest _ILoginRequest;
        public HomeController(ILogger<HomeController> logger, ILoginRequest IloginRequest)
        {
            _logger = logger;
            _ILoginRequest = IloginRequest;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(User user)
        {
            try
            {
                var result = await _ILoginRequest.Login(user, "https://localhost:44306/api/Token", "");
                HttpContext.Session.SetString(gettoken.token, result.token);
                return RedirectToAction("product");
            }
            catch (Exception)
            {
                ViewBag.status = "Tên tài khoản hoặc mật khẩu không chính xác";
                return View();
            }
            
        }
        public async Task<IActionResult> product()
        {
            var product = await _ILoginRequest.GetProducts("https://localhost:44306/api/products", getsringtoken());
            return View(product);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public string getsringtoken()
        {
            try
            {
                string token = HttpContext.Session.GetString(gettoken.token);
                return token;
            }
            catch
            {
                return "";
            }
        }
    }
}
