using log4net;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Starfield;
using Noggog;
using System.Globalization;
using System.Reactive.Joins;
using System.Windows.Forms;
using YamlDotNet.Core.Tokens;
using static Mutagen.Bethesda.FormKeys.Starfield.Starfield;

namespace StarArmory
{
    public partial class StarArmory : Form
    {
        public static StarfieldMod myMod;
        public static SettingsManager settingsManager;

        public static readonly ILog log = LogManager.GetLogger(typeof(StarArmory));

        public static List<String> AllFactions = new List<string>();

        public StarArmory()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();
            log.Info("StarArmory Starting...");
            settingsManager = new SettingsManager();
            modfilter.SelectedIndex = 0;
            try
            {
                log.Info("Loading Settings.yaml");
                settingsManager = YamlImporter.getObjectFromFile<SettingsManager>("Settings.yaml");
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                log.Info("Exception loading Settings.yaml: " + ex.Message);
                log.Info("Creating new Settings.yaml");
                settingsManager = new SettingsManager();
                YamlExporter.WriteObjToYamlFile("Settings.yaml", settingsManager);
            }

            try
            {
                log.Info("DataPath in Settings.yaml is: " + settingsManager.DataPath);
                using (var env = GetGameEnvironment())
                {
                    log.Info("Plugin.txt location: " + env.LoadOrderFilePath);
                    log.Info("Load order length: " + env.LoadOrder.Count);
                    log.Info("Scanning Load Order for valid armor mods...");

                    foreach (var mod in env.LoadOrder)
                    {
                        bool added = false;
                        if (mod.Value.ModKey.FileName == "Starfield.esm") continue;
                        if (mod.Value.Mod != null)
                        {
                            if (mod.Value.Mod.Armors != null)
                            {
                                if (mod.Value.Mod.Armors.Count > 0)
                                {
                                    loadedMods.Items.Add(mod.Value.FileName, true);
                                    log.Info(mod.Value.ModKey.FileName + " has " + mod.Value.Mod.Armors.Count + " Armors");
                                    added = true;
                                }
                            }
                            else
                            {
                                log.Info(mod.Value.ModKey.FileName + " has no Armors, skipping.");
                            }
                            if (mod.Value.Mod.Weapons != null && !added)
                            {
                                if (mod.Value.Mod.Weapons.Count > 0)
                                {
                                    loadedMods.Items.Add(mod.Value.FileName, true);
                                    log.Info(mod.Value.ModKey.FileName + " has " + mod.Value.Mod.Weapons.Count + " Weapons");
                                }
                            }
                            else
                            {
                                log.Info(mod.Value.ModKey.FileName + " has no Weapons, skipping.");
                            }
                        }
                    }
                }

                Armory.clothes = new List<IArmorGetter>();
                Armory.plans = new List<FactionPlan>();
                Armory.factions = new Dictionary<string, Faction>();

                log.Info("Loading Faction Files...");
                string[] fileEntries = Directory.GetFiles("Factions/");
                foreach (var entry in fileEntries)
                {
                    var faction = YamlImporter.getObjectFromFile<Faction>(entry);
                    Armory.factions.Add(faction.Name, faction);
                    FactionList.Items.Add(faction.Name);
                    AllFactions.Add(faction.Name);
                }
                FactionList.SelectedItem = FactionList.Items[0];
            }
            catch (Exception ex)
            {
                log.Info("Exception loading mods : " + ex.Message);
                MessageBox.Show(ex.Message);
            }
            //Used to check these existed before loading them, but it was triggering virus scanners.
            try
            {
                log.Info("Loading Plan.yaml");
                Armory.plans = YamlImporter.getObjectFromFile<List<FactionPlan>>("Plan.yaml");
                UpdatePlan();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                log.Info("Exception loading Plan.yaml. Assuming it's just not there.");
                Armory.plans = new List<FactionPlan>();
            }
            genderdropdown.SelectedIndex = 0;
        }

