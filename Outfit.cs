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
    public class Outfit
    {
        public static void DoOutfits(StarfieldMod myMod, List<string> mods)
        {
            foreach (var newoutfit in Armory.gameEnvironment.LoadOrder[0].Mod.Outfits.ToList())
            {
                if (!newoutfit.EditorID.Contains("Crowd"))
                {
                    continue;
                }
                Random rand = new Random();

                var outfit = Armory.immutableLoadOrderLinkCache.Resolve<IOutfitGetter>(newoutfit.FormKey);

                var newlist = myMod.Outfits.GetOrAddAsOverride(outfit);
                newlist.Items.Clear();

                int hatrand = rand.Next(Armory.hats.Count);
                newlist.Items.Add(Armory.hats[hatrand].ToLink<ILeveledItemGetter>());

                int clothesrand = rand.Next(Armory.clothes.Count);
                newlist.Items.Add(Armory.clothes[clothesrand].ToLink<ILeveledItemGetter>());
            }
        }
    }
}
