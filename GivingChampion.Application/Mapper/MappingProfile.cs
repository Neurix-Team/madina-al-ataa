using AutoMapper;
using GivingChampion.Common.DTO.ServiceRequestDto;
using GivingChampion.Common.DTO.VolunteerOrder;
using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
namespace GivingChampion.Application.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {
        #region Constructor

        public MappingProfile()
        {
            #region Service Request Mapping

            #region ServiceRequest To ServiceRequestDto

            /*
             * Maps ServiceRequest entity to ServiceRequestDto.
             * Used when returning service request data to the client.
             */
            CreateMap<ServiceRequest, ServiceRequestDto>()
               // Map FullName from Partner.FullName because ServiceRequest itself does not contain FullName directly.
               .ForMember(dest => dest.FullName,
    opt => opt.MapFrom(src => src.Partner != null
        ? src.Partner.OrgName
        : string.Empty))

                // Convert RequestStatus enum to string.
                // Example: RequestStatus.Pending => "Pending"
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))

                // Return empty string instead of null for BriefDescription.
                .ForMember(dest => dest.BriefDescription,
                    opt => opt.MapFrom(src => src.BriefDescription ?? string.Empty));

            #endregion

            #region CreateServiceRequestDto To ServiceRequest

            /*
             * Maps CreateServiceRequestDto to ServiceRequest entity.
             * Used when creating a new service request.
             */
            CreateMap<CreateServiceRequestDto, ServiceRequest>()
                // Generate a new Id for the new service request.
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(_ => Guid.NewGuid()))

                // Trim extra spaces from Title before saving.
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom(src => src.Title.Trim()))

                // Trim extra spaces from RequiredSkill before saving.
                .ForMember(dest => dest.RequiredSkill,
                    opt => opt.MapFrom(src => src.RequiredSkill.Trim()))

                // Handle null, empty, or whitespace BriefDescription.
                .ForMember(dest => dest.BriefDescription,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.BriefDescription)
                        ? string.Empty
                        : src.BriefDescription.Trim()))

                // New service requests should start with Pending status.
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(_ => RequestStatus.Pending))

                // New service requests should not be marked as deleted.
                .ForMember(dest => dest.IsDeleted,
                    opt => opt.MapFrom(_ => false))

                // DeletedAt should stay empty when creating a new request.
                .ForMember(dest => dest.DeletedAt,
                    opt => opt.Ignore())

                // Set CreatedAt automatically when creating a new request.
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow))

                // Do not map Partner navigation property.
                // The relationship is handled by PartnerId.
                .ForMember(dest => dest.Partner,
                    opt => opt.Ignore());

            #endregion

            #region UpdateServiceRequestDto To ServiceRequest

            /*
             * Maps UpdateServiceRequestDto to an existing ServiceRequest entity.
             * Used when updating an existing service request.
             */
            CreateMap<UpdateServiceRequestDto, ServiceRequest>()
                // Do not allow update DTO to change the entity Id.
                .ForMember(dest => dest.Id,
                    opt => opt.Ignore())

                // Trim extra spaces from Title before updating.
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom(src => src.Title.Trim()))

                // Trim extra spaces from RequiredSkill before updating.
                .ForMember(dest => dest.RequiredSkill,
                    opt => opt.MapFrom(src => src.RequiredSkill.Trim()))

                // Handle null, empty, or whitespace BriefDescription.
                .ForMember(dest => dest.BriefDescription,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.BriefDescription)
                        ? string.Empty
                        : src.BriefDescription.Trim()))

                // Status should not be changed from the normal update endpoint.
                .ForMember(dest => dest.Status,
                    opt => opt.Ignore())

                // Soft delete fields should not be changed from the normal update endpoint.
                .ForMember(dest => dest.IsDeleted,
                    opt => opt.Ignore())

                // DeletedAt should not be changed from the normal update endpoint.
                .ForMember(dest => dest.DeletedAt,
                    opt => opt.Ignore())

                // CreatedAt should keep its original value.
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore())

                // PartnerId should not be changed from this update DTO.
                .ForMember(dest => dest.PartnerId,
                    opt => opt.Ignore())

                // Do not update Partner navigation property.
                .ForMember(dest => dest.Partner,
                    opt => opt.Ignore());

            #endregion

            #endregion
            #region Service Request Mapping

            #region ServiceRequest To ServiceRequestDto

            /*
             * Maps ServiceRequest entity to ServiceRequestDto.
             * Used when returning service request data to the client.
             */
            CreateMap<ServiceRequest, ServiceRequestDto>()
               // Map FullName from Partner.FullName because ServiceRequest itself does not contain FullName directly.
               .ForMember(dest => dest.FullName,
    opt => opt.MapFrom(src => src.Partner != null
        ? src.Partner.OrgName
        : string.Empty))

                // Convert RequestStatus enum to string.
                // Example: RequestStatus.Pending => "Pending"
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))

                // Return empty string instead of null for BriefDescription.
                .ForMember(dest => dest.BriefDescription,
                    opt => opt.MapFrom(src => src.BriefDescription ?? string.Empty));

            #endregion

            #region CreateServiceRequestDto To ServiceRequest

            /*
             * Maps CreateServiceRequestDto to ServiceRequest entity.
             * Used when creating a new service request.
             */
            CreateMap<CreateServiceRequestDto, ServiceRequest>()
                // Generate a new Id for the new service request.
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(_ => Guid.NewGuid()))

                // Trim extra spaces from Title before saving.
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom(src => src.Title.Trim()))

                // Trim extra spaces from RequiredSkill before saving.
                .ForMember(dest => dest.RequiredSkill,
                    opt => opt.MapFrom(src => src.RequiredSkill.Trim()))

                // Handle null, empty, or whitespace BriefDescription.
                .ForMember(dest => dest.BriefDescription,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.BriefDescription)
                        ? string.Empty
                        : src.BriefDescription.Trim()))

                // New service requests should start with Pending status.
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(_ => RequestStatus.Pending))

                // New service requests should not be marked as deleted.
                .ForMember(dest => dest.IsDeleted,
                    opt => opt.MapFrom(_ => false))

                // DeletedAt should stay empty when creating a new request.
                .ForMember(dest => dest.DeletedAt,
                    opt => opt.Ignore())

                // Set CreatedAt automatically when creating a new request.
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(_ => DateTime.UtcNow))

                // Do not map Partner navigation property.
                // The relationship is handled by PartnerId.
                .ForMember(dest => dest.Partner,
                    opt => opt.Ignore());

            #endregion

            #region UpdateServiceRequestDto To ServiceRequest

            /*
             * Maps UpdateServiceRequestDto to an existing ServiceRequest entity.
             * Used when updating an existing service request.
             */
            CreateMap<UpdateServiceRequestDto, ServiceRequest>()
                // Do not allow update DTO to change the entity Id.
                .ForMember(dest => dest.Id,
                    opt => opt.Ignore())

                // Trim extra spaces from Title before updating.
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom(src => src.Title.Trim()))

                // Trim extra spaces from RequiredSkill before updating.
                .ForMember(dest => dest.RequiredSkill,
                    opt => opt.MapFrom(src => src.RequiredSkill.Trim()))

                // Handle null, empty, or whitespace BriefDescription.
                .ForMember(dest => dest.BriefDescription,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.BriefDescription)
                        ? string.Empty
                        : src.BriefDescription.Trim()))

                // Status should not be changed from the normal update endpoint.
                .ForMember(dest => dest.Status,
                    opt => opt.Ignore())

                // Soft delete fields should not be changed from the normal update endpoint.
                .ForMember(dest => dest.IsDeleted,
                    opt => opt.Ignore())

                // DeletedAt should not be changed from the normal update endpoint.
                .ForMember(dest => dest.DeletedAt,
                    opt => opt.Ignore())

                // CreatedAt should keep its original value.
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore())

                // PartnerId should not be changed from this update DTO.
                .ForMember(dest => dest.PartnerId,
                    opt => opt.Ignore())

                // Do not update Partner navigation property.
                .ForMember(dest => dest.Partner,
                    opt => opt.Ignore());

            #endregion

            #endregion

            #region Volunteer Order Mapping

            #region VolunteerOrder To VolunteerOrderDto

            /*
             * Maps VolunteerOrder entity to VolunteerOrderDto.
             * Used when returning volunteer order data to the client.
             */
            CreateMap<VolunteerOrder, VolunteerOrderDto>()
                // Convert OrderStatus enum to string.
                // Example: OrderStatus.Pending => "Pending"
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));

            #endregion

            #region CreateVolunteerOrderDto To VolunteerOrder

            /*
             * Maps CreateVolunteerOrderDto to VolunteerOrder entity.
             * Used when creating a new volunteer order.
             */
            CreateMap<CreateVolunteerOrderDto, VolunteerOrder>()
                // Generate a new Id for the new volunteer order.
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(_ => Guid.NewGuid()))

                // VolunteerId should not come from the client.
                // It is taken from the authenticated user's JWT token in the service layer.
                .ForMember(dest => dest.VolunteerId,
                    opt => opt.Ignore())

                // Status should not come from the client.
                // New volunteer orders usually start with Pending status in the service layer.
                .ForMember(dest => dest.Status,
                    opt => opt.Ignore())

                // New volunteer orders should not be marked as deleted.
                // This value is controlled in the service layer.
                .ForMember(dest => dest.IsDeleted,
                    opt => opt.Ignore())

                // DeletedAt should stay empty when creating a new volunteer order.
                .ForMember(dest => dest.DeletedAt,
                    opt => opt.Ignore())

                // CreatedAt is set automatically in the service layer.
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore());

            #endregion

            #region UpdateVolunteerOrderDto To VolunteerOrder

            /*
             * Maps UpdateVolunteerOrderDto to an existing VolunteerOrder entity.
             * Used when updating an existing volunteer order.
             */
            CreateMap<UpdateVolunteerOrderDto, VolunteerOrder>()
                // Do not allow update DTO to change the entity Id.
                .ForMember(dest => dest.Id,
                    opt => opt.Ignore())

                // VolunteerId should not be changed from the update DTO.
                .ForMember(dest => dest.VolunteerId,
                    opt => opt.Ignore())

                // Status should not be changed from the normal update endpoint.
                // If needed, it should be changed through a separate business flow.
                .ForMember(dest => dest.Status,
                    opt => opt.Ignore())

                // Soft delete fields should not be changed from the normal update endpoint.
                .ForMember(dest => dest.IsDeleted,
                    opt => opt.Ignore())

                // DeletedAt should not be changed from the normal update endpoint.
                .ForMember(dest => dest.DeletedAt,
                    opt => opt.Ignore())

                // CreatedAt should keep its original value.
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.Ignore());

            #endregion

            #endregion

        }

        #endregion
    }
}