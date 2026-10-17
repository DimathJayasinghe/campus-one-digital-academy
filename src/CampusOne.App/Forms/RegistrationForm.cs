using CampusOne.Core.Interfaces;
using CampusOne.Core.Models;
using CampusOne.Data;
using CampusOne.Data.Repositories;
using System.Text.RegularExpressions;

namespace CampusOne.App.Forms;

/// <summary>
/// Registration Form – Allows the administrator to Create, Read, Update, and Delete student records.
/// Fully conforms to Figure II and project specification guidelines.
/// </summary>
public class RegistrationForm : Form
{
    /// <summary>Raised when the user clicks the Logout link.</summary>
    public event Action? LogoutRequested;

    // ── Four Group Boxes (per specification) ──────────────────────────────────
    private GroupBox grpStudentRegistration = null!; // Outer container
    private GroupBox grpBasic               = null!;
    private GroupBox grpContact             = null!;
    private GroupBox grpParent              = null!;

    // ── Thirteen Required Labels ─────────────────────────────────────────────
    private Label lblTitle      = null!; // Campus One Digital Academy
    private Label lblRegNo      = null!;
    private Label lblFirstName  = null!;
    private Label lblLastName   = null!;
    private Label lblDob        = null!;
    private Label lblGender     = null!;
    private Label lblAddress    = null!;
    private Label lblEmail      = null!;
    private Label lblMobile     = null!;
    private Label lblHome       = null!;
    private Label lblParentName = null!;
    private Label lblNic        = null!;
    private Label lblContactNo  = null!;

    // ── Nine Required Text Boxes ─────────────────────────────────────────────
    private TextBox txtFirstName  = null!;
    private TextBox txtLastName   = null!;
    private TextBox txtAddress    = null!; // Multiline per brief
    private TextBox txtEmail      = null!;
    private TextBox txtMobile     = null!;
    private TextBox txtHome       = null!;
    private TextBox txtParentName = null!;
    private TextBox txtNic        = null!;
    private TextBox txtContactNo  = null!;

    // ── Other Controls (per specification) ───────────────────────────────────
    private ComboBox       cmbRegNo    = null!;
    private DateTimePicker dtpDob      = null!;
    private RadioButton    rdMale      = null!;
    private RadioButton    rdFemale    = null!;
    private Button         btnRegister = null!;
    private Button         btnUpdate   = null!;
    private Button         btnClear    = null!;
    private Button         btnDelete   = null!;
    private LinkLabel      llLogout    = null!;
    private LinkLabel      llExit      = null!;

    // ── Value Additions (Extra Marks) ────────────────────────────────────────
    private LinkLabel llViewDirectory = null!;
    private LinkLabel llManageUsers   = null!;

    // ── Dependencies ─────────────────────────────────────────────────────────
    private readonly IStudentRepository _studentRepo;

    public RegistrationForm(IStudentRepository studentRepo)
    {
        _studentRepo = studentRepo;
        InitializeComponent();
        LoadRegNos();
    }

