using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Dicts.ProveedorDicts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.FoliosFormaValorada.Response
{
    public class FolioFormaValoradaImpresionResponse : vFoliosFormaValoradaImpresion
    {
        [JsonIgnore]
        public string AprobadoPor
        {
            get
            {

                var aprobado = "";
                switch (IdCatTipoCertificado)
                {
                    case Dicts.TipoCertificado.DobleCero:
                    case Dicts.TipoCertificado.Cero:
                    case Dicts.TipoCertificado.Uno:
                    case Dicts.TipoCertificado.Dos:
                    case Dicts.TipoCertificado.Exentos:
                        if (PruebaEmisiones ?? false)
                        {
                            aprobado = AprobadoDict.Emisiones;
                        }
                        else if (PruebaObd ?? false)
                        {
                            aprobado = AprobadoDict.Obd;
                        }
                        else if (PruebaOpacidad ?? false)
                        {
                            aprobado = AprobadoDict.Opacidad;
                        }
                        if (IdCatTipoCertificado == Dicts.TipoCertificado.Exentos)
                        {
                            aprobado = AprobadoDict.Exento;

                        }
                        break;
                    case Dicts.TipoCertificado.ConstanciasNoAprobado:
                        if (C_RECHAZO == CausaRechazo.OBD)
                            aprobado = "No aprobado. " + OBD.DictOBD[C_RECHAZO_OBD ?? 0];
                        else
                            aprobado = "No aprobado por " + CausaRechazo.DictClaveRechazo[C_RECHAZO ?? 0];
                        break;
                }

                return aprobado;

            }
        }
        public string Semestre { get; set; }
        public string LeyendaCNA { get; set; }
        public DateTime? VigenciaVistaPrevia { get; set; }
        [JsonIgnore]
        public string VigenciaVistaPreviaStr
        {
            get
            { return VigenciaVistaPrevia.HasValue ? VigenciaVistaPrevia.Value.ToString("d", new CultureInfo("es-MX")) : "-"; }
        }
        [JsonIgnore]
        public string VigenciaStr
        {
            get
            {
                string? vigencia;
                if (IdRefrendoExento != null || IdExento != null)
                {
                    vigencia = "VIGENCIA PERMANENTE";
                    if (Combustible == TipoCombustible.DictTipoCombustible[TipoCombustible.Hibridos])
                    {
                        var inicioVigencia = Vigencia.Value.AddYears(-8).ToString("MMMM yyyy", new CultureInfo("es-MX")).ToUpper();
                        var finVigencia = Vigencia.Value.ToString("MMMM yyyy", new CultureInfo("es-MX")).ToUpper();
                        vigencia = $"{inicioVigencia} - {finVigencia}";

                    }
                }
                else
                {
                    vigencia = $"{FechaRegistro.Value.ToString("MMMM", new CultureInfo("es-MX")).ToUpper()}";

                    if (FechaRegistro.Value.Year != FechaRegistro.Value.Year)
                    {
                        vigencia += $" {FechaRegistro.Value.Year}";
                    }
                    vigencia += $" - {Vigencia.Value.ToString("MMMM", new CultureInfo("es-MX")).ToUpper()} {FechaRegistro.Value.Year}";
                }

                return vigencia;
            }
        }
    }
}
