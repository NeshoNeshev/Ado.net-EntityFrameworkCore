using System;
using System.Data.SqlClient;
using System.Linq;

namespace _4.AddMinion
{
    class Program
    {
        static void Main(string[] args)
        {
            const string sqlConnectionString = "Server=.;Database=MinionsDB;Integrated Security = true";

            using var connection = new SqlConnection(sqlConnectionString);
            connection.Open();

            string[] minionInfo = Console.ReadLine().Split(' ').ToArray();
            string[] villainInfo = Console.ReadLine().Split(" ").ToArray();

            string minionName = minionInfo[1];
            int age = int.Parse(minionInfo[2]);
            string town = minionInfo[3];

            int? townId = GetTownId(connection, town);
            if (townId == null)
            {
                string createTownQuery = "INSERT INTO Towns(Id, Name) VALUES(6,@name)";
                using var command = new SqlCommand(createTownQuery, connection);
                command.Parameters.AddWithValue("@name", town);
                command.ExecuteNonQuery();
                townId = GetTownId(connection, town);

                Console.WriteLine($"Town {town} was added to the database.");
            }

            string villainName = villainInfo[1];
            int? villainId = GetVillainId(connection, villainName);
            if (villainId == null)
            {
                string createVillain = "INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";
                using var command = new SqlCommand(createVillain, connection);
                command.Parameters.AddWithValue("@villainName", villainName);
                command.ExecuteNonQuery();
                villainId = GetVillainId(connection, villainName);
                Console.WriteLine($"Villain {villainName} was added to the database.");
            }

            CreateMinion(connection, minionName, age, townId);

            var minionId = GetMinionId(connection, minionName);

            InsertMinionVillain(connection, villainId, minionId);
            Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");

        }

        private static void InsertMinionVillain(SqlConnection connection, int? villainId, int? minionId)
        {
            var insertIntoMinionVillain = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@villainId, @minionId)";
            using var command = new SqlCommand(insertIntoMinionVillain, connection);
            command.Parameters.AddWithValue("@villainId", minionId);
            command.Parameters.AddWithValue("@minionId", villainId);
            command.ExecuteNonQuery();
        }

        private static int? GetMinionId(SqlConnection connection, string minionName)
        {
            var minionIdQuery = "SELECT Id FROM Minions WHERE Name = @Name";
            using var command = new SqlCommand(minionIdQuery, connection);
            command.Parameters.AddWithValue("@Name", minionName);
            var minionId = command.ExecuteScalar();
            return (int?)minionId;
        }

        private static void CreateMinion(SqlConnection connection, string minionName, int age, int? townId)
        {
            string createMinion = "INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId)";
            using var sqlCommand = new SqlCommand(createMinion, connection);
            sqlCommand.Parameters.AddWithValue("@name", minionName);
            sqlCommand.Parameters.AddWithValue("@age", age);
            sqlCommand.Parameters.AddWithValue("@townId", townId);
            sqlCommand.ExecuteNonQuery();

        }

        private static int? GetVillainId(SqlConnection connection, string villainName)
        {

            string createVilainQuery = "SELECT Id FROM Villains WHERE Name = @Name";
            using var sqlCommand = new SqlCommand(createVilainQuery, connection);
            sqlCommand.Parameters.AddWithValue("@Name", villainName);
            var villainId = sqlCommand.ExecuteScalar();
            return (int?)villainId;
        }

        private static int? GetTownId(SqlConnection connection, string town)
        {
            string townIdQuery = "SELECT Id FROM Towns WHERE Name = @townName";

            using var sqlCommand = new SqlCommand(townIdQuery, connection);
            sqlCommand.Parameters.AddWithValue("@townName", town);
            var townId = sqlCommand.ExecuteScalar();
            return (int?)townId;
        }
    }
}
