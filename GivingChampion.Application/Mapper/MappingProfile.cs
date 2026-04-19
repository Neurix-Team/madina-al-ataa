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
using GivingChampion.Domain.Entities;
using GivingChampion.Domain.Enums;

namespace GivingChampion.Application.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            #region AiAvatar Mappings
            CreateMap<AiAvatar, AiAvatarDto>()
                .ReverseMap(); // Reverse mapping for AiAvatar -> AiAvatarDto and vice versa

            CreateMap<CreateAiAvatarDto, AiAvatar>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id for Create Mapping
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            CreateMap<UpdateAiAvatarDto, AiAvatar>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());
            #endregion

            #region Avatar Mappings
            CreateMap<Avatar, AvatarDto>()
                .ReverseMap(); // Reverse mapping for Avatar -> AvatarDto and vice versa

            CreateMap<CreateAvatarDto, Avatar>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            CreateMap<UpdateAvatarDto, Avatar>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());
            #endregion

            #region Level Mappings
            CreateMap<Level, LevelDto>()
                .ReverseMap(); // Reverse mapping for Level -> LevelDto and vice versa

            CreateMap<CreateLevelDto, Level>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            CreateMap<UpdateLevelDto, Level>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());
            #endregion

            #region Badge Mappings
            CreateMap<Badge, BadgeDto>()
                .ReverseMap(); // Reverse mapping for Badge -> BadgeDto and vice versa

            CreateMap<CreateBadgeDto, Badge>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            CreateMap<UpdateBadgeDto, Badge>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());
            #endregion

            #region UserBadge Mappings
            CreateMap<UserBadge, UserBadgeDto>()
                .ReverseMap(); // Reverse mapping for UserBadge -> UserBadgeDto and vice versa

            CreateMap<CreateUserBadgeDto, UserBadge>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            CreateMap<UpdateUserBadgeDto, UserBadge>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());
            #endregion

            #region UserLevel Mappings
            CreateMap<UserLevel, UserLevelDto>()
                .ForMember(dest => dest.LevelId, opt => opt.MapFrom(src => src.Level.Id)) // Mapping level id explicitly
                .ReverseMap(); // Reverse mapping for UserLevel -> UserLevelDto and vice versa

            CreateMap<CreateUserLevelDto, UserLevel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            CreateMap<UpdateUserLevelDto, UserLevel>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());
            #endregion

            #region Profile Mappings
            CreateMap<Domain.Entities.Profile, ProfileDto>()
                .ReverseMap(); // Reverse mapping for Profile -> ProfileDto and vice versa

            CreateMap<CreateProfileDto, Domain.Entities.Profile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            CreateMap<UpdateProfileDto, Domain.Entities.Profile>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());
            #endregion

            #region Review Mappings
            CreateMap<Review, ReviewDto>()
                .ReverseMap();

            CreateMap<CreateReviewDto, Review>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            CreateMap<UpdateReviewDto, Review>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());
            #endregion
            // ====================== User Mappings ======================
            CreateMap<ApplicationUser, GetUserDto>().ReverseMap();

            CreateMap<PagedList<ApplicationUser>, PagedList<GetUserDto>>().ReverseMap();

            CreateMap<CreateUserDto, ApplicationUser>().ReverseMap();

            CreateMap<UpdateUser, ApplicationUser>().ReverseMap();

            // ====================== Donor Mappings ======================
            CreateMap<Donor, DonorDto>().ReverseMap();

            CreateMap<CreateDonorDto, Donor>().ReverseMap();

            CreateMap<UpdateDonorDto, Donor>().ReverseMap();

            // ====================== Child Mappings ======================
            CreateMap<Child, ChildDto>().ReverseMap();

            CreateMap<CreateChildDto, Child>().ReverseMap();

            // For approval/rejection flows (if needed)
            CreateMap<ApproveChildDto, Child>().ReverseMap();

            // ====================== Notification Mappings ======================
            CreateMap<Notification, NotificationDto>().ReverseMap();

            // ====================== Mission Mappings ======================
            CreateMap<Mission, MissionDto>().ReverseMap();
            CreateMap<CreateMissionDto, Mission>().ReverseMap();
            CreateMap<UpdateMissionDto, Mission>().ReverseMap();

            // ====================== Pagination Mappings ======================
            // ONLY this one generic mapping for PagedList - remove any other PagedList mappings
            CreateMap(typeof(PagedList<>), typeof(PagedList<>))
                .ConvertUsing(typeof(PagedListConverter<,>));

            // Location Mappings
            CreateMap<Location, LocationDto>().ReverseMap();

            CreateMap<CreateLocationDto, Location>().ReverseMap();

            CreateMap<UpdateLocationDto, Location>().ReverseMap();
        }

        // Helper converter for generic PagedList mapping
        public class PagedListConverter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>>
        {
            public PagedList<TDestination> Convert(PagedList<TSource> source, PagedList<TDestination> destination, ResolutionContext context)
            {
                var items = context.Mapper.Map<List<TDestination>>(source.Items);
                return new PagedList<TDestination>(
                    items,
                    source.TotalCount,
                    source.PageNumber,
                    source.PageSize
                );
            }
        }
    }
}