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

namespace Hibernate.Controllers
{
    public class GroupsController : Controller
    {
        

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
            return View(await _context.Groups.ToListAsync());
        }
        public async Task<IActionResult> AdminIndex()
        {
            return View(await _context.Groups.ToListAsync());
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

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }
            return View(@group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,City,State")] Group @group)
        {
            if (id != @group.GroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.GroupId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@group);
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
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.GroupId == id);
        }
    }
}