        public void UpdatePlan()
        {
            factionPlanTree.Nodes.Clear();
            factionPlanTree.BeginUpdate();
            factionPlanTree.Nodes.Add("Plan");
            for (int i = 0; i < Armory.plans.Count; i++)
            {
                Armory.plans[i].faction = Armory.factions[Armory.plans[i].faction.Name];
                factionPlanTree.Nodes[0].Nodes.Add(Armory.plans[i].faction.Name);
                factionPlanTree.Nodes[0].Nodes[i].Nodes.Add("Clear Vanilla: " + Armory.plans[i].clearvanillaitems.ToString());
                for (int j = 0; j < Armory.plans[i].mods.Count; j++)
                {
                    factionPlanTree.Nodes[0].Nodes[i].Nodes.Add(Armory.plans[i].mods[j]);
                }
            }
            factionPlanTree.Nodes[0].Expand();
            factionPlanTree.EndUpdate();
        }

        public static IGameEnvironment<IStarfieldMod, IStarfieldModGetter> GetGameEnvironment()
        {
            try
            {
                if (settingsManager.DataPath != "../")
                {
                    var env = GameEnvironment.Typical.Builder<IStarfieldMod, IStarfieldModGetter>(GameRelease.Starfield)
                        .TransformLoadOrderListings(x => x.Where(x => !x.ModKey.Name.Contains("StarArmory")))
                        .WithTargetDataFolder(new DirectoryPath(settingsManager.DataPath)).Build();
                    return env;
                }
                else
                {
                    var env = GameEnvironment.Typical.Builder<IStarfieldMod, IStarfieldModGetter>(GameRelease.Starfield)
                                        .TransformLoadOrderListings(x => x.Where(x => !x.ModKey.Name.Contains("StarArmory"))).Build();
                    return env;
                }
                //throw new Exception();//Trigger fallback                
            }
            catch (Exception e)
            {
                log.Info("Exception loading environment: " + e.Message);
                var env = GameEnvironment.Typical.Builder<IStarfieldMod, IStarfieldModGetter>(GameRelease.Starfield)
                    .TransformLoadOrderListings(x => x.Where(x => !x.ModKey.Name.Contains("StarArmory")))
                    .WithTargetDataFolder(new DirectoryPath(settingsManager.DataPath)).Build();
                return env;
            }
        }


        private void AddToPlanButton(object sender, EventArgs e)
        {
            try
            {
                var factionplan = new FactionPlan
                {
                    faction = YamlImporter.getObjectFromYaml<Faction>(YamlExporter.BuildYaml(Armory.factions[FactionList.SelectedItem.ToString()])), //Yeah, looks stupid but it creates a clone.
                    clearvanillaitems = donotusevanilla.Checked,
                    gender = genderdropdown.SelectedItem.ToString()
                };

                if (factionplan.gender != "All")
                {
                    factionplan.faction.Name += " - " + factionplan.gender;
                }
                log.Info("Adding " + factionplan.faction.Name + " to plan.");
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
                UpdatePlan();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Info("Exception adding faction: " + ex.Message);
            }
        }


