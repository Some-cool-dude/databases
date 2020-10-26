using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace lab2
{
    using FilteredCalls = System.Tuple<string, string, string, string>;
    public class Controller
    {
        private Users usersModel;
        private Tariffs tariffsModel;
        private PhoneCalls callsModel;

        private Viewer viewer;
        NpgsqlConnection con;

        UsersController usersController;
        TariffsController tariffsController;
        CallsController callsController;
        
        public Controller()
        {
            var cs = "Host=localhost;Username=shylo;Password=shylo;Database=lab1";
            con = new NpgsqlConnection(cs);
            con.Open();

            usersModel = new Users(con);
            tariffsModel = new Tariffs(con);
            callsModel = new PhoneCalls(con);
            viewer = new Viewer();
            usersController = new UsersController(usersModel, tariffsModel, viewer);
            tariffsController = new TariffsController(tariffsModel, viewer);
            callsController = new CallsController(usersModel, callsModel, viewer);
        }

        private int GetOperation()
        {
            string op = Console.ReadLine();
            int operation = -1;
            try
            {
                return operation = int.Parse(op);
            }
            catch(Exception)
            {
                return operation = -1;
            }
        }

        private void Generate(string type)
        {
            Console.WriteLine("Enter item`s quantity:");
            try
            {
                int n = int.Parse(Console.ReadLine());
                if(n > 0)
                {
                    if(type == "users")
                    {
                        usersModel.Generate(n);
                    }
                    else if(type == "tariffs")
                    {
                        tariffsModel.Generate(n);
                    }
                    else
                    {
                        callsModel.Generate(n);
                    }
                    Console.Clear();
                }
            }
            catch(Exception)
            {
                Console.Clear();
                Console.WriteLine("Invalide number");
            }
        }

        public void Start()
        {
            bool exit = false;
            while(!exit)
            {
                viewer.Models();
                int operation = GetOperation();            
                Console.Clear();
                switch(operation)
                {
                    case 0:
                        exit = true;
                        break;
                    case 1:
                        Users();
                        break;
                    case 2:
                        Tariffs();
                        break;
                    case 3:
                        PhoneCalls();
                        break;
                }
            }
        }

        private void Users()
        {
            bool exit = false;
            List<User> filtered = null;
            List<User> users = null;
            while(!exit)
            {
                if(filtered == null)
                {
                    users = usersModel.GetUsers();
                    viewer.ShowUsers(users);
                }
                else
                {
                    viewer.ShowFilterdUsers(filtered);
                }
                Console.WriteLine("");
                viewer.Operations();
                int operation = GetOperation();
                Console.Clear();
                filtered = null;
                switch(operation)
                {
                    case 0:
                        exit = true;
                        break;
                    case 1:
                        usersController.AddUser();
                        break;
                    case 2:
                        usersController.UpdateUser(users);
                        break;
                    case 3:
                        usersController.DeleteUser(users);
                        break;
                    case 4:
                        Generate("users");
                        break;
                    case 5:
                        filtered = usersController.FilterUsers();
                        break;
                }
            }
        }

        private void Tariffs()
        {
            bool exit = false;
            List<Tariff> filtered = null;
            List<Tariff> tariffs = null;
            while(!exit)
            {
                if(filtered == null)
                {
                    tariffs = tariffsModel.GetTariffs();
                    viewer.ShowTariffs(tariffs);
                }
                else
                {
                    viewer.ShowFilteredTariffs(filtered);
                }
                Console.WriteLine("");
                viewer.Operations();
                int operation = GetOperation();
                Console.Clear();
                filtered = null;
                switch(operation)
                {
                    case 0:
                        exit = true;
                        break;
                    case 1:
                        tariffsController.AddTariff();
                        break;
                    case 2:
                        tariffsController.UpdateTariff(tariffs);
                        break;
                    case 3:
                        tariffsController.DeleteTariff(tariffs);
                        break;
                    case 4:
                        Generate("tariffs");
                        break;
                    case 5:
                        filtered = tariffsController.FilterTariffs();
                        break;
                }
            }
        }

        private void PhoneCalls()
        {
            bool exit = false;
            List<FilteredCalls> filtered = null;
            List<Call> calls = null;
            while(!exit)
            {
                if(filtered == null)
                {
                    calls = callsModel.GetPhoneCalls();
                    viewer.ShowPhoneCalls(calls);
                }
                else
                {
                    viewer.ShowFilteredCalls(filtered);
                }
                Console.WriteLine("");
                viewer.Operations();
                int operation = GetOperation();
                Console.Clear();
                filtered = null;
                switch(operation)
                {
                    case 0:
                        exit = true;
                        break;
                    case 1:
                        callsController.AddCall();
                        break;
                    case 2:
                        callsController.UpdateCall(calls);
                        break;
                    case 3:
                        callsController.DeleteCall(calls);
                        break;
                    case 4:
                        Generate("calls");
                        break;
                    case 5:
                        filtered = callsController.FilterCalls();
                        break;
                }
            }
        }
    }
}