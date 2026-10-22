using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using CliniqueVet.DAL;
using CliniqueVet.Models;

namespace CliniqueVet.Forms
{
    public class FormProprietaires : Form
    {
        private DataGridView dgv;
        private TextBox txtSearch;
        private Button btnAdd, btnEdit, btnDelete;
        private ProprietaireDAL dal = new ProprietaireDAL();

        public FormProprietaires()
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
            lblTitle.Text = "Gestion des Proprietaires";
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
            flow.Size = new Size(600, 36);
            flow.FlowDirection = FlowDirection.LeftToRight;
            flow.WrapContents = false;
            flow.BackColor = Color.Transparent;

            btnAdd = MakeBtn("Ajouter", Color.FromArgb(39, 174, 96));
            btnAdd.Click += (s, e) => { var f = new FormProprietaireEdit(null); if (f.ShowDialog() == DialogResult.OK) LoadData(); };
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

        private void LoadData(List<Proprietaire> data = null)
        {
            dgv.Columns.Clear();
            dgv.Columns.Add("id_prop", "ID"); dgv.Columns["id_prop"].Visible = false;
            dgv.Columns.Add("CIN", "CIN");
            dgv.Columns.Add("Nom", "Nom");
            dgv.Rows.Clear();
            foreach (var p in data ?? dal.GetAll())
                dgv.Rows.Add(p.IdProp, p.CIN, p.Nom);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez un proprietaire."); return; }
            var row = dgv.SelectedRows[0];
            var p = new Proprietaire { IdProp = (int)row.Cells["id_prop"].Value, CIN = row.Cells["CIN"].Value.ToString(), Nom = row.Cells["Nom"].Value.ToString() };
            var f = new FormProprietaireEdit(p);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez un proprietaire."); return; }
            if (MessageBox.Show("Confirmer la suppression?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try { dal.Delete((int)dgv.SelectedRows[0].Cells["id_prop"].Value); LoadData(); }
                catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
    }

    public class FormProprietaireEdit : Form
    {
        private TextBox txtCIN, txtNom;
        private ProprietaireDAL dal = new ProprietaireDAL();
        private Proprietaire _proprietaire;

        public FormProprietaireEdit(Proprietaire p)
        {
            _proprietaire = p;
            this.Text = p == null ? "Ajouter un proprietaire" : "Modifier un proprietaire";
            this.Size = new Size(380, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            InitUI();
            if (p != null) { txtCIN.Text = p.CIN; txtNom.Text = p.Nom; }
        }

        private void InitUI()
        {
            int y = 20;
            AddField("CIN:", ref txtCIN, ref y);
            AddField("Nom:", ref txtNom, ref y);

            var btnSave = new Button { Text = "Enregistrer", DialogResult = DialogResult.OK, Location = new Point(100, y + 10), Size = new Size(100, 32), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            var btnCancel = new Button { Text = "Annuler", DialogResult = DialogResult.Cancel, Location = new Point(215, y + 10), Size = new Size(100, 32), BackColor = Color.FromArgb(127, 140, 141), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnSave); this.Controls.Add(btnCancel);
            this.AcceptButton = btnSave; this.CancelButton = btnCancel;
        }

        private void AddField(string label, ref TextBox txt, ref int y)
        {
            this.Controls.Add(new Label { Text = label, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            txt = new TextBox { Location = new Point(20, y + 20), Size = new Size(320, 25), Font = new Font("Segoe UI", 10) };
            this.Controls.Add(txt);
            y += 55;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCIN.Text) || string.IsNullOrWhiteSpace(txtNom.Text))
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None; return;
            }
            try
            {
                if (_proprietaire == null)
                    dal.Insert(new Proprietaire { CIN = txtCIN.Text.Trim(), Nom = txtNom.Text.Trim() });
                else { _proprietaire.CIN = txtCIN.Text.Trim(); _proprietaire.Nom = txtNom.Text.Trim(); dal.Update(_proprietaire); }
            }
            catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message); this.DialogResult = DialogResult.None; }
        }
    }
}
