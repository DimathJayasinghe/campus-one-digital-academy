using CampusOne.Core.Interfaces;
using CampusOne.Data;
using CampusOne.Data.Repositories;

namespace CampusOne.App.Forms;

/// <summary>
/// Login Form – first window shown when the application starts.
/// Validates credentials against the Logins database table.
/// </summary>
public class LoginForm : Form
{
    // ── Controls ────────────────────────────────────────────────────────────
    private Label lblTitle      = null!;
    private Label lblUsername   = null!;
    private Label lblPassword   = null!;
    private TextBox txtUsername = null!;
    private TextBox txtPassword = null!;
    private Button btnLogin     = null!;
    private Button btnClear     = null!;
    private Button btnExit      = null!;
    private GroupBox grpLogin   = null!;
    private PictureBox picLogo  = null!;

    // ── Dependencies ─────────────────────────────────────────────────────────
    private readonly ILoginRepository _loginRepo;

    public LoginForm(ILoginRepository loginRepo)
    {
        _loginRepo = loginRepo;
        InitializeComponent();
    }

    // ── Designer-equivalent initialisation ──────────────────────────────────
    private void InitializeComponent()
    {
        Text            = "Login – Campus One Digital Academy";
        Size            = new Size(420, 340);
        StartPosition   = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox     = false;
        BackColor       = Color.WhiteSmoke;
        Font            = new Font("Times New Roman", 12f);

        picLogo = new PictureBox
        {
            Size        = new Size(80, 80),
            Location    = new Point(160, 10),
            SizeMode    = PictureBoxSizeMode.Zoom,
            BackColor   = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        lblTitle = new Label
        {
            Text      = "Campus One Digital Academy",
            Font      = new Font("Times New Roman", 14f, FontStyle.Bold),
            Location  = new Point(20, 100),
            Size      = new Size(380, 30),
            TextAlign = ContentAlignment.MiddleCenter
        };

        grpLogin = new GroupBox
        {
            Text     = "Login",
            Location = new Point(20, 130),
            Size     = new Size(370, 135)
        };

        lblUsername = new Label  { Text = "Username", Location = new Point(10, 30),  Size = new Size(90, 25) };
        lblPassword = new Label  { Text = "Password", Location = new Point(10, 65),  Size = new Size(90, 25) };
        txtUsername = new TextBox { Location = new Point(110, 28), Size = new Size(240, 25) };
        txtPassword = new TextBox { Location = new Point(110, 63), Size = new Size(240, 25), UseSystemPasswordChar = true };

        btnLogin = new Button { Text = "Login", Location = new Point(110, 97), Size = new Size(80, 28), BackColor = Color.SteelBlue,  ForeColor = Color.White };
        btnClear = new Button { Text = "Clear", Location = new Point(200, 97), Size = new Size(80, 28), BackColor = Color.Gray,       ForeColor = Color.White };
        btnExit  = new Button { Text = "Exit",  Location = new Point(290, 97), Size = new Size(80, 28), BackColor = Color.IndianRed,  ForeColor = Color.White };

        grpLogin.Controls.AddRange(new Control[] { lblUsername, lblPassword, txtUsername, txtPassword, btnLogin, btnClear, btnExit });
        Controls.AddRange(new Control[] { picLogo, lblTitle, grpLogin });

        // Wire events
        btnLogin.Click += BtnLogin_Click;
        btnClear.Click += BtnClear_Click;
        btnExit.Click  += BtnExit_Click;
        KeyPreview = true;
        KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) BtnLogin_Click(this, EventArgs.Empty); };
    }

    // ── Event handlers ───────────────────────────────────────────────────────

    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        var user = _loginRepo.Authenticate(txtUsername.Text.Trim(), txtPassword.Text);
        if (user is null)
        {
            MessageBox.Show(
                "Invalid username or password. Please try again.",
                "Login Failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            txtPassword.Clear();
            txtPassword.Focus();
            return;
        }

        // Open Registration form and hide this one
        var factory  = new DbConnectionFactory(AppConfig.ConnectionString);
        var studRepo = new StudentRepository(factory);
        var regForm  = new RegistrationForm(studRepo);

        regForm.LogoutRequested += () =>
        {
            regForm.Hide();
            Show();
            txtUsername.Clear();
            txtPassword.Clear();
            txtUsername.Focus();
        };

        Hide();
        regForm.ShowDialog();
    }

    private void BtnClear_Click(object? sender, EventArgs e)
    {
        txtUsername.Clear();
        txtPassword.Clear();
        txtUsername.Focus();
    }

    private void BtnExit_Click(object? sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you want to exit?",
            "Exit",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
            Application.Exit();
    }
}
