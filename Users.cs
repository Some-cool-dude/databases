using System.Data;
using System.Collections.Generic;
using System;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;

namespace lab3
{
    [Alias("Users")]
    public class User
    {     
        [AutoId]
        public Guid? Id {get; set;}

        public string Name {get; set;}

        public string TelephoneNumber {get; set;}

        [Default(0)]
        public int Funds {get; set;}
        
        [ForeignKey(typeof(Tariff), OnDelete = "SET NULL")]
        public Guid? TariffId {get; set;}

        public override string ToString() => $"{Id, -36} {Name, -20} {TelephoneNumber, -10} {Funds, -4} {TariffId}";
    }

    public class Users
    {
        IDbConnection con;

        public Users(IDbConnection con)
        {
            this.con = con;
        }

        public List<User> GetUsers()
        {
            try
            {
                return this.con.Select<User>();
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return new List<User>();
            }
        }

        public void AddUser(User user)
        {
            try
            {
               this.con.Insert(user); 
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        public void UpdateUser(User user)
        {
            try
            {
                this.con.Update(user);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        public void DeleteUser(Guid id)
        {
            try
            {
                this.con.Delete<User>(user => user.Id == id);
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
                            INSERT INTO users
                            (id, name, telephone_number, funds, tariff_id)
                            SELECT 
                                uuid_generate_v4(),
                                array_to_string(ARRAY(SELECT chr((48 + round(random() * 59)) :: integer) FROM generate_series(1,15 + generator*0)), ''),
                                array_to_string(ARRAY(SELECT chr((48 + round(random() * 9)) :: integer) FROM generate_series(1,10 + generator*0)), ''),
                                trunc(random()*10000) :: integer,
                                (SELECT id FROM tariffs ORDER BY random() LIMIT (1+generator*0))
                            FROM generate_series(1, @n) as generator", new {n});
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
                var q = this.con.From<User>().LeftJoin<User, Tariff>()
                .Where(u => u.Name.Contains(name))
                .And(u => u.TelephoneNumber.StartsWith(tel))
                .And<Tariff>(t => t.Cost < cost);
                return this.con.Select<User>(q);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return new List<User>();
            }
        }
    }
}