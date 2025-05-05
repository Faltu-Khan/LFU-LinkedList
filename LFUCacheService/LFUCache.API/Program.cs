using LFUCache.Library.Interfaces;
using LFU = LFUCache.Library.Services.LFUCache;

var builder = WebApplication.CreateBuilder(args);

// Read cache capacity from command-line args or use default = 10
int capacity = 10;

var capacityArg = builder.Configuration.GetValue<string>("capacity"); // Get from `--capacity`
if (int.TryParse(capacityArg, out var parsed))
{
    capacity = parsed;
}

// Register LFUCache with resolved capacity
builder.Services.AddSingleton<ICache<int, string>>(new LFU(capacity));

builder.Services.AddControllers();
var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.Run();
