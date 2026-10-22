using System;
using System.Windows.Forms;
using System.Drawing;
using CliniqueVet.DAL;
using CliniqueVet.Models;

namespace CliniqueVet.Forms
{
    public class FormVaccinations : Form
    {
        private DataGridView dgv;
        private Button btnAdd, btnEdit, btnDelete;
        private VaccinationDAL dal = new VaccinationDAL();

        public FormVaccinations()
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
            lblTitle.Text = "Gestion des Vaccinations";
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
            btnAdd.Click += (s, e) => { var f = new FormVaccinationEdit(null); if (f.ShowDialog() == DialogResult.OK) LoadData(); };
            btnEdit = MakeBtn("Modifier", Color.FromArgb(41, 128, 185));
            btnEdit.Click += BtnEdit_Click;
            btnDelete = MakeBtn("Supprimer", Color.FromArgb(192, 57, 43));
            btnDelete.Click += BtnDelete_Click;
            var btnRefresh = MakeBtn("Actualiser", Color.FromArgb(127, 140, 141));
            btnRefresh.Click += (s, e) => LoadData();

            flow.Controls.Add(btnAdd); flow.Controls.Add(btnEdit);
            flow.Controls.Add(btnDelete); flow.Controls.Add(btnRefresh);
            panelTop.Controls.Add(flow);

            var panelGrid = new Panel();
            panelGrid.Dock = DockStyle.Fill;
            panelGrid.Padding = new Padding(10, 5, 10, 10);

            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ReadOnly = true; dgv.AllowUserToAddRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.MultiSelect = false;
            dgv.RowHeadersVisible = false; dgv.BackgroundColor = Color.White; dgv.BorderStyle = BorderStyle.None;
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
            var btn = new Button(); btn.Text = text; btn.Size = new Size(105, 30); btn.Margin = new Padding(0, 0, 5, 0);
            btn.BackColor = color; btn.ForeColor = Color.White; btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0; btn.Font = new Font("Segoe UI", 9, FontStyle.Bold); btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void LoadData()
        {
            dgv.Columns.Clear();
            dgv.Columns.Add("id_vaccin", "ID"); dgv.Columns["id_vaccin"].Visible = false;
            dgv.Columns.Add("Nom", "Vaccin"); dgv.Columns.Add("Animal", "Animal");
            dgv.Columns.Add("DateVaccin", "Date"); dgv.Columns.Add("DateRappel", "Rappel");
            dgv.Rows.Clear();
            foreach (var v in dal.GetAll())
                dgv.Rows.Add(v.IdVaccin, v.Nom, v.NomAnimal, v.DateVaccin.ToShortDateString(), v.DateRappel.HasValue ? v.DateRappel.Value.ToShortDateString() : "-");
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez une vaccination."); return; }
            var row = dgv.SelectedRows[0];
            var v = new Vaccination { IdVaccin = (int)row.Cells["id_vaccin"].Value, Nom = row.Cells["Nom"].Value.ToString(), DateVaccin = DateTime.Parse(row.Cells["DateVaccin"].Value.ToString()) };
            var f = new FormVaccinationEdit(v);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez une vaccination."); return; }
            if (MessageBox.Show("Supprimer?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try { dal.Delete((int)dgv.SelectedRows[0].Cells["id_vaccin"].Value); LoadData(); }
                catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message); }
            }
        }
    }

    public class FormVaccinationEdit : Form
    {
        private TextBox txtNom;
        private DateTimePicker dtpDate, dtpRappel;
        private CheckBox chkRappel;
        private ComboBox cmbAnimal;
        private Vaccination _vaccin;
        private VaccinationDAL dal = new VaccinationDAL();
        private AnimalDAL dalA = new AnimalDAL();

        public FormVaccinationEdit(Vaccination v)
        {
            _vaccin = v;
            this.Text = v == null ? "Nouvelle vaccination" : "Modifier vaccination";
            this.Size = new Size(440, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.White;
            InitUI();
        }

        private void InitUI()
        {
            int y = 20;
            this.Controls.Add(new Label { Text = "Animal:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            cmbAnimal = new ComboBox { Location = new Point(20, y + 20), Size = new Size(380, 25), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            var animals = dalA.GetAll(); cmbAnimal.DisplayMember = "Nom"; cmbAnimal.ValueMember = "IdAnimal"; cmbAnimal.DataSource = animals;
            this.Controls.Add(cmbAnimal); y += 55;

            this.Controls.Add(new Label { Text = "Nom du vaccin:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            txtNom = new TextBox { Location = new Point(20, y + 20), Size = new Size(380, 25), Font = new Font("Segoe UI", 10) };
            this.Controls.Add(txtNom); y += 55;

            this.Controls.Add(new Label { Text = "Date de vaccination:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            dtpDate = new DateTimePicker { Location = new Point(20, y + 20), Size = new Size(380, 25), Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short };
            this.Controls.Add(dtpDate); y += 55;

            chkRappel = new CheckBox { Text = "Definir une date de rappel", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) };
            chkRappel.CheckedChanged += (s, e) => dtpRappel.Enabled = chkRappel.Checked;
            this.Controls.Add(chkRappel); y += 28;

            this.Controls.Add(new Label { Text = "Date de rappel:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            dtpRappel = new DateTimePicker { Location = new Point(20, y + 20), Size = new Size(380, 25), Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short, Enabled = false };
            this.Controls.Add(dtpRappel); y += 55;

            var btnSave = new Button { Text = "Enregistrer", DialogResult = DialogResult.OK, Location = new Point(110, y + 10), Size = new Size(120, 32), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.FlatAppearance.BorderSize = 0; btnSave.Click += BtnSave_Click;
            var btnCancel = new Button { Text = "Annuler", DialogResult = DialogResult.Cancel, Location = new Point(245, y + 10), Size = new Size(100, 32), BackColor = Color.FromArgb(127, 140, 141), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnSave); this.Controls.Add(btnCancel);

            if (_vaccin != null) { txtNom.Text = _vaccin.Nom; dtpDate.Value = _vaccin.DateVaccin; if (_vaccin.DateRappel.HasValue) { chkRappel.Checked = true; dtpRappel.Value = _vaccin.DateRappel.Value; dtpRappel.Enabled = true; } }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNom.Text) || cmbAnimal.SelectedItem == null) { MessageBox.Show("Champs obligatoires manquants."); this.DialogResult = DialogResult.None; return; }
            try
            {
                int idAnimal = ((Animal)cmbAnimal.SelectedItem).IdAnimal;
                DateTime? rappel = chkRappel.Checked ? (DateTime?)dtpRappel.Value.Date : null;
                if (_vaccin == null) dal.Insert(new Vaccination { Nom = txtNom.Text.Trim(), DateVaccin = dtpDate.Value.Date, DateRappel = rappel, IdAnimal = idAnimal });
                else { _vaccin.Nom = txtNom.Text.Trim(); _vaccin.DateVaccin = dtpDate.Value.Date; _vaccin.DateRappel = rappel; _vaccin.IdAnimal = idAnimal; dal.Update(_vaccin); }
            }
            catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message); this.DialogResult = DialogResult.None; }
        }
    }

    public class FormMedicaments : Form
    {
        private DataGridView dgv;
        private Button btnAdd, btnEdit, btnDelete;
        private MedicamentDAL dal = new MedicamentDAL();

        public FormMedicaments()
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
            lblTitle.Text = "Gestion des Medicaments";
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
            btnAdd.Click += (s, e) => { var f = new FormMedicamentEdit(null); if (f.ShowDialog() == DialogResult.OK) LoadData(); };
            btnEdit = MakeBtn("Modifier", Color.FromArgb(41, 128, 185));
            btnEdit.Click += BtnEdit_Click;
            btnDelete = MakeBtn("Supprimer", Color.FromArgb(192, 57, 43));
            btnDelete.Click += BtnDelete_Click;
            var btnRefresh = MakeBtn("Actualiser", Color.FromArgb(127, 140, 141));
            btnRefresh.Click += (s, e) => LoadData();

            flow.Controls.Add(btnAdd); flow.Controls.Add(btnEdit);
            flow.Controls.Add(btnDelete); flow.Controls.Add(btnRefresh);
            panelTop.Controls.Add(flow);

            var panelGrid = new Panel();
            panelGrid.Dock = DockStyle.Fill;
            panelGrid.Padding = new Padding(10, 5, 10, 10);

            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ReadOnly = true; dgv.AllowUserToAddRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgv.MultiSelect = false;
            dgv.RowHeadersVisible = false; dgv.BackgroundColor = Color.White; dgv.BorderStyle = BorderStyle.None;
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
            var btn = new Button(); btn.Text = text; btn.Size = new Size(105, 30); btn.Margin = new Padding(0, 0, 5, 0);
            btn.BackColor = color; btn.ForeColor = Color.White; btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0; btn.Font = new Font("Segoe UI", 9, FontStyle.Bold); btn.Cursor = Cursors.Hand;
            return btn;
        }

        private void LoadData()
        {
            dgv.Columns.Clear();
            dgv.Columns.Add("id_med", "ID"); dgv.Columns["id_med"].Visible = false;
            dgv.Columns.Add("Nom", "Medicament");
            dgv.Columns.Add("Quantite", "Quantite en stock");
            dgv.Columns.Add("Expiration", "Date d'expiration");
            dgv.Rows.Clear();
            foreach (var m in dal.GetAll())
            {
                int idx = dgv.Rows.Add(m.IdMedicament, m.Nom, m.Quantite, m.DateExpiration.HasValue ? m.DateExpiration.Value.ToShortDateString() : "N/A");
                if (m.Quantite < 10) dgv.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                else if (m.DateExpiration.HasValue && m.DateExpiration.Value < DateTime.Now.AddMonths(3)) dgv.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 220);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez un medicament."); return; }
            var row = dgv.SelectedRows[0];
            var m = new Medicament { IdMedicament = (int)row.Cells["id_med"].Value, Nom = row.Cells["Nom"].Value.ToString(), Quantite = (int)row.Cells["Quantite"].Value };
            var f = new FormMedicamentEdit(m);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez un medicament."); return; }
            if (MessageBox.Show("Supprimer?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try { dal.Delete((int)dgv.SelectedRows[0].Cells["id_med"].Value); LoadData(); }
                catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message); }
            }
        }
    }

    public class FormMedicamentEdit : Form
    {
        private TextBox txtNom;
        private NumericUpDown numQte;
        private DateTimePicker dtpExp;
        private CheckBox chkExp;
        private Medicament _med;
        private MedicamentDAL dal = new MedicamentDAL();

        public FormMedicamentEdit(Medicament m)
        {
            _med = m;
            this.Text = m == null ? "Nouveau medicament" : "Modifier medicament";
            this.Size = new Size(400, 340);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; this.BackColor = Color.White;
            InitUI();
        }

        private void InitUI()
        {
            int y = 20;
            this.Controls.Add(new Label { Text = "Nom du medicament:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            txtNom = new TextBox { Location = new Point(20, y + 20), Size = new Size(340, 25), Font = new Font("Segoe UI", 10) };
            this.Controls.Add(txtNom); y += 55;

            this.Controls.Add(new Label { Text = "Quantite en stock:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            numQte = new NumericUpDown { Location = new Point(20, y + 20), Size = new Size(340, 25), Font = new Font("Segoe UI", 10), Minimum = 0, Maximum = 100000 };
            this.Controls.Add(numQte); y += 55;

            chkExp = new CheckBox { Text = "Definir une date d'expiration", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) };
            chkExp.CheckedChanged += (s, e) => dtpExp.Enabled = chkExp.Checked;
            this.Controls.Add(chkExp); y += 28;

            this.Controls.Add(new Label { Text = "Date d'expiration:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            dtpExp = new DateTimePicker { Location = new Point(20, y + 20), Size = new Size(340, 25), Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short, Enabled = false };
            this.Controls.Add(dtpExp); y += 55;

            var btnSave = new Button { Text = "Enregistrer", DialogResult = DialogResult.OK, Location = new Point(80, y + 10), Size = new Size(120, 32), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.FlatAppearance.BorderSize = 0; btnSave.Click += BtnSave_Click;
            var btnCancel = new Button { Text = "Annuler", DialogResult = DialogResult.Cancel, Location = new Point(215, y + 10), Size = new Size(100, 32), BackColor = Color.FromArgb(127, 140, 141), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnSave); this.Controls.Add(btnCancel);

            if (_med != null) { txtNom.Text = _med.Nom; numQte.Value = _med.Quantite; if (_med.DateExpiration.HasValue) { chkExp.Checked = true; dtpExp.Value = _med.DateExpiration.Value; dtpExp.Enabled = true; } }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNom.Text)) { MessageBox.Show("Le nom est obligatoire."); this.DialogResult = DialogResult.None; return; }
            try
            {
                DateTime? exp = chkExp.Checked ? (DateTime?)dtpExp.Value.Date : null;
                if (_med == null) dal.Insert(new Medicament { Nom = txtNom.Text.Trim(), Quantite = (int)numQte.Value, DateExpiration = exp });
                else { _med.Nom = txtNom.Text.Trim(); _med.Quantite = (int)numQte.Value; _med.DateExpiration = exp; dal.Update(_med); }
            }
            catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message); this.DialogResult = DialogResult.None; }
        }
    }
}
