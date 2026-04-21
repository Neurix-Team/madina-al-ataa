using AutoMapper;
using GivingChampion.Common.DTO.AiAvatarDto;
using GivingChampion.Common.DTO.AvatarDto;
using GivingChampion.Common.DTO.BadgeDto;
using GivingChampion.Common.DTO.LevelDto;
using GivingChampion.Common.DTO.ProfileDto;
using GivingChampion.Common.DTO.ReviewDto;
using GivingChampion.Common.DTO.UserBadgeDto;
using GivingChampion.Common.DTO.UserLevelDto;
using GivingChampion.Domain.Entities;
using GivingChampion.Common.DTO.Child;
using GivingChampion.Common.DTO.Donor;
using GivingChampion.Common.DTO.Location;
using GivingChampion.Common.DTO.Mission;
using GivingChampion.Common.DTO.Notification;
using GivingChampion.Common.DTO.User;
using GivingChampion.Common.Pagination;
using GivingChampion.Domain.Enums;
using System.Collections.Generic;

namespace GivingChampion.Application.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
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