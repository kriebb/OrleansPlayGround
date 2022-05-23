using Orleans;

namespace HelloWorld_API;

public class IdGeneratorActor : Grain, IIdGeneratorActor
{
    public int _nextId;
    public Task<int> Next()
    {
        return Task.FromResult(_nextId++);
    }
}