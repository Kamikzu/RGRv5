using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using RGRv5.Models;
using RGRv5.Models.StartModel;
using RGRv5.UserInteract;
using RGRv5.Models.Tests;

namespace RGRv5.Controllers
{
    public class WebController : Controller
    {
        DB db = new DB();
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TestViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = db.GetConnection())
                {
                    connection.Open();

                    // Insert the test data
                    var insertTestCommand = new MySqlCommand("INSERT INTO tests_data (title) VALUES (@TestsTitle)", connection);
                    insertTestCommand.Parameters.AddWithValue("@TestsTitle", model.Title);
                    insertTestCommand.ExecuteNonQuery();

                    // Get the ID of the newly inserted test
                    var selectTestIdCommand = new MySqlCommand("SELECT id FROM tests_data WHERE title=@TestsTitle", connection);
                    selectTestIdCommand.Parameters.AddWithValue("@TestsTitle", model.Title);
                    var testId = (int)selectTestIdCommand.ExecuteScalar();

                    // Make sure there are exactly 10 questions
                    if (model.Questions.Count != 10)
                    {
                        ModelState.AddModelError("", "There must be exactly 10 questions.");
                        return View(model);
                    }

                    // Insert each question and its answers into the database
                    for (var i = 0; i < model.Questions.Count; i++)
                    {
                        var question = model.Questions[i];
                        var insertQuestionCommand = new MySqlCommand(
                            "INSERT INTO test_questions (test_id, question, answer_1, answer_2, answer_3, answer_4, correct_answer_index) VALUES (@TestId, @Question, @Answer1, @Answer2, @Answer3, @Answer4, @CorrectAnswerIndex)",
                            connection);
                        insertQuestionCommand.Parameters.AddWithValue("@TestId", testId);
                        insertQuestionCommand.Parameters.AddWithValue("@Question", question.Question);
                        insertQuestionCommand.Parameters.AddWithValue("@Answer1", question.Answer1);
                        insertQuestionCommand.Parameters.AddWithValue("@Answer2", question.Answer2);
                        insertQuestionCommand.Parameters.AddWithValue("@Answer3", question.Answer3);
                        insertQuestionCommand.Parameters.AddWithValue("@Answer4", question.Answer4);
                        insertQuestionCommand.Parameters.AddWithValue("@CorrectAnswerIndex", question.CorrectAnswerIndex - 1);
                        insertQuestionCommand.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Update");
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            using (var connection = db.GetConnection())
            {
                connection.Open();

                // Get the test data
                var selectTestCommand = new MySqlCommand("SELECT * FROM tests_data WHERE id = @Id", connection);
                selectTestCommand.Parameters.AddWithValue("@Id", id);
                var testReader = selectTestCommand.ExecuteReader();

                if (!testReader.Read())
                {
                    // Test not found
                    return NotFound();
                }
                var model = new TestViewModel
                {
                    Id = id,
                    Title = testReader.GetString("title")
                };
                connection.Close();
                connection.Open();
                // Get the questions and answers for the test
                var selectQuestionsCommand = new MySqlCommand("SELECT * FROM test_questions WHERE test_id = @TestId", connection);
                selectQuestionsCommand.Parameters.AddWithValue("@TestId", id);
                var questionsReader = selectQuestionsCommand.ExecuteReader();
                while (questionsReader.Read())
                {
                    model.Questions.Add(new QuestionViewModel
                    {
                        Id = questionsReader.GetInt32("id"),
                        Question = questionsReader.GetString("question"),
                        Answer1 = questionsReader.GetString("answer_1"),
                        Answer2 = questionsReader.GetString("answer_2"),
                        Answer3 = questionsReader.GetString("answer_3"),
                        Answer4 = questionsReader.GetString("answer_4"),
                        CorrectAnswerIndex = questionsReader.GetInt32("correct_answer_index")
                    });
                }

                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TestViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = db.GetConnection())
                {
                    connection.Open();

                    // Update the test data
                    var updateTestCommand = new MySqlCommand("UPDATE tests_data SET title = @Title WHERE id = @Id", connection);
                    updateTestCommand.Parameters.AddWithValue("@Title", model.Title);
                    updateTestCommand.Parameters.AddWithValue("@Id", model.Id);
                    updateTestCommand.ExecuteNonQuery();

                    // Make sure there are exactly 10 questions
                    if (model.Questions.Count != 10)
                    {
                        ModelState.AddModelError("", "There must be exactly 10 questions.");
                        return View(model);
                    }

                    // Update or insert each question and its answers into the database
                    for (var i = 0; i < model.Questions.Count; i++)
                    {
                        var question = model.Questions[i];
                        if (question.Id == 0)
                        {
                            // This is a new question, so insert it
                            var insertQuestionCommand = new MySqlCommand(
                                "INSERT INTO test_questions (test_id, question, answer_1, answer_2, answer_3, answer_4, correct_answer_index) VALUES (@TestId, @Question, @Answer1, @Answer2, @Answer3, @Answer4, @CorrectAnswerIndex)",
                                connection);
                            insertQuestionCommand.Parameters.AddWithValue("@TestId", model.Id);
                            insertQuestionCommand.Parameters.AddWithValue("@Question", question.Question);
                            insertQuestionCommand.Parameters.AddWithValue("@Answer1", question.Answer1);
                            insertQuestionCommand.Parameters.AddWithValue("@Answer2", question.Answer2);
                            insertQuestionCommand.Parameters.AddWithValue("@Answer3", question.Answer3);
                            insertQuestionCommand.Parameters.AddWithValue("@Answer4", question.Answer4);
                            insertQuestionCommand.Parameters.AddWithValue("@CorrectAnswerIndex", question.CorrectAnswerIndex - 1);
                            insertQuestionCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            // This is an existing question, so update it
                            var updateQuestionCommand = new MySqlCommand(
                                "UPDATE test_questions SET question = @Question, answer_1 = @Answer1, answer_2 = @Answer2, answer_3 = @Answer3, answer_4 = @Answer4, correct_answer_index = @CorrectAnswerIndex WHERE id = @Id",
                                connection);
                            updateQuestionCommand.Parameters.AddWithValue("@Question", question.Question);
                            updateQuestionCommand.Parameters.AddWithValue("@Answer1", question.Answer1);
                            updateQuestionCommand.Parameters.AddWithValue("@Answer2", question.Answer2);
                            updateQuestionCommand.Parameters.AddWithValue("@Answer3", question.Answer3);
                            updateQuestionCommand.Parameters.AddWithValue("@Answer4", question.Answer4);
                            updateQuestionCommand.Parameters.AddWithValue("@CorrectAnswerIndex", question.CorrectAnswerIndex - 1);
                            updateQuestionCommand.Parameters.AddWithValue("@Id", question.Id);
                            updateQuestionCommand.ExecuteNonQuery();
                        }
                    }

                    // Delete any questions that were removed
                    var existingQuestionIds = model.Questions.Where(q => q.Id > 0).Select(q => q.Id).ToList();
                    var deleteQuestionsCommand =
                        new MySqlCommand(
                            $"DELETE FROM test_questions WHERE test_id = @TestId AND id NOT IN ({string.Join(",", existingQuestionIds)})",
                            connection);
                    deleteQuestionsCommand.Parameters.AddWithValue("@TestId", model.Id);
                    deleteQuestionsCommand.ExecuteNonQuery();
                }

                return RedirectToAction("Update");
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Update()
        {
            using (var connection = db.GetConnection())
            {
                connection.Open();

                // Get a list of all tests and their question count
                var selectTestsCommand =
                    new MySqlCommand(
                        "SELECT t.*, COUNT(q.id) AS question_count FROM tests_data t LEFT JOIN test_questions q ON t.id = q.test_id GROUP BY t.id",
                        connection);
                var testsReader = selectTestsCommand.ExecuteReader();

                var model = new List<TestSummaryViewModel>();
                while (testsReader.Read())
                {
                    model.Add(new TestSummaryViewModel
                    {
                        Id = testsReader.GetInt32("id"),
                        Title = testsReader.GetString("title"),
                        QuestionCount = testsReader.GetInt32("question_count")
                    });
                }

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Web/DeleteTest/{id}")]
        public IActionResult DeleteTest(int id)
        {
            db.OpenConnection();
            // Delete the test and its questions
            var deleteUserAnswersCommand = new MySqlCommand("DELETE FROM user_answers WHERE test_id = @TestId", db.GetConnection());
            deleteUserAnswersCommand.Parameters.AddWithValue("@TestId", id);
            deleteUserAnswersCommand.ExecuteNonQuery();

            var deleteQuestionsCommand = new MySqlCommand("DELETE FROM test_questions WHERE test_id = @Testid", db.GetConnection());
            deleteQuestionsCommand.Parameters.AddWithValue("@TestId", id);
            deleteQuestionsCommand.ExecuteNonQuery();

            var deleteUserTestsCommand = new MySqlCommand("DELETE FROM user_tests WHERE test_id = @TestId", db.GetConnection());
            deleteUserTestsCommand.Parameters.AddWithValue("@TestId", id);
            deleteUserTestsCommand.ExecuteNonQuery();

            var deleteTestCommand = new MySqlCommand("DELETE FROM tests_data WHERE id = @Id", db.GetConnection());
            deleteTestCommand.Parameters.AddWithValue("@Id", id);
            deleteTestCommand.ExecuteNonQuery();

            db.CloseConnection();

            return RedirectToAction("Update");
        }

        [HttpPost]
        [Route("Web/DeleteQuestion/{id}")]
        public IActionResult DeleteQuestion(int id)
        {
            using (var connection = db.GetConnection())
            {
                connection.Open();

                var deleteQuestionCommand = new MySqlCommand("DELETE FROM user_answers WHERE question_id = @Id", connection);
                deleteQuestionCommand.Parameters.AddWithValue("@Id", id);
                deleteQuestionCommand.ExecuteNonQuery();

                var deleteQuestionCommand2 = new MySqlCommand("DELETE FROM test_questions WHERE id = @Id", connection);
                deleteQuestionCommand2.Parameters.AddWithValue("@Id", id);
                deleteQuestionCommand2.ExecuteNonQuery();
            }

            return Ok();
        }
        public IActionResult Index()
        {
            var tests = new List<Test>();
            using (var conn = db.GetConnection())
            {
                conn.Open();
                using (var command = new MySqlCommand("SELECT * FROM tests_data", conn))
                {
                    using (var adapter = new MySqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var test = new Test();
                            test.Id = Convert.ToInt32(row["id"]);
                            test.Title = row["title"].ToString();

                            // Получаем вопросы и ответы для теста
                            test.Questions = GetQuestions(test.Id, conn);

                            tests.Add(test);
                        }
                    }
                }
            }

            return View(tests);
        }

        private List<TestQuestion> GetQuestions(int testId, MySqlConnection conn)
        {
            var questions = new List<TestQuestion>();

            using (var command = new MySqlCommand($"SELECT * FROM test_questions WHERE test_id = @testId", conn))
            {
                command.Parameters.AddWithValue("@testId", testId);

                using (var adapter = new MySqlDataAdapter(command))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var question = new TestQuestion();
                        question.Id = Convert.ToInt32(row["id"]);
                        question.Question = row["question"].ToString();
                        question.CorrectAnswerIndex = Convert.ToInt32(row["correct_answer_index"]);

                        // Получаем все ответы для текущего вопроса
                        question.Answers = new List<string?>();
                        for (int i = 1; i <= 4; i++)
                        {
                            if (!string.IsNullOrEmpty(row[$"answer_{i}"].ToString()))
                            {
                                question.Answers.Add(row[$"answer_{i}"].ToString());
                            }
                        }

                        questions.Add(question);
                    }
                }
            }

            return questions;
        }

        public IActionResult TakeTest(int id)
        {
            var test = new Test();
            using (var conn = db.GetConnection())
            {
                conn.Open();
                using (var command = new MySqlCommand("SELECT * FROM tests_data WHERE id = @id", conn))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            test.Id = Convert.ToInt32(reader["id"]);
                            test.Title = reader["title"].ToString();

                            // Закрыть DataReader
                            reader.Close();

                            // Получаем вопросы и ответы для теста
                            test.Questions = GetQuestions(test.Id, conn);

                            return View(test);
                        }
                    }
                }
            }

            return RedirectToAction("Index");
        }

        // Сохранение ответов пользователя
        [HttpPost]
        public IActionResult SaveAnswers(int testId, List<int> answerIndexes, string timeSpent)
        {
            using (var conn = db.GetConnection())
            {
                conn.Open();
                for (int i = 0; i < answerIndexes.Count; i++)
                {
                    using (var command =
                           new MySqlCommand(
                               "INSERT INTO user_answers (test_id, question_id, answer_index) VALUES (@testId, @questionId, @answerIndex)",
                               conn))
                    {
                        command.Parameters.AddWithValue("@testId", testId);
                        command.Parameters.AddWithValue("@questionId", i + 1);
                        command.Parameters.AddWithValue("@answerIndex", answerIndexes[i]);
                        command.ExecuteNonQuery();
                    }
                }

                // Сохраняем время прохождения теста
                using (var command = new MySqlCommand("INSERT INTO user_tests (test_id, time_spent) VALUES (@testId, @timeSpent)",
                           conn))
                {
                    command.Parameters.AddWithValue("@testId", testId);
                    command.Parameters.AddWithValue("@timeSpent", timeSpent);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Results", new { testId });
        }

        // Результаты прохождения теста
        public IActionResult Results(int testId)
        {
            int totalScore = 0;
            int maxScore = 10;
            int numberOfQuestions = 10;

            List<int> correctAnswerIndexes = new List<int>();
            List<int> userAnswerIndexes = new List<int>();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                // Получаем все правильные индексы ответов для данного теста
                using (var command =
                       new MySqlCommand("SELECT correct_answer_index FROM test_questions WHERE test_id = @testId",
                           conn))
                {
                    command.Parameters.AddWithValue("@testId", testId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            correctAnswerIndexes.Add(Convert.ToInt32(reader["correct_answer_index"]));
                        }

                        reader.Close();
                    }
                }

                ViewBag.CorrectAnswerIndexes = correctAnswerIndexes;

                // Получаем время прохождения теста
                using (var command =
                       new MySqlCommand("SELECT time_spent FROM user_tests WHERE test_id = @testId", conn))
                {
                    command.Parameters.AddWithValue("@testId", testId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string timeSpentString = reader["time_spent"].ToString();
                            TimeSpan totalTime = TimeSpan.Parse(timeSpentString);
                            ViewBag.TotalTime = $"{totalTime.Hours:00}:{totalTime.Minutes:00}:{totalTime.Seconds:00}";
                        }

                        reader.Close();
                    }
                }

                for (int questionIndex = 0; questionIndex < numberOfQuestions; questionIndex++)
                {
                    using (var command1 =
                           new MySqlCommand("SELECT * FROM user_answers WHERE test_id = @testId AND question_id = @questionId",
                               conn))
                    {
                        command1.Parameters.AddWithValue("@testId", testId);
                        command1.Parameters.AddWithValue("@questionId", questionIndex + 1);

                        using (var reader1 = command1.ExecuteReader())
                        {
                            if (reader1.Read())
                            {
                                int answerIndex = Convert.ToInt32(reader1["answer_index"]);

                                // Используем правильный индекс ответа из списка
                                int correctAnswerIndex = correctAnswerIndexes[questionIndex];

                                if (correctAnswerIndex == answerIndex)
                                {
                                    totalScore += maxScore;
                                }
                                userAnswerIndexes.Add(answerIndex); // Добавляем пользовательский ответ в список
                            }

                            reader1.Close();
                        }
                    }
                }
                var deleteCommand = new MySqlCommand("DELETE FROM user_answers WHERE test_id = @testId", conn);
                deleteCommand.Parameters.AddWithValue("@testId", testId);
                deleteCommand.ExecuteNonQuery();

                var deleteCommand2 = new MySqlCommand("DELETE FROM user_tests WHERE test_id = @testId", conn);
                deleteCommand2.Parameters.AddWithValue("@testId", testId);
                deleteCommand2.ExecuteNonQuery();

                ViewBag.UserAnswers = userAnswerIndexes; // Добавляем список пользовательских ответов в ViewBag
                ViewBag.TotalScore = totalScore;
                ViewBag.MaxScore = maxScore * numberOfQuestions;
            }

            return View();
        }

        private readonly ILogger<WebController> _logger;

        public WebController(ILogger<WebController> logger)
        {
            _logger = logger;
        }
        public IActionResult GoToAuthorization()
        {
            return Redirect("Authorization");
        }
        public IActionResult GoToRegistration()
        {
            return Redirect("Registration");
        }
        public IActionResult GoToTests()
        {
            return Redirect("Index");
        }
        public IActionResult GoToUpdateTests()
        {
            return Redirect("Update");
        }
        public IActionResult GoToTopPlayers()
        {
            return Redirect("Registration");
        }
        public IActionResult Register(Registration registration)
        {
            DateTime regestrationTime = DateTime.Now;
            UserRegistration userRegistration = new UserRegistration();
            db.OpenConnection();
            if (userRegistration.Registration(db.GetConnection(), registration.Login, registration.Password,
                    registration.FirstName,
                    registration.LastName, regestrationTime))
            {
                db.CloseConnection();
                return Redirect("Home");
            }
            db.CloseConnection();
            return Redirect("/");
        }
        //1
        public IActionResult Authoriz(Authorization authorization)
        {
            UserAuthorization userAuthorization = new UserAuthorization();
            String _role = userAuthorization.Authorization(authorization.Login, authorization.Password);
            if (_role != "-1")
            {
                HttpContext.Session.SetString("_role", _role);
                return Redirect("Home");
            }
            return Redirect("/");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Home()
        {
            return View();
        }
        public IActionResult Registration()
        {
            return View();
        }
        public IActionResult Authorization()
        {
            return View();
        }
        public IActionResult UserProfile()
        {
            return View("Profile");
        }
    }
}
