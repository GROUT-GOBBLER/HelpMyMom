using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace momUI.models
{
    internal class SearchHelper
    {
        public string? FName { get; set; }

        public string? LName { get; set; }

        public string FullName => $"{FName ?? ""} {LName ?? ""}".Trim();

        public int Rating { get; set; } = 0;

        public string? Description { get; set; }

        public string FullDecriprion => $"{Rating.ToString() ?? ""} {Description ?? ""}".Trim();

        public string? Specs { get; set; }
    }
}
