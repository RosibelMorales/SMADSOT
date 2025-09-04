using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.FoliosFormaValorada.Response
{
    public class FolioFormaValoradaResponse : vVerificacionReposicion
    {
        public string Nombre { get; set; }
        public string TipoCertificadoStr
        {
            get
            {
                try
                {

                    return Dicts.TipoCertificado.DictNombreCertificado[IdCatTipoCertificado ?? 0];
                }
                catch (Exception)
                {

                    return "-";
                }
            }
        }
        public string Actions { get; set; }
        public string FechaStr
        {
            get
            {
                return FechaRegistro.Value.ToString("g");
            }
        }
        public string FolioStr
        {
            get
            {
                try
                {

                    return FolioCertificado.ToString("000000000");
                }
                catch (Exception)
                {

                    return "-";
                }
            }
        }
        public string ImporteStr
        {
            get
            {
                try
                {

                    return ImporteActual.ToString("C2");
                }
                catch (Exception)
                {

                    return "-";
                }
            }
        }
    }
    public class RecalcularResponse
    {
        public long IdFoliosFormaValoradaVerificentro { get; set; }

        public long IdVerificentro { get; set; }
    }
}
