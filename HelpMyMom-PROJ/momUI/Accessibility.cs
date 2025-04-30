using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace momUI
{
    internal class Accessibility
    {
        private static Accessibility accessibilitySettings;

        public int fontsize { get; set; }
        
        private Accessibility(int fontsize)
        {
            this.fontsize = fontsize;
        }
      
        public static Accessibility getAccessibilitySettings()
        {
            if (accessibilitySettings == null)
            {
                accessibilitySettings = new Accessibility(15);
            }
          
            return accessibilitySettings;
        }

        public void setFontSize(int fontsize)
        {
            this.fontsize = fontsize;
        }

    }
}