using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzArena_AdminPanel.API.Restaurant
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
        public string openingHours { get; set; }
        public string address { get; set; }
    }
}
