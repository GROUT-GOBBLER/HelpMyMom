using System;
using System.Collections.Generic;

namespace HelpMyMom_PROJ.models;

public partial class Account
{
    public string Username { get; set; } = null!;

    public string? Password { get; set; }

    public int? MomId { get; set; }

    public int? ChildId { get; set; }

    public int? HelperId { get; set; }
}
