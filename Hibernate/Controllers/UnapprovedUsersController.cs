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
    public class UnapprovedUsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;


        public UnapprovedUsersController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var part_list = _db.Participants.ToList();
            var all_users = await _userManager.GetUsersInRoleAsync("Participant");
            List<ApplicationUser> unapproved_list = new List<ApplicationUser>();

            foreach(var user in all_users)
            {
                if(user.isApproved == false)
                {
                    unapproved_list.Add(user);
                }
            }


            foreach(var user in unapproved_list)
            {
                user.GroupId = _db.Participants.Where(u => u.UserId == user.Id).Select(e => e.GroupId).FirstOrDefault();
                user.GroupName = _db.Groups.Where(u => u.GroupId == user.GroupId).Select(e => e.Name).FirstOrDefault();

            }
            return View(unapproved_list);
        }


        [HttpPost]
        public IActionResult Delete(string userId)
        {
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }
            _db.ApplicationUser.Remove(objFromDb);
            _db.SaveChanges();
            TempData[SD.Success] = "User rejected succesfully";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult Approve(string userId)
        {
            var objFromdb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromdb == null)
            {
                return NotFound();
            }

            if (objFromdb.isApproved == false)
            {
                //sends an email to admin requesting approval for new user
                var email = objFromdb.Email;
                var subject = "Accepted";
                var body = "<a href='https://localhost:44316/Account/Login'>Click here to sign in </a>";
                var mailHelper = new MailHelper(_configuration);
                mailHelper.Send(_configuration["Gmail:Username"], email, subject, body);
                objFromdb.isApproved = true;
                TempData[SD.Success] = "User approved successfully.";
            }


            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult EditRole(string userId)
        {
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }

            return View(objFromDb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == user.Id);
                if (objFromDb == null)
                {
                    return NotFound();
                }
                var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == objFromDb.Id);
                if (userRole != null)
                {
                    var previousRoleName = _db.Roles.Where(u => u.Id == userRole.RoleId).Select(e => e.Name).FirstOrDefault();
                    //removing the old role
                    await _userManager.RemoveFromRoleAsync(objFromDb, previousRoleName);

                }
                objFromDb.FirstName = user.FirstName;
                objFromDb.LastName = user.LastName;                
                //add new role
                await _userManager.AddToRoleAsync(objFromDb, user.RoleId);
                _db.SaveChanges();
                TempData[SD.Success] = "User has been edited successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
    }
}
