using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Order;
using System.Reactive.Joins;
using System;

namespace StarArmory
{
    public partial class StarArmory : Form
    {
        public static StarfieldMod myMod;

        public StarArmory()
        {
            InitializeComponent();
            using (var env = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield))
            {
                foreach (var mod in env.LoadOrder)
                {
                    if (mod.Value.ModKey.FileName == "Starfield.esm") continue;
                    if (mod.Value.Mod != null)
                    {
                        if (mod.Value.Mod.Armors != null)
                        {
                            if (mod.Value.Mod.Armors.Count > 0)
                            {
                                loadedMods.Items.Add(mod.Value.FileName, true);
                            }
                        }
                    }
                }
            }

            Armory.clothes = new List<IArmorGetter>();

            Armory.plans = new List<FactionPlan>();
            Armory.factions = new Dictionary<string, Faction>();

            string[] fileEntries = Directory.GetFiles("Factions/");
            foreach (var entry in fileEntries)
            {
                var faction = YamlImporter.getObjectFromFile<Faction>(entry);
                Armory.factions.Add(faction.Name, faction);
                FactionList.Items.Add(faction.Name);
            }

        }
        private void AddToPlanButton(object sender, EventArgs e)
        {
            var factionplan = new FactionPlan();
            factionplan.faction = Armory.factions[FactionList.SelectedItem.ToString()];
            List<string> checkedmods = new List<string>();
            foreach (var item in loadedMods.CheckedItems)
            {
                checkedmods.Add(item.ToString());
            }

            factionplan.mods = checkedmods;
            Armory.plans.Add(factionplan);

            factionPlanTree.Nodes.Clear();

            factionPlanTree.BeginUpdate();
            factionPlanTree.Nodes.Add("Plan");
            for (int i = 0; i < Armory.plans.Count; i++)
            {
                factionPlanTree.Nodes[0].Nodes.Add(Armory.plans[i].faction.Name);
                for (int j = 0; j < Armory.plans[i].mods.Count; j++)
                {
                    factionPlanTree.Nodes[0].Nodes[i].Nodes.Add(Armory.plans[i].mods[j]);
                }
            }
            factionPlanTree.EndUpdate();
        }

        private void ExportESMButton(object sender, EventArgs e)
        {

            ModKey newMod = new ModKey("StarArmoryPatch", ModType.Master);
            myMod = new StarfieldMod(newMod, StarfieldRelease.Starfield);

            foreach (var plan in Armory.plans)
            {
                Armory.Clear();
                Armory.LoadClothes(plan.mods);
                if (plan.faction.Hats != null)
                {
                    foreach (var hat in plan.faction.Hats)
                    {
                        LeveledItem.AddItemsToLevelledList(myMod, Armory.hats, hat.modname, hat.formkey);
                    }
                }
                if (plan.faction.Clothes != null)
                {
                    foreach (var clothes in plan.faction.Clothes)
                    {
                        LeveledItem.AddItemsToLevelledList(myMod, Armory.clothes, clothes.modname, clothes.formkey);
                    }
                }
                if (plan.faction.Spacesuits != null)
                {
                    foreach (var suit in plan.faction.Spacesuits)
                    {
                        LeveledItem.AddItemsToLevelledList(myMod, Armory.spacesuits, suit.modname, suit.formkey);
                    }
                }
                if (plan.faction.SpaceHelmets != null)
                {
                    foreach (var helm in plan.faction.SpaceHelmets)
                    {
                        LeveledItem.AddItemsToLevelledList(myMod, Armory.spacehelmets, helm.modname, helm.formkey);
                    }
                }
                if (plan.faction.Boostpacks != null)
                {
                    foreach (var pack in plan.faction.Boostpacks)
                    {
                        LeveledItem.AddItemsToLevelledList(myMod, Armory.boostpacks, pack.modname, pack.formkey);
                    }
                }
            }
            string datapath = "";
            using (var env = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield))
            {
                datapath = env.DataFolderPath;
            }
            //Export
            try
            {
                myMod.WriteToBinary(datapath + "\\StarArmoryPatch.esm");
                MessageBox.Show("Exported StarArmoryPatch.esm to Data Folder");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}