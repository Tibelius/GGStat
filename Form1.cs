using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static GGStat.Data;

namespace GGStat {
    public partial class Form1 : Form {

        List<Player> rawPlayers;

        public Form1() {
            InitializeComponent();
            logBox.Enter += (s, e) => { logBox.Parent.Focus(); };
        }

        private void Form1_Load(object sender, EventArgs e) {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if (Program.gameThread.IsAlive) {
                Program.gameThread.Abort();
            }
        }

        public void WriteToLog(string text) {
            logBox.AppendText(text);
        }


        delegate void TableInitializeCallback(object[] data);
        public void SetTableData(object[] data) {
            if (tablePanel.InvokeRequired) {
                TableInitializeCallback d = new TableInitializeCallback(InitializeTableLayoutPanel);
                Invoke(d, new object[] { data });
            } else {
                InitializeTableLayoutPanel(data);
            }
        }

        private void ResetTableLayoutPanel() {
            tablePanel.ColumnCount = 0;
            tablePanel.RowCount = 0;
            tablePanel.Controls.Clear();
        }
        private void InitializeTableLayoutPanel(object[] data) {
            ResetTableLayoutPanel();
            if (rawPlayers == null) {
                rawPlayers = getAllPlayers();
                playerQueryCombo.DataSource = rawPlayers;
            }

            if (data[0] is Match) {
                tablePanel.ColumnCount = 7;
                tablePanel.RowCount = data.Length + 1;
                for (int i = 0; i < tablePanel.RowCount; i++) {
                    for (int j = 0; j < tablePanel.ColumnCount; j++) {
                        var tableCell = new TableCell();
                        tableCell.Visible = true;
                        tableCell.Dock = DockStyle.Fill;
                        tableCell.Margin = new Padding(0);
                        if (i > 0) {
                            tableCell.MouseDoubleClick += new MouseEventHandler(CellOnClick);
                        }
                        
                        switch (j) {
                            case 0:
                                tableCell.Text = (i == 0) ? "ID" : ((Match)data[i-1]).id.ToString();
                                break;
                            case 1:
                                tableCell.Text = (i == 0) ? "WINNER" : ((Match)data[i-1]).winner.ToString();
                                break;
                            case 2:
                                tableCell.Text = (i == 0) ? "P1" : rawPlayers.Find(x => x.id == ((Match)data[i - 1]).player1.id).latestAlias;
                                break;
                            case 3:
                                tableCell.Text = (i == 0) ? "P1 CHARACTER" : Character[((Match)data[i-1]).player1_character_id];
                                break;
                            case 4:
                                tableCell.Text = (i == 0) ? "P2" : rawPlayers.Find(x => x.id == ((Match)data[i - 1]).player2.id).latestAlias;
                                break;
                            case 5:
                                tableCell.Text = (i == 0) ? "P2 CHARACTER" : Character[((Match)data[i-1]).player2_character_id];
                                break;
                            case 6:
                                tableCell.Text = (i == 0) ? "TIMESTAMP" : UnixTimeStampToDateTime(double.Parse(((Match)data[i - 1]).timestamp.ToString())).ToString("dddd, dd MMMM yyyy HH\u003Amm\u003Ass");
                                break;
                            default:
                                tableCell.Text = "N/A";
                                break;
                        }

                        tablePanel.Controls.Add(tableCell, j, i);
                    }
                }
            } else if (data[0] is Round) {
                Program.Log("Hello from InitializeTableLayoutPanel! This is a Round");
                tablePanel.ColumnCount = 8;
                tablePanel.RowCount = data.Length;
            }

        }

        private void DisableTableLayoutPanel() {
            tablePanel.Enabled = false;
        }
        private void EnableTableLayoutPanel() {
            tablePanel.Enabled = true;
        }

        private void CellOnClick(object sender, MouseEventArgs e) {
            if (e.Clicks >= 2) {
                Program.Log("Hello from Table Cell");
                DisableTableLayoutPanel();
            }
        }

        private class TableCell : Label {
            public object link;

            public TableCell() {
                link = null;
            }
            public TableCell(object link) {
                this.link = link;
            }

            public Type GetLinkType() {
                return link.GetType();
            }
        }

        private void SearchButton_Click(object sender, EventArgs e) {
            searchButton.Enabled = false;
            DisableTableLayoutPanel();
            try {
                SetTableData(getAllMatchesWithPlayer((Player)playerQueryCombo.SelectedItem).ToArray());
            } finally {
                EnableTableLayoutPanel();
                searchButton.Enabled = true;
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
