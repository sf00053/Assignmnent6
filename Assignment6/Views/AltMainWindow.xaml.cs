using Assignment6.ViewModels;
using System.Windows;

namespace Assignment6.Views
{
    public partial class AltMainWindow : Window
    {
        public AltMainWindow()
        {
            InitializeComponent();

            MainViewModel viewModel = new();
            DataContext = viewModel;

            viewModel.PersonAdded += (sender, args) =>
            {
                PeopleListBox.ScrollIntoView(viewModel.SelectedPerson);
                FirstNameTextBox.Focus();
                FirstNameTextBox.SelectAll();
            };

            Closed += (sender, args) => viewModel.Dispose();
        }
    }
}