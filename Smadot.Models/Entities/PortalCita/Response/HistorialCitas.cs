using System.Globalization;
using System.Data;
using Smadot.Models.DataBase;
using Smadot.Models.Dicts;
using Smadot.Models.Dicts.ProveedorDicts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.PortalCita.Response
{
    public class HistorialCitasResponse : vHistorialCitum
    {

        public string FechaStr
        {
            get
            {
                return Fecha.ToString("g", new CultureInfo("es-MX"));
            }
        }
        public string Acciones { get; set; }
        public string IngresoManualStr
        {
            get
            {
                return EstatusPrueba == null || EstatusPrueba < EstatusVerificacion.TerminaPruebaVisual ? "-" : (IngresoManual ?? false) ? "Ingreso Manual" : "Ingreso OCR";
            }
        }
        public string ResultadoStr
        {
            get
            {
                try
                {
                    return Resultados.DictResultados[RESULTADO ?? 0];

                }
                catch (Exception)
                {

                    return "";
                }
            }
        }
        public string Progreso
        {
            get
            {
                try
                {
                    return EstatusPrueba == null ? (IdRecepcionDocumentos != null ? "Documentos Recibidos" : "Cita Sin Atender") : EstatusVerificacion.Dict[EstatusPrueba.Value];

                }
                catch (Exception)
                {

                    return "";
                }
            }
        }

        public string Atendida
        {
            get
            {
                try
                {
                    return EstatusPrueba == null ? "Cita Sin Atender" : EstatusVerificacion.Dict[EstatusPrueba.Value];

                }
                catch (Exception)
                {

                    return "";
                }
            }
        }
        public string NombrePropietario
        {
            get
            {
                try
                {
                    return NombrePersona ?? RazonSocial ?? "";

                }
                catch (Exception)
                {

                    return "";
                }
            }
        }
    }

    public class ListDocumentos
    {
        public string DocName { get; set; }
        public string Extension { get; set; }
        public string? UrlName { get; set; }
    }
}
