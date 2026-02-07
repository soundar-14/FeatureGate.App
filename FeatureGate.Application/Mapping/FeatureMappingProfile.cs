using AutoMapper;
using FeatureGate.Application.DTOs.Evaluations;
using FeatureGate.Application.DTOs.FeatureOverrides;
using FeatureGate.Application.DTOs.Features;
using FeatureGate.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace FeatureGate.Application.Mapping
{
    [ExcludeFromCodeCoverage]
    public class FeatureMappingProfile : Profile
    {
        public FeatureMappingProfile()
        {

            CreateMap<Feature, FeatureDto>().ReverseMap();

            CreateMap<FeatureOverride, FeatureOverrideDto>()
            .ReverseMap();

            CreateMap<(bool Result, string EvaluatedBy), FeatureEvaluationResultDto>()
               .ForMember(d => d.IsEnabled, o => o.MapFrom(s => s.Result))
               .ForMember(d => d.EvaluatedBy, o => o.MapFrom(s => s.EvaluatedBy));
        }
    }
}
