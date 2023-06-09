namespace RGRv5.Models.Tests
{
    // Класс для хранения информации о тесте
    public class TestInfo
    {
        public List<TestQuestion> Questions { get; set; }
        public int MaxScore { get; set; }
            
    }
    // Класс для хранения результатов прохождения теста пользователем
    class UserTestResult
    {
        public int TestId { get; set; }
        public List<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }

// Класс для хранения ответа пользователя на вопрос
    class UserAnswer
    {
        public int QuestionId { get; set; }
        public int AnswerIndex { get; set; }
    }
}
