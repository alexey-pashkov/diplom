using System;
using System.Collections.Generic;

namespace AuthService.Models;

public partial class UsersInChat
{
    public int UserId { get; set; }

    public int ChatId { get; set; }

    public int UserRole { get; set; }

    public virtual Chat Chat { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual UserRole UserRoleNavigation { get; set; } = null!;
}
