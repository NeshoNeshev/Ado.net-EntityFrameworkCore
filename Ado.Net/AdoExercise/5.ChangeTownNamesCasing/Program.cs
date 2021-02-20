using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace _5.ChangeTownNamesCasing
{
    class Program
    {
        private static string connectionString = @"Server=.;Database=MinionsDB;Integrated Security=true";

        private static SqlConnection connection = new SqlConnection(connectionString);

        const string ChangeCityNames =
            "UPDATE Towns SET Name = UPPER(Name)WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";

        const string FindAllCityNames =
            "SELECT t.Name FROM Towns as t JOIN Countries AS c ON c.Id = t.CountryCode WHERE c.Name = @countryName";

        static void Main(string[] args)
        {

            string country = Console.ReadLine();

            connection.Open();

            using (connection)
            {
                SqlCommand command = new SqlCommand(ChangeCityNames, connection);
                command.Parameters.AddWithValue("@countryName", country);

                using (command)
                {
                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows == 0)
                    {
                        Console.WriteLine("No town names were affected.");
                    }
                    else
                    {
                        Console.WriteLine($"{affectedRows} town names were affected.");

                        PrintCityNames(country);
                    }
                }
            }

        }

        private static void PrintCityNames(string countryName)
        {
            SqlCommand command = new SqlCommand(FindAllCityNames, connection);
            command.Parameters.AddWithValue("@countryName", countryName);

            using (command)
            {
                SqlDataReader reader = command.ExecuteReader();
                List<string> cityNames = new List<string>();
                while (reader.Read())
                {
                    cityNames.Add((string) reader["Name"]);
                }

                Console.WriteLine($"[{string.Join(", ", cityNames)}]");
            }
        }
    }
}
