using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UaiLink.Filters;
using UaiLink.Helper;
using UaiLink.Models;

namespace UaiLink.Controllers
{
    public class UsuarioController : Controller
    {
        public Context context;
        private readonly Isessao _isessao;
        public List<TagDto> ListaTag = new List<TagDto>();
        private readonly IWebHostEnvironment _appEnvironment;

        public UsuarioController(Context ctx, Isessao sessao, IWebHostEnvironment env)
        {
            context = ctx;
            _isessao = sessao;
            _appEnvironment = env;
        }

        [PaginaParaUsuarioLogado]
        public IActionResult Index()
        {
            return View(context.Usuarios);
        }

        [PaginaParaUsuarioLogado]
        public IActionResult deslogar()
        {
            _isessao.RemoverSessaoUsuario();

            return RedirectToAction("index", "Usuario");
        }

        public IActionResult Logar()
        {
            if (_isessao.BuscarSessaoDoUsuario() != null)
            {
                return RedirectToAction("index", "Home");
            }
            ViewBag.UsuarioId = new SelectList(context.Usuarios.OrderBy(u => u.Nome));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logar(UsuarioDto usuario)
        {

            if (await verificarLogin(usuario))
            {
                var result = await context.Usuarios.Where(w => (w.Cpf.Contains(usuario.Cpf) || w.Email.ToUpper().Contains(usuario.Email.ToUpper()))).ToListAsync();

                _isessao.CriarSessaoDoUsuario(result[0]);

                return RedirectToAction("index", "Home");
            }
            else
            {
                TempData["Login"] = "Login Invalido";
            }

            return View(usuario);
        }
        [PaginaParaUsuarioLogado]
        public IActionResult Sair()
        {
            _isessao.RemoverSessaoUsuario();

            return RedirectToAction("Logar", "Usuario");
        }

        public IActionResult Create()
        {
            ViewBag.UsuarioId = new SelectList(context.Usuarios.OrderBy(u => u.Nome));
            return View();
        }


        //Controle de Usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioDto usuario)
        {
            try
            {
                if (usuario.TipoUsuario == Models.Enum.TipoUsuarioEnum.Visitante)
                {
                    if (await verificarUsuario(usuario))
                    {
                        context.Add(usuario);
                        context.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Erro"] = "Já Existe um usuario com esse Cpf ou Email";
                    }
                }
                else
                    if (ModelState.IsValid)
                    {
                        if (await verificarUsuario(usuario))
                        {
                            context.Add(usuario);
                            context.SaveChanges();
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["Erro"] = "Já Existe um usuario com esse Cpf ou Email";
                        }
                    }

                return View(usuario);
            }
            catch (Exception x)
            {
                TempData["MensagemErro"] = $"Teste, {x.Message}";
                return RedirectToAction("Index", "Home");
            }
        }
        [PaginaParaUsuarioLogado]
        public IActionResult Details(int id)
        {
            var Logado = _isessao.BuscarSessaoDoUsuario();

            var Loga = Logado == null ? "Deslogado" : "Logado";

            ViewBag.Nome = Loga;
            ViewBag.TipoUsuario = Logado == null ? "" : Logado.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno ? "Aluno" : "Visitante";

            if (id == 0)
            {
                id = Logado == null ? 0 : Logado.UsuarioID;
            }

            var usuario = context.Usuarios
                .FirstOrDefault(p => p.UsuarioID == id);
            
           var tags = context.tags.Where(w => w.UsuarioID == usuario.UsuarioID).ToList();

            usuario.Tag = tags;

            return View(usuario);
        }
        [PaginaParaUsuarioLogado]
        public IActionResult DetailsVisitante(int id)
        {
            var Logado = _isessao.BuscarSessaoDoUsuario();

            var Loga = Logado == null ? "Deslogado" : "Logado";

            ViewBag.Nome = Loga;
            ViewBag.TipoUsuario = Logado == null ? "" : Logado.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno ? "Aluno" : "Visitante";

            if (id == 0)
            {
                id = Logado == null ? 0 : Logado.UsuarioID;
            }

            var usuario = context.Usuarios
                .FirstOrDefault(p => p.UsuarioID == id);

            return View(usuario);
        }
        [PaginaParaUsuarioLogado]
        public IActionResult EditVisitante(int id)
        {
            var Logado = _isessao.BuscarSessaoDoUsuario();

            var Loga = Logado == null ? "Deslogado" : "Logado";

            ViewBag.Nome = Loga;
            ViewBag.TipoUsuario = Logado == null ? "" : Logado.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno ? "Aluno" : "Visitante";

            if (id == 0)
            {
                id = Logado == null ? 0 : Logado.UsuarioID;
            }

            var usuario = context.Usuarios.Find(id);

            return View(usuario);
        }
        [PaginaParaUsuarioLogado]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditVisitante(UsuarioDto usuario)
        {
            context.Entry(usuario).State = EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction($"details", new { id = usuario.UsuarioID });
        }
        [PaginaParaUsuarioLogado]
        public IActionResult Edit(int id)
        {
            var Logado = _isessao.BuscarSessaoDoUsuario();

            var Loga = Logado == null ? "Deslogado" : "Logado";

            ViewBag.Nome = Loga;
            ViewBag.TipoUsuario = Logado == null ? "" : Logado.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno ? "Aluno" : "Visitante";

            if (id == 0)
            {
                id = Logado == null ? 0 : Logado.UsuarioID;
            }

            var usuario = context.Usuarios.Find(id);

            var tags = context.tags.Where(w=> w.UsuarioID == usuario.UsuarioID).ToList();

            usuario.Tag = tags;

            Global.ExisteFoto = usuario.UrlFoto;

            return View(usuario);
        }
        [PaginaParaUsuarioLogado]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UsuarioDto usuario)
        {
            var tags = context.tags.Where(w => w.UsuarioID == usuario.UsuarioID).ToList();

            usuario.Tag = tags;

            if (Global.Diretorio != null)
                usuario.UrlFoto = Global.Diretorio == Global.ExisteFoto? Global.ExisteFoto : Global.Diretorio;
            else
                usuario.UrlFoto = Global.ExisteFoto == null ? null : Global.ExisteFoto;

            context.Entry(usuario).State = EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction($"details", new { id = usuario.UsuarioID });
        }
        [PaginaParaUsuarioLogado]
        [HttpPost]
        public IActionResult DeletarUsuario(string id)
        {
            var resultUsuario = context.Usuarios.Where(w=>w.UsuarioID == Convert.ToInt32(id)).ToList();
            var resultTags = context.tags.Where(w => w.UsuarioID == Convert.ToInt32(id)).ToList();
            var resultProjetos = context.Projetos.Where(w => w.UsuarioID == Convert.ToInt32(id)).ToList();

            foreach (var item in resultTags)
            {
                context.Remove(item);
            }
            foreach (var item in resultProjetos)
            {
                context.Remove(item);
            }
            context.Remove(resultUsuario.FirstOrDefault());
            context.SaveChanges();

            deslogar();

            return RedirectToAction("Index");
        }

