using PizzArena_AdminPanel.API;
using PizzArena_AdminPanel.API.Category;
using PizzArena_AdminPanel.API.ChefSpecial;
using PizzArena_AdminPanel.API.GlobalSettings;
using PizzArena_AdminPanel.API.Order;
using PizzArena_AdminPanel.API.OrderItem;
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
using System.ComponentModel;
using System.Windows.Data;

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
        private readonly ObservableCollection<OrderDto> _orders = new();
        private readonly ObservableCollection<OrderItemDto> _orderitems = new();
        private readonly ObservableCollection<ChefSpecialDto> _chefspecials = new();
        private ICollectionView productView;
        private ICollectionView userView;
        private ICollectionView orderView;
        private ICollectionView orderitemView;

        public AdminPanel(ApiService api)
        {
            InitializeComponent();

            _api = api;
            CategoryGrid.ItemsSource = _categories;
            RestaurantGrid.ItemsSource = _restaurants;
            ChefSpecialGrid.ItemsSource = _chefspecials;
            productView = CollectionViewSource.GetDefaultView(_products);
            ProductGrid.ItemsSource = productView;
            userView = CollectionViewSource.GetDefaultView(_users);
            UsersGrid.ItemsSource = userView;
            orderView = CollectionViewSource.GetDefaultView(_orders);
            orderitemView = CollectionViewSource.GetDefaultView(_orderitems);

            productView.Filter = item =>
            {
                if (string.IsNullOrWhiteSpace(SearchProductTextBox.Text))
                    return true;

                var product = item as ProductDto;
                return product != null && product.Name.Contains(SearchProductTextBox.Text, StringComparison.OrdinalIgnoreCase);
            };

            userView.Filter = item =>
            {
                if (string.IsNullOrWhiteSpace(SearchUserTextBox.Text))
                    return true;

                var user = item as UserDto;
                return user != null && user.userName.Contains(SearchUserTextBox.Text, StringComparison.OrdinalIgnoreCase);
            };

            


            Loaded += async (_, __) => await LoadCategories();
            Loaded += async (_, __) => await LoadProducts();
            Loaded += async (_, __) => await LoadUsers();
            Loaded += async (_, __) => await LoadRestaurants();
            Loaded += async (_, __) => await LoadOrders();
            Loaded += async (_, __) => await LoadGlobalSettings();
            Loaded += async (_, __) => await LoadChefSpecials();
        }

        private async Task LoadChefSpecials()
        {
            _chefspecials.Clear();
            var list = await _api.GetAllChefSpecial();

            foreach (var item in list)
            {
                _chefspecials.Add(item);
            }
        }

        private async Task LoadGlobalSettings()
        {
            var settings = await _api.GetGlobalSettings();

            if (settings != null)
            {
                GlobalContactEmailTextBox.Text = settings.ContactEmail;
                GlobalDeliveryTextBox.Text = settings.DeliveryTime;
                GlobalFacebookTextBox.Text = settings.FacebookUrl;
                GlobalInstagramTextBox.Text = settings.InstagramUrl;
            }
            else
            {
                MessageBox.Show("Nem sikerült betölteni a globális beállításokat.");
            }
        }


        private async Task LoadOrders()
        {
            _orders.Clear();
            var list = await _api.GetAllOrder();

            foreach (var item in list)
            {
                _orders.Add(item);
            }
        }


        private async Task LoadRestaurants()
        {
            _restaurants.Clear();
            var list = await _api.GetAllRestaurant();

            foreach (var item in list)
            {
                _restaurants.Add(item);
            }

            OrderRestaurantGrid.ItemsSource = _restaurants;
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



        //categories
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


            var (ok, error) = await _api.CreateUserAdmin(UsernameTextBox.Text, UserEmailTextbox.Text, UserPasswordTextBox.Text);

            if (!ok)
            {
                MessageBox.Show(error, "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return; 
            }

            MessageBox.Show("Felhasználó sikeresen létrehozva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);

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
            if (RestaurantGrid.SelectedItem is RestaurantDto selected)
            {
                RestaurantNameTextBox.Text = selected.name;
                RestaurantDescriptionTextBox.Text = selected.description;
                RestaurantimageUrlTextBox.Text = selected.imageUrl;
                RestaurantopeningHoursTextBox.Text = selected.openingHours;
                RestaurantaddressTextBox.Text = selected.address;
            }
        }

        private async void RestaurantReload_Click(object sender, RoutedEventArgs e)
        {
            await LoadRestaurants();
        }

        private async void RestaurantAdd_Click(object sender, RoutedEventArgs e)
        {
            var name = RestaurantNameTextBox.Text.Trim();
            var description = RestaurantDescriptionTextBox.Text.Trim();
            var imageurl = RestaurantimageUrlTextBox.Text.Trim();
            var openinghours = RestaurantopeningHoursTextBox.Text.Trim();
            var address = RestaurantaddressTextBox.Text.Trim();
            var contactphone = RestaurantcontactphoneTextBox.Text.Trim();
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
            if (string.IsNullOrWhiteSpace(imageurl))
            {
                MessageBox.Show("Adj meg egy kép url-t.");
                return;
            }
            if (string.IsNullOrWhiteSpace(openinghours))
            {
                MessageBox.Show("Adj meg egy nyitvatartási időt.");
                return;
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Adj meg egy címet.");
                return;
            }
            if (string.IsNullOrWhiteSpace(contactphone))
            {
                MessageBox.Show("Adj meg egy telefonszámot.");
                return;
            }


            var ok = await _api.CreateRestaurant(name, description, imageurl,openinghours,address, contactphone);
            if (!ok)
            {
                MessageBox.Show("Nem sikerült létrehozni.");
                return;
            }

            RestaurantNameTextBox.Clear();
            RestaurantDescriptionTextBox.Clear();
            RestaurantimageUrlTextBox.Clear();
            RestaurantopeningHoursTextBox.Clear();
            RestaurantaddressTextBox.Clear();
            await LoadRestaurants();
        }

        private async void RestaurantUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (RestaurantGrid.SelectedItem is not RestaurantDto selected)
            {
                MessageBox.Show("Válassz ki egy éttermet.");
                return;
            }

            var name = RestaurantNameTextBox.Text.Trim();
            var description = RestaurantDescriptionTextBox.Text.Trim();
            var imageurl = RestaurantimageUrlTextBox.Text.Trim();
            var openinghours = RestaurantopeningHoursTextBox.Text.Trim();
            var address = RestaurantaddressTextBox.Text.Trim();
            var contactphone = RestaurantcontactphoneTextBox.Text.Trim();
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
            if (string.IsNullOrWhiteSpace(imageurl))
            {
                MessageBox.Show("Adj meg egy kép url-t.");
                return;
            }
            if (string.IsNullOrWhiteSpace(openinghours))
            {
                MessageBox.Show("Adj meg egy nyitvatartási időt.");
                return;
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Adj meg egy címet.");
                return;
            }
            if (string.IsNullOrWhiteSpace(contactphone))
            {
                MessageBox.Show("Adj meg egy telefonszámot.");
                return;
            }

            var ok = await _api.UpdateRestaurant(selected.Id, name, description, imageurl,openinghours,address, contactphone);
            if (!ok)
            {
                MessageBox.Show("Nem sikerült módosítani.");
                return;
            }

            await LoadRestaurants();
        }

        private async void RestaurantDelete_Click(object sender, RoutedEventArgs e)
        {
            if (RestaurantGrid.SelectedItem is not RestaurantDto selected)
            {
                MessageBox.Show("Válassz ki egy éttermet.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Biztos törlöd? (Id={selected.Id}, Name={selected.name})",
                "Törlés",
                MessageBoxButton.YesNo
            );

            if (confirm != MessageBoxResult.Yes)
                return;

            var ok = await _api.DeleteRestaurant(selected.Id);
            if (!ok)
            {
                MessageBox.Show("Nem sikerült törölni.");
                return;
            }

            RestaurantNameTextBox.Clear();
            RestaurantDescriptionTextBox.Clear();
            RestaurantimageUrlTextBox.Clear();
            RestaurantopeningHoursTextBox.Clear();
            RestaurantaddressTextBox.Clear();
            await LoadRestaurants();
        }

        //globalsettings

        private async void GlobalUpdate_Click(object sender, RoutedEventArgs e)
        {
            var contactemail = GlobalContactEmailTextBox.Text.Trim();
            var globaldelivery = GlobalDeliveryTextBox.Text.Trim();
            var facebook = GlobalFacebookTextBox.Text.Trim();
            var instagram = GlobalInstagramTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(contactemail) || string.IsNullOrWhiteSpace(globaldelivery) ||
                string.IsNullOrWhiteSpace(facebook) || string.IsNullOrWhiteSpace(instagram))
            {
                MessageBox.Show("Minden mezőt ki kell tölteni!");
                return;
            }

            var result = await _api.UpdateGlobalSettings(contactemail, globaldelivery, facebook, instagram);

            if (!result)
            {
                MessageBox.Show("Nem sikerült módosítani a beállításokat. Ellenőrizd a hálózatot vagy a jogosultságokat!");
                return;
            }

            MessageBox.Show("Beállítások sikeresen frissítve!");

            await LoadGlobalSettings();
        }

        private async void GlobalReload_Click(object sender, RoutedEventArgs e)
        {
            await LoadGlobalSettings();
        }

        //chefspecial

        private void ChefSpecialGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChefSpecialGrid.SelectedItem is ChefSpecialDto selected)
            {
                ChefSpecialProductIdTextBox.Text = selected.ProductId.ToString();
                ChefSpecialCustomNoteTextBox.Text = selected.CustomNote;
            }
            else
            {
                ChefSpecialProductIdTextBox.Clear();
                ChefSpecialCustomNoteTextBox.Clear();
            }
        }

        private async void ChefSpecialReload_Click(object sender, RoutedEventArgs e)
        {
            await LoadChefSpecials();
        }

        private async void ChefSpecialAdd_Click(object sender, RoutedEventArgs e)
        {
            var productid = ChefSpecialProductIdTextBox.Text.Trim();
            var customnote = ChefSpecialCustomNoteTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(productid))
            {
                MessageBox.Show("Adj meg egy termék id-t.");
                return;
            }
            if (string.IsNullOrWhiteSpace(customnote))
            {
                MessageBox.Show("Adj meg egy leirást.");
                return;
            }
           


            var ok = await _api.CreateChefSpecial(Convert.ToInt32( productid),customnote);
            if (!ok)
            {
                MessageBox.Show("Nem sikerült létrehozni.");
                return;
            }

            ChefSpecialProductIdTextBox.Clear();
            ChefSpecialCustomNoteTextBox.Clear();
            await LoadChefSpecials();
        }

        private async void ChefSpecialUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ChefSpecialGrid.SelectedItem is not ChefSpecialDto selected)
            {
                MessageBox.Show("Válassz ki egy séf ajánlást.");
                return;
            }

            var productid = ChefSpecialProductIdTextBox.Text.Trim();
            var customnote = ChefSpecialCustomNoteTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(productid))
            {
                MessageBox.Show("Adj meg egy termék id-t.");
                return;
            }
            if (string.IsNullOrWhiteSpace(customnote))
            {
                MessageBox.Show("Adj meg egy leirást.");
                return;
            }



            var ok = await _api.UpdateChefSpecial(selected.Id, Convert.ToInt32(productid), customnote);
            if (!ok)
            {
                MessageBox.Show("Nem sikerült frissíteni.");
                return;
            }

            ChefSpecialProductIdTextBox.Clear();
            ChefSpecialCustomNoteTextBox.Clear();
            await LoadChefSpecials();
        }

        private async void ChefSpecialtDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ChefSpecialGrid.SelectedItem is not ChefSpecialDto selected)
            {
                MessageBox.Show("Válassz ki egy séf ajánlást.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Biztos törlöd? (Id={selected.ProductId}, Name={selected.CustomNote})",
                "Törlés",
                MessageBoxButton.YesNo
            );

            if (confirm != MessageBoxResult.Yes)
                return;

            var ok = await _api.DeleteChefSpecial(selected.Id);
            if (!ok)
            {
                MessageBox.Show("Nem sikerült törölni.");
                return;
            }

            ChefSpecialProductIdTextBox.Clear();
            ChefSpecialCustomNoteTextBox.Clear();
            await LoadChefSpecials();
        }

        //search

        private void SearchProductTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            productView?.Refresh();
        }

        private void SearchUserTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            userView?.Refresh();
        }

        private void SearchOrderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            orderView?.Refresh();
        }

        private void SearchOrderItemTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            orderitemView?.Refresh();
        }


        //orders

        private void OpenRestaurantOrders_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is RestaurantDto selectedRestaurant)
            {
                var ordersWindow = new RestaurantOrdersWindow(_api, selectedRestaurant);
                ordersWindow.Owner = this; 
                ordersWindow.ShowDialog();
            }
        }
    }
}
