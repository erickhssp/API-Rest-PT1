using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Alura.WebAPI.WebApp.Api
{
    [ApiController]     //todo controlador de uma APi deve ter a anotação ApiController7//
    [Route("[controller]")]
    public class LivrosController : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public LivrosController(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        [HttpGet]
        public IActionResult ListaDeLivros()
        {
            var lista = _repo.All.Select(l => l.ToModel()).ToList();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public IActionResult Recuperar(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }

            return Ok(model.ToModel());

        }

        [HttpPost]
        public IActionResult Incluir([FromBody]LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                _repo.Incluir(livro);
                var url = Url.Action("Recuperar", new { id = livro.Id });
                return Created(url, livro);
            }
            return BadRequest();    // código 201
        }

        [HttpPut]
        public IActionResult Alterar([FromBody]LivroUpload model) // [FromBody] Garante a requisição vindo do corpo.
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if (model.Capa == null)
                {
                    livro.ImagemCapa = _repo.All
                        .Where(l => l.Id == livro.Id)
                        .Select(l => l.ImagemCapa)
                        .FirstOrDefault();
                }
                _repo.Alterar(livro);
                return Ok();    
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public IActionResult Remover(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            _repo.Excluir(model);
            return NoContent(); //código 204
        }
    }
}