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
        public static void AddItemsToLevelledList(StarfieldMod myMod, List<IArmorGetter> newitems, uint levellist)
        {
            FormKey formKey = new FormKey(Armory.gameEnvironment.LoadOrder[0].ModKey, levellist);
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
