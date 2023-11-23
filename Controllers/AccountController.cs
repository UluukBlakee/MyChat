using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyChat.Models;
using MyChat.ViewModels;

namespace MyChat.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Пользователь с такой почтой уже зарегистрирован.");
                    return View(model);
                }
                var existingUserName = await _userManager.FindByNameAsync(model.UserName);
                if (existingUserName != null)
                {
                    ModelState.AddModelError("UserName", "Пользователь с таким никнеймом уже зарегистрирован.");
                    return View(model);
                }
                if (model.DateOfBirth.AddYears(18) > DateTime.Now)
                {
                    ModelState.AddModelError("DateOfBirth", "Вы должны быть старше 18 лет для регистрации.");
                    return View(model);
                }
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    Avatar = model.Avatar != null ? model.Avatar : "https://herrmans.eu/wp-content/uploads/2019/01/765-default-avatar.png",
                    DateOfBirth = model.DateOfBirth.ToUniversalTime()
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "user");
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Chat", "Messages");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByEmailAsync(model.Login);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(model.Login);
                }
                if (user != null)
                {
                    if (user.IsBlocked())
                    {
                        ModelState.AddModelError("", "Ваш аккаунт заблокирован.");
                        return View(model);
                    }
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    model.RememberMe,
                    false
                    );
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }

                        return RedirectToAction("Chat", "Messages");
                    }
                }
                ModelState.AddModelError("", "Неправильный логин и (или) пароль");
            }
            return View(model);
        }

        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Chat", "Messages");
        }
    }
}
