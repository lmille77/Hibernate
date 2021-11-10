using Hibernate.Data;
using Hibernate.Models;
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
    public class SalesRepController : Controller
    {
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;

        private readonly ApplicationDbContext _db;

        //variables used to implement management of the user
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public SalesRepController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
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
            //var gList = _db.Groups.ToList();

            var id = _userManager.GetUserId(User);

            var sId = _db.SalesReps.Where(u => u.UserId == id).Select(u => u.SalesRepId).FirstOrDefault();

            var gList = _db.Groups.Where(u => u.SalesRepId == sId).ToList();

            var orders = _db.Orders.ToList();

            var orderItems = _db.Order_Items.ToList();


            int i = 0;


            foreach (var groups in gList)
            {
                foreach (var item in orders)
                {
                    if (item.GroupId == groups.GroupId)
                    {
                        groups.Total += item.Total;
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



            return View(gList);
        }


     
    }
}
