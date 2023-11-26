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
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(733, 167);
            button1.Name = "button1";
            button1.Size = new Size(102, 86);
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
            loadedMods.Size = new Size(277, 310);
            loadedMods.TabIndex = 3;
            // 
            // FactionList
            // 
            FactionList.FormattingEnabled = true;
            FactionList.Location = new Point(12, 41);
            FactionList.Name = "FactionList";
            FactionList.Size = new Size(277, 23);
            FactionList.TabIndex = 5;
            // 
            // button2
            // 
            button2.Location = new Point(295, 167);
            button2.Name = "button2";
            button2.Size = new Size(93, 86);
            button2.TabIndex = 6;
            button2.Text = "Add to Plan ->";
            button2.UseVisualStyleBackColor = true;
            button2.Click += AddToPlanButton;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(394, 20);
            label1.Name = "label1";
            label1.Size = new Size(77, 15);
            label1.TabIndex = 7;
            label1.Text = "Faction Plans";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(695, 149);
            label2.Name = "label2";
            label2.Size = new Size(180, 15);
            label2.TabIndex = 8;
            label2.Text = "Use once all factions are planned";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 20);
            label3.Name = "label3";
            label3.Size = new Size(51, 15);
            label3.TabIndex = 9;
            label3.Text = "Factions";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 77);
            label4.Name = "label4";
            label4.Size = new Size(53, 15);
            label4.TabIndex = 10;
            label4.Text = "Mod List";
            // 
            // factionPlanTree
            // 
            factionPlanTree.Location = new Point(394, 41);
            factionPlanTree.Name = "factionPlanTree";
            factionPlanTree.Size = new Size(295, 364);
            factionPlanTree.TabIndex = 11;
            // 
            // StarArmory
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(876, 428);
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
    }
}