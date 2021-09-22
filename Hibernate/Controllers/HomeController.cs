using Hibernate.Models;
using Hibernate.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
         SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
            {

                return RedirectToAction("Index", "Admin");
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Group"))
            {
                return RedirectToAction("Index", "Group");
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Participant"))
            {
                return RedirectToAction("Index", "Participant");
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Unapproved"))
            {                
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
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
    }
}
