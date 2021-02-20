using System;
using System.Data.SqlClient;

namespace SqlInjection
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("UserName: ");// validen user i -- ili ' OR 1=1 --(hack) ili ' ; DELETE FROM Users; --
            string userName = Console.ReadLine();
            Console.Write("Password: ");//password never mind
            string password = Console.ReadLine();

            using (SqlConnection sqlConnection = new SqlConnection("Server=.;Database=Sevice;Integrated Security=true"))

            {
                sqlConnection.Open();
                // Wrong my be hacked
                var sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = '"
                                                + userName + "' AND Password ='" + password + "';", sqlConnection);// ако въведем валиден юзер и -- и грешна парола пак ще има достъп поради това ,че -- закоментира и прави валидна заявката 
                


                //Правилно да се ползва винаги
                var sqlCommand1 = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = '@Username' AND Password ='@Password';", sqlConnection);
                sqlCommand1.Parameters.AddWithValue("@Username", userName);
                sqlCommand1.Parameters.AddWithValue("@Password", password);

                int userCount = (int)sqlCommand.ExecuteScalar();
                if (userCount>0)
                {
                    Console.WriteLine("Welcome");
                }
                else
                {
                    Console.WriteLine("Access Forbidden");
                }

               
            }
        }
    }
}
