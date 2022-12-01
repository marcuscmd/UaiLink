using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using UaiLink.Models;

namespace UaiLink.Helper
{
    public class Sessao : Isessao
    {
        private readonly IHttpContextAccessor _httpContext;
        
        public Sessao(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public UsuarioDto BuscarSessaoDoUsuario()
        {
            string sessaoUsuario = _httpContext.HttpContext.Session.GetString("sessaoUsuarioLogado");

            if (string.IsNullOrEmpty(sessaoUsuario)) return null;

            return JsonConvert.DeserializeObject<UsuarioDto>(sessaoUsuario);
        }

        public void CriarSessaoDoUsuario(UsuarioDto usuario)
        {
            string valor = JsonConvert.SerializeObject(usuario);

            _httpContext.HttpContext.Session.SetString("sessaoUsuarioLogado", valor);
        }

        public void RemoverSessaoUsuario()
        {
            _httpContext.HttpContext.Session.Remove("sessaoUsuarioLogado");
        }
    }
}
