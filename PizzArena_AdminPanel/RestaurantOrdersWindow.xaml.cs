using PizzArena_AdminPanel.API;
using PizzArena_AdminPanel.API.Order;
using PizzArena_AdminPanel.API.Restaurant;
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
    /// Interaction logic for RestaurantOrdersWindow.xaml
    /// </summary>
    public partial class RestaurantOrdersWindow : Window
    {
        private readonly ApiService _api;
        private readonly RestaurantDto _restaurant;
        private ObservableCollection<OrderDto> _orders = new();

        public RestaurantOrdersWindow(ApiService api, RestaurantDto restaurant)
        {
            InitializeComponent();
            _api = api;
            _restaurant = restaurant;
            this.Title = $"Rendelések - {restaurant.name}";

            OrdersGrid.ItemsSource = _orders;
            LoadOrders();
        }

        private async void LoadOrders()
        {
            var allOrders = await _api.GetAllOrder();
            _orders.Clear();
            foreach (var o in allOrders.Where(x => x.RestaurantId == _restaurant.Id))
            {
                _orders.Add(o);
            }
        }

        private async void OrdersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is OrderDto selectedOrder)
            {
                CustomerInfoLabel.Text = $"{selectedOrder.CustomerName} ({selectedOrder.CustomerPhone})";
                AddressLabel.Text = $"{selectedOrder.PostalCode} {selectedOrder.City}, {selectedOrder.Street}";

                var items = await _api.GetOrderItemsByOrderId(selectedOrder.Id);
                OrderItemsGrid.ItemsSource = items;
            }
        }

        private void ReloadOrders_Click(object sender, RoutedEventArgs e) => LoadOrders();

        private async void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            // 1. Ellenőrizzük, hogy van-e kijelölt elem a bal oldali rácsban
            if (OrdersGrid.SelectedItem is not OrderDto selectedOrder)
            {
                MessageBox.Show("Kérlek, válassz ki egy rendelést a törléshez!");
                return;
            }

            // 2. Megerősítő kérdés a felhasználónak
            var result = MessageBox.Show(
                $"Biztosan törölni szeretnéd a(z) {selectedOrder.Id}. számú rendelést ({selectedOrder.CustomerName})?",
                "Törlés megerősítése",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // 3. API hívás (a már meglévő DeleteOrder metódusoddal)
                var success = await _api.DeleteOrder(selectedOrder.Id);

                if (success)
                {
                    MessageBox.Show("Rendelés sikeresen törölve.");

                    // 4. UI Takarítás: ürítjük a részletező mezőket és a tételeket
                    CustomerInfoLabel.Text = "Válasszon rendelést!";
                    AddressLabel.Text = "";
                    OrderItemsGrid.ItemsSource = null;
                    StatusComboBox.SelectedIndex = -1;

                    // 5. Lista frissítése
                    LoadOrders();
                }
                else
                {
                    // Ha hiba történik (pl. adatbázis kényszer miatt nem törölhető)
                    MessageBox.Show("Hiba történt a törlés során. Valószínűleg a rendeléshez még tartoznak tételek az adatbázisban.");
                }
            }
        }

        private async void UpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is not OrderDto selectedOrder) return;

            if (StatusComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                if (int.TryParse(selectedItem.Tag?.ToString(), out int statusValue))
                {
                    var success = await _api.UpdateOrderStatus(selectedOrder.Id, statusValue);

                    if (success)
                    {
                        MessageBox.Show("Rendelés állapota sikeresen frissítve!");
                        LoadOrders(); 
                    }
                    else
                    {
                        MessageBox.Show("Hiba történt a státusz frissítésekor.");
                    }
                }
            }
        }
    }
        
    
}
