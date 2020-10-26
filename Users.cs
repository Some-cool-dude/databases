using System.Collections.Generic;
using System;
using Npgsql;

namespace lab2
{

    public readonly struct User
    {
        public User(string name, string telNum, int funds = 0,Guid? id = null, Guid? tariffID = null)
        {
            Id = id;
            Name = name;
            TelepnoneNumber = telNum;
            Funds = funds;
            TariffID = tariffID;
        }

        
        public Guid? Id {get;}
        public string Name {get;}
        public string TelepnoneNumber {get;}
        public int Funds {get;}
        public Guid? TariffID {get;}

        public override string ToString() => $"{Id, -36} {Name, -20} {TelepnoneNumber, -10} {Funds, -4} {TariffID}";
    }

    public class Users
    {
        private NpgsqlConnection con;

        private string db = "\"Users\"";

        public Users(NpgsqlConnection con)
        {
            this.con = con;
        }

        public List<User> GetUsers()
        {
            try
            {
                string sql = $"SELECT * FROM {this.db}";
                using var cmd = new NpgsqlCommand(sql, this.con);
                using NpgsqlDataReader rdr = cmd.ExecuteReader();
                List<User> users = new List<User>();
                
                while(rdr.Read())
                {
                    User user= new User(rdr.GetString(3), rdr.GetString(1), rdr.GetInt32(0), rdr.GetGuid(2), rdr.GetValue(4) as Guid? ?? null);
                    users.Add(user);
                }

                return users;
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return new List<User>();
            }
        }

        private void AddUpdate(string sql, User user)
        {
            try
            {
                using var cmd = new NpgsqlCommand(sql, this.con);

                cmd.Parameters.AddWithValue("name", user.Name);
                cmd.Parameters.AddWithValue("funds", user.Funds);
                cmd.Parameters.AddWithValue("telNum", user.TelepnoneNumber);
                cmd.Parameters.AddWithValue("tariffID", (object)user.TariffID ?? DBNull.Value);
                if(user.Id != null)
                {
                    cmd.Parameters.AddWithValue("id", user.Id);
                }
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        public void AddUser(User user)
        {
            string sql = $"INSERT INTO {this.db}(\"Funds\", \"TelephoneNumber\", \"Name\", \"TariffID\") VALUES(@funds, @telNum, @name, @tariffID)";
            AddUpdate(sql, user);
        }

        public void UpdateUser(User user)
        {
            string sql = $"UPDATE {this.db} SET \"Funds\"=@funds, \"TelephoneNumber\"=@telNum, \"Name\"=@name, \"TariffID\"=@tariffID WHERE \"ID\"=@id ";
            AddUpdate(sql, user);
        }

        public void DeleteUser(Guid id)
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
                            (""ID"", ""Name"", ""TelephoneNumber"", ""Funds"", ""TariffID"")
                            SELECT 
                                uuid_generate_v4(),
                                array_to_string(ARRAY(SELECT chr((48 + round(random() * 59)) :: integer) FROM generate_series(1,15 + generator*0)), ''),
                                array_to_string(ARRAY(SELECT chr((48 + round(random() * 9)) :: integer) FROM generate_series(1,10 + generator*0)), ''),
                                trunc(random()*10000) :: integer,
                                (SELECT ""ID"" FROM ""Tariffs"" ORDER BY random() LIMIT (1+generator*0))
                            FROM generate_series(1, @n) as generator", this.db);
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

        public List<User> GetFilteredUsers(string name, string tel, int cost)
        {
            try
            {
                string joinDB = "\"Tariffs\"";
                string Name = $"{this.db}.\"Name\"";
                string Tel = "\"TelephoneNumber\"";
                string Cost = "\"Cost\"";
                string sql = $"SELECT {Name}, {Tel}, {Cost} FROM {this.db} LEFT OUTER JOIN {joinDB} ON {this.db}.\"TariffID\" = {joinDB}.\"ID\" WHERE {Name} ILIKE @name AND {Tel} LIKE @tel AND {Cost} < @cost";
                using var cmd = new NpgsqlCommand(sql, this.con);
                cmd.Parameters.AddWithValue("name", $"%{name}%");
                cmd.Parameters.AddWithValue("tel", $"{tel}%");
                cmd.Parameters.AddWithValue("cost", cost);
                cmd.Prepare();
                using NpgsqlDataReader rdr = cmd.ExecuteReader();
                List<User> users = new List<User>();
                
                while(rdr.Read())
                {
                    User user= new User(rdr.GetString(0), rdr.GetString(1), rdr.GetInt32(2));
                    users.Add(user);
                }

                return users;
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return new List<User>();
            }
        }
    }
}