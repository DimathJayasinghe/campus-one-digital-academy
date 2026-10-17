using CampusOne.Core.Interfaces;
using CampusOne.Core.Models;
using System.Text;

namespace CampusOne.App.Forms;

/// <summary>
/// Student Records Directory Form – Value-add feature.
/// Displays all student records in a DataGridView with live search, filtering, and CSV export.
/// </summary>
public class StudentRecordsForm : Form
{
    private readonly IStudentRepository _studentRepo;
    private List<Student> _allStudents = new();

    /// <summary>
    /// Event triggered when a user selects a student to open in the registration form.
    /// Passes the selected student's RegNo.
    /// </summary>
    public event Action<int>? StudentSelected;

    // ── Controls ─────────────────────────────────────────────────────────────
    private Label lblTitle          = null!;
    private Label lblSearch         = null!;
    private TextBox txtSearch       = null!;
    private Label lblGenderFilter   = null!;
    private ComboBox cmbGender      = null!;
    private Label lblCount          = null!;
    private DataGridView dgvStudents = null!;
    private Button btnOpen          = null!;
    private Button btnRefresh       = null!;
    private Button btnExportCsv     = null!;
    private Button btnClose         = null!;

    public StudentRecordsForm(IStudentRepository studentRepo)
    {
        _studentRepo = studentRepo;
        InitializeComponent();
        LoadData();
    }

