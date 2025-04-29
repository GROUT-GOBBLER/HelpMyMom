using System;
using System.Collections.Generic;

namespace momUI.models;

public partial class Ticket
{
    public int Id { get; set; }

    public int? MomId { get; set; }

    public int? ChildId { get; set; }

    public int? HelperId { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public string? LogForm { get; set; }

    public int? ReviewId { get; set; }

    public virtual ICollection<ChatLog> ChatLogs { get; set; } = new List<ChatLog>();

    public virtual Child? Child { get; set; }

    public virtual Helper? Helper { get; set; }

    public virtual Mother? Mom { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual Review? Review { get; set; }
}