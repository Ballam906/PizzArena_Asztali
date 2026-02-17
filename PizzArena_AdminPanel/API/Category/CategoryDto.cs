using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzArena_AdminPanel.API.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public object? Products { get; set; }
    }
}
