using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GestPipePowerPonit.Models.DTOs;
using GestPipePowerPonit.Services;


namespace GestPipePowerPonit.Views.Profile
{
    public partial class ChangePasswordForm : Form
    {
        private readonly ProfileService _profileService;
        private readonly string _userId;


        public ChangePasswordForm(string userId)
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Designer Error: {ex.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            _profileService = new ProfileService();
            _userId = userId;


            // ✅ Match parent ProfileForm size
            this.Size = new Size(1537, 960);
            this.Location = new Point(0, 0);
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);


            // ✅ Position relative to parent
            if (this.Owner != null)
            {
                this.Location = this.Owner.Location;
                this.Size = this.Owner.Size;
            }


            ApplyLanguage();
        }


        private void ApplyLanguage()
        {
            try
            {
                if (lblOldPassword != null) lblOldPassword.Text = "Old Password";
                if (lblNewPassword != null) lblNewPassword.Text = "New Password";
                if (lblConfirmPassword != null) lblConfirmPassword.Text = "Re-enter Password";
                if (btnSave != null) btnSave.Text = "Save";
                if (btnCancel != null) btnCancel.Text = "Cancel";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ApplyLanguage] Error: {ex.Message}");
            }
        }


        private void ClearErrors()
        {
            Color defaultBorder = Color.FromArgb(64, 64, 64);


            if (lblOldPasswordError != null) lblOldPasswordError.Visible = false;
            if (lblNewPasswordError != null) lblNewPasswordError.Visible = false;
            if (lblConfirmPasswordError != null) lblConfirmPasswordError.Visible = false;


            if (txtOldPassword != null) txtOldPassword.BorderColor = defaultBorder;
            if (txtNewPassword != null) txtNewPassword.BorderColor = defaultBorder;
            if (txtConfirmPassword != null) txtConfirmPassword.BorderColor = defaultBorder;
        }


        private bool ValidateInputs(out List<string> errors)
        {
            errors = new List<string>();
            ClearErrors();


            bool hasErrors = false;
            Color errorBorder = Color.FromArgb(255, 193, 7);


            if (txtOldPassword == null || string.IsNullOrWhiteSpace(txtOldPassword.Text))
            {
                if (lblOldPasswordError != null)
                {
                    lblOldPasswordError.Text = "Old password is required";
                    lblOldPasswordError.Visible = true;
                }
                if (txtOldPassword != null) txtOldPassword.BorderColor = errorBorder;
                errors.Add("Old password is required");
                hasErrors = true;
            }


            string newPassword = txtNewPassword?.Text ?? "";
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                if (lblNewPasswordError != null)
                {
                    lblNewPasswordError.Text = "Password must be at least 6 characters";
                    lblNewPasswordError.Visible = true;
                }
                if (txtNewPassword != null) txtNewPassword.BorderColor = errorBorder;
                errors.Add("New password is required");
                hasErrors = true;
            }
            else if (newPassword.Length < 6)
            {
                if (lblNewPasswordError != null)
                {
                    lblNewPasswordError.Text = "Password must be at least 6 characters";
                    lblNewPasswordError.Visible = true;
                }
                if (txtNewPassword != null) txtNewPassword.BorderColor = errorBorder;
                errors.Add("Password must be at least 6 characters");
                hasErrors = true;
            }


            if (txtConfirmPassword == null || string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                if (lblConfirmPasswordError != null)
                {
                    lblConfirmPasswordError.Text = "Please confirm your password";
                    lblConfirmPasswordError.Visible = true;
                }
                if (txtConfirmPassword != null) txtConfirmPassword.BorderColor = errorBorder;
                errors.Add("Please confirm your password");
                hasErrors = true;
            }
            else if (txtNewPassword?.Text != txtConfirmPassword?.Text)
            {
                if (lblConfirmPasswordError != null)
                {
                    lblConfirmPasswordError.Text = "Passwords do not match";
                    lblConfirmPasswordError.Visible = true;
                }
                if (txtConfirmPassword != null) txtConfirmPassword.BorderColor = errorBorder;
                errors.Add("Passwords do not match");
                hasErrors = true;
            }


            return !hasErrors;
        }


        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out var validationErrors))
            {
                return;
            }


            if (btnSave != null)
            {
                btnSave.Enabled = false;
                btnSave.Text = "Saving...";
            }
            if (btnCancel != null) btnCancel.Enabled = false;


            try
            {
                var changePasswordDto = new ChangePasswordDto
                {
                    OldPassword = txtOldPassword?.Text ?? "",
                    NewPassword = txtNewPassword?.Text ?? "",
                    ConfirmPassword = txtConfirmPassword?.Text ?? ""
                };


                var response = await _profileService.ChangePasswordAsync(_userId, changePasswordDto);


                if (response?.Success == true)
                {
                    CustomMessageBox.ShowSuccess(
                        "Password changed successfully!",
                        "Success"
                    );


                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    string errorMessage = response?.Message ?? "Failed to change password.";


                    if (errorMessage.ToLower().Contains("incorrect") || errorMessage.ToLower().Contains("wrong") || errorMessage.ToLower().Contains("old password"))
                    {
                        if (lblOldPasswordError != null)
                        {
                            lblOldPasswordError.Text = "Incorrect password. Please try again";
                            lblOldPasswordError.Visible = true;
                        }
                        if (txtOldPassword != null)
                        {
                            txtOldPassword.BorderColor = Color.FromArgb(255, 193, 7);
                            txtOldPassword.Clear();
                            txtOldPassword.Focus();
                        }
                    }
                    else if (errorMessage.ToLower().Contains("google") && errorMessage.ToLower().Contains("password"))
                    {
                        CustomMessageBox.ShowInfo(errorMessage, "Information");


                        // Option: disable nút Save cho account này
                        // if (btnSave != null) btnSave.Enabled = false;
                    }
                    else
                    {
                        CustomMessageBox.ShowError(errorMessage, "Error");
                    }
                }


            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowError($"Error: {ex.Message}", "Error");
            }
            finally
            {
                if (btnSave != null)
                {
                    btnSave.Enabled = true;
                    btnSave.Text = "Save";
                }
                if (btnCancel != null) btnCancel.Enabled = true;
            }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


        private void panelOverlay_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


        private void txtOldPassword_IconRightClick(object sender, EventArgs e)
        {
            if (txtOldPassword != null)
                txtOldPassword.PasswordChar = txtOldPassword.PasswordChar == '\0' ? '●' : '\0';
        }


        private void txtNewPassword_IconRightClick(object sender, EventArgs e)
        {
            if (txtNewPassword != null)
                txtNewPassword.PasswordChar = txtNewPassword.PasswordChar == '\0' ? '●' : '\0';
        }


        private void txtConfirmPassword_IconRightClick(object sender, EventArgs e)
        {
            if (txtConfirmPassword != null)
                txtConfirmPassword.PasswordChar = txtConfirmPassword.PasswordChar == '\0' ? '●' : '\0';
        }


        private void txtOldPassword_TextChanged(object sender, EventArgs e)
        {
            if (txtOldPassword != null && !string.IsNullOrWhiteSpace(txtOldPassword.Text))
            {
                if (lblOldPasswordError != null) lblOldPasswordError.Visible = false;
                txtOldPassword.BorderColor = Color.FromArgb(64, 64, 64);
            }
        }


        private void txtNewPassword_TextChanged(object sender, EventArgs e)
        {
            if (txtNewPassword == null) return;


            string newPassword = txtNewPassword.Text;


            if (string.IsNullOrWhiteSpace(newPassword))
            {
                if (lblNewPasswordError != null) lblNewPasswordError.Visible = false;
                txtNewPassword.BorderColor = Color.FromArgb(64, 64, 64);
            }
            else if (newPassword.Length < 6)
            {
                if (lblNewPasswordError != null)
                {
                    lblNewPasswordError.Text = $"{6 - newPassword.Length} more characters needed";
                    lblNewPasswordError.Visible = true;
                }
                txtNewPassword.BorderColor = Color.FromArgb(255, 193, 7);
            }
            else
            {
                if (lblNewPasswordError != null) lblNewPasswordError.Visible = false;
                txtNewPassword.BorderColor = Color.FromArgb(94, 148, 255);
            }


            if (txtConfirmPassword != null && !string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                txtConfirmPassword_TextChanged(sender, e);
            }
        }


        private void txtConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            if (txtConfirmPassword == null || txtNewPassword == null) return;


            if (string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                if (lblConfirmPasswordError != null) lblConfirmPasswordError.Visible = false;
                txtConfirmPassword.BorderColor = Color.FromArgb(64, 64, 64);
            }
            else if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                if (lblConfirmPasswordError != null)
                {
                    lblConfirmPasswordError.Text = "Passwords do not match";
                    lblConfirmPasswordError.Visible = true;
                }
                txtConfirmPassword.BorderColor = Color.FromArgb(255, 193, 7);
            }
            else
            {
                if (lblConfirmPasswordError != null) lblConfirmPasswordError.Visible = false;
                txtConfirmPassword.BorderColor = Color.FromArgb(94, 148, 255);
            }
        }
    }
}