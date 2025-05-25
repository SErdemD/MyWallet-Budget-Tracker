using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace MyWallet
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static string? GetFullName(string username)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderPath = System.IO.Path.Combine(documentsPath, "MyWallet");
            string filePath = System.IO.Path.Combine(folderPath, "users.json");

            if (!File.Exists(filePath))
                return null;

            string json = File.ReadAllText(filePath);
            var users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();

            var user = users.FirstOrDefault(u => u.Username == username);
            return user?.Fullname;
        }

        public static bool ValidateLogin(string username, string enteredPassword)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderPath = System.IO.Path.Combine(documentsPath, "MyWallet");
            string filePath = System.IO.Path.Combine(folderPath, "users.json");

            if (!File.Exists(filePath))
                return false;

            string json = File.ReadAllText(filePath);
            var users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();

            var user = users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return false;

            string enteredHash = Global.HashPassword(enteredPassword, user.Salt);
            return enteredHash == user.PasswordHash; 
        }
        public LoginWindow()
        {
            InitializeComponent();
        }
        
        private void loginClick(object sender, RoutedEventArgs e)
        {
            string username = user_tb.Text;
            string password = pass_tb.Password;
            if(username == null || password == null) 
            {
                MessageBox.Show("Please enter your username and password.");
            }
            else if(ValidateLogin(username, password))
            {
                MessageBox.Show("Successfully logged in");
                MainWindow mainWindow = new MainWindow(username, GetFullName(username));
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("wrong");
            }
        }
        private void registerClick(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close(); 
        }


    }
}

