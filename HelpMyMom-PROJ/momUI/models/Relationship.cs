using System;
using System.Collections.Generic;

namespace momUI.models;

public partial class Relationship
{
    public int Id { get; set; }

    public int? MomId { get; set; }

    public int? ChildId { get; set; }

    public virtual Child? Child { get; set; }

    public virtual Mother? Mom { get; set; }
}