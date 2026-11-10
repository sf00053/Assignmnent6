using Assignment6.ViewModels;
using System.Windows;

namespace Assignment6.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainViewModel viewModel = new();
            DataContext = viewModel;

            viewModel.PersonAdded += (sender, args) =>
            {
                PeopleGrid.ScrollIntoView(viewModel.SelectedPerson);
                FirstNameTextBox.Focus();
                FirstNameTextBox.SelectAll();
            };

            Closed += (sender, args) => viewModel.Dispose();
        }
    }
}