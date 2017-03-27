using BigMountainX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;
using System.ServiceModel;

namespace ProfileGenerator
{


    class Program : IProfileGenerator
    {
        static AppState State { get; set; }


        public List<string> GetMailingList(string file_name)
        {
            WriteLine("Retrieving...");
            string profile_path = $"../../{file_name}.xml";
            var app_state = AppState.FromFileAsync(profile_path);
            WriteLine($"{app_state.MailingList.Count} items found.");
            return app_state.MailingList;
        }

        static void Main(string[] args)
        {
            ServiceHost host = null;
            while (true)
            {
                Write(">");
                var input = ReadLine().Trim().ToLower();
                if (string.IsNullOrWhiteSpace(input))
                    break;
                if (input == "load")
                {
                    Write("File name>");
                    input = ReadLine().Trim().ToLower();
                    string bio_path = $"../../{input}.xml";

                    State = AppState.FromFileAsync(bio_path);
                    WriteLine($"Loaded {input}.xml");
                }
                else if (input == "end")
                    break;
                else if (input == "clear")
                {
                    State.Delete();
                }
                else if (input == "generate")
                {
                    Write("Number of records>");
                    input = ReadLine().Trim().ToLower();
                    int count;
                    if (int.TryParse(input, out count))
                    {
                        GenerateProfile(count);
                        WriteLine("Generated");
                    }
                    else
                    {
                        WriteLine($"I dont understand '{input}'.");
                        WriteLine("Enter 'end' or a number.\n");
                    }
                }
                else if (input == "publish")
                {
                    if (host == null)
                    {
                        host = new ServiceHost(typeof(Program));
                        NetTcpBinding tcp_binding = new NetTcpBinding(SecurityMode.None);
                        var address = "net.tcp://localhost:9999/profile-generator";
                        host.AddServiceEndpoint(typeof(IProfileGenerator), tcp_binding, address);
                        host.Open();
                    }
                }
                else
                {
                    WriteLine($"I dont understand '{input}'.");
                }
            }
        }

        private static bool GenerateProfile(int number_of_records)
        {

            // State.UserProfile = null;
            if (State == null)
            {
                WriteLine("call load first.");
                return false;
            }

            //create sample customers
            State.Customers = new List<BMXCustomerInfo>();
            int dob_years = (number_of_records % 20); //keep it to a max of 20 years
            foreach (var item_id in Enumerable.Range(0, number_of_records))
            {
                State.Customers.Add(new BMXCustomerInfo
                {
                    CustomerID = Guid.NewGuid(),
                    CustomerName = $"John Smith {item_id}",
                    DOB = DateTime.Now.Subtract(TimeSpan.FromDays(365 * dob_years)),
                    Email = $"test{item_id}@test.com",
                });
            }

            //create sample events
            foreach (var item_id in Enumerable.Range(0, number_of_records))
            {
                State.Events.Add(new BMXEvent
                {
                    Address = $"{item_id} test street",
                    Latitude = 40.7484,
                    Longitude = -73.9857,
                    CreateDate = DateTime.Now,
                    Duration = TimeSpan.FromHours(item_id % 5), //keep it to a max of 5 hours
                    EventID = Guid.NewGuid(),
                    Description = $"A night of wine and comedy-{item_id}",
                    EventTitle = $"Comedy Night at the Empire State-{item_id}",
                    StartDateTime = DateTime.Now.AddDays(item_id),
                    Feature = new BMXFeaturedPerformer
                    {
                        BIO = File.ReadAllText("../../artistbio.txt"),
                    },
                });
            }
            //next event
            State.NextEvent = State.Events.OrderBy(i => i.StartDateTime).FirstOrDefault();

            //create sample mailing list data
            foreach (var item_id in Enumerable.Range(0, number_of_records))
            {
                State.MailingList.Add($"patron{item_id}@test.com");
            }

            State.Save();
            return true;
        }
    }
}
