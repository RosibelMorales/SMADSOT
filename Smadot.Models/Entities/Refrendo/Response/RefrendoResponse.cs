using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Refrendo.Response
{
    public class RefrendoGridResponse : vRefrendo
    {
        public string Acciones { get; set; }

        public List<vExento> Exentos { get; set;}

        public bool VerificacionBool { get; set; }
    }

    public class RefrendoAutocompleteResponse
    {
        public long Id { get; set; }
        public string Text { get; set; }
    }
}
