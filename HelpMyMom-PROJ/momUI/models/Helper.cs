using System;
using System.Collections.Generic;

namespace momUI.models;

public partial class Helper
{
    public int Id { get; set; }

    public string? FName { get; set; }

    public string? LName { get; set; }

    public string? Email { get; set; }

    public string? Specs { get; set; }

    public string? Description { get; set; }

    public DateOnly? Dob { get; set; }

    public double? Tokens { get; set; }

    public short? Banned { get; set; }

    public byte[]? Pfp { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
