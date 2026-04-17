using PizzArena_AdminPanel.API;
using System.Configuration;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PizzArena_AdminPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbUsername.Text) || string.IsNullOrWhiteSpace(pbPassword.Password))
            {
                MessageBox.Show("Kérjük, töltsön ki minden mezőt!", "Hiányzó adatok",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string baseUrl = ConfigurationManager.AppSettings["ApiUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                MessageBox.Show(
                    "Hiba: Az API elérési útvonala (ApiUrl) nincs beállítva a konfigurációs fájlban!\n\n" +
                    "Kérjük, ellenőrizze a PizzArena_AdminPanel.dll.config fájlt.",
                    "Konfigurációs hiba",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Application.Current.Shutdown();
                return;
            }

            var api = new ApiService();
            var result = await api.Login(tbUsername.Text, pbPassword.Password);

            if (result == null)
            {
                lError.Text = "Hibás felhasználónév vagy jelszó!";
                return;
            }
            

            TokenStorage.Token = result.Token;

            var admin = new AdminPanel(api);
            admin.Show();
            this.Close();
        }
    }
}