    private void InitializeComponent()
    {
        Text            = "Student Records Directory – Campus One Digital Academy";
        Size            = new Size(950, 580);
        StartPosition   = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = true;
        BackColor       = Color.WhiteSmoke;
        Font            = new Font("Times New Roman", 11f);

        // Header Title
        lblTitle = new Label
        {
            Text      = "Student Records Directory",
            Font      = new Font("Times New Roman", 16f, FontStyle.Bold),
            Location  = new Point(20, 15),
            Size      = new Size(900, 30),
            TextAlign = ContentAlignment.MiddleLeft
        };

        // Filter / Search controls
        lblSearch = new Label
        {
            Text      = "Search:",
            Location  = new Point(20, 58),
            Size      = new Size(60, 25),
            TextAlign = ContentAlignment.MiddleLeft
        };

        txtSearch = new TextBox
        {
            Location    = new Point(85, 56),
            Size        = new Size(220, 26),
            PlaceholderText = "Search by Reg No, Name, NIC..."
        };

        lblGenderFilter = new Label
        {
            Text      = "Gender:",
            Location  = new Point(330, 58),
            Size      = new Size(60, 25),
            TextAlign = ContentAlignment.MiddleLeft
        };

        cmbGender = new ComboBox
        {
            Location      = new Point(395, 56),
            Size          = new Size(120, 26),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbGender.Items.AddRange(new object[] { "All", "Male", "Female" });
        cmbGender.SelectedIndex = 0;

        lblCount = new Label
        {
            Text      = "Showing 0 students",
            Location  = new Point(540, 58),
            Size      = new Size(250, 25),
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.DarkSlateGray
        };

        // DataGridView
        dgvStudents = new DataGridView
        {
            Location             = new Point(20, 95),
            Size                 = new Size(895, 370),
            Anchor               = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            ReadOnly             = true,
            AllowUserToAddRows   = false,
            AllowUserToDeleteRows= false,
            SelectionMode        = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect          = false,
            AutoSizeColumnsMode  = DataGridViewAutoSizeColumnsMode.DisplayedCells,
            BackgroundColor      = Color.White,
            BorderStyle          = BorderStyle.Fixed3D,
            RowHeadersVisible    = false
        };

        SetupGridColumns();

        // Bottom Action Buttons
        btnOpen = new Button
        {
            Text      = "Open in Registration",
            Location  = new Point(20, 485),
            Size      = new Size(180, 34),
            BackColor = Color.SteelBlue,
            ForeColor = Color.White
        };

        btnRefresh = new Button
        {
            Text      = "Refresh",
            Location  = new Point(210, 485),
            Size      = new Size(100, 34),
            BackColor = Color.SlateGray,
            ForeColor = Color.White
        };

        btnExportCsv = new Button
        {
            Text      = "Export to CSV",
            Location  = new Point(320, 485),
            Size      = new Size(130, 34),
            BackColor = Color.SeaGreen,
            ForeColor = Color.White
        };

        btnClose = new Button
        {
            Text      = "Close",
            Location  = new Point(815, 485),
            Size      = new Size(100, 34),
            BackColor = Color.IndianRed,
            ForeColor = Color.White
        };

        Controls.AddRange(new Control[]
        {
            lblTitle, lblSearch, txtSearch, lblGenderFilter, cmbGender, lblCount,
            dgvStudents, btnOpen, btnRefresh, btnExportCsv, btnClose
        });

        // Event wiring
        txtSearch.TextChanged         += (_, _) => ApplyFilter();
        cmbGender.SelectedIndexChanged += (_, _) => ApplyFilter();
        btnRefresh.Click              += (_, _) => LoadData();
        btnExportCsv.Click            += BtnExportCsv_Click;
        btnOpen.Click                 += BtnOpen_Click;
        btnClose.Click                += (_, _) => Close();
        dgvStudents.CellDoubleClick   += (_, _) => BtnOpen_Click(this, EventArgs.Empty);
    }

    private void SetupGridColumns()
    {
        dgvStudents.Columns.Clear();
        dgvStudents.Columns.Add("RegNo",       "Reg No");
        dgvStudents.Columns.Add("FullName",    "Full Name");
        dgvStudents.Columns.Add("DateOfBirth", "DOB");
        dgvStudents.Columns.Add("Gender",      "Gender");
        dgvStudents.Columns.Add("Address",     "Address");
        dgvStudents.Columns.Add("Email",       "Email");
        dgvStudents.Columns.Add("MobilePhone", "Mobile");
        dgvStudents.Columns.Add("HomePhone",   "Home Phone");
        dgvStudents.Columns.Add("ParentName",  "Parent Name");
        dgvStudents.Columns.Add("Nic",         "NIC");
        dgvStudents.Columns.Add("ContactNo",   "Contact No");
    }

    private void LoadData()
    {
        try
        {
            _allStudents = _studentRepo.GetAll().ToList();
            ApplyFilter();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load student records: {ex.Message}", "Database Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ApplyFilter()
    {
        var query = txtSearch.Text.Trim().ToLowerInvariant();
        var selectedGender = cmbGender.SelectedItem?.ToString() ?? "All";

        var filtered = _allStudents.Where(s =>
        {
            var matchesGender = selectedGender == "All" ||
                                string.Equals(s.Gender, selectedGender, StringComparison.OrdinalIgnoreCase);

            if (!matchesGender) return false;

            if (string.IsNullOrWhiteSpace(query)) return true;

            return s.RegNo.ToString().Contains(query) ||
                   s.FirstName.ToLowerInvariant().Contains(query) ||
                   s.LastName.ToLowerInvariant().Contains(query) ||
                   s.Email.ToLowerInvariant().Contains(query) ||
                   s.Nic.ToLowerInvariant().Contains(query) ||
                   s.MobilePhone.ToString().Contains(query) ||
                   s.Address.ToLowerInvariant().Contains(query);
        }).ToList();

        dgvStudents.Rows.Clear();
        foreach (var s in filtered)
        {
            dgvStudents.Rows.Add(
                s.RegNo,
                s.FullName,
                s.DateOfBirth.ToString("yyyy-MM-dd"),
                s.Gender,
                s.Address,
                s.Email,
                s.MobilePhone,
                s.HomePhone == 0 ? "-" : s.HomePhone.ToString(),
                s.ParentName,
                s.Nic,
                s.ContactNo == 0 ? "-" : s.ContactNo.ToString()
            );
        }

        lblCount.Text = $"Showing {filtered.Count} of {_allStudents.Count} students";
    }

    private void BtnOpen_Click(object? sender, EventArgs e)
    {
        if (dgvStudents.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a student from the list.", "Selection Required",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var cellValue = dgvStudents.SelectedRows[0].Cells["RegNo"].Value;
        if (cellValue != null && int.TryParse(cellValue.ToString(), out var regNo))
        {
            StudentSelected?.Invoke(regNo);
            Close();
        }
    }

    private void BtnExportCsv_Click(object? sender, EventArgs e)
    {
        if (dgvStudents.Rows.Count == 0)
        {
            MessageBox.Show("No records available to export.", "Export CSV",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var sfd = new SaveFileDialog
        {
            Filter = "CSV Files (*.csv)|*.csv",
            FileName = $"CampusOne_Students_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
            Title = "Export Student Records to CSV"
        };

        if (sfd.ShowDialog() != DialogResult.OK) return;

        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("RegNo,FirstName,LastName,DateOfBirth,Gender,Address,Email,MobilePhone,HomePhone,ParentName,NIC,ContactNo");

            foreach (DataGridViewRow row in dgvStudents.Rows)
            {
                var regNo   = row.Cells["RegNo"].Value?.ToString();
                if (string.IsNullOrEmpty(regNo)) continue;

                var student = _allStudents.FirstOrDefault(s => s.RegNo.ToString() == regNo);
                if (student == null) continue;

                sb.AppendLine(string.Join(",",
                    student.RegNo,
                    EscapeCsv(student.FirstName),
                    EscapeCsv(student.LastName),
                    student.DateOfBirth.ToString("yyyy-MM-dd"),
                    EscapeCsv(student.Gender),
                    EscapeCsv(student.Address),
                    EscapeCsv(student.Email),
                    student.MobilePhone,
                    student.HomePhone,
                    EscapeCsv(student.ParentName),
                    EscapeCsv(student.Nic),
                    student.ContactNo
                ));
            }

            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show("Student records successfully exported to CSV!", "Export Complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to export CSV: {ex.Message}", "Export Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value)) return "\"\"";
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return $"\"{value}\"";
    }
}
