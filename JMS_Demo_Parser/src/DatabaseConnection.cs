using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JMS_Demo_Parser.src
{
    class DatabaseConnection
    {
        private DataSet ds = new DataSet();
        private NpgsqlDataAdapter da;
        private NpgsqlConnection conn;

        public DatabaseConnection()
        {
            try
            {
                // PostgeSQL-style connection string
                string connstring = String.Format("Server={0};Port={1};" +
                    "User Id={2};Password={3};Database={4};",
                    "127.0.0.1", "5432", "postgres",
                    "postgres", "csgo-scores");
                // Making connection with Npgsql provider
                conn = new NpgsqlConnection(connstring);
                conn.Open();
            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                throw;
            }
        }

        public Boolean addTeam(string team)
        {
            string sql = "SELECT COUNT(*) FROM team WHERE name = :team";
            NpgsqlCommand command = new NpgsqlCommand(sql, conn);
            command.Parameters.Add(new NpgsqlParameter("team", NpgsqlDbType.Text));
            command.Parameters[0].Value = team;

            long numFound = (long) command.ExecuteScalar();

            if (numFound > 0)
            {
                sql = "UPDATE team SET num_matches = num_matches + 1 WHERE name = :team";
                command = new NpgsqlCommand(sql, conn);
                command.Parameters.Add(new NpgsqlParameter("team", team));

                command.ExecuteNonQuery();
            } else
            {
                sql = "INSERT INTO team (name, num_matches) VALUES (:team, 1)";
                command = new NpgsqlCommand(sql, conn);
                command.Parameters.Add(new NpgsqlParameter("team", team));

                command.ExecuteNonQuery();
            }
            
            return true;
        }
    }
}
