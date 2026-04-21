using AutoMapper;
using GivingChampion.Common.DTO;
using GivingChampion.Common.DTO.AiAvatarDto;
using GivingChampion.Common.DTO.AvatarDto;
using GivingChampion.Common.DTO.BadgeDto;
using GivingChampion.Common.DTO.CertificateDto;
using GivingChampion.Common.DTO.Child;
using GivingChampion.Common.DTO.DonationOrder;
using GivingChampion.Common.DTO.DonationRequest;
using GivingChampion.Common.DTO.Donor;
using GivingChampion.Common.DTO.GeoQuestDto;
using GivingChampion.Common.DTO.LevelDto;
using GivingChampion.Common.DTO.Location;
using GivingChampion.Common.DTO.Mission;
using GivingChampion.Common.DTO.Notification;
using GivingChampion.Common.DTO.Partner;
using GivingChampion.Common.DTO.PartnerDto;
using GivingChampion.Common.DTO.ProfileDto;
using GivingChampion.Common.DTO.ReviewDto;
using GivingChampion.Common.DTO.ServiceRequestDto;
using GivingChampion.Common.DTO.User;
using GivingChampion.Common.DTO.UserBadgeDto;
using GivingChampion.Common.DTO.UserLevelDto;
using GivingChampion.Common.DTO.VolunteerDto;
using GivingChampion.Common.DTO.VolunteerOrder;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;
using System.Collections.Generic;
using GivingChampion.Common.DTO.UserGeoQuestDto;

using GivingChampion.Common.DTO.ServiceRequestDto;
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
                    opt => opt.MapFrom(src => src.Partner != null ? src.Partner.OrgName : string.Empty)).ReverseMap();

            // CreateServiceRequestDto -> ServiceRequest
            CreateMap<CreateServiceRequestDto, ServiceRequest>().ReverseMap();

            // UpdateServiceRequestDto -> ServiceRequest (normal update)
            CreateMap<UpdateServiceRequestDto, ServiceRequest>().ReverseMap();

            // VolunteerOrder -> VolunteerOrderDto
            CreateMap<VolunteerOrder, VolunteerOrderDto>().ReverseMap();

            // CreateVolunteerOrderDto -> VolunteerOrder
            CreateMap<CreateVolunteerOrderDto, VolunteerOrder>().ReverseMap();


            // Partner -> PartnerDto
            CreateMap<Partner, PartnerDto>()
                .ForMember(dest => dest.OrgTypeName, opt => opt.MapFrom(src => src.OrgType.ToString()));
            CreateMap<Partner, CreatePartnerDto>().ReverseMap();

            // UpdatePartnerDto -> Partner
            CreateMap<UpdatePartnerDto, Partner>().ReverseMap();

            // Volunteer mappings
            CreateMap<Volunteer, VolunteerDto>().ReverseMap();

            CreateMap<CreateVolunteerDto, Volunteer>().ReverseMap();

            CreateMap<UpdateVolunteerDto, Volunteer>().ReverseMap();
            // Certificate mappings
            CreateMap<Certificate, CertificateReadAllDto>().ReverseMap();

            CreateMap<CreateVolunteerOrderDto, VolunteerOrder>()
      // Generate a unique ID for the new Order
      .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))

      // Explicitly map the ServiceRequestId from the DTO
      .ForMember(dest => dest.ServiceRequestId, opt => opt.MapFrom(src => src.ServiceRequestId))

      // Set default values for a new request
      .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => OrderStatus.Pending))
      .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
      .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            CreateMap<CreateGeoQuestDto, GeoQuest>().ReverseMap();

            CreateMap<UpdateGeoQuestDto, GeoQuest>().ReverseMap();

            CreateMap<DonationRequest, DonationRequestDto>();
            CreateMap<CreateDonationRequestDto, DonationRequest>();
            CreateMap<UpdateDonationRequestDto, DonationRequest>();

            #region UserGeoQuest Mappings

            CreateMap<UserGeoQuest, UserGeoQuestDto>()
                .ForMember(dest => dest.GeoQuestId,
                    opt => opt.MapFrom(src => src.GeoQuestId))
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom(src =>
                        src.GeoQuest != null ? src.GeoQuest.Title : src.Title))
                .ForMember(dest => dest.LocationLatitude,
                    opt => opt.MapFrom(src =>
                        src.GeoQuest != null ? src.GeoQuest.LocationLatitude : 0))
                .ForMember(dest => dest.LocationLongitude,
                    opt => opt.MapFrom(src =>
                        src.GeoQuest != null ? src.GeoQuest.LocationLongitude : 0))
                .ReverseMap();

            CreateMap<UpdateDonationRequestDto, DonationRequest>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Title, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Title)))  // Only map if not null or empty
            .ForMember(dest => dest.Location, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Location)))
            .ForMember(dest => dest.DonateAmount, opt => opt.Condition(src => src.DonateAmount.HasValue))
            .ForMember(dest => dest.UrgencyLevel, opt => opt.Condition(src => src.UrgencyLevel.HasValue))
            .ForMember(dest => dest.BriefDescription, opt => opt.Condition(src => !string.IsNullOrEmpty(src.BriefDescription)));
            CreateMap<CreateUserGeoQuestDto, UserGeoQuest>().ReverseMap();

            CreateMap<UpdateUserGeoQuestDto, UserGeoQuest>().ReverseMap();

            #endregion

            // Mapping from DonationOrder Entity to DTOs
            CreateMap<DonationOrder, DonationOrderReadDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<DonationOrder, DonationOrderDetailsDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<DonationOrder, DonationOrderDetailsDto>().ReverseMap();
            // Mapping from CreateDonationOrderDto to DonationOrder Entity
            CreateMap<CreateDonationOrderDto, DonationOrder>().ReverseMap();
            // Mapping from UpdateDonationOrderDTO to DonationOrder Entity
            CreateMap<UpdateDonationOrderDTO, DonationOrder>().ReverseMap();
            CreateMap<DonationOrder, DonationOrderReadDto>();
            CreateMap<DonationOrder, DonationOrderDetailsDto>();
            CreateMap<CreateDonationOrderDto, DonationOrder>();
            CreateMap<UpdateDonationOrderDTO, DonationOrder>();
  

            #region AiAvatar Mappings
            CreateMap<AiAvatar, AiAvatarDto>().ReverseMap();

            CreateMap<CreateAiAvatarDto, AiAvatar>().ReverseMap();

            CreateMap<UpdateAiAvatarDto, AiAvatar>().ReverseMap();
            #endregion

            #region Avatar Mappings
            CreateMap<Avatar, AvatarDto>().ReverseMap();

            CreateMap<CreateAvatarDto, Avatar>().ReverseMap();

            CreateMap<UpdateAvatarDto, Avatar>().ReverseMap();
            #endregion

            #region Level Mappings
            CreateMap<Level, LevelDto>().ReverseMap(); // Reverse mapping for Level -> LevelDto and vice versa

            CreateMap<CreateLevelDto, Level>().ReverseMap();

            CreateMap<UpdateLevelDto, Level>().ReverseMap();
            #endregion

            #region Badge Mappings
            CreateMap<Badge, BadgeDto>()
                .ReverseMap(); // Reverse mapping for Badge -> BadgeDto and vice versa

            CreateMap<CreateBadgeDto, Badge>().ReverseMap();

            CreateMap<UpdateBadgeDto, Badge>().ReverseMap();
            #endregion

            #region UserBadge Mappings
            CreateMap<UserBadge, UserBadgeDto>()
                .ReverseMap(); // Reverse mapping for UserBadge -> UserBadgeDto and vice versa

            CreateMap<CreateUserBadgeDto, UserBadge>().ReverseMap();

            CreateMap<UpdateUserBadgeDto, UserBadge>().ReverseMap();
            #endregion

            #region UserLevel Mappings
            CreateMap<UserLevel, UserLevelDto>()
                .ForMember(dest => dest.LevelId, opt => opt.MapFrom(src => src.Level.Id)) // Mapping level id explicitly
                .ReverseMap(); // Reverse mapping for UserLevel -> UserLevelDto and vice versa

            CreateMap<CreateUserLevelDto, UserLevel>().ReverseMap();

            CreateMap<UpdateUserLevelDto, UserLevel>().ReverseMap();
            #endregion

            #region Profile Mappings
            CreateMap<Domain.Entities.Profile, ProfileDto>().ReverseMap(); // Reverse mapping for Profile -> ProfileDto and vice versa

            CreateMap<CreateProfileDto, Domain.Entities.Profile>().ReverseMap();

            CreateMap<UpdateProfileDto, Domain.Entities.Profile>().ReverseMap();
            #endregion

            #region Review Mappings
            CreateMap<Review, ReviewDto>()
                .ReverseMap();

            CreateMap<CreateReviewDto, Review>().ReverseMap();

            CreateMap<UpdateReviewDto, Review>().ReverseMap();
            #endregion

            #region User Mappings
            // ====================== User Mappings ======================
            CreateMap<ApplicationUser, GetUserDto>().ReverseMap();

            //CreateMap<PagedList<ApplicationUser>, PagedList<GetUserDto>>().ReverseMap();

            CreateMap<CreateUserDto, ApplicationUser>().ReverseMap();

            CreateMap<UpdateUser, ApplicationUser>().ReverseMap();

            #endregion

            #region Donor Mappings
            // ====================== Donor Mappings ======================
            CreateMap<Donor, DonorDto>().ReverseMap();

            CreateMap<CreateDonorDto, Donor>().ReverseMap();

            CreateMap<UpdateDonorDto, Donor>().ReverseMap();

            #endregion
            #region Child Mappings
            // ====================== Child Mappings ======================
            CreateMap<Child, ChildDto>().ReverseMap();

            CreateMap<CreateChildDto, Child>().ReverseMap();

            // For approval/rejection flows (if needed)
            CreateMap<ApproveChildDto, Child>().ReverseMap();

            #endregion
            #region Notification Mappings
            // ====================== Notification Mappings ======================
            CreateMap<Notification, NotificationDto>().ReverseMap();

            #endregion
            #region Mission Mappings
            // ====================== Mission Mappings ======================
            CreateMap<Mission, MissionDto>().ReverseMap();
            CreateMap<CreateMissionDto, Mission>().ReverseMap();
            CreateMap<UpdateMissionDto, Mission>().ReverseMap();
            #endregion

            #region UserMission Mappings
            // ====================== Mission Mappings ======================
            CreateMap<UserMission, UserMissionDto>().ReverseMap();
            CreateMap<StartMissionDto, Mission>().ReverseMap();
            CreateMap<UpdateProgressDto, Mission>().ReverseMap();
            #endregion
            // NOT WORKING
            //#region Pagination Mappings
            //// ====================== Pagination Mappings ======================
            //// ONLY this one generic mapping for PagedList - remove any other PagedList mappings
            //CreateMap(typeof(PagedList<>), typeof(PagedList<>))
            //    .ConvertUsing(typeof(PagedListConverter<,>));

            //#endregion
            #region Location Mappings
            // Location Mappings
            CreateMap<Location, LocationDto>().ReverseMap();

            CreateMap<CreateLocationDto, Location>().ReverseMap();

            CreateMap<UpdateLocationDto, Location>().ReverseMap();

            #endregion
            #region Service Request Mapping

            #region ServiceRequest To ServiceRequestDto

            /*
             * Maps ServiceRequest entity to ServiceRequestDto.
             * Used when returning service request data to the client.
             */
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

            #region Volunteer Mapping

            // Map Volunteer to VolunteerDto (Returning from Database to User)
            CreateMap<Volunteer, VolunteerDto>()
                .ForMember(dest => dest.TotalHours, opt => opt.MapFrom(src => src.TotalHours))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills))
                .ForMember(dest => dest.Availability, opt => opt.MapFrom(src => src.Availability));

            // Map CreateVolunteerDto to Volunteer (Creating a new Volunteer)
            CreateMap<CreateVolunteerDto, Volunteer>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))  // Ensure Id is generated on creation
                .ForMember(dest => dest.TotalHours, opt => opt.MapFrom(_ => 0))  // Default to 0 hours
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false)) // Set IsDeleted to false by default
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore()) // Ignore DeletedAt, will not be used on creation
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId)) // Set UserId passed from Admin/Claims
                .ForMember(dest => dest.Availability, opt => opt.MapFrom(src => src.Availability)) // Map from DTO to Entity
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills)); // Map from DTO to Entity

            // Map UpdateVolunteerDto to Volunteer (Updating an existing Volunteer)
            CreateMap<UpdateVolunteerDto, Volunteer>()
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills)) // Update skills
                .ForMember(dest => dest.Availability, opt => opt.MapFrom(src => src.Availability)) // Update availability
                .ForMember(dest => dest.UserId, opt => opt.Ignore()); // UserId should remain the same as when initially created

            #endregion

            // Map from Certificate entity to CertificateReadAllDto
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
    // NOT WORKING
    // Helper converter for generic PagedList mapping
    //public class PagedListConverter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>>
    //{
    //    public PagedList<TDestination> Convert(PagedList<TSource> source, PagedList<TDestination> destination, ResolutionContext context)
    //    {
    //        if (source == null) return null;

    //        // Map the internal list of items
    //        var mappedItems = context.Mapper.Map<List<TDestination>>(source.Items);

    //        // Create the new paged list with the mapped items and existing metadata
    //        return new PagedList<TDestination>(
    //            mappedItems,
    //            source.TotalCount,
    //            source.PageNumber,
    //            source.PageSize
    //        );
    //    }
    //}
}
}