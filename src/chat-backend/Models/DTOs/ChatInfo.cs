namespace ChatApp.Models.DTOs;

public class ChatInfoDto
{
    public int ChatId { get; set; }
    public string Title { get; set; }
    public DateOnly Created { get; set; }
}