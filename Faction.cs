using Mutagen.Bethesda.Plugins;

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
        public List<SimpleForm> OutfitSpacesuit { get; set; }
        public List<SimpleForm> OutfitClothes { get; set; }
        public List<SimpleForm> RangedWeapons { get; set; }
        public List<SimpleForm> MeleeWeapons { get; set; }
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

        //Optional but useful when making factions.
        public string editorId { get; set; }

        public bool? Helmet { get; set; }

        public FormKey GetFormKey()
        {
            string stripped = this.modname.Replace(".esm", "");
            ModKey modKey = new ModKey(stripped, ModType.Master);
            FormKey key = new FormKey(modKey, this.formkey);
            return key;
        }
    }

    class FactionPlan
    {
        public bool clearvanillaitems { get; set; }
        public Faction faction { get; set; }

        public List<string> mods { get; set; }
    }
}
