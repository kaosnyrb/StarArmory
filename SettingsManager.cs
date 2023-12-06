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

        public List<FirstPersonFlag> clothesFirstPersonFlagsBlacklist { get; set; }
        public List<ulong> clothesFirstPersonFlagsLongBlacklist { get; set; }

        public string DataPath { get; set; }
        public SettingsManager()
        {
            HatChance = 50f;
            DataPath = "../";
            clothesFirstPersonFlagsBlacklist = new List<FirstPersonFlag>
            {
                FirstPersonFlag.Backpack,
                FirstPersonFlag.SSBackpackMisc,
                FirstPersonFlag.SSMisc1,
                FirstPersonFlag.SSMisc2,
                FirstPersonFlag.SSMisc3,
                FirstPersonFlag.AddonRig,
                FirstPersonFlag.SSAddonRig,
                FirstPersonFlag.Hat
            };
            clothesFirstPersonFlagsLongBlacklist = new List<ulong>
            {
                288230376151711744
            };
        }
    }
}
