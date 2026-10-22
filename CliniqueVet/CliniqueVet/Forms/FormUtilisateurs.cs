using System;
using System.Windows.Forms;
using System.Drawing;
using CliniqueVet.DAL;
using CliniqueVet.Models;

namespace CliniqueVet.Forms
{
    public class FormUtilisateurs : Form
    {
        private DataGridView dgv;
        private Button btnAdd, btnEdit, btnDelete;
        private UtilisateurDAL dal = new UtilisateurDAL();

        public FormUtilisateurs()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            InitUI();
            LoadData();
        }

        private void InitUI()
        {
            var panelTop = new Panel();
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 95;
            panelTop.BackColor = Color.FromArgb(245, 247, 250);

            var lblTitle = new Label();
            lblTitle.Text = "Gestion des Utilisateurs";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(40, 55, 71);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(10, 8);
            panelTop.Controls.Add(lblTitle);

            var flow = new FlowLayoutPanel();
            flow.Location = new Point(10, 51);
            flow.Size = new Size(500, 36);
            flow.FlowDirection = FlowDirection.LeftToRight;
            flow.WrapContents = false;
            flow.BackColor = Color.Transparent;

            btnAdd = MakeBtn("Ajouter", Color.FromArgb(39, 174, 96));
            btnAdd.Click += (s, e) => { var f = new FormUtilisateurEdit(null); if (f.ShowDialog() == DialogResult.OK) LoadData(); };
            btnEdit = MakeBtn("Modifier", Color.FromArgb(41, 128, 185));
            btnEdit.Click += BtnEdit_Click;
            btnDelete = MakeBtn("Supprimer", Color.FromArgb(192, 57, 43));
            btnDelete.Click += BtnDelete_Click;
            var btnRefresh = MakeBtn("Actualiser", Color.FromArgb(127, 140, 141));
            btnRefresh.Click += (s, e) => LoadData();

            flow.Controls.Add(btnAdd);
            flow.Controls.Add(btnEdit);
            flow.Controls.Add(btnDelete);
            flow.Controls.Add(btnRefresh);
            panelTop.Controls.Add(flow);

            var panelGrid = new Panel();
            panelGrid.Dock = DockStyle.Fill;
            panelGrid.Padding = new Padding(10, 5, 10, 10);

            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.Font = new Font("Segoe UI", 9);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            panelGrid.Controls.Add(dgv);

            this.Controls.Add(panelGrid);
            this.Controls.Add(panelTop);
        }

        private Button MakeBtn(string text, Color color)
        {
            var btn = new Button();
            btn.Text = text;
            btn.Size = new Size(105, 30);
            btn.Margin = new Padding(0, 0, 5, 0);
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void LoadData()
        {
            dgv.Columns.Clear();
            dgv.Columns.Add("id_user", "ID"); dgv.Columns["id_user"].Visible = false;
            dgv.Columns.Add("Nom", "Nom");
            dgv.Columns.Add("Prenom", "Prenom");
            dgv.Columns.Add("CIN", "CIN");
            dgv.Columns.Add("Email", "Email");
            dgv.Columns.Add("Role", "Role");
            dgv.Rows.Clear();
            foreach (var u in dal.GetAll())
                dgv.Rows.Add(u.IdUser, u.Nom, u.Prenom, u.CIN, u.Email, u.Role);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez un utilisateur."); return; }
            var row = dgv.SelectedRows[0];
            var u = new Utilisateur { IdUser = (int)row.Cells["id_user"].Value, Nom = row.Cells["Nom"].Value.ToString(), Prenom = row.Cells["Prenom"].Value.ToString(), CIN = row.Cells["CIN"].Value.ToString(), Email = row.Cells["Email"].Value.ToString(), Role = row.Cells["Role"].Value.ToString() };
            var f = new FormUtilisateurEdit(u);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez un utilisateur."); return; }
            int id = (int)dgv.SelectedRows[0].Cells["id_user"].Value;
            if (id == Session.CurrentUser.IdUser) { MessageBox.Show("Vous ne pouvez pas supprimer votre propre compte.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (MessageBox.Show("Supprimer cet utilisateur?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try { dal.Delete(id); LoadData(); }
                catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message); }
            }
        }
    }

    public class FormUtilisateurEdit : Form
    {
        private TextBox txtNom, txtPrenom, txtCIN, txtEmail, txtPassword;
        private ComboBox cmbRole;
        private Utilisateur _user;
        private UtilisateurDAL dal = new UtilisateurDAL();

        public FormUtilisateurEdit(Utilisateur u)
        {
            _user = u;
            this.Text = u == null ? "Nouvel utilisateur" : "Modifier utilisateur";
            this.Size = new Size(420, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            InitUI();
        }

        private void InitUI()
        {
            int y = 20;
            AddField("Nom:", ref txtNom, ref y);
            AddField("Prenom:", ref txtPrenom, ref y);
            AddField("CIN:", ref txtCIN, ref y);
            AddField("Email:", ref txtEmail, ref y);
            AddField("Mot de passe:", ref txtPassword, ref y);
            txtPassword.PasswordChar = '*';

            this.Controls.Add(new Label { Text = "Role:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            cmbRole = new ComboBox { Location = new Point(20, y + 20), Size = new Size(360, 25), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new string[] { "Administrateur", "Secretaire", "Veterinaire" });
            cmbRole.SelectedIndex = 1;
            this.Controls.Add(cmbRole); y += 55;

            var btnSave = new Button { Text = "Enregistrer", DialogResult = DialogResult.OK, Location = new Point(90, y + 10), Size = new Size(120, 32), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.FlatAppearance.BorderSize = 0; btnSave.Click += BtnSave_Click;
            var btnCancel = new Button { Text = "Annuler", DialogResult = DialogResult.Cancel, Location = new Point(225, y + 10), Size = new Size(100, 32), BackColor = Color.FromArgb(127, 140, 141), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnSave); this.Controls.Add(btnCancel);
            this.AcceptButton = btnSave; this.CancelButton = btnCancel;

            if (_user != null) { txtNom.Text = _user.Nom; txtPrenom.Text = _user.Prenom; txtCIN.Text = _user.CIN; txtEmail.Text = _user.Email; txtPassword.Text = _user.Password; cmbRole.SelectedItem = _user.Role; }
        }

        private void AddField(string label, ref TextBox txt, ref int y)
        {
            this.Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            txt = new TextBox { Location = new Point(20, y + 20), Size = new Size(360, 25), Font = new Font("Segoe UI", 10) };
            this.Controls.Add(txt); y += 55;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtPrenom.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            { MessageBox.Show("Tous les champs obligatoires doivent etre remplis."); this.DialogResult = DialogResult.None; return; }
            try
            {
                var u = new Utilisateur { Nom = txtNom.Text.Trim(), Prenom = txtPrenom.Text.Trim(), CIN = txtCIN.Text.Trim(), Email = txtEmail.Text.Trim(), Password = txtPassword.Text, Role = cmbRole.SelectedItem.ToString() };
                if (_user == null) dal.Insert(u);
                else { u.IdUser = _user.IdUser; dal.Update(u); }
            }
            catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message); this.DialogResult = DialogResult.None; }
        }
    }
}
