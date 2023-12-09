using Mutagen.Bethesda.Starfield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarArmory
{
    public class SettingsManager
    {
        public float HatChance { get; set; }

        public string DataPath { get; set; }

        public SettingsManager()
        {
            HatChance = 50f;
            DataPath = "../";
        }
    }
}
