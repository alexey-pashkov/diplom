namespace AuthService.Models.DTOs;

public class UserInfo
{
    public int UserId { get; set; }
    public string Login { get; set; }

    public ICollection<Chat> Chats { get; set; }
}