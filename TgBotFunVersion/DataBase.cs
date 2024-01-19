using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Telegram.Bot.Types;



namespace TgBotFunVersion
{
    public class DataBase
    {
        private static string connectionString { get; set; }
        
        public DataBase(string connection)
        {
            connectionString = connection;
        }
       
        public string LastAnswer(string id)
        {
            string result;
            using SqlConnection sqlConnection = new SqlConnection(connectionString);

            sqlConnection.Open();
            string queryString = "SELECT LastAnswer FROM dbo.TableState1 WHERE Id LIKE (@value1)";
            SqlCommand addState = new SqlCommand(queryString, sqlConnection);
            addState.Parameters.AddWithValue("@value1", id);
            SqlDataReader reader = addState.ExecuteReader();
            try
            {
                result = reader["LastAnswer"].ToString();
            }
            catch 
            { 
                result = null; 
            }
            sqlConnection.Close();
            return result;
        }

        public List<string> LastState(string id)
        {
            using SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            string queryString = "SELECT State,LastAnswer FROM dbo.TableState1 WHERE Id LIKE (@value1)";
            SqlCommand addState = new SqlCommand(queryString, sqlConnection);
            addState.Parameters.AddWithValue("@value1", id);
            SqlDataReader reader = addState.ExecuteReader();
            List<string> result = new List<string>();
            while (reader.Read())
            {
                result.Add(reader["State"].ToString());
                result.Add(reader["LastAnswer"].ToString());
            }
            sqlConnection.Close();
            return result;
        }
        private bool HaveId(string id)
        {
            using SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            string queryString = "SELECT * FROM dbo.TableState1 WHERE Id LIKE (@value1)";
            SqlCommand addState = new SqlCommand(queryString, sqlConnection);
            addState.Parameters.AddWithValue("@value1", id);
            var result = addState.ExecuteScalar();
            bool doesExist = Convert.ToUInt64(result) > 0;
            sqlConnection.Close();
            return doesExist;
        }
        public void UpdateLastAnswer(Chat chat, string lastAnswer)
        {
            using SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            string queryString = "UPDATE dbo.TableState1 SET LastAnswer = (@value1) WHERE Id LIKE (@value2)";
            SqlCommand addState = new SqlCommand(queryString, sqlConnection);
            addState.Parameters.AddWithValue("@value1", lastAnswer);
            addState.Parameters.AddWithValue("@value2", chat.Id.ToString());
            addState.ExecuteNonQuery();
            sqlConnection.Close();
        }
        
        public void StateTable(string state, Chat chat)
        {
            if (HaveId(chat.Id.ToString()) == true)
            {
                using SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                string queryString = "UPDATE dbo.TableState1 SET State = (@value1) WHERE Id LIKE (@value2)";
                SqlCommand addState = new SqlCommand(queryString, sqlConnection);
                addState.Parameters.AddWithValue("@value1", state);
                addState.Parameters.AddWithValue("@value2", chat.Id.ToString());
                addState.ExecuteNonQuery();
                sqlConnection.Close();
            }
            else
            {
                using SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                string queryString = "INSERT INTO dbo.TableState1 (Id, State, LastAnswer) VALUES (@value1, @value2)";
                SqlCommand addState = new SqlCommand(queryString, sqlConnection);
                addState.Parameters.AddWithValue("@value1", chat.Id.ToString());
                addState.Parameters.AddWithValue("@value2", state);
                addState.ExecuteNonQuery();
                sqlConnection.Close();
            }
            
        } 
    }
}
