using AutoMapper;
using UphReport.Entities;
using UphReport.Entities.Wave;
using UphReport.Models.User;
using UphReport.Models.Wave;

namespace UphReport.AutoMapper;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<WaveAKRequest, WaveAPIKey>();

        CreateMap<RegisterUser, User>();
    }
}
