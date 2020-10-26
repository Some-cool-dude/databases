using System;
using System.Collections.Generic;
using Npgsql;

namespace lab2
{
    using FilteredCalls = System.Tuple<string, string, string, string>;
    public readonly struct Call
    {
        public Call(Guid firstUserId, Guid secondUserId, int duration = 0,Guid? id = null)
        {
            Id = id;
            FirstUserId = firstUserId;
            SecondUserId = secondUserId;
            Duration = duration;
        }

        
        public Guid? Id {get;}
        public Guid FirstUserId {get;}
        public Guid SecondUserId {get;}
        public int Duration {get;}

        public override string ToString() => $"{Id, -36} {Duration, -8} {FirstUserId, -36} {SecondUserId}";
    }

    public class PhoneCalls
    {
        private NpgsqlConnection con;

        private string db = "\"PhoneCalls\"";

        public PhoneCalls(NpgsqlConnection con)
        {
            this.con = con;
        }

        public List<Call> GetPhoneCalls()
        {
            try
            {
                string sql = $"SELECT * FROM {this.db}";
                using var cmd = new NpgsqlCommand(sql, this.con);
                using NpgsqlDataReader rdr = cmd.ExecuteReader();
                List<Call> calls = new List<Call>();
                
                while(rdr.Read())
                {
                    Call call= new Call(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetInt32(3), rdr.GetGuid(2));
                    calls.Add(call);
                }
                return calls;
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return new List<Call>();
            }
            
        }

        private void AddUpdate(string sql, Call call)
        {
            try
            {
                using var cmd = new NpgsqlCommand(sql, this.con);

                cmd.Parameters.AddWithValue("firstId", call.FirstUserId);
                cmd.Parameters.AddWithValue("secondId", call.SecondUserId);
                cmd.Parameters.AddWithValue("duration", call.Duration);
                if(call.Id != null)
                {
                    cmd.Parameters.AddWithValue("id", call.Id);
                }
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        public void AddPhoneCall(Call call)
        {
            string sql = $"INSERT INTO {this.db}(\"FirstUserID\", \"SecondUserID\", \"Duration\") VALUES(@firstId, @secondId, @duration)";
            AddUpdate(sql, call);
        }

        public void UpdatePhoneCall(Call call)
        {
            string sql = $"UPDATE {this.db} SET \"FirstUserID\"=@firstId, \"SecondUserID\"=@secondId, \"Duration\"=@duration, WHERE \"ID\"=@id ";
            AddUpdate(sql, call);
        }

        public void DeletePhoneCall(Guid id)
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
                        (""ID"", ""Duration"", ""FirstUserID"", ""SecondUserID"")
                        SELECT 
                            uuid_generate_v4(),
                            trunc(random()*10000) :: integer,
                            (SELECT ""ID"" FROM ""Users"" ORDER BY random() LIMIT (1+generator*0)),
                            (SELECT ""ID"" FROM ""Users"" ORDER BY random() LIMIT (1+generator*0))
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
        
        public List<FilteredCalls> GetFilteredCalls(string name1, string tel1, string name2, string tel2)
        {
            try
            {
                string sql = String.Format(@"SELECT u1.""Name"", u1.""TelephoneNumber"", u2.""Name"", u2.""TelephoneNumber"" FROM {0}
                                        INNER JOIN ""Users"" as u1 ON ""FirstUserID"" = u1.""ID""
                                        INNER JOIN ""Users"" as u2 ON ""SecondUserID"" = u2.""ID""
                                        WHERE u1.""Name"" ILIKE @name1 AND u1.""TelephoneNumber"" LIKE @tel1 AND
                                        u2.""Name"" ILIKE @name2 AND u2.""TelephoneNumber"" LIKE @tel2", this.db);
                using var cmd = new NpgsqlCommand(sql, this.con);
                cmd.Parameters.AddWithValue("name1", $"%{name1}%");
                cmd.Parameters.AddWithValue("tel1", $"{tel1}%");
                cmd.Parameters.AddWithValue("name2", $"%{name2}%");
                cmd.Parameters.AddWithValue("tel2", $"{tel2}%");
                cmd.Prepare();
                using NpgsqlDataReader rdr = cmd.ExecuteReader();
                List<FilteredCalls> calls = new List<FilteredCalls>();
                
                while(rdr.Read())
                {
                    FilteredCalls call= new FilteredCalls(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetString(3));
                    calls.Add(call);
                }

                return calls;
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return new List<FilteredCalls>();
            }
        }
    }
}