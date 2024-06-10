using System;
using System.Collections.Generic;

namespace ChatApp.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public virtual ICollection<Message> Messeges { get; set; } = new List<Message>();

    public virtual ICollection<UsersInChats> UsersInChats { get; set; } = new List<UsersInChats>();
}
