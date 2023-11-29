using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

using Mutagen.Bethesda;
using Noggog;

namespace StarArmory
{
    class LeveledItem
    {
        public static void AddItemsToLevelledList(StarfieldMod myMod, List<IArmorGetter> newitems, string filename, uint levellist)
        {
            using (var env = StarArmory.GetGameEnvironment())
            {
                var immutableLoadOrderLinkCache = env.LoadOrder.ToImmutableLinkCache();

                ModKey key = new ModKey();
                bool found = false;
                for (int i = 0; i < env.LoadOrder.Count; i++)
                {
                    if (env.LoadOrder[i].FileName == filename)
                    {
                        key = env.LoadOrder[i].ModKey;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    new Exception("Couldn't file mod name " + filename + " in load order! Check the factions yamls for the error file.");
                }
                FormKey formKey = new FormKey(key, levellist);
                var citizenclothes = immutableLoadOrderLinkCache.Resolve<ILeveledItemGetter>(formKey);
                //Remove any existing patchs.
                bool result = myMod.LeveledItems.Remove(formKey);
                var newlist = myMod.LeveledItems.GetOrAddAsOverride(citizenclothes);
                for (int i = 0; i < newitems.Count; i++)
                {
                    newlist.Entries.Add(new LeveledItemEntry()
                    {
                        Level = 1,
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
