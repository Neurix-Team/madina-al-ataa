namespace GivingChampion.Application.DTO.User
{
    public class UpdateUser
    {
        public string FullName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime BirthDay { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
