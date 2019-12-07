namespace GGStat {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.logBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.searchButton = new System.Windows.Forms.Button();
            this.playerQueryCombo = new System.Windows.Forms.ComboBox();
            this.queryLabel = new System.Windows.Forms.Label();
            this.lossesText = new System.Windows.Forms.TextBox();
            this.lossesLabel = new System.Windows.Forms.Label();
            this.winsText = new System.Windows.Forms.TextBox();
            this.winsLabel = new System.Windows.Forms.Label();
            this.tablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // logBox
            // 
            this.logBox.BackColor = System.Drawing.SystemColors.Window;
            this.logBox.CausesValidation = false;
            this.logBox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.logBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.logBox.Location = new System.Drawing.Point(0, 0);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logBox.Size = new System.Drawing.Size(1643, 198);
            this.logBox.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.searchButton);
            this.panel1.Controls.Add(this.playerQueryCombo);
            this.panel1.Controls.Add(this.queryLabel);
            this.panel1.Controls.Add(this.lossesText);
            this.panel1.Controls.Add(this.lossesLabel);
            this.panel1.Controls.Add(this.winsText);
            this.panel1.Controls.Add(this.winsLabel);
            this.panel1.Controls.Add(this.tablePanel);
            this.panel1.Controls.Add(this.logBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1643, 736);
            this.panel1.TabIndex = 1;
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(1508, 221);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(75, 23);
            this.searchButton.TabIndex = 9;
            this.searchButton.Text = "Get";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // playerQueryCombo
            // 
            this.playerQueryCombo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.playerQueryCombo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.playerQueryCombo.FormattingEnabled = true;
            this.playerQueryCombo.Location = new System.Drawing.Point(1149, 223);
            this.playerQueryCombo.Name = "playerQueryCombo";
            this.playerQueryCombo.Size = new System.Drawing.Size(353, 21);
            this.playerQueryCombo.TabIndex = 8;
            // 
            // queryLabel
            // 
            this.queryLabel.AutoSize = true;
            this.queryLabel.Location = new System.Drawing.Point(1010, 226);
            this.queryLabel.Name = "queryLabel";
            this.queryLabel.Size = new System.Drawing.Size(133, 13);
            this.queryLabel.TabIndex = 6;
            this.queryLabel.Text = "Get all matches with player";
            // 
            // lossesText
            // 
            this.lossesText.Enabled = false;
            this.lossesText.Location = new System.Drawing.Point(1057, 407);
            this.lossesText.Name = "lossesText";
            this.lossesText.Size = new System.Drawing.Size(100, 20);
            this.lossesText.TabIndex = 5;
            // 
            // lossesLabel
            // 
            this.lossesLabel.AutoSize = true;
            this.lossesLabel.Location = new System.Drawing.Point(1001, 410);
            this.lossesLabel.Name = "lossesLabel";
            this.lossesLabel.Size = new System.Drawing.Size(40, 13);
            this.lossesLabel.TabIndex = 4;
            this.lossesLabel.Text = "Losses";
            // 
            // winsText
            // 
            this.winsText.Enabled = false;
            this.winsText.Location = new System.Drawing.Point(1057, 370);
            this.winsText.Name = "winsText";
            this.winsText.Size = new System.Drawing.Size(100, 20);
            this.winsText.TabIndex = 3;
            // 
            // winsLabel
            // 
            this.winsLabel.AutoSize = true;
            this.winsLabel.Location = new System.Drawing.Point(1010, 373);
            this.winsLabel.Name = "winsLabel";
            this.winsLabel.Size = new System.Drawing.Size(31, 13);
            this.winsLabel.TabIndex = 2;
            this.winsLabel.Text = "Wins";
            // 
            // tablePanel
            // 
            this.tablePanel.AutoScroll = true;
            this.tablePanel.BackColor = System.Drawing.SystemColors.Window;
            this.tablePanel.ColumnCount = 7;
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tablePanel.Location = new System.Drawing.Point(12, 213);
            this.tablePanel.Name = "tablePanel";
            this.tablePanel.RowCount = 2;
            this.tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tablePanel.Size = new System.Drawing.Size(963, 491);
            this.tablePanel.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1643, 736);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tablePanel;
        private System.Windows.Forms.TextBox winsText;
        private System.Windows.Forms.Label winsLabel;
        private System.Windows.Forms.TextBox lossesText;
        private System.Windows.Forms.Label lossesLabel;
        private System.Windows.Forms.Label queryLabel;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.ComboBox playerQueryCombo;
    }
}

