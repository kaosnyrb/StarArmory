using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Noggog;

namespace StarArmory
{
    public partial class StarArmory : Form
    {
        public static StarfieldMod myMod;
        public static SettingsManager settingsManager;
        public static IGameEnvironment<IStarfieldMod, IStarfieldModGetter> GetGameEnvironment()
        {
            try
            {
                var env = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
                //throw new Exception();
                return env;
            }
            catch
            {
                var env = GameEnvironment.Typical.Builder<IStarfieldMod, IStarfieldModGetter>(GameRelease.Starfield)
                    .TransformLoadOrderListings(x => x.Where(x => !x.ModKey.Name.Contains("StarArmory")))
                    .WithTargetDataFolder(new DirectoryPath("../")).Build();
                return env;
            }
        }

        public StarArmory()
        {
            InitializeComponent();
            try
            {
                using (var env = GetGameEnvironment())
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
                FactionList.SelectedItem = FactionList.Items[0];

                if (File.Exists("./Plan.yaml"))
                {
                    Armory.plans = YamlImporter.getObjectFromFile<List<FactionPlan>>("Plan.yaml");
                    factionPlanTree.Nodes.Clear();
                    factionPlanTree.BeginUpdate();
                    factionPlanTree.Nodes.Add("Plan");
                    for (int i = 0; i < Armory.plans.Count; i++)
                    {
                        Armory.plans[i].faction = Armory.factions[Armory.plans[i].faction.Name];
                        factionPlanTree.Nodes[0].Nodes.Add(Armory.plans[i].faction.Name);
                        for (int j = 0; j < Armory.plans[i].mods.Count; j++)
                        {
                            factionPlanTree.Nodes[0].Nodes[i].Nodes.Add(Armory.plans[i].mods[j]);
                        }
                    }
                    factionPlanTree.Nodes[0].Expand();
                    factionPlanTree.EndUpdate();
                }
                settingsManager = new SettingsManager();
                if (!File.Exists("./Settings.yaml"))
                {
                    YamlExporter.WriteObjToYamlFile("Settings.yaml", settingsManager);
                }
                else
                {
                    settingsManager = YamlImporter.getObjectFromFile<SettingsManager>("Settings.yaml");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AddToPlanButton(object sender, EventArgs e)
        {
            try
            {
                var factionplan = new FactionPlan();
                factionplan.faction = Armory.factions[FactionList.SelectedItem.ToString()];
                List<string> checkedmods = new List<string>();
                foreach (var item in loadedMods.CheckedItems)
                {
                    checkedmods.Add(item.ToString());
                }

                factionplan.mods = checkedmods;
                for (int i = 0; i < Armory.plans.Count; i++)
                {
                    if (Armory.plans[i].faction.Name == factionplan.faction.Name)
                    {
                        Armory.plans.RemoveAt(i);
                    }
                }

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
                factionPlanTree.Nodes[0].Expand();
                factionPlanTree.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExportESMButton(object sender, EventArgs e)
        {
            try
            {
                YamlExporter.WriteObjToYamlFile("Plan.yaml", Armory.plans);
                string datapath = "";
                using (var env = GetGameEnvironment())
                {
                    datapath = env.DataFolderPath;
                }
                File.Delete(datapath + "\\StarArmoryPatch.esm");

                ModKey newMod = new ModKey("StarArmoryPatch", ModType.Master);
                myMod = new StarfieldMod(newMod, StarfieldRelease.Starfield);

                foreach (var plan in Armory.plans)
                {
                    Armory.Clear();
                    Armory.LoadClothes(plan.mods);
                    //Levelled Lists
                    //Here we inject the modded items into the levelled lists defined in the faction files.
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
                    //Outfits
                    //Outfits have another layer of complexity to the levelled lists
                    //In vanilla some outfits link directly to armor, things like the starborn.
                    //We need to create a new levelled list then replace the outfit with these lists.
                    //We also want to peserve the original outfit as an option.
                    using (var env = StarArmory.GetGameEnvironment())
                    {
                        BuildSpaceOutfit(plan, env);
                        BuildClothesOutfit(plan, env);
                    }
                }
                //Export
                myMod.WriteToBinary(datapath + "\\StarArmoryPatch.esm");
                MessageBox.Show("Exported StarArmoryPatch.esm to Data Folder");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void BuildSpaceOutfit(FactionPlan plan, IGameEnvironment<IStarfieldMod, IStarfieldModGetter> env)
        {
            var immutableLoadOrderLinkCache = env.LoadOrder.ToImmutableLinkCache();
            FormKey spacesuit = new FormKey(env.LoadOrder[0].ModKey, 2344896);//ArmorTypeSpacesuitBody [KYWD:0023C7C0]
            FormKey spacehelmet = new FormKey(env.LoadOrder[0].ModKey, 2344897);//ArmorTypeSpacesuitBackpack [KYWD:0023C7BF]
            FormKey boostpack = new FormKey(env.LoadOrder[0].ModKey, 2344895);//ArmorTypeSpacesuitHelmet[KYWD: 0023C7C1]

            if (plan.faction.OutfitSpacesuit != null)
            {
                foreach (var outfit in plan.faction.OutfitSpacesuit)
                {
                    FormKey key = outfit.GetFormKey();
                    //grab items in outfit and save for later
                    var link = immutableLoadOrderLinkCache.Resolve<IOutfitGetter>(key);
                    var olditems = link.Items.ToList();
                    List<IArmorGetter> spacesuits = new List<IArmorGetter>();
                    List<IArmorGetter> spacehelmets = new List<IArmorGetter>();
                    List<IArmorGetter> boostpacks = new List<IArmorGetter>();
                    foreach (var item in olditems)
                    {
                        IArmorGetter? armorGetter;
                        immutableLoadOrderLinkCache.TryResolve<IArmorGetter>(item.FormKey, out armorGetter);
                        if (armorGetter != null)
                        {
                            // Note that things can multiple keywords, IE Starborn spacesuits count as suits/helmets and boost
                            if (armorGetter.HasKeyword(spacesuit))
                            {
                                spacesuits.Add(armorGetter);
                            }
                            else if (armorGetter.HasKeyword(spacehelmet))
                            {
                                spacehelmets.Add(armorGetter);
                            }
                            else if (armorGetter.HasKeyword(boostpack))
                            {
                                boostpacks.Add(armorGetter);
                            }
                        }
                    }
                    //clear outfit
                    var newoutfit = myMod.Outfits.GetOrAddAsOverride(link);
                    newoutfit.Items.Clear();

                    var ListOutfitSpacesuit = myMod.LeveledItems.AddNew();
                    ListOutfitSpacesuit.EditorID = link.EditorID + "_SA_spacesuit";
                    ListOutfitSpacesuit.Entries = new ExtendedList<LeveledItemEntry>();

                    var ListOutfitSpacehelmets = myMod.LeveledItems.AddNew();
                    ListOutfitSpacehelmets.EditorID = link.EditorID + "_SA_spacehelmets";
                    ListOutfitSpacehelmets.Entries = new ExtendedList<LeveledItemEntry>();

                    var ListOutfitBoostPacks = myMod.LeveledItems.AddNew();
                    ListOutfitBoostPacks.EditorID = link.EditorID + "_SA_boostpacks";
                    ListOutfitBoostPacks.Entries = new ExtendedList<LeveledItemEntry>();
                    //add vanilla items from 1 to levelled list
                    foreach (var suit in spacesuits)
                    {
                        ListOutfitSpacesuit.Entries.Add(new LeveledItemEntry()
                        {
                            Level = 1,
                            ChanceNone = new Noggog.Percent(0),
                            Count = 1,
                            Reference = suit.ToLink<ILeveledItemGetter>()
                        });
                    }
                    foreach (var helmets in spacehelmets)
                    {
                        ListOutfitSpacehelmets.Entries.Add(new LeveledItemEntry()
                        {
                            Level = 1,
                            ChanceNone = new Noggog.Percent(0),
                            Count = 1,
                            Reference = helmets.ToLink<ILeveledItemGetter>()
                        });
                    }
                    foreach (var boost in boostpacks)
                    {
                        ListOutfitBoostPacks.Entries.Add(new LeveledItemEntry()
                        {
                            Level = 1,
                            ChanceNone = new Noggog.Percent(0),
                            Count = 1,
                            Reference = boost.ToLink<ILeveledItemGetter>()
                        });
                    }

                    LeveledItem.AddItemsToList(myMod, Armory.spacesuits, ListOutfitSpacesuit.Entries, 0);
                    LeveledItem.AddItemsToList(myMod, Armory.spacehelmets, ListOutfitSpacehelmets.Entries, 0);
                    LeveledItem.AddItemsToList(myMod, Armory.boostpacks, ListOutfitBoostPacks.Entries, 0);

                    newoutfit.Items.Add(ListOutfitSpacesuit);
                    newoutfit.Items.Add(ListOutfitSpacehelmets);
                    newoutfit.Items.Add(ListOutfitBoostPacks);
                }
            }
        }
        private static void BuildClothesOutfit(FactionPlan plan, IGameEnvironment<IStarfieldMod, IStarfieldModGetter> env)
        {
            var immutableLoadOrderLinkCache = env.LoadOrder.ToImmutableLinkCache();
            FormKey Apparel = new FormKey(env.LoadOrder[0].ModKey, 918668);//ArmorTypeApparelOrNakedBody[KYWD: 000E048C]
            FormKey Head = new FormKey(env.LoadOrder[0].ModKey, 918667);//ArmorTypeApparelHead [KYWD:000E048B]

            if (plan.faction.OutfitClothes != null)
            {
                foreach (var outfit in plan.faction.OutfitClothes)
                {
                    FormKey key = outfit.GetFormKey();
                    //grab items in outfit and save for later
                    var link = immutableLoadOrderLinkCache.Resolve<IOutfitGetter>(key);
                    var olditems = link.Items.ToList();
                    List<IArmorGetter> clothes = new List<IArmorGetter>();
                    List<IArmorGetter> hats = new List<IArmorGetter>();
                    foreach (var item in olditems)
                    {
                        IArmorGetter? armorGetter;
                        immutableLoadOrderLinkCache.TryResolve<IArmorGetter>(item.FormKey, out armorGetter);
                        if (armorGetter != null)
                        {
                            // Note that things can multiple keywords, IE Starborn spacesuits count as suits/helmets and boost
                            if (armorGetter.HasKeyword(Apparel))
                            {
                                clothes.Add(armorGetter);
                            }
                            else if (armorGetter.HasKeyword(Head))
                            {
                                hats.Add(armorGetter);
                            }
                        }
                    }
                    //clear outfit
                    var newoutfit = myMod.Outfits.GetOrAddAsOverride(link);
                    newoutfit.Items.Clear();

                    //create levelled list for each category for this outfit

                    var ListOutfitHats = myMod.LeveledItems.AddNew();
                    ListOutfitHats.EditorID = link.EditorID + "_SA_hats";
                    ListOutfitHats.Entries = new ExtendedList<LeveledItemEntry>();
                    ListOutfitHats.ChanceNone = settingsManager.HatChance;

                    var ListOutfitClothes = myMod.LeveledItems.AddNew();
                    ListOutfitClothes.EditorID = link.EditorID + "_SA_clothes";
                    ListOutfitClothes.Entries = new ExtendedList<LeveledItemEntry>();
                    //add vanilla items from 1 to levelled list
                    foreach (var hat in hats)
                    {
                        ListOutfitHats.Entries.Add(new LeveledItemEntry()
                        {
                            Level = 1,
                            ChanceNone = new Noggog.Percent(0),
                            Count = 1,
                            Reference = hat.ToLink<ILeveledItemGetter>()
                        });
                    }
                    foreach (var cloth in clothes)
                    {
                        ListOutfitClothes.Entries.Add(new LeveledItemEntry()
                        {
                            Level = 1,
                            ChanceNone = new Noggog.Percent(0),
                            Count = 1,
                            Reference = cloth.ToLink<ILeveledItemGetter>()
                        });
                    }
                    //add modded items to each levelled list                                
                    LeveledItem.AddItemsToList(myMod, Armory.hats, ListOutfitHats.Entries, 0);
                    LeveledItem.AddItemsToList(myMod, Armory.clothes, ListOutfitClothes.Entries, 0);
                    //Add each levelled list to outfit
                    newoutfit.Items.Add(ListOutfitHats);
                    newoutfit.Items.Add(ListOutfitClothes);
                }
            }
        }
    }
}