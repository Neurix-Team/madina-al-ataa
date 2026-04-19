using AutoMapper;
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