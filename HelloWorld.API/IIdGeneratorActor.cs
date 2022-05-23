using Orleans;

namespace HelloWorld_API;

public interface IIdGeneratorActor : IGrainWithStringKey
{
    Task<int> Next();
}