        private async Task<bool> verificarUsuario(UsuarioDto usuario)
        {
            if (usuario == null) return false;

            var result = await context.Usuarios.Where(w => (w.Cpf.Contains(usuario.Cpf) || w.Email.ToUpper().Contains(usuario.Email.ToUpper()))).ToListAsync();

            if (result.Count >= 1)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> verificarLogin(UsuarioDto usuario)
        {
            if (usuario == null) return false;

            var result = await context.Usuarios.Where(w => (w.Email.Contains(usuario.Email) && w.Senha.Contains(usuario.Senha))).ToListAsync();

            if (result.Count >= 1)
            {
                return true;
            }

            return false;
        }

        [HttpPost]
        public JsonResult GetDados(string modelo)
        {
            List<TagDto> json = JsonConvert.DeserializeObject<List<TagDto>>(modelo);

            try
            {
                if (json.Count > 0)
                {
                    var Tagdelete = context.tags.Where(w => w.UsuarioID == json.Select(s => s.UsuarioID).First()).ToList();
                    foreach (var item in Tagdelete)
                    {
                        context.Remove(item);
                        context.SaveChanges();
                    }

                    foreach (var tag in json)
                    {
                        context.Add(new TagDto()
                        {
                            TagId = 0,
                            TagName = tag.TagName,
                            UsuarioID = tag.UsuarioID
                        });
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {}
            return Json(json);
        }

        [HttpPost]
        public async Task<IActionResult> GetFotoPerfil(IFormFile fileUpload)
        {
            UsuarioDto projeto = new UsuarioDto();

            try
            {
                var BuscarUsuario = _isessao.BuscarSessaoDoUsuario();

                if (fileUpload != null)
                {
                    if (BuscarUsuario != null)
                    {
                        string pasta = "Arquivos_Usuario";

                        string nomeArquivo = fileUpload.FileName;

                        string caminho_WebRoot = _appEnvironment.WebRootPath;

                        string filePath = Path.Combine(caminho_WebRoot, pasta);

                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }

                        filePath = Path.Combine(filePath, nomeArquivo);

                        FileInfo file = new FileInfo(filePath);

                        if (file.Exists)
                        {
                            file.Delete();
                        }

                        using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
                        {
                            await fileUpload.CopyToAsync(stream);
                        }

                        Global.Diretorio = $"/Arquivos_Usuario/{nomeArquivo}";

                    }
                }
            }
            catch (Exception)
            {
            }
            return View(projeto);
        }
    }
}
