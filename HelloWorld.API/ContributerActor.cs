using System.Drawing;
using Orleans;

namespace HelloWorld_API;

public class ContributerActor : Grain, IContributerActor
{
    private readonly IGrainFactory _grainFactory;
    private int _id;
    private string _name;

    public ContributerActor(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    public void SetId(int id)
    {
        _id = id;
    }

    public Task<int> GetId()
    {
        return Task.FromResult(_id);
    }

    public async Task<Beer?> TakeFrom(string fridgeCountry)
    {
        var fridgeActor = _grainFactory.GetGrain<IFridgeActor>(fridgeCountry);
        var beer = (await fridgeActor.GetBeers()).SingleOrDefault(); //Datastore
        if (beer != null)
        {
            (await fridgeActor.GetBeers()).Remove(beer);
            Colorful.Console.WriteLine("[Contributer {0}]: Beer removed from fridge " + fridgeCountry,  _id, Color.Orange);

        }
        else
        {
            Colorful.Console.WriteLine("[Contributer {0}]: No more beers found in fridge " + fridgeCountry,  _id, Color.Red);
        }


        return beer;
    }

    public async Task PutIn(string fridgeCountry, Beer beer)
    {
        Colorful.Console.WriteLine("[Contributer {0}]: Added beer from " + beer.Supplier + " in the fridge " + fridgeCountry,  _id, Color.Orange);
        var fridgeActor = _grainFactory.GetGrain<IFridgeActor>(fridgeCountry);
        fridgeActor.AddBeer(beer); //Datastore
        Colorful.Console.WriteLine("[Contributer {0}]: FridgeActor " + fridgeCountry + " number of beers:" + (await fridgeActor.GetBeers()).Count,  _id, Color.White);
    }

    public void SetName(string name)
    {
        _name = name;
    }
}