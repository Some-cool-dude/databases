using System;
using System.Collections.Generic;
using System.Linq;

namespace lab3
{
    public class UsersController
    {
        Viewer viewer;
        Users usersModel;
        Tariffs tariffsModel;
        public UsersController(Users users, Tariffs tariffs, Viewer viewer)
        {
            this.viewer = viewer;
            usersModel = users;
            tariffsModel = tariffs;
        }

        public void AddUser()
        {
            try
            {
                Console.WriteLine("Enter name:");
                string name = Console.ReadLine();
                if(name == "")
                {
                    throw new Exception();
                }
                Console.WriteLine("Enter telephone number:");
                string tel = Console.ReadLine();
                if(tel == "")
                {
                    throw new Exception();
                }
                Console.WriteLine("Enter funds:");
                int funds = 0;
                funds = int.Parse(Console.ReadLine());
                List<Tariff> tariffs = tariffsModel.GetTariffs();
                viewer.ShowTariffs(tariffs);
                Console.WriteLine("\nEnter tariff id or nothing:");
                string id = Console.ReadLine();
                User user = new User();
                user.Name = name;
                user.TelephoneNumber = tel;
                user.Funds = funds;
                if(id != "")
                {
                    Guid tariffId = new Guid(id);
                    var result = tariffs.Where(item => item.Id == tariffId);
                    if(!result.Any())
                    {
                        throw new Exception();
                    }
                    user.TariffId = tariffId;
                    usersModel.AddUser(user);
                }
                else
                {
                    usersModel.AddUser(user);
                }
                Console.Clear();
            }
            catch(Exception)
            {
                Console.Clear();
                Console.WriteLine("Invalid data");
            }
        }

        public void UpdateUser(List<User> users)
        {
            try
            {
                viewer.ShowUsers(users);
                Console.WriteLine("\nEnter id of desire user:");
                string id = Console.ReadLine();
                Guid userId = new Guid(id);
                var result = users.Where(item => item.Id == userId);
                if(!result.Any())
                {
                    throw new Exception();
                }
                User user = result.First();

                Console.WriteLine("Change name:");
                string name = viewer.ReadLine(user.Name);
                if(name == "")
                {
                    throw new Exception();
                }

                Console.WriteLine("Change telephone number:");
                string tel = viewer.ReadLine(user.TelephoneNumber);
                if(tel == "")
                {
                    throw new Exception();
                }

                Console.WriteLine("Change funds:");
                int funds = int.Parse(viewer.ReadLine(user.Funds.ToString()));
                List<Tariff> tariffs = tariffsModel.GetTariffs();
                viewer.ShowTariffs(tariffs);
                Console.WriteLine("\nChange tariff id:");
                id = viewer.ReadLine(user.TariffId.ToString());
                user.Name = name;
                user.TelephoneNumber = tel;
                user.Funds = funds;
                if(id != "")
                {
                    Guid tariffId = new Guid(id);
                    var tariff = tariffs.Where(item => item.Id == tariffId);
                    if(!tariff.Any())
                    {
                        throw new Exception();
                    }
                    user.TariffId = tariffId;
                    usersModel.UpdateUser(user);
                }
                else
                {
                    usersModel.UpdateUser(user);
                }
                Console.Clear();
            }
            catch(Exception)
            {
                Console.Clear();
                Console.WriteLine("Invalid data");
            }
        }

        public void DeleteUser(List<User> users)
        {
            viewer.ShowUsers(users);
            Console.WriteLine("\nEnter id of desire user:");
            string id = Console.ReadLine();
            try
            {
                Guid userId = new Guid(id);
                var result = users.Where(item => item.Id == userId);
                if(!result.Any())
                {
                    throw new Exception();
                }
                usersModel.DeleteUser(userId);
                Console.Clear();
            }
            catch(Exception)
            {
                Console.Clear();
                Console.WriteLine("Wrong Id");
            }
        }

        public List<User> FilterUsers()
        {
            Console.WriteLine("Enter name or skip:");
            string name = Console.ReadLine();
            Console.WriteLine("Enter telephone number or skip:");
            string tel = Console.ReadLine();
            Console.WriteLine("Tariff const is less than:");
            string cost = Console.ReadLine();
            Console.Clear();
            try
            {
                int num = int.Parse(cost);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                List<User> users = usersModel.GetFilteredUsers(name, tel, num);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.Clear();
                Console.WriteLine($"Execution time:{elapsedMs}");
                return users;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}