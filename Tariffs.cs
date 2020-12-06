using System;
using System.Collections.Generic;
using System.Data;
using ServiceStack.OrmLite;
using ServiceStack.DataAnnotations;

namespace lab3
{
    [Alias("Tariffs")]
    public class Tariff
    {
        [AutoId]
        public Guid? Id {get;set;}
        
        public string Name {get;set;}

        [Default(0)]
        public int Cost {get;set;}

        public override string ToString() => $"{Id, -36} {Name, -32} {Cost, -4}";
    }

    public class Tariffs
    {
        IDbConnection con;

        public Tariffs(IDbConnection con)
        {
            this.con = con;
        }

        public List<Tariff> GetTariffs()
        {
            try
            {
                return this.con.Select<Tariff>();
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e);
                return new List<Tariff>();
            }
        }

        public void AddTariff(Tariff tariff)
        {
            this.con.Insert(tariff);
        }

        public void UpdateTariffs(Tariff tariff)
        {
            this.con.Update(tariff);
        }

        public void DeleteTariff(Guid id)
        {
            try
            {
                this.con.Delete<Tariff>(t => t.Id == id);
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
                            INSERT INTO tariffs
                            (id, name, cost)
                            SELECT 
                                uuid_generate_v4(),
                                md5(random()::text),
                                trunc(random()*10000) :: integer
                            FROM generate_series(1, @n)", new {n});
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
                var q = this.con.From<Tariff>()
                .Where(t => t.Name.Contains(name))
                .And(t => t.Cost < cost);
                return this.con.Select<Tariff>(q);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return new List<Tariff>();
            }
        }
    }
}