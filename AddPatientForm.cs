// ============================================================
//  AddPatientForm.cs  –  Dialog to add a new patient
// ============================================================

using System;
using System.Drawing;
using System.Windows.Forms;

namespace HospitalMedSystem
{
    /// <summary>
    /// Modal dialog that collects data for a new Patient.
    /// Returns the created Patient via the NewPatient property.
    /// </summary>
    public class AddPatientForm : Form
    {
        // ── Controls ─────────────────────────────────────────────
        private TextBox  txtPatientId, txtFirstName, txtLastName, txtAge, txtWard;
        private DateTimePicker dtpAdmissionDate;
        private Button   btnSave, btnCancel;
        private Label    lblTitle;

        /// <summary>The newly created patient (null if cancelled).</summary>
        public Patient NewPatient { get; private set; }

        // ── Constructor ──────────────────────────────────────────
        public AddPatientForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // ── Form properties ──────────────────────────────────
            this.Text            = "Add New Patient";
            this.Size            = new Size(420, 430);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = Color.White;

            // ── Title label ──────────────────────────────────────
            lblTitle = new Label
            {
                Text      = "Add New Patient",
                Font      = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 153),
                Location  = new Point(20, 15),
                Size      = new Size(360, 35)
            };

            // ── Helper: creates a Label + TextBox pair ────────────
            int yPos = 65;
            Label MakeLabel(string text)
            {
                var lbl = new Label
                {
                    Text     = text,
                    Font     = new Font("Segoe UI", 9.5f),
                    Location = new Point(25, yPos),
                    Size     = new Size(350, 20)
                };
                yPos += 22;
                return lbl;
            }
            TextBox MakeTextBox(string placeholder)
            {
                var tb = new TextBox
                {
                    Font        = new Font("Segoe UI", 10),
                    Location    = new Point(25, yPos),
                    Size        = new Size(360, 28),
                    PlaceholderText = placeholder
                };
                yPos += 38;
                return tb;
            }

            // ── Patient ID ───────────────────────────────────────
            var lblId = MakeLabel("Patient ID *");
            txtPatientId = MakeTextBox("e.g. P005");

            // ── First Name ───────────────────────────────────────
            var lblFirst = MakeLabel("First Name *");
            txtFirstName = MakeTextBox("First name");

            // ── Last Name ────────────────────────────────────────
            var lblLast = MakeLabel("Last Name *");
            txtLastName = MakeTextBox("Last name");

            // ── Age ──────────────────────────────────────────────
            var lblAge = MakeLabel("Age *");
            txtAge = MakeTextBox("e.g. 45");

            // ── Ward ─────────────────────────────────────────────
            var lblWard = MakeLabel("Ward *");
            txtWard = MakeTextBox("e.g. Cardiology");

            // ── Admission Date ───────────────────────────────────
            var lblDate = MakeLabel("Admission Date *");
            dtpAdmissionDate = new DateTimePicker
            {
                Font     = new Font("Segoe UI", 10),
                Location = new Point(25, yPos),
                Size     = new Size(360, 28),
                Format   = DateTimePickerFormat.Short,
                Value    = DateTime.Today
            };
            yPos += 45;

            // ── Buttons ──────────────────────────────────────────
            btnSave = new Button
            {
                Text      = "Save Patient",
                Location  = new Point(25, yPos),
                Size      = new Size(170, 38),
                BackColor = Color.FromArgb(0, 120, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text      = "Cancel",
                Location  = new Point(215, yPos),
                Size      = new Size(170, 38),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            // ── Resize form to fit all controls ──────────────────
            this.Size = new Size(420, yPos + 90);

            // ── Add controls to form ─────────────────────────────
            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                lblId,  txtPatientId,
                lblFirst, txtFirstName,
                lblLast,  txtLastName,
                lblAge,   txtAge,
                lblWard,  txtWard,
                lblDate,  dtpAdmissionDate,
                btnSave, btnCancel
            });
        }

        // ── Event handler: Save button ────────────────────────────
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // VALIDATION – throw descriptive errors for bad input
                if (string.IsNullOrWhiteSpace(txtPatientId.Text))
                    throw new ArgumentException("Patient ID is required.");

                if (!int.TryParse(txtAge.Text.Trim(), out int age))
                    throw new FormatException("Age must be a valid whole number.");

                // Create the Patient (property setters validate further)
                NewPatient = new Patient(
                    txtPatientId.Text.Trim(),
                    txtFirstName.Text.Trim(),
                    txtLastName.Text.Trim(),
                    age,
                    txtWard.Text.Trim(),
                    dtpAdmissionDate.Value.Date
                );

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                // EXCEPTION HANDLING – show user-friendly error
                MessageBox.Show(ex.Message, "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
