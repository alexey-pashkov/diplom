using System;
using System.Collections.Generic;

namespace AuthService.Models;

public partial class Chat
{
    public int ChatId { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly CreationDate { get; set; }

    public virtual ICollection<UsersInChat> UsersInChats { get; set; } = new List<UsersInChat>();
}
