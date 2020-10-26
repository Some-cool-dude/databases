using System;
using System.Collections.Generic;
using Npgsql;

namespace lab2
{
    public readonly struct Tariff
    {
        public Tariff(string name, int cost = 0,Guid? id = null)
        {
            Id = id;
            Name = name;
            Cost = cost;
        }

        
        public Guid? Id {get;}
        public string Name {get;}
        public int Cost {get;}

        public override string ToString() => $"{Id, -36} {Name, -32} {Cost, -4}";
    }

    public class Tariffs
    {
        private NpgsqlConnection con;

        private string db = "\"Tariffs\"";

        public Tariffs(NpgsqlConnection con)
        {
            this.con = con;
        }

        public List<Tariff> GetTariffs()
        {
            try
            {
                string sql = $"SELECT * FROM {this.db}";
                using var cmd = new NpgsqlCommand(sql, this.con);
                using NpgsqlDataReader rdr = cmd.ExecuteReader();
                List<Tariff> tariffs = new List<Tariff>();
                
                while(rdr.Read())
                {
                    Tariff tariff= new Tariff(rdr.GetString(0), rdr.GetInt32(1), rdr.GetGuid(2));
                    tariffs.Add(tariff);
                }

                return tariffs;
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e);
                return new List<Tariff>();
            }
        }

        private void AddUpdate(string sql, Tariff tariff)
        {
            try
            {
                using var cmd = new NpgsqlCommand(sql, this.con);

                cmd.Parameters.AddWithValue("name", tariff.Name);
                cmd.Parameters.AddWithValue("cost", tariff.Cost);
                if(tariff.Id != null)
                {
                    cmd.Parameters.AddWithValue("id", tariff.Id);
                }
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        public void AddTariff(Tariff tariff)
        {
            string sql = $"INSERT INTO {this.db}(\"Name\", \"Cost\") VALUES(@name, @cost)";
            AddUpdate(sql, tariff);
        }

        public void UpdateTariffs(Tariff tariff)
        {
            string sql = $"UPDATE {this.db} SET \"Name\"=@name, \"Cost\"=@cost WHERE \"ID\"=@id ";
            AddUpdate(sql, tariff);
        }

        public void DeleteTariff(Guid id)
        {
            try
            {
                string sql = $"DELETE FROM {this.db} WHERE \"ID\"=@id ";
                using var cmd = new NpgsqlCommand(sql, this.con);
                cmd.Parameters.AddWithValue("id", id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        public void Generate(int n)
        {
            try
            {
                string sql = string.Format(@"
                            INSERT INTO {0}
                            (""ID"", ""Name"", ""Cost"")
                            SELECT 
                                uuid_generate_v4(),
                                md5(random()::text),
                                trunc(random()*10000) :: integer
                            FROM generate_series(1, @n)", this.db);
                using var cmd = new NpgsqlCommand(sql, this.con);
                cmd.Parameters.AddWithValue("n", n);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        public List<Tariff> GetFilteredTariffs(string name, int cost)
        {
            try
            {
                string sql = $"SELECT * FROM {this.db} WHERE \"Name\" ILIKE @name AND \"Cost\" < @cost";
                using var cmd = new NpgsqlCommand(sql, this.con);
                cmd.Parameters.AddWithValue("name", $"%{name}%");
                cmd.Parameters.AddWithValue("cost", cost);
                cmd.Prepare();
                using NpgsqlDataReader rdr = cmd.ExecuteReader();
                List<Tariff> tariffs = new List<Tariff>();
                
                while(rdr.Read())
                {
                    Tariff tariff= new Tariff(rdr.GetString(0), rdr.GetInt32(1), rdr.GetGuid(2));
                    tariffs.Add(tariff);
                }

                return tariffs;
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return new List<Tariff>();
            }
        }
    }
}