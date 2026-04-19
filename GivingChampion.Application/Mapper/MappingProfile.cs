using AutoMapper;
using GivingChampion.Common.DTO.AiAvatarDto;
using GivingChampion.Common.DTO.AvatarDto;
using GivingChampion.Common.DTO.BadgeDto;
using GivingChampion.Common.DTO.LevelDto;
using GivingChampion.Common.DTO.ProfileDto;
using GivingChampion.Common.DTO.UserBadgeDto;
using GivingChampion.Common.DTO.UserLevelDto;
using GivingChampion.Domain.Entities;

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
        }
    }
}