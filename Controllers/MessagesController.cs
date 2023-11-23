using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyChat.Models;

namespace MyChat.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly ChatContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public MessagesController(ChatContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Chat()
        {
            return View();
        }
        public async Task<IActionResult> GetMessage()
        {
            List<Message> messages = await _context.Messages.Include(m => m.User).OrderByDescending(m => m.DepartureDate).Take(30).ToListAsync();
            return Json(messages);
        }
        [HttpPost]
        public async Task<IActionResult> AddMessage(string userName, string text)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            Message message = new Message
            {
                MessageText = text,
                UserId = user.Id,
                DepartureDate = DateTime.UtcNow
            };
            if (message != null)
            {
                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();
            }
            return Json(message);
        }
    }
}
