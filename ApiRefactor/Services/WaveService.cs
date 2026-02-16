using ApiRefactor.Models;
using ApiRefactor.Repositories;

namespace ApiRefactor.Services;

public class WaveService : IWaveService
{
    private readonly IWaveRepository _repo;

    public WaveService(IWaveRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Wave>> GetWavesAsync()
        => _repo.GetAllAsync();

    public Task<Wave?> GetWaveAsync(Guid id)
        => _repo.GetByIdAsync(id);

    public Task UpsertWaveAsync(Wave wave)
    {
        if (string.IsNullOrWhiteSpace(wave.Name))
            throw new ArgumentException("Wave name required");

        return _repo.UpsertAsync(wave);
    }
}
