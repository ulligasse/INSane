using System;
using System.Xml;
using System.Collections.Generic;
using System.Windows.Forms;
using static INSane.classSANE;
using System.Reflection;
using System.Configuration;
using System.IO;

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

        private Dictionary<string, string> translation = new Dictionary<string, string>()
        {
            { "Flatbed", "Flachbett" },
            { "Adf-front", "Vorderseite" },
            { "ADF Front", "Vorderseite" },
            { "Adf-back", "Rückseite" },
            { "Adf-duplex", "Beidseitig" },
            { "ADF Duplex", "Beidseitig" },
            { "Lineart", "Schwarzweiß" },
            { "Gray", "Graustufen" },
            { "Color", "Farbig" }
        };

        private List<ComboBoxItem> sources = new List<ComboBoxItem>(){};
        private List<ComboBoxItem> modes = new List<ComboBoxItem>(){};
        private List<ComboBoxItem> resolutions = new List<ComboBoxItem>(){};

        private Configuration config = null;
        private XmlDocument settings = new XmlDocument();
        private XmlNode scannerSettings = null;

        public formScan()
        {
            config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            if (config.AppSettings.Settings["configRedirect"] != null && config.AppSettings.Settings["configRedirect"].Value.Length > 0) {
                if (File.Exists(config.AppSettings.Settings["configRedirect"].Value)){
                    config = ConfigurationManager.OpenExeConfiguration(config.AppSettings.Settings["configRedirect"].Value);
                }
            }

            InitializeComponent();

            cbPageAuto.Checked = getScannerSetting("pageAuto").Length > 0 ? Boolean.Parse(getScannerSetting("pageAuto")) : false;
            cbBlankPageDetection.Checked = getScannerSetting("blankPageSkip").Length > 0 ? Boolean.Parse(getScannerSetting("blankPageSkip")) : false;
            tbBlankPageDetectionSensitivity.Value = getScannerSetting("blankPageSkipSensitivity").Length > 0 ? Convert.ToInt32(getScannerSetting("blankPageSkipSensitivity")) : 0;
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

        public void SetHostInformation()
        {
            tbxHost.Text = SANE.hostname;
            tbxScanner.Text = SANE.networkDevice.name;
            cbxSource.Enabled = SANE.DEVICE_DUPLEX;
        }

        internal void SetUserDefaults()
        {
            if (SANE.networkDeviceOptions != null)
            {
                if (SANE.networkDeviceOptions.FindIndex(v => v.name == "source") != -1)
                    SANE.Net_Control_Option("source", getScannerSetting("source"));

                if (SANE.networkDeviceOptions.FindIndex(v => v.name == "mode") != -1)
                    SANE.Net_Control_Option("mode", getScannerSetting("mode"));

                if (SANE.networkDeviceOptions.FindIndex(v => v.name == "resolution") != -1)
                    SANE.Net_Control_Option("resolution", getScannerSetting("resolution"));

                if (SANE.networkDeviceOptions.FindIndex(v => v.name == "page-auto") != -1)
                    SANE.Net_Control_Option("page-auto", getScannerSetting("pageAuto"));

                if (SANE.networkDeviceOptions.FindIndex(v => v.name == "blank-page-skip") != -1)
                    SANE.Net_Control_Option("blank-page-skip", getScannerSetting("blankPageSkip"));

                if (SANE.networkDeviceOptions.FindIndex(v => v.name == "blank-page-skip-sensitivity") != -1)
                    SANE.Net_Control_Option("blank-page-skip-sensitivity", getScannerSetting("blankPageSkipSensitivity"));
            }
        }

        internal void SetSANEConnection(classSANE _SANE)
        {
            this.SANE = _SANE;
        }

        internal void SetFormControlSource(NetworkDeviceOption option)
        {        
            if(cbxSource.Items.Count > 0)
            {
                sources.Clear();
                cbxSource.Items.Clear();
            }

            foreach (string constraint in option.constraint_values)
            {

                string displayName = translation.ContainsKey(constraint) ? translation[constraint] : constraint;
                sources.Add(new ComboBoxItem(displayName, constraint));
                cbxSource.Items.Add(displayName);
            }

            int index = -1;
            index = sources.FindIndex(v => v.value == getScannerSetting("source"));
            if (index >= 0) cbxSource.SelectedIndex = index;
            else cbxSource.SelectedIndex = 0;
        }

        internal void SetFormControlMode(NetworkDeviceOption option)
        {
            if (cbxMode.Items.Count > 0) {
                modes.Clear();
                cbxMode.Items.Clear();
            }

            foreach (string constraint in option.constraint_values)
            {
                string displayName = translation.ContainsKey(constraint) ? translation[constraint] : constraint;
                modes.Add(new ComboBoxItem(displayName, constraint));
                cbxMode.Items.Add(displayName);
            }

            int index = -1;
            index = modes.FindIndex(v => v.value == getScannerSetting("mode"));
            if (index >= 0) cbxMode.SelectedIndex = index;
            else cbxMode.SelectedIndex = 0;
        }

        internal void SetFormControlResolution(NetworkDeviceOption option)
        {
            if (cbxResolution.Items.Count > 0) {
                resolutions.Clear();
                cbxResolution.Items.Clear();
            }

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

            int index = -1;
            index = resolutions.FindIndex(v => v.value == getScannerSetting("resolution"));
            if (index >= 0) cbxResolution.SelectedIndex = index;
            else cbxResolution.SelectedIndex = 0;
        }

        internal void SetFormControls()
        {
            if (SANE.networkDeviceOptions != null)
            {
                int index = -1;

                index = SANE.networkDeviceOptions.FindIndex(v => v.name == "source");
                if (index >= 0) SetFormControlSource(SANE.networkDeviceOptions[index]);

                index = SANE.networkDeviceOptions.FindIndex(v => v.name == "mode");
                if (index >= 0) SetFormControlMode(SANE.networkDeviceOptions[index]);

                index = SANE.networkDeviceOptions.FindIndex(v => v.name == "resolution");
                if (index >= 0) SetFormControlResolution(SANE.networkDeviceOptions[index]);
            }
            else
                SetControlsEnables(false);
        }

        private void cbxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxSource.SelectedIndex != -1) saveScannerSetting("source", sources.Find(v => v.displayName == (string)cbxSource.SelectedItem).value);
        }

        private void cbxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxMode.SelectedIndex != -1) saveScannerSetting("mode", modes.Find(v => v.displayName == (string)cbxMode.SelectedItem).value);
        }

        private void cbxResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxResolution.SelectedIndex != -1) saveScannerSetting("resolution", resolutions.Find(v => v.displayName == (string)cbxResolution.SelectedItem).value);
        }

        private void cbPageAuto_CheckedChanged(object sender, EventArgs e)
        {
            saveScannerSetting("pageAuto", Convert.ToString(cbPageAuto.Checked));
        }

        private void cbBlankPageDetection_CheckedChanged(object sender, EventArgs e)
        {
            tbBlankPageDetectionSensitivity.Enabled = cbBlankPageDetection.Checked;
            saveScannerSetting("blankPageSkip", Convert.ToString(cbBlankPageDetection.Checked));
        }

        private void tbBlankPageDetectionSensitivity_ValueChanged(object sender, EventArgs e)
        {
            saveScannerSetting("blankPageSkipSensitivity", Convert.ToString(tbBlankPageDetectionSensitivity.Value));
        }

        public void saveScannerSetting(string _setting, string _value)
        {
            settings.Load(config.FilePath);
            scannerSettings = settings.DocumentElement.SelectSingleNode("scannerSettings");

            foreach (XmlNode node in scannerSettings.ChildNodes)
                if (node.Attributes["key"].Value == _setting)
                {
                    node.Attributes["value"].Value = _value;
                    settings.Save(config.FilePath);
                    return;
                }

            XmlElement setting = settings.CreateElement("setting");
            XmlAttribute key = settings.CreateAttribute("key");
            XmlAttribute value = settings.CreateAttribute("value");

            key.Value = _setting;
            value.Value = _value;

            setting.SetAttributeNode(key);
            setting.SetAttributeNode(value);

            scannerSettings.AppendChild(setting);
            settings.Save(config.FilePath);
            return;
        }

        public string getScannerSetting(string _setting)
        {
            settings.Load(config.FilePath);
            scannerSettings = settings.DocumentElement.SelectSingleNode("scannerSettings");

            foreach (XmlNode node in scannerSettings.ChildNodes)
                if (node.Attributes["key"].Value == _setting)
                    return node.Attributes["value"].Value;

            return "";
        }
    }
}
