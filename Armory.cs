using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Noggog;
using System.Linq;

namespace StarArmory
{
    class Armory
    {        
        public static List<IArmorGetter> clothes { get; set; } = new List<IArmorGetter>();
        public static List<IArmorGetter> hats { get; set; } = new List<IArmorGetter>();
        public static List<IArmorGetter> spacesuits { get; set; } = new List<IArmorGetter>();
        public static List<IArmorGetter> spacehelmets { get; set; } = new List<IArmorGetter>();
        public static List<IArmorGetter> boostpacks { get; set; } = new List<IArmorGetter>();

        public static List<FactionPlan> plans;
        public static Dictionary<string, Faction> factions;


        public static void Clear()
        {
            clothes.Clear();
            hats.Clear();
            spacesuits.Clear();
            spacehelmets.Clear();
            boostpacks.Clear();
        }

        public static void LoadClothes(List<string> mods)
        {
            using (var env = StarArmory.GetGameEnvironment())
            {
                var immutableLoadOrderLinkCache = env.LoadOrder.ToImmutableLinkCache();
                FormKey Apparel = new FormKey(env.LoadOrder[0].ModKey, 918668);//ArmorTypeApparelOrNakedBody[KYWD: 000E048C]
                FormKey Head = new FormKey(env.LoadOrder[0].ModKey, 918667);//ArmorTypeApparelHead [KYWD:000E048B]
                FormKey spacesuit = new FormKey(env.LoadOrder[0].ModKey, 2344896);//ArmorTypeSpacesuitBody [KYWD:0023C7C0]
                FormKey spacehelmet = new FormKey(env.LoadOrder[0].ModKey, 2344897);//ArmorTypeSpacesuitBackpack [KYWD:0023C7BF]
                FormKey boostpack = new FormKey(env.LoadOrder[0].ModKey, 2344895);//ArmorTypeSpacesuitHelmet[KYWD: 0023C7C1]


                foreach (var mod in env.LoadOrder)
                {
                    if (!mods.Contains(mod.Value.FileName))
                    {
                        continue;
                    }
                    if (mod.Value.Mod != null)
                    {
                        var armours = mod.Value.Mod.Armors.ToList();
                        foreach (var armor in armours)
                        {
                            bool added = false;
                            bool Keyword = false;
                            var link = immutableLoadOrderLinkCache.Resolve<IArmorGetter>(armor.FormKey);
                            if (armor.HasKeyword(Apparel))
                            {
                                //These are based on various armors i've in my mod list. There will be more here.
                                if (!StarArmory.settingsManager.clothesFirstPersonFlagsBlacklist.Contains(armor.FirstPersonFlags.Value) &&
                                    !StarArmory.settingsManager.clothesFirstPersonFlagsLongBlacklist.Contains((ulong)armor.FirstPersonFlags.Value))
                                {
                                    clothes.Add(link);
                                    added = true;
                                }
                                Keyword = true;
                            }
                            if (armor.HasKeyword(Head))
                            {
                                hats.Add(link);
                                added = true;
                                Keyword = true;
                            }
                            if (armor.HasKeyword(spacesuit))
                            {
                                //Check that it actually is a spacesuit.
                                if (armor.FirstPersonFlags.Value == FirstPersonFlag.SSBODY)
                                {
                                    spacesuits.Add(link);
                                    added = true;
                                }
                                Keyword = true;
                            }
                            if (armor.HasKeyword(spacehelmet))
                            {
                                //Check that it actually is a helmet
                                if (armor.FirstPersonFlags.Value == FirstPersonFlag.SSHelmet ||
                                    armor.FirstPersonFlags.Value == FirstPersonFlag.Eyepatch
                                    )
                                {
                                    spacehelmets.Add(link);
                                    added = true;
                                }
                                Keyword = true;
                            }
                            if (armor.HasKeyword(boostpack))
                            {
                                boostpacks.Add(link);
                                added = true;
                                Keyword = true;
                            }
                            if (!added)
                            {
                                StarArmory.logr.WriteLine("Didn't load armor " + armor.EditorID + " from mod " + mod.Value.FileName + " Keyword found: " + Keyword.ToString() + " | FirstPersonFlags checks : " + added.ToString());
                            }
                        }
                    }
                }
                StarArmory.logr.Flush();
            }
        }
    }
}
