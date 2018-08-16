using joselima.dicom.viewer.wpf.ViewModels;
using joselima.dicom.viewer.wpf.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace joselima.dicom.viewer.wpf {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {

            base.OnStartup(e);

            var testFile1 = @"C:\Users\user1\Documents\DICOM\testData\Anonymized20180815\series-000000\image-000000.dcm";

            var dicomFile = new FileParser().Parse(testFile1, 0x7FE0000F);

            var dataSetVM = new DataSetViewModel() {
                Items = dicomFile.Attributes.Select(x => 
                    new AttributeViewModel() {
                        Tag = x.Value.Tag.ToString(),
                        Value = x.Value.Value.ToString().Trim(),
                    }).ToList()
            };

            var mainWindow = new MainWindow() {
                DataContext = dataSetVM
            };
            mainWindow.Show();
        }
    }
}
