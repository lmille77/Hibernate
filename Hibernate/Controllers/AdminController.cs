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

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
            {
             
                var gList = _db.Groups.ToList();

                var orders = _db.Orders.ToList();

                var orderItems = _db.Order_Items.ToList();

                double t = 0;
                int i = 0;


                foreach (var groups in gList)
                {
                    foreach (var item in orders)
                    {
                        if (item.GroupId == groups.GroupId)
                        {
                            groups.Total += item.Total;
                            t += item.Total;
                            groups.OrderId[i] = item.OrderId;
                            i++;
                        }
                    }
                }


                foreach (var group in gList)
                {
                    foreach (var item in orderItems)
                    {
                        for (int j = 0; j < group.OrderId.Length; j++)
                        {
                            if (group.OrderId[j] == item.OrderId)
                            {
                                if (item.ItemId == 1)
                                {
                                    group.BedSheets++;
                                }

                                if (item.ItemId == 2)
                                {
                                    group.PillowCases++;
                                }

                            }
                        }

                    }
                }

                Groups G = new Groups
                {
                    Gs = gList,
                    GTotal = t
                };



                return View(G);

            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Sales Rep"))
            {
                return RedirectToAction("Index", "SalesRep");
            }
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Group Leader"))
            {
                return RedirectToAction("Index", "GroupLeader");
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


    }
}
