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

            // UpdateVolunteerOrderDto -> VolunteerOrder
            CreateMap<UpdateVolunteerOrderDto, VolunteerOrder>().ReverseMap();

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

            CreateMap<CertificateCreateDto, Certificate>().ReverseMap();
            // GeoQuest mappings
            CreateMap<GeoQuest, GeoQuestDto>().ReverseMap();

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

            CreateMap<CreateUserGeoQuestDto, UserGeoQuest>().ReverseMap();

            CreateMap<UpdateUserGeoQuestDto, UserGeoQuest>().ReverseMap();

            #endregion

            // Mapping from DonationOrder Entity to DTOs
            CreateMap<DonationOrder, DonationOrderReadDto>().ReverseMap();

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