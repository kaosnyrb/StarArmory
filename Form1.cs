using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Order;

namespace StarArmory
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var env = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
            //            var armours = env.LoadOrder.PriorityOrder.Armor().WinningOverrides().ToList<object>();

            foreach (var mod in env.LoadOrder)
            {
                if (mod.Value.FileName == "Starfield.esm")
                {
                    continue;
                }
                if (mod.Value.Mod != null)
                {
                    var armours = mod.Value.Mod.Armors.ToList();
                    foreach (var armor in armours)
                    {
                        textBox1.Text += armor.Name + Environment.NewLine;
                    }
                }
            }
            //Setup
            var starfieldesmKey = env.LoadOrder[0].ModKey;
            var linkCache = env.LoadOrder.ToImmutableLinkCache();
            ModKey newMod = new ModKey("StarArmoryPatch",ModType.Master);
            StarfieldMod myMod = new StarfieldMod(newMod, StarfieldRelease.Starfield);


            //LL_Clothes_Civilian_Casual_Any [LVLI:00134208]
            FormKey formKey = new FormKey(starfieldesmKey, 1262088);
            var citizenclothes =  linkCache.Resolve<ILeveledItemGetter>(formKey);
            var newlist = myMod.LeveledItems.GetOrAddAsOverride(citizenclothes);
            newlist.Entries.Clear();

            foreach (var mod in env.LoadOrder)
            {
                if (mod.Value.FileName == "Starfield.esm")
                {
                    continue;
                }
                if (mod.Value.Mod != null)
                {
                    var armours = mod.Value.Mod.Armors.ToList();
                    foreach (var armor in armours)
                    {
                        textBox1.Text += armor.Name + Environment.NewLine;

                        var link = linkCache.Resolve<IArmorGetter>(armor.FormKey);
                        newlist.Entries.Add(new LeveledItemEntry()
                        {
                            Level = 1,
                            ChanceNone = new Noggog.Percent(0),
                            Count = 1,
                            Reference = link.ToLink<ILeveledItemGetter>()
                        });
                    }
                }
            }

            // Modify the name to be different inside myMod
            newlist.EditorID = "Updated";

            //Export
            myMod.WriteToBinary("StarArmoryPatch.esm");
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}