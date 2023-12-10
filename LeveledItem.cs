using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

using Mutagen.Bethesda;
using Noggog;
using Microsoft.VisualBasic.Logging;

namespace StarArmory
{

    class LeveledItem
    {
        public static void AddItemsToLevelledList(StarfieldMod myMod, List<IArmorGetter> newitems, string esmname, uint levellist, bool clearlist = false)
        {
            using (var env = StarArmory.GetGameEnvironment())
            {
                var immutableLoadOrderLinkCache = env.LoadOrder.ToImmutableLinkCache();
                ModKey key = new ModKey();
                bool found = false;
                for (int i = 0; i < env.LoadOrder.Count; i++)
                {
                    if (env.LoadOrder[i].FileName == esmname)
                    {
                        key = env.LoadOrder[i].ModKey;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    new Exception("Couldn't file mod name " + esmname + " in load order! Check the factions yamls for the error file.");
                    StarArmory.log.Info("Couldn't file mod name " + esmname + " in load order! Check the factions yamls for the error file.");
                }
                FormKey formKey = new FormKey(key, levellist);
                var citizenclothes = immutableLoadOrderLinkCache.Resolve<ILeveledItemGetter>(formKey);
                var newlist = myMod.LeveledItems.GetOrAddAsOverride(citizenclothes);

                if (clearlist)
                {
                    newlist.Entries.Clear();
                }

                for (int i = 0; i < newitems.Count; i++)
                {
                    //Add Legendary and quality options to any armours that don't have them.
                    try
                    {
                        FormKey legrank1 = new FormKey(env.LoadOrder[0].ModKey, 1979080);//ap_Legendary_rank_1 [KYWD:001E32C8]                        
                        if (!Armory.UpgradedItems.Contains(newitems[i].EditorID))
                        {
                            if (newitems[i].AttachParentSlots == null)
                            {

                                var newArmour = myMod.Armors.GetOrAddAsOverride(newitems[i]);
                                newArmour.AttachParentSlots = new ExtendedList<IFormLinkGetter<IKeywordGetter>>
                            {
                                legrank1,
                                new FormKey(env.LoadOrder[0].ModKey, 3316412),//ap_Legendary_rank_2 [KYWD:00329ABC]
                                new FormKey(env.LoadOrder[0].ModKey, 3316413),//ap_Legendary_rank_3 [KYWD:00329ABD]
                                new FormKey(env.LoadOrder[0].ModKey, 1172161)//ap_armor_Quality "Quality" [KYWD:0011E2C1]
                            };
                                Armory.UpgradedItems.Add(newArmour.EditorID);
                            }
                            else if (!newitems[i].AttachParentSlots.Contains(legrank1))
                            {
                                var newArmour = myMod.Armors.GetOrAddAsOverride(newitems[i]);
                                newArmour.AttachParentSlots.Add(legrank1);
                                newArmour.AttachParentSlots.Add(new FormKey(env.LoadOrder[0].ModKey, 3316412));//ap_Legendary_rank_2 [KYWD:00329ABC]
                                newArmour.AttachParentSlots.Add(new FormKey(env.LoadOrder[0].ModKey, 3316413));//ap_Legendary_rank_3 [KYWD:00329ABD]
                                newArmour.AttachParentSlots.Add(new FormKey(env.LoadOrder[0].ModKey, 1172161));//ap_armor_Quality "Quality" [KYWD:0011E2C1]
                                Armory.UpgradedItems.Add(newArmour.EditorID);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        StarArmory.log.Info("Error added legendary flags: " + ex.Message + " " + newitems[i].EditorID);
                    }
                    short level = 1;
                    
                    if (newitems[i].ArmorRating > 0)
                    {
                        level = (short)(newitems[i].ArmorRating / 10);
                    }
                    newlist.Entries.Add(new LeveledItemEntry()
                    {
                        Level = level,
                        ChanceNone = new Noggog.Percent(0),
                        Count = 1,
                        Reference = newitems[i].ToLink<ILeveledItemGetter>()
                    });
                }
            }
        }

        public static void AddItemsToLevelledList(StarfieldMod myMod, List<IWeaponGetter> newitems, string esmname, uint levellist, bool clearlist = false)
        {
            using (var env = StarArmory.GetGameEnvironment())
            {
                var immutableLoadOrderLinkCache = env.LoadOrder.ToImmutableLinkCache();

                ModKey key = new ModKey();
                bool found = false;
                for (int i = 0; i < env.LoadOrder.Count; i++)
                {
                    if (env.LoadOrder[i].FileName == esmname)
                    {
                        key = env.LoadOrder[i].ModKey;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    new Exception("Couldn't find mod name " + esmname + " in load order! Check the factions yamls for the error file.");
                    StarArmory.log.Info("Couldn't find mod name " + esmname + " in load order! Check the factions yamls for the error file.");

                }
                FormKey formKey = new FormKey(key, levellist);
                var citizenclothes = immutableLoadOrderLinkCache.Resolve<ILeveledItemGetter>(formKey);

                //Remove any existing patchs.
                bool result = myMod.LeveledItems.Remove(formKey);
                var newlist = myMod.LeveledItems.GetOrAddAsOverride(citizenclothes);
                if (newlist.Flags == Mutagen.Bethesda.Starfield.LeveledItem.Flag.UseAll)
                {
                    newlist.Flags = 0;//Don't use all for weapons.
                }
                if (clearlist)
                {
                    newlist.Entries.Clear();
                }
                for (int i = 0; i < newitems.Count; i++)
                {
                    float speed = newitems[i].AttackSeconds;
                    if (speed == 0) speed = newitems[i].BaseSpeed;
                    float dps = newitems[i].AttackDamage * speed;                    
                    short level = (short)(dps / 10);
                    newlist.Entries.Add(new LeveledItemEntry()
                    {
                        Level = level,
                        ChanceNone = new Noggog.Percent(0),
                        Count = 1,
                        Reference = newitems[i].ToLink<ILeveledItemGetter>()
                    });
                }
            }
        }

        public static void AddItemsToList(StarfieldMod myMod, List<IArmorGetter> newitems, ExtendedList<LeveledItemEntry> leveledItemEntries, double ChanceNone)
        {
            for (int i = 0; i < newitems.Count; i++)
            {
                leveledItemEntries.Add(new LeveledItemEntry()
                {
                    Level = 1,
                    ChanceNone = new Noggog.Percent(ChanceNone),
                    Count = 1,
                    Reference = newitems[i].ToLink<ILeveledItemGetter>()
                });
            }
        }
    }
}
