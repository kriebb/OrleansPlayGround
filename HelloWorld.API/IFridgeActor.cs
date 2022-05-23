using Orleans;

namespace HelloWorld_API;

public interface IFridgeActor: IGrainWithStringKey
{
    void SetCountry(string country);
    Task<string> GetCountry();
    void AddBeer(Beer beer);
    Task<List<Beer>> GetBeers();
}