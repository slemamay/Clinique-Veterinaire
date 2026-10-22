using System;
using System.Windows.Forms;
using System.Drawing;
using CliniqueVet.DAL;
using CliniqueVet.Models;

namespace CliniqueVet.Forms
{
    public class FormRDV : Form
    {
        private DataGridView dgv;
        private TextBox txtSearch;
        private Button btnAdd, btnEdit, btnDelete;
        private CheckBox chkUpcoming;
        private RDVDAL dal = new RDVDAL();

        public FormRDV()
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
            lblTitle.Text = "Gestion des Rendez-vous";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(40, 55, 71);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(10, 8);
            panelTop.Controls.Add(lblTitle);

            chkUpcoming = new CheckBox();
            chkUpcoming.Text = "RDV a venir uniquement";
            chkUpcoming.Location = new Point(10, 55);
            chkUpcoming.AutoSize = true;
            chkUpcoming.Font = new Font("Segoe UI", 9);
            chkUpcoming.Checked = true;
            chkUpcoming.CheckedChanged += (s, e) => LoadData();
            panelTop.Controls.Add(chkUpcoming);

            var flow = new FlowLayoutPanel();
            flow.Location = new Point(230, 51);
            flow.Size = new Size(650, 36);
            flow.FlowDirection = FlowDirection.LeftToRight;
            flow.WrapContents = false;
            flow.BackColor = Color.Transparent;

            btnAdd = MakeBtn("Ajouter", Color.FromArgb(39, 174, 96));
            btnAdd.Click += (s, e) => { var f = new FormRDVEdit(null); if (f.ShowDialog() == DialogResult.OK) LoadData(); };
            btnEdit = MakeBtn("Modifier", Color.FromArgb(41, 128, 185));
            btnEdit.Click += BtnEdit_Click;
            btnDelete = MakeBtn("Annuler RDV", Color.FromArgb(192, 57, 43));
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
            btn.Size = new Size(110, 30);
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
            dgv.Columns.Add("id_rdv", "ID"); dgv.Columns["id_rdv"].Visible = false;
            dgv.Columns.Add("Date", "Date");
            dgv.Columns.Add("Heure", "Heure");
            dgv.Columns.Add("Animal", "Animal");
            dgv.Columns.Add("Proprietaire", "Proprietaire");
            dgv.Columns.Add("Motif", "Motif");
            dgv.Rows.Clear();
            var list = chkUpcoming.Checked ? dal.GetUpcoming() : dal.GetAll();
            foreach (var r in list)
                dgv.Rows.Add(r.IdRdv, r.DateRdv.ToShortDateString(), r.HeureRdv.ToString(@"hh\:mm"), r.NomAnimal, r.NomProprietaire, r.Motif);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez un rendez-vous."); return; }
            var row = dgv.SelectedRows[0];
            var r = new RDV { IdRdv = (int)row.Cells["id_rdv"].Value, DateRdv = DateTime.Parse(row.Cells["Date"].Value.ToString()), HeureRdv = TimeSpan.Parse(row.Cells["Heure"].Value.ToString()), Motif = row.Cells["Motif"].Value?.ToString() };
            var f = new FormRDVEdit(r);
            if (f.ShowDialog() == DialogResult.OK) LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Selectionnez un rendez-vous."); return; }
            if (MessageBox.Show("Annuler ce rendez-vous?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try { dal.Delete((int)dgv.SelectedRows[0].Cells["id_rdv"].Value); LoadData(); }
                catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message); }
            }
        }
    }

    public class FormRDVEdit : Form
    {
        private DateTimePicker dtpDate, dtpHeure;
        private TextBox txtMotif;
        private ComboBox cmbAnimal;
        private RDV _rdv;
        private RDVDAL dal = new RDVDAL();
        private AnimalDAL dalA = new AnimalDAL();

        public FormRDVEdit(RDV r)
        {
            _rdv = r;
            this.Text = r == null ? "Nouveau rendez-vous" : "Modifier rendez-vous";
            this.Size = new Size(420, 370);
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
            cmbAnimal = new ComboBox { Location = new Point(20, y + 20), Size = new Size(360, 25), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            var animals = dalA.GetAll();
            cmbAnimal.DisplayMember = "Nom"; cmbAnimal.ValueMember = "IdAnimal"; cmbAnimal.DataSource = animals;
            this.Controls.Add(cmbAnimal); y += 55;

            this.Controls.Add(new Label { Text = "Date:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            dtpDate = new DateTimePicker { Location = new Point(20, y + 20), Size = new Size(360, 25), Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short };
            this.Controls.Add(dtpDate); y += 55;

            this.Controls.Add(new Label { Text = "Heure:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            dtpHeure = new DateTimePicker { Location = new Point(20, y + 20), Size = new Size(360, 25), Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Time, ShowUpDown = true };
            this.Controls.Add(dtpHeure); y += 55;

            this.Controls.Add(new Label { Text = "Motif:", Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9) });
            txtMotif = new TextBox { Location = new Point(20, y + 20), Size = new Size(360, 25), Font = new Font("Segoe UI", 10) };
            this.Controls.Add(txtMotif); y += 55;

            var btnSave = new Button { Text = "Enregistrer", DialogResult = DialogResult.OK, Location = new Point(100, y + 10), Size = new Size(120, 32), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSave.FlatAppearance.BorderSize = 0; btnSave.Click += BtnSave_Click;
            var btnCancel = new Button { Text = "Annuler", DialogResult = DialogResult.Cancel, Location = new Point(235, y + 10), Size = new Size(100, 32), BackColor = Color.FromArgb(127, 140, 141), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancel.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnSave); this.Controls.Add(btnCancel);
            this.AcceptButton = btnSave; this.CancelButton = btnCancel;

            if (_rdv != null) { dtpDate.Value = _rdv.DateRdv; dtpHeure.Value = DateTime.Today.Add(_rdv.HeureRdv); txtMotif.Text = _rdv.Motif; }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbAnimal.SelectedItem == null) { MessageBox.Show("Selectionnez un animal."); this.DialogResult = DialogResult.None; return; }
            try
            {
                int idAnimal = ((Animal)cmbAnimal.SelectedItem).IdAnimal;
                if (_rdv == null)
                    dal.Insert(new RDV { DateRdv = dtpDate.Value.Date, HeureRdv = dtpHeure.Value.TimeOfDay, Motif = txtMotif.Text.Trim(), IdAnimal = idAnimal });
                else { _rdv.DateRdv = dtpDate.Value.Date; _rdv.HeureRdv = dtpHeure.Value.TimeOfDay; _rdv.Motif = txtMotif.Text.Trim(); _rdv.IdAnimal = idAnimal; dal.Update(_rdv); }
            }
            catch (Exception ex) { MessageBox.Show("Erreur: " + ex.Message); this.DialogResult = DialogResult.None; }
        }
    }
}
