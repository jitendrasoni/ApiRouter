using ApiRefactor.Models;

namespace ApiRefactor.Services;

public interface IWaveService
{
    Task<List<Wave>> GetWavesAsync();
    Task<Wave?> GetWaveAsync(Guid id);
    Task UpsertWaveAsync(Wave wave);
}
