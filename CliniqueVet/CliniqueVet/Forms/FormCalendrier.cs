using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using CliniqueVet.DAL;
using CliniqueVet.Models;

namespace CliniqueVet.Forms
{
    public class FormCalendrier : Form
    {
        private MonthCalendar calendar;
        private ListBox lstRdv;
        private Label lblJour;
        private RDVDAL dal = new RDVDAL();
        private List<RDV> _allRdv = new List<RDV>();

        public FormCalendrier()
        {
            this.Text = "Calendrier des Rendez-vous";
            this.Size = new Size(800, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 247, 250);
            InitUI();
            LoadAll();
        }

        private void InitUI()
        {
            var panelTop = new Panel();
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 55;
            panelTop.BackColor = Color.FromArgb(41, 128, 185);

            var lblTitle = new Label();
            lblTitle.Text = "Vue Calendrier - Rendez-vous";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(15, 14);
            panelTop.Controls.Add(lblTitle);

            // Left: calendar
            var panelLeft = new Panel();
            panelLeft.Location = new Point(15, 70);
            panelLeft.Size = new Size(320, 440);
            panelLeft.BackColor = Color.White;

            calendar = new MonthCalendar();
            calendar.Location = new Point(5, 5);
            calendar.MaxSelectionCount = 1;
            calendar.Font = new Font("Segoe UI", 9);
            calendar.DateChanged += Calendar_DateChanged;
            panelLeft.Controls.Add(calendar);

            // Right: list of RDV for selected day
            var panelRight = new Panel();
            panelRight.Location = new Point(350, 70);
            panelRight.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelRight.Size = new Size(420, 440);
            panelRight.BackColor = Color.White;

            lblJour = new Label();
            lblJour.Text = "Rendez-vous du " + DateTime.Today.ToLongDateString();
            lblJour.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblJour.ForeColor = Color.FromArgb(41, 128, 185);
            lblJour.AutoSize = true;
            lblJour.Location = new Point(10, 10);

            lstRdv = new ListBox();
            lstRdv.Location = new Point(10, 40);
            lstRdv.Size = new Size(400, 380);
            lstRdv.Font = new Font("Segoe UI", 9);
            lstRdv.BorderStyle = BorderStyle.None;
            lstRdv.BackColor = Color.White;
            lstRdv.ItemHeight = 28;

            panelRight.Controls.Add(lblJour);
            panelRight.Controls.Add(lstRdv);

            this.Controls.Add(panelTop);
            this.Controls.Add(panelLeft);
            this.Controls.Add(panelRight);
        }

        private void LoadAll()
        {
            _allRdv = dal.GetAll();
            ShowRdvForDate(DateTime.Today);

            // Bold dates that have RDV
            var boldDates = new List<DateTime>();
            foreach (var r in _allRdv)
                if (!boldDates.Contains(r.DateRdv.Date))
                    boldDates.Add(r.DateRdv.Date);
            if (boldDates.Count > 0)
                calendar.BoldedDates = boldDates.ToArray();
        }

        private void ShowRdvForDate(DateTime date)
        {
            lstRdv.Items.Clear();
            lblJour.Text = "Rendez-vous du " + date.ToString("dddd dd MMMM yyyy",
                new System.Globalization.CultureInfo("fr-FR"));

            bool found = false;
            foreach (var r in _allRdv)
            {
                if (r.DateRdv.Date == date.Date)
                {
                    lstRdv.Items.Add(string.Format("  {0}   {1} ({2})   {3}",
                        r.HeureRdv.ToString(@"hh\:mm"),
                        r.NomAnimal,
                        r.NomProprietaire,
                        string.IsNullOrWhiteSpace(r.Motif) ? "" : "- " + r.Motif));
                    found = true;
                }
            }

            if (!found)
                lstRdv.Items.Add("  Aucun rendez-vous ce jour");
        }

        private void Calendar_DateChanged(object sender, DateRangeEventArgs e)
        {
            ShowRdvForDate(e.Start);
        }
    }
}
