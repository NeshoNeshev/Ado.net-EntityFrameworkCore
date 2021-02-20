using System;
using System.Data.SqlClient;

namespace AdoNetDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //.                 =>MyComputer=>127.0.0.1
            // localhost        =>MyComputer=>127.0.0.1
            //127.0.0.1         =>MyComputer=>127.0.0.1
            //DESKTOP-627PSJV   =>MyComputer=>127.0.0.1

            //string connectionString = "Server=.;Database=SoftUni;User Id=Nesho;Password=123456";//локална база данни sql aouthorization
            // string connectionString = "Server=.;Database=SoftUni;Integrated Security=true";//локална база данни windows user
            using (SqlConnection sqlConnection = new SqlConnection("Server=.;Database=SoftUni;Integrated Security=true"))
            {
                sqlConnection.Open();
                string command = "SELECT [FirstName], [LastName], Salary FROM EMPLOYEES WHERE FirstName LIKE 'N%'";
                var sqlCommand = new SqlCommand(command, sqlConnection);
                //object result = sqlCommand.ExecuteScalar();//може да се кастне ако занаем какво ще върне с (int)=>exception или as int?=>nullable,връща една клетка
                //var reader = sqlCommand.ExecuteReader();//връща таблица
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string firstName = (string)reader["FirstName"];//може и с индекс (string)reader[0]
                        string lastName = (string)reader["LastName"];
                        decimal salary = (decimal)reader["Salary"];
                        Console.WriteLine(firstName + " " + lastName + "=>" + salary);
                    }
                }
                
                
                var updateSalaryComand = new SqlCommand("UPDATE Employees SET Salary = Salary * 1.10",sqlConnection);
                updateSalaryComand.ExecuteNonQuery();

                var reader2 = sqlCommand.ExecuteReader();
                using (reader2)
                {
                    while (reader2.Read())
                    {
                        string firstName = (string)reader2["FirstName"];
                        string lastName = (string)reader2["LastName"];
                        decimal salary = (decimal)reader2["Salary"];
                        Console.WriteLine(firstName + " " + lastName + "=>" + salary);
                    }
                }
            }
        }
    }
}
