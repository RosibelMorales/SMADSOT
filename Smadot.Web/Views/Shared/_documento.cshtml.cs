using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Smadot.Web.Views.Shared
{
    public class _documentoModel : PageModel
    {
        public string UrlDocumento { get; set; }
        public string Nombre { get; set; }
        public string Linea { get; set; }
        public void OnGet()
        {
        }
    }
}
