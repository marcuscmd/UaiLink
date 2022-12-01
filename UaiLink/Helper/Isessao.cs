using UaiLink.Models;

namespace UaiLink.Helper
{
    public interface Isessao
    {
        void CriarSessaoDoUsuario(UsuarioDto usuario);
        void RemoverSessaoUsuario();
        UsuarioDto BuscarSessaoDoUsuario();
    }
}
