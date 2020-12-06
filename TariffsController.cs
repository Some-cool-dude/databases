using System;
using System.Collections.Generic;
using System.Linq;

namespace lab3
{
    public class TariffsController
    {

        Tariffs tariffsModel;
        Viewer viewer;
        public TariffsController(Tariffs tariffs, Viewer viewer)
        {
            tariffsModel = tariffs;
            this.viewer = viewer;
        }

        public void AddTariff()
        {
            try
            {
                Console.WriteLine("Enter name:");
                string name = Console.ReadLine();
                if(name == "")
                {
                    throw new Exception();
                }
                Console.WriteLine("Enter cost:");
                int cost = 10;
                cost = int.Parse(Console.ReadLine());
                Tariff tariff = new Tariff();
                tariff.Name = name;
                tariff.Cost = cost;
                tariffsModel.AddTariff(tariff);
                Console.Clear();
            }
            catch(Exception)
            {
                Console.Clear();
                Console.WriteLine("Invalid data");
            }
        }

        public void UpdateTariff(List<Tariff> tariffs)
        {
            try
            {
                viewer.ShowTariffs(tariffs);
                Console.WriteLine("\nEnter id of desire tariff:");
                string id = Console.ReadLine();
                
                Guid tariffId = new Guid(id);
                var result = tariffs.Where(item => item.Id == tariffId);
                if(!result.Any())
                {
                    throw new Exception();
                }
                Tariff tariff = result.First();

                Console.WriteLine("Change name:");
                string name = viewer.ReadLine(tariff.Name);
                if(name == "")
                {
                    throw new Exception();
                }

                int cost = int.Parse(viewer.ReadLine(tariff.Cost.ToString()));
                tariff.Name = name;
                tariff.Cost = cost;
                tariffsModel.UpdateTariffs(tariff);
                Console.Clear();
            }
            catch(Exception)
            {
                Console.Clear();
                Console.WriteLine("Invalid data");
            }
        }

        public void DeleteTariff(List<Tariff> tariffs)
        {
            viewer.ShowTariffs(tariffs);
            Console.WriteLine("\nEnter id of desire tariff:");
            string id = Console.ReadLine();;
            try
            {
                Guid tariffId = new Guid(id);
                var result = tariffs.Where(item => item.Id == tariffId);
                if(!result.Any())
                {
                    throw new Exception();
                }
                tariffsModel.DeleteTariff(tariffId);
                Console.Clear();
            }
            catch(Exception)
            {
                Console.Clear();
                Console.WriteLine("Wrong Id");
            }
        }

        public List<Tariff> FilterTariffs()
        {
            Console.WriteLine("Enter name or skip:");
            string name = Console.ReadLine();
            Console.WriteLine("Tariff const is less than:");
            string cost = Console.ReadLine();
            Console.Clear();
            try
            {
                int num = int.Parse(cost);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                List<Tariff> tariffs = tariffsModel.GetFilteredTariffs(name, num);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.Clear();
                Console.WriteLine($"Execution time:{elapsedMs}");
                return tariffs;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}