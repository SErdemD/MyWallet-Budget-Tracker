using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MyWallet;
namespace MyWallet
{
    /// <summary>
    /// RegisterWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow() 
        {
            InitializeComponent();
        }

        public string generateSalt()
        {
            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);
            return Convert.ToBase64String(salt);
        }
        public static void SaveUser(User user)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderPath = System.IO.Path.Combine(documentsPath, "MyWallet");
            string filePath = System.IO.Path.Combine(folderPath, "users.json");

            Directory.CreateDirectory(folderPath); 

            List<User> users = new List<User>();

            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(existingJson))
                {
                    users = JsonSerializer.Deserialize<List<User>>(existingJson) ?? new List<User>();
                }
            }

            users.Add(user);

            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        private void registerClick(object sender, RoutedEventArgs e)
        {
            User user = new User();
            user.Fullname = fullName_tb.Text;
            user.Username = user_tb.Text;
            user.Salt = generateSalt();
            user.PasswordHash = Global.HashPassword(pass_tb.Password,user.Salt);
            try 
            {
                SaveUser(user);
                MessageBox.Show("Registered Successfuly");
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            catch
            {
                MessageBox.Show("An error occured.");
            }
        }
        private void loginClick(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
