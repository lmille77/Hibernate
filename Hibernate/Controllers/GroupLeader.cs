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
    public class GroupLeader : Controller
    {

        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;


        private readonly ApplicationDbContext _db;

        //variables used to implement management of the user
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public GroupLeader(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
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
            var pList = _db.Participants.ToList();

            var uId = _userManager.GetUserId(User);

            var gId = _db.GroupLeaders.Where(u => u.UserId == uId).Select(u => u.GroupId).FirstOrDefault();

            var orders = _db.Orders.ToList();

            var orderItems = _db.Order_Items.ToList();

            List<Participant> participants = new List<Participant>();

            var pUsers = _db.ApplicationUser.ToList();
           double gtotal = 0;

            int i = 0;
 

            foreach(var item in pList)
            {
                if(item.GroupId == gId)
                {
                    participants.Add(item);
                }
            }


            foreach (var item in participants)
            {
                foreach (var parts in pUsers)
                {
                    if (item.UserId == parts.Id)
                    {
                        item.Name = parts.FirstName + " " + parts.LastName;
                    }
                }
            }



            foreach (var parts in participants)
            {
                foreach (var item in orders)
                {
                    if (item.ParticipantId == parts.ParticipantId)
                    {
                        parts.Total += item.Total;
                        gtotal += item.Total;
                        parts.OrderId[i] = item.OrderId;
                        i++;
                    }
                }
            }


           


            foreach (var parts in participants)
            {
                foreach (var item in orderItems)
                {               
                    for(int j = 0; j < parts.OrderId.Length; j++)
                    {
                        if (parts.OrderId[j] == item.OrderId)
                        {
                            if (item.ItemId == 1)
                            {
                                parts.BedSheets++;
                            }

                            if (item.ItemId == 2)
                            {
                                parts.PillowCases++;
                            }
                        }
                    }
                       
                }
            }

            Parts partsView = new Parts
            {
                Participants = participants,
                GTotal = gtotal,
                GId = gId
            };


            return View(partsView);
        }
    }
}
