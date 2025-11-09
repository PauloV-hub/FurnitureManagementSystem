using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArteFormas.Data;
using ArteFormas.Models;
using ArteFormas.ViewModels;


namespace ArteFormas.Controllers
{
    public class PedidoesController : Controller
    {
        private readonly ArteFormasContext _context;

        public PedidoesController(ArteFormasContext context)
        {
            _context = context;
        }

        // GET: Pedidoes
        public async Task<IActionResult> Index()
        {
            var arteFormasContext = _context.Pedido.
                Include(p => p.Cliente).Include(p=> p.ItensDoPedido).
                ThenInclude(item => item.Movel);

            return View(await arteFormasContext.ToListAsync());
        }
        //POST : Pedidoes/MaracarComoEntregue/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarComoEntregue(int id)
        {
            var pedido = await _context.Pedido.FindAsync(id);
            if(pedido == null)
            {
                return NotFound();
            }
            pedido.Status = "Entregue";
            _context.Update(pedido);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        //GET : Pedidoes/GetPrecoMoveis
        [HttpGet]
        public async Task<IActionResult> GetPrecoMovel(int id)
        {
            var movel = await _context.Movel.FindAsync(id);
            if (movel == null)
            {
                return NotFound();
            }

            return Json(new { preco = movel.Preco });
        }

        // GET : Pedidoes/BuscarPorCliente
        public async Task<IActionResult> BuscarPorCliente(string nomeCliente)
        {
            var pedidos = _context.Pedido.
                Include(p=> p.Cliente).
                Include(p => p.ItensDoPedido).
                OrderByDescending(p => p.DataPedido)
                .AsQueryable();

            if (!string.IsNullOrEmpty(nomeCliente))
            {
                pedidos = pedidos.Where(p => p.Cliente.Nome.ToLower().Contains(nomeCliente.ToLower()));
                 
            }
            var pedidosResultantes = await pedidos.ToListAsync();
            return View("Index",pedidosResultantes);
        }

        //POST : Pedidoes/MaracarComoPendente/5
        public async Task<IActionResult> MarcarComoPendente(int id)
        {
            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            pedido.Status = "Pendente";
            _context.Update(pedido);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Pedidoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(p => p.Cliente).
                Include(p=> p.ItensDoPedido).
                ThenInclude(item=> item.Movel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // GET: Pedidoes/Create
        public IActionResult Create()
        {
            var viewModel = new PedidoViewModel
            {
                ListaDeClientes = new SelectList(_context.Cliente, "Id", "Nome"),
                ListaDeMoveis = new SelectList(_context.Movel, "Id", "Nome"),
            };
            return View(viewModel);
        }

        // POST: Pedidoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PedidoViewModel viewModel, string ClienteNome, string ClienteEndereco, string ClienteTelefone)
        {
            ModelState.Remove("Pedido.Cliente");
            foreach(var key in ModelState.Keys.Where(k=> k.StartsWith("ItensDoPedido[")))
            {
                ModelState.Remove(key);
            }

            if (ModelState.IsValid)
            {
                var pedido = viewModel.Pedido;

                pedido.ItensDoPedido = viewModel.ItensDoPedido;
                if(pedido.ClienteId == 0)
                {
                    var novoCliente = new Cliente
                    {
                        Nome = ClienteNome,
                        Endereco = ClienteEndereco,
                        Telefone = ClienteTelefone,
                    };
                    pedido.Cliente = novoCliente;
                }
                pedido.ItensDoPedido = viewModel.ItensDoPedido;

                foreach(var item in pedido.ItensDoPedido)
                {
                    var movel = await _context.Movel.FindAsync(item.MovelId);
                    if(movel != null)
                    {
                        item.PrecoUnitario = movel.Preco;
                    }
                }
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            viewModel.ListaDeClientes = new SelectList(_context.Cliente, "Id", "Nome", viewModel.Pedido.ClienteId);
            viewModel.ListaDeMoveis = new SelectList(_context.Movel, "Id", "Nome");
            return View(viewModel);
        }

        // GET: Pedidoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(p=>p.ItensDoPedido).
                ThenInclude(item=>item.Movel).
                Include(p => p.Cliente).FirstOrDefaultAsync(p=> p.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }
            var viewModel = new PedidoViewModel
            {
                Pedido = pedido,
                ItensDoPedido = pedido.ItensDoPedido ?? new List<ItemPedido>(),
                ListaDeClientes = new SelectList(_context.Cliente, "Id", "Nome", pedido.ClienteId),
                ListaDeMoveis = new SelectList(_context.Movel, "Id", "Nome")
            };
            return View(viewModel);
        }

        /// POST: Pedidoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PedidoViewModel viewModel)
        {
            if (id != viewModel.Pedido.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Pedido.Cliente");
            ModelState.Remove("Pedido.Cliente.Nome");
            ModelState.Remove("Pedido.ItensDoPedido");

            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ItensDoPedido[")).ToList())
            {
                ModelState.Remove(key);
            }

            // DEBUG: Ver se há erros no ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                // Adicione um breakpoint aqui ou veja os erros
                System.Diagnostics.Debug.WriteLine("Erros de validação:");
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine($"- {error}");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var pedidoNoBanco = await _context.Pedido
                        .Include(p => p.ItensDoPedido)
                        .Include(p => p.Cliente)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (pedidoNoBanco == null)
                    {
                        return NotFound();
                    }

                    // 1. Atualizar dados do pedido (exceto Cliente e ItensDoPedido)
                    pedidoNoBanco.DataEntrega = viewModel.Pedido.DataEntrega;
                    pedidoNoBanco.Status = viewModel.Pedido.Status;
                    pedidoNoBanco.InformacoesPedido = viewModel.Pedido.InformacoesPedido;

                    // 2. Atualizar dados do cliente
                    if (pedidoNoBanco.Cliente != null)
                    {
                        pedidoNoBanco.Cliente.Telefone = viewModel.Pedido.Cliente?.Telefone ?? pedidoNoBanco.Cliente.Telefone;
                        pedidoNoBanco.Cliente.Endereco = viewModel.Pedido.Cliente?.Endereco ?? pedidoNoBanco.Cliente.Endereco;
                    }

                    // 3. Remover todos os itens antigos
                    _context.ItemPedido.RemoveRange(pedidoNoBanco.ItensDoPedido);
                    pedidoNoBanco.ItensDoPedido.Clear();

                    // 4. Adicionar os novos itens do formulário
                    if (viewModel.ItensDoPedido != null && viewModel.ItensDoPedido.Any())
                    {
                        foreach (var itemViewModel in viewModel.ItensDoPedido)
                        {
                            if (itemViewModel.MovelId > 0 && itemViewModel.Quantidade > 0)
                            {
                                var movel = await _context.Movel.FindAsync(itemViewModel.MovelId);
                                if (movel != null)
                                {
                                    var novoItem = new ItemPedido
                                    {
                                        MovelId = itemViewModel.MovelId,
                                        Quantidade = itemViewModel.Quantidade,
                                        PrecoUnitario = movel.Preco,
                                        PedidoId = pedidoNoBanco.Id
                                    };
                                    pedidoNoBanco.ItensDoPedido.Add(novoItem);
                                }
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(viewModel.Pedido.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    // Log do erro para debug
                    System.Diagnostics.Debug.WriteLine($"Erro ao salvar: {ex.Message}");
                    ModelState.AddModelError("", $"Erro ao salvar as alterações: {ex.Message}");
                }
            }

            if (viewModel.ItensDoPedido != null)
            {
                foreach (var item in viewModel.ItensDoPedido)
                {
                    if (item.MovelId > 0)
                    {
                        item.Movel = await _context.Movel.FindAsync(item.MovelId);
                    }
                }
            }
            viewModel.ListaDeClientes = new SelectList(_context.Cliente, "Id", "Nome", viewModel.Pedido.ClienteId);
            viewModel.ListaDeMoveis = new SelectList(_context.Movel, "Id", "Nome");

            return View(viewModel);
        }

        // GET: Pedidoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(p => p.Cliente)
                .Include(p => p.ItensDoPedido).
                ThenInclude(item => item.Movel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }
        
        // POST: Pedidoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedido
                .Include(p => p.ItensDoPedido)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido != null)
            {
                _context.ItemPedido.RemoveRange(pedido.ItensDoPedido);
                _context.Pedido.Remove(pedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedido.Any(e => e.Id == id);
        }
    }
}
