using Hibernate.Data;
using Hibernate.Helpers;
using Hibernate.Models;
using Hibernate.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Hibernate.Controllers
{
    public class AccountController : Controller
    {
        //variables used to host email service
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;


        private readonly ApplicationDbContext _db;

        //variables used to implement management of the user
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public AccountController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
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

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("Sales Rep"));
                await _roleManager.CreateAsync(new IdentityRole("Group Leader"));
                await _roleManager.CreateAsync(new IdentityRole("Participant"));
                await _roleManager.CreateAsync(new IdentityRole("Unapproved"));
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = "miller4277@gmail.com",
                    CustomUsername = "Admin1",
                    FirstName = "Admin",
                    Email = "miller4277@gmail.com",
                    isApproved = true
                };
                var result = await _userManager.CreateAsync(user, "Admin123!");
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
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
            }
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel obj)
        {
            if (ModelState.IsValid)
            {
                //checks database to see if user and password are correct
                var result = await _signInManager.PasswordSignInAsync(obj.Email, obj.Password, false, lockoutOnFailure: true);
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                if (result.Succeeded)
                {
                    //checks to see if a user has been approved by an admin and redirects accordingly
                    var curr_user = await _userManager.FindByNameAsync(obj.Email);
                    var admin_role_list = await _userManager.GetUsersInRoleAsync("Admin");
                    var group_role_list = await _userManager.GetUsersInRoleAsync("Group");
                    var participant_role_list = await _userManager.GetUsersInRoleAsync("Participant");

                    if (curr_user.isApproved == true && admin_role_list.Contains(curr_user))
                    {
                       
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (curr_user.isApproved == true && group_role_list.Contains(curr_user))
                    {
                        
                        return RedirectToAction("Index", "Group");
                    }
                    else if (curr_user.isApproved == true && participant_role_list.Contains(curr_user))
                    {
                       
                        return RedirectToAction("Index", "Participant");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Invalid Email or Password.");
                }
            }
            return View(obj);
        }

        [HttpGet]
        public async Task<IActionResult> Register(string returnurl = null)
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
        public async Task<IActionResult> Register(RegisterViewModel obj)
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
                    isApproved = false,
                    PasswordDate = DateTime.Now,
                    groupName = obj.GroupSelected
                    
                    
                };
                var id = user.Id;
                

                //creates user
                var result = await _userManager.CreateAsync(user, obj.Password);


                if (result.Succeeded)
                {

                    
                    await _userManager.AddToRoleAsync(user, "Participant");                  

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                   
                    
                    var callbackurl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                    var Email = obj.Email;
                    var subject = "Account Confirmation";
                    var body = "Please confirm you account by clicking <a href=\"" + callbackurl + "\"> here";
                    var mailHelper = new MailHelper(_configuration);
                    mailHelper.Send(_configuration["Gmail:Username"], Email, subject, body);

                    TempData[SD.Success] = "Account Created. Awaiting approval.";
                    return RedirectToAction("Login", "Account");

                }
                else
                {
                    ModelState.AddModelError("", "An account with the entered email already exists.");
                }                
            }
            return View(obj);
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");

        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logoff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        //Password Reset Action

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(Message obj)
        {
            var toEmail = obj.ToEmail;
            var subject = "Password Reset Confirmation";
            var body = "Please click the link to reset your password. https://localhost:44316/Account/ConfirmResetPassword";
            var mailHelper = new MailHelper(_configuration);
            mailHelper.Send(_configuration["Gmail:Username"], toEmail, subject, body);
            return RedirectToAction("Index", "Admin");

        }
        //Password Reset Confirmation Action
        [HttpGet]
        public IActionResult ConfirmResetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmResetPassword(PasswdReset obj)
        {
            if (ModelState.IsValid)
            {
                var curr_user = await _userManager.FindByNameAsync(obj.Email);

                bool passcheck = false;


                var result = await _userManager.CheckPasswordAsync(curr_user, obj.NewPass);

                PasswordVerificationResult passMatch = PasswordVerificationResult.Failed;
                PasswordVerificationResult passMatch2 = PasswordVerificationResult.Failed;

                if (curr_user.LastPass1 != null)
                {
                    passMatch = _userManager.PasswordHasher.VerifyHashedPassword(curr_user, curr_user.LastPass1, obj.NewPass);
                }

                if (curr_user.LastPass2 != null)
                {
                    passMatch2 = _userManager.PasswordHasher.VerifyHashedPassword(curr_user, curr_user.LastPass2, obj.NewPass);
                }

                if (result == true)
                {
                    ViewBag.ErrorMessage = "You cannot use your last three passwords.";
                }

                else if (passMatch == PasswordVerificationResult.Success)
                {
                    ViewBag.ErrorMessage = "You cannot use your last three passwords.";
                }

                else if (passMatch2 == PasswordVerificationResult.Success)
                {
                    ViewBag.ErrorMessage = "You cannot use your last three passwords.";
                }

                else
                    passcheck = true;

                if (passcheck == true)
                {

                    curr_user.LastPass2 = curr_user.LastPass1;
                    curr_user.LastPass1 = curr_user.PasswordHash;
                    await _userManager.UpdateAsync(curr_user);

                    await _userManager.RemovePasswordAsync(curr_user);
                    await _userManager.AddPasswordAsync(curr_user, obj.NewPass);


                    return RedirectToAction("Login", "Account");
                }
            }
            return View(obj);
        }
    }
}
