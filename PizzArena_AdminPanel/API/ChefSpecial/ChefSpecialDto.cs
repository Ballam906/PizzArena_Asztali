using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzArena_AdminPanel.API.ChefSpecial
{
    public class ChefSpecialDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string CustomNote { get; set; }
    }
}
