using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using CliniqueVet.DAL;

namespace CliniqueVet.Forms
{
    public class FormRapportFinancier : Form
    {
        private FactureDAL dalF = new FactureDAL();
        private AnimalDAL dalA = new AnimalDAL();

        public FormRapportFinancier()
        {
            this.Text = "Rapport Financier";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 247, 250);
            InitUI();
        }

        private void InitUI()
        {
            // Header
            var panelTop = new Panel();
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 55;
            panelTop.BackColor = Color.FromArgb(41, 128, 185);

            var lblTitle = new Label();
            lblTitle.Text = "Rapport Financier";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(15, 14);
            panelTop.Controls.Add(lblTitle);

            // Year selector
            var lblAnnee = new Label();
            lblAnnee.Text = "Annee:";
            lblAnnee.Font = new Font("Segoe UI", 10);
            lblAnnee.ForeColor = Color.White;
            lblAnnee.AutoSize = true;
            lblAnnee.Location = new Point(700, 18);
            panelTop.Controls.Add(lblAnnee);

            var cmbAnnee = new ComboBox();
            cmbAnnee.Location = new Point(760, 14);
            cmbAnnee.Size = new Size(90, 28);
            cmbAnnee.Font = new Font("Segoe UI", 10);
            cmbAnnee.DropDownStyle = ComboBoxStyle.DropDownList;
            for (int y = DateTime.Now.Year; y >= DateTime.Now.Year - 5; y--)
                cmbAnnee.Items.Add(y);
            cmbAnnee.SelectedIndex = 0;
            cmbAnnee.SelectedIndexChanged += (s, e) => LoadCharts((int)cmbAnnee.SelectedItem);
            panelTop.Controls.Add(cmbAnnee);

            // Stat summary row
            var panelStats = new Panel();
            panelStats.Location = new Point(10, 65);
            panelStats.Size = new Size(960, 80);
            panelStats.BackColor = Color.Transparent;

            decimal revTotal = dalF.GetRevenuTotal();
            decimal revMois = dalF.GetRevenuMois();

            AddStatCard(panelStats, "Revenu Total (DT)", revTotal.ToString("F2"), Color.FromArgb(39, 174, 96), 0);
            AddStatCard(panelStats, "Revenu ce mois (DT)", revMois.ToString("F2"), Color.FromArgb(41, 128, 185), 1);
            AddStatCard(panelStats, "Nb Factures Total", GetFactureCount().ToString(), Color.FromArgb(142, 68, 173), 2);
            AddStatCard(panelStats, "Moyenne / Facture (DT)", GetMoyenneFacture().ToString("F2"), Color.FromArgb(211, 84, 0), 3);

            // Chart 1: Revenue per month (bar)
            var chartRevenu = new Chart();
            chartRevenu.Location = new Point(10, 155);
            chartRevenu.Size = new Size(480, 300);
            chartRevenu.BackColor = Color.White;
            chartRevenu.BorderlineColor = Color.FromArgb(220, 220, 220);
            chartRevenu.BorderlineDashStyle = ChartDashStyle.Solid;
            chartRevenu.BorderlineWidth = 1;

            var areaRevenu = new ChartArea("areaRevenu");
            areaRevenu.BackColor = Color.White;
            areaRevenu.AxisX.Title = "Mois";
            areaRevenu.AxisY.Title = "DT";
            areaRevenu.AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            areaRevenu.AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            areaRevenu.AxisX.LabelStyle.Font = new Font("Segoe UI", 8);
            areaRevenu.AxisY.LabelStyle.Font = new Font("Segoe UI", 8);
            chartRevenu.ChartAreas.Add(areaRevenu);

            var titleRevenu = new Title("Revenus par mois");
            titleRevenu.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            titleRevenu.ForeColor = Color.FromArgb(40, 55, 71);
            chartRevenu.Titles.Add(titleRevenu);

            var seriesRevenu = new Series("Revenus");
            seriesRevenu.ChartType = SeriesChartType.Column;
            seriesRevenu.Color = Color.FromArgb(41, 128, 185);
            seriesRevenu.IsValueShownAsLabel = true;
            seriesRevenu.Font = new Font("Segoe UI", 7);
            seriesRevenu.LabelForeColor = Color.FromArgb(40, 55, 71);
            chartRevenu.Series.Add(seriesRevenu);
            chartRevenu.Name = "chartRevenu";

            // Chart 2: Consultations per month (line)
            var chartConsult = new Chart();
            chartConsult.Location = new Point(505, 155);
            chartConsult.Size = new Size(480, 300);
            chartConsult.BackColor = Color.White;
            chartConsult.BorderlineColor = Color.FromArgb(220, 220, 220);
            chartConsult.BorderlineDashStyle = ChartDashStyle.Solid;
            chartConsult.BorderlineWidth = 1;

            var areaConsult = new ChartArea("areaConsult");
            areaConsult.BackColor = Color.White;
            areaConsult.AxisX.Title = "Mois";
            areaConsult.AxisY.Title = "Nb Consultations";
            areaConsult.AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            areaConsult.AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
            areaConsult.AxisX.LabelStyle.Font = new Font("Segoe UI", 8);
            areaConsult.AxisY.LabelStyle.Font = new Font("Segoe UI", 8);
            chartConsult.ChartAreas.Add(areaConsult);

            var titleConsult = new Title("Consultations par mois");
            titleConsult.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            titleConsult.ForeColor = Color.FromArgb(40, 55, 71);
            chartConsult.Titles.Add(titleConsult);

            var seriesConsult = new Series("Consultations");
            seriesConsult.ChartType = SeriesChartType.Line;
            seriesConsult.Color = Color.FromArgb(39, 174, 96);
            seriesConsult.IsValueShownAsLabel = true;
            seriesConsult.Font = new Font("Segoe UI", 7);
            seriesConsult.BorderWidth = 3;
            seriesConsult.MarkerStyle = MarkerStyle.Circle;
            seriesConsult.MarkerSize = 8;
            chartConsult.Series.Add(seriesConsult);
            chartConsult.Name = "chartConsult";

            // Chart 3: Species distribution (pie) - bottom left
            var chartEspeces = new Chart();
            chartEspeces.Location = new Point(10, 465);
            chartEspeces.Size = new Size(480, 150);
            chartEspeces.BackColor = Color.White;
            chartEspeces.BorderlineColor = Color.FromArgb(220, 220, 220);
            chartEspeces.BorderlineDashStyle = ChartDashStyle.Solid;
            chartEspeces.BorderlineWidth = 1;

            var areaEsp = new ChartArea("areaEsp");
            areaEsp.BackColor = Color.White;
            chartEspeces.ChartAreas.Add(areaEsp);

            var titleEsp = new Title("Repartition par espece");
            titleEsp.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            titleEsp.ForeColor = Color.FromArgb(40, 55, 71);
            chartEspeces.Titles.Add(titleEsp);

            var seriesEsp = new Series("Especes");
            seriesEsp.ChartType = SeriesChartType.Pie;
            seriesEsp.IsValueShownAsLabel = true;
            seriesEsp.Label = "#PERCENT{P0} #VALX";
            seriesEsp.Font = new Font("Segoe UI", 8);
            chartEspeces.Series.Add(seriesEsp);
            chartEspeces.Name = "chartEspeces";

            this.Controls.Add(panelTop);
            this.Controls.Add(panelStats);
            this.Controls.Add(chartRevenu);
            this.Controls.Add(chartConsult);
            this.Controls.Add(chartEspeces);

            LoadCharts(DateTime.Now.Year);
        }

