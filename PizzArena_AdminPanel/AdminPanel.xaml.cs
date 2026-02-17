using PizzArena_AdminPanel.API;
using PizzArena_AdminPanel.API.Category;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PizzArena_AdminPanel
{
    /// <summary>
    /// Interaction logic for AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        private readonly ApiService _api;
        private readonly ObservableCollection<CategoryDto> _categories = new();

        public AdminPanel(ApiService api)
        {
            InitializeComponent();

            _api = api;
            CategoryGrid.ItemsSource = _categories;

            Loaded += async (_, __) => await LoadCategories();
        }

        private async Task LoadCategories()
        {
            _categories.Clear();
            var list = await _api.GetAllCategories();

            foreach (var item in list)
                _categories.Add(item);
        }

        private async void CategoryReload_Click(object sender, RoutedEventArgs e)
        {
            await LoadCategories();
        }

        private async void CategoryAdd_Click(object sender, RoutedEventArgs e)
        {
            var name = CategoryNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Adj meg egy nevet.");
                return;
            }

            var ok = await _api.CreateCategory(name);
            if (!ok)
            {
                MessageBox.Show("Nem sikerült létrehozni.");
                return;
            }

            CategoryNameTextBox.Clear();
            await LoadCategories();
        }

        private async void CategoryUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryGrid.SelectedItem is not CategoryDto selected)
            {
                MessageBox.Show("Válassz ki egy kategóriát.");
                return;
            }

            var name = CategoryNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Adj meg egy nevet.");
                return;
            }

            var ok = await _api.UpdateCategory(selected.Id, name);
            if (!ok)
            {
                MessageBox.Show("Nem sikerült módosítani.");
                return;
            }

            await LoadCategories();
        }

        private void CategoryGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryGrid.SelectedItem is CategoryDto selected)
                CategoryNameTextBox.Text = selected.Name;
        }

        private async void CategoryDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryGrid.SelectedItem is not CategoryDto selected)
            {
                MessageBox.Show("Válassz ki egy kategóriát.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Biztos törlöd? (Id={selected.Id}, Name={selected.Name})",
                "Törlés",
                MessageBoxButton.YesNo
            );

            if (confirm != MessageBoxResult.Yes)
                return;

            var ok = await _api.DeleteCategory(selected.Id);
            if (!ok)
            {
                MessageBox.Show("Nem sikerült törölni.");
                return;
            }

            CategoryNameTextBox.Clear();
            await LoadCategories();
        }
    }
}
