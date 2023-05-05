using AutoMapper;
using UphReport.Entities.Wave;
using UphReport.Models.Wave;

namespace UphReport.AutoMapper;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<WaveAKRequest, WaveAPIKey>();
	}
}
