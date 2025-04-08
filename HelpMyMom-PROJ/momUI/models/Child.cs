using System;
using System.Collections.Generic;

namespace momUI.models;

public partial class Child
{
    public int Id { get; set; }

    public string? FName { get; set; }

    public string? LName { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Relationship> Relationships { get; set; } = new List<Relationship>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
