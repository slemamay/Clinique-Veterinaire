using System;
using System.Windows.Forms;
using System.Drawing;
using CliniqueVet.DAL;

namespace CliniqueVet.Forms
{
    public class FormDashboard : Form
    {
        public FormDashboard()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            this.Controls.Clear();

            var lblTitle = new Label();
            lblTitle.Text = "Tableau de bord";
            lblTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(40, 55, 71);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 20);
            this.Controls.Add(lblTitle);

            var lblDate = new Label();
            lblDate.Text = DateTime.Now.ToString("dddd dd MMMM yyyy", new System.Globalization.CultureInfo("fr-FR"));
            lblDate.Font = new Font("Segoe UI", 10);
            lblDate.ForeColor = Color.FromArgb(120, 140, 160);
            lblDate.AutoSize = true;
            lblDate.Location = new Point(22, 55);
            this.Controls.Add(lblDate);

            try
            {
                int rdvAuj = new RDVDAL().GetCountToday();
                int consultMois = new ConsultationDAL().GetCountThisMonth();
                int totalAnimaux = new AnimalDAL().GetCount();
                decimal revenuMois = new FactureDAL().GetRevenuMois();
                var rappels = new VaccinationDAL().GetUpcomingRappels(30);

                int col = 0;
                AddStatCard("RDV aujourd'hui", rdvAuj.ToString(), Color.FromArgb(41, 128, 185), col++);
                AddStatCard("Consultations ce mois", consultMois.ToString(), Color.FromArgb(39, 174, 96), col++);
                AddStatCard("Total animaux", totalAnimaux.ToString(), Color.FromArgb(142, 68, 173), col++);
                AddStatCard("Revenus ce mois (DT)", revenuMois.ToString("F2"), Color.FromArgb(211, 84, 0), col++);

                var lblRappels = new Label();
                lblRappels.Text = "Rappels de vaccination a venir (30 jours)";
                lblRappels.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                lblRappels.ForeColor = Color.FromArgb(40, 55, 71);
                lblRappels.AutoSize = true;
                lblRappels.Location = new Point(20, 200);
                this.Controls.Add(lblRappels);

                var grid = new DataGridView();
                grid.Location = new Point(20, 230);
                grid.Size = new Size(800, 200);
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                grid.ReadOnly = true;
                grid.AllowUserToAddRows = false;
                grid.RowHeadersVisible = false;
                grid.BackgroundColor = Color.White;
                grid.BorderStyle = BorderStyle.None;
                grid.Font = new Font("Segoe UI", 9);
                grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                grid.EnableHeadersVisualStyles = false;
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

                grid.Columns.Add("Animal", "Animal");
                grid.Columns.Add("Vaccin", "Vaccin");
                grid.Columns.Add("DateRappel", "Date de rappel");

                foreach (var v in rappels)
                {
                    grid.Rows.Add(v.NomAnimal, v.Nom, v.DateRappel.HasValue ? v.DateRappel.Value.ToShortDateString() : "");
                }

                if (rappels.Count == 0)
                    grid.Rows.Add("-", "Aucun rappel prevu", "");

                this.Controls.Add(grid);
            }
            catch (Exception ex)
            {
                var lbl = new Label();
                lbl.Text = "Erreur lors du chargement: " + ex.Message;
                lbl.ForeColor = Color.Red;
                lbl.AutoSize = true;
                lbl.Location = new Point(20, 100);
                this.Controls.Add(lbl);
            }
        }

        private void AddStatCard(string title, string value, Color color, int col)
        {
            var panel = new Panel();
            panel.Size = new Size(190, 100);
            panel.Location = new Point(20 + col * 205, 80);
            panel.BackColor = Color.White;

            var borderPanel = new Panel();
            borderPanel.Dock = DockStyle.Left;
            borderPanel.Width = 6;
            borderPanel.BackColor = color;
            panel.Controls.Add(borderPanel);

            var lblVal = new Label();
            lblVal.Text = value;
            lblVal.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblVal.ForeColor = color;
            lblVal.AutoSize = true;
            lblVal.Location = new Point(18, 15);
            panel.Controls.Add(lblVal);

            var lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 8);
            lblTitle.ForeColor = Color.FromArgb(120, 140, 160);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(18, 60);
            panel.Controls.Add(lblTitle);

            this.Controls.Add(panel);
        }
    }
}
