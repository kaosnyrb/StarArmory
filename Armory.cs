using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Starfield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mutagen.Bethesda.FormKeys.Starfield.Starfield;

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
            using (var env = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield))
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
                            if (armor.HasKeyword(Apparel))
                            {
                                var link = immutableLoadOrderLinkCache.Resolve<IArmorGetter>(armor.FormKey);
                                clothes.Add(link);
                            }
                            if (armor.HasKeyword(Head))
                            {
                                var link = immutableLoadOrderLinkCache.Resolve<IArmorGetter>(armor.FormKey);
                                hats.Add(link);
                            }
                            if (armor.HasKeyword(spacesuit))
                            {
                                var link = immutableLoadOrderLinkCache.Resolve<IArmorGetter>(armor.FormKey);
                                spacesuits.Add(link);
                            }
                            if (armor.HasKeyword(spacehelmet))
                            {
                                var link = immutableLoadOrderLinkCache.Resolve<IArmorGetter>(armor.FormKey);
                                spacehelmets.Add(link);
                            }
                            if (armor.HasKeyword(boostpack))
                            {
                                var link = immutableLoadOrderLinkCache.Resolve<IArmorGetter>(armor.FormKey);
                                boostpacks.Add(link);
                            }
                        }
                    }
                }
            }
        }
    }
}
