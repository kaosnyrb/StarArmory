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
            Outfit.DoOutfits();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}