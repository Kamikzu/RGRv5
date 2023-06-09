using System.ComponentModel.DataAnnotations;

namespace RGRv5.Models.Tests
{
    public class QuestionViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer1 { get; set; }

        [Required]
        public string Answer2 { get; set; }

        [Required]
        public string Answer3 { get; set; }

        [Required]
        public string Answer4 { get; set; }

        [Range(0, 3)]
        public int CorrectAnswerIndex { get; set; }
    }
}
