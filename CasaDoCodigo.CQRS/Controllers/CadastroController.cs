using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Controllers
{
    public class CadastroController : Controller
    {
        //public async Task<IActionResult> Cadastro()
        //{
        //    try
        //    {
        //        int pedidoId = GetPedidoId() ?? throw new ArgumentNullException("pedidoId");
        //        PedidoViewModel pedido = await apiService.GetPedido(pedidoId);

        //        if (pedido == null)
        //        {
        //            return RedirectToAction("Carrossel");
        //        }

        //        return View(pedido.Cadastro);
        //    }
        //    catch (BrokenCircuitException)
        //    {
        //        HandleBrokenCircuitException();
        //    }
        //    catch (Exception e)
        //    {
        //        HandleBrokenCircuitException();
        //    }
        //    return View();
        //}
    }
}
