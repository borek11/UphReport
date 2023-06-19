using UphReport.Entities.Wave;
using UphReport.Models.Wave;

namespace UphReport.Interfaces;

public interface IWaveAPIKeyService
{
    Task<bool> AddKey(WaveAKRequest key);
    Task<string> GetAPIKey();
    Task DeleteDeprecatedKeys();
    Task UpdateKey(WaveAKUpdate waveAKUpdate);
    Task<List<WaveAPIKey>> GetAll();
    Task<bool> DeleteById(Guid guid);
}