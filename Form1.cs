using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Order;

namespace StarArmory
{
    public partial class StarArmory : Form
    {
        public StarArmory()
        {
            InitializeComponent();
            var env = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
            foreach (var mod in env.LoadOrder)
            {
                if (mod.Value.Mod != null)
                {
                    if (mod.Value.Mod.Armors != null)
                    {
                        if (mod.Value.Mod.Armors.Count > 0)
                        {
                            loadedMods.Items.Add(mod.Value.FileName,true);
                        }
                    }
                }
            }

            Armory.clothes = new List<IArmorGetter>();
            Armory.gameEnvironment = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
            Armory.immutableLoadOrderLinkCache = Armory.gameEnvironment.LoadOrder.ToImmutableLinkCache();



        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> checkedmods = new List<string>();
            foreach(var item in loadedMods.CheckedItems)
            {
                checkedmods.Add(item.ToString());
            }
            Armory.LoadClothes(checkedmods);

            ModKey newMod = new ModKey("StarArmoryPatch", ModType.Master);
            StarfieldMod myMod = new StarfieldMod(newMod, StarfieldRelease.Starfield);

            //Outfit.DoOutfits(myMod,checkedmods);
            LeveledItem.AddItemsToLevelledList(myMod,Armory.hats, 737735);//LL_Crowd_Hairs_Hats_Neon [LVLI:000B41C7]

            var targets = new List<uint>() { };
            targets.Add(1004639);// LL_Crowd_Clothes_Neon_Male[LVLI: 000F545F]
            targets.Add(684083);//LL_Crowd_Clothes_Neon_Female_Thin [LVLI:000A7033]
            targets.Add(1004638);//LL_Crowd_Clothes_Neon_Female[LVLI: 000F545E]
            targets.Add(684082);//LL_Crowd_Clothes_Neon_Female_Overweight [LVLI:000A7032]
            for (int i = 0; i < targets.Count; i++) {
                LeveledItem.AddItemsToLevelledList(myMod, Armory.clothes, targets[i]);
            }

            LeveledItem.AddItemsToLevelledList(myMod, Armory.spacesuits, 496138);//LL_Armor_Spacer_Body_Leveled_Any [LVLI:0007920A]
            LeveledItem.AddItemsToLevelledList(myMod, Armory.spacehelmets, 496140);//LL_Armor_Spacer_Helmet_Leveled_Any [LVLI:0007920C]
            LeveledItem.AddItemsToLevelledList(myMod, Armory.boostpacks, 496136);//LL_Armor_Spacer_Backpack_Leveled_Any[LVLI: 00079208]

            //Export
            Armory.gameEnvironment.Dispose();
            myMod.WriteToBinary(Armory.gameEnvironment.DataFolderPath + "\\StarArmoryPatch.esm", new BinaryWriteParameters()
            {
                MastersListOrdering = new MastersListOrderingByLoadOrder(Armory.gameEnvironment.LoadOrder)
            });
            MessageBox.Show("Exported StarArmoryPatch.esm to Data Folder");

        }
    }
}