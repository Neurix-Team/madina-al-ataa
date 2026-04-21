using AutoMapper;
using GivingChampion.Common.DTO.CertificateDto;
using GivingChampion.Common.DTO.DonationOrder;
using GivingChampion.Common.DTO.DonationRequest;
using GivingChampion.Common.DTO.Partner;
using GivingChampion.Common.DTO.PartnerDto;
using GivingChampion.Common.DTO.ServiceRequestDto;
using GivingChampion.Common.DTO.VolunteerDto;
using GivingChampion.Common.DTO.VolunteerOrder;
using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;

namespace GivingChampion.Application.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            // ServiceRequest -> ServiceRequestDto
            CreateMap<ServiceRequest, ServiceRequestDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.Partner != null ? src.Partner.OrgName : string.Empty))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.BriefDescription,
                    opt => opt.MapFrom(src => src.BriefDescription ?? string.Empty));

            // CreateServiceRequestDto -> ServiceRequest
            CreateMap<CreateServiceRequestDto, ServiceRequest>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.Trim()))
                .ForMember(dest => dest.RequiredSkill, opt => opt.MapFrom(src => src.RequiredSkill.Trim()))
                .ForMember(dest => dest.BriefDescription,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.BriefDescription) ? string.Empty : src.BriefDescription.Trim()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => RequestStatus.Pending))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Partner, opt => opt.Ignore());

            // UpdateServiceRequestDto -> ServiceRequest (normal update)
            CreateMap<UpdateServiceRequestDto, ServiceRequest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title.Trim()))
                .ForMember(dest => dest.RequiredSkill, opt => opt.MapFrom(src => src.RequiredSkill.Trim()))
                .ForMember(dest => dest.BriefDescription,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.BriefDescription) ? string.Empty : src.BriefDescription.Trim()))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PartnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Partner, opt => opt.Ignore());

            // VolunteerOrder -> VolunteerOrderDto
            CreateMap<VolunteerOrder, VolunteerOrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // CreateVolunteerOrderDto -> VolunteerOrder
            CreateMap<CreateVolunteerOrderDto, VolunteerOrder>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.VolunteerId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // UpdateVolunteerOrderDto -> VolunteerOrder
            CreateMap<UpdateVolunteerOrderDto, VolunteerOrder>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.VolunteerId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            // Partner -> PartnerDto
            CreateMap<Partner, PartnerDto>()
                .ForMember(dest => dest.OrgTypeName, opt => opt.MapFrom(src => src.OrgType.ToString()));

            // UpdatePartnerDto -> Partner
            CreateMap<UpdatePartnerDto, Partner>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Verified, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectsCount, opt => opt.Ignore());

            // Volunteer mappings
            CreateMap<Volunteer, VolunteerDto>()
                .ForMember(dest => dest.TotalHours, opt => opt.MapFrom(src => src.TotalHours))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills))
                .ForMember(dest => dest.Availability, opt => opt.MapFrom(src => src.Availability));

            CreateMap<CreateVolunteerDto, Volunteer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.TotalHours, opt => opt.MapFrom(_ => 0))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Availability, opt => opt.MapFrom(src => src.Availability))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills));

            CreateMap<UpdateVolunteerDto, Volunteer>()
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills)).ForMember(dest => dest.Availability, opt => opt.MapFrom(src => src.Availability)).ForMember(dest => dest.UserId, opt => opt.Ignore());

            // Certificate mappings
            CreateMap<Certificate, CertificateReadAllDto>();

            CreateMap<CertificateCreateDto, Certificate>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IssuedDate, opt => opt.Ignore());



            CreateMap<UpdateDonationRequestDto, DonationRequest>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())  
            .ForMember(dest => dest.Title, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Title)))  // Only map if not null or empty
            .ForMember(dest => dest.Location, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Location)))
            .ForMember(dest => dest.DonateAmount, opt => opt.Condition(src => src.DonateAmount.HasValue))
            .ForMember(dest => dest.UrgencyLevel, opt => opt.Condition(src => src.UrgencyLevel.HasValue))
            .ForMember(dest => dest.BriefDescription, opt => opt.Condition(src => !string.IsNullOrEmpty(src.BriefDescription)));



            // Mapping from DonationOrder Entity to DTOs
            CreateMap<DonationOrder, DonationOrderReadDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()));

            CreateMap<DonationOrder, DonationOrderDetailsDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()));

            // Mapping from CreateDonationOrderDto to DonationOrder Entity
            CreateMap<CreateDonationOrderDto, DonationOrder>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => OrderStatus.Pending)); // Default Status

            // Mapping from UpdateDonationOrderDTO to DonationOrder Entity
            CreateMap<UpdateDonationOrderDTO, DonationOrder>();
        }
    }

    }

