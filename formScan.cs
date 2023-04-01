using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace INSane
{
    partial struct ComboBoxItem
    {
        public string displayName;
        public string value;

        public ComboBoxItem(string _displayName, string _value)
        {
            displayName = _displayName;
            value = _value;
        }
    }

    public partial class formScan : Form
    {
        public bool GOT_MSG_CLOSEDS = false;
        private classSANE SANE;

        private List<ComboBoxItem> sources = new List<ComboBoxItem>()
        {
             new ComboBoxItem("Vorderseite", "Adf-Front"),
             new ComboBoxItem("Rückseite", "Adf-Back"),
             new ComboBoxItem("Beidseitig", "Adf-Duplex"),
        };

        private List<ComboBoxItem> modes = new List<ComboBoxItem>()
        {
             new ComboBoxItem("Schwarzweiß", "Lineart"),
             new ComboBoxItem("Graustufe", "Gray"),
             new ComboBoxItem("Farbig", "Color"),
        };

        private List<ComboBoxItem> resolutions = new List<ComboBoxItem>()
        {
             new ComboBoxItem("Niedrige Qualität ( 150dpi )", "150"),
             new ComboBoxItem("Mittlere Qualität ( 240dpi )", "240"),
             new ComboBoxItem("Hohe Qualität ( 600dpi )", "600"),
        };

        public formScan()
        {
            InitializeComponent();

            sources.ForEach(v => cbxSource.Items.Add(v.displayName));
            modes.ForEach(v => cbxMode.Items.Add(v.displayName));
            resolutions.ForEach(v => cbxResolution.Items.Add(v.displayName));

            cbxSource.SelectedItem = sources.Find(v => v.value == Properties.Settings.Default.source).displayName;
            cbxMode.SelectedItem = modes.Find(v => v.value == Properties.Settings.Default.mode).displayName;
            cbxResolution.SelectedItem = resolutions.Find(v => v.value == Properties.Settings.Default.resolution).displayName;
            cbPageAuto.Checked = Boolean.Parse(Properties.Settings.Default.pageAuto);
            cbBlankPageDetection.Checked = Boolean.Parse(Properties.Settings.Default.blankPageSkip);
            tbBlankPageDetectionSensitivity.Value = Convert.ToInt32(Properties.Settings.Default.blankPageSkipSensitivity);
            tbBlankPageDetectionSensitivity.Enabled = cbBlankPageDetection.Checked;
        }

        public void SetControlsEnables(bool t)
        {
            cbxSource.Enabled = t;
            cbxMode.Enabled = t;
            cbxResolution.Enabled = t;
            cbPageAuto.Enabled = t;
            cbBlankPageDetection.Enabled = t;
            tbBlankPageDetectionSensitivity.Enabled = t;
            btn_scan.Enabled = t;

            if (cbBlankPageDetection.Enabled && cbBlankPageDetection.Checked == false) tbBlankPageDetectionSensitivity.Enabled = cbBlankPageDetection.Checked;
        }

        public void SetHostInformation(string host, string device, bool duplex)
        {
            tbxHost.Text = host;
            tbxScanner.Text = device;
            cbxSource.Enabled = duplex;
        }

        internal void SetUserDefaults()
        {
            SANE.Net_Control_Option("source", Properties.Settings.Default.source);
            SANE.Net_Control_Option("mode", Properties.Settings.Default.mode);
            SANE.Net_Control_Option("resolution", Properties.Settings.Default.resolution);
            SANE.Net_Control_Option("page-auto", Properties.Settings.Default.pageAuto);
            SANE.Net_Control_Option("blank-page-skip", Properties.Settings.Default.blankPageSkip);
            SANE.Net_Control_Option("blank-page-skip-sensitivity", Properties.Settings.Default.blankPageSkipSensitivity);
        }

        internal void SetSANEConnection(classSANE _SANE)
        {
            this.SANE = _SANE;
        }

        private void cbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSource.SelectedIndex != -1) Properties.Settings.Default.source = sources.Find(v => v.displayName == (string)cbxSource.SelectedItem).value;
            Properties.Settings.Default.Save();
        }

        private void cbxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxMode.SelectedIndex != -1) Properties.Settings.Default.mode = modes.Find(v => v.displayName == (string)cbxMode.SelectedItem).value;
            Properties.Settings.Default.Save();
        }

        private void cbxResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxResolution.SelectedIndex != -1) Properties.Settings.Default.resolution = resolutions.Find(v => v.displayName == (string)cbxResolution.SelectedItem).value;
            Properties.Settings.Default.Save();
        }

        private void cbPageAuto_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.pageAuto = Convert.ToString(cbPageAuto.Checked);
            Properties.Settings.Default.Save();
        }

        private void cbBlankPageDetection_CheckedChanged(object sender, EventArgs e)
        {
            tbBlankPageDetectionSensitivity.Enabled = cbBlankPageDetection.Checked;
            Properties.Settings.Default.blankPageSkip = Convert.ToString(cbBlankPageDetection.Checked);
            Properties.Settings.Default.Save();
        }

        private void tbBlankPageDetectionSensitivity_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.blankPageSkipSensitivity = Convert.ToString(tbBlankPageDetectionSensitivity.Value);
            Properties.Settings.Default.Save();
        }
    }
}
