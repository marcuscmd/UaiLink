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
using System.Net;
using System.Threading.Tasks;
using UaiLink.Filters;
using UaiLink.Helper;
using UaiLink.Models;

namespace UaiLink.Controllerss
{
    [PaginaParaUsuarioLogado]
    public class ProjetoController : Controller
    {
        public Context context;
        private readonly Isessao _isessao;
        private List<ProjetoDto> projetos;
        private List<AnexoDto> anexos;
        private readonly IWebHostEnvironment _appEnvironment;

        public ProjetoController(Context ctx, Isessao sessao, IWebHostEnvironment env)
        {
            context = ctx;
            _isessao = sessao;
            _appEnvironment = env;
        }

        public async Task<IActionResult> Index()
        {
            projetos = new List<ProjetoDto>();
            anexos = new List<AnexoDto>();

            var Logado = _isessao.BuscarSessaoDoUsuario();

            var Loga = Logado == null ? "Deslogado" : "Logado";

            var id = Logado == null ? 0 : Logado.UsuarioID;

            projetos = await context.Projetos.Where(w => w.UsuarioID == id).ToListAsync();
            foreach (var item in projetos)
            {
                anexos = await context.Anexos.Where(w => w.ProjetoID == item.ProjetoID).ToListAsync();

                item.Anexo = anexos.FirstOrDefault();

                item.Anexo.NomeAnexo = item.Anexo.NomeAnexo == "" ? "Sem Anexo" : item.Anexo.NomeAnexo;

            }
            ViewBag.Nome = Loga;

            ViewBag.IdUsuario = id;
            ViewBag.TipoUsuario = Logado == null ? "" : Logado.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno ? "Aluno" : "Visitante";
            ViewBag.NomeUsuario = Logado.Nome;

            return View(projetos);
        }

        public IActionResult Create()
        {
            var Logado = _isessao.BuscarSessaoDoUsuario();

            var Loga = Logado == null ? "Deslogado" : "Logado";

            ViewBag.Nome = Loga;

            ViewBag.IdUsuario = Logado.UsuarioID;
            ViewBag.NomeUsuario = Logado.Nome;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProjetoDto projeto)
        {
            var Logado = _isessao.BuscarSessaoDoUsuario();

            projeto.UsuarioID = Logado.UsuarioID;

            AnexoDto anexo = new AnexoDto();

            anexo.CampoAnexo = Global.Diretorio == null ? "" : Global.Diretorio;
            anexo.NomeAnexo = Global.NomeAnexo == null ? "" : Global.NomeAnexo;

            projeto.Anexo = anexo;
            
            context.Add(projeto);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var Projeto = context.Projetos
                .Include(f => f.Usuario)
                .FirstOrDefault(p => p.ProjetoID == id);
            return View(Projeto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            projetos = new List<ProjetoDto>();
            anexos = new List<AnexoDto>();

            var Logado = _isessao.BuscarSessaoDoUsuario();

            var Loga = Logado == null ? "Deslogado" : "Logado";

            var projeto = context.Projetos.Find(id);

            anexos = await context.Anexos.Where(w => w.ProjetoID == projeto.ProjetoID).ToListAsync();

            projeto.Anexo = anexos.FirstOrDefault();

            projeto.Anexo.NomeAnexo = projeto.Anexo.NomeAnexo == "" ? "Sem Anexo" : projeto.Anexo.NomeAnexo;

            ViewBag.TipoUsuario = Logado == null ? "" : Logado.TipoUsuario == Models.Enum.TipoUsuarioEnum.Aluno ? "Aluno" : "Visitante";

            ViewBag.Nome = Loga;

            ViewBag.IdUsuario = Logado.UsuarioID;
            ViewBag.NomeUsuario = Logado.Nome;

            return View(projeto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProjetoDto projeto)
        {
            var Logado = _isessao.BuscarSessaoDoUsuario();

            var resultAnexo = context.Anexos.Where(w => w.ProjetoID == projeto.ProjetoID).Select(s=>s.AnexoID).ToList();

            var Anexo = new AnexoDto {
                AnexoID = resultAnexo.FirstOrDefault(),
                CampoAnexo = Global.Diretorio,
                NomeAnexo = Global.NomeAnexo,
                ProjetoID = projeto.ProjetoID,
                Projeto = projeto
            };

            context.Entry(Anexo).State = EntityState.Modified;
            context.SaveChanges();

            projeto.UsuarioID = Logado.UsuarioID;

            context.Entry(projeto).State = EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeletarProjeto(string id)
        {
            var projeto = context.Projetos.Where(w => w.ProjetoID == Convert.ToInt32(id)).ToList();

            context.Remove(projeto.FirstOrDefault());
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> GetDados(IFormFile Anexo)
        {
            ProjetoDto projeto = new ProjetoDto();

            try
            {
                var BuscarUsuario = _isessao.BuscarSessaoDoUsuario();

                if (BuscarUsuario != null)
                {
                    string pasta = "Arquivos_Usuario";

                    string nomeArquivo = Anexo.FileName;

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
                        await Anexo.CopyToAsync(stream);
                    }

                    Global.Diretorio = filePath;
                    Global.NomeAnexo = Anexo.FileName;
                }
            }
            catch (Exception)
            {
            }
            return View(projeto);
        }
    }
}
