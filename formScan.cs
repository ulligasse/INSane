using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using static INSane.classSANE;

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

        private List<ComboBoxItem> sources = new List<ComboBoxItem>(){};
        private List<ComboBoxItem> modes = new List<ComboBoxItem>(){};

        private List<ComboBoxItem> resolutions = new List<ComboBoxItem>(){};

        public formScan()
        {
            InitializeComponent();
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

        internal void SetFormControlSource(NetworkDeviceOption option)
        {          
            foreach(string constraint in option.constraint_values)
            {
                if (constraint.Trim().Length > 0)
                {
                    string displayName = constraint;

                    if (constraint == "Flatbed") displayName = "Flachbett";
                    if (constraint == "Adf-front") displayName = "Vorderseite";
                    if (constraint == "Adf-back") displayName = "Rückseite";
                    if (constraint == "Adf-duplex") displayName = "Beidseitig";

                    sources.Add(new ComboBoxItem(displayName, constraint));
                    cbxSource.Items.Add(displayName);
                }
            }

            if(cbxSource.Items.Count > 1)
                cbxSource.Enabled = true;

            int index = -1;
            index = sources.FindIndex(v => v.value == Properties.Settings.Default.source);
            if (index >= 0) cbxSource.SelectedIndex = index;
            else cbxSource.SelectedIndex = 0;
        }

        internal void SetFormControlMode(NetworkDeviceOption option)
        {
            foreach (string constraint in option.constraint_values)
            {
                if (constraint.Trim().Length > 0)
                {
                    string displayName = constraint;

                    if (constraint == "Lineart") displayName = "Schwarzweiß";
                    if (constraint == "Gray") displayName = "Graustufe";
                    if (constraint == "Color") displayName = "Farbig";

                    modes.Add(new ComboBoxItem(displayName, constraint));
                    cbxMode.Items.Add(displayName);
                }
            }

            if (cbxMode.Items.Count > 1)
                cbxMode.Enabled = true;

            int index = -1;
            index = modes.FindIndex(v => v.value == Properties.Settings.Default.mode);
            if (index >= 0) cbxMode.SelectedIndex = index;
            else cbxMode.SelectedIndex = 0;
        }

        internal void SetFormControlResolution(NetworkDeviceOption option)
        {
            int max_resolution = 0;
            foreach (string constraint in option.constraint_values)
            {
                if(constraint.Trim().Length > 0 && int.TryParse(constraint, out _))
                {
                    int resolution = Convert.ToInt32(constraint);
                    if (max_resolution < resolution)
                        max_resolution = resolution;
                }
            }

            if (max_resolution >= 150)
                resolutions.Add(new ComboBoxItem("Niedrige Qualität ( 150dpi )", "150"));
            if (max_resolution >= 240)
                resolutions.Add(new ComboBoxItem("Mittlere Qualität ( 240dpi )", "240"));
            if (max_resolution >= 600)
                resolutions.Add(new ComboBoxItem("Hohe Qualität ( 600dpi )", "600"));

            resolutions.ForEach(v => cbxResolution.Items.Add(v.displayName));

            if (cbxResolution.Items.Count > 1)
                cbxResolution.Enabled = true;

            int index = -1;
            index = resolutions.FindIndex(v => v.value == Properties.Settings.Default.resolution);
            if (index >= 0) cbxResolution.SelectedIndex = index;
            else cbxResolution.SelectedIndex = 0;
        }

        internal void SetFormControlPageAuto(NetworkDeviceOption option)
        {
            cbPageAuto.Enabled= true;
            cbPageAuto.Checked = Boolean.Parse(Properties.Settings.Default.pageAuto);
        }

        internal void SetFormControlBlankPageSkip(NetworkDeviceOption option)
        {
            cbBlankPageDetection.Enabled= true;
            cbBlankPageDetection.Checked = Boolean.Parse(Properties.Settings.Default.blankPageSkip);
            tbBlankPageDetectionSensitivity.Value = Convert.ToInt32(Properties.Settings.Default.blankPageSkipSensitivity);
            tbBlankPageDetectionSensitivity.Enabled = cbBlankPageDetection.Checked;
        }

        internal void SetFormControls()
        {
            int index = -1;

            index = SANE.networkDeviceOptions.FindIndex(v => v.name == "source");
            if (index >= 0) SetFormControlSource(SANE.networkDeviceOptions[index]);

            index = SANE.networkDeviceOptions.FindIndex(v => v.name == "mode");
            if (index >= 0) SetFormControlMode(SANE.networkDeviceOptions[index]);

            index = SANE.networkDeviceOptions.FindIndex(v => v.name == "resolution");
            if (index >= 0) SetFormControlResolution(SANE.networkDeviceOptions[index]);

            index = SANE.networkDeviceOptions.FindIndex(v => v.name == "page-auto");
            if (index >= 0) SetFormControlPageAuto(SANE.networkDeviceOptions[index]);

            index = SANE.networkDeviceOptions.FindIndex(v => v.name == "blank-page-skip");
            if (index >= 0) SetFormControlBlankPageSkip(SANE.networkDeviceOptions[index]);
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
