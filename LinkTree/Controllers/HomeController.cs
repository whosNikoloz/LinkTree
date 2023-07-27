using LinkTree.Models;
using LinkTree.Models.UserDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace LinkTree.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Main()
        {
            if (_httpContextAccessor.HttpContext.Session.GetString("User") != null)
            {
                var userJson = _httpContextAccessor.HttpContext.Session.GetString("User");

                var user = JsonConvert.DeserializeObject<User>(userJson);

                return View(user);
            }
            else
            {
                return RedirectToAction("Authentication", "Account");
            }

        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}