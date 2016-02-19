using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Microsoft.Win32;

namespace LogitechG930Fix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _ProgramFilesFolderPath = "C:\\Program Files";
        private string _ProgramFilesx86FolderPath = "C:\\Program Files(x86)";
        private string LGSResourcePath = "Logitech Gaming Software\\Resources\\G930\\Manifest";
        private string _ManifestFileName = "Device_Manifest.xml";

        private string _ManifestFilePath;

        private XmlDocument _ManifestXMLData;
        private XmlAttribute _ManifestBatteryTurnoffInterval;
        private XmlAttribute _ManifestBatteryWarningThreshold;
        private XmlAttribute _ManifestBatteryWarningFrequency;

        public MainWindow()
        {
            InitializeComponent();
            var programFilesManifestExists = File.Exists(_ProgramFilesFolderPath + "\\" + LGSResourcePath + "\\" + _ManifestFileName);
            if (programFilesManifestExists)
            {
                _ManifestFilePath = _ProgramFilesFolderPath + "\\" + LGSResourcePath + "\\" + _ManifestFileName;
                FolderPath.Content = _ManifestFilePath;

                LoadXMLFileData(_ManifestFilePath);
            }

            var programFilesx86ManifestExists = File.Exists(_ProgramFilesx86FolderPath + "\\" + LGSResourcePath + "\\" + _ManifestFileName);
            if (programFilesx86ManifestExists)
            {
                _ManifestFilePath = _ProgramFilesx86FolderPath + "\\" + LGSResourcePath + "\\" + _ManifestFileName;
                FolderPath.Content = _ManifestFilePath;

                LoadXMLFileData(_ManifestFilePath);
            }

            if (programFilesManifestExists || programFilesx86ManifestExists)
            {
                FolderPathLabel.Content = "Auto-Detected " + FolderPathLabel.Content;
            }
            else
            {
                ManualLocateButton.Visibility = Visibility.Visible;
            }
        }

        private void LocateManifestManually(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();

            var userSelectedFile = fileDialog.ShowDialog();

            if (userSelectedFile.HasValue && userSelectedFile.Value)
            {
                LoadXMLFileData(fileDialog.FileName);
                FolderPathLabel.Content = "Selected " + FolderPathLabel.Content;
                FolderPath.Content = fileDialog.FileName;
            }
        }

        private void LoadXMLFileData(string filePath)
        {
            _ManifestXMLData = new XmlDocument();
            _ManifestXMLData.Load(filePath);

            var batteryNode = _ManifestXMLData.DocumentElement.GetElementsByTagName("battery")[0];

            _ManifestBatteryTurnoffInterval = batteryNode.Attributes["turnOffInterval"];
            BatteryTimeoutTextBox.Text = _ManifestBatteryTurnoffInterval.Value;

            _ManifestBatteryWarningThreshold = batteryNode.Attributes["lowBattTime"];
            BatteryWarningTextBox.Text = _ManifestBatteryWarningThreshold.Value;

            _ManifestBatteryWarningFrequency = batteryNode.Attributes["lowBattInformInterval"];
            BatteryWarningFrequencyTextBox.Text = _ManifestBatteryWarningFrequency.Value;
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            try
            {
                _ManifestBatteryTurnoffInterval.Value = BatteryTimeoutTextBox.Text;
                _ManifestBatteryWarningThreshold.Value = BatteryWarningTextBox.Text;
                _ManifestBatteryWarningFrequency.Value = BatteryWarningFrequencyTextBox.Text;
                _ManifestXMLData.Save(_ManifestFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
