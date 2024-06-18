// See https://aka.ms/new-console-template for more information

using Bogus;
using ClefViewer.DataGenerator;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

var logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.File(new CompactJsonFormatter(), "./logs/logs.json")
    .CreateLogger();

var userIds = 0;
var userFaker = new Faker<User>();
userFaker.CustomInstantiator(f => new User
{
    Id = userIds++
}).RuleFor(u => u.Email, f => f.Internet.Email()).RuleFor(u => u.Name, f => f.Person.FullName);

var thingFaker = new Faker<Thing>();
thingFaker.RuleFor(t => t.Subject, f => f.Commerce.ProductDescription())
    .RuleFor(t => t.Value, f => f.Random.Number(0, 100));

var messageFaker = new Faker<Message>()
    .RuleFor(m => m.Type, f => f.PickRandom<MessageType>())
    .FinishWith((f, m) =>
    {
        var user = userFaker.Generate();
        var thing = thingFaker.Generate();
        switch (m.Type)
        {
            case MessageType.UserCreated:
                logger.Write(LogEventLevel.Debug, "User {@User} created", user);
                break;
            case MessageType.Thing:
                logger.Write(LogEventLevel.Verbose, "Thing {Subject} has value {Value}", thing.Subject, thing.Value);
                break;
            case MessageType.DidThing:
                logger.Write(LogEventLevel.Information, "User {Name} did thing {Thing}", user.Name, thing.Subject);
                break;
            case MessageType.ThingFailed:
                logger.Write(LogEventLevel.Warning, "User {Name} failed to do thing {Thing}", user.Name, thing.Subject);
                break;
            case MessageType.ThingExploded:
                logger.Write(LogEventLevel.Error,
                    new Exception($"Something terrible has happened to thing with value {thing.Value}"),
                    "Thing {Thing} exploded!!!", thing.Subject);
                break;
        }
    });

var messages = messageFaker.GenerateBetween(100, 200);
Console.WriteLine($"Generated {messages.Count} messages");