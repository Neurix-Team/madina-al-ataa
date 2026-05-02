namespace GivingChampion.Application.DTO.User
{
    public class CreateUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDay { get; set; }
    }
}
