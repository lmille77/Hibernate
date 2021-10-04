using Hibernate.Data;
using Hibernate.Helpers;
using Hibernate.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibernate.Controllers
{
    public class AdminController : Controller
    {
        //variables used to host email service
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;


        private readonly ApplicationDbContext _db;

        //variables used to implement management of the user
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
         SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
         IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
            {
                return View();
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Sales Rep"))
            {
                return RedirectToAction("Index", "SalesRep");
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Group Leader"))
            {
                return RedirectToAction("Index", "Group");
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Participant"))
            {
                return RedirectToAction("Index", "Participant");
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Unapproved"))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

        }
        //Action for Sending Admin Messages
        [HttpGet]
        public IActionResult Send()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Send(Message obj)
        {
            var toEmail = obj.ToEmail;
            var subject = obj.Subject;
            var body = obj.Body;
            var mailHelper = new MailHelper(_configuration);
            mailHelper.Send(_configuration["Gmail:Username"], toEmail, subject, body);
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public IActionResult GoBack()
        {
            return RedirectToAction("Index", "Admin");

        }

        //public IActionResult ExpiredPass()
        //{

        //  var exp_pass = _db.Users.FirstOrDefault(u => u.LastPass1 != null);


        //    return View(exp_pass);



        //}


    }
}
