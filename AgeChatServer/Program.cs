using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            DataBase db = new DataBase();
            MySqlCommand command = new MySqlCommand("SELECT username FROM `users` WHERE `passwordHash` = SHA1('123')", db.getConnection());
            db.openConnection();
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            string user = "";
            while (reader.Read())
            {
                user = reader.GetString(0);
            }
            if(user != "")
            {
                Console.WriteLine(user);
            }
            else
            {
                Console.WriteLine("User not found!");
            }
            db.closeConnection();
            Console.ReadKey();
        }
    }
}
