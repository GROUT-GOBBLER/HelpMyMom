using System;
using System.Collections.Generic;

namespace HelpMyMom_PROJ.models;

public partial class Review
{
    public int Id { get; set; }

    public int? HelperId { get; set; }

    public int? MomId { get; set; }

    public short? Stars { get; set; }

    public string? Text { get; set; }

    public virtual Helper? Helper { get; set; }

    public virtual Mother? Mom { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
