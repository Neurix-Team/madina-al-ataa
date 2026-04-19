namespace GivingChampion.Common.DTO.Location
{
    public class UpdateLocationDto
    {
        public string? Name { get; set; }
        public int? RequiredLevel { get; set; }
        public string? Longitude { get; set; }
        public string? Latitude { get; set; }
    }
}