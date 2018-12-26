using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        }

        /// <summary>
        /// Обработчик кнопи ConnectButton. Подключается к серверу.
        /// </summary>
        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {

            int.TryParse(PortBox.Text.ToString(), out int intPort);
            await (DataContext as ClientViewModel).ConnectToServer(AddressBox.Text.ToString(), intPort);
        }


        /// <summary>
        /// Обрабатывает двойной клик мыши по файлу, директории или апперу.
        /// Скачивает файл или запрашивает путь к директории у сервера.
        /// </summary>
        private async void FilesMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var fileInfo = ((sender as ListView).SelectedItem) as FileInfo;

            if (fileInfo.itemType == FileItemType.Upper)
            {
                await (DataContext as ClientViewModel).GetDirectory(fileInfo.FullName);
            }
            else if (fileInfo.itemType == FileItemType.Directory)
            {
                await (DataContext as ClientViewModel).GetDirectory(fileInfo.FullName);
            }
            else if (fileInfo.itemType == FileItemType.File)
            {
                await (DataContext as ClientViewModel).DownloadFile(fileInfo, downloadTextBox.Text.ToString());
            }
        }

        /// <summary>
        /// Скачивает все файлы в текущей директории.
        /// </summary>
        private void DownloadAllButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
