using AutoMapper;
using GivingChampion.Common.DTO.ServiceRequestDto;
using GivingChampion.Common.DTO.VolunteerOrder;
using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
namespace GivingChampion.Application.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {

        public MappingProfile()
        {
            #region ServiceRequest Mapping

            // Map ServiceRequest to ServiceRequestDto
            // Map ServiceRequest to ServiceRequestDto
            CreateMap<ServiceRequest, ServiceRequestDto>()
               .ForMember(dest => dest.FullName,
                           opt => opt.MapFrom(src => src.Partner != null ? src.Partner.OrgName : string.Empty))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.BriefDescription,
                    opt => opt.MapFrom(src => src.BriefDescription ?? string.Empty));

            // Map CreateServiceRequestDto to ServiceRequest
            CreateMap<CreateServiceRequestDto, ServiceRequest>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom(src => src.Title.Trim()))
                .ForMember(dest => dest.RequiredSkill,
                    opt => opt.MapFrom(src => src.RequiredSkill.Trim()))
                .ForMember(dest => dest.BriefDescription,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.BriefDescription)
                        ? string.Empty
                        : src.BriefDescription.Trim()))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(_ => RequestStatus.Pending))
                .ForMember(dest => dest.IsDeleted,
                    opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.DeletedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Partner,
                    opt => opt.Ignore());

            // Map UpdateServiceRequestDto to ServiceRequest
            CreateMap<UpdateServiceRequestDto, ServiceRequest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom(src => src.Title.Trim()))
                .ForMember(dest => dest.RequiredSkill,
                    opt => opt.MapFrom(src => src.RequiredSkill.Trim()))
                .ForMember(dest => dest.BriefDescription,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.BriefDescription)
                        ? string.Empty
                        : src.BriefDescription.Trim()))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PartnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Partner, opt => opt.Ignore());

            #endregion

            #region VolunteerOrder Mapping

            // Map VolunteerOrder to VolunteerOrderDto
            CreateMap<VolunteerOrder, VolunteerOrderDto>()
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status.ToString()));

            // Map CreateVolunteerOrderDto to VolunteerOrder
            CreateMap<CreateVolunteerOrderDto, VolunteerOrder>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.VolunteerId,
                           opt => opt.Ignore())
                .ForMember(dest => dest.Status,
                           opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted,
                           opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore());

            // Map UpdateVolunteerOrderDto to VolunteerOrder
            CreateMap<UpdateVolunteerOrderDto, VolunteerOrder>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.VolunteerId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Status,
                    opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted,
                    opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore());

            #endregion

            #region Partner Mapping

            // Map Partner to PartnerDto

            // Map CreatePartnerDto to Partner
            CreateMap<CreatePartnerDto, Partner>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt,
                           opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted,
                           opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.DeletedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Verified,
                           opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.ProjectsCount,
                           opt => opt.MapFrom(_ => 0));

            // Map UpdatePartnerDto to Partner
                .ForMember(dest => dest.IsDeleted,
                    opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt,
                    opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore());

            #endregion


        }

        #endregion
    }
}