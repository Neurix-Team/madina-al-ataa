using GivingChampion.Application.DTO.AiAvatarDto;
using GivingChampion.Application.DTO.AvatarDto;
using GivingChampion.Application.DTO.BadgeDto;
using GivingChampion.Application.DTO.CertificateDto;
using GivingChampion.Application.DTO.Child;
using GivingChampion.Application.DTO.DonationOrder;
using GivingChampion.Application.DTO.DonationRequest;
using GivingChampion.Application.DTO.Donor;
using GivingChampion.Application.DTO.LevelDto;
using GivingChampion.Application.DTO.Location;
using GivingChampion.Application.DTO.Mission;
using GivingChampion.Application.DTO.Notification;
using GivingChampion.Application.DTO.Partner;
using GivingChampion.Application.DTO.PartnerDto;
using GivingChampion.Application.DTO.ProfileDto;
using GivingChampion.Application.DTO.ReviewDto;
using GivingChampion.Application.DTO.ServiceRequestDto;
using GivingChampion.Application.DTO.User;
using GivingChampion.Application.DTO.UserBadge;
using GivingChampion.Application.DTO.UserLevelDto;
using GivingChampion.Application.DTO.Volunteer;
using GivingChampion.Application.DTO.VolunteerOrder;
using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
using GivingChampion.Application.DTO;
using GivingChampion.Application.DTO.GeoQuestDto;
using GivingChampion.Domain.Enums;
using GivingChampion.Application.DTO.UserGeoQuestDto;
using GivingChampion.Application.DTO.GivingChampion.Application.DTO.ActivityDto;
using GivingChampion.Application.DTO.ActivityDto;

namespace GivingChampion.Application.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            #region ServiceRequest Mappings

            CreateMap<ServiceRequest, ServiceRequestDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.Partner != null ? src.Partner.OrgName : string.Empty)).ReverseMap();

            CreateMap<CreateServiceRequestDto, ServiceRequest>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.ScheduleDate,
                    opt => opt.MapFrom(src => DateTime.SpecifyKind(src.ScheduleDate.Date, DateTimeKind.Utc)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => RequestStatus.Pending))
                .ForMember(dest => dest.Progress, opt => opt.MapFrom(_ => 0))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.RequiredLevelId, opt => opt.Ignore())
                .ForMember(dest => dest.MaxOrders, opt => opt.Ignore())
                .ForMember(dest => dest.Partner, opt => opt.Ignore())
                .ForMember(dest => dest.VolunteerUserId, opt => opt.Ignore());
            // CreateServiceRequestDto -> ServiceRequest
            CreateMap<CreateServiceRequestDto, ServiceRequest>().ReverseMap();

            CreateMap<UpdateServiceRequestDto, ServiceRequest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ScheduleDate,
                    opt => opt.MapFrom(src => DateTime.SpecifyKind(src.ScheduleDate.Date, DateTimeKind.Utc)))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.RequiredLevelId, opt => opt.Ignore())
                .ForMember(dest => dest.MaxOrders, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.PartnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Partner, opt => opt.Ignore());
            // UpdateServiceRequestDto -> ServiceRequest (normal update)
            CreateMap<UpdateServiceRequestDto, ServiceRequest>().ReverseMap();

            #endregion

            CreateMap<VolunteerOrder, VolunteerOrderDto>()
       .ForMember(dest => dest.ServiceType,
           opt => opt.MapFrom(src => src.ServiceRequest != null
               ? src.ServiceRequest.ServiceType.ToString()
               : string.Empty))
        .ForMember(dest => dest.Progress,
       opt => opt.MapFrom(src => src.ServiceRequest != null
           ? src.ServiceRequest.Progress
           : 0))
       .ForMember(dest => dest.Description,
           opt => opt.MapFrom(src => src.ServiceRequest != null
               ? src.ServiceRequest.BriefDescription
               : string.Empty))
       .ForMember(dest => dest.ScheduleDate,
           opt => opt.MapFrom(src => src.ServiceRequest != null
               ? src.ServiceRequest.ScheduleDate
               : default)).ReverseMap();

            #region Volunteer Mappings

            CreateMap<Volunteer, VolunteerDto>()
                .ForMember(dest => dest.TotalHours, opt => opt.MapFrom(src => src.TotalHours))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills))
                .ForMember(dest => dest.Availability, opt => opt.MapFrom(src => src.Availability));
            // Volunteer mappings
            CreateMap<Volunteer, VolunteerDto>().ReverseMap();

            CreateMap<CreateVolunteerDto, Volunteer>().ReverseMap();

            CreateMap<UpdateVolunteerDto, Volunteer>().ReverseMap();
            // Certificate mappings
            CreateMap<Certificate, CertificateReadAllDto>().ReverseMap();

            CreateMap<UpdateVolunteerDto, Volunteer>()
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills))
                .ForMember(dest => dest.Availability, opt => opt.MapFrom(src => src.Availability))
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            #endregion
            #region Partner Mappings

            CreateMap<CreatePartnerDto, Partner>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.OrgName, opt => opt.MapFrom(src => src.OrgName.Trim()))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Verified, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.ProjectsCount, opt => opt.MapFrom(_ => 0));

            CreateMap<Partner, PartnerDto>()
                .ForMember(dest => dest.OrgTypeName,
                    opt => opt.MapFrom(src => src.OrgType.ToString()));
            CreateMap<Partner, CreatePartnerDto>().ReverseMap();

            CreateMap<UpdatePartnerDto, Partner>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrgName, opt => opt.MapFrom(src => src.OrgName.Trim()))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Verified, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectsCount, opt => opt.Ignore());
            // UpdatePartnerDto -> Partner
            CreateMap<UpdatePartnerDto, Partner>().ReverseMap();

            #endregion

            #region Certificate Mappings
            CreateMap<CreateGeoQuestDto, GeoQuest>().ReverseMap();
            CreateMap<GeoQuestDto, GeoQuest>().ReverseMap();

            CreateMap<Certificate, CertificateReadAllDto>();
            CreateMap<UpdateGeoQuestDto, GeoQuest>().ReverseMap();

            CreateMap<DonationRequest, DonationRequestDto>();
            CreateMap<CreateDonationRequestDto, DonationRequest>();
            CreateMap<UpdateDonationRequestDto, DonationRequest>();

            #endregion

            #region VolunteerHistory Mappings

            //CreateMap<VolunteerHistories, VolunteerHistoryDto>();
            CreateMap<Activity, ActivityDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty))
                .ReverseMap();
            CreateMap<CreateActivityDto, Activity>().ReverseMap();

            #endregion

            #region DonationRequest Mappings

            CreateMap<UserGeoQuest, UserGeoQuestDto>()
                .ForMember(dest => dest.GeoQuestId, opt => opt.MapFrom(src => src.GeoQuestId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src =>
                    src.GeoQuest != null && !string.IsNullOrWhiteSpace(src.GeoQuest.Title)
                        ? src.GeoQuest.Title
                        : src.Title))
                
                .ForMember(dest => dest.LocationLatitude, opt => opt.MapFrom(src =>src.GeoQuest.Location.Latitude ?? "0"))
                .ForMember(dest => dest.LocationLongitude, opt => opt.MapFrom(src =>src.GeoQuest.Location.Longitude ?? "0"))
                .ReverseMap();

            CreateMap<UpdateDonationRequestDto, DonationRequest>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Title, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Title)))
                .ForMember(dest => dest.Location, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Location)))
                .ForMember(dest => dest.DonateAmount, opt => opt.Condition(src => src.DonateAmount.HasValue))
                .ForMember(dest => dest.UrgencyLevel, opt => opt.Condition(src => src.UrgencyLevel.HasValue))
                .ForMember(dest => dest.BriefDescription, opt => opt.Condition(src => !string.IsNullOrEmpty(src.BriefDescription)));
            CreateMap<CreateUserGeoQuestDto, UserGeoQuest>().ReverseMap();

            #endregion
            CreateMap<UpdateUserGeoQuestDto, UserGeoQuest>().ReverseMap();

            #region DonationOrder Mappings
            #endregion

            CreateMap<DonationOrder, DonationOrderReadDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<DonationOrder, DonationOrderDetailsDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateDonationOrderDto, DonationOrder>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => OrderStatus.Pending))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

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

            CreateMap<Level, LevelDto>().ReverseMap();
            CreateMap<CreateLevelDto, Level>().ReverseMap();
            CreateMap<UpdateLevelDto, Level>().ReverseMap();

            #endregion

            #region Badge Mappings

            CreateMap<Badge, BadgeDto>().ReverseMap();
            CreateMap<CreateBadgeDto, Badge>().ReverseMap();
            CreateMap<UpdateBadgeDto, Badge>().ReverseMap();

            #endregion

            #region UserBadge Mappings

            CreateMap<UserBadge, UserBadgeDto>().ReverseMap();
            CreateMap<CreateUserBadgeDto, UserBadge>().ReverseMap();
            CreateMap<UpdateUserBadgeDto, UserBadge>().ReverseMap();

            #endregion

            #region UserLevel Mappings

            CreateMap<UserLevel, UserLevelDto>()
                .ForMember(dest => dest.LevelId,
                    opt => opt.MapFrom(src => src.Level.Id))
                .ReverseMap();

            CreateMap<CreateUserLevelDto, UserLevel>().ReverseMap();
            CreateMap<UpdateUserLevelDto, UserLevel>().ReverseMap();

            #endregion

            #region Profile Mappings

            CreateMap<Domain.Entities.Profile, ProfileDto>()
                .ForMember(dest => dest.LevelNumber, opt => opt.MapFrom(src => src.UserLevel.Level.Number)).ReverseMap();
            CreateMap<CreateProfileDto, Domain.Entities.Profile>().ReverseMap();
            CreateMap<UpdateProfileDto, Domain.Entities.Profile>().ReverseMap();

            #endregion

            #region Review Mappings

            CreateMap<Review, ReviewDto>().ReverseMap();
            CreateMap<CreateReviewDto, Review>().ReverseMap();
            CreateMap<UpdateReviewDto, Review>().ReverseMap();

            #endregion

            #region User Mappings

            CreateMap<ApplicationUser, GetUserDto>().ReverseMap();
            CreateMap<CreateUserDto, ApplicationUser>().ReverseMap();
            CreateMap<UpdateUser, ApplicationUser>().ReverseMap();

            #endregion

            #region Donor Mappings

            CreateMap<Donor, DonorDto>().ReverseMap();
            CreateMap<CreateDonorDto, Donor>().ReverseMap();
            CreateMap<UpdateDonorDto, Donor>().ReverseMap();

            #endregion

            #region Child Mappings

            CreateMap<Child, ChildDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
                .ForMember(dest => dest.BirthDay, opt => opt.MapFrom(src => src.User != null ? src.User.BirthDay : default));

            CreateMap<CreateChildDto, Child>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.ParentId, opt => opt.Ignore())
                .ForMember(dest => dest.Parent, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedById, opt => opt.Ignore())
                .ForMember(dest => dest.Approver, opt => opt.Ignore())
                .ForMember(dest => dest.ApprovedAt, opt => opt.Ignore())
                .ForMember(dest => dest.RejectionReason, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());
            CreateMap<ApproveChildDto, Child>().ReverseMap();

            #endregion

            #region Notification Mappings

            CreateMap<Notification, NotificationDto>().ReverseMap();
            CreateMap<CreateNotificationDto, Notification>().ReverseMap();

            #endregion

            #region Mission Mappings

            CreateMap<Mission, MissionDto>().ReverseMap();
            CreateMap<CreateMissionDto, Mission>().ReverseMap();
            CreateMap<UpdateMissionDto, Mission>().ReverseMap();

            #endregion

            #region UserMission Mappings

            CreateMap<UserMission, UserMissionDto>()
                .ForMember(dest => dest.KPReward, opt => opt.MapFrom(src => src.Mission.KPReward))
                .ForMember(dest => dest.XPReward, opt => opt.MapFrom(src => src.Mission.XPReward))
                .ReverseMap();
            CreateMap<StartMissionDto, Mission>().ReverseMap();
            CreateMap<Application.DTO.Mission.UpdateProgressDto, Mission>().ReverseMap();

            #endregion

            #region Location Mappings

            CreateMap<Location, LocationDto>().ReverseMap();
            CreateMap<CreateLocationDto, Location>().ReverseMap();
            CreateMap<UpdateLocationDto, Location>().ReverseMap();

            #endregion
        


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

            //// VolunteerOrder -> VolunteerOrderDto
            //CreateMap<VolunteerOrder, VolunteerOrderDto>()
            //    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            //// CreateVolunteerOrderDto -> VolunteerOrder
            //CreateMap<CreateVolunteerOrderDto, VolunteerOrder>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            //    .ForMember(dest => dest.Status, opt => opt.Ignore())
            //    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            //    .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            //    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());


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

            CreateMap<CreateVolunteerOrderDto, VolunteerOrder>()
      // Generate a unique ID for the new Order
      .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))

      // Explicitly map the ServiceRequestId from the DTO
      .ForMember(dest => dest.ServiceRequestId, opt => opt.MapFrom(src => src.ServiceRequestId))

      // Set default values for a new request
      .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => OrderStatus.Pending))
      .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
      .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

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
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<DonationOrder, DonationOrderDetailsDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Mapping from CreateDonationOrderDto to DonationOrder Entity
            CreateMap<CreateDonationOrderDto, DonationOrder>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => OrderStatus.Pending)); // Default Status

            // Mapping from UpdateDonationOrderDTO to DonationOrder Entity
            CreateMap<UpdateDonationOrderDTO, DonationOrder>();
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
    }
}
