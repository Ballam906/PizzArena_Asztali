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
        private readonly ObservableCollection<GlobalSettingsDto> _globalsettings = new();
        private readonly ObservableCollection<ChefSpecialDto> _chefspecials = new();

        public AdminPanel(ApiService api)
        {
            InitializeComponent();

            _api = api;
            CategoryGrid.ItemsSource = _categories;
            ProductGrid.ItemsSource = _products;
            UsersGrid.ItemsSource = _users;
            RestaurantGrid.ItemsSource = _restaurants;
            OrderGrid.ItemsSource = _orders;
            OrderItemGrid.ItemsSource = _orderitems;
            ChefSpecialGrid.ItemsSource = _chefspecials;


            Loaded += async (_, __) => await LoadCategories();
            Loaded += async (_, __) => await LoadProducts();
            Loaded += async (_, __) => await LoadUsers();
            Loaded += async (_, __) => await LoadRestaurants();
            Loaded += async (_, __) => await LoadOrders();
            Loaded += async (_, __) => await LoadOrderItems();
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
            _globalsettings.Clear();
            var list = await _api.GetGlobalSettings();

            foreach (var item in list)
            {
                _globalsettings.Add(item);
            }

            if (_globalsettings.Count > 0)
            {
                var settings = _globalsettings[0];
                GlobalContactEmailTextBox.Text = settings.ContactEmail;
                GlobalDeliveryTextBox.Text = settings.DeliveryTime;
                GlobalFacebookTextBox.Text = settings.FacebookUrl;
                GlobalInstagramTextBox.Text = settings.InstagramUrl;
            }
        }

        private async Task LoadOrderItems()
        {
            _orderitems.Clear();
            var list = await _api.GetAllOrderItem();

            foreach (var item in list)
            {
                _orderitems.Add(item);
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


            var ok = await _api.CreateRestaurant(name, description, imageurl,openinghours,address);
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

            var ok = await _api.UpdateRestaurant(selected.Id, name, description, imageurl,openinghours,address);
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

        //order

        private void OrderGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrderGrid.SelectedItem is OrderDto selected)
            {
                OrderNameTextBox.Text = selected.CustomerName;
                OrderEmailTextBox.Text = selected.CustomerEmail;
                OrderPhoneTextBox.Text = selected.CustomerPhone;
                OrderPostalCodeTextBox.Text = selected.PostalCode;
                OrderCityTextBox.Text = selected.City;
                OrderStreetTextBox.Text = selected.Street;
                OrderOtherTextBox.Text = selected.Other;
                OrderUserIdTextBox.Text = selected.User_Id;
            }
        }

        private async void OrderReload_Click(object sender, RoutedEventArgs e)
        {
            await LoadOrders();
        }

        private async void OrderUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (OrderGrid.SelectedItem is not OrderDto selected)
            {
                MessageBox.Show("Válassz ki egy rendelést.");
                return;
            }

            var name = OrderNameTextBox.Text.Trim();
            var email = OrderEmailTextBox.Text.Trim();
            var phone = OrderPhoneTextBox.Text.Trim();
            var postalcode = OrderPostalCodeTextBox.Text.Trim();
            var city = OrderCityTextBox.Text.Trim();
            var street = OrderStreetTextBox.Text.Trim();
            var other = OrderOtherTextBox.Text.Trim();
            var userid = OrderUserIdTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Adj meg egy nevet.");
                return;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Adj meg egy email-t.");
                return;
            }
            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Adj meg egy telefonszámot.");
                return;
            }
            if (string.IsNullOrWhiteSpace(postalcode))
            {
                MessageBox.Show("Adj meg egy irányítószámot.");
                return;
            }
            if (string.IsNullOrWhiteSpace(city))
            {
                MessageBox.Show("Adj meg egy várost.");
                return;
            }
            if (string.IsNullOrWhiteSpace(street))
            {
                MessageBox.Show("Adj meg egy utcát.");
                return;
            }
            if (string.IsNullOrWhiteSpace(other))
            {
                MessageBox.Show("Add meg a szállítási cím kiegészítését is.");
                return;
            }
            if (string.IsNullOrWhiteSpace(userid))
            {
                MessageBox.Show("Adj meg egy fiókot a rendeléshez.");
                return;
            }

            var ok = await _api.UpdateOrder(selected.Id, name,email,phone,postalcode,city,street,other,userid);
            if (!ok)
            {
                MessageBox.Show("Nem sikerült módosítani.");
                return;
            }

            await LoadOrders();
        }

        private async void OrderDelete_Click(object sender, RoutedEventArgs e)
        {
            if (OrderGrid.SelectedItem is not OrderDto selected)
            {
                MessageBox.Show("Válassz ki egy rendelést.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Biztos törlöd? (Id={selected.Id}, Name={selected.CustomerName})",
                "Törlés",
                MessageBoxButton.YesNo
            );

            if (confirm != MessageBoxResult.Yes)
                return;

            var ok = await _api.DeleteOrder(selected.Id);
            if (!ok)
            {
                MessageBox.Show("Nem sikerült törölni.");
                return;
            }

            OrderNameTextBox.Clear();
            OrderEmailTextBox.Clear();
            OrderPhoneTextBox.Clear();
            OrderPostalCodeTextBox.Clear();
            OrderCityTextBox.Clear();
            OrderStreetTextBox.Clear();
            OrderOtherTextBox.Clear();
            OrderUserIdTextBox.Clear();
            await LoadOrders();
        }


        //orderitem

        private void OrderItemGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrderItemGrid.SelectedItem is OrderItemDto selected)
            {
                OrderItemItemPriceTextBox.Clear();
                OrderItemPieceTextBox.Clear();
            }
        }

        private async void OrderItemUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (OrderItemGrid.SelectedItem is not OrderItemDto selected)
            {
                MessageBox.Show("Válassz ki egy rendelés terméket.");
                return;
            }

            var itempiece = OrderItemPieceTextBox.Text.Trim();
            var itemprice = OrderItemItemPriceTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(itempiece))
            {
                MessageBox.Show("Adj meg egy db-ot.");
                return;
            }
            if (string.IsNullOrWhiteSpace(itemprice))
            {
                MessageBox.Show("Adj meg egy árat.");
                return;
            }
            

            var ok = await _api.UpdateOrderItem(selected.Id,Convert.ToInt32( itemprice),Convert.ToInt32( itempiece));
            if (!ok)
            {
                MessageBox.Show("Nem sikerült módosítani.");
                return;
            }

            await LoadOrderItems();
        }

        private async void OrderItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if (OrderItemGrid.SelectedItem is not OrderItemDto selected)
            {
                MessageBox.Show("Válassz ki egy rendelés terméket.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Biztos törlöd? (Id={selected.Id}, Name={selected.Order_Id})",
                "Törlés",
                MessageBoxButton.YesNo
            );

            if (confirm != MessageBoxResult.Yes)
                return;

            var ok = await _api.DeleteOrderItem(selected.Id);
            if (!ok)
            {
                MessageBox.Show("Nem sikerült törölni.");
                return;
            }

            OrderItemPieceTextBox.Clear();
            OrderItemItemPriceTextBox.Clear();
            await LoadOrderItems();
        }

        private async void OrderItemReload_Click(object sender, RoutedEventArgs e)
        {
            await LoadOrderItems();
        }


        //globalsettings
        

        private async void GlobalUpdate_Click(object sender, RoutedEventArgs e)
        {
            var contactemail = GlobalContactEmailTextBox.Text.Trim();
            var globaldelivery = GlobalDeliveryTextBox.Text.Trim();
            var facebook = GlobalFacebookTextBox.Text.Trim();
            var instagram = GlobalInstagramTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(contactemail))
            {
                MessageBox.Show("Adj meg egy kapcsolat tartási email-t.");
                return;
            }
            if (string.IsNullOrWhiteSpace(globaldelivery))
            {
                MessageBox.Show("Adj meg egy becsült szállítási időt.");
                return;
            }
            if (string.IsNullOrWhiteSpace(facebook))
            {
                MessageBox.Show("Adj meg egy facebook url-t.");
                return;
            }
            if (string.IsNullOrWhiteSpace(instagram))
            {
                MessageBox.Show("Adj meg egy instagram url-t.");
                return;
            }


            var result = await _api.UpdateGlobalSettings(2, contactemail, globaldelivery, facebook, instagram);

            if (!result)
            {
                MessageBox.Show("Nem sikerült módosítani.");
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
    }
}
