using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hibernate.Data;
using Hibernate.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Hibernate.Models.ViewModels;
using Hibernate.Helpers;

namespace Hibernate.Controllers
{
    public class GroupsController : Controller
    {

        private readonly ApplicationDbContext _db;
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;


        private readonly ApplicationDbContext _context;

        //variables used to implement management of the user
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;

        public GroupsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
         SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager,
         IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }



        // GET: Groups
        public async Task<IActionResult> SRIndex()
        {
            var id = _userManager.GetUserId(User);
            var gList = _context.Groups.ToList();
            var sId = _context.SalesReps.Where(u => u.UserId == id).Select(u => u.SalesRepId).FirstOrDefault();

            List<Group> groups = new List<Group>();
            
            foreach(var item in gList)
            {
                if(item.SalesRepId == sId)
                {
                    groups.Add(item);
                }
            }
            foreach(var item in groups)
            {
                var userid = _context.GroupLeaders.Where(u => u.GroupId == item.GroupId).Select(e => e.UserId).FirstOrDefault();
                item.GroupLeader = _context.ApplicationUser.Where(u => u.Id == userid).Select(e => e.FirstName + " " + e.LastName).FirstOrDefault();
            }
            return View(groups);
        }
        
        
        public IActionResult AdminIndex()
        {
            var userList = _context.ApplicationUser.ToList();

            var gList = _context.Groups.ToList();

            var sList = _context.SalesReps.ToList();

           // var userId = _context.SalesReps.Where(i => i.UserId == id).Select(i => i.SalesRepId).SingleOrDefault();
           foreach(var item in gList)
            {
                //needs to be adjusted. Probably make name a column, instead of NotMapped
                item.SalesRepName = sList.Where(i => i.SalesRepId == item.SalesRepId).Select(i => i.Name).SingleOrDefault();

                if (item.SalesRepName == null)
                {
                    item.SalesRepName = "None";
                }
                var userid = _context.GroupLeaders.Where(u => u.GroupId == item.GroupId).Select(e => e.UserId).FirstOrDefault();
                item.GroupLeader = _context.ApplicationUser.Where(u => u.Id == userid).Select(e => e.FirstName + " " + e.LastName).FirstOrDefault();
           }
            return View(gList);


        }



        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // GET: Groups/Create
        [HttpGet]
        public async Task<IActionResult> SRCreate()
        {
            return View();
        }

        //[Bind("Id,Name,Address,City,State")] Group @group
        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SRCreate(Group obj)
        {
            if (ModelState.IsValid)
            {                
                var id = _userManager.GetUserId(User);
                //search salerep table 
               
                int repId = _context.SalesReps.Where(i => i.UserId == id).Select(i => i.SalesRepId).SingleOrDefault();

                var groupToAdd = new Group
                {
                    SalesRepId = repId,
                    Name = obj.Name,
                    Address = obj.Address,
                    City = obj.City,
                    State = obj.State

                    
                };
                _context.Add(groupToAdd);

                //_context.Add(@group);
                await _context.SaveChangesAsync();
                return RedirectToAction("SRIndex","Groups");
            }
            return View(obj);
        }

        [HttpGet]
        public async Task<IActionResult> AdminCreate()
        {
            var userList = _context.ApplicationUser.ToList();
            var srList = _context.SalesReps.ToList();
            List<SelectListItem> salesreps = new List<SelectListItem>();
            foreach (var sr in srList)
            {
                foreach(var user in userList)
                {
                    if(user.Id == sr.UserId)
                    {
                        SelectListItem li = new SelectListItem
                        {
                            Value = sr.UserId,
                            Text = user.FirstName + " " + user.LastName,

                        };
                        salesreps.Add(li);
                        ViewBag.Users = salesreps;
                    }
                    
                }                
                
            }
            return View();
        }

        //[Bind("Id,Name,Address,City,State")] Group @group
        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminCreate(Group obj)
        {
            var userList = _context.ApplicationUser.ToList();
            var srList = _context.SalesReps.ToList();
            List<SelectListItem> salesreps = new List<SelectListItem>();
            foreach (var sr in srList)
            {
                foreach (var user in userList)
                {
                    if (user.Id == sr.UserId)
                    {
                        SelectListItem li = new SelectListItem
                        {
                            Value = sr.UserId,
                            Text = user.FirstName + " " + user.LastName,

                        };
                        salesreps.Add(li);
                        ViewBag.Users = salesreps;
                    }

                }

            }
            if (ModelState.IsValid)
            {
                if(obj.AssignId == null)
                {
                    ModelState.AddModelError("", "You must select a Sales Rep.");
                    return View(obj);
                }

                var id = obj.AssignId;
                //search salerep table 

                int repId = _context.SalesReps.Where(i => i.UserId == id).Select(i => i.SalesRepId).SingleOrDefault();
                

                var groupToAdd = new Group
                {
                    SalesRepId = repId,
                    Name = obj.Name,
                    Address = obj.Address,
                    City = obj.City,
                    State = obj.State


                };
                _context.Add(groupToAdd);
                _context.SaveChanges();
                TempData[SD.Success] = "Account Created";
                return RedirectToAction("AdminIndex", "Groups");
            }
            return View(obj);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            var gl_role = await _roleManager.FindByNameAsync("Group Leader");
            var roles_list = _context.UserRoles.ToList();
            var user_list = _context.ApplicationUser.ToList();
            List<ApplicationUser> gl_to_add = new List<ApplicationUser>();
            List<SelectListItem> selectitems = new List<SelectListItem>();

            foreach(var user in user_list)
            {
                foreach(var role in roles_list)
                {
                    if(gl_role.Id == role.RoleId && role.UserId == user.Id)
                    {
                        gl_to_add.Add(user);
                    }
                }
            }

            group.GroupLeaderObj = _context.GroupLeaders.Where(u => u.GroupId == group.GroupId).Select(e => e).FirstOrDefault();

            foreach (var item in gl_to_add)
            {
                if (item.Id == group.GroupLeaderObj.UserId)
                {
                    SelectListItem newitem = new SelectListItem
                    {
                        Text = item.FirstName + " " + item.LastName,
                        Value = item.Id,
                        Selected = true
                    };
                    selectitems.Add(newitem);
                }
                else
                {
                    SelectListItem newitem = new SelectListItem
                    {
                        Text = item.FirstName + " " + item.LastName,
                        Value = item.Id,
                        Selected = false
                    };
                    selectitems.Add(newitem);
                } 
            }

            group.GL_List = selectitems;
            return View(group);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Group group)
        {
            //update group info
            var groupfromdb = await _context.Groups.FindAsync(id);
            groupfromdb.Name = group.Name;
            groupfromdb.Address = group.Address;
            groupfromdb.City = group.City;
            groupfromdb.State = group.State;

            //update group leader
            var groupleadersfromdb = _context.GroupLeaders.ToList();
            foreach(var item in groupleadersfromdb)
            {
                if(item.GroupId == groupfromdb.GroupId)
                {
                    item.UserId = group.GroupLeaderId;
                }
            }



            _context.SaveChanges();
            if(User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminIndex");
            }
            else
            {
                return RedirectToAction("SRIndex");
            }
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @group = await _context.Groups.FindAsync(id);
            _context.Groups.Remove(@group);
            await _context.SaveChangesAsync();
            if(User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminIndex", "Groups");
            }
            else
            {
                return RedirectToAction("SRIndex", "Groups");
            }
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.GroupId == id);
        }
        
        
    }
}
