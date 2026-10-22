using System;
using System.Windows.Forms;
using System.Drawing;
using CliniqueVet.DAL;
using CliniqueVet.Models;

namespace CliniqueVet.Forms
{
    public class FormFicheAnimal : Form
    {
        private Animal _animal;
        private TabControl tabControl;
        private AnimalDAL dalA = new AnimalDAL();
        private ConsultationDAL dalC = new ConsultationDAL();
        private VaccinationDAL dalV = new VaccinationDAL();
        private RDVDAL dalR = new RDVDAL();

        public FormFicheAnimal(Animal animal)
        {
            _animal = animal;
            this.Text = "Fiche Animal - " + animal.Nom;
            this.Size = new Size(850, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 247, 250);
            InitUI();
            LoadAll();
        }

        private void InitUI()
        {
            // Header panel
            var panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 90;
            panelHeader.BackColor = Color.FromArgb(41, 128, 185);
            panelHeader.Padding = new Padding(20, 10, 20, 10);

            var lblNom = new Label();
            lblNom.Text = _animal.Nom;
            lblNom.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblNom.ForeColor = Color.White;
            lblNom.AutoSize = true;
            lblNom.Location = new Point(20, 10);

            var lblInfo = new Label();
            lblInfo.Text = "Espece: " + _animal.Espece + "   |   Proprietaire: " + _animal.NomProprietaire;
            lblInfo.Font = new Font("Segoe UI", 10);
            lblInfo.ForeColor = Color.FromArgb(200, 230, 255);
            lblInfo.AutoSize = true;
            lblInfo.Location = new Point(22, 52);

            panelHeader.Controls.Add(lblNom);
            panelHeader.Controls.Add(lblInfo);

            // Tab control
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Font = new Font("Segoe UI", 9);

            tabControl.TabPages.Add(CreateTab("Consultations", "tabConsult"));
            tabControl.TabPages.Add(CreateTab("Vaccinations", "tabVaccin"));
            tabControl.TabPages.Add(CreateTab("Rendez-vous", "tabRdv"));

            this.Controls.Add(tabControl);
            this.Controls.Add(panelHeader);
        }

        private TabPage CreateTab(string text, string name)
        {
            var tab = new TabPage(text);
            tab.Name = name;
            tab.BackColor = Color.FromArgb(245, 247, 250);
            tab.Padding = new Padding(10);
            return tab;
        }

        private DataGridView BuildGrid()
        {
            var g = new DataGridView();
            g.Dock = DockStyle.Fill;
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.RowHeadersVisible = false;
            g.BackgroundColor = Color.White;
            g.BorderStyle = BorderStyle.None;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.Font = new Font("Segoe UI", 9);
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            g.EnableHeadersVisualStyles = false;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            return g;
        }

        private void LoadAll()
        {
            // Consultations tab
            var dgvC = BuildGrid();
            dgvC.Columns.Add("Date", "Date");
            dgvC.Columns.Add("Diagnostic", "Diagnostic / Traitement");
            var consults = dalC.GetByAnimal(_animal.IdAnimal);
            foreach (var c in consults)
                dgvC.Rows.Add(c.DateConsult.ToShortDateString(), c.Diagnostic);
            if (consults.Count == 0) dgvC.Rows.Add("-", "Aucune consultation enregistree");
            tabControl.TabPages["tabConsult"].Controls.Add(dgvC);

            // Vaccinations tab
            var dgvV = BuildGrid();
            dgvV.Columns.Add("Nom", "Vaccin");
            dgvV.Columns.Add("Date", "Date");
            dgvV.Columns.Add("Rappel", "Rappel");
            var vaccins = dalV.GetByAnimal(_animal.IdAnimal);
            foreach (var v in vaccins)
                dgvV.Rows.Add(v.Nom, v.DateVaccin.ToShortDateString(),
                    v.DateRappel.HasValue ? v.DateRappel.Value.ToShortDateString() : "-");
            if (vaccins.Count == 0) dgvV.Rows.Add("Aucune vaccination", "-", "-");
            tabControl.TabPages["tabVaccin"].Controls.Add(dgvV);

            // RDV tab
            var dgvR = BuildGrid();
            dgvR.Columns.Add("Date", "Date");
            dgvR.Columns.Add("Heure", "Heure");
            dgvR.Columns.Add("Motif", "Motif");
            var rdvs = dalR.GetAll();
            bool hasRdv = false;
            foreach (var r in rdvs)
            {
                if (r.IdAnimal == _animal.IdAnimal)
                {
                    dgvR.Rows.Add(r.DateRdv.ToShortDateString(), r.HeureRdv.ToString(@"hh\:mm"), r.Motif);
                    hasRdv = true;
                }
            }
            if (!hasRdv) dgvR.Rows.Add("-", "-", "Aucun rendez-vous");
            tabControl.TabPages["tabRdv"].Controls.Add(dgvR);
        }
    }
}
