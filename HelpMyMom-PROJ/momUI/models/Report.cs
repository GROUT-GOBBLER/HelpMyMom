using System;
using System.Collections.Generic;

namespace momUI.models;

public partial class Report
{
    public int Id { get; set; }

    public int? HelperId { get; set; }

    public int? MomId { get; set; }

    public int? ChildId { get; set; }

    public int? TicketId { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }

    public virtual Child? Child { get; set; }

    public virtual Helper? Helper { get; set; }

    public virtual Mother? Mom { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
