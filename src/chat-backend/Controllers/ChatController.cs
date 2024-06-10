using System.Security.Claims;
using AutoMapper;
using ChatApp;
using ChatApp.Models;
using ChatApp.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;


[Route("api/[controller]")]
[ApiController]
[Authorize]
[GetIdentifiersFilter]
public class ChatController : Controller
{
    private readonly ChatDbContext dbContext;
    private readonly IHubContext<ChatHub> hubContext;
    private readonly IMapper mapper;

    public ChatController(ChatDbContext dbContext, IHubContext<ChatHub> hubContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.hubContext = hubContext;
        this.mapper = mapper;
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest newMessage)
    {
        int userId = (int)HttpContext.Items["UserId"]!;
        int chatId = (int)HttpContext.Items["ChatId"]!;

        Chat chat = await dbContext.Chats.FindAsync(chatId);
        User user = await dbContext.Users.FindAsync(userId);

        Message message = new Message
        {
            ChatId = chatId,
            UserId = userId,
            Content = newMessage.Content,
            Chat = chat,
            User = user
        };

        await dbContext.Messeges.AddAsync(message);
        chat.Messeges.Add(message);

        await dbContext.SaveChangesAsync();

        await hubContext.Clients
            .Group(chat.Name)
            .SendAsync("NewMessage", newMessage.Content);
        
        return Ok();
    }

    [HttpDelete("delete-message")]
    public async Task<IActionResult> DeleteMessage([FromBody] DeleteMessageRequest toDelete)
    {
        int chatId = (int)HttpContext.Items["ChatId"]!;

        Chat chat = await dbContext.Chats.FindAsync(chatId);
        Message message = await dbContext.Messeges.FindAsync(toDelete.MessageId);

        chat.Messeges.Remove(message);
        dbContext.Messeges.Remove(message);

        await dbContext.SaveChangesAsync();

        await hubContext.Clients
            .Group(chat.Name)
            .SendAsync("NewMessage", toDelete.MessageId);

        return Ok();
    }

    [HttpGet("messages")]
    public async Task<IActionResult> GetMesseges()
    {
        int chatId = (int)HttpContext.Items["ChatId"]!;

        Chat? chat = await dbContext.Chats.FindAsync(chatId);

        return Ok(chat.Messeges);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        int chatId = (int)HttpContext.Items["ChatId"]!;

        Chat? chat = await dbContext.Chats.FindAsync(chatId);

        var users = chat.UsersInChats
            .Select((item) => item.User)
            .ToList();

        return Ok(users);        
    }

    [HttpDelete("delete-user")]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest toDelete)
    {
        int chatId = (int)HttpContext.Items["ChatId"]!;

        Chat? chat = await dbContext.Chats.FindAsync(chatId);
        UsersInChats userInChat = chat.UsersInChats.Single((item) => item.UserId == toDelete.UserId);

        chat.UsersInChats.Remove(userInChat);
        dbContext.UsersInChats.Remove(userInChat);

        await dbContext.SaveChangesAsync();

        await hubContext.Clients
            .Group(chat.Name)
            .SendAsync("UserDeleted", userInChat.User.Login);

        return Ok();
    }
}