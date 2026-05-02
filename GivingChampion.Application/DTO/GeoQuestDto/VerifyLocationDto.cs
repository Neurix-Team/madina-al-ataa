namespace GivingChampion.Application.DTO.GeoQuestDto
{
    public class VerifyLocationDto
    {
        public Guid UserGeoQuestId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}