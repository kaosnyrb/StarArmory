using Mutagen.Bethesda.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarArmory
{
    class Faction
    {
        public string Name { get; set; }
        public List<SimpleForm> Hats { get; set; }
        public List<SimpleForm> Clothes { get; set; }
        public List<SimpleForm> Spacesuits { get; set; }
        public List<SimpleForm> SpaceHelmets { get; set; }
        public List<SimpleForm> Boostpacks { get; set; }
    }

    class SimpleForm
    {
        public SimpleForm(string modname, uint formkey)
        {
            this.modname = modname;
            this.formkey = formkey;
        }

        public SimpleForm() { }

        public uint formkey { get; set; }
        public string modname { get; set; }
    }

    class FactionPlan
    {
        public Faction faction { get; set; }

        public List<string> mods { get; set; }
    }
}
