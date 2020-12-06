using System;
using System.Collections.Generic;
using System.Linq;

namespace lab3
{
    public class CallsController
    {
        Users usersModel;
        PhoneCalls callsModel;
        Viewer viewer;
        public CallsController(Users users, PhoneCalls calls, Viewer viewer)
        {
            usersModel = users;
            callsModel = calls;
            this.viewer = viewer;
        }

        public void AddCall()
        {
            try
            {
                Console.WriteLine("Enter duration:");
                int duration = int.Parse(Console.ReadLine());
                List<User> users = usersModel.GetUsers();
                viewer.ShowUsers(users);
                Console.WriteLine("\nEnter id of first user:");
                string id = Console.ReadLine();
                Guid userIdFirst = new Guid(id);
                var result = users.Where(item => item.Id == userIdFirst);
                if(!result.Any())
                {
                    throw new Exception();
                }

                Console.WriteLine("\nEnter id of second user:");
                id = Console.ReadLine();
                Guid userIdSecond = new Guid(id);
                result = users.Where(item => item.Id == userIdSecond);
                if(!result.Any() || userIdFirst == userIdSecond)
                {
                    throw new Exception();
                }
                Call call = new Call();
                call.FirstUserId = userIdFirst;
                call.SecondUserId = userIdSecond;
                call.Duration = duration;
                callsModel.AddPhoneCall(call);
                Console.Clear();
            }
            catch(Exception)
            {
                Console.Clear();
                Console.WriteLine("Invalid data");
            }
        }

        public void UpdateCall(List<Call> calls)
        {
            try
            {
                viewer.ShowPhoneCalls(calls);
                Console.WriteLine("\nEnter id of desire call:");
                string id = Console.ReadLine();
                
                Guid callId = new Guid(id);
                var result = calls.Where(item => item.Id == callId);
                if(!result.Any())
                {
                    throw new Exception();
                }
                Call call = result.First();

                Console.WriteLine("Change duration:");
                int duration = int.Parse(viewer.ReadLine(call.Duration.ToString()));
                List<User> users = usersModel.GetUsers();
                viewer.ShowUsers(users);
                Console.WriteLine("\nChange id of first user:");
                id = viewer.ReadLine(call.FirstUserId.ToString());
                Guid userIdFirst = new Guid(id);
                var user = users.Where(item => item.Id == userIdFirst);
                if(!user.Any())
                {
                    throw new Exception();
                }

                Console.WriteLine("\nChange id of second user:");
                id = viewer.ReadLine(call.SecondUserId.ToString());
                Guid userIdSecond = new Guid(id);
                user = users.Where(item => item.Id == userIdSecond);
                if(!user.Any() || userIdFirst == userIdSecond)
                {
                    throw new Exception();
                }
                call.FirstUserId = userIdFirst;
                call.SecondUserId = userIdSecond;
                call.Duration = duration;
                callsModel.UpdatePhoneCall(call);
                Console.Clear();
            }
            catch(Exception)
            {
                Console.Clear();
                Console.WriteLine("Invalid data");
            }
        }

        public void DeleteCall(List<Call> calls)
        {
            viewer.ShowPhoneCalls(calls);
            Console.WriteLine("\nEnter id of desire call:");
            string id = Console.ReadLine();;
            try
            {
                Guid callId = new Guid(id);
                var result = calls.Where(item => item.Id == callId);
                if(!result.Any())
                {
                    throw new Exception();
                }
                callsModel.DeletePhoneCall(callId);
                Console.Clear();
            }
            catch(Exception)
            {
                Console.Clear();
                Console.WriteLine("Wrong Id");
            }
        }

        public List<(string, string, string, string)> FilterCalls()
        {
            Console.WriteLine("Enter first name or skip:");
            string name1 = Console.ReadLine();
            Console.WriteLine("Enter first telephone or skip:");
            string tel1 = Console.ReadLine();
            Console.WriteLine("Enter second name or skip:");
            string name2 = Console.ReadLine();
            Console.WriteLine("Enter secong telephone or skip:");
            string tel2 = Console.ReadLine();
            Console.Clear();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var calls = callsModel.GetFilteredCalls(name1, tel1, name2, tel2);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.Clear();
            Console.WriteLine($"Execution time:{elapsedMs}");
            return calls;
        }
    }
}