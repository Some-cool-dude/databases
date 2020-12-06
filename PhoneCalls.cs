using System.Data;
using System.Collections.Generic;
using System;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;

namespace lab3
{
    [Alias("PhoneCalls")]
    public class Call
    { 
        [AutoId]
        public Guid? Id {get;set;}

        [ForeignKey(typeof(User), OnDelete = "CASCADE")]
        public Guid FirstUserId {get;set;}

        [ForeignKey(typeof(User), OnDelete = "CASCADE")]
        public Guid SecondUserId {get;set;}

        [Default(0)]
        public int Duration {get;set;}

        public override string ToString() => $"{Id, -36} {Duration, -8} {FirstUserId, -36} {SecondUserId}";
    }

    public class PhoneCalls
    {
        private IDbConnection con;

        public PhoneCalls(IDbConnection con)
        {
            this.con = con;
        }

        public List<Call> GetPhoneCalls()
        {
            try
            {
                return this.con.Select<Call>();
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return new List<Call>();
            }
            
        }

        public void AddPhoneCall(Call call)
        {
            this.con.Insert(call);
        }

        public void UpdatePhoneCall(Call call)
        {
            this.con.Update(call);
        }

        public void DeletePhoneCall(Guid id)
        {
            try
            {
                this.con.Delete<Call>(c => c.Id == id);
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
                this.con.ExecuteSql(@"
                        INSERT INTO phone_calls
                        (id, duration, first_user_id, second_user_id)
                        SELECT 
                            uuid_generate_v4(),
                            trunc(random()*10000) :: integer,
                            (SELECT id FROM users ORDER BY random() LIMIT (1+generator*0)),
                            (SELECT id FROM users ORDER BY random() LIMIT (1+generator*0))
                        FROM generate_series(1, @n) as generator", new {n});
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
        
        public List<(string, string, string, string)> GetFilteredCalls(string name1, string tel1, string name2, string tel2)
        {
            try
            {                               
                return this.con.SqlList<(string name1, string tel1, string name2, string tel2)>(
                    @"SELECT u1.name, u1.telephone_number, u2.name, u2.telephone_number FROM phone_calls
                    INNER JOIN users as u1 ON first_user_id = u1.id
                    INNER JOIN users as u2 ON second_user_id = u2.id
                    WHERE u1.name ILIKE @name1 AND u1.telephone_number LIKE @tel1 AND
                    u2.name ILIKE @name2 AND u2.telephone_number LIKE @tel2", new {name1 = $"%{name1}%", name2 = $"%{name2}%", tel1 = $"{tel1}%", tel2 = $"{tel2}%"}
                );
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e);
                return new List<(string, string, string, string)>();
            }
        }
    }
}