using AutoMapper;
using GivingChampion.Common.DTO.Partner;
using GivingChampion.Common.DTO.Partner;
using GivingChampion.Common.DTO.PartnerDto;
using GivingChampion.Common.DTO.ServiceRequestDto;
using GivingChampion.Common.Enums;
using GivingChampion.Domain.Entities;
using GivingChampion.Common.DTO.ServiceRequestDto;
using GivingChampion.Common.DTO.VolunteerDto;
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
            CreateMap<Partner, PartnerDto>()
                .ForMember(dest => dest.OrgTypeName,
                           opt => opt.MapFrom(src => src.OrgType.ToString()));

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
            CreateMap<UpdatePartnerDto, Partner>()
                .ForMember(dest => dest.UpdatedAt,
                           opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted,
                           opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt,
                           opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                           opt => opt.Ignore())
                .ForMember(dest => dest.Verified,
                           opt => opt.Ignore())
                .ForMember(dest => dest.ProjectsCount,
                           opt => opt.Ignore());

            #endregion

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
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow)) // Set creation time
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId)) // Set UserId passed from Admin/Claims
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Ignore UpdatedAt as it's not needed on creation
                .ForMember(dest => dest.Availability, opt => opt.MapFrom(src => src.Availability)) // Map from DTO to Entity
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills)); // Map from DTO to Entity

            // Map UpdateVolunteerDto to Volunteer (Updating an existing Volunteer)
            CreateMap<UpdateVolunteerDto, Volunteer>()
                .ForMember(dest => dest.TotalHours, opt => opt.MapFrom(src => src.TotalHours)) // Update total hours
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills)) // Update skills
                .ForMember(dest => dest.Availability, opt => opt.MapFrom(src => src.Availability)) // Update availability
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // UserId should remain the same as when initially created
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow)); // Set updated time

            #endregion

>>>>>>>>> Temporary merge branch 2
        }


    }
}