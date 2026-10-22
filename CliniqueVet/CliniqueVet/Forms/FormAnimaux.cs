using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using CliniqueVet.DAL;
using CliniqueVet.Models;

namespace CliniqueVet.Forms
{
    public class FormAnimaux : Form
    {
        private DataGridView dgv;
        private TextBox txtSearch;
        private Button btnAdd, btnEdit, btnDelete;
        private AnimalDAL dal = new AnimalDAL();

        public FormAnimaux()
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
            lblTitle.Text = "Gestion des Animaux";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(40, 55, 71);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(10, 8);
            panelTop.Controls.Add(lblTitle);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(10, 55);
            txtSearch.Size = new Size(260, 28);
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Text = "Rechercher...";
            txtSearch.GotFocus += (sx, ex) => { if (txtSearch.Text == "Rechercher...") { txtSearch.Text = ""; txtSearch.ForeColor = Color.Black; } };
            txtSearch.LostFocus += (sx, ex) => { if (txtSearch.Text == "") { txtSearch.Text = "Rechercher..."; txtSearch.ForeColor = Color.Gray; } };
            txtSearch.TextChanged += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text) || txtSearch.Text == "Rechercher...") LoadData(); else LoadData(dal.Search(txtSearch.Text)); };
            panelTop.Controls.Add(txtSearch);

            var flow = new FlowLayoutPanel();
            flow.Location = new Point(280, 51);
            flow.Size = new Size(750, 36);
            flow.FlowDirection = FlowDirection.LeftToRight;
            flow.WrapContents = false;
            flow.BackColor = Color.Transparent;

            btnAdd = MakeBtn("Ajouter", Color.FromArgb(39, 174, 96));
            btnAdd.Click += (s, e) => { var f = new FormAnimalEdit(null); if (f.ShowDialog() == DialogResult.OK) LoadData(); };
            btnEdit = MakeBtn("Modifier", Color.FromArgb(41, 128, 185));
            btnEdit.Click += BtnEdit_Click;
            btnDelete = MakeBtn("Supprimer", Color.FromArgb(192, 57, 43));
            btnDelete.Click += BtnDelete_Click;
            var btnRefresh = MakeBtn("Actualiser", Color.FromArgb(127, 140, 141));
            btnRefresh.Click += (s, e) => LoadData();
            var btnFiche = MakeBtn("Fiche Animal", Color.FromArgb(22, 160, 133));
            btnFiche.Click += BtnFiche_Click;

            flow.Controls.Add(btnAdd);
            flow.Controls.Add(btnEdit);
            flow.Controls.Add(btnDelete);
            flow.Controls.Add(btnRefresh);
            flow.Controls.Add(btnFiche);
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

        private void LoadData(List<Animal> data = null)
        {
            dgv.Columns.Clear();
            dgv.Columns.Add("id_animal", "ID"); dgv.Columns["id_animal"].Visible = false;
            dgv.Columns.Add("Nom", "Nom");
            dgv.Columns.Add("Espece", "Espece");
            dgv.Columns.Add("Proprietaire", "Proprietaire");
            dgv.Rows.Clear();
            foreach (var a in data ?? dal.GetAll())
                dgv.Rows.Add(a.IdAnimal, a.Nom, a.Espece, a.NomProprietaire);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez un animal."); return; }
            var row = dgv.SelectedRows[0];
            var a = new Animal { IdAnimal = (int)row.Cells["id_animal"].Value, Nom = row.Cells["Nom"].Value.ToString(), Espece = row.Cells["Espece"].Value.ToString() };
            var f = new FormAnimalEdit(a);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez un animal."); return; }
            if (MessageBox.Show("Confirmer la suppression?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try { dal.Delete((int)dgv.SelectedRows[0].Cells["id_animal"].Value); LoadData(); }
                catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void BtnFiche_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez un animal."); return; }
            var row = dgv.SelectedRows[0];
            var a = new Animal
            {
                IdAnimal = (int)row.Cells["id_animal"].Value,
                Nom = row.Cells["Nom"].Value.ToString(),
                Espece = row.Cells["Espece"].Value.ToString(),
                NomProprietaire = row.Cells["Proprietaire"].Value.ToString()
            };
            new FormFicheAnimal(a).ShowDialog();
        }
    }

    public class FormAnimalEdit : Form
    {
        private TextBox txtNom, txtEspece;
        private ComboBox cmbProprietaire;
        private Animal _animal;
        private AnimalDAL dalA = new AnimalDAL();
        private ProprietaireDAL dalP = new ProprietaireDAL();

        public FormAnimalEdit(Animal a)
        {
            _animal = a;
            this.Text = a == null ? "Ajouter un animal" : "Modifier un animal";
            this.Size = new Size(400, 320);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            InitUI();
        }

        private void InitUI()
        {
            int y = 20;
            AddField("Nom de l'animal:", ref txtNom, ref y);
            AddField("Espece:", ref txtEspece, ref y);

            this.Controls.Add(new Label { Text = "Proprietaire:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            cmbProprietaire = new ComboBox { Location = new Point(20, y + 20), Size = new Size(340, 25), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            var props = dalP.GetAll();
            cmbProprietaire.DisplayMember = "Nom";
            cmbProprietaire.ValueMember = "IdProp";
            cmbProprietaire.DataSource = props;
            this.Controls.Add(cmbProprietaire);
            y += 55;

            var btnSave = new Button { Text = "Enregistrer", DialogResult = DialogResult.OK, Location = new Point(100, y + 10), Size = new Size(100, 32), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            var btnCancel = new Button { Text = "Annuler", DialogResult = DialogResult.Cancel, Location = new Point(215, y + 10), Size = new Size(100, 32), BackColor = Color.FromArgb(127, 140, 141), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnSave); this.Controls.Add(btnCancel);
            this.AcceptButton = btnSave; this.CancelButton = btnCancel;

            if (_animal != null)
            {
                txtNom.Text = _animal.Nom;
                txtEspece.Text = _animal.Espece;
                foreach (Proprietaire p in props)
                    if (p.IdProp == _animal.IdProp) { cmbProprietaire.SelectedItem = p; break; }
            }
        }

        private void AddField(string label, ref TextBox txt, ref int y)
        {
            this.Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            txt = new TextBox { Location = new Point(20, y + 20), Size = new Size(340, 25), Font = new Font("Segoe UI", 10) };
            this.Controls.Add(txt);
            y += 55;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtEspece.Text) || cmbProprietaire.SelectedItem == null)
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None; return;
            }
            try
            {
                int idProp = ((Proprietaire)cmbProprietaire.SelectedItem).IdProp;
                if (_animal == null)
                    dalA.Insert(new Animal { Nom = txtNom.Text.Trim(), Espece = txtEspece.Text.Trim(), IdProp = idProp });
                else { _animal.Nom = txtNom.Text.Trim(); _animal.Espece = txtEspece.Text.Trim(); _animal.IdProp = idProp; dalA.Update(_animal); }
            }
            catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); this.DialogResult = DialogResult.None; }
        }
    }
}
