// ============================================================
//  MainForm.cs  –  Primary Windows Forms UI
// ============================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HospitalMedSystem
{
    /// <summary>
    /// The main application window.
    /// Layout (top → bottom):
    ///   ① Header bar (title + logged-in nurse info)
    ///   ② Search panel (TextBox + Search button)
    ///   ③ Patient info panel (shows selected patient details)
    ///   ④ Medications DataGridView
    ///   ⑤ Action buttons (Add Patient, Add Medication, Save, Load All)
    ///   ⑥ Status bar
    /// </summary>
    public class MainForm : Form
    {
        // ── The nurse using the system ────────────────────────────
        private readonly Nurse _loggedInNurse =
            new Nurse("N001", "Nour", "Ibrahim", 30, "General Medicine", "Day");

        // ── Currently selected patient ────────────────────────────
        private Patient _selectedPatient = null;

        // ── Controls ─────────────────────────────────────────────
        private Panel          pnlHeader, pnlSearch, pnlPatientInfo, pnlButtons;
        private Label          lblAppTitle, lblNurseInfo;
        private TextBox        txtSearch;
        private Button         btnSearch, btnClearSearch;
        private Button         btnAddPatient, btnAddMedication, btnSave, btnLoadAll;
        private DataGridView   dgvMedications;
        private ListBox        lstPatients;
        private Label          lblPatientDetail, lblMedTitle;
        private StatusStrip    statusBar;
        private ToolStripStatusLabel statusLabel;

        // ── Constructor ──────────────────────────────────────────
        public MainForm()
        {
            InitializeComponents();
            LoadDataOnStartup();
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║           UI CONSTRUCTION                           ║
        // ╚══════════════════════════════════════════════════════╝
        private void InitializeComponents()
        {
            // ── Form ─────────────────────────────────────────────
            this.Text            = "Hospital Medication Management System";
            this.Size            = new Size(1050, 720);
            this.MinimumSize     = new Size(900, 640);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.BackColor       = Color.FromArgb(245, 247, 250);
            this.Font            = new Font("Segoe UI", 9.5f);

            // ─────────────────────────────────────────────────────
            // ① HEADER
            // ─────────────────────────────────────────────────────
            pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 65,
                BackColor = Color.FromArgb(0, 102, 153)
            };

            lblAppTitle = new Label
            {
                Text      = "🏥  Hospital Medication Management System",
                Font      = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location  = new Point(15, 12),
                AutoSize  = true
            };

            lblNurseInfo = new Label
            {
                Text      = $"Nurse: {_loggedInNurse.FullName}  |  " +
                             $"Dept: {_loggedInNurse.Department}  |  " +
                             $"Shift: {_loggedInNurse.Shift}  |  " +
                             $"{DateTime.Now:dddd, dd MMMM yyyy}",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(200, 230, 255),
                Location  = new Point(17, 42),
                AutoSize  = true
            };

            pnlHeader.Controls.AddRange(new Control[] { lblAppTitle, lblNurseInfo });

            // ─────────────────────────────────────────────────────
            // ② SEARCH PANEL
            // ─────────────────────────────────────────────────────
            pnlSearch = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 60,
                BackColor = Color.White,
                Padding   = new Padding(10, 0, 10, 0)
            };

            // Draw a bottom border on the search panel
            pnlSearch.Paint += (s, e) =>
            {
                e.Graphics.DrawLine(
                    new Pen(Color.FromArgb(220, 220, 220)),
                    0, pnlSearch.Height - 1,
                    pnlSearch.Width, pnlSearch.Height - 1);
            };

            var lblSearchHint = new Label
            {
                Text     = "Search Patient by Name:",
                Font     = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(15, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(60, 60, 80)
            };

            txtSearch = new TextBox
            {
                PlaceholderText = "Enter patient name...",
                Font     = new Font("Segoe UI", 11),
                Location = new Point(210, 15),
                Size     = new Size(280, 30)
            };
            // Allow pressing Enter to search
            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) BtnSearch_Click(s, e);
            };

            btnSearch = MakeButton("🔍  Search", Color.FromArgb(0, 120, 180), 500, 14, 120, 32);
            btnSearch.Click += BtnSearch_Click;

            btnClearSearch = MakeButton("✖  Clear", Color.FromArgb(108, 117, 125), 630, 14, 100, 32);
            btnClearSearch.Click += BtnClearSearch_Click;

            pnlSearch.Controls.AddRange(new Control[]
                { lblSearchHint, txtSearch, btnSearch, btnClearSearch });

            // ─────────────────────────────────────────────────────
            // ③ SPLIT LAYOUT  (left = patient list, right = meds)
            // ─────────────────────────────────────────────────────
            var splitContainer = new SplitContainer
            {
                Dock             = DockStyle.Fill,
                SplitterDistance = 300,
                BackColor        = Color.FromArgb(240, 242, 245)
            };

            // ── LEFT: Patient list ────────────────────────────────
            var pnlLeft = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            var lblPatientList = new Label
            {
                Text      = "Patients",
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 153),
                Dock      = DockStyle.Top,
                Height    = 36,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(10, 0, 0, 0),
                BackColor = Color.FromArgb(236, 245, 255)
            };

            lstPatients = new ListBox
            {
                Dock           = DockStyle.Fill,
                Font           = new Font("Segoe UI", 10),
                BorderStyle    = BorderStyle.None,
                BackColor      = Color.White,
                ItemHeight     = 28,
                DrawMode       = DrawMode.OwnerDrawFixed
            };
            // Custom draw for alternating row colours
            lstPatients.DrawItem += LstPatients_DrawItem;
            lstPatients.SelectedIndexChanged += LstPatients_SelectedIndexChanged;

            pnlLeft.Controls.Add(lstPatients);
            pnlLeft.Controls.Add(lblPatientList);

            // ── RIGHT: Patient detail + medications grid ───────────
            var pnlRight = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            // Patient info strip
            pnlPatientInfo = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 70,
                BackColor = Color.FromArgb(236, 245, 255),
                Padding   = new Padding(10, 5, 10, 5)
            };

            lblPatientDetail = new Label
            {
                Dock      = DockStyle.Fill,
                Font      = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(40, 40, 80),
                TextAlign = ContentAlignment.MiddleLeft,
                Text      = "← Select a patient from the list to view their medications"
            };
            pnlPatientInfo.Controls.Add(lblPatientDetail);

            // Medications title
            lblMedTitle = new Label
            {
                Text      = "Medications",
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 153),
                Dock      = DockStyle.Top,
                Height    = 36,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(10, 0, 0, 0),
                BackColor = Color.FromArgb(236, 245, 255)
            };

            // ── DataGridView for medications ──────────────────────
            dgvMedications = new DataGridView
            {
                Dock                = DockStyle.Fill,
                ReadOnly            = true,
                AllowUserToAddRows  = false,
                AllowUserToDeleteRows = false,
                SelectionMode       = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect         = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle         = BorderStyle.None,
                GridColor           = Color.FromArgb(220, 225, 235),
                BackgroundColor     = Color.White,
                RowHeadersVisible   = false,
                Font                = new Font("Segoe UI", 10),
                RowTemplate         = { Height = 30 }
            };

            // Style the column headers
            dgvMedications.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(0, 102, 153),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding   = new Padding(5, 0, 0, 0)
            };
            dgvMedications.ColumnHeadersHeight = 36;
            dgvMedications.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Alternating row colours
            dgvMedications.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(245, 248, 255);
            dgvMedications.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(0, 150, 200);
            dgvMedications.DefaultCellStyle.Padding = new Padding(5, 0, 0, 0);

            SetupGridColumns();

            pnlRight.Controls.Add(dgvMedications);
            pnlRight.Controls.Add(lblMedTitle);
            pnlRight.Controls.Add(pnlPatientInfo);

            splitContainer.Panel1.Controls.Add(pnlLeft);
            splitContainer.Panel2.Controls.Add(pnlRight);

            // ─────────────────────────────────────────────────────
            // ⑤ BOTTOM BUTTON BAR
            // ─────────────────────────────────────────────────────
            pnlButtons = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 58,
                BackColor = Color.FromArgb(250, 250, 252),
                Padding   = new Padding(10, 10, 10, 10)
            };
            pnlButtons.Paint += (s, e) =>
            {
                e.Graphics.DrawLine(
                    new Pen(Color.FromArgb(210, 215, 220)),
                    0, 0, pnlButtons.Width, 0);
            };

            btnAddPatient = MakeButton("➕ Add Patient",
                Color.FromArgb(0, 120, 180), 10, 10, 140, 36);
            btnAddPatient.Click += BtnAddPatient_Click;

            btnAddMedication = MakeButton("💊 Add Medication",
                Color.FromArgb(40, 167, 69), 160, 10, 155, 36);
            btnAddMedication.Click += BtnAddMedication_Click;
            btnAddMedication.Enabled = false;   // enabled only when patient selected

            btnSave = MakeButton("💾 Save Data",
                Color.FromArgb(255, 140, 0), 325, 10, 130, 36);
            btnSave.Click += BtnSave_Click;

            btnLoadAll = MakeButton("🔄 Show All",
                Color.FromArgb(108, 117, 125), 465, 10, 120, 36);
            btnLoadAll.Click += BtnLoadAll_Click;

            pnlButtons.Controls.AddRange(new Control[]
                { btnAddPatient, btnAddMedication, btnSave, btnLoadAll });

            // ─────────────────────────────────────────────────────
            // ⑥ STATUS BAR
            // ─────────────────────────────────────────────────────
            statusBar   = new StatusStrip { BackColor = Color.FromArgb(0, 80, 120) };
            statusLabel = new ToolStripStatusLabel
            {
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9f),
                Text      = "Ready"
            };
            statusBar.Items.Add(statusLabel);

            // ─────────────────────────────────────────────────────
            // ADD ALL TO FORM (order matters for DockStyle)
            // ─────────────────────────────────────────────────────
            this.Controls.Add(splitContainer);   // Fill – must be added before Top/Bottom
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlButtons);
            this.Controls.Add(statusBar);
        }

        // ── Define DataGridView columns ───────────────────────────
        private void SetupGridColumns()
        {
            dgvMedications.Columns.Clear();
            dgvMedications.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDrug",  HeaderText = "💊  Drug Name",     FillWeight = 25
            });
            dgvMedications.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDosage", HeaderText = "📏  Dosage",        FillWeight = 15
            });
            dgvMedications.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colType",   HeaderText = "💉  Type",          FillWeight = 15
            });
            dgvMedications.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colTime",   HeaderText = "🕐  Time",          FillWeight = 18
            });
            dgvMedications.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNotes",  HeaderText = "📝  Notes",         FillWeight = 27
            });
        }

        // ── Button factory helper ─────────────────────────────────
        private Button MakeButton(string text, Color color,
                                  int x, int y, int w, int h)
        {
            var btn = new Button
            {
                Text      = text,
                Location  = new Point(x, y),
                Size      = new Size(w, h),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║           DATA LOADING                              ║
        // ╚══════════════════════════════════════════════════════╝

        private void LoadDataOnStartup()
        {
            try
            {
                HospitalDataService.LoadFromFile();

                // First run: no data file → load sample data
                if (HospitalDataService.PatientCount == 0)
                {
                    HospitalDataService.LoadSampleData();
                    SetStatus("Sample data loaded. Use 'Save Data' to persist your changes.");
                }
                else
                {
                    SetStatus($"Data loaded: {HospitalDataService.PatientCount} patient(s).");
                }

                RefreshPatientList(HospitalDataService.Patients);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load data file:\n" + ex.Message,
                    "Load Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                HospitalDataService.LoadSampleData();
                RefreshPatientList(HospitalDataService.Patients);
            }
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║           UI REFRESH HELPERS                        ║
        // ╚══════════════════════════════════════════════════════╝

        /// <summary>Fills the patient ListBox with a given collection.</summary>
        private void RefreshPatientList(IEnumerable<Patient> patients)
        {
            lstPatients.Items.Clear();
            foreach (var p in patients)
                lstPatients.Items.Add(p);   // uses Patient.ToString()

            SetStatus($"Showing {lstPatients.Items.Count} patient(s).");
        }

        /// <summary>Fills the DataGridView with the selected patient's medications.</summary>
        private void RefreshMedicationGrid(Patient patient)
        {
            dgvMedications.Rows.Clear();

            if (patient == null) return;

            foreach (Medication med in patient.Medications)
            {
                dgvMedications.Rows.Add(
                    med.DrugName,
                    med.Dosage,
                    med.Type.ToString(),
                    med.TimeOfAdministration.ToString(),
                    med.Notes
                );
            }
        }

        private void SetStatus(string message)
        {
            statusLabel.Text = $"[{DateTime.Now:HH:mm:ss}]  {message}";
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║           EVENT HANDLERS                            ║
        // ╚══════════════════════════════════════════════════════╝

        // ── Search ────────────────────────────────────────────────
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string term = txtSearch.Text.Trim();

                // EXCEPTION HANDLING: validate empty search
                if (string.IsNullOrWhiteSpace(term))
                    throw new ArgumentException("Please enter a name to search.");

                List<Patient> results = HospitalDataService.SearchByName(term);

                if (results.Count == 0)
                {
                    MessageBox.Show($"No patients found matching \"{term}\".",
                        "No Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                RefreshPatientList(results);
                SetStatus($"Search: '{term}' → {results.Count} result(s) found.");

                // Auto-select the first result
                if (lstPatients.Items.Count > 0)
                    lstPatients.SelectedIndex = 0;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Search Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            _selectedPatient = null;
            lblPatientDetail.Text = "← Select a patient from the list to view their medications";
            dgvMedications.Rows.Clear();
            btnAddMedication.Enabled = false;
            RefreshPatientList(HospitalDataService.Patients);
            SetStatus("Search cleared. Showing all patients.");
        }

        // ── Patient list selection ────────────────────────────────
        private void LstPatients_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPatients.SelectedItem is Patient patient)
            {
                _selectedPatient = patient;
                lblPatientDetail.Text = patient.GetDisplayInfo();
                RefreshMedicationGrid(patient);
                btnAddMedication.Enabled = true;
                SetStatus($"Selected: {patient.FullName}  |  " +
                           $"{patient.Medications.Count} medication(s).");
            }
        }

        // ── Add Patient ───────────────────────────────────────────
        private void BtnAddPatient_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dlg = new AddPatientForm())
                {
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        HospitalDataService.AddPatient(dlg.NewPatient);
                        RefreshPatientList(HospitalDataService.Patients);
                        SetStatus($"Patient '{dlg.NewPatient.FullName}' added successfully.");

                        // Select the newly added patient
                        lstPatients.SelectedItem = dlg.NewPatient;
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                // Duplicate ID
                MessageBox.Show(ex.Message, "Duplicate Patient",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Add Medication ────────────────────────────────────────
        private void BtnAddMedication_Click(object sender, EventArgs e)
        {
            if (_selectedPatient == null)
            {
                MessageBox.Show("Please select a patient first.",
                    "No Patient Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var dlg = new AddMedicationForm(_selectedPatient.FullName))
                {
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        _selectedPatient.AddMedication(dlg.NewMedication);
                        RefreshMedicationGrid(_selectedPatient);
                        SetStatus($"Medication '{dlg.NewMedication.DrugName}' " +
                                  $"added to {_selectedPatient.FullName}.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not add medication:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Save ──────────────────────────────────────────────────
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                HospitalDataService.SaveToFile();
                SetStatus($"All data saved successfully at {DateTime.Now:HH:mm:ss}.");
                MessageBox.Show("Data saved successfully!", "Saved",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save data:\n" + ex.Message,
                    "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Load All (refresh) ────────────────────────────────────
        private void BtnLoadAll_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            _selectedPatient = null;
            dgvMedications.Rows.Clear();
            btnAddMedication.Enabled = false;
            lblPatientDetail.Text =
                "← Select a patient from the list to view their medications";
            RefreshPatientList(HospitalDataService.Patients);
            SetStatus("Showing all patients.");
        }

        // ── Custom patient list row drawing ───────────────────────
        private void LstPatients_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            // Alternating row colour
            Color bg = (e.Index % 2 == 0)
                       ? Color.White
                       : Color.FromArgb(240, 246, 255);

            // Selected row colour
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                bg = Color.FromArgb(0, 140, 200);

            e.Graphics.FillRectangle(new SolidBrush(bg), e.Bounds);

            Color fg = ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                       ? Color.White : Color.FromArgb(30, 30, 60);

            string text = lstPatients.Items[e.Index]?.ToString() ?? "";
            e.Graphics.DrawString(text,
                new Font("Segoe UI", 10), new SolidBrush(fg),
                e.Bounds.X + 8, e.Bounds.Y + 5);
        }

        // ── Save on close ─────────────────────────────────────────
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
                "Save changes before exiting?",
                "Exit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try { HospitalDataService.SaveToFile(); }
                catch { /* ignore on exit */ }
            }
            else if (result == DialogResult.Cancel)
            {
                e.Cancel = true;   // user changed their mind
            }

            base.OnFormClosing(e);
        }
    }
}
