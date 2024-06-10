using System;
using System.Collections.Generic;

namespace AuthService.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public virtual ICollection<UsersInChat> UsersInChats { get; set; } = new List<UsersInChat>();
}
