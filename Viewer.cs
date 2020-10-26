using System;
using System.Collections.Generic;

namespace lab2
{
    using FilteredCalls = System.Tuple<string, string, string, string>;
    public class Viewer
    {
        public string ReadLine(string Default)
        {
            int pos = Console.CursorLeft;
            Console.Write(Default);
            ConsoleKeyInfo info;
            List<char> chars = new List<char> ();
            if (string.IsNullOrEmpty(Default) == false) {
                chars.AddRange(Default.ToCharArray());
            }

            while (true)
            {
                info = Console.ReadKey(true);
                if (info.Key == ConsoleKey.Backspace && Console.CursorLeft > pos)
                {
                    chars.RemoveAt(chars.Count - 1);
                    Console.CursorLeft -= 1;
                    Console.Write(' ');
                    Console.CursorLeft -= 1;

                }
                else if (info.Key == ConsoleKey.Enter) { Console.Write(Environment.NewLine); break; }
                else if (char.IsLetterOrDigit(info.KeyChar))
                {
                    Console.Write(info.KeyChar);
                    chars.Add(info.KeyChar);
                }
            }
            return new string(chars.ToArray ());
        }
        public void Operations()
        {
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Add");
            Console.WriteLine("2. Update");
            Console.WriteLine("3. Delete");
            Console.WriteLine("4. Generate");
            Console.WriteLine("5. Filter");
            Console.WriteLine("\nEnter operation:");
        }

        public void Models()
        {
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Users");
            Console.WriteLine("2. Tariffs");
            Console.WriteLine("3. Phone calls");
            Console.WriteLine("\nEnter operation:");
        }

        public void ShowUsers(List<User> users)
        {
            Console.WriteLine($"{"Id", -36} {"Name", -20} {"Telephone", -10} {"Funds", -4} {"TariffID"}");
            foreach(User user in users)
            {
                Console.WriteLine(user.ToString());
            }
        }

        public void ShowFilterdUsers(List<User> users)
        {
            Console.WriteLine($"{"Name", -20} {"Telephone", -10} {"Cost", -4}");
            foreach(User user in users)
            {
                Console.WriteLine($"{user.Name, -20} {user.TelepnoneNumber, -10} {user.Funds, -4}");
            }
        }

        public void ShowTariffs(List<Tariff> tariffs)
        {
            Console.WriteLine($"{"Id", -36} {"Name", -20} {"Cost", -4}");
            foreach(Tariff tariff in tariffs)
            {
                Console.WriteLine(tariff.ToString());
            }
        }

        public void ShowFilteredTariffs(List<Tariff> tariffs)
        {
            ShowTariffs(tariffs);
        }

        public void ShowPhoneCalls(List<Call> calls)
        {
            Console.WriteLine($"{"Id", -36} {"Duration", -4} {"FirstUserId", -36} {"SecondUserId"}");
            foreach(Call call in calls)
            {
                Console.WriteLine(call.ToString());
            }
        }

        public void ShowFilteredCalls(List<FilteredCalls> calls)
        {
            Console.WriteLine($"{"Name1", -20}, {"Telephone1", -10}, {"Name2", -20}, {"Telephone2", -10}");
            foreach(FilteredCalls call in calls)
            {
                Console.WriteLine($"{call.Item1, -20}, {call.Item2, -10}, {call.Item3, -20}, {call.Item4, -10}");
            }
        }
    }
}