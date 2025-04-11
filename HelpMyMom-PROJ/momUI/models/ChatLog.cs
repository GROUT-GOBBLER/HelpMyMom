using System;
using System.Collections.Generic;

namespace momUI.models;

public partial class ChatLog
{
    public long Id { get; set; }

    public int? TicketId { get; set; }

    public DateTime? Time { get; set; }

    public string? IsMom { get; set; }

    public string? Text { get; set; }

    public byte[]? Image { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
