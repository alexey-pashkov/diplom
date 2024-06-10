using System;
using System.Collections.Generic;

namespace ChatApp.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int? UserId { get; set; }

    public int ChatId { get; set; }

    public string? Content { get; set; }

    public virtual Chat Chat { get; set; } = null!;
    public virtual ICollection<Media> Media { get; set; }

    public virtual User? User { get; set; }
}
