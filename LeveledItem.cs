using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using System.Drawing.Drawing2D;
using static System.Windows.Forms.LinkLabel;

namespace StarArmory
{
    class LeveledItem
    {
        public static void AddItemsToLevelledList(StarfieldMod myMod, List<IArmorGetter> newitems, string filename, uint levellist)
        {
            ModKey key = new ModKey();
            bool found = false;
            for(int i =0; i < Armory.gameEnvironment.LoadOrder.Count; i++)
            {
                if (Armory.gameEnvironment.LoadOrder[0].FileName == filename)
                {
                    key = Armory.gameEnvironment.LoadOrder[0].ModKey;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                new Exception("Couldn't file mod name " + filename + " in load order! Check the factions yamls for the error file.");
            }
            FormKey formKey = new FormKey(key, levellist);
            var citizenclothes = Armory.immutableLoadOrderLinkCache.Resolve<ILeveledItemGetter>(formKey);
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
}
