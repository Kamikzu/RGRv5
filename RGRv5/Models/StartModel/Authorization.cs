using System.ComponentModel.DataAnnotations;

namespace RGRv5.Models.StartModel
{
    public class Authorization 
    {
        
        [Display(Name = "Введите логин")]
        [Required(ErrorMessage = "Вам нужно ввести логин")]
        public string Login { get; set; }
        
        [Display(Name = "Введите пароль")]
        [Required(ErrorMessage = "Вам нужно ввести пароль")]
        public string Password { get; set; }

    }
}
