using ApiRefactor.Models;

namespace ApiRefactor.Repositories;

public interface IWaveRepository
{
    Task<List<Wave>> GetAllAsync();
    Task<Wave?> GetByIdAsync(Guid id);
    Task UpsertAsync(Wave wave);
}
