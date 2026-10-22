using System;
using System.Windows.Forms;
using System.Drawing;
using CliniqueVet.DAL;
using CliniqueVet.Models;
using System.Collections.Generic;

namespace CliniqueVet.Forms
{
    public class FormAlertesMedicaments : Form
    {
        private MedicamentDAL dal = new MedicamentDAL();

        public FormAlertesMedicaments()
        {
            this.Text = "Alertes Medicaments";
            this.Size = new Size(700, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);
            InitUI();
        }

        private void InitUI()
        {
            var panelTop = new Panel();
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 60;
            panelTop.BackColor = Color.FromArgb(211, 84, 0);

            var lblTitle = new Label();
            lblTitle.Text = "Alertes stock et expiration des medicaments";
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(15, 18);
            panelTop.Controls.Add(lblTitle);

            var dgv = new DataGridView();
            dgv.Location = new Point(15, 75);
            dgv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgv.Size = new Size(655, 300);
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.Font = new Font("Segoe UI", 9);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(211, 84, 0);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;

            dgv.Columns.Add("Medicament", "Medicament");
            dgv.Columns.Add("Quantite", "Quantite en stock");
            dgv.Columns.Add("Expiration", "Date expiration");
            dgv.Columns.Add("Alerte", "Type d'alerte");

            var meds = dal.GetAll();
            foreach (var m in meds)
            {
                bool stockFaible = m.Quantite < 10;
                bool expirationProche = m.DateExpiration.HasValue && m.DateExpiration.Value < DateTime.Now.AddMonths(3);

                if (stockFaible || expirationProche)
                {
                    string alerte = "";
                    if (stockFaible && expirationProche) alerte = "Stock faible + Expiration proche";
                    else if (stockFaible) alerte = "Stock faible";
                    else alerte = "Expiration proche";

                    int rowIdx = dgv.Rows.Add(
                        m.Nom,
                        m.Quantite,
                        m.DateExpiration.HasValue ? m.DateExpiration.Value.ToShortDateString() : "N/A",
                        alerte
                    );

                    if (stockFaible && expirationProche)
                        dgv.Rows[rowIdx].DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                    else if (stockFaible)
                        dgv.Rows[rowIdx].DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                    else
                        dgv.Rows[rowIdx].DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 220);
                }
            }

            if (dgv.Rows.Count == 0)
            {
                dgv.Rows.Add("-", "-", "-", "Aucune alerte - tout est en ordre");
            }

            var btnClose = new Button();
            btnClose.Text = "Fermer";
            btnClose.Location = new Point(565, 385);
            btnClose.Size = new Size(105, 32);
            btnClose.BackColor = Color.FromArgb(127, 140, 141);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(panelTop);
            this.Controls.Add(dgv);
            this.Controls.Add(btnClose);
        }

        public static void CheckAndShow()
        {
            try
            {
                var dal = new MedicamentDAL();
                var meds = dal.GetAll();
                bool hasAlert = false;
                foreach (var m in meds)
                {
                    if (m.Quantite < 10 || (m.DateExpiration.HasValue && m.DateExpiration.Value < DateTime.Now.AddMonths(3)))
                    {
                        hasAlert = true;
                        break;
                    }
                }
                if (hasAlert)
                {
                    var form = new FormAlertesMedicaments();
                    form.Show();
                }
            }
            catch { }
        }
    }
}
