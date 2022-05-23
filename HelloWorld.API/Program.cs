// Program.cs

using Orleans;
using Orleans.Hosting;


namespace HelloWorld_API;

public static class Program
{


    public static void Main(string[] args)

    {

        var builder = WebApplication.CreateBuilder(args);

        // create the silo host
        builder.Host
            .UseOrleans(b =>
                {

                    b.ConfigureServices(services =>
                    {
                        services.AddTransient<IFridgeActor, FridgeActor>();
                        services.AddTransient<IContributerActor, ContributerActor>();
                        services.AddTransient<IIdGeneratorActor, IdGeneratorActor>();
                    });
                    b.ConfigureApplicationParts(c => c.AddFromAppDomain());
                    b.UseLocalhostClustering();

                }
                )
            .ConfigureServices(services =>
            {
                services.AddTransient<IFridgeActor, FridgeActor>();
                services.AddTransient<IContributerActor, ContributerActor>();
                services.AddTransient<IIdGeneratorActor, IdGeneratorActor>();

                services.AddControllers();
                services.AddEndpointsApiExplorer();
                services.AddSwaggerGen();
            })
            .ConfigureLogging(b => b.AddConsole());
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();





    }
}
