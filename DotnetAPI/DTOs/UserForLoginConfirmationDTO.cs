namespace DotnetAPI.DTOs
{
    partial class UserForLoginConfirmationDTO
    {
        byte[] PasswordHash { get; set; } = new byte[0];
        byte[] PasswordSalt { get; set; } = new byte[0];
    }
}
