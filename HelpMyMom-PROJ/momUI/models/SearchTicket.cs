using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace momUI.models
{
    internal class SearchTicket
    {
        public int Id { get; set; }

        public string? MomName { get; set; }

        public string? HelperName { get; set; }

        public string? Status { get; set; }

        public string? Details { get; set; }

        public int TextOffset { get; set; } = 0;

        public int headerFontOut => TextOffset + 25;
        public int normalFontOut => TextOffset + 18;
    }
}