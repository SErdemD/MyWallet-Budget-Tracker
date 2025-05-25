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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using MyWallet;

namespace MyWallet
{
    /// <summary>
    /// AddExpenseWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class AddExpenseWindow : Window
    {
        private ObservableCollection<Expense> _expenses;

        static string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        static string folderPath = System.IO.Path.Combine(documentsPath, "MyWallet");
        string filePath = System.IO.Path.Combine(folderPath, "expenses.json");

        public string _username;
        public AddExpenseWindow(ObservableCollection<Expense> expenses, string username)
        {
            InitializeComponent();
            _expenses = expenses;
            _username = username;
            categoryCombo.ItemsSource = Enum.GetValues(typeof(Category));
            categoryCombo.SelectedIndex = 0; 
        }
        private void addExpense_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(amountBox.Text, out double amount))
            {
                MessageBox.Show("Please enter a valid amount.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (categoryCombo.SelectedItem is not Category selectedCategory)
            {
                MessageBox.Show("Please select a category.", "Missing Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (datePicker.SelectedDate is not DateTime selectedDate)
            {
                MessageBox.Show("Please select a date.", "Missing Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newExpense = new Expense
            (
                _username,
                selectedCategory,
                amount,
                selectedDate
            );

            _expenses.Add(newExpense);
            SaveExpensesToFile();

            Close();
        }
        private void SaveExpensesToFile()
        {
            try
            {
                File.WriteAllText(filePath, JsonSerializer.Serialize(_expenses, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save expenses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
