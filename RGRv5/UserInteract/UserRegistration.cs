using System.Data;
using MySql.Data.MySqlClient;

namespace RGRv5.UserInteract
{
    public class UserRegistration
    {
        public bool Registration(MySqlConnection connection, string userlogin, string userPassword, string userFirstName, string userLastName, DateTime registrationTime)
        {
            if (IsUserExists(connection, userlogin))
            {
                return false;
            }

            using (var command = new MySqlCommand("INSERT INTO `tests`.`user_data` (`login`, `password`) VALUES (@uL, @uP)", connection))
            {
                command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = userlogin; 
                command.Parameters.Add("@uP", MySqlDbType.VarChar).Value = userPassword;
        
                if (command.ExecuteNonQuery() != 1) 
                { 
                    return false; 
                }
            }

            using (var command2 = new MySqlCommand("INSERT INTO `tests`.`user_profile` (`first_name`, `last_name`, `registration_time`) VALUES (@uFN, @uLN, @rT)", connection))
            {
                command2.Parameters.Add("@uFN", MySqlDbType.VarChar).Value = userFirstName; 
                command2.Parameters.Add("@uLN", MySqlDbType.VarChar).Value = userLastName; 
                command2.Parameters.Add("@rT", MySqlDbType.Timestamp).Value = registrationTime;
        
                if (command2.ExecuteNonQuery() != 1) 
                { 
                    return false; 
                }
            }

            return true;
        }

        private bool IsUserExists(MySqlConnection connection, string userLogin)
        {
            using (var command = new MySqlCommand("SELECT `login` FROM `tests`.`user_data` WHERE `login`=@uL", connection))
            {
                command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = userLogin;

                using (var reader = command.ExecuteReader())
                {
                    return reader.HasRows;
                }
            }
        }

    }
}
