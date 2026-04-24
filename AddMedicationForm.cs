// ============================================================
//  AddMedicationForm.cs  –  Dialog to add a medication to a patient
// ============================================================

using System;
using System.Drawing;
using System.Windows.Forms;

namespace HospitalMedSystem
{
    /// <summary>
    /// Modal dialog that collects data for a new Medication.
    /// The selected patient's name is shown for context.
    /// </summary>
    public class AddMedicationForm : Form
    {
        // ── Controls ─────────────────────────────────────────────
        private TextBox   txtDrugName, txtDosage, txtNotes;
        private ComboBox  cboDrugType, cboAdminTime;
        private Button    btnSave, btnCancel;
        private Label     lblTitle;

        /// <summary>The newly created medication (null if cancelled).</summary>
        public Medication NewMedication { get; private set; }

        // ── Constructor ──────────────────────────────────────────
        public AddMedicationForm(string patientName)
        {
            InitializeComponents(patientName);
        }

        private void InitializeComponents(string patientName)
        {
            // ── Form properties ──────────────────────────────────
            this.Text            = "Add Medication";
            this.Size            = new Size(430, 430);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = Color.White;

            // ── Title ────────────────────────────────────────────
            lblTitle = new Label
            {
                Text      = $"Add Medication  ►  {patientName}",
                Font      = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 153),
                Location  = new Point(20, 15),
                Size      = new Size(380, 35)
            };

            // ── Layout helpers ───────────────────────────────────
            int yPos = 60;
            Label MakeLabel(string text)
            {
                var lbl = new Label
                {
                    Text     = text,
                    Font     = new Font("Segoe UI", 9.5f),
                    Location = new Point(25, yPos),
                    Size     = new Size(360, 20)
                };
                yPos += 22;
                return lbl;
            }
            TextBox MakeTextBox(string placeholder)
            {
                var tb = new TextBox
                {
                    Font            = new Font("Segoe UI", 10),
                    Location        = new Point(25, yPos),
                    Size            = new Size(370, 28),
                    PlaceholderText = placeholder
                };
                yPos += 38;
                return tb;
            }
            ComboBox MakeComboBox()
            {
                var cb = new ComboBox
                {
                    Font         = new Font("Segoe UI", 10),
                    Location     = new Point(25, yPos),
                    Size         = new Size(370, 28),
                    DropDownStyle= ComboBoxStyle.DropDownList
                };
                yPos += 38;
                return cb;
            }

            // ── Drug Name ────────────────────────────────────────
            var lblDrug = MakeLabel("Drug Name *");
            txtDrugName = MakeTextBox("e.g. Aspirin");

            // ── Dosage ───────────────────────────────────────────
            var lblDosage = MakeLabel("Dosage *");
            txtDosage = MakeTextBox("e.g. 100 mg");

            // ── Drug Type (enum ComboBox) ─────────────────────────
            var lblType = MakeLabel("Drug Type *");
            cboDrugType = MakeComboBox();
            // Populate from the DrugType enum
            foreach (DrugType dt in Enum.GetValues(typeof(DrugType)))
                cboDrugType.Items.Add(dt);
            cboDrugType.SelectedIndex = 0;

            // ── Administration Time (enum ComboBox) ───────────────
            var lblTime = MakeLabel("Administration Time *");
            cboAdminTime = MakeComboBox();
            foreach (AdministrationTime at in Enum.GetValues(typeof(AdministrationTime)))
                cboAdminTime.Items.Add(at);
            cboAdminTime.SelectedIndex = 0;

            // ── Notes ────────────────────────────────────────────
            var lblNotes = MakeLabel("Notes (optional)");
            txtNotes = MakeTextBox("e.g. Take with food");

            // ── Buttons ──────────────────────────────────────────
            btnSave = new Button
            {
                Text      = "Add Medication",
                Location  = new Point(25, yPos),
                Size      = new Size(175, 38),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text      = "Cancel",
                Location  = new Point(220, yPos),
                Size      = new Size(175, 38),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            this.Size = new Size(430, yPos + 90);

            // ── Add all controls ─────────────────────────────────
            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                lblDrug,  txtDrugName,
                lblDosage, txtDosage,
                lblType,  cboDrugType,
                lblTime,  cboAdminTime,
                lblNotes, txtNotes,
                btnSave, btnCancel
            });
        }

        // ── Save button handler ───────────────────────────────────
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDrugName.Text))
                    throw new ArgumentException("Drug name is required.");
                if (string.IsNullOrWhiteSpace(txtDosage.Text))
                    throw new ArgumentException("Dosage is required.");

                var drugType  = (DrugType)cboDrugType.SelectedItem;
                var adminTime = (AdministrationTime)cboAdminTime.SelectedItem;

                NewMedication = new Medication(
                    txtDrugName.Text.Trim(),
                    txtDosage.Text.Trim(),
                    drugType,
                    adminTime,
                    txtNotes.Text.Trim()
                );

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
