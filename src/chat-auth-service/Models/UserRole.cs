using System;
using System.Collections.Generic;

namespace AuthService.Models;

public partial class UserRole
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<UsersInChat> UsersInChats { get; set; } = new List<UsersInChat>();
}
