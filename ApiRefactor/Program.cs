using ApiRefactor.Models;
using ApiRefactor.Repositories;
using ApiRefactor.Services;

var builder = WebApplication.CreateBuilder(args);

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
    return wave is null ? Results.NotFound() : Results.Ok(wave);
});

app.MapPost("/api/waves", async (Wave wave, IWaveService service) =>
{
    await service.UpsertWaveAsync(wave);
    return Results.Ok();
});

app.Run();
