using System.ComponentModel.DataAnnotations;

namespace MyChat.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле 'Логин' обязательно для заполнения")]
        [Display(Name = "Логин")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Поле 'Email' обязательно для заполнения")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Аватар")]
        public string? Avatar { get; set; }
        [Required(ErrorMessage = "Поле 'Дата рождение' обязательно для заполнения")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождение")]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Поле 'Пароль' обязательно для заполнения")]
        [StringLength(100, ErrorMessage = "Пароль должен содержать минимум {2} символа.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "Пароль должен содержать как минимум одну заглавную букву, одну строчную букву и одну цифру.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Поле 'Подтвердить пароль' обязательно для заполнения")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
    }
}
