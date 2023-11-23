using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MyChat.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email или Логин")]
        public string Login { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
