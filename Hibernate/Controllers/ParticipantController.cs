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
    public class ParticipantController : Controller
    {
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;


        private readonly ApplicationDbContext _db;

        //variables used to implement management of the user
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public ParticipantController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
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
            
            var list = await _userManager.GetUsersInRoleAsync("Participant");
            
           
            //foreach (var user in list)
            //{
            //    //this will find if there are any roles in the userRole table
            //    var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
            //    if (role == null)
            //    {
            //        user.Role = "None";
            //    }
            //    else
            //    {
            //        user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
            //    }
            //}

            return View(list);
        }


        public IActionResult Edit(string userId)
        {
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }


            //var userRole = _db.UserRoles.ToList();
            //var roles = _db.Roles.ToList();

            //this will find if there are any roles assigned to the user
            //var role = userRole.FirstOrDefault(u => u.UserId == objFromDb.Id);
            //if (role != null)
            //{
            //    objFromDb.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId).Id;
            //}
            //objFromDb.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            //{
            //    Text = u.Name,
            //    Value = u.Id
            //});

            return View(objFromDb);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == user.Id);
                if (objFromDb == null)
                {
                    return NotFound();
                }
                //var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == objFromDb.Id);
                //if (userRole != null)
                //{
                //    var previousRoleName = _db.Roles.Where(u => u.Id == userRole.RoleId).Select(e => e.Name).FirstOrDefault();
                //    //removing the old role
                //    await _userManager.RemoveFromRoleAsync(objFromDb, previousRoleName);

                //}

                ////add new role
                //await _userManager.AddToRoleAsync(objFromDb, _db.Roles.FirstOrDefault(u => u.Id == user.RoleId).Name);
                objFromDb.CustomUsername = user.CustomUsername;
                objFromDb.FirstName = user.FirstName;
                objFromDb.LastName = user.LastName;
                objFromDb.DOB = user.DOB;
                objFromDb.Address = user.Address;
                _db.SaveChanges();
                TempData[SD.Success] = "User has been edited successfully.";
                return RedirectToAction(nameof(Index));
            }


            user.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });
            return View(user);
        }

        [HttpGet]
        public IActionResult Upsert(string id)
        {


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(RegisterViewModel obj)
        {

            string _Firstname = obj.FirstName.ToLower();
            string _Lastname = obj.LastName.ToLower();



            if (ModelState.IsValid)
            {
                //object created by user input
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = obj.Email,
                    CustomUsername = _Firstname[0] + _Lastname + DateTime.Now.ToString("yyMM"),
                    FirstName = obj.FirstName,
                    LastName = obj.LastName,
                    Email = obj.Email,
                    isApproved = false,
                    DOB = obj.DOB,
                    Address = obj.Address,
                    PasswordDate = DateTime.Now
                };



                //creates user
                var result = await _userManager.CreateAsync(user, obj.Password);


                if (result.Succeeded)
                {

                    //if (obj.RoleSelected != null && obj.RoleSelected.Length > 0 && obj.RoleSelected == "Admin")
                    //{
                    //    await _userManager.AddToRoleAsync(user, "Admin");
                    //}
                    //if (obj.RoleSelected != null && obj.RoleSelected.Length > 0 && obj.RoleSelected == "Sales Rep")
                    //{
                    //    await _userManager.AddToRoleAsync(user, "Sales Rep");
                    //}

                    //if (obj.RoleSelected != null && obj.RoleSelected.Length > 0 && obj.RoleSelected == "Group Leader")
                    //{
                    //    await _userManager.AddToRoleAsync(user, "Group Leader");
                    //}

                    //if (obj.RoleSelected != null && obj.RoleSelected.Length > 0 && obj.RoleSelected == "Participant")
                    //{
                    //    await _userManager.AddToRoleAsync(user, "Admin");
                    //}


                    await _userManager.AddToRoleAsync(user, "Participant");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);


                    var callbackurl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                    var Email = obj.Email;
                    var subject = "Account Confirmation";
                    var body = "Please confirm you account by clicking <a href=\"" + callbackurl + "\"> here";
                    var mailHelper = new MailHelper(_configuration);
                    mailHelper.Send(_configuration["Gmail:Username"], Email, subject, body);

                    TempData[SD.Success] = "Account Created";
                    return RedirectToAction("Index", "Participant");

                }
                else
                {
                    ModelState.AddModelError("", "An account with the entered email already exists.");
                }


            }
            return View(obj);
        }



    }
}
