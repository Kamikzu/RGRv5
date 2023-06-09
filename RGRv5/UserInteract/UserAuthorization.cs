using System.Data;
using MySql.Data.MySqlClient;


namespace RGRv5.UserInteract
{
    public class UserAuthorization
    {
        public string Authorization(string userLogin, string userPassword)
        {
            DB db = new DB();
            db.OpenConnection();
            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand(
                "SELECT * FROM `tests`.`user_data` WHERE `login`=@uL AND `password`=@uP",
                db.GetConnection()
            );
            command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = userLogin;
            command.Parameters.Add("@uP", MySqlDbType.VarChar).Value = userPassword;

            adapter.SelectCommand = command;
            adapter.Fill(table);

            String _role = "0";
            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    _role = row["_role"].ToString();
                }
            } else
            {
                _role = "-1";
            }

            db.CloseConnection();
            return _role;//(table.Rows.Count != 0);
        }
    }
}
