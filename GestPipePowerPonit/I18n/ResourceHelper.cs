using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace GestPipePowerPonit.I18n
{
    public static class ResourceHelper
    {
        // Set culture and apply resources to a WinForms form instance
        public static void SetCulture(string cultureCode, Form targetForm)
        {
            if (string.IsNullOrWhiteSpace(cultureCode) || targetForm == null) return;

            var culture = new CultureInfo(cultureCode);

            // Set default thread cultures so new threads inherit culture
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Immediate effect for current thread
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // Apply resources from the form's designer resources (HomeUser.resx etc.)
            var res = new ComponentResourceManager(targetForm.GetType());
            res.ApplyResources(targetForm, "$this");
            ApplyResourcesToControls(res, targetForm);
        }

        private static void ApplyResourcesToControls(ComponentResourceManager res, Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                try
                {
                    res.ApplyResources(c, c.Name, CultureInfo.CurrentUICulture);
                }
                catch
                {
                    // ignore if no resource found for this control
                }

                if (c.HasChildren)
                {
                    ApplyResourcesToControls(res, c);
                }
            }

            // For ToolStrip/MenuStrip, you may need to iterate items separately.
        }
    }
}