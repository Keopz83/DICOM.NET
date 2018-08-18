using joselima.dicom.viewer.wpf.ViewModels;
using System;
using System.Windows;

namespace joselima.dicom.viewer.wpf.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Label_Drop(object sender, DragEventArgs e) {

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files) {
                Console.WriteLine($"File dropped {file}...");
            }
            (DataContext as DataSetViewModel).Load(files);
            
        }
    }
}
