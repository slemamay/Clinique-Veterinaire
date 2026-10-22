using System;
using System.Windows.Forms;
using System.Drawing;
using CliniqueVet.DAL;
using CliniqueVet.Models;

namespace CliniqueVet.Forms
{
    public class FormLogin : Form
    {
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnLogin;

        public FormLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Clinique Veterinaire - Connexion";
            this.Size = new Size(440, 480);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // Top banner
            var panelTop = new Panel();
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 160;
            panelTop.BackColor = Color.FromArgb(41, 128, 185);

            var lblTitle = new Label();
            lblTitle.Text = "Clinique Veterinaire";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 40);

            var lblSubtitle = new Label();
            lblSubtitle.Text = "Systeme de Gestion";
            lblSubtitle.Font = new Font("Segoe UI", 11);
            lblSubtitle.ForeColor = Color.FromArgb(200, 230, 255);
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(22, 85);

            panelTop.Controls.Add(lblTitle);
            panelTop.Controls.Add(lblSubtitle);

            // Form panel
            var panelForm = new Panel();
            panelForm.Location = new Point(40, 185);
            panelForm.Size = new Size(345, 240);

            var lblEmail = new Label();
            lblEmail.Text = "Email";
            lblEmail.Font = new Font("Segoe UI", 9);
            lblEmail.ForeColor = Color.FromArgb(80, 80, 80);
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(0, 0);

            txtEmail = new TextBox();
            txtEmail.Location = new Point(0, 22);
            txtEmail.Size = new Size(345, 28);
            txtEmail.Font = new Font("Segoe UI", 10);
            txtEmail.BorderStyle = BorderStyle.FixedSingle;
            txtEmail.Text = "";

            var lblPassword = new Label();
            lblPassword.Text = "Mot de passe";
            lblPassword.Font = new Font("Segoe UI", 9);
            lblPassword.ForeColor = Color.FromArgb(80, 80, 80);
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(0, 65);

            txtPassword = new TextBox();
            txtPassword.Location = new Point(0, 87);
            txtPassword.Size = new Size(345, 28);
            txtPassword.Font = new Font("Segoe UI", 10);
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.PasswordChar = '*';
            txtPassword.Text = "";

            btnLogin = new Button();
            btnLogin.Text = "Se connecter";
            btnLogin.Location = new Point(0, 145);
            btnLogin.Size = new Size(345, 44);
            btnLogin.BackColor = Color.FromArgb(41, 128, 185);
            btnLogin.ForeColor = Color.White;
            btnLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;

            panelForm.Controls.Add(lblEmail);
            panelForm.Controls.Add(txtEmail);
            panelForm.Controls.Add(lblPassword);
            panelForm.Controls.Add(txtPassword);
            panelForm.Controls.Add(btnLogin);

            this.Controls.Add(panelTop);
            this.Controls.Add(panelForm);

            this.AcceptButton = btnLogin;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Veuillez saisir votre email et mot de passe.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Use hardcoded connection - can be changed here if needed
            DatabaseManager.Initialize(".\\SQLEXPRESS01", "FRAME");

            try
            {
                var dal = new UtilisateurDAL();
                Utilisateur user = dal.Authenticate(txtEmail.Text.Trim(), txtPassword.Text);

                if (user == null)
                {
                    MessageBox.Show("Email ou mot de passe incorrect.", "Connexion echouee", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Session.CurrentUser = user;
                var mainForm = new FormMain();
                mainForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de connexion a la base de donnees:\n" + ex.Message +
                    "\n\nVerifiez que SQL Server est demarre et que la base FRAME existe.",
                    "Erreur de connexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
