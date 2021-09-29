using Hibernate.Data;
using Hibernate.Helpers;
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
                await _roleManager.CreateAsync(new IdentityRole("Group"));
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
                        TempData[SD.Error] = "Your password will expire in three days.";
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (curr_user.isApproved == true && group_role_list.Contains(curr_user))
                    {
                        TempData[SD.Error] = "Your password will expire in three days.";
                        return RedirectToAction("Index", "Group");
                    }
                    else if (curr_user.isApproved == true && participant_role_list.Contains(curr_user))
                    {
                        TempData[SD.Error] = "Your password will expire in three days.";
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
            ////if the user roles are not already stored in the database, then they are added            
            //if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
            //{
            //    return RedirectToAction("Index", "Admin");
            //}
            //else if (_signInManager.IsSignedIn(User) && User.IsInRole("Group"))
            //{
            //    return RedirectToAction("Index", "Group");
            //}
            //else if (_signInManager.IsSignedIn(User) && User.IsInRole("Participant"))
            //{
            //    return RedirectToAction("Index", "Participant");
            //}
            //else if (_signInManager.IsSignedIn(User) && User.IsInRole("Unapproved"))
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem()
            {
                Value = "Admin",
                Text = "Admin"
            });
            listItems.Add(new SelectListItem()
            {
                Value = "Sales Rep",
                Text = "Sales Rep"
            });

            listItems.Add(new SelectListItem()
            {
                Value = "Group Leader",
                Text = "Group Leader"
            });

            listItems.Add(new SelectListItem()
            {
                Value = "Participant",
                Text = "Participant"
            });



            ViewData["ReturnUrl"] = returnurl;
            RegisterViewModel registerViewModel = new RegisterViewModel()
            {
                RoleList = listItems
            };

            return View(registerViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel obj)
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

                    if (obj.RoleSelected != null && obj.RoleSelected.Length > 0 && obj.RoleSelected == "Admin")
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    if (obj.RoleSelected != null && obj.RoleSelected.Length > 0 && obj.RoleSelected == "Sales Rep")
                    {
                        await _userManager.AddToRoleAsync(user, "Sales Rep");
                    }

                    if (obj.RoleSelected != null && obj.RoleSelected.Length > 0 && obj.RoleSelected == "Group Leader")
                    {
                        await _userManager.AddToRoleAsync(user, "Group Leader");
                    }

                    if (obj.RoleSelected != null && obj.RoleSelected.Length > 0 && obj.RoleSelected == "Participant")
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                   
                    
                    var callbackurl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                    var Email = obj.Email;
                    var subject = "Account Confirmation";
                    var body = "Please confirm you account by clicking <a href=\"" + callbackurl + "\"> here";
                    var mailHelper = new MailHelper(_configuration);
                    mailHelper.Send(_configuration["Gmail:Username"], Email, subject, body);

                    TempData[SD.Success] = "Account Created";
                    return RedirectToAction("Index", "Admin");

                }
                else
                {
                    ModelState.AddModelError("", "An account with the entered email already exists.");
                }


                ////finds  all admin user
                //var admin_users = await _userManager.GetUsersInRoleAsync("Admin");
                //var admin_email = "";

                    //foreach (ApplicationUser admin_user in admin_users)
                    //{
                    //    if (admin_user.isApproved == true)
                    //    {
                    //        admin_email = admin_user.Email;
                    //        break;
                    //    }
                    //}

                    //if (result.Succeeded)
                    //{
                    //    //sends an email to admin requesting approval for new user
                    //    var subject = "Add new user";
                    //    var body = "<a href='https://localhost:44316/Account/Login'>Click to Add User </a>";
                    //    var mailHelper = new MailHelper(_configuration);
                    //    mailHelper.Send(_configuration["Gmail:Username"], admin_email, subject, body);

                    //    //adds user to database but without admin approval
                    //    await _userManager.AddToRoleAsync(user, "Unapproved");
                    //    await _signInManager.SignInAsync(user, isPersistent: false);
                    //    return RedirectToAction("Index", "Home");
                //    //}
                //else
                //{
                //    ModelState.AddModelError("", "An account with the entered email already exists.");
                //}
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
