using ArteFormas.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ArteFormas.ViewModels
{
    public class PedidoViewModel
    {
        public Pedido Pedido { get; set; } = new();
        public List<ItemPedido> ItensDoPedido { get; set; } = new();
        public SelectList? ListaDeClientes { get; set; }
        public SelectList? ListaDeMoveis { get; set; }

    }
}
