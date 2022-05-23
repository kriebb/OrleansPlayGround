using Orleans;

namespace HelloWorld_API;

public class FridgeActor : Grain, IFridgeActor
{
    private string  _country;
    private List<Beer> _beers = new List<Beer>();

    public void SetCountry(string country)
    {
        _country = country;
    }

    public Task<string> GetCountry()
    {
        return Task.FromResult(_country);
    }

    public void AddBeer(Beer beer)
    {
        _beers.Add(beer);
    }

    public Task<List<Beer>> GetBeers()
    {
        return Task.FromResult(_beers);
    }
}