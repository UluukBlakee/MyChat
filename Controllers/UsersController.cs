using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyChat.Models;
using MyChat.Services;
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
        private readonly EmailService emailService;
        public UsersController(ChatContext context, UserManager<User> userManager, SignInManager<User> signInManager, EmailService emailService)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            this.emailService = emailService;
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
                user = await _context.Users.Include(u => u.Messages).FirstOrDefaultAsync(u => u.Id == id);
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
        public async Task<IActionResult> Edit(User user)
        {
            User currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user != null)
            {
                bool emailChanged = currentUser.Email != user.Email;
                bool userNameChanged = currentUser.UserName != user.UserName;

                currentUser.Avatar = user.Avatar;
                currentUser.Email = user.Email;
                currentUser.UserName = user.UserName;
                await _userManager.UpdateAsync(currentUser);

                string message = "Изменения в вашем профиле:\n\n";
                if (emailChanged)
                    message += $"Новый адрес электронной почты: {user.Email}\n";

                if (userNameChanged)
                    message += $"Новый логин: {user.UserName}\n";

                await emailService.SendEmailAsync(user.Email, "Уведомление: Изменения в вашем профиле", message);
            }
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(currentUser, false);
            return RedirectToAction("Details", new { id = user.Id });
        }

        public async Task<IActionResult> DataRequest(int id)
        {
            User user = await _context.Users.Include(u => u.Messages).FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                string message = $"Логин: {user.UserName}\n" +
                    $"Почта: {user.Email}\n" +
                    $"Дата рождения: {user.DateOfBirth}\n" +
                    $"Количество сообщений: {user.Messages.Count}";
                await emailService.SendEmailAsync(user.Email, "Личные данные", message);
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
