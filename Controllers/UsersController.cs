using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyChat.Models;
using MyChat.ViewModels;
using System.Xml.Linq;

namespace MyChat.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ChatContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public UsersController(ChatContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index()
        {
            List<User> users = await _context.Users.ToListAsync();
            return View(users);
        }
        public async Task<IActionResult> Details(int? id, string? name)
        {
            User user = await _context.Users.Include(u => u.Messages).FirstOrDefaultAsync(u => u.UserName == name);
            if (user == null)
            {
                user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            }
            return View(user);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (user != null)
            {
                _context.Users.AddAsync(user);
                _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = user.Id });
        }

        [HttpPost]
        public async Task<IActionResult> BlockUser(int id)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.MaxValue;
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }


    }
}
