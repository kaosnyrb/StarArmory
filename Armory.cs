﻿using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

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
                            var link = immutableLoadOrderLinkCache.Resolve<IArmorGetter>(armor.FormKey);
                            if (armor.HasKeyword(Apparel))
                            {
                                //These are based on various armors i've in my mod list. There will be more here.
                                if (armor.FirstPersonFlags.Value != FirstPersonFlag.Backpack &&
                                    armor.FirstPersonFlags.Value != FirstPersonFlag.SSBackpackMisc &&
                                    armor.FirstPersonFlags.Value != FirstPersonFlag.SSMisc1 &&
                                    armor.FirstPersonFlags.Value != FirstPersonFlag.SSMisc2 &&
                                    armor.FirstPersonFlags.Value != FirstPersonFlag.SSMisc3 &&
                                    armor.FirstPersonFlags.Value != FirstPersonFlag.AddonRig &&
                                    armor.FirstPersonFlags.Value != FirstPersonFlag.SSAddonRig &&
                                    armor.FirstPersonFlags.Value != FirstPersonFlag.Hat &&
                                    (ulong) armor.FirstPersonFlags.Value != (long)288230376151711744 //"Unused"
                                    )

                                {
                                    clothes.Add(link);
                                }
                            }
                            if (armor.HasKeyword(Head))
                            {
                                hats.Add(link);
                            }
                            if (armor.HasKeyword(spacesuit))
                            {
                                //Check that it actually is a spacesuit.
                                if (armor.FirstPersonFlags.Value == FirstPersonFlag.SSBODY)
                                {
                                    spacesuits.Add(link);
                                }
                            }
                            if (armor.HasKeyword(spacehelmet))
                            {
                                //Check that it actually is a helmet
                                if (armor.FirstPersonFlags.Value == FirstPersonFlag.SSHelmet ||
                                    armor.FirstPersonFlags.Value == FirstPersonFlag.Eyepatch
                                    )
                                {
                                    spacehelmets.Add(link);
                                }
                            }
                            if (armor.HasKeyword(boostpack))
                            {
                                boostpacks.Add(link);
                            }
                        }
                    }
                }
            }
        }
    }
}
