using System;
using System.Collections.Generic;

namespace ChatApp.Models;

public partial class Chat
{
    public int ChatId { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly CreationDate { get; set; }

    public virtual ICollection<Message> Messeges { get; set; } = new List<Message>();

    public virtual ICollection<UsersInChats> UsersInChats { get; set; } = new List<UsersInChats>();
}
