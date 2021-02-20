using System;
using System.Data.SqlClient;

namespace _3.MinionNames
{
    class Program
    {
        static void Main(string[] args)
        {
            const string sqlConnectionString = "Server=.;Database=MinionsDB;Integrated Security = true";

            using var connection = new SqlConnection(sqlConnectionString);
            connection.Open();

            int id = int.Parse(Console.ReadLine());

            string villianNameQuery = @"SELECT Name FROM Villains WHERE Id = @Id";

            using var command = new SqlCommand(villianNameQuery, connection);
            command.Parameters.AddWithValue("@Id", id);
            var result = command.ExecuteScalar();

            string minionQuery = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";

            if (result == null)
            {
                Console.WriteLine($"No villain with ID {id} exists in the database.");
            }
            else
            {
                Console.WriteLine($"Villain: {result}");
                using (var commands = new SqlCommand(minionQuery, connection))
                {
                    commands.Parameters.AddWithValue("@Id", id);
                    using (var reader = commands.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("(no minions)");
                        }

                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader[0]}. {reader[1]} {reader[2]}");
                        }
                    }
                }
            }

        }
    }
}
