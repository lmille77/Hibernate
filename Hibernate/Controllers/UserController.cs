using Hibernate;
using Hibernate.Data;
using Hibernate.Helpers;
using Hibernate.Models;
using Hibernate.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewSwift.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;

        public UserController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, 
            IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _db = db;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult Index()
        {
            var userList = _db.ApplicationUser.ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            foreach (var user in userList)
            {
                //this will find if there are any roles in the userRole table
                var role = userRole.FirstOrDefault(u => u.UserId == user.Id);
                if (role == null)
                {
                    user.Role = "None";
                }
                else
                {
                    user.Role = roles.FirstOrDefault(u => u.Id == role.RoleId).Name;
                }
            }

            return View(userList);

        }


        public IActionResult Edit(string userId)
        {
            var objFromDb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromDb == null)
            {
                return NotFound();
            }


            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            //this will find if there are any roles assigned to the user
            var role = userRole.FirstOrDefault(u => u.UserId == objFromDb.Id);
            if (role != null)
            {
                objFromDb.RoleId = roles.FirstOrDefault(u => u.Id == role.RoleId).Id;
            }
            objFromDb.RoleList = _db.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = u.Name,
                Value = u.Id
            });

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
                var userRole = _db.UserRoles.FirstOrDefault(u => u.UserId == objFromDb.Id);
                var repId = _db.SalesReps.FirstOrDefault(u => u.UserId == objFromDb.Id);
               
                if (userRole != null)
                {
                    var previousRoleName = _db.Roles.Where(u => u.Id == userRole.RoleId).Select(e => e.Name).FirstOrDefault();


                    //removing the old role
                    await _userManager.RemoveFromRoleAsync(objFromDb, previousRoleName);
                    if(repId != null)
                    {
                        _db.SalesReps.Remove(repId);
                    }

                }

                //add new role
                await _userManager.AddToRoleAsync(objFromDb, _db.Roles.FirstOrDefault(u => u.Id == user.RoleId).Name);
                objFromDb.FirstName = user.FirstName;
                objFromDb.LastName = user.LastName;



                var repRole = _db.Roles.Where(u => u.Name == "Sales Rep").Select(e => e.Id).FirstOrDefault();

                if (user.RoleId == repRole)
                {
                    var id = user.Id;
                    var repToAdd = new SalesRep
                    {
                        UserId = id
                    };

                    _db.SalesReps.Add(repToAdd);
                    
                }
                
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




        [HttpPost]
        public IActionResult LockUnlock(string userId)
        {
            var objFromdb = _db.ApplicationUser.FirstOrDefault(u => u.Id == userId);
            if (objFromdb == null)
            {
                return NotFound();
            }

            if (objFromdb.LockoutEnd != null && objFromdb.LockoutEnd > DateTime.Now)
            {
                //this mean user is locked and will remain lcoked until lockoutend time
                //clickng will unlock user
                objFromdb.LockoutEnd = DateTime.Now;
                TempData[SD.Success] = "User unlocked successfully.";
            }
            else
            {
                //user is not locked and we want to lock the user
                objFromdb.LockoutEnd = DateTime.Now.AddYears(1000);
                TempData[SD.Success] = "User locked successfully.";
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
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
            TempData[SD.Success] = "User deleted succesfully";
            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        public async Task<IActionResult> CreateParticipant()
        {
            var groupList = _db.Groups.ToList();
            List<SelectListItem> groups = new List<SelectListItem>();
            foreach (var group in groupList)
            {
                SelectListItem li = new SelectListItem
                {
                    Value = group.Name,
                    Text = group.Name,

                };
                groups.Add(li);
                ViewBag.Users = groups;
            }
            return View();
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateParticipant(RegisterViewModel obj)
        {

            string _Firstname = obj.FirstName.ToLower();
            string _Lastname = obj.LastName.ToLower();

            var groupList = _db.Groups.ToList();
            List<SelectListItem> groups = new List<SelectListItem>();
            foreach (var group in groupList)
            {
                SelectListItem li = new SelectListItem
                {
                    Value = group.Name,
                    Text = group.Name,

                };
                groups.Add(li);
                ViewBag.Users = groups;
            }

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
                    isApproved = true,
                    PasswordDate = DateTime.Now,

                };
                var id = user.Id;


                //creates user
                var result = await _userManager.CreateAsync(user, obj.Password);
                var groupId = _db.Groups.Where(u => u.Name == obj.GroupSelected).Select(e => e.GroupId).FirstOrDefault();

                if (result.Succeeded)
                {

                    var partToAdd = new Participant
                    {
                        UserId = user.Id,
                        GroupId = groupId
                    };

                    _db.Participants.Add(partToAdd);
                    _db.SaveChanges();


                    await _userManager.AddToRoleAsync(user, "Participant");                 

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackurl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    var Email = obj.Email;
                    var subject = "Account Confirmation";
                    var body = "Please confirm you account by clicking <a href=\"" + callbackurl + "\"> here";
                    var mailHelper = new MailHelper(_configuration);
                    mailHelper.Send(_configuration["Gmail:Username"], Email, subject, body);
                    TempData[SD.Success] = "Account Created";
                    return RedirectToAction("Index", "User");

                }
                else
                {
                    ModelState.AddModelError("", "An account with the entered email already exists.");
                }
                               
            }
            return View(obj);
        }





        [HttpGet]
        public async Task<IActionResult> CreateGL()
        {
            var groupList = _db.Groups.ToList();
            List<SelectListItem> groups = new List<SelectListItem>();
            foreach (var group in groupList)
            {
                SelectListItem li = new SelectListItem
                {
                    Value = group.Name,
                    Text = group.Name,

                };
                groups.Add(li);
                ViewBag.Users = groups;
            }
            return View();
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGL(RegisterViewModel obj)
        {

            string _Firstname = obj.FirstName.ToLower();
            string _Lastname = obj.LastName.ToLower();

            var groupList = _db.Groups.ToList();
            List<SelectListItem> groups = new List<SelectListItem>();
            foreach (var group in groupList)
            {
                SelectListItem li = new SelectListItem
                {
                    Value = group.Name,
                    Text = group.Name,

                };
                groups.Add(li);
                ViewBag.Users = groups;
            }

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
                    isApproved = true,
                    PasswordDate = DateTime.Now,
                    //groupName = obj.GroupSelected

                };
                
                var groupId = _db.Groups.Where(u => u.Name == obj.GroupSelected).Select(e => e.GroupId).FirstOrDefault();

                var id = user.Id;


                //creates user
                var result = await _userManager.CreateAsync(user, obj.Password);
                //creates user
               

                if (result.Succeeded)
                {
                    var GLToAdd = new GroupLeader
                    {
                        UserId = user.Id,
                        GroupId = groupId
                    };

                    _db.GroupLeaders.Add(GLToAdd);
                    _db.SaveChanges();

                    await _userManager.AddToRoleAsync(user, "Group Leader");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackurl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    var Email = obj.Email;
                    var subject = "Account Confirmation";
                    var body = "Please confirm you account by clicking <a href=\"" + callbackurl + "\"> here";
                    var mailHelper = new MailHelper(_configuration);
                    mailHelper.Send(_configuration["Gmail:Username"], Email, subject, body);
                    TempData[SD.Success] = "Account Created";
                    return RedirectToAction("Index", "User");

                }
                else
                {
                    ModelState.AddModelError("", "An account with the entered email already exists.");
                }

            }
            return View(obj);
        }





        [HttpGet]
        public async Task<IActionResult> CreateSR()
        {
            var groupList = _db.Groups.ToList();
            List<SelectListItem> groups = new List<SelectListItem>();
            foreach (var group in groupList)
            {
                SelectListItem li = new SelectListItem
                {
                    Value = group.Name,
                    Text = group.Name,

                };
                groups.Add(li);
                ViewBag.Users = groups;
            }
            return View();
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSR(RegisterViewModel obj)
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
                    isApproved = true,
                    PasswordDate = DateTime.Now,

                };
                var id = user.Id;


                //creates user
                var result = await _userManager.CreateAsync(user, obj.Password);


                if (result.Succeeded)
                {
                    var repToAdd = new SalesRep
                    {
                        UserId = user.Id,
                        Name = obj.FirstName + " " + obj.LastName
                    };

                    _db.SalesReps.Add(repToAdd);
                    _db.SaveChanges();
                    await _userManager.AddToRoleAsync(user, "Sales Rep");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackurl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    var Email = obj.Email;
                    var subject = "Account Confirmation";
                    var body = "Please confirm you account by clicking <a href=\"" + callbackurl + "\"> here";
                    var mailHelper = new MailHelper(_configuration);
                    mailHelper.Send(_configuration["Gmail:Username"], Email, subject, body);
                    TempData[SD.Success] = "Account Created";
                    return RedirectToAction("Index", "User");

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

