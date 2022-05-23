using System.Collections.Concurrent;
using System.Drawing;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace HelloWorld_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SimulateController : ControllerBase
    {
        private readonly IClusterClient _clusterClient;
        private string[] fridgeCountry = new[] { "nl", "de", "be" };
        private string[] whatToDoChoice = new[] { "take", "put" };

        private static readonly ConcurrentQueue<Beer> beers = new ConcurrentQueue<Beer>();

        public SimulateController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }
        [HttpGet]
        public void MimicFillingAndTakingBeerFromFridge()
        {
            var faker = new Faker();
            var task1 = Task.Run(async () =>
            {
                var suppliers = new string[] { "jupiler", "heineken" };

                while (true)
                {
                    var beer = new Beer();

                    beer.Supplier = faker.PickRandom(suppliers);

                    beers.Enqueue(beer);

                    Colorful.Console.WriteLine("[Supplier {0}]: Beer arrived. Total number of beers: {1}", beer.Supplier, beers.Count, Color.Orange);
                    await Task.Delay(faker.Random.Int(1000, 5000));
                }
            });
            var task2 = Task.Run(async () =>
            {

                int maxNumber = 70;
                int counter = 0;
                while (counter < maxNumber)
                {
                    var id = _clusterClient.GetGrain<IIdGeneratorActor>("ContributerIdGenerator").Next();
                    var contributer = _clusterClient.GetGrain<IContributerActor>(await id);
                    contributer.SetId(await id);
                    var name = faker.Name.FullName();
                    contributer.SetName(name);
                    Colorful.Console.WriteLine("[Contributer {0}]: {1} arrived.", await id, name, Color.Blue);
                    counter++;

                    await Task.Delay(faker.Random.Int(1000, 5000));
                }
            });

            var task3 = Task.Run(() =>
           {
               var fridgeNlActor = _clusterClient.GetGrain<IFridgeActor>("nl");
               fridgeNlActor.SetCountry("nl");
               var fridgeDeActor = _clusterClient.GetGrain<IFridgeActor>("de");
               fridgeDeActor.SetCountry("de");

               var fridgeBeActor = _clusterClient.GetGrain<IFridgeActor>("be");
               fridgeBeActor.SetCountry("be");

               Colorful.Console.WriteLine("Fridges arrived", Color.Green);

           });
            var task4 = Task.Run(async () =>
            {
                while (true)
                {
                    var contributerId = faker.Random.Int(0, 70);
                    var contributer = _clusterClient.GetGrain<IContributerActor>(contributerId);
                    var fridgeChoice = faker.PickRandom(fridgeCountry);

                    var whatToDo = faker.PickRandom(whatToDoChoice);
                    switch (whatToDo)
                    {
                        case "take":
                            var coldBeer = await contributer.TakeFrom(fridgeChoice);

                            break;
                        case "put":
                            if (beers.TryDequeue(out var warmBeer))
                            {
                                await contributer.PutIn(fridgeChoice, warmBeer);
                            }
                            else
                            {
                                Colorful.Console.WriteLine("[Contributer {0}]: Is thirsty. No more in the staging area",
                                    await contributer.GetId(),Color.Red);
                            }

                            break;
                    }

                    await Task.Delay(faker.Random.Int(1000, 5000));
                }
            });

            Task.WaitAll(task1, task2, task3, task4);
        }
    }
}
