using joselima.dicom.viewer.wpf.ViewModels;
using joselima.dicom.viewer.wpf.Views;
using System;
using System.Windows;

namespace joselima.dicom.viewer.wpf {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        private static MainWindow _mainWindow;

        protected override void OnStartup(StartupEventArgs e) {

            base.OnStartup(e);

            DataSetViewModel.OnException += NotificationHandler;

            _mainWindow = new MainWindow() {
                DataContext = new DataSetViewModel()
            };
            _mainWindow.Show();
        }

        private static void NotificationHandler(object obj, Exception exc) {

            var notificationWindow = new NotificationWindow() {
                DataContext = new NotificationViewModel() { Message = exc.ToString() }
            };
            notificationWindow.Owner = _mainWindow;
            notificationWindow.Show();
        }
    }
}
