using System;
using System.Collections.Generic;
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

namespace GUIForFTP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var clientViewModel = new ClientViewModel();

            this.DataContext = clientViewModel;

            //FileList.ItemsSource = clientViewModel.Files;

        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {

            int.TryParse(PortBox.Text.ToString(), out int intPort);
            await (DataContext as ClientViewModel).ConnectToServer(AddressBox.Text.ToString(), intPort);


        }

        private async void FilesMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!((sender as ListBox).SelectedItem is FileInfo fileInfo))
            {
                return;
            }


        }
    }
}
