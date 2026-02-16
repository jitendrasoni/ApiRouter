using ApiRefactor.Models;
using ApiRefactor.Repositories;
using ApiRefactor.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IWaveRepository, WaveRepository>();
builder.Services.AddScoped<IWaveService, WaveService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler("/error");

app.Map("/error", () =>
{
    return Results.Problem(
        title: "Unexpected error",
        detail: "An unexpected error occurred while processing the request",
        statusCode: 500
    );
});

app.UseHttpsRedirection();

app.MapGet("/api/waves", async (IWaveService service) =>
{
    var waves = await service.GetWavesAsync();
    return Results.Ok(waves);
});

app.MapGet("/api/waves/{id}", async (Guid id, IWaveService service) =>
{
    var wave = await service.GetWaveAsync(id);
    return wave is null
        ? Results.NotFound(new { message = $"Wave {id} not found" })
        : Results.Ok(wave);
});

app.MapPost("/api/waves", async (Wave wave, IWaveService service) =>
{
    if (wave == null)
        return Results.BadRequest("Request body is required");

    if (string.IsNullOrWhiteSpace(wave.Name))
        return Results.BadRequest("Wave name is required");

    try
    {
        var isNew = wave.Id == Guid.Empty;
        if (isNew)
            wave.Id = Guid.NewGuid();

        await service.UpsertWaveAsync(wave);

        return Results.Created($"/api/waves/{wave.Id}", wave);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();