        private void LoadCharts(int year)
        {
            string[] moisNoms = { "Jan", "Fev", "Mar", "Avr", "Mai", "Jun", "Jul", "Aou", "Sep", "Oct", "Nov", "Dec" };

            // Find charts by name
            Chart chartRevenu = null, chartConsult = null, chartEspeces = null;
            foreach (Control c in this.Controls)
            {
                if (c.Name == "chartRevenu") chartRevenu = (Chart)c;
                else if (c.Name == "chartConsult") chartConsult = (Chart)c;
                else if (c.Name == "chartEspeces") chartEspeces = (Chart)c;
            }

            if (chartRevenu == null || chartConsult == null || chartEspeces == null) return;

            chartRevenu.Series["Revenus"].Points.Clear();
            chartConsult.Series["Consultations"].Points.Clear();
            chartEspeces.Series["Especes"].Points.Clear();

            try
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();

                    // Revenue per month
                    string sqlRev = @"SELECT MONTH(date_facture) as mois, ISNULL(SUM(montant),0) as total
                                      FROM Facture WHERE YEAR(date_facture)=@year
                                      GROUP BY MONTH(date_facture) ORDER BY mois";
                    var revenusParMois = new decimal[13];
                    using (var cmd = new SqlCommand(sqlRev, conn))
                    {
                        cmd.Parameters.AddWithValue("@year", year);
                        using (var r = cmd.ExecuteReader())
                            while (r.Read())
                                revenusParMois[r.GetInt32(0)] = r.GetDecimal(1);
                    }
                    for (int m = 1; m <= 12; m++)
                        chartRevenu.Series["Revenus"].Points.AddXY(moisNoms[m - 1], revenusParMois[m]);

                    // Consultations per month
                    string sqlCons = @"SELECT MONTH(date_consult) as mois, COUNT(*) as nb
                                       FROM Consultation WHERE YEAR(date_consult)=@year
                                       GROUP BY MONTH(date_consult) ORDER BY mois";
                    var consultsParMois = new int[13];
                    using (var cmd = new SqlCommand(sqlCons, conn))
                    {
                        cmd.Parameters.AddWithValue("@year", year);
                        using (var r = cmd.ExecuteReader())
                            while (r.Read())
                                consultsParMois[r.GetInt32(0)] = r.GetInt32(1);
                    }
                    for (int m = 1; m <= 12; m++)
                        chartConsult.Series["Consultations"].Points.AddXY(moisNoms[m - 1], consultsParMois[m]);

                    // Species distribution
                    string sqlEsp = @"SELECT espece, COUNT(*) as nb FROM Animal GROUP BY espece ORDER BY nb DESC";
                    using (var cmd = new SqlCommand(sqlEsp, conn))
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                            chartEspeces.Series["Especes"].Points.AddXY(r.GetString(0), r.GetInt32(1));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur chargement graphiques: " + ex.Message);
            }
        }

        private void AddStatCard(Panel parent, string title, string value, Color color, int col)
        {
            var panel = new Panel();
            panel.Size = new Size(228, 68);
            panel.Location = new Point(col * 238, 0);
            panel.BackColor = Color.White;

            var border = new Panel();
            border.Dock = DockStyle.Left;
            border.Width = 6;
            border.BackColor = color;

            var lblVal = new Label();
            lblVal.Text = value;
            lblVal.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblVal.ForeColor = color;
            lblVal.AutoSize = true;
            lblVal.Location = new Point(14, 8);

            var lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 8);
            lblTitle.ForeColor = Color.FromArgb(120, 140, 160);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(14, 44);

            panel.Controls.Add(lblVal);
            panel.Controls.Add(lblTitle);
            panel.Controls.Add(border);
            parent.Controls.Add(panel);
        }

        private int GetFactureCount()
        {
            try
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Facture", conn))
                        return (int)cmd.ExecuteScalar();
                }
            }
            catch { return 0; }
        }

        private decimal GetMoyenneFacture()
        {
            try
            {
                using (var conn = DatabaseManager.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT ISNULL(AVG(montant),0) FROM Facture", conn))
                        return (decimal)cmd.ExecuteScalar();
                }
            }
            catch { return 0; }
        }
    }
}
