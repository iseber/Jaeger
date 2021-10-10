using OpenTracing;
using Jaeger.Senders;
using Jaeger.Senders.Thrift;
using Jaeger;
using Jaeger.Samplers;
using OpenTracing.Util;
using Jaeger.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration
    .SetBasePath(Environment.CurrentDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Jaeger", Version = "v1" }));

builder.Services.AddTransient<WeatherForecastService>();

builder.Services.AddOpenTracing();
builder.Services.AddSingleton<ITracer>(serviceProvider =>
{
    var serviceName = serviceProvider.GetRequiredService<IWebHostEnvironment>().ApplicationName;
    var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

    // This is necessary to pick the correct sender, otherwise a NoopSender is used!
    Jaeger.Configuration.SenderConfiguration.DefaultSenderResolver = new SenderResolver(loggerFactory)
        .RegisterSenderFactory<ThriftSenderFactory>();

    // This will log to a default localhost installation of Jaeger.
    var tracer = new Tracer.Builder(serviceName)
        .WithLoggerFactory(loggerFactory)
        .WithSampler(new ConstSampler(true))
        .Build();

    // Allows code that can't use DI to also access the tracer.
    GlobalTracer.Register(tracer);
    //TraceAs.InitTracer(tracer, loggerFactory.CreateLogger<TraceAs>());

    return tracer;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Jaeger v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
