using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PizzArena_AdminPanel.API.OrderItem
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ItemPrice { get; set; }
        public int Piece { get; set; }
        public int Order_Id { get; set; }
        public int? Item_Id { get; set; }
        public string ItemName { get; set; }

    }
}
