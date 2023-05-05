using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UphReport.Data;
using UphReport.Entities.Wave;
using UphReport.Exceptions;
using UphReport.Interfaces;
using UphReport.Models.Wave;

namespace UphReport.Services;

public class WaveAPIKeyService : IWaveAPIKeyService
{
	private readonly MyDbContext _myDbContext;
	private readonly IMapper _mapper;

	public WaveAPIKeyService(MyDbContext myDbContext, IMapper mapper)
	{
		_myDbContext = myDbContext;
		_mapper = mapper;
	}

	public async Task<bool> AddKey(WaveAKRequest key)
	{
		var waveAPIKey = _mapper.Map<WaveAPIKey>(key);
		await _myDbContext.WaveAPIKeys.AddAsync(waveAPIKey);
		var result = await _myDbContext.SaveChangesAsync();

		if (result > 0)
			return true;

		return false;
		//sprawdzic czy dziala
	}
	public async Task<string> GetAPIKey()
	{
		var result = await _myDbContext.WaveAPIKeys.FirstOrDefaultAsync(x => x.CreditsRemaining >= 3);
		if(result == null)
		{
			throw new NotFoundException("Not found any api key for Wave");
		}
		return result.APIKey;
	}
	public async Task DeleteDeprecatedKeys() 
	{
		var result = await _myDbContext.WaveAPIKeys.Where(x => x.CreditsRemaining < 3).ToListAsync();
		_myDbContext.WaveAPIKeys.RemoveRange(result);
		await _myDbContext.SaveChangesAsync();
	}
	public async Task UpdateKey(WaveAKUpdate waveAKUpdate)
	{
		var keyFromDB = await _myDbContext.WaveAPIKeys.FirstOrDefaultAsync(x => x.APIKey == waveAKUpdate.APIKey);

		if (keyFromDB == null)
			throw new NotFoundException("API Key not found");
		if (waveAKUpdate.CreditsRemaining < 0)
			throw new BadRequestException("Remaining Credits must be grather than 0");
		
		if(waveAKUpdate.CreditsRemaining >= 0 && waveAKUpdate.CreditsRemaining < 3)
		{
			_myDbContext.WaveAPIKeys.Remove(keyFromDB);
		}
		else
		{
            keyFromDB.CreditsRemaining = waveAKUpdate.CreditsRemaining;
            _myDbContext.WaveAPIKeys.Update(keyFromDB);
        }

		await _myDbContext.SaveChangesAsync();
	}
}
