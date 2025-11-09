using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace ArteFormas.Models
{
    public class Movel 
    {
        public int Id { get; set; }
        [Display(Name ="Nome")]
        [Required(ErrorMessage ="O nome é obrigatório")]
        [StringLength(100,ErrorMessage ="O nome não pode ter mais de 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        [Display(Name ="Descrição")]
        [StringLength(500,ErrorMessage ="A descrição não pode ter mais de 500 caracteres")]
        public string Descricao { get; set; } = string.Empty;
        [Display(Name ="Preço")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 999999.99, ErrorMessage ="O preço deve ser maior que zero")]
        [Required(ErrorMessage ="O preço é obrigatório")]
        public decimal Preco { get; set; }
        [Display(Name ="Material")]
        [StringLength(100,ErrorMessage ="O material não pode ter mais de 100 caracteres")]
        public string Material { get; set; } = string.Empty;
    }
    
    public class Cliente
    {
        public int Id { get; set; }
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, ErrorMessage = "O nome não pode ter mais de 200 caracteres")]
        public string Nome { get; set; } = string.Empty;
        [Display(Name = "Endereço")]
        [StringLength(300, ErrorMessage = "O endereço não pode ter mais de 300 caracteres")]
        public string Endereco { get; set; } = string.Empty;
        [Display(Name = "Telefone")]
        [Phone(ErrorMessage ="Telefone inválido")]
        [StringLength(20, ErrorMessage = "O telefone não pode ter mais de 20 caracteres")]
        public string Telefone { get; set; } = string.Empty;
    }
    
    public class Pedido
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;
        public List<ItemPedido> ItensDoPedido { get; set; } = new();

        [DataType(DataType.Date)]
        [Display(Name = "Data do Pedido")]
        public DateTime DataPedido { get; set; } = DateTime.Today;
        
        [DataType(DataType.Date)]
        [Display(Name = "Data de Entrega")]
        public DateTime DataEntrega { get; set; } = DateTime.Today.AddDays(7);

        [Display(Name = "Status")]
        [StringLength(50, ErrorMessage = "O status não pode ter mais de 50 caracteres")]
        public string Status { get; set; } = "Pendente";

        [Display(Name = "Informações do Pedido")]
        [StringLength(500, ErrorMessage = "As informações não podem ter mais de 500 caracteres")]
        public string InformacoesPedido { get; set; } = string.Empty;

        [NotMapped]
        [Display(Name = "Total do Pedido")]
        public decimal TotalPedido => ItensDoPedido?.Sum(i => i.Subtotal) ?? 0;

        public bool ValidarDatas()
        {
            if (DataPedido.Date < DateTime.Today)
                return false;
            if (DataEntrega.Date < DateTime.Today)
                return false;
            if (DataEntrega.Date < DataPedido.Date)
                return false;
            return true;
        }
    }
    
    public class ItemPedido
    {
        public int Id { get; set; }
        
        [Display(Name ="Quantidade")]
        [Range(1, 1000, ErrorMessage = "A quantidade deve ser entre 1 e 1000")]
        [Required(ErrorMessage = "A quantidade é obrigatória")]
        public int Quantidade { get; set; }
        
        [Display(Name = "Preço Unitário")]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, 999999.99, ErrorMessage = "O preço deve ser maior que zero")]
        public decimal PrecoUnitario { get; set; }
        
        [NotMapped]
        [Display(Name = "Subtotal")]
        public decimal Subtotal => Quantidade * PrecoUnitario;
        
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; } = null!;
        
        public int MovelId { get; set; }
        public Movel Movel { get; set; } = null!;
    }
}