    private void InitializeComponent()
    {
        Text            = "Student Registration – Campus One Digital Academy";
        Size            = new Size(720, 680);
        StartPosition   = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        BackColor       = Color.WhiteSmoke;
        Font            = new Font("Times New Roman", 11f);

        // Header Title Label (Label 1)
        lblTitle = new Label
        {
            Text      = "Campus One Digital Academy",
            Font      = new Font("Times New Roman", 18f, FontStyle.Bold),
            Location  = new Point(20, 10),
            Size      = new Size(660, 35),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Navigation links on top
        llLogout = new LinkLabel
        {
            Text      = "Logout",
            Location  = new Point(550, 48),
            Size      = new Size(60, 20),
            TextAlign = ContentAlignment.MiddleRight
        };

        llExit = new LinkLabel
        {
            Text      = "Exit",
            Location  = new Point(620, 48),
            Size      = new Size(50, 20),
            TextAlign = ContentAlignment.MiddleRight
        };

        llViewDirectory = new LinkLabel
        {
            Text      = "📁 View Directory",
            Location  = new Point(30, 48),
            Size      = new Size(130, 20),
            TextAlign = ContentAlignment.MiddleLeft,
            LinkColor = Color.DarkSlateGray
        };

        llManageUsers = new LinkLabel
        {
            Text      = "👤 Manage Users",
            Location  = new Point(170, 48),
            Size      = new Size(130, 20),
            TextAlign = ContentAlignment.MiddleLeft,
            LinkColor = Color.DarkSlateGray
        };

        // ═════════════════════════════════════════════════════════════════════
        // GroupBox 1: Student Registration (Outer Container)
        // ═════════════════════════════════════════════════════════════════════
        grpStudentRegistration = new GroupBox
        {
            Text     = "Student Registration",
            Location = new Point(20, 70),
            Size     = new Size(660, 560),
            Font     = new Font("Times New Roman", 11f, FontStyle.Regular)
        };

        // ─────────────────────────────────────────────────────────────────────
        // GroupBox 2: Basic Details
        // ─────────────────────────────────────────────────────────────────────
        grpBasic = new GroupBox
        {
            Text     = "Basic Details",
            Location = new Point(15, 25),
            Size     = new Size(630, 215)
        };

        lblRegNo     = MakeLabel("Reg No",        15, 25);
        lblFirstName = MakeLabel("First Name",    15, 55);
        lblLastName  = MakeLabel("Last Name",     15, 85);
        lblDob       = MakeLabel("Date of Birth", 15, 115);
        lblGender    = MakeLabel("Gender",        15, 145);
        lblAddress   = MakeLabel("Address",       15, 175);

        cmbRegNo     = new ComboBox       { Location = new Point(140, 22),  Size = new Size(160, 25), DropDownStyle = ComboBoxStyle.DropDown };
        txtFirstName = new TextBox        { Location = new Point(140, 52),  Size = new Size(465, 25) };
        txtLastName  = new TextBox        { Location = new Point(140, 82),  Size = new Size(465, 25) };
        dtpDob       = new DateTimePicker { Location = new Point(140, 112), Size = new Size(200, 25), Format = DateTimePickerFormat.Short };
        rdMale       = new RadioButton    { Text = "Male",   Location = new Point(140, 142), Size = new Size(70, 25), Checked = true };
        rdFemale     = new RadioButton    { Text = "Female", Location = new Point(220, 142), Size = new Size(80, 25) };
        txtAddress   = new TextBox        { Location = new Point(140, 172), Size = new Size(465, 34), Multiline = true, ScrollBars = ScrollBars.Vertical };

        grpBasic.Controls.AddRange(new Control[]
        {
            lblRegNo, lblFirstName, lblLastName, lblDob, lblGender, lblAddress,
            cmbRegNo, txtFirstName, txtLastName, dtpDob, rdMale, rdFemale, txtAddress
        });

        // ─────────────────────────────────────────────────────────────────────
        // GroupBox 3: Contact Details
        // ─────────────────────────────────────────────────────────────────────
        grpContact = new GroupBox
        {
            Text     = "Contact Details",
            Location = new Point(15, 245),
            Size     = new Size(630, 115)
        };

        lblEmail  = MakeLabel("Email",        15, 25);
        lblMobile = MakeLabel("Mobile Phone", 15, 55);
        lblHome   = MakeLabel("Home Phone",   15, 85);

        txtEmail  = new TextBox { Location = new Point(140, 22), Size = new Size(465, 25) };
        txtMobile = new TextBox { Location = new Point(140, 52), Size = new Size(200, 25) };
        txtHome   = new TextBox { Location = new Point(140, 82), Size = new Size(200, 25) };

        grpContact.Controls.AddRange(new Control[]
        {
            lblEmail, lblMobile, lblHome,
            txtEmail, txtMobile, txtHome
        });

        // ─────────────────────────────────────────────────────────────────────
        // GroupBox 4: Parent Details
        // ─────────────────────────────────────────────────────────────────────
        grpParent = new GroupBox
        {
            Text     = "Parent Details",
            Location = new Point(15, 365),
            Size     = new Size(630, 115)
        };

        lblParentName = MakeLabel("Parent Name", 15, 25);
        lblNic        = MakeLabel("NIC",         15, 55);
        lblContactNo  = MakeLabel("Contact No",  15, 85);

        txtParentName = new TextBox { Location = new Point(140, 22), Size = new Size(465, 25) };
        txtNic        = new TextBox { Location = new Point(140, 52), Size = new Size(200, 25) };
        txtContactNo  = new TextBox { Location = new Point(140, 82), Size = new Size(200, 25) };

        grpParent.Controls.AddRange(new Control[]
        {
            lblParentName, lblNic, lblContactNo,
            txtParentName, txtNic, txtContactNo
        });

        // ─────────────────────────────────────────────────────────────────────
        // Four Action Buttons (per Figure II)
        // ─────────────────────────────────────────────────────────────────────
        btnRegister = MakeButton("Register", 15,  495, Color.SteelBlue);
        btnUpdate   = MakeButton("Update",   145, 495, Color.DarkSeaGreen);
        btnClear    = MakeButton("Clear",    275, 495, Color.Gray);
        btnDelete   = MakeButton("Delete",   505, 495, Color.IndianRed);

        grpStudentRegistration.Controls.AddRange(new Control[]
        {
            grpBasic, grpContact, grpParent,
            btnRegister, btnUpdate, btnClear, btnDelete
        });

        Controls.AddRange(new Control[]
        {
            lblTitle, llViewDirectory, llManageUsers, llLogout, llExit, grpStudentRegistration
        });

        // ── Event Wiring ─────────────────────────────────────────────────────
        cmbRegNo.SelectedIndexChanged += CmbRegNo_SelectedIndexChanged;
        cmbRegNo.KeyDown              += CmbRegNo_KeyDown;
        btnRegister.Click             += BtnRegister_Click;
        btnUpdate.Click               += BtnUpdate_Click;
        btnClear.Click                += (_, _) => ClearForm();
        btnDelete.Click               += BtnDelete_Click;
        llLogout.LinkClicked          += (_, _) => LogoutRequested?.Invoke();
        llExit.LinkClicked            += (_, _) => ConfirmExit();
        llViewDirectory.LinkClicked   += LlViewDirectory_LinkClicked;
        llManageUsers.LinkClicked     += LlManageUsers_LinkClicked;

        FormClosing += (_, e) =>
        {
            e.Cancel = true;
            ConfirmExit();
        };
    }

    // ── Helper Data Methods ──────────────────────────────────────────────────

    private void LoadRegNos()
    {
        cmbRegNo.Items.Clear();
        foreach (var n in _studentRepo.GetAllRegNos())
            cmbRegNo.Items.Add(n);
    }

    public void LoadStudentByRegNo(int regNo)
    {
        var student = _studentRepo.GetByRegNo(regNo);
        if (student != null)
        {
            FillForm(student);
        }
        else
        {
            MessageBox.Show($"Student with Registration No {regNo} was not found.", "Search",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void FillForm(Student s)
    {
        cmbRegNo.Text      = s.RegNo.ToString();
        txtFirstName.Text  = s.FirstName;
        txtLastName.Text   = s.LastName;
        dtpDob.Value       = s.DateOfBirth;
        rdMale.Checked     = string.Equals(s.Gender, "Male", StringComparison.OrdinalIgnoreCase);
        rdFemale.Checked   = string.Equals(s.Gender, "Female", StringComparison.OrdinalIgnoreCase);
        txtAddress.Text    = s.Address;
        txtEmail.Text      = s.Email;
        txtMobile.Text     = s.MobilePhone.ToString();
        txtHome.Text       = s.HomePhone == 0 ? string.Empty : s.HomePhone.ToString();
        txtParentName.Text = s.ParentName;
        txtNic.Text        = s.Nic;
        txtContactNo.Text  = s.ContactNo == 0 ? string.Empty : s.ContactNo.ToString();
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

    private bool ValidateInputs()
    {
        // 1. Required fields
        if (string.IsNullOrWhiteSpace(cmbRegNo.Text) ||
            string.IsNullOrWhiteSpace(txtFirstName.Text) ||
            string.IsNullOrWhiteSpace(txtLastName.Text) ||
            string.IsNullOrWhiteSpace(txtAddress.Text) ||
            string.IsNullOrWhiteSpace(txtEmail.Text) ||
            string.IsNullOrWhiteSpace(txtMobile.Text))
        {
            MessageBox.Show("Please fill in all compulsory fields (Reg No, First Name, Last Name, Address, Email, Mobile Phone).",
                "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        // 2. RegNo numeric validation
        if (!int.TryParse(cmbRegNo.Text.Trim(), out var regNo) || regNo <= 0)
        {
            MessageBox.Show("Registration Number must be a valid positive integer.",
                "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cmbRegNo.Focus();
            return false;
        }

        // 3. Email structure validation
        var email = txtEmail.Text.Trim();
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            MessageBox.Show("Please enter a valid email address (e.g. student@example.com).",
                "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtEmail.Focus();
            return false;
        }

        // 4. Mobile Phone validation
        if (!long.TryParse(txtMobile.Text.Trim(), out var mobile) || mobile <= 0)
        {
            MessageBox.Show("Mobile Phone must contain numeric digits only.",
                "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtMobile.Focus();
            return false;
        }

        // 5. Home Phone validation (optional field, but if provided must be numeric)
        if (!string.IsNullOrWhiteSpace(txtHome.Text) && (!long.TryParse(txtHome.Text.Trim(), out var home) || home < 0))
        {
            MessageBox.Show("Home Phone must contain numeric digits only.",
                "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtHome.Focus();
            return false;
        }

        // 6. Contact No validation (optional field, but if provided must be numeric)
        if (!string.IsNullOrWhiteSpace(txtContactNo.Text) && (!long.TryParse(txtContactNo.Text.Trim(), out var contact) || contact < 0))
        {
            MessageBox.Show("Contact Number must contain numeric digits only.",
                "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtContactNo.Focus();
            return false;
        }

        return true;
    }

    private void ClearForm()
    {
        cmbRegNo.Text      = string.Empty;
        txtFirstName.Clear();
        txtLastName.Clear();
        dtpDob.Value       = DateTime.Today;
        rdMale.Checked     = true;
        txtAddress.Clear();
        txtEmail.Clear();
        txtMobile.Clear();
        txtHome.Clear();
        txtParentName.Clear();
        txtNic.Clear();
        txtContactNo.Clear();
        cmbRegNo.Focus();
    }

    // ── Event Handlers ───────────────────────────────────────────────────────

    private void CmbRegNo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cmbRegNo.SelectedItem is int regNo)
        {
            LoadStudentByRegNo(regNo);
        }
    }

    private void CmbRegNo_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            if (int.TryParse(cmbRegNo.Text.Trim(), out var regNo))
            {
                LoadStudentByRegNo(regNo);
            }
        }
    }

    private void BtnRegister_Click(object? sender, EventArgs e)
    {
        if (!ValidateInputs()) return;

        var regNo = int.Parse(cmbRegNo.Text.Trim());

        // Check for existing registration number
        var existing = _studentRepo.GetByRegNo(regNo);
        if (existing != null)
        {
            MessageBox.Show($"A student record with Registration Number {regNo} already exists.\nPlease use the Update button or enter a unique Registration Number.",
                "Duplicate Registration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _studentRepo.Add(ReadForm());
            MessageBox.Show("Record Added Successfully", "Register Student",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadRegNos();
            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding record: {ex.Message}", "Registration Failed",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnUpdate_Click(object? sender, EventArgs e)
    {
        if (!ValidateInputs()) return;

        var regNo = int.Parse(cmbRegNo.Text.Trim());

        // Check if record exists before update
        var existing = _studentRepo.GetByRegNo(regNo);
        if (existing == null)
        {
            MessageBox.Show($"Cannot update: No student found with Registration Number {regNo}.",
                "Record Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _studentRepo.Update(ReadForm());
            MessageBox.Show("Record Updated Successfully", "Update Student",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating record: {ex.Message}", "Update Failed",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(cmbRegNo.Text))
        {
            MessageBox.Show("Please select or enter a Registration Number to delete.", "Delete Student",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!int.TryParse(cmbRegNo.Text.Trim(), out var regNo))
        {
            MessageBox.Show("Registration Number must be a valid integer.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var existing = _studentRepo.GetByRegNo(regNo);
        if (existing == null)
        {
            MessageBox.Show($"Cannot delete: No student found with Registration Number {regNo}.",
                "Record Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            "Are you sure, Do you really wanted to Delete this Record...?",
            "Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        try
        {
            _studentRepo.Delete(regNo);
            MessageBox.Show("Record Deleted Successfully", "Delete Student",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadRegNos();
            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting record: {ex.Message}", "Delete Failed",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LlViewDirectory_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        var recordsForm = new StudentRecordsForm(_studentRepo);
        recordsForm.StudentSelected += regNo => LoadStudentByRegNo(regNo);
        recordsForm.ShowDialog(this);
    }

    private void LlManageUsers_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        var factory   = new DbConnectionFactory(AppConfig.ConnectionString);
        var loginRepo = new LoginRepository(factory);
        var usersForm = new ManageUsersForm(loginRepo);
        usersForm.ShowDialog(this);
    }

    private static void ConfirmExit()
    {
        var r = MessageBox.Show(
            "Are you sure you want to exit?",
            "Exit",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (r == DialogResult.Yes)
            Application.Exit();
    }

    // ── Control Helpers ──────────────────────────────────────────────────────

    private static Label MakeLabel(string text, int x, int y) => new()
    {
        Text      = text,
        Location  = new Point(x, y),
        Size      = new Size(120, 25),
        TextAlign = ContentAlignment.MiddleLeft
    };

    private static Button MakeButton(string text, int x, int y, Color back) => new()
    {
        Text      = text,
        Location  = new Point(x, y),
        Size      = new Size(110, 32),
        BackColor = back,
        ForeColor = Color.White,
        Font      = new Font("Times New Roman", 11f, FontStyle.Bold)
    };
}
