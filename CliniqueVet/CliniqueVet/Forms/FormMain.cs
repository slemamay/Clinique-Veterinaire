using System;
using System.Windows.Forms;
using System.Drawing;
using CliniqueVet.DAL;

namespace CliniqueVet.Forms
{
    public class FormMain : Form
    {
        private Panel panelSidebar;
        private Panel panelContent;
        private Panel panelHeader;
        private Label lblTitle;
        private Label lblUser;
        private Button btnCurrentActive;

        public FormMain()
        {
            InitializeComponent();
            LoadDashboard();
            // Feature 3: check medication alerts on startup
            FormAlertesMedicaments.CheckAndShow();
        }

        private void InitializeComponent()
        {
            this.Text = "Clinique Veterinaire - " + Session.CurrentUser.NomComplet + " (" + Session.CurrentUser.Role + ")";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1100, 650);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 55;
            panelHeader.BackColor = Color.FromArgb(41, 128, 185);

            lblTitle = new Label();
            lblTitle.Text = "Clinique Veterinaire";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(15, 15);

            lblUser = new Label();
            lblUser.Text = Session.CurrentUser.NomComplet + " | " + Session.CurrentUser.Role;
            lblUser.Font = new Font("Segoe UI", 9);
            lblUser.ForeColor = Color.FromArgb(200, 230, 255);
            lblUser.AutoSize = true;
            lblUser.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            var btnLogout = new Button();
            btnLogout.Text = "Deconnexion";
            btnLogout.Font = new Font("Segoe UI", 9);
            btnLogout.ForeColor = Color.White;
            btnLogout.BackColor = Color.FromArgb(192, 57, 43);
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Size = new Size(110, 30);
            btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Click += (s, e) =>
            {
                Session.CurrentUser = null;
                var login = new FormLogin();
                login.Show();
                this.Close();
            };

            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblUser);
            panelHeader.Controls.Add(btnLogout);

            panelHeader.Resize += (s, e) =>
            {
                lblUser.Location = new Point(panelHeader.Width - lblUser.Width - 130, 10);
                btnLogout.Location = new Point(panelHeader.Width - 120, 12);
            };

            panelSidebar = new Panel();
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Width = 210;
            panelSidebar.BackColor = Color.FromArgb(35, 47, 62);

            panelContent = new Panel();
            panelContent.Dock = DockStyle.Fill;
            panelContent.BackColor = Color.FromArgb(245, 247, 250);
            panelContent.Padding = new Padding(15);

            BuildSidebar();

            this.Controls.Add(panelContent);
            this.Controls.Add(panelSidebar);
            this.Controls.Add(panelHeader);
        }

        private void BuildSidebar()
        {
            panelSidebar.Controls.Clear();
            int y = 15;

            AddNavButton("Tableau de bord", y, () => LoadDashboard()); y += 48;

            if (Session.IsAdmin || Session.IsSecretaire)
            {
                AddNavLabel("GESTION", y); y += 28;
                AddNavButton("Proprietaires", y, () => LoadForm(new FormProprietaires())); y += 44;
                AddNavButton("Animaux", y, () => LoadForm(new FormAnimaux())); y += 44;
                AddNavButton("Rendez-vous", y, () => LoadForm(new FormRDV())); y += 44;
                AddNavButton("Calendrier RDV", y, () => { var f = new FormCalendrier(); f.ShowDialog(); }); y += 44;
                AddNavButton("Factures", y, () => LoadForm(new FormFactures())); y += 44;
            }

            if (Session.IsAdmin || Session.IsVeterinaire)
            {
                AddNavLabel("MEDICAL", y); y += 28;
                AddNavButton("Consultations", y, () => LoadForm(new FormConsultations())); y += 44;
                AddNavButton("Vaccinations", y, () => LoadForm(new FormVaccinations())); y += 44;
                AddNavButton("Medicaments", y, () => LoadForm(new FormMedicaments())); y += 44;
                AddNavButton("Alertes Meds", y, () => { var f = new FormAlertesMedicaments(); f.ShowDialog(); }); y += 44;
            }

            if (Session.IsVeterinaire)
            {
                AddNavLabel("PLANNING", y); y += 28;
                AddNavButton("Mon Planning", y, () => LoadForm(new FormRDV())); y += 44;
            }

            if (Session.IsAdmin)
            {
                AddNavLabel("ADMINISTRATION", y); y += 28;
                AddNavButton("Utilisateurs", y, () => LoadForm(new FormUtilisateurs())); y += 44;
                AddNavButton("Rapport Financier", y, () => { var f = new FormRapportFinancier(); f.ShowDialog(); }); y += 44;
            }
        }

        private void AddNavLabel(string text, int y)
        {
            var lbl = new Label();
            lbl.Text = text;
            lbl.Location = new Point(15, y);
            lbl.AutoSize = true;
            lbl.Font = new Font("Segoe UI", 7, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(100, 130, 160);
            panelSidebar.Controls.Add(lbl);
        }

        private void AddNavButton(string text, int y, Action onClick)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Location = new Point(4, y);
            btn.Size = new Size(202, 38);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 9);
            btn.ForeColor = Color.FromArgb(200, 215, 230);
            btn.BackColor = Color.Transparent;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(15, 0, 0, 0);
            btn.Cursor = Cursors.Hand;
            btn.Click += (s, e) =>
            {
                if (btnCurrentActive != null)
                {
                    btnCurrentActive.BackColor = Color.Transparent;
                    btnCurrentActive.ForeColor = Color.FromArgb(200, 215, 230);
                }
                btn.BackColor = Color.FromArgb(52, 73, 94);
                btn.ForeColor = Color.White;
                btnCurrentActive = btn;
                onClick();
            };
            panelSidebar.Controls.Add(btn);
        }

        public void LoadForm(Form form)
        {
            panelContent.Controls.Clear();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            panelContent.Controls.Add(form);
            form.Show();
        }

        private void LoadDashboard()
        {
            LoadForm(new FormDashboard());
        }
    }
}
