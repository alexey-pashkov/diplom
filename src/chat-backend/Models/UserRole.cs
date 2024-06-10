using System;
using System.Collections.Generic;

namespace ChatApp.Models;

public partial class UserRole
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<UsersInChats> UsersInChats { get; set; } = new List<UsersInChats>();
}
