using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ArteFormas.Models;

namespace ArteFormas.Data
{
    public class ArteFormasContext : DbContext
    {
        public ArteFormasContext (DbContextOptions<ArteFormasContext> options)
            : base(options)
        {
        }

        public DbSet<ArteFormas.Models.Movel> Movel { get; set; } = default!;
        public DbSet<ArteFormas.Models.Cliente> Cliente { get; set; } = default!;
        public DbSet<ArteFormas.Models.Pedido> Pedido { get; set; } = default!;
        public DbSet<ItemPedido> ItemPedido { get; set; }
    }
}
