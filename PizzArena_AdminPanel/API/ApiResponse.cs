using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzArena_AdminPanel.API
{
    public class ApiResponse<T>
    {
        public T? Result { get; set; }
        public string? Message { get; set; }
    }
}
