using Orleans;

namespace HelloWorld_API;

public interface IContributerActor : IGrainWithIntegerKey
{
    public void SetId(int id);
    public Task<int> GetId();

    Task<Beer?> TakeFrom(string fridgeCountry);
    Task PutIn(string fridgeCountry, Beer beer);
    void SetName(string name);
}