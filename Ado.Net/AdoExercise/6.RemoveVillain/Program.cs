using System;
using System.Data.SqlClient;

namespace _6.RemoveVillain
{
    class Program
    {
        private static string connectionString = @"Server=.;Database=MinionsDB;Integrated Security=true";
        private static SqlConnection connection = new SqlConnection(connectionString);
        public const string DeleteVillainFromVillains = "DELETE FROM Villains WHERE Id = @villainId";
        const string FindVillainName = "SELECT Name FROM Villains WHERE Id = @villainId";
        const string DeleteVillainFromMV = "DELETE FROM MinionsVillains WHERE VillainId = @villainId";
        

         

        
        static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());
            connection.Open();
            using (connection)
            {
                SqlCommand command = new SqlCommand(FindVillainName, connection);
                command.Parameters.AddWithValue("@villainId", villainId);

                using (command)
                {
                    object result = command.ExecuteScalar();

                    if (result == null)
                    {
                        Console.WriteLine("No such villain was found.");
                    }

                    else
                    {
                        string villainName = (string)result;
                        int affectedRows = DeleteVillainFromMVA(villainId);
                        DeleteVillainByID(villainId);

                        Console.WriteLine($"{villainName} was deleted.");
                        Console.WriteLine($"{affectedRows} minions were released.");

                    }
                }
            }
        }
        private static void DeleteVillainByID(int villainId)
        {
            SqlCommand sqlCommand = new SqlCommand(DeleteVillainFromVillains, connection);
            sqlCommand.Parameters.AddWithValue("@villainId", villainId);

            using (sqlCommand)
            {
                sqlCommand.ExecuteNonQuery();
            }
        }

        private static int DeleteVillainFromMVA(int villainId)
        {
            SqlCommand command = new SqlCommand(DeleteVillainFromMV, connection);
            command.Parameters.AddWithValue("@villainId", villainId);

            using (command)
            {
                int affectedRows = command.ExecuteNonQuery();
                return affectedRows;
            }
        }
    }
}
