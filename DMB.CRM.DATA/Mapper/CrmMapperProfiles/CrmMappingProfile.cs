using AutoMapper;
using Dmb.Crm.Data.Entities;
using Dmb.Crm.Model.Dtos.Auth;
using Dmb.Crm.Model.Dtos.Company;
using Dmb.Crm.Model.Dtos.Contact;
using Dmb.Crm.Model.Dtos.Location;
using Dmb.Crm.Model.Dtos.Opportunity;
using Dmb.Crm.Model.Dtos.Pipeline;
using Dmb.Crm.Model.Dtos.Tag;

namespace Dmb.Crm.Data.Mapper.CrmMapperProfiles;

public class CrmMappingProfile : Profile
{
    public CrmMappingProfile()
    {
        CreateMap<CrmUser, LoggedInUserDto>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.Id));

        CreateMap<Location, ReadLocationDto>();

        CreateMap<Company, ReadCompanyDto>();
        CreateMap<CreateCompanyDto, Company>();

        CreateMap<Tag, ReadTagDto>();
        CreateMap<Tag, ReadTagRefDto>();
        CreateMap<CreateTagDto, Tag>();

        CreateMap<PipelineStage, ReadPipelineStageDto>();
        CreateMap<Pipeline, ReadPipelineDto>();

        CreateMap<Opportunity, ReadOpportunityDto>()
            .ForMember(d => d.PipelineName, o => o.MapFrom(s => s.Pipeline.Name))
            .ForMember(d => d.StageName, o => o.MapFrom(s => s.Stage.Name))
            .ForMember(d => d.SortOrder, o => o.MapFrom(s => s.Stage.SortOrder));

        CreateMap<Contact, ReadContactDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => $"{s.FirstName} {s.LastName}".Trim()))
            .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company != null ? s.Company.Name : null))
            .ForMember(d => d.Tags, o => o.MapFrom(s => s.ContactTags.Select(t => t.Tag)));
    }
}
