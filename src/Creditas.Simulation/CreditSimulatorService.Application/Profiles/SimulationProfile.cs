using AutoMapper;
using CreditSimulatorService.Application.DTOs;
using CreditSimulatorService.Domain.Mongo;

namespace CreditSimulatorService.Application.Profiles
{
    public class SimulationProfile : Profile
    {
        public SimulationProfile()
        {
            // Entity → DTO
            CreateMap<LoanSimulationDocument, LoanSimulationResponseDto>();
        }
    }
}
