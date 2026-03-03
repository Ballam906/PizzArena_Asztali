using PizzArena_AdminPanel.API;
using PizzArena_AdminPanel.API.Category;
using PizzArena_AdminPanel.API.Product;
using PizzArena_AdminPanel.API.Restaurant;
using PizzArena_AdminPanel.API.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private readonly ObservableCollection<ProductDto> _products = new();
        private readonly ObservableCollection<UserDto> _users = new();
        private readonly ObservableCollection<RestaurantDto> _restaurants = new();

        public AdminPanel(ApiService api)
        {
            InitializeComponent();

            _api = api;
            CategoryGrid.ItemsSource = _categories;
            ProductGrid.ItemsSource = _products;
            UsersGrid.ItemsSource = _users;
            RestaurantGrid.ItemsSource = _restaurants;

            Loaded += async (_, __) => await LoadCategories();
            Loaded += async (_, __) => await LoadProducts();
            Loaded += async (_, __) => await LoadUsers();
            Loaded += async (_, __) => await LoadRestaurants();
        }
        private async Task LoadRestaurants()
        {
            _restaurants.Clear();
            var list = await _api.GetAllRestaurant();

            foreach (var item in list)
            {
                _restaurants.Add(item);
            }
        }


        private async Task LoadUsers()
        {
            _users.Clear();
            var list = await _api.GetAllUsers();

            foreach (var item in list)
            {
                _users.Add(item);
            }
        }

        private async Task LoadProducts()
        {
            _products.Clear();
            var list = await _api.GetAllProducts();

            foreach (var item in list)
            {
                _products.Add(item);
            }
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

        //product

        private async void ProductReload_Click(object sender, RoutedEventArgs e)
        {
            await LoadProducts();
        }

        private async void ProductAdd_Click(object sender, RoutedEventArgs e)
        {
            var name = ProductNameTextBox.Text.Trim();
            var description = ProductDescriptionTextBox.Text.Trim();
            var price = ProductPriceTextBox.Text.Trim();
            var isavailable = ProductIsAvailableTextBox.Text.Trim();
            var imageurl = ProductImageUrlTextBox.Text.Trim();
            var categoryid = ProductCategoryIdTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Adj meg egy nevet.");
                return;
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Adj meg egy leirást.");
                return;
            }
            if (string.IsNullOrWhiteSpace(price))
            {
                MessageBox.Show("Adj meg egy árat.");
                return;
            }
            if (string.IsNullOrWhiteSpace(isavailable))
            {
                MessageBox.Show("Adj meg egy elérhetőséget.");
                return;
            }
            if (string.IsNullOrWhiteSpace(imageurl))
            {
                MessageBox.Show("Adj meg egy kép url-t.");
                return;
            }
            if (string.IsNullOrWhiteSpace(categoryid))
            {
                MessageBox.Show("Adj meg egy kategória id-t.");
                return;
            }


            var ok = await _api.CreateProduct(name, description, Convert.ToInt32( price), Convert.ToBoolean( isavailable), imageurl,Convert.ToInt32( categoryid));
            if (!ok)
            {
                MessageBox.Show("Nem sikerült létrehozni.");
                return;
            }

            ProductNameTextBox.Clear();
            ProductPriceTextBox.Clear();
            ProductDescriptionTextBox.Clear();
            ProductIsAvailableTextBox.Clear();
            ProductImageUrlTextBox.Clear();
            ProductCategoryIdTextBox.Clear();
            await LoadProducts();
        }

        private async void ProductyUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ProductGrid.SelectedItem is not ProductDto selected)
            {
                MessageBox.Show("Válassz ki egy kategóriát.");
                return;
            }

            var name = ProductNameTextBox.Text.Trim();
            var description = ProductDescriptionTextBox.Text.Trim();
            var price = ProductPriceTextBox.Text.Trim();
            var isavailable = ProductIsAvailableTextBox.Text.Trim();
            var imageurl = ProductImageUrlTextBox.Text.Trim();
            var categoryid = ProductCategoryIdTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Adj meg egy nevet.");
                return;
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Adj meg egy leirást.");
                return;
            }
            if (string.IsNullOrWhiteSpace(price))
            {
                MessageBox.Show("Adj meg egy árat.");
                return;
            }
            if (string.IsNullOrWhiteSpace(isavailable))
            {
                MessageBox.Show("Adj meg egy elérhetőséget.");
                return;
            }
            if (string.IsNullOrWhiteSpace(imageurl))
            {
                MessageBox.Show("Adj meg egy kép url-t.");
                return;
            }
            if (string.IsNullOrWhiteSpace(categoryid))
            {
                MessageBox.Show("Adj meg egy kategória id-t.");
                return;
            }

            var ok = await _api.UpdateProduct(selected.Id, name, description, Convert.ToInt32(price), Convert.ToBoolean(isavailable), imageurl, Convert.ToInt32(categoryid));
            if (!ok)
            {
                MessageBox.Show("Nem sikerült módosítani.");
                return;
            }

            await LoadProducts();
        }

        private async void ProductDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ProductGrid.SelectedItem is not ProductDto selected)
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

            var ok = await _api.DeleteProduct(selected.Id);
            if (!ok)
            {
                MessageBox.Show("Nem sikerült törölni.");
                return;
            }

            ProductNameTextBox.Clear();
            ProductPriceTextBox.Clear();
            ProductDescriptionTextBox.Clear();
            ProductIsAvailableTextBox.Clear();
            ProductImageUrlTextBox.Clear();
            ProductCategoryIdTextBox.Clear();
            await LoadProducts();
        }

        private void ProductGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductGrid.SelectedItem is ProductDto selected)
            {
                ProductNameTextBox.Text = selected.Name;
                ProductPriceTextBox.Text = Convert.ToString(selected.Price);
                ProductDescriptionTextBox.Text = selected.Description;
                ProductIsAvailableTextBox.Text = Convert.ToString(selected.IsAvailable);
                ProductImageUrlTextBox.Text = selected.Image_Url;
                ProductCategoryIdTextBox.Text = Convert.ToString(selected.CategoryId);
            }

        }

        //user

        private void UsersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private async void UserAdd_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text.Trim();
            var email = UserEmailTextbox.Text.Trim();
            var password = UserPasswordTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Adj meg egy nevet.");
                return;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Adj meg egy email-t.");
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Adj meg egy jelszót.");
                return;
            }


            var ok = await _api.CreateUser(username,email,password);
            if (!ok)
            {
                MessageBox.Show("Nem sikerült létrehozni.");
                return;
            }

            UsernameTextBox.Clear();
            UserEmailTextbox.Clear();
            UserPasswordTextBox.Clear();
            await LoadUsers();
        }

        private async void UserReload_Click(object sender, RoutedEventArgs e)
        {
            await LoadUsers();
        }

        private async void UserDelete_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is not UserDto selected)
            {
                MessageBox.Show("Válassz ki egy felhasználót.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Biztos törlöd? (Id={selected.Id}, Name={selected.userName})",
                "Törlés",
                MessageBoxButton.YesNo
            );

            if (confirm != MessageBoxResult.Yes)
                return;

            var (ok, error) = await _api.DeleteUser(selected.Id);

            if (!ok)
            {
                MessageBox.Show($"Nem sikerült törölni.\n\nHiba: {error}");
                return;
            }

            await LoadUsers();
        }

        //restaurant

        private void RestaurantGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void RestaurantReload_Click(object sender, RoutedEventArgs e)
        {
            await LoadRestaurants();
        }

        private void RestaurantAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RestaurantUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RestaurantDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
