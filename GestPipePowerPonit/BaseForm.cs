using System;
using System.Drawing;
using System.Windows.Forms;

namespace GestPipePowerPonit
{
    public partial class BaseForm : Form
    {
        public BaseForm()
        {
            SetupResponsiveSettings();
        }

        private void SetupResponsiveSettings()
        {
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.MinimumSize = new Size(1024, 600);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        protected void SetupSidebarButtons(params Control[] buttons)
        {
            foreach (Control button in buttons)
            {
                if (button != null)
                {
                    button.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                }
            }
        }

        protected void SetupHeaderButtons(params Control[] buttons)
        {
            foreach (Control button in buttons)
            {
                if (button != null)
                {
                    button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                }
            }
        }
    }
}