using Guna.UI2.WinForms;
using System.Windows.Forms;
using System.Drawing;

namespace GestPipePowerPonit.Views.Auth
{
    partial class RegisterForm
    {
        private System.ComponentModel.IContainer components = null;

        private Guna2Panel pnlCard;
        private Label lblTitle, lblEmail, lblFullName, lblPassword, lblConfirmPassword,
                            lblPhoneNumber, lblGender, lblBirthDate, lblAddress,
                            lblEdu, lblCompany, lblOccupation;
        private Label lblEmailError, lblFullNameError, lblPasswordError, lblConfirmPasswordError,
                      lblPhoneNumberError, lblGenderError, lblBirthDateError, lblAddressError,
                      lblEduError, lblCompanyError, lblOccupationError;
        private Guna2TextBox txtEmail, txtFullName, txtPassword, txtConfirmPassword,
                                    txtPhoneNumber, txtCompany, txtOccupation;
        private Guna2ComboBox cmbGender, cmbAddress, cmbEdu;
        private Guna2DateTimePicker dtpBirthDate;
        private Guna2GradientButton btnRegister;
        private LinkLabel lnkLogin;
        private Guna2Button btnGoogle;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterForm));
            this.pnlCard = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblEmailError = new System.Windows.Forms.Label();
            this.lblFullName = new System.Windows.Forms.Label();
            this.txtFullName = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblFullNameError = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblPasswordError = new System.Windows.Forms.Label();
            this.lblConfirmPassword = new System.Windows.Forms.Label();
            this.txtConfirmPassword = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblConfirmPasswordError = new System.Windows.Forms.Label();
            this.lblPhoneNumber = new System.Windows.Forms.Label();
            this.txtPhoneNumber = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblPhoneNumberError = new System.Windows.Forms.Label();
            this.lblGender = new System.Windows.Forms.Label();
            this.cmbGender = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lblGenderError = new System.Windows.Forms.Label();
            this.lblBirthDate = new System.Windows.Forms.Label();
            this.dtpBirthDate = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.lblBirthDateError = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.cmbAddress = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lblAddressError = new System.Windows.Forms.Label();
            this.lblEdu = new System.Windows.Forms.Label();
            this.cmbEdu = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lblEduError = new System.Windows.Forms.Label();
            this.lblCompany = new System.Windows.Forms.Label();
            this.txtCompany = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblCompanyError = new System.Windows.Forms.Label();
            this.lblOccupation = new System.Windows.Forms.Label();
            this.txtOccupation = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblOccupationError = new System.Windows.Forms.Label();
            this.btnRegister = new Guna.UI2.WinForms.Guna2GradientButton();
            this.lnkLogin = new System.Windows.Forms.LinkLabel();
            this.btnGoogle = new Guna.UI2.WinForms.Guna2Button();
            this.guna2ControlBoxClose = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBoxMinimize = new Guna.UI2.WinForms.Guna2ControlBox();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.BackColor = System.Drawing.Color.Transparent;
            this.pnlCard.BorderRadius = 25;
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblEmail);
            this.pnlCard.Controls.Add(this.txtEmail);
            this.pnlCard.Controls.Add(this.lblEmailError);
            this.pnlCard.Controls.Add(this.lblFullName);
            this.pnlCard.Controls.Add(this.txtFullName);
            this.pnlCard.Controls.Add(this.lblFullNameError);
            this.pnlCard.Controls.Add(this.lblPassword);
            this.pnlCard.Controls.Add(this.txtPassword);
            this.pnlCard.Controls.Add(this.lblPasswordError);
            this.pnlCard.Controls.Add(this.lblConfirmPassword);
            this.pnlCard.Controls.Add(this.txtConfirmPassword);
            this.pnlCard.Controls.Add(this.lblConfirmPasswordError);
            this.pnlCard.Controls.Add(this.lblPhoneNumber);
            this.pnlCard.Controls.Add(this.txtPhoneNumber);
            this.pnlCard.Controls.Add(this.lblPhoneNumberError);
            this.pnlCard.Controls.Add(this.lblGender);
            this.pnlCard.Controls.Add(this.cmbGender);
            this.pnlCard.Controls.Add(this.lblGenderError);
            this.pnlCard.Controls.Add(this.lblBirthDate);
            this.pnlCard.Controls.Add(this.dtpBirthDate);
            this.pnlCard.Controls.Add(this.lblBirthDateError);
            this.pnlCard.Controls.Add(this.lblAddress);
            this.pnlCard.Controls.Add(this.cmbAddress);
            this.pnlCard.Controls.Add(this.lblAddressError);
            this.pnlCard.Controls.Add(this.lblEdu);
            this.pnlCard.Controls.Add(this.cmbEdu);
            this.pnlCard.Controls.Add(this.lblEduError);
            this.pnlCard.Controls.Add(this.lblCompany);
            this.pnlCard.Controls.Add(this.txtCompany);
            this.pnlCard.Controls.Add(this.lblCompanyError);
            this.pnlCard.Controls.Add(this.lblOccupation);
            this.pnlCard.Controls.Add(this.txtOccupation);
            this.pnlCard.Controls.Add(this.lblOccupationError);
            this.pnlCard.Controls.Add(this.btnRegister);
            this.pnlCard.Controls.Add(this.lnkLogin);
            this.pnlCard.Controls.Add(this.btnGoogle);
            this.pnlCard.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.pnlCard.Location = new System.Drawing.Point(133, 68);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Padding = new System.Windows.Forms.Padding(40);
            this.pnlCard.Size = new System.Drawing.Size(1100, 842);
            this.pnlCard.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.lblTitle.Location = new System.Drawing.Point(221, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(645, 90);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Welcome To GestPipe";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblEmail.ForeColor = System.Drawing.Color.White;
            this.lblEmail.Location = new System.Drawing.Point(71, 130);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(51, 20);
            this.lblEmail.TabIndex = 1;
            this.lblEmail.Text = "Gmail:";
            // 
            // txtEmail
            // 
            this.txtEmail.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtEmail.BorderRadius = 10;
            this.txtEmail.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEmail.DefaultText = "";
            this.txtEmail.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtEmail.ForeColor = System.Drawing.Color.Black;
            this.txtEmail.Location = new System.Drawing.Point(71, 150);
            this.txtEmail.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.PlaceholderText = "Enter gmail";
            this.txtEmail.SelectedText = "";
            this.txtEmail.Size = new System.Drawing.Size(360, 34);
            this.txtEmail.TabIndex = 0;
            this.txtEmail.TextChanged += new System.EventHandler(this.txtEmail_TextChanged);
            // 
            // lblEmailError
            // 
            this.lblEmailError.AutoSize = true;
            this.lblEmailError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblEmailError.ForeColor = System.Drawing.Color.Wheat;
            this.lblEmailError.Location = new System.Drawing.Point(71, 188);
            this.lblEmailError.Name = "lblEmailError";
            this.lblEmailError.Size = new System.Drawing.Size(0, 19);
            this.lblEmailError.TabIndex = 2;
            this.lblEmailError.Visible = false;
            // 
            // lblFullName
            // 
            this.lblFullName.AutoSize = true;
            this.lblFullName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblFullName.ForeColor = System.Drawing.Color.White;
            this.lblFullName.Location = new System.Drawing.Point(71, 211);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(79, 20);
            this.lblFullName.TabIndex = 3;
            this.lblFullName.Text = "Full Name:";
            // 
            // txtFullName
            // 
            this.txtFullName.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtFullName.BorderRadius = 10;
            this.txtFullName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtFullName.DefaultText = "";
            this.txtFullName.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.txtFullName.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtFullName.ForeColor = System.Drawing.Color.Black;
            this.txtFullName.Location = new System.Drawing.Point(71, 231);
            this.txtFullName.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.PlaceholderText = "Enter full name";
            this.txtFullName.SelectedText = "";
            this.txtFullName.Size = new System.Drawing.Size(360, 34);
            this.txtFullName.TabIndex = 1;
            this.txtFullName.TextChanged += new System.EventHandler(this.txtFullName_TextChanged);
            // 
            // lblFullNameError
            // 
            this.lblFullNameError.AutoSize = true;
            this.lblFullNameError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblFullNameError.ForeColor = System.Drawing.Color.Wheat;
            this.lblFullNameError.Location = new System.Drawing.Point(71, 269);
            this.lblFullNameError.Name = "lblFullNameError";
            this.lblFullNameError.Size = new System.Drawing.Size(0, 19);
            this.lblFullNameError.TabIndex = 4;
            this.lblFullNameError.Visible = false;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPassword.ForeColor = System.Drawing.Color.White;
            this.lblPassword.Location = new System.Drawing.Point(71, 292);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(73, 20);
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtPassword.BorderRadius = 10;
            this.txtPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPassword.DefaultText = "";
            this.txtPassword.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPassword.ForeColor = System.Drawing.Color.Black;
            this.txtPassword.IconRight = global::GestPipePowerPonit.Properties.Resources.eye;
            this.txtPassword.IconRightCursor = System.Windows.Forms.Cursors.Hand;
            this.txtPassword.Location = new System.Drawing.Point(71, 312);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.PlaceholderText = "Enter password";
            this.txtPassword.SelectedText = "";
            this.txtPassword.Size = new System.Drawing.Size(360, 34);
            this.txtPassword.TabIndex = 2;
            this.txtPassword.IconRightClick += new System.EventHandler(this.txtPassword_IconRightClick);
            this.txtPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            // 
            // lblPasswordError
            // 
            this.lblPasswordError.AutoSize = true;
            this.lblPasswordError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblPasswordError.ForeColor = System.Drawing.Color.Wheat;
            this.lblPasswordError.Location = new System.Drawing.Point(71, 350);
            this.lblPasswordError.Name = "lblPasswordError";
            this.lblPasswordError.Size = new System.Drawing.Size(0, 19);
            this.lblPasswordError.TabIndex = 6;
            this.lblPasswordError.Visible = false;
            // 
            // lblConfirmPassword
            // 
            this.lblConfirmPassword.AutoSize = true;
            this.lblConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblConfirmPassword.ForeColor = System.Drawing.Color.White;
            this.lblConfirmPassword.Location = new System.Drawing.Point(71, 373);
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            this.lblConfirmPassword.Size = new System.Drawing.Size(134, 20);
            this.lblConfirmPassword.TabIndex = 7;
            this.lblConfirmPassword.Text = "Re-enter Password:";
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtConfirmPassword.BorderRadius = 10;
            this.txtConfirmPassword.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtConfirmPassword.DefaultText = "";
            this.txtConfirmPassword.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtConfirmPassword.ForeColor = System.Drawing.Color.Black;
            this.txtConfirmPassword.IconRight = global::GestPipePowerPonit.Properties.Resources.eye;
            this.txtConfirmPassword.IconRightCursor = System.Windows.Forms.Cursors.Hand;
            this.txtConfirmPassword.Location = new System.Drawing.Point(71, 393);
            this.txtConfirmPassword.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '●';
            this.txtConfirmPassword.PlaceholderText = "Enter password";
            this.txtConfirmPassword.SelectedText = "";
            this.txtConfirmPassword.Size = new System.Drawing.Size(360, 34);
            this.txtConfirmPassword.TabIndex = 3;
            this.txtConfirmPassword.IconRightClick += new System.EventHandler(this.txtConfirmPassword_IconRightClick);
            this.txtConfirmPassword.TextChanged += new System.EventHandler(this.txtConfirmPassword_TextChanged);
            // 
            // lblConfirmPasswordError
            // 
            this.lblConfirmPasswordError.AutoSize = true;
            this.lblConfirmPasswordError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblConfirmPasswordError.ForeColor = System.Drawing.Color.Wheat;
            this.lblConfirmPasswordError.Location = new System.Drawing.Point(71, 431);
            this.lblConfirmPasswordError.Name = "lblConfirmPasswordError";
            this.lblConfirmPasswordError.Size = new System.Drawing.Size(0, 19);
            this.lblConfirmPasswordError.TabIndex = 8;
            this.lblConfirmPasswordError.Visible = false;
            // 
            // lblPhoneNumber
            // 
            this.lblPhoneNumber.AutoSize = true;
            this.lblPhoneNumber.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPhoneNumber.ForeColor = System.Drawing.Color.White;
            this.lblPhoneNumber.Location = new System.Drawing.Point(71, 454);
            this.lblPhoneNumber.Name = "lblPhoneNumber";
            this.lblPhoneNumber.Size = new System.Drawing.Size(111, 20);
            this.lblPhoneNumber.TabIndex = 9;
            this.lblPhoneNumber.Text = "Phone Number:";
            // 
            // txtPhoneNumber
            // 
            this.txtPhoneNumber.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtPhoneNumber.BorderRadius = 10;
            this.txtPhoneNumber.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPhoneNumber.DefaultText = "";
            this.txtPhoneNumber.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.txtPhoneNumber.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtPhoneNumber.ForeColor = System.Drawing.Color.Black;
            this.txtPhoneNumber.Location = new System.Drawing.Point(71, 474);
            this.txtPhoneNumber.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.PlaceholderText = "Enter phone number";
            this.txtPhoneNumber.SelectedText = "";
            this.txtPhoneNumber.Size = new System.Drawing.Size(360, 34);
            this.txtPhoneNumber.TabIndex = 4;
            this.txtPhoneNumber.TextChanged += new System.EventHandler(this.txtPhoneNumber_TextChanged);
            // 
            // lblPhoneNumberError
            // 
            this.lblPhoneNumberError.AutoSize = true;
            this.lblPhoneNumberError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblPhoneNumberError.ForeColor = System.Drawing.Color.Wheat;
            this.lblPhoneNumberError.Location = new System.Drawing.Point(71, 512);
            this.lblPhoneNumberError.Name = "lblPhoneNumberError";
            this.lblPhoneNumberError.Size = new System.Drawing.Size(0, 19);
            this.lblPhoneNumberError.TabIndex = 10;
            this.lblPhoneNumberError.Visible = false;
            // 
            // lblGender
            // 
            this.lblGender.AutoSize = true;
            this.lblGender.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblGender.ForeColor = System.Drawing.Color.White;
            this.lblGender.Location = new System.Drawing.Point(71, 535);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(60, 20);
            this.lblGender.TabIndex = 11;
            this.lblGender.Text = "Gender:";
            // 
            // cmbGender
            // 
            this.cmbGender.BackColor = System.Drawing.Color.Transparent;
            this.cmbGender.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.cmbGender.BorderRadius = 10;
            this.cmbGender.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.cmbGender.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.cmbGender.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbGender.ForeColor = System.Drawing.Color.Black;
            this.cmbGender.ItemHeight = 30;
            this.cmbGender.Items.AddRange(new object[] {
            "Select gender",
            "Male",
            "Female",
            "Other"});
            this.cmbGender.Location = new System.Drawing.Point(71, 555);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(360, 36);
            this.cmbGender.TabIndex = 5;
            this.cmbGender.SelectedIndexChanged += new System.EventHandler(this.cmbGender_SelectedIndexChanged);
            // 
            // lblGenderError
            // 
            this.lblGenderError.AutoSize = true;
            this.lblGenderError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblGenderError.ForeColor = System.Drawing.Color.Wheat;
            this.lblGenderError.Location = new System.Drawing.Point(71, 595);
            this.lblGenderError.Name = "lblGenderError";
            this.lblGenderError.Size = new System.Drawing.Size(0, 19);
            this.lblGenderError.TabIndex = 12;
            this.lblGenderError.Visible = false;
            // 
            // lblBirthDate
            // 
            this.lblBirthDate.AutoSize = true;
            this.lblBirthDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBirthDate.ForeColor = System.Drawing.Color.White;
            this.lblBirthDate.Location = new System.Drawing.Point(591, 130);
            this.lblBirthDate.Name = "lblBirthDate";
            this.lblBirthDate.Size = new System.Drawing.Size(121, 20);
            this.lblBirthDate.TabIndex = 13;
            this.lblBirthDate.Text = "Date of Birthday:";
            // 
            // dtpBirthDate
            // 
            this.dtpBirthDate.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.dtpBirthDate.BorderRadius = 10;
            this.dtpBirthDate.BorderThickness = 1;
            this.dtpBirthDate.Checked = true;
            this.dtpBirthDate.FillColor = System.Drawing.Color.White;
            this.dtpBirthDate.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.dtpBirthDate.ForeColor = System.Drawing.Color.Black;
            this.dtpBirthDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBirthDate.Location = new System.Drawing.Point(591, 150);
            this.dtpBirthDate.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtpBirthDate.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtpBirthDate.Name = "dtpBirthDate";
            this.dtpBirthDate.Size = new System.Drawing.Size(360, 34);
            this.dtpBirthDate.TabIndex = 6;
            this.dtpBirthDate.Value = new System.DateTime(2025, 11, 4, 13, 1, 55, 546);
            this.dtpBirthDate.ValueChanged += new System.EventHandler(this.dtpBirthDate_ValueChanged);
            // 
            // lblBirthDateError
            // 
            this.lblBirthDateError.AutoSize = true;
            this.lblBirthDateError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblBirthDateError.ForeColor = System.Drawing.Color.Wheat;
            this.lblBirthDateError.Location = new System.Drawing.Point(591, 188);
            this.lblBirthDateError.Name = "lblBirthDateError";
            this.lblBirthDateError.Size = new System.Drawing.Size(0, 19);
            this.lblBirthDateError.TabIndex = 14;
            this.lblBirthDateError.Visible = false;
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblAddress.ForeColor = System.Drawing.Color.White;
            this.lblAddress.Location = new System.Drawing.Point(591, 211);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(96, 20);
            this.lblAddress.TabIndex = 15;
            this.lblAddress.Text = "Province/City";
            // 
            // cmbAddress
            // 
            this.cmbAddress.BackColor = System.Drawing.Color.Transparent;
            this.cmbAddress.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.cmbAddress.BorderRadius = 10;
            this.cmbAddress.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbAddress.DropDownHeight = 340;
            this.cmbAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAddress.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.cmbAddress.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.cmbAddress.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbAddress.ForeColor = System.Drawing.Color.Black;
            this.cmbAddress.IntegralHeight = false;
            this.cmbAddress.ItemHeight = 30;
            this.cmbAddress.Items.AddRange(new object[] {
            "Chọn Tỉnh/Thành phố",
            "Thành phố Hà Nội",
            "Thành phố Huế",
            "Tỉnh Lai Châu",
            "Tỉnh Điện Biên",
            "Tỉnh Sơn La",
            "Tỉnh Lạng Sơn",
            "Tỉnh Quảng Ninh",
            "Tỉnh Thanh Hóa",
            "Tỉnh Nghệ An",
            "Tỉnh Hà Tĩnh",
            "Tỉnh Cao Bằng",
            "Tỉnh Tuyên Quang",
            "Tỉnh Lào Cai",
            "Tỉnh Thái Nguyên",
            "Tỉnh Phú Thọ",
            "Tỉnh Bắc Ninh",
            "Tỉnh Hưng Yên",
            "Thành phố Hải Phòng",
            "Tỉnh Ninh Bình",
            "Tỉnh Quảng Trị",
            "Thành phố Đà Nẵng",
            "Tỉnh Bình Định",
            "Tỉnh Khánh Hòa",
            "Tỉnh Đắk Lắk",
            "Tỉnh Gia Lai",
            "Tỉnh Lâm Đồng",
            "Tỉnh Đồng Nai",
            "Thành phố Hồ Chí Minh",
            "Tỉnh Tây Ninh",
            "Tỉnh Tiền Giang",
            "Tỉnh Vĩnh Long",
            "Thành phố Cần Thơ",
            "Tỉnh An Giang",
            "Tỉnh Cà Mau"});
            this.cmbAddress.Location = new System.Drawing.Point(591, 231);
            this.cmbAddress.MaxDropDownItems = 5;
            this.cmbAddress.Name = "cmbAddress";
            this.cmbAddress.Size = new System.Drawing.Size(360, 36);
            this.cmbAddress.TabIndex = 7;
            this.cmbAddress.SelectedIndexChanged += new System.EventHandler(this.cmbAddress_SelectedIndexChanged);
            // 
            // lblAddressError
            // 
            this.lblAddressError.AutoSize = true;
            this.lblAddressError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblAddressError.ForeColor = System.Drawing.Color.Wheat;
            this.lblAddressError.Location = new System.Drawing.Point(591, 271);
            this.lblAddressError.Name = "lblAddressError";
            this.lblAddressError.Size = new System.Drawing.Size(0, 19);
            this.lblAddressError.TabIndex = 16;
            this.lblAddressError.Visible = false;
            // 
            // lblEdu
            // 
            this.lblEdu.AutoSize = true;
            this.lblEdu.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblEdu.ForeColor = System.Drawing.Color.White;
            this.lblEdu.Location = new System.Drawing.Point(591, 294);
            this.lblEdu.Name = "lblEdu";
            this.lblEdu.Size = new System.Drawing.Size(128, 20);
            this.lblEdu.TabIndex = 17;
            this.lblEdu.Text = "Educational Level:";
            // 
            // cmbEdu
            // 
            this.cmbEdu.BackColor = System.Drawing.Color.Transparent;
            this.cmbEdu.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.cmbEdu.BorderRadius = 10;
            this.cmbEdu.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbEdu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEdu.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.cmbEdu.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.cmbEdu.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbEdu.ForeColor = System.Drawing.Color.Black;
            this.cmbEdu.ItemHeight = 30;
            this.cmbEdu.Items.AddRange(new object[] {
            "Select educational level",
            "High School",
            "College",
            "Bachelor",
            "Master",
            "PhD"});
            this.cmbEdu.Location = new System.Drawing.Point(591, 314);
            this.cmbEdu.Name = "cmbEdu";
            this.cmbEdu.Size = new System.Drawing.Size(360, 36);
            this.cmbEdu.TabIndex = 8;
            this.cmbEdu.SelectedIndexChanged += new System.EventHandler(this.cmbEdu_SelectedIndexChanged);
            // 
            // lblEduError
            // 
            this.lblEduError.AutoSize = true;
            this.lblEduError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblEduError.ForeColor = System.Drawing.Color.Wheat;
            this.lblEduError.Location = new System.Drawing.Point(591, 354);
            this.lblEduError.Name = "lblEduError";
            this.lblEduError.Size = new System.Drawing.Size(0, 19);
            this.lblEduError.TabIndex = 18;
            this.lblEduError.Visible = false;
            // 
            // lblCompany
            // 
            this.lblCompany.AutoSize = true;
            this.lblCompany.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCompany.ForeColor = System.Drawing.Color.White;
            this.lblCompany.Location = new System.Drawing.Point(591, 373);
            this.lblCompany.Name = "lblCompany";
            this.lblCompany.Size = new System.Drawing.Size(108, 20);
            this.lblCompany.TabIndex = 19;
            this.lblCompany.Text = "Your Company:";
            // 
            // txtCompany
            // 
            this.txtCompany.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtCompany.BorderRadius = 10;
            this.txtCompany.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCompany.DefaultText = "";
            this.txtCompany.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.txtCompany.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtCompany.ForeColor = System.Drawing.Color.Black;
            this.txtCompany.Location = new System.Drawing.Point(591, 393);
            this.txtCompany.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtCompany.Name = "txtCompany";
            this.txtCompany.PlaceholderText = "Enter company";
            this.txtCompany.SelectedText = "";
            this.txtCompany.Size = new System.Drawing.Size(360, 34);
            this.txtCompany.TabIndex = 9;
            this.txtCompany.TextChanged += new System.EventHandler(this.txtCompany_TextChanged);
            // 
            // lblCompanyError
            // 
            this.lblCompanyError.AutoSize = true;
            this.lblCompanyError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblCompanyError.ForeColor = System.Drawing.Color.Wheat;
            this.lblCompanyError.Location = new System.Drawing.Point(591, 435);
            this.lblCompanyError.Name = "lblCompanyError";
            this.lblCompanyError.Size = new System.Drawing.Size(0, 19);
            this.lblCompanyError.TabIndex = 20;
            this.lblCompanyError.Visible = false;
            // 
            // lblOccupation
            // 
            this.lblOccupation.AutoSize = true;
            this.lblOccupation.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblOccupation.ForeColor = System.Drawing.Color.White;
            this.lblOccupation.Location = new System.Drawing.Point(591, 454);
            this.lblOccupation.Name = "lblOccupation";
            this.lblOccupation.Size = new System.Drawing.Size(88, 20);
            this.lblOccupation.TabIndex = 21;
            this.lblOccupation.Text = "Occupation:";
            // 
            // txtOccupation
            // 
            this.txtOccupation.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtOccupation.BorderRadius = 10;
            this.txtOccupation.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtOccupation.DefaultText = "";
            this.txtOccupation.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.txtOccupation.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtOccupation.ForeColor = System.Drawing.Color.Black;
            this.txtOccupation.Location = new System.Drawing.Point(591, 474);
            this.txtOccupation.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtOccupation.Name = "txtOccupation";
            this.txtOccupation.PlaceholderText = "Enter occupation";
            this.txtOccupation.SelectedText = "";
            this.txtOccupation.Size = new System.Drawing.Size(360, 34);
            this.txtOccupation.TabIndex = 10;
            this.txtOccupation.TextChanged += new System.EventHandler(this.txtOccupation_TextChanged);
            // 
            // lblOccupationError
            // 
            this.lblOccupationError.AutoSize = true;
            this.lblOccupationError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblOccupationError.ForeColor = System.Drawing.Color.Wheat;
            this.lblOccupationError.Location = new System.Drawing.Point(591, 518);
            this.lblOccupationError.Name = "lblOccupationError";
            this.lblOccupationError.Size = new System.Drawing.Size(0, 19);
            this.lblOccupationError.TabIndex = 22;
            this.lblOccupationError.Visible = false;
            // 
            // btnRegister
            // 
            this.btnRegister.BorderRadius = 10;
            this.btnRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegister.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(188)))), ((int)(((byte)(212)))));
            this.btnRegister.FillColor2 = System.Drawing.SystemColors.Highlight;
            this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnRegister.ForeColor = System.Drawing.Color.White;
            this.btnRegister.Location = new System.Drawing.Point(439, 631);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(222, 42);
            this.btnRegister.TabIndex = 11;
            this.btnRegister.Text = "Register";
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // lnkLogin
            // 
            this.lnkLogin.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.lnkLogin.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lnkLogin.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(180)))), ((int)(((byte)(218)))));
            this.lnkLogin.Location = new System.Drawing.Point(0, 678);
            this.lnkLogin.Name = "lnkLogin";
            this.lnkLogin.Size = new System.Drawing.Size(1100, 25);
            this.lnkLogin.TabIndex = 12;
            this.lnkLogin.TabStop = true;
            this.lnkLogin.Text = "Login?";
            this.lnkLogin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lnkLogin.VisitedLinkColor = System.Drawing.Color.White;
            this.lnkLogin.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLogin_LinkClicked);
            // 
            // btnGoogle
            // 
            this.btnGoogle.BorderColor = System.Drawing.Color.DarkGray;
            this.btnGoogle.BorderRadius = 10;
            this.btnGoogle.BorderThickness = 1;
            this.btnGoogle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGoogle.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnGoogle.FillColor = System.Drawing.Color.White;
            this.btnGoogle.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnGoogle.ForeColor = System.Drawing.Color.Black;
            this.btnGoogle.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.btnGoogle.Image = global::GestPipePowerPonit.Properties.Resources.google;
            this.btnGoogle.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnGoogle.ImageOffset = new System.Drawing.Point(15, 0);
            this.btnGoogle.ImageSize = new System.Drawing.Size(18, 18);
            this.btnGoogle.Location = new System.Drawing.Point(398, 709);
            this.btnGoogle.Name = "btnGoogle";
            this.btnGoogle.Size = new System.Drawing.Size(300, 38);
            this.btnGoogle.TabIndex = 14;
            this.btnGoogle.Text = "Continue with Google";
            this.btnGoogle.TextOffset = new System.Drawing.Point(15, 0);
            this.btnGoogle.Click += new System.EventHandler(this.btnGoogle_Click);
            // 
            // guna2ControlBoxClose
            // 
            this.guna2ControlBoxClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBoxClose.BackColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxClose.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxClose.HoverState.FillColor = System.Drawing.Color.Red;
            this.guna2ControlBoxClose.IconColor = System.Drawing.Color.White;
            this.guna2ControlBoxClose.Location = new System.Drawing.Point(1304, 12);
            this.guna2ControlBoxClose.Name = "guna2ControlBoxClose";
            this.guna2ControlBoxClose.Size = new System.Drawing.Size(50, 29);
            this.guna2ControlBoxClose.TabIndex = 23;
            this.guna2ControlBoxClose.Click += new System.EventHandler(this.guna2ControlBoxClose_Click);
            // 
            // guna2ControlBoxMinimize
            // 
            this.guna2ControlBoxMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2ControlBoxMinimize.BackColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxMinimize.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            this.guna2ControlBoxMinimize.FillColor = System.Drawing.Color.Transparent;
            this.guna2ControlBoxMinimize.IconColor = System.Drawing.Color.White;
            this.guna2ControlBoxMinimize.Location = new System.Drawing.Point(1248, 12);
            this.guna2ControlBoxMinimize.Name = "guna2ControlBoxMinimize";
            this.guna2ControlBoxMinimize.Size = new System.Drawing.Size(50, 29);
            this.guna2ControlBoxMinimize.TabIndex = 24;
            // 
            // RegisterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1366, 768);
            this.Controls.Add(this.guna2ControlBoxClose);
            this.Controls.Add(this.guna2ControlBoxMinimize);
            this.Controls.Add(this.pnlCard);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "RegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Register - GestPipe";
            this.Load += new System.EventHandler(this.RegisterForm_Load);
            this.Resize += new System.EventHandler(this.RegisterForm_Resize);
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            this.ResumeLayout(false);

        }

        private Guna2ControlBox guna2ControlBoxClose;
        private Guna2ControlBox guna2ControlBoxMinimize;
    }
}
