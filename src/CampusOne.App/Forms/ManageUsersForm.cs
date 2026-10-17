using CampusOne.Core.Interfaces;
using CampusOne.Core.Models;

namespace CampusOne.App.Forms;

/// <summary>
/// Manage Users Form – Multi-user administrative management.
/// Supports viewing, adding, updating password, and deleting system users.
/// </summary>
public class ManageUsersForm : Form
{
    private readonly ILoginRepository _loginRepo;

    // ── Controls ─────────────────────────────────────────────────────────────
    private Label lblTitle          = null!;
    private ListBox lstUsers        = null!;
    private Label lblUsers          = null!;
    private GroupBox grpAddUser     = null!;
    private Label lblNewUsername    = null!;
    private TextBox txtNewUsername  = null!;
    private Label lblNewPassword    = null!;
    private TextBox txtNewPassword  = null!;
    private Button btnAddUser       = null!;

    private GroupBox grpChangePwd   = null!;
    private Label lblSelectedUser   = null!;
    private Label lblNewPwd         = null!;
    private TextBox txtChangePwd    = null!;
    private Button btnChangePwd     = null!;

    private Button btnDeleteUser    = null!;
    private Button btnClose         = null!;

    public ManageUsersForm(ILoginRepository loginRepo)
    {
        _loginRepo = loginRepo;
        InitializeComponent();
        LoadUsers();
    }

    private void InitializeComponent()
    {
        Text            = "User Account Management – Campus One Digital Academy";
        Size            = new Size(580, 480);
        StartPosition   = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        BackColor       = Color.WhiteSmoke;
        Font            = new Font("Times New Roman", 11f);

        lblTitle = new Label
        {
            Text      = "System User Accounts",
            Font      = new Font("Times New Roman", 14f, FontStyle.Bold),
            Location  = new Point(20, 15),
            Size      = new Size(520, 30),
            TextAlign = ContentAlignment.MiddleLeft
        };

        // Users List
        lblUsers = new Label { Text = "Registered Users:", Location = new Point(20, 55), Size = new Size(180, 20) };
        lstUsers = new ListBox
        {
            Location = new Point(20, 80),
            Size     = new Size(180, 290)
        };

        // Group 1: Add New User
        grpAddUser = new GroupBox
        {
            Text     = "Add New User",
            Location = new Point(220, 70),
            Size     = new Size(320, 150)
        };

        lblNewUsername = new Label { Text = "Username:", Location = new Point(15, 30), Size = new Size(80, 22) };
        txtNewUsername = new TextBox { Location = new Point(105, 28), Size = new Size(195, 24) };

        lblNewPassword = new Label { Text = "Password:", Location = new Point(15, 65), Size = new Size(80, 22) };
        txtNewPassword = new TextBox { Location = new Point(105, 63), Size = new Size(195, 24), UseSystemPasswordChar = true };

        btnAddUser = new Button
        {
            Text      = "Create Account",
            Location  = new Point(105, 100),
            Size      = new Size(130, 30),
            BackColor = Color.SteelBlue,
            ForeColor = Color.White
        };
        grpAddUser.Controls.AddRange(new Control[] { lblNewUsername, txtNewUsername, lblNewPassword, txtNewPassword, btnAddUser });

        // Group 2: Change Password
        grpChangePwd = new GroupBox
        {
            Text     = "Change Password",
            Location = new Point(220, 235),
            Size     = new Size(320, 135)
        };

        lblSelectedUser = new Label
        {
            Text      = "Select a user from the list",
            Location  = new Point(15, 25),
            Size      = new Size(290, 22),
            Font      = new Font("Times New Roman", 10f, FontStyle.Italic),
            ForeColor = Color.DarkSlateGray
        };

        lblNewPwd = new Label { Text = "New Pwd:", Location = new Point(15, 55), Size = new Size(80, 22) };
        txtChangePwd = new TextBox { Location = new Point(105, 53), Size = new Size(195, 24), UseSystemPasswordChar = true };

        btnChangePwd = new Button
        {
            Text      = "Update Password",
            Location  = new Point(105, 88),
            Size      = new Size(140, 30),
            BackColor = Color.SeaGreen,
            ForeColor = Color.White
        };
        grpChangePwd.Controls.AddRange(new Control[] { lblSelectedUser, lblNewPwd, txtChangePwd, btnChangePwd });

        // Delete & Close buttons
        btnDeleteUser = new Button
        {
            Text      = "Delete User",
            Location  = new Point(20, 385),
            Size      = new Size(180, 32),
            BackColor = Color.IndianRed,
            ForeColor = Color.White
        };

        btnClose = new Button
        {
            Text      = "Close",
            Location  = new Point(440, 385),
            Size      = new Size(100, 32),
            BackColor = Color.Gray,
            ForeColor = Color.White
        };

        Controls.AddRange(new Control[]
        {
            lblTitle, lblUsers, lstUsers, grpAddUser, grpChangePwd, btnDeleteUser, btnClose
        });

        // Event wiring
        lstUsers.SelectedIndexChanged += LstUsers_SelectedIndexChanged;
        btnAddUser.Click    += BtnAddUser_Click;
        btnChangePwd.Click  += BtnChangePwd_Click;
        btnDeleteUser.Click += BtnDeleteUser_Click;
        btnClose.Click      += (_, _) => Close();
    }

    private void LoadUsers()
    {
        try
        {
            lstUsers.Items.Clear();
            var users = _loginRepo.GetAllUsers();
            foreach (var u in users)
            {
                lstUsers.Items.Add(u.Username);
            }

            if (lstUsers.Items.Count > 0)
                lstUsers.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load users: {ex.Message}", "Database Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LstUsers_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var username = lstUsers.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(username))
        {
            lblSelectedUser.Text = "Select a user from the list";
            btnDeleteUser.Enabled = false;
        }
        else
        {
            lblSelectedUser.Text = $"User: {username}";
            btnDeleteUser.Enabled = !string.Equals(username, "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }

    private void BtnAddUser_Click(object? sender, EventArgs e)
    {
        var username = txtNewUsername.Text.Trim();
        var password = txtNewPassword.Text.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Please enter both username and password.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var success = _loginRepo.CreateUser(username, password);
            if (success)
            {
                MessageBox.Show($"User '{username}' created successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNewUsername.Clear();
                txtNewPassword.Clear();
                LoadUsers();
            }
            else
            {
                MessageBox.Show($"User '{username}' already exists. Please choose a different username.", "Duplicate User",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating user: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnChangePwd_Click(object? sender, EventArgs e)
    {
        var username = lstUsers.SelectedItem?.ToString();
        var newPwd   = txtChangePwd.Text.Trim();

        if (string.IsNullOrEmpty(username))
        {
            MessageBox.Show("Please select a user first.", "Selection Required",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(newPwd))
        {
            MessageBox.Show("Please enter a new password.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var success = _loginRepo.UpdatePassword(username, newPwd);
            if (success)
            {
                MessageBox.Show($"Password for '{username}' updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtChangePwd.Clear();
            }
            else
            {
                MessageBox.Show("Failed to update password.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating password: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnDeleteUser_Click(object? sender, EventArgs e)
    {
        var username = lstUsers.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(username)) return;

        if (string.Equals(username, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show("The primary 'Admin' user cannot be deleted.", "Action Restricted",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            $"Are you sure you want to delete user '{username}'?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        try
        {
            var success = _loginRepo.DeleteUser(username);
            if (success)
            {
                MessageBox.Show($"User '{username}' deleted.", "Deleted",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting user: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
