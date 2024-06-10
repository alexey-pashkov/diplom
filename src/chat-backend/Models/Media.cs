using System;
using System.Collections.Generic;

namespace ChatApp.Models;

public partial class Media
{
    public int MediaId { get; set; }

    public int MessegeId { get; set; }

    public string Location { get; set; } = null!;

    public virtual Message Messege { get; set; } = null!;
}
