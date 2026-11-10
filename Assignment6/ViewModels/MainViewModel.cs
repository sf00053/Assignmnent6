using Assignment6.Data;
using Assignment6.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Assignment6.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        private const string NAME_SORT_TEXT = "Sorted by last name, then first name";
        private const string AGE_SORT_TEXT = "Sorted by age";
        private const string DEFAULT_STATUS_TEXT =
            "Click Save Changes to store additions, edits, and deletions in SQLite.";
        private const string SAVED_STATUS_TEXT = "Changes saved.";

        private readonly PersonContext database = new();

        private ObservableCollection<Person> people = new();
        private Person? selectedPerson;
        private string searchText = string.Empty;
        private string sortDescription = NAME_SORT_TEXT;
        private string statusMessage = DEFAULT_STATUS_TEXT;
        private bool sortByAge = false;

        public ObservableCollection<Person> People
        {
            get
            {
                return people;
            }
            private set
            {
                people = value;
                OnPropertyChanged();
            }
        }

        public Person? SelectedPerson
        {
            get
            {
                return selectedPerson;
            }
            set
            {
                selectedPerson = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                searchText = value;
                OnPropertyChanged();
                UpdatePeople();
            }
        }

        public string SortDescription
        {
            get
            {
                return sortDescription;
            }
            private set
            {
                sortDescription = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get
            {
                return statusMessage;
            }
            private set
            {
                statusMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand SortByNameCommand { get; }
        public ICommand SortByAgeCommand { get; }
        public ICommand AddPersonCommand { get; }
        public ICommand DeletePersonCommand { get; }
        public ICommand SaveChangesCommand { get; }

        // The View listens for this so it can move focus, which is a View job.
        public event EventHandler? PersonAdded;

        public MainViewModel()
        {
            SortByNameCommand = new RelayCommand(SortByName);
            SortByAgeCommand = new RelayCommand(SortByAge);
            AddPersonCommand = new RelayCommand(AddPerson);
            DeletePersonCommand = new RelayCommand(DeletePerson, () => SelectedPerson != null);
            SaveChangesCommand = new RelayCommand(SaveChanges);

            database.Database.EnsureCreated();
            SeedIfEmpty();
            database.People.Load();

            UpdatePeople();
        }

        private void SeedIfEmpty()
        {
            if (database.People.Any())
            {
                return;
            }

            database.People.AddRange(
                new Person { FirstName = "Taylor", LastName = "Brown", Age = 19 },
                new Person { FirstName = "Morgan", LastName = "Davis", Age = 21 },
                new Person { FirstName = "Alex", LastName = "Johnson", Age = 20 },
                new Person { FirstName = "Jordan", LastName = "Smith", Age = 22 });

            database.SaveChanges();
        }

        // LINQ to Objects over the loaded entities, so unsaved adds and deletes show up too.
        private void UpdatePeople()
        {
            IEnumerable<Person> query = database.People.Local;

            if (!string.IsNullOrEmpty(SearchText))
            {
                query = query.Where(p =>
                    p.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    p.LastName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            if (sortByAge)
            {
                query = query.OrderBy(p => p.Age);
            }
            else
            {
                query = query.OrderBy(p => p.LastName).ThenBy(p => p.FirstName);
            }

            Person? previousSelection = SelectedPerson;

            People = new ObservableCollection<Person>(query.ToList());

            if (previousSelection != null && People.Contains(previousSelection))
            {
                SelectedPerson = previousSelection;
            }
        }

        private void SortByName()
        {
            sortByAge = false;
            SortDescription = NAME_SORT_TEXT;
            UpdatePeople();
        }

        private void SortByAge()
        {
            sortByAge = true;
            SortDescription = AGE_SORT_TEXT;
            UpdatePeople();
        }

        private void AddPerson()
        {
            SearchText = string.Empty;

            Person person = new()
            {
                FirstName = "New",
                LastName = "Person",
                Age = 18
            };

            database.People.Add(person);
            UpdatePeople();

            SelectedPerson = person;
            PersonAdded?.Invoke(this, EventArgs.Empty);
        }

        private void DeletePerson()
        {
            if (SelectedPerson == null)
            {
                return;
            }

            database.People.Remove(SelectedPerson);
            SelectedPerson = null;
            UpdatePeople();
        }

        private void SaveChanges()
        {
            database.SaveChanges();
            StatusMessage = SAVED_STATUS_TEXT;
        }

        public void Dispose()
        {
            database.Dispose();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}