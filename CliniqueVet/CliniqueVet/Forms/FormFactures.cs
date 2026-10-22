using System;
using System.Windows.Forms;
using System.Drawing;
using CliniqueVet.DAL;
using CliniqueVet.Models;

namespace CliniqueVet.Forms
{
    public class FormFactures : Form
    {
        private DataGridView dgv;
        private Button btnEdit, btnDelete;
        private FactureDAL dal = new FactureDAL();

        public FormFactures()
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
            lblTitle.Text = "Gestion des Factures";
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

            btnEdit = MakeBtn("Modifier", Color.FromArgb(41, 128, 185));
            btnEdit.Click += BtnEdit_Click;
            btnDelete = MakeBtn("Supprimer", Color.FromArgb(192, 57, 43));
            btnDelete.Click += BtnDelete_Click;
            var btnRefresh = MakeBtn("Actualiser", Color.FromArgb(127, 140, 141));
            btnRefresh.Click += (s, e) => LoadData();

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
            dgv.Columns.Add("id_facture", "ID"); dgv.Columns["id_facture"].Visible = false;
            dgv.Columns.Add("id_consult", "ID Consult"); dgv.Columns["id_consult"].Visible = false;
            dgv.Columns.Add("Date", "Date");
            dgv.Columns.Add("Animal", "Animal");
            dgv.Columns.Add("Proprietaire", "Proprietaire");
            dgv.Columns.Add("Montant", "Montant (DT)");
            dgv.Rows.Clear();
            foreach (var f in dal.GetAll())
                dgv.Rows.Add(f.IdFacture, f.IdConsult, f.DateFacture.ToShortDateString(), f.NomAnimal, f.NomProprietaire, f.Montant.ToString("F2"));
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez une facture."); return; }
            var row = dgv.SelectedRows[0];
            var f = new Facture { IdFacture = (int)row.Cells["id_facture"].Value, IdConsult = (int)row.Cells["id_consult"].Value, DateFacture = DateTime.Parse(row.Cells["Date"].Value.ToString()), Montant = decimal.Parse(row.Cells["Montant"].Value.ToString()) };
            var form = new FormFactureEdit(f, f.IdConsult);
            if (form.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez une facture."); return; }
            if (MessageBox.Show("Supprimer cette facture?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try { dal.Delete((int)dgv.SelectedRows[0].Cells["id_facture"].Value); LoadData(); }
                catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message); }
            }
        }
    }

    public class FormFactureEdit : Form
    {
        private DateTimePicker dtpDate;
        private NumericUpDown numMontant;
        private Facture _facture;
        private int _idConsult;
        private FactureDAL dal = new FactureDAL();

        public FormFactureEdit(Facture f, int idConsult)
        {
            _facture = f; _idConsult = idConsult;
            this.Text = f == null ? "Nouvelle facture" : "Modifier facture";
            this.Size = new Size(360, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            InitUI();
        }

        private void InitUI()
        {
            int y = 20;
            this.Controls.Add(new Label { Text = "Consultation N.: " + _idConsult, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(41, 128, 185) });
            y += 35;

            this.Controls.Add(new Label { Text = "Date de facture:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            dtpDate = new DateTimePicker { Location = new Point(20, y + 20), Size = new Size(300, 25), Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short };
            this.Controls.Add(dtpDate); y += 55;

            this.Controls.Add(new Label { Text = "Montant (DT):", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            numMontant = new NumericUpDown { Location = new Point(20, y + 20), Size = new Size(300, 25), Font = new Font("Segoe UI", 10), Minimum = 0, Maximum = 100000, DecimalPlaces = 2, Increment = 5 };
            this.Controls.Add(numMontant); y += 55;

            var btnSave = new Button { Text = "Enregistrer", DialogResult = DialogResult.OK, Location = new Point(80, y + 10), Size = new Size(110, 32), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.FlatAppearance.BorderSize = 0; btnSave.Click += BtnSave_Click;
            var btnCancel = new Button { Text = "Annuler", DialogResult = DialogResult.Cancel, Location = new Point(205, y + 10), Size = new Size(100, 32), BackColor = Color.FromArgb(127, 140, 141), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnSave); this.Controls.Add(btnCancel);

            if (_facture != null) { dtpDate.Value = _facture.DateFacture; numMontant.Value = _facture.Montant; }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_facture == null)
                    dal.Insert(new Facture { DateFacture = dtpDate.Value.Date, Montant = numMontant.Value, IdConsult = _idConsult });
                else { _facture.DateFacture = dtpDate.Value.Date; _facture.Montant = numMontant.Value; dal.Update(_facture); }
            }
            catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message); this.DialogResult = DialogResult.None; }
        }
    }
}
