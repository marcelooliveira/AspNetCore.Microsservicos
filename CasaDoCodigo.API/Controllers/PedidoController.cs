//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using CasaDoCodigo.Models;
//using CasaDoCodigo.Repositories;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace CasaDoCodigo.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class PedidoController : ControllerBase
//    {
//        private readonly IPedidoRepository pedidoRepository;

//        public PedidoController(IPedidoRepository pedidoRepository)
//        {
//            this.pedidoRepository = pedidoRepository;
//        }

//        // GET: api/Pedido
//        [HttpGet]
//        public IEnumerable<string> Get()
//        {
//            return new string[] { "value1", "value2" };
//        }

//        // GET: api/Pedido/5
//        [HttpGet("{id}", Name = "Get")]
//        public async Task<Pedido> Get(int id)
//        {
//            return await pedidoRepository.GetPedido();
//        }

//        // POST: api/Pedido
//        [HttpPost]
//        public void Post([FromBody] string value)
//        {
//        }

//        // PUT: api/Pedido/5
//        [HttpPut("{id}")]
//        public void Put(int id, [FromBody] string value)
//        {
//        }

//        // DELETE: api/ApiWithActions/5
//        [HttpDelete("{id}")]
//        public void Delete(int id)
//        {
//        }
//    }
//}