        private void ExportESMButton(object sender, EventArgs e)
        {
            SimpleForm lastformkey = new SimpleForm();
            Armory.UpgradedItems = new List<string>();
            try
            {
                log.Info("Exporting Plan.yaml");
                YamlExporter.WriteObjToYamlFile("Plan.yaml", Armory.plans);
                string datapath = "";
                string pluginspath = "";
                using (var env = GetGameEnvironment())
                {
                    datapath = env.DataFolderPath;
                    pluginspath = env.LoadOrderFilePath;
                }
                log.Info("Data folder at:" + datapath);
                //File.Delete(datapath + "\\StarArmoryPatch.esm");

                ModKey newMod = new ModKey("StarArmoryPatch", ModType.Master);
                myMod = new StarfieldMod(newMod, StarfieldRelease.Starfield);
                myMod.Clear();
                foreach (var plan in Armory.plans)
                {
                    log.Info("Staring Faction : " + plan.faction.Name);

                    Armory.Clear();
                    Armory.LoadClothes(plan.mods);
                    //Levelled Lists
                    //Here we inject the modded items into the levelled lists defined in the faction files.
                    //If we're gendered ignore this section
                    if (plan.gender == "All" || plan.gender == null)
                    {
                        if (plan.faction.Hats != null)
                        {
                            foreach (var hat in plan.faction.Hats)
                            {
                                lastformkey = hat;
                                LeveledItem.AddItemsToLevelledList(myMod, Armory.hats, hat.modname, hat.formkey, plan.clearvanillaitems);
                            }
                        }
                        if (plan.faction.Clothes != null)
                        {
                            foreach (var clothes in plan.faction.Clothes)
                            {
                                lastformkey = clothes;
                                LeveledItem.AddItemsToLevelledList(myMod, Armory.clothes, clothes.modname, clothes.formkey, plan.clearvanillaitems);
                            }
                        }
                        if (plan.faction.Spacesuits != null)
                        {
                            foreach (var suit in plan.faction.Spacesuits)
                            {
                                lastformkey = suit;
                                LeveledItem.AddItemsToLevelledList(myMod, Armory.spacesuits, suit.modname, suit.formkey, plan.clearvanillaitems);
                            }
                        }
                        if (plan.faction.SpaceHelmets != null)
                        {
                            foreach (var helm in plan.faction.SpaceHelmets)
                            {
                                lastformkey = helm;
                                LeveledItem.AddItemsToLevelledList(myMod, Armory.spacehelmets, helm.modname, helm.formkey, plan.clearvanillaitems);
                            }
                        }
                        if (plan.faction.Boostpacks != null)
                        {
                            foreach (var pack in plan.faction.Boostpacks)
                            {
                                lastformkey = pack;
                                LeveledItem.AddItemsToLevelledList(myMod, Armory.boostpacks, pack.modname, pack.formkey, plan.clearvanillaitems);
                            }
                        }
                        if (plan.faction.RangedWeapons != null)
                        {
                            foreach (var rangedweapon in plan.faction.RangedWeapons)
                            {
                                lastformkey = rangedweapon;
                                LeveledItem.AddItemsToLevelledList(myMod, Armory.ranged_weapons, rangedweapon.modname, rangedweapon.formkey, plan.clearvanillaitems);
                            }
                        }
                        if (plan.faction.MeleeWeapons != null)
                        {
                            foreach (var meleeweapon in plan.faction.MeleeWeapons)
                            {
                                lastformkey = meleeweapon;
                                LeveledItem.AddItemsToLevelledList(myMod, Armory.melee_weapons, meleeweapon.modname, meleeweapon.formkey, plan.clearvanillaitems);
                            }
                        }
                        if (plan.faction.Grenades != null)
                        {
                            foreach (var nade in plan.faction.Grenades)
                            {
                                lastformkey = nade;
                                LeveledItem.AddItemsToLevelledList(myMod, Armory.grenades, nade.modname, nade.formkey, plan.clearvanillaitems);
                            }
                        }
                    }
                    //Outfits
                    //Outfits have another layer of complexity to the levelled lists
                    //In vanilla some outfits link directly to armor, things like the starborn.
                    //We need to create a new levelled list then replace the outfit with these lists.
                    //We also want to peserve the original outfit as an option.
                    log.Info("Starting Outfits");
                    using (var env = StarArmory.GetGameEnvironment())
                    {
                        BuildSpaceOutfit(plan, env);
                        BuildClothesOutfit(plan, env);
                    }
                }

                //Export
                log.Info("Exporting to " + datapath + "\\StarArmoryPatch.esm");
                myMod.WriteToBinary(datapath + "\\StarArmoryPatch.esm");
                MessageBox.Show("Exported StarArmoryPatch.esm to Data Folder. Make sure to add it to plugins.txt at " + pluginspath);
                log.Info("Export Complete");
            }
            catch (Exception ex)
            {
                log.Info("Erroring editorid was: " + lastformkey.editorId);
                log.Info("Erroring modname was: " + lastformkey.modname);
                MessageBox.Show(ex.Message);
                log.Info(ex.Message);
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
                    try
                    {
                        var link = immutableLoadOrderLinkCache.Resolve<IOutfitGetter>(key);
                        var olditems = link.Items.ToList();
                        List<IArmorGetter> spacesuits = new List<IArmorGetter>();
                        List<IArmorGetter> spacehelmets = new List<IArmorGetter>();
                        List<IArmorGetter> boostpacks = new List<IArmorGetter>();
                        if (!plan.clearvanillaitems)
                        {
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
                        }

                        //clear outfit
                        var newoutfit = myMod.Outfits.GetOrAddAsOverride(link);
                        newoutfit.Items.Clear();

                        var ListOutfitSpacesuit = myMod.LeveledItems.AddNew();
                        ListOutfitSpacesuit.EditorID = link.EditorID + "_SA_spacesuit";
                        ListOutfitSpacesuit.Entries = new ExtendedList<LeveledItemEntry>();


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
                        LeveledItem.AddItemsToList(myMod, Armory.spacesuits, ListOutfitSpacesuit.Entries, 0);
                        newoutfit.Items.Add(ListOutfitSpacesuit);
                        if (outfit.Helmet == null)
                        {
                            var ListOutfitSpacehelmets = myMod.LeveledItems.AddNew();
                            ListOutfitSpacehelmets.EditorID = link.EditorID + "_SA_spacehelmets";
                            ListOutfitSpacehelmets.Entries = new ExtendedList<LeveledItemEntry>();

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
                            LeveledItem.AddItemsToList(myMod, Armory.spacehelmets, ListOutfitSpacehelmets.Entries, 0);
                            newoutfit.Items.Add(ListOutfitSpacehelmets);
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
                        LeveledItem.AddItemsToList(myMod, Armory.boostpacks, ListOutfitBoostPacks.Entries, 0);
                        newoutfit.Items.Add(ListOutfitBoostPacks);
                    }
                    catch (Exception ex)
                    {
                        log.Info("Erroring editorid was: " + outfit.editorId);
                        log.Info("Erroring modname was: " + outfit.modname);
                        MessageBox.Show(ex.Message);
                        log.Info(ex.Message);
                    }
                }
            }
        }
        private static void BuildClothesOutfit(FactionPlan plan, IGameEnvironment<IStarfieldMod, IStarfieldModGetter> env)
        {
            var immutableLoadOrderLinkCache = env.LoadOrder.ToImmutableLinkCache();
            FormKey Apparel = new FormKey(env.LoadOrder[0].ModKey, 918668);//ArmorTypeApparelOrNakedBody[KYWD: 000E048C]
            FormKey Head = new FormKey(env.LoadOrder[0].ModKey, 918667);//ArmorTypeApparelHead [KYWD:000E048B]

            Dictionary<string, string> outfitmapping = new Dictionary<string, string>();

            if (plan.faction.OutfitClothes != null)
            {
                foreach (var outfit in plan.faction.OutfitClothes)
                {
                    try
                    {
                        FormKey key = outfit.GetFormKey();
                        //grab items in outfit and save for later
                        var link = immutableLoadOrderLinkCache.Resolve<IOutfitGetter>(key);
                        var olditems = link.Items.ToList();
                        List<IArmorGetter> clothes = new List<IArmorGetter>();
                        List<IArmorGetter> hats = new List<IArmorGetter>();
                        if (!plan.clearvanillaitems)
                        {
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
                        }
                        //clear outfit
                        var newoutfit = myMod.Outfits.GetOrAddAsOverride(link);
                        newoutfit.Items.Clear();

                        //create levelled list for each category for this outfit                        
                        var ListOutfitHats = myMod.LeveledItems.AddNew();
                        ListOutfitHats.EditorID = link.EditorID + "_SA_hats_" + plan.gender;
                        ListOutfitHats.Entries = new ExtendedList<LeveledItemEntry>();
                        ListOutfitHats.ChanceNone = settingsManager.HatChance;

                        var ListOutfitClothes = myMod.LeveledItems.AddNew();
                        ListOutfitClothes.EditorID = link.EditorID + "_SA_clothes_" + plan.gender;
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
                        if (plan.gender == "All")
                        {
                            if (outfit.Helmet == null)
                            {
                                newoutfit.Items.Add(ListOutfitHats);
                            }
                            newoutfit.Items.Add(ListOutfitClothes);
                        }
                        else
                        {
                            var topleveloutfit = myMod.Outfits.AddNew();
                            topleveloutfit.EditorID = newoutfit.EditorID + "_SA_" + plan.gender;
                            topleveloutfit.Items = new ExtendedList<IFormLinkGetter<IOutfitTargetGetter>>();
                            if (!outfitmapping.Keys.Contains(newoutfit.EditorID))
                            {
                                outfitmapping.Add(newoutfit.EditorID, newoutfit.EditorID + "_SA_" + plan.gender);
                            }
                            if (outfit.Helmet == null)
                            {
                                topleveloutfit.Items.Add(ListOutfitHats);
                            }
                            topleveloutfit.Items.Add(ListOutfitClothes);

                            //Fun part, now we need to update every npc of a bodytype to wear the new outfit.
                            //Loop through all npcs.... :|
                            //Mutagen doesn't support this yet...
                            /*
                            foreach (var mod in env.LoadOrder)
                            {
                                mod.Value.Mod.Npcs
                            }*/
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Info("Erroring editorid was: " + outfit.editorId);
                        log.Info("Erroring modname was: " + outfit.modname);
                        MessageBox.Show(ex.Message);
                        log.Info(ex.Message);
                    }
                }
            }
            log.Info("Outfits to map : " + outfitmapping.Count);
        }

        private void selectallbutton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < loadedMods.Items.Count; i++)
            {
                loadedMods.SetItemChecked(i, true);
            }
        }

        private void clearbutton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < loadedMods.Items.Count; i++)
            {
                loadedMods.SetItemChecked(i, false);
            }
        }

        private void clearplanbutton_Click(object sender, EventArgs e)
        {
            Armory.plans.Clear();
            UpdatePlan();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void modfilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (var env = GetGameEnvironment())
            {
                loadedMods.Items.Clear();
                var filter = modfilter.SelectedItem;
                foreach (var mod in env.LoadOrder)
                {
                    bool added = false;
                    if (mod.Value.ModKey.FileName == "Starfield.esm") continue;
                    if (mod.Value.Mod != null)
                    {
                        /*
                         All
                            Clothes
                            Spacesuits
                            Weapons
                            Aid
                         */
                        switch (filter)
                        {
                            case "All":
                                loadedMods.Items.Add(mod.Value.FileName, true);
                                break;
                            case "Clothes":
                                if (mod.Value.Mod.Armors != null)
                                {
                                    if (mod.Value.Mod.Armors.Count > 0)
                                    {
                                        FormKey Apparel = new FormKey(env.LoadOrder[0].ModKey, 918668);//ArmorTypeApparelOrNakedBody[KYWD: 000E048C]
                                        FormKey Head = new FormKey(env.LoadOrder[0].ModKey, 918667);//ArmorTypeApparelHead [KYWD:000E048B]
                                        bool clothesinmod = false;
                                        foreach (var armor in mod.Value.Mod.Armors)
                                        {
                                            if (armor.Keywords.Contains(Apparel) || armor.Keywords.Contains(Head))
                                            {
                                                clothesinmod = true;
                                                break;
                                            }
                                        }
                                        if (clothesinmod)
                                        {
                                            loadedMods.Items.Add(mod.Value.FileName, true);
                                        }
                                    }
                                }
                                break;
                            case "Spacesuit":
                                if (mod.Value.Mod.Armors != null)
                                {
                                    if (mod.Value.Mod.Armors.Count > 0)
                                    {
                                        FormKey spacesuit = new FormKey(env.LoadOrder[0].ModKey, 2344896);//ArmorTypeSpacesuitBody [KYWD:0023C7C0]
                                        FormKey spacehelmet = new FormKey(env.LoadOrder[0].ModKey, 2344897);//ArmorTypeSpacesuitBackpack [KYWD:0023C7BF]
                                        FormKey boostpack = new FormKey(env.LoadOrder[0].ModKey, 2344895);//ArmorTypeSpacesuitHelmet[KYWD: 0023C7C1]
                                        bool clothesinmod = false;
                                        foreach (var armor in mod.Value.Mod.Armors)
                                        {
                                            if (armor.Keywords.Contains(spacesuit) || armor.Keywords.Contains(spacehelmet) || armor.Keywords.Contains(boostpack))
                                            {
                                                clothesinmod = true;
                                                break;
                                            }
                                        }
                                        if (clothesinmod)
                                        {
                                            loadedMods.Items.Add(mod.Value.FileName, true);
                                        }
                                    }
                                }
                                break;
                            case "Weapons":
                                if (mod.Value.Mod.Weapons != null)
                                {
                                    if (mod.Value.Mod.Weapons.Count > 0)
                                    {
                                        FormKey weapon_ranged = new FormKey(env.LoadOrder[0].ModKey, 177940);//WeaponTypeRanged [KYWD:0002B714]
                                        FormKey weapon_melee = new FormKey(env.LoadOrder[0].ModKey, 303268);//WeaponTypeMelee1H [KYWD:0004A0A4]
                                        FormKey grenade = new FormKey(env.LoadOrder[0].ModKey, 303270);//WeaponTypeThrown [KYWD:0004A0A6]
                                        bool inmod = false;
                                        foreach (var armor in mod.Value.Mod.Weapons)
                                        {
                                            if (armor.Keywords.Contains(weapon_ranged) || armor.Keywords.Contains(weapon_melee) || armor.Keywords.Contains(grenade))
                                            {
                                                inmod = true;
                                                break;
                                            }
                                        }
                                        if (inmod)
                                        {
                                            loadedMods.Items.Add(mod.Value.FileName, true);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                //Filter the faction list by the category
                FactionList.Items.Clear();
                if (filter == "All")
                {
                    foreach (var faction in AllFactions)
                    {
                        FactionList.Items.Add(faction);
                    }
                }
                else
                {
                    foreach (var faction in AllFactions)
                    {
                        if (faction.Contains(filter.ToString()))
                        {
                            FactionList.Items.Add(faction);
                        }
                    }
                }
                //FactionList.SelectedIndex = 0;
            }
        }

        private void deleteplan_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Armory.plans.Count; i++)
            {
                if (Armory.plans[i].faction.Name == factionPlanTree.SelectedNode.Text)
                {
                    Armory.plans.RemoveAt(i);
                    UpdatePlan();
                    break;
                }
            }
        }

        private void FactionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool found = false;
            for (int i = 0; i < Armory.plans.Count; i++)
            {
                if (Armory.plans[i].faction.Name == FactionList.SelectedItem.ToString())
                {
                    for (int j = 0; j < loadedMods.Items.Count; j++)
                    {
                        loadedMods.SetItemChecked(j, Armory.plans[i].mods.Contains(loadedMods.Items[j].ToString()));
                        found = true;
                    }
                    donotusevanilla.Checked = Armory.plans[i].clearvanillaitems;
                    break;
                }
            }
            if (!found)
            {
                for (int j = 0; j < loadedMods.Items.Count; j++)
                {
                    loadedMods.SetItemChecked(j, false);
                }
            }
        }
    }
}