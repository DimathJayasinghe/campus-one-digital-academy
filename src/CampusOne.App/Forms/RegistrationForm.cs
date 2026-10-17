using CampusOne.Core.Interfaces;
using CampusOne.Core.Models;

namespace CampusOne.App.Forms;

/// <summary>
/// Registration Form – allows the admin to Create, Read, Update, and Delete student records.
/// </summary>
public class RegistrationForm : Form
{
    /// <summary>Raised when the user clicks the Logout link.</summary>
    public event Action? LogoutRequested;

    // ── Controls ─────────────────────────────────────────────────────────────
    private GroupBox        grpBasic      = null!;
    private GroupBox        grpContact    = null!;
    private GroupBox        grpParent     = null!;
    private Label           lblTitle      = null!;
    private ComboBox        cmbRegNo      = null!;
    private TextBox         txtFirstName  = null!;
    private TextBox         txtLastName   = null!;
    private DateTimePicker  dtpDob        = null!;
    private RadioButton     rdMale        = null!;
    private RadioButton     rdFemale      = null!;
    private TextBox         txtAddress    = null!;
    private TextBox         txtEmail      = null!;
    private TextBox         txtMobile     = null!;
    private TextBox         txtHome       = null!;
    private TextBox         txtParentName = null!;
    private TextBox         txtNic        = null!;
    private TextBox         txtContactNo  = null!;
    private Button          btnRegister   = null!;
    private Button          btnUpdate     = null!;
    private Button          btnDelete     = null!;
    private Button          btnClear      = null!;
    private LinkLabel       llLogout      = null!;
    private LinkLabel       llExit        = null!;

    // ── Dependency ────────────────────────────────────────────────────────────
    private readonly IStudentRepository _studentRepo;

    public RegistrationForm(IStudentRepository studentRepo)
    {
        _studentRepo = studentRepo;
        InitializeComponent();
        LoadRegNos();
    }

    // ── Designer-equivalent initialisation ───────────────────────────────────
    private void InitializeComponent()
    {
        Text            = "Student Registration – Campus One Digital Academy";
        Size            = new Size(680, 600);
        StartPosition   = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        BackColor       = Color.WhiteSmoke;
        Font            = new Font("Times New Roman", 11f);

        lblTitle = new Label
        {
            Text      = "Campus One Digital Academy",
            Font      = new Font("Times New Roman", 16f, FontStyle.Bold),
            Location  = new Point(100, 10),
            Size      = new Size(480, 34),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // ── Basic Details ─────────────────────────────────────────────────────
        grpBasic = new GroupBox { Text = "Basic Details", Location = new Point(20, 55), Size = new Size(630, 200) };

        cmbRegNo      = new ComboBox      { Location = new Point(150, 22),  Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDown };
        txtFirstName  = new TextBox       { Location = new Point(150, 52),  Size = new Size(440, 25) };
        txtLastName   = new TextBox       { Location = new Point(150, 82),  Size = new Size(440, 25) };
        dtpDob        = new DateTimePicker { Location = new Point(150, 112), Size = new Size(200, 25), Format = DateTimePickerFormat.Short };
        rdMale        = new RadioButton   { Text = "Male",   Location = new Point(150, 142), Checked = true };
        rdFemale      = new RadioButton   { Text = "Female", Location = new Point(220, 142) };
        txtAddress    = new TextBox       { Location = new Point(150, 170), Size = new Size(440, 25) };

        grpBasic.Controls.AddRange(new Control[]
        {
            MakeLabel("Reg No",       10, 25), MakeLabel("First Name", 10, 55),
            MakeLabel("Last Name",    10, 85), MakeLabel("Date of Birth", 10, 115),
            MakeLabel("Gender",       10, 145), MakeLabel("Address",   10, 175),
            cmbRegNo, txtFirstName, txtLastName, dtpDob, rdMale, rdFemale, txtAddress
        });

        // ── Contact Details ───────────────────────────────────────────────────
        grpContact = new GroupBox { Text = "Contact Details", Location = new Point(20, 265), Size = new Size(630, 110) };

        txtEmail  = new TextBox { Location = new Point(150, 22), Size = new Size(440, 25) };
        txtMobile = new TextBox { Location = new Point(150, 52), Size = new Size(200, 25) };
        txtHome   = new TextBox { Location = new Point(150, 82), Size = new Size(200, 25) };

        grpContact.Controls.AddRange(new Control[]
        {
            MakeLabel("Email", 10, 25), MakeLabel("Mobile Phone", 10, 55), MakeLabel("Home Phone", 10, 85),
            txtEmail, txtMobile, txtHome
        });

        // ── Parent Details ────────────────────────────────────────────────────
        grpParent = new GroupBox { Text = "Parent Details", Location = new Point(20, 385), Size = new Size(630, 110) };

        txtParentName = new TextBox { Location = new Point(150, 22), Size = new Size(440, 25) };
        txtNic        = new TextBox { Location = new Point(150, 52), Size = new Size(200, 25) };
        txtContactNo  = new TextBox { Location = new Point(150, 82), Size = new Size(200, 25) };

        grpParent.Controls.AddRange(new Control[]
        {
            MakeLabel("Parent Name", 10, 25), MakeLabel("NIC", 10, 55), MakeLabel("Contact No", 10, 85),
            txtParentName, txtNic, txtContactNo
        });

        // ── Buttons & Links ───────────────────────────────────────────────────
        btnRegister = MakeButton("Register", 20,  505, Color.SteelBlue);
        btnUpdate   = MakeButton("Update",   145, 505, Color.DarkSeaGreen);
        btnDelete   = MakeButton("Delete",   270, 505, Color.IndianRed);
        btnClear    = MakeButton("Clear",    395, 505, Color.Gray);

        llLogout = new LinkLabel { Text = "Logout", Location = new Point(530, 512), Size = new Size(60, 25) };
        llExit   = new LinkLabel { Text = "Exit",   Location = new Point(610, 512), Size = new Size(40, 25) };

        Controls.AddRange(new Control[] { lblTitle, grpBasic, grpContact, grpParent,
            btnRegister, btnUpdate, btnDelete, btnClear, llLogout, llExit });

        // ── Wire events ───────────────────────────────────────────────────────
        cmbRegNo.SelectedIndexChanged += CmbRegNo_SelectedIndexChanged;
        btnRegister.Click += BtnRegister_Click;
        btnUpdate.Click   += BtnUpdate_Click;
        btnDelete.Click   += BtnDelete_Click;
        btnClear.Click    += (_, _) => ClearForm();
        llLogout.LinkClicked += (_, _) => LogoutRequested?.Invoke();
        llExit.LinkClicked   += (_, _) => ConfirmExit();
        FormClosing += (_, e) => { e.Cancel = true; ConfirmExit(); };
    }

    // ── Data helpers ─────────────────────────────────────────────────────────

    private void LoadRegNos()
    {
        cmbRegNo.Items.Clear();
        foreach (var n in _studentRepo.GetAllRegNos())
            cmbRegNo.Items.Add(n);
    }

    private void FillForm(Student s)
    {
        cmbRegNo.Text      = s.RegNo.ToString();
        txtFirstName.Text  = s.FirstName;
        txtLastName.Text   = s.LastName;
        dtpDob.Value       = s.DateOfBirth;
        rdMale.Checked     = s.Gender == "Male";
        rdFemale.Checked   = s.Gender == "Female";
        txtAddress.Text    = s.Address;
        txtEmail.Text      = s.Email;
        txtMobile.Text     = s.MobilePhone.ToString();
        txtHome.Text       = s.HomePhone.ToString();
        txtParentName.Text = s.ParentName;
        txtNic.Text        = s.Nic;
        txtContactNo.Text  = s.ContactNo.ToString();
    }

    private Student ReadForm() => new()
    {
        RegNo       = int.Parse(cmbRegNo.Text.Trim()),
        FirstName   = txtFirstName.Text.Trim(),
        LastName    = txtLastName.Text.Trim(),
        DateOfBirth = dtpDob.Value,
        Gender      = rdMale.Checked ? "Male" : "Female",
        Address     = txtAddress.Text.Trim(),
        Email       = txtEmail.Text.Trim(),
        MobilePhone = long.Parse(txtMobile.Text.Trim()),
        HomePhone   = long.TryParse(txtHome.Text.Trim(), out var hp) ? hp : 0,
        ParentName  = txtParentName.Text.Trim(),
        Nic         = txtNic.Text.Trim(),
        ContactNo   = long.TryParse(txtContactNo.Text.Trim(), out var cn) ? cn : 0
    };

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(cmbRegNo.Text) ||
            string.IsNullOrWhiteSpace(txtFirstName.Text) ||
            string.IsNullOrWhiteSpace(txtLastName.Text) ||
            string.IsNullOrWhiteSpace(txtAddress.Text) ||
            string.IsNullOrWhiteSpace(txtEmail.Text) ||
            string.IsNullOrWhiteSpace(txtMobile.Text))
        {
            MessageBox.Show("Please fill in all required fields.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    private void ClearForm()
    {
        cmbRegNo.Text      = string.Empty;
        txtFirstName.Clear(); txtLastName.Clear();
        dtpDob.Value       = DateTime.Today;
        rdMale.Checked     = true;
        txtAddress.Clear(); txtEmail.Clear(); txtMobile.Clear(); txtHome.Clear();
        txtParentName.Clear(); txtNic.Clear(); txtContactNo.Clear();
    }

    // ── Event handlers ────────────────────────────────────────────────────────

    private void CmbRegNo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cmbRegNo.SelectedItem is int regNo)
        {
            var student = _studentRepo.GetByRegNo(regNo);
            if (student != null) FillForm(student);
        }
    }

    private void BtnRegister_Click(object? sender, EventArgs e)
    {
        if (!ValidateForm()) return;
        try
        {
            _studentRepo.Add(ReadForm());
            MessageBox.Show("Student registered successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadRegNos();
            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Registration Failed",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnUpdate_Click(object? sender, EventArgs e)
    {
        if (!ValidateForm()) return;
        try
        {
            _studentRepo.Update(ReadForm());
            MessageBox.Show("Record updated successfully!", "Update",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Update Failed",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(cmbRegNo.Text)) return;

        var confirm = MessageBox.Show(
            "Are you sure you want to delete this record?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes) return;

        try
        {
            _studentRepo.Delete(int.Parse(cmbRegNo.Text.Trim()));
            MessageBox.Show("Record deleted successfully.", "Deleted",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadRegNos();
            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Delete Failed",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void ConfirmExit()
    {
        var r = MessageBox.Show("Are you sure you want to exit?", "Exit",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (r == DialogResult.Yes) Application.Exit();
    }

    // ── Control factory helpers ────────────────────────────────────────────────

    private static Label MakeLabel(string text, int x, int y) => new()
        { Text = text, Location = new Point(x, y), Size = new Size(135, 25), TextAlign = ContentAlignment.MiddleLeft };

    private static Button MakeButton(string text, int x, int y, Color back) => new()
        { Text = text, Location = new Point(x, y), Size = new Size(110, 32), BackColor = back, ForeColor = Color.White };
}
