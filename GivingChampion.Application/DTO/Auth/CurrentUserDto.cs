namespace GivingChampion.Application.DTO.Auth
{
    public class CurrentUserDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string[] Roles { get; set; } = [];
    }
}
