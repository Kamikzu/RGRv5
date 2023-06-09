using System.ComponentModel.DataAnnotations;

namespace RGRv5.Models.Tests
{
    public class TestViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
    }
}
