using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace hwork7
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (AtistContext context = new AtistContext())
            {
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();


                Console.Write("Enter name: ");
                string name = Console.ReadLine();
                Console.Write("Enter country: ");
                string country = Console.ReadLine();
                Console.Write("Enter city: ");
                string city = Console.ReadLine();
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "HW7 application");
                    HttpResponseMessage searchResponse =
                        await client.GetAsync($"https://nominatim.openstreetmap.org/search?country={country}&city={city}&format=jsonv2");
                    if (searchResponse.IsSuccessStatusCode)
                    {
                        if (searchResponse.Content.Headers.ContentLength != 2)
                        {
                            List<SearchJson> elsements = await searchResponse.Content.ReadFromJsonAsync<List<SearchJson>>();

                            if (elsements.Count() > 0 && elsements.First().type == "city")
                            {
                                IQueryable<Customer> users = from user in context.Customers
                                                             where (user.Name == name)
                                                             select user;
                                if (users.Count() != 0)
                                {
                                    Console.WriteLine("User is exists");
                                }
                                else
                                {
                                    IDbContextTransaction transaction =
                                        await context.Database.BeginTransactionAsync();
                                    try
                                    {
                                        Customer newCustomer = new Customer()
                                        { Name = name, Country = country, City = city };
                                        context.Customers.Add(newCustomer);
                                        await transaction.CommitAsync();
                                        await context.SaveChangesAsync();
                                        Console.WriteLine("SUCCESS");
                                    }
                                    catch
                                    {
                                        Console.WriteLine("FAILED");
                                    }

                                }
                            }
                            else
                            {
                                Console.WriteLine("Bad city");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Bad country");
                        }
                    }
                    else
                    {
                        string resp = await searchResponse.Content.ReadAsStringAsync();
                        Console.WriteLine(resp);
                    }
                }
            }
        }
    }
}
