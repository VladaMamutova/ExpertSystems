using System;
using System.Windows;
using SearchWithVariables.Logic;

namespace SearchWithVariables
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var searcher = Searcher.InitFromFile("data.txt");
            searcher.DebugMode = true;
            Data.Text = searcher.GetDataAsString();
            Data.Text += Environment.NewLine + searcher.GetStepData();
            searcher.Search();
            Result.Text += searcher.DebugLog;

        }
    }
}
