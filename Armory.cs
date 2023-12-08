using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Noggog;
using System.IO.Abstractions;
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
        public static List<IWeaponGetter> ranged_weapons { get; set; } = new List<IWeaponGetter>();
        public static List<IWeaponGetter> melee_weapons { get; set; } = new List<IWeaponGetter>();

        public static List<FactionPlan> plans;
        public static Dictionary<string, Faction> factions;

        public static List<string> UpgradedItems;//This is to prevent adding multiple keywords.

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
            StarArmory.logr.WriteLine("Starting to Load Mod Content");
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
                        int itemsAdded = 0;
                        try
                        {
                            var armours = mod.Value.Mod.Armors.ToList();
                            StarArmory.logr.WriteLine("Loading Mod: " + mod.Value.FileName);
                            foreach (var armor in armours)
                            {
                                bool added = false;
                                bool Keyword = false;
                                var link = immutableLoadOrderLinkCache.Resolve<IArmorGetter>(armor.FormKey);
                                if (armor.HasKeyword(Apparel))
                                {
                                    StarArmory.logr.WriteLine("Processing Armor: " + armor.EditorID + " flags " + armor.FirstPersonFlags.Value);
                                    //These are based on various armors i've in my mod list. There will be more here.
                                    if (!StarArmory.settingsManager.clothesFirstPersonFlagsBlacklist.Contains(armor.FirstPersonFlags.Value) &&
                                        !StarArmory.settingsManager.clothesFirstPersonFlagsLongBlacklist.Contains((ulong)armor.FirstPersonFlags.Value))
                                    {
                                        clothes.Add(link);
                                        added = true;
                                        itemsAdded++;
                                    }
                                    else
                                    {
                                        //Things that aren't outfits are probably accessories.
                                        hats.Add(link);
                                        added = true;
                                        itemsAdded++;
                                    }
                                    Keyword = true;
                                }
                                if (armor.HasKeyword(Head))
                                {
                                    hats.Add(link);
                                    added = true;
                                    Keyword = true;
                                    itemsAdded++;
                                }
                                if (armor.HasKeyword(spacesuit))
                                {
                                    //Check that it actually is a spacesuit.
                                    if (armor.FirstPersonFlags.Value == FirstPersonFlag.SSBODY)
                                    {
                                        spacesuits.Add(link);
                                        added = true;
                                        itemsAdded++;
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
                                        itemsAdded++;
                                    }
                                    Keyword = true;
                                }
                                if (armor.HasKeyword(boostpack))
                                {
                                    boostpacks.Add(link);
                                    added = true;
                                    Keyword = true;
                                    itemsAdded++;
                                }
                                if (!added)
                                {
                                    StarArmory.logr.WriteLine("Didn't load armor " + armor.EditorID + " from mod " + mod.Value.FileName + " Keyword found: " + Keyword.ToString() + " | FirstPersonFlags checks : " + added.ToString());
                                }
                            }
                        }
                        catch (Exception ex) 
                        {
                            StarArmory.logr.WriteLine("Exception in Armor Processing in Mod: " + mod.Value.FileName);
                            StarArmory.logr.WriteLine("Exception: " + ex);
                        }


                        try
                        {
                            FormKey weapon_ranged = new FormKey(env.LoadOrder[0].ModKey, 177940);//WeaponTypeRanged [KYWD:0002B714]
                            FormKey weapon_melee = new FormKey(env.LoadOrder[0].ModKey, 303268);//WeaponTypeMelee1H [KYWD:0004A0A4]

                            var weapons = mod.Value.Mod.Weapons.ToList();
                            foreach (var weapon in weapons)
                            {
                                bool added = false;
                                var link = immutableLoadOrderLinkCache.Resolve<IWeaponGetter>(weapon.FormKey);
                                if (weapon.HasKeyword(weapon_ranged))
                                {
                                    ranged_weapons.Add(link);
                                    added = true;
                                    itemsAdded++;
                                }
                                if (weapon.HasKeyword(weapon_melee))
                                {
                                    melee_weapons.Add(link);
                                    added = true;
                                    itemsAdded++;
                                }
                                if (!added)
                                {
                                    StarArmory.logr.WriteLine("Didn't load weapon " + weapon.EditorID + " from mod " + mod.Value.FileName + " Keyword found: " + added.ToString());
                                }
                            }
                        } 
                        catch (Exception ex)
                        {
                            StarArmory.logr.WriteLine("Exception in Weapon Processing in Mod: " + mod.Value.FileName);
                            StarArmory.logr.WriteLine("Exception: " + ex);
                        }
                        StarArmory.logr.WriteLine("Finished procressing Mod: " + mod.Value.FileName);
                        StarArmory.logr.WriteLine(mod.Value.FileName + " contained " + itemsAdded + " valid items");
                    }
                }
                StarArmory.logr.WriteLine("Finished procressing mods.");
                StarArmory.logr.Flush();
            }
        }
    }
}
