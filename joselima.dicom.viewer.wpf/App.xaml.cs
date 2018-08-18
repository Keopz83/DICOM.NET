using joselima.dicom.viewer.wpf.ViewModels;
using joselima.dicom.viewer.wpf.Views;
using System.Data;
using System.Linq;
using System.Windows;

namespace joselima.dicom.viewer.wpf {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {

            base.OnStartup(e);

            var mainWindow = new MainWindow() {
                DataContext = new DataSetViewModel()
            };
            mainWindow.Show();
        }
    }
}
