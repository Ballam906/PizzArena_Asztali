using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzArena_AdminPanel.API.Enums
{
    public static class EnumExtension
    {
        public static string ToHungarianString(this OrderStatus status)
        {
            return status switch
            {
                OrderStatus.New => "Új",
                OrderStatus.Processing => "Függőben",
                OrderStatus.Shipped => "Kiszállítás alatt",
                OrderStatus.Delivered => "Kiszállítva",
                OrderStatus.Cancelled => "Törölve",
                _ => "Ismeretlen"
            };
        }
    }
}
