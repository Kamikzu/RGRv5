namespace RGRv5.Models.Tests
{
    public class TestQuestion
    {
        public int Id { get; set; }
        public string? Question { get; set; }
        public List<string?> Answers { get; set; }
        public int CorrectAnswerIndex { get; set; } 
    }

    public class Test
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public List<TestQuestion> Questions { get; set; }
    }

}
