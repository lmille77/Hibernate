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
using System.IO;
using System.Threading.Tasks;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hibernate.Models;

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


        public IActionResult Index()
        {            
            return View();
        }



        public async Task<IActionResult> list()
        {

            var uId = _userManager.GetUserId(User);

            int gId = _db.GroupLeaders.Where(u => u.UserId == uId).Select(i => i.GroupId).FirstOrDefault();

            var pList = _db.Participants.Where(i => i.GroupId == gId).Select(i => i.UserId).ToList();

            //if(pList == null)
            //{
            //    return NotFound();
            //}

            var userList = _db.ApplicationUser.ToList();

            List<ApplicationUser> users = new List<ApplicationUser>();

            foreach (var item in userList)
            {
                foreach (var id in pList)
                {
                    if (id == item.Id && item.isApproved == true)
                    {
                        users.Add(item);
                    }
                }
            }

          

            return View(users);
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
                
                objFromDb.FirstName = user.FirstName;
                objFromDb.LastName = user.LastName;
                _db.SaveChanges();
                TempData[SD.Success] = "User has been edited successfully.";
                return RedirectToAction("list", "Participant");
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

            var groupList = _db.Groups.ToList();
            //List<SelectListItem> groups = new List<SelectListItem>();
            //foreach (var group in groupList)
            //{
            //    SelectListItem li = new SelectListItem
            //    {
            //        Value = group.Name,
            //        Text = group.Name,

            //    };
            //    groups.Add(li);
            //    ViewBag.Users = groups;
            //}

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
                var uId = _userManager.GetUserId(User);
                var groupId = _db.GroupLeaders.Where(u => u.UserId == uId).Select(u => u.GroupId).FirstOrDefault();

                //creates user
                var result = await _userManager.CreateAsync(user, obj.Password);
                //var groupId = _db.Groups.Where(u => u.Name == obj.GroupSelected).Select(e => e.GroupId).FirstOrDefault();

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
                    return RedirectToAction("list", "Participant");

                }
                else
                {
                    ModelState.AddModelError("", "An account with the entered email already exists.");
                }

            }
            return View(obj);
        }


      

        public IActionResult SupList()
        {
          

            var uId = _userManager.GetUserId(User);

            int pId = _db.Participants.Where(i => i.UserId == uId).Select(i => i.ParticipantId).SingleOrDefault();

            var pList = _db.Supporters.Where(i => i.ParticipantId == pId).ToList();

            


            return View(pList);
        }

        public IActionResult participant()
        {
            var participantList = _db.Participants.ToList();

            var OrderList = _db.Order_Items.ToList();

            return View();
        }

    

        public async Task<IActionResult> pending()
        {
            var part_list = _db.Participants.ToList();
            var all_parts = await _userManager.GetUsersInRoleAsync("Participant");
            List<ApplicationUser> unapproved_list = new List<ApplicationUser>();

            var id = _userManager.GetUserId(User);

            var gId = _db.GroupLeaders.Where(u => u.UserId == id).Select(e => e.GroupId).FirstOrDefault();

           
            foreach (var user in all_parts)
            {
                if(user.isApproved != true)
                {
                    user.GroupId = _db.Participants.Where(u => u.UserId == user.Id).Select(e => e.GroupId).FirstOrDefault();
                    user.GroupName = _db.Groups.Where(u => u.GroupId == user.GroupId).Select(e => e.Name).FirstOrDefault();
                }
              

            }


            foreach (var user in all_parts)
            {
                if (user.isApproved == false && user.GroupId == gId)
                {
                    unapproved_list.Add(user);
                }
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
            return RedirectToAction("list", "Participant");
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
            return RedirectToAction("pending", "Participant");
        }
    }
}
