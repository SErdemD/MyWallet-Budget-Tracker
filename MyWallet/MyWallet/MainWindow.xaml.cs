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
using System.Text.Json;
using System.IO;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;


namespace MyWallet
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {



        public ISeries[] PieSeries { get; set; }
        public ObservableCollection<Expense> Expenses { get; set; } = new();

        public string _username;
        public static List<Expense> LoadExpensesFromJson()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderPath = System.IO.Path.Combine(documentsPath, "MyWallet");
            string filePath = System.IO.Path.Combine(folderPath, "expenses.json");

            if (!File.Exists(filePath)) return new List<Expense>();

            string json = File.ReadAllText(filePath);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };

            return JsonSerializer.Deserialize<List<Expense>>(json,options) ?? new List<Expense>();
        }
        public double CalculateSpentThisMonth(List<Expense> allExpenses, string currentUsername)
        {
            DateTime now = DateTime.Now;
            int currentYear = now.Year;
            int currentMonth = now.Month;

            double total = allExpenses
                .Where(e => e.username == currentUsername)
                .Where(e => e.timestamp.Year == currentYear && e.timestamp.Month == currentMonth)
                .Sum(e => e.amount);

            return total;
        }
        public MainWindow(string username, string fullname)
        {
            InitializeComponent();
            _username = username;
            DataContext = this;
            var expenses = LoadExpensesFromJson().Where(e => e.username == username);
            welcome_tb.Text = $"Welcome, {fullname}";
            foreach (var expense in expenses)
            {
                Expenses.Add(expense);
            }
            CalculateSpentThisMonth(LoadExpensesFromJson(), username);

            double totalSpentThisMonth = expenses
            .Where(e => e.timestamp.Month == DateTime.Now.Month && e.timestamp.Year == DateTime.Now.Year)
            .Sum(e => e.amount);

            spentThisMonth.Text = $"You've spent {totalSpentThisMonth:0.00}$ this month";



            var categoryTotals = expenses
                .Where(e => e.timestamp.Year == DateTime.Now.Year && e.timestamp.Month == DateTime.Now.Month)
                .GroupBy(e => e.category)
                .Select(g => new { Category = g.Key.ToString(), Total = g.Sum(e => e.amount) })
                .ToList();


            PieSeries = categoryTotals
                .Select(c => new PieSeries<double>
                {
                    Values = new[] { c.Total },
                    Name = c.Category, 
                    DataLabelsSize = 14,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
                })
                .ToArray();

            DataContext = this;


        }

        private void addExpense_Click(object sender, EventArgs e)
        {
            var addWindow = new AddExpenseWindow(Expenses,_username);
            addWindow.Owner = this; 
            addWindow.ShowDialog();

            RefreshDashboard();
        }

        private void RefreshDashboard()
        {
           
            double totalSpentThisMonth = Expenses
                .Where(e => e.timestamp.Year == DateTime.Now.Year
                         && e.timestamp.Month == DateTime.Now.Month)
                .Sum(e => e.amount);
            spentThisMonth.Text = $"You've spent {totalSpentThisMonth:0.00}$ this month";

            

            var categoryTotals = Expenses
                .Where(e => e.timestamp.Year == DateTime.Now.Year && e.timestamp.Month == DateTime.Now.Month)
                .GroupBy(e => e.category)
                .Select(g => new { Category = g.Key.ToString(), Total = g.Sum(x => x.amount) })
                .ToList();


            PieSeries = categoryTotals
                .Select(c => new PieSeries<double>
                {
                    Values = new[] { c.Total },
                    Name = c.Category,
                    DataLabelsSize = 14,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
                })
                .ToArray();

            
            DataContext = null;
            DataContext = this;
        }
        private void signOutBtn_Click(object sender, RoutedEventArgs e)
        {
            
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

    }
}
