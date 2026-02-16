using ApiRefactor.Models;
using ApiRefactor.Repositories;
using Microsoft.Extensions.Logging;

namespace ApiRefactor.Services;

public class WaveService : IWaveService
{
    private readonly IWaveRepository _repo;
    private readonly ILogger<WaveService> _logger;

    public WaveService(IWaveRepository repo, ILogger<WaveService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<Wave>> GetWavesAsync()
    {
        _logger.LogInformation("Fetching all waves");
        return await _repo.GetAllAsync();
    }

    public async Task<Wave?> GetWaveAsync(Guid id)
    {
        _logger.LogInformation("Fetching wave {WaveId}", id);
        return await _repo.GetByIdAsync(id);
    }

    public async Task UpsertWaveAsync(Wave wave)
    {
        if (string.IsNullOrWhiteSpace(wave.Name))
        {
            _logger.LogWarning("Wave validation failed: Name missing");
            throw new ArgumentException("Wave name required");
        }

        _logger.LogInformation("Saving wave {WaveId} with name {WaveName}", wave.Id, wave.Name);
        await _repo.UpsertAsync(wave);
    }
}
