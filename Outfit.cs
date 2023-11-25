using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
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
        public static void DoOutfits(List<string> mods)
        {
            //Setup
            var env = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
            var starfieldesmKey = env.LoadOrder[0].ModKey;
            var linkCache = env.LoadOrder.ToImmutableLinkCache();
            ModKey newMod = new ModKey("StarArmoryPatch", ModType.Master);
            StarfieldMod myMod = new StarfieldMod(newMod, StarfieldRelease.Starfield);

            //Outfit_Clothes_Neon_Security_with_Helmet [OTFT:00133D9D]
            FormKey formKey = new FormKey(starfieldesmKey, 1260957);
            
            foreach (var newoutfit in env.LoadOrder[0].Mod.Outfits.ToList())
            {
                var outfit = linkCache.Resolve<IOutfitGetter>(newoutfit.FormKey);
                var newlist = myMod.Outfits.GetOrAddAsOverride(outfit);
                newlist.Items.Clear();

                foreach (var mod in env.LoadOrder)
                {
                    if (mod.Value.FileName == "Starfield.esm")
                    {
                        continue;
                    }
                    if (!mods.Contains(mod.Value.FileName))
                    {
                        continue;
                    }
                    if (mod.Value.Mod != null)
                    {
                        var armours = mod.Value.Mod.Armors.ToList();
                        foreach (var armor in armours)
                        {
                            var link = linkCache.Resolve<IArmorGetter>(armor.FormKey);
                            newlist.Items.Add(link.ToLink<ILeveledItemGetter>());
                        }
                    }
                }
            }

            env.Dispose();
            //Export
            myMod.WriteToBinary(env.DataFolderPath + "\\StarArmoryPatch.esm", new BinaryWriteParameters()
            {
                MastersListOrdering = new MastersListOrderingByLoadOrder(env.LoadOrder)
            });
            MessageBox.Show("Exported StarArmoryPatch.esm to Data Folder");
        }
    }
}
