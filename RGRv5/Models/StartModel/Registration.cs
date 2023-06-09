using System.ComponentModel.DataAnnotations;

namespace RGRv5.Models.StartModel
{
    public class Registration
    {
        [Display(Name = "Введите имя")]
        [Required(ErrorMessage = "Вам нужно ввести имя")]
        public string FirstName { get; set; }
        
        [Display(Name = "Введите фамилию")]
        [Required(ErrorMessage = "Вам нужно ввести фамилию")]
        public string LastName { get; set; }
        
        [Display(Name = "Введите логин")]
        [Required(ErrorMessage = "Вам нужно ввести логин")]
        public string Login { get; set; }
        
        [Display(Name = "Введите пароль")]
        [Required(ErrorMessage = "Вам нужно ввести пароль")]
        public string Password { get; set; }
    }
}
