namespace StarArmory
{
    partial class StarArmory
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StarArmory));
            button1 = new Button();
            loadedMods = new CheckedListBox();
            FactionList = new ComboBox();
            button2 = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            factionPlanTree = new TreeView();
            selectallbutton = new Button();
            clearbutton = new Button();
            label5 = new Label();
            donotusevanilla = new CheckBox();
            clearplanbutton = new Button();
            genderdropdown = new ComboBox();
            label6 = new Label();
            modfilter = new ComboBox();
            deleteplan = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(808, 484);
            button1.Name = "button1";
            button1.Size = new Size(180, 47);
            button1.TabIndex = 0;
            button1.Text = "Build ESM";
            button1.UseVisualStyleBackColor = true;
            button1.Click += ExportESMButton;
            // 
            // loadedMods
            // 
            loadedMods.FormattingEnabled = true;
            loadedMods.Location = new Point(12, 95);
            loadedMods.Name = "loadedMods";
            loadedMods.Size = new Size(277, 436);
            loadedMods.TabIndex = 3;
            // 
            // FactionList
            // 
            FactionList.FormattingEnabled = true;
            FactionList.Location = new Point(12, 64);
            FactionList.Name = "FactionList";
            FactionList.Size = new Size(277, 23);
            FactionList.TabIndex = 5;
            FactionList.SelectedIndexChanged += FactionList_SelectedIndexChanged;
            // 
            // button2
            // 
            button2.Location = new Point(295, 484);
            button2.Name = "button2";
            button2.Size = new Size(206, 47);
            button2.TabIndex = 6;
            button2.Text = "Add to Faction Leveled List ->";
            button2.UseVisualStyleBackColor = true;
            button2.Click += AddToPlanButton;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(507, 2);
            label1.Name = "label1";
            label1.Size = new Size(77, 15);
            label1.TabIndex = 7;
            label1.Text = "Faction Plans";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(808, 466);
            label2.Name = "label2";
            label2.Size = new Size(180, 15);
            label2.TabIndex = 8;
            label2.Text = "Use once all factions are planned";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 46);
            label3.Name = "label3";
            label3.Size = new Size(51, 15);
            label3.TabIndex = 9;
            label3.Text = "Factions";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 2);
            label4.Name = "label4";
            label4.Size = new Size(55, 15);
            label4.TabIndex = 10;
            label4.Text = "Category";
            // 
            // factionPlanTree
            // 
            factionPlanTree.Location = new Point(507, 20);
            factionPlanTree.Name = "factionPlanTree";
            factionPlanTree.Size = new Size(295, 511);
            factionPlanTree.TabIndex = 11;
            // 
            // selectallbutton
            // 
            selectallbutton.Location = new Point(12, 537);
            selectallbutton.Name = "selectallbutton";
            selectallbutton.Size = new Size(75, 23);
            selectallbutton.TabIndex = 12;
            selectallbutton.Text = "Select All";
            selectallbutton.UseVisualStyleBackColor = true;
            selectallbutton.Click += selectallbutton_Click;
            // 
            // clearbutton
            // 
            clearbutton.Location = new Point(214, 537);
            clearbutton.Name = "clearbutton";
            clearbutton.Size = new Size(75, 23);
            clearbutton.TabIndex = 13;
            clearbutton.Text = "Clear All";
            clearbutton.UseVisualStyleBackColor = true;
            clearbutton.Click += clearbutton_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(295, 2);
            label5.Name = "label5";
            label5.Size = new Size(91, 15);
            label5.TabIndex = 14;
            label5.Text = "Faction Settings";
            label5.Click += label5_Click;
            // 
            // donotusevanilla
            // 
            donotusevanilla.AutoSize = true;
            donotusevanilla.Location = new Point(295, 22);
            donotusevanilla.Name = "donotusevanilla";
            donotusevanilla.Size = new Size(138, 19);
            donotusevanilla.TabIndex = 15;
            donotusevanilla.Text = "Remove Vanilla Items";
            donotusevanilla.UseVisualStyleBackColor = true;
            // 
            // clearplanbutton
            // 
            clearplanbutton.Location = new Point(727, 537);
            clearplanbutton.Name = "clearplanbutton";
            clearplanbutton.Size = new Size(75, 23);
            clearplanbutton.TabIndex = 16;
            clearplanbutton.Text = "Clear Plan";
            clearplanbutton.UseVisualStyleBackColor = true;
            clearplanbutton.Click += clearplanbutton_Click;
            // 
            // genderdropdown
            // 
            genderdropdown.FormattingEnabled = true;
            genderdropdown.Items.AddRange(new object[] { "All", "Female Only", "Male Only" });
            genderdropdown.Location = new Point(295, 64);
            genderdropdown.Name = "genderdropdown";
            genderdropdown.Size = new Size(206, 23);
            genderdropdown.TabIndex = 17;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(295, 46);
            label6.Name = "label6";
            label6.Size = new Size(45, 15);
            label6.TabIndex = 18;
            label6.Text = "Gender";
            // 
            // modfilter
            // 
            modfilter.FormattingEnabled = true;
            modfilter.Items.AddRange(new object[] { "All", "Clothes", "Spacesuit", "Weapons", "Consumables" });
            modfilter.Location = new Point(12, 20);
            modfilter.Name = "modfilter";
            modfilter.Size = new Size(277, 23);
            modfilter.TabIndex = 19;
            modfilter.SelectedIndexChanged += modfilter_SelectedIndexChanged;
            // 
            // deleteplan
            // 
            deleteplan.Location = new Point(507, 537);
            deleteplan.Name = "deleteplan";
            deleteplan.Size = new Size(117, 23);
            deleteplan.TabIndex = 20;
            deleteplan.Text = "Delete Faction Plan";
            deleteplan.UseVisualStyleBackColor = true;
            deleteplan.Click += deleteplan_Click;
            // 
            // StarArmory
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 579);
            Controls.Add(deleteplan);
            Controls.Add(modfilter);
            Controls.Add(label6);
            Controls.Add(genderdropdown);
            Controls.Add(clearplanbutton);
            Controls.Add(donotusevanilla);
            Controls.Add(label5);
            Controls.Add(clearbutton);
            Controls.Add(selectallbutton);
            Controls.Add(factionPlanTree);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button2);
            Controls.Add(FactionList);
            Controls.Add(loadedMods);
            Controls.Add(button1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "StarArmory";
            Text = "StarArmory";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private CheckedListBox loadedMods;
        private ComboBox FactionList;
        private Button button2;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TreeView factionPlanTree;
        private Button selectallbutton;
        private Button clearbutton;
        private Label label5;
        private CheckBox donotusevanilla;
        private Button clearplanbutton;
        private ComboBox genderdropdown;
        private Label label6;
        private ComboBox modfilter;
        private Button deleteplan;
    }
}