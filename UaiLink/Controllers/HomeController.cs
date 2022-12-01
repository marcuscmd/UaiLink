using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UaiLink.Filters;
using UaiLink.Helper;
using UaiLink.Models;

namespace UaiLink.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public Context context;
        private List<UsuarioDto> result;
        private readonly Isessao _isessao;

        public HomeController(ILogger<HomeController> logger,Isessao sessao, Context ctx)
        {
            _logger = logger;
            _isessao = sessao;
            context = ctx;
        }

        public async Task<IActionResult> Index(string tag, string curso, string nome)
        {
            var sucesso = await Consulta(tag,curso,nome);

            var Logado = _isessao.BuscarSessaoDoUsuario();

            var Loga = Logado == null ? "Deslogado" : "Logado";

            ViewBag.Nome = Loga;
            ViewBag.IdUsuario = Logado == null ? 0 : Logado.UsuarioID;
            ViewBag.TipoUsuario = Logado == null ? "" : Logado.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno ? "Aluno" : "Visitante";


            foreach (var item in result)
            {
                var tags = await context.tags.Where(w => w.UsuarioID == item.UsuarioID).ToListAsync();

                item.Tag = tags;

                item.NomeTags = String.Join(" , ", tags.Select(s=> s.TagName));

            }

            return View(result);
        }

        public IActionResult DetalhesPerfil(int id)
        {
            var anexos = new List<AnexoDto>();

            var usuario = context.Usuarios.Find(id);

            var tags = context.tags.Where(w => w.UsuarioID == usuario.UsuarioID).ToList();

            var projetos = context.Projetos.Where(w => w.UsuarioID == usuario.UsuarioID).ToList();

            foreach (var item in projetos)
            {
                anexos = context.Anexos.Where(w => w.ProjetoID == item.ProjetoID).ToList();

                item.Anexo = anexos.FirstOrDefault();

                item.Anexo.NomeAnexo = item.Anexo.NomeAnexo == "" ? "Sem Anexo" : item.Anexo.NomeAnexo;
            }

            usuario.NomeTags = String.Join(" , ", tags.Select(s => s.TagName));

            usuario.Tag = tags.Count > 0 ? tags : new List<TagDto>();

            usuario.Projetos= projetos.Count > 0 ? projetos : new List<ProjetoDto>();

            var Logado = _isessao.BuscarSessaoDoUsuario();

            if (Logado != null)
            {
                var Loga = Logado == null ? "Deslogado" : "Logado";
                ViewBag.Nome = Loga;

            }

            return View(usuario);
        }

        public async Task<bool> Consulta(string tag, string curso, string nome)
        {
            result = new List<UsuarioDto>();

            try
            {
                List<int> resultIDTag = new List<int>();
                //quando for por tag ja deixa padrao para nao precisar pesquisa denovo
                if (!string.IsNullOrEmpty(tag))
                {
                    resultIDTag = await context.tags.Where(w => w.TagName.ToUpper() == tag.ToUpper()).Select(s => s.UsuarioID).ToListAsync();
                }

                if (!string.IsNullOrEmpty(tag) && string.IsNullOrEmpty(curso) && string.IsNullOrEmpty(nome))
                {
                    result = await context.Usuarios.Where(w => w.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno && resultIDTag.Contains(w.UsuarioID)).ToListAsync();
                }
                if (!string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(curso) && string.IsNullOrEmpty(nome))
                {
                    result = await context.Usuarios.Where(w => w.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno && resultIDTag.Contains(w.UsuarioID) && w.Curso.ToUpper() == curso.ToUpper()).ToListAsync();
                }
                if (!string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(curso) && !string.IsNullOrEmpty(nome))
                {
                    result = await context.Usuarios.Where(w => w.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno && resultIDTag.Contains(w.UsuarioID) && w.Curso.ToUpper() == curso.ToUpper() && w.Nome.ToUpper() == nome.ToUpper()).ToListAsync();
                }
                if (string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(curso) && string.IsNullOrEmpty(nome))
                {
                    result = await context.Usuarios.Where(w => w.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno && w.Curso.ToUpper() == curso.ToUpper()).ToListAsync();
                }
                if (string.IsNullOrEmpty(tag) && string.IsNullOrEmpty(curso) && !string.IsNullOrEmpty(nome))
                {
                    result = await context.Usuarios.Where(w => w.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno && w.Nome.ToUpper() == nome.ToUpper()).ToListAsync();
                }
                if (string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(curso) && !string.IsNullOrEmpty(nome))
                {
                    result = await context.Usuarios.Where(w => w.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno && w.Nome.ToUpper() == nome.ToUpper() && w.Curso.ToUpper() == curso.ToUpper()).ToListAsync();
                }
                if (!string.IsNullOrEmpty(tag) && string.IsNullOrEmpty(curso) && !string.IsNullOrEmpty(nome))
                {
                    result = await context.Usuarios.Where(w => w.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno && resultIDTag.Contains(w.UsuarioID) && w.Nome.ToUpper() == nome.ToUpper()).ToListAsync();
                }
                if (string.IsNullOrEmpty(tag) && string.IsNullOrEmpty(curso) && string.IsNullOrEmpty(nome))
                {
                    result = await context.Usuarios.Where(w => w.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno).ToListAsync();
                }
                return true;

            }
            catch (Exception e)
            {
                return false;
                throw;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
