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
                if (mod.Value.FileName == "Starfield.esm") continue;
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


        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> checkedmods = new List<string>();
            foreach(var item in loadedMods.CheckedItems)
            {
                checkedmods.Add(item.ToString());
            }
            Outfit.DoOutfits(checkedmods);
        }
    }
}