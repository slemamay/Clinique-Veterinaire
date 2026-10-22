using System;
using System.Windows.Forms;
using System.Drawing;
using CliniqueVet.DAL;
using CliniqueVet.Models;

namespace CliniqueVet.Forms
{
    public class FormConsultations : Form
    {
        private DataGridView dgv;
        private Button btnAdd, btnEdit, btnDelete;
        private ConsultationDAL dal = new ConsultationDAL();
        private FactureDAL dalF = new FactureDAL();

        public FormConsultations()
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
            lblTitle.Text = "Gestion des Consultations";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(40, 55, 71);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(10, 8);
            panelTop.Controls.Add(lblTitle);

            var flow = new FlowLayoutPanel();
            flow.Location = new Point(10, 51);
            flow.Size = new Size(800, 36);
            flow.FlowDirection = FlowDirection.LeftToRight;
            flow.WrapContents = false;
            flow.BackColor = Color.Transparent;

            btnAdd = MakeBtn("Ajouter", Color.FromArgb(39, 174, 96));
            btnAdd.Click += (s, e) => { var f = new FormConsultationEdit(null); if (f.ShowDialog() == DialogResult.OK) LoadData(); };
            btnEdit = MakeBtn("Modifier", Color.FromArgb(41, 128, 185));
            btnEdit.Click += BtnEdit_Click;
            btnDelete = MakeBtn("Supprimer", Color.FromArgb(192, 57, 43));
            btnDelete.Click += BtnDelete_Click;
            var btnFacture = MakeBtn("Creer Facture", Color.FromArgb(142, 68, 173));
            btnFacture.Click += BtnFacture_Click;
            var btnMeds = MakeBtn("Medicaments", Color.FromArgb(211, 84, 0));
            btnMeds.Click += BtnMeds_Click;
            var btnRefresh = MakeBtn("Actualiser", Color.FromArgb(127, 140, 141));
            btnRefresh.Click += (s, e) => LoadData();

            flow.Controls.Add(btnAdd);
            flow.Controls.Add(btnEdit);
            flow.Controls.Add(btnDelete);
            flow.Controls.Add(btnFacture);
            flow.Controls.Add(btnMeds);
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
            dgv.Columns.Add("id_consult", "ID"); dgv.Columns["id_consult"].Visible = false;
            dgv.Columns.Add("Date", "Date");
            dgv.Columns.Add("Animal", "Animal");
            dgv.Columns.Add("Proprietaire", "Proprietaire");
            dgv.Columns.Add("Diagnostic", "Diagnostic");
            dgv.Rows.Clear();
            foreach (var c in dal.GetAll())
                dgv.Rows.Add(c.IdConsult, c.DateConsult.ToShortDateString(), c.NomAnimal, c.NomProprietaire, c.Diagnostic);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez une consultation."); return; }
            var row = dgv.SelectedRows[0];
            var c = new Consultation { IdConsult = (int)row.Cells["id_consult"].Value, DateConsult = DateTime.Parse(row.Cells["Date"].Value.ToString()), Diagnostic = row.Cells["Diagnostic"].Value?.ToString() };
            var f = new FormConsultationEdit(c);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez une consultation."); return; }
            if (MessageBox.Show("Supprimer cette consultation et sa facture?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try { dal.Delete((int)dgv.SelectedRows[0].Cells["id_consult"].Value); LoadData(); }
                catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message); }
            }
        }

        private void BtnFacture_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez une consultation."); return; }
            int idConsult = (int)dgv.SelectedRows[0].Cells["id_consult"].Value;
            if (dalF.ExistsForConsultation(idConsult)) { MessageBox.Show("Une facture existe deja pour cette consultation.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            var f = new FormFactureEdit(null, idConsult);
            if (f.ShowDialog() == DialogResult.OK) MessageBox.Show("Facture creee.", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnMeds_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez une consultation."); return; }
            new FormConsultationMedicaments((int)dgv.SelectedRows[0].Cells["id_consult"].Value).ShowDialog();
        }
    }

    public class FormConsultationEdit : Form
    {
        private DateTimePicker dtpDate;
        private TextBox txtDiagnostic;
        private ComboBox cmbAnimal;
        private Consultation _consult;
        private ConsultationDAL dal = new ConsultationDAL();
        private AnimalDAL dalA = new AnimalDAL();

        public FormConsultationEdit(Consultation c)
        {
            _consult = c;
            this.Text = c == null ? "Nouvelle consultation" : "Modifier consultation";
            this.Size = new Size(440, 360);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            InitUI();
        }

        private void InitUI()
        {
            int y = 20;
            this.Controls.Add(new Label { Text = "Animal:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            cmbAnimal = new ComboBox { Location = new Point(20, y + 20), Size = new Size(380, 25), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            var animals = dalA.GetAll();
            cmbAnimal.DisplayMember = "Nom"; cmbAnimal.ValueMember = "IdAnimal"; cmbAnimal.DataSource = animals;
            this.Controls.Add(cmbAnimal); y += 55;

            this.Controls.Add(new Label { Text = "Date:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            dtpDate = new DateTimePicker { Location = new Point(20, y + 20), Size = new Size(380, 25), Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short };
            this.Controls.Add(dtpDate); y += 55;

            this.Controls.Add(new Label { Text = "Diagnostic / Traitement:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            txtDiagnostic = new TextBox { Location = new Point(20, y + 20), Size = new Size(380, 80), Font = new Font("Segoe UI", 10), Multiline = true, ScrollBars = ScrollBars.Vertical };
            this.Controls.Add(txtDiagnostic); y += 105;

            var btnSave = new Button { Text = "Enregistrer", DialogResult = DialogResult.OK, Location = new Point(110, y + 10), Size = new Size(120, 32), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.FlatAppearance.BorderSize = 0; btnSave.Click += BtnSave_Click;
            var btnCancel = new Button { Text = "Annuler", DialogResult = DialogResult.Cancel, Location = new Point(245, y + 10), Size = new Size(100, 32), BackColor = Color.FromArgb(127, 140, 141), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnSave); this.Controls.Add(btnCancel);
            this.AcceptButton = btnSave; this.CancelButton = btnCancel;

            if (_consult != null) { dtpDate.Value = _consult.DateConsult; txtDiagnostic.Text = _consult.Diagnostic; }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbAnimal.SelectedItem == null) { MessageBox.Show("Selectionnez un animal."); this.DialogResult = DialogResult.None; return; }
            try
            {
                int idAnimal = ((Animal)cmbAnimal.SelectedItem).IdAnimal;
                if (_consult == null)
                    dal.Insert(new Consultation { DateConsult = dtpDate.Value.Date, Diagnostic = txtDiagnostic.Text.Trim(), IdAnimal = idAnimal });
                else { _consult.DateConsult = dtpDate.Value.Date; _consult.Diagnostic = txtDiagnostic.Text.Trim(); _consult.IdAnimal = idAnimal; dal.Update(_consult); }
            }
            catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message); this.DialogResult = DialogResult.None; }
        }
    }

    public class FormConsultationMedicaments : Form
    {
        private DataGridView dgv;
        private ComboBox cmbMed;
        private NumericUpDown numQte;
        private int _idConsult;
        private ConsultationDAL dal = new ConsultationDAL();
        private MedicamentDAL dalM = new MedicamentDAL();

        public FormConsultationMedicaments(int idConsult)
        {
            _idConsult = idConsult;
            this.Text = "Medicaments de la consultation";
            this.Size = new Size(560, 420);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            InitUI();
            LoadMeds();
        }

        private void InitUI()
        {
            this.Controls.Add(new Label { Text = "Ajouter un medicament:", Location = new Point(15, 15), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) });
            this.Controls.Add(new Label { Text = "Medicament:", Location = new Point(15, 45), AutoSize = true });
            cmbMed = new ComboBox { Location = new Point(110, 42), Size = new Size(250, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9) };
            var meds = dalM.GetAll();
            cmbMed.DisplayMember = "Nom"; cmbMed.ValueMember = "IdMedicament"; cmbMed.DataSource = meds;
            this.Controls.Add(cmbMed);

            this.Controls.Add(new Label { Text = "Quantite:", Location = new Point(375, 45), AutoSize = true });
            numQte = new NumericUpDown { Location = new Point(440, 42), Size = new Size(60, 25), Minimum = 1, Maximum = 1000, Value = 1 };
            this.Controls.Add(numQte);

            var btnAdd = new Button { Text = "Ajouter", Location = new Point(15, 75), Size = new Size(100, 30), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            dgv = new DataGridView { Location = new Point(15, 115), Size = new Size(510, 230), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 9) };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            this.Controls.Add(dgv);

            var btnClose = new Button { Text = "Fermer", Location = new Point(395, 355), Size = new Size(130, 32), BackColor = Color.FromArgb(127, 140, 141), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.OK };
            btnClose.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnClose);
        }

        private void LoadMeds()
        {
            dgv.Columns.Clear();
            dgv.Columns.Add("Medicament", "Medicament");
            dgv.Columns.Add("Quantite", "Quantite utilisee");
            dgv.Rows.Clear();
            foreach (var m in dal.GetMedicamentsForConsultation(_idConsult))
                dgv.Rows.Add(m.Nom, m.Quantite);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (cmbMed.SelectedItem == null) return;
            try
            {
                dal.AddMedicament(_idConsult, ((Medicament)cmbMed.SelectedItem).IdMedicament, (int)numQte.Value);
                LoadMeds();
                MessageBox.Show("Medicament ajoute.", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message); }
        }
    }
}
