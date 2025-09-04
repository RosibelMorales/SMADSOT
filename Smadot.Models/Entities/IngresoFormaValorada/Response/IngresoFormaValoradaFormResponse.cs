using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smadot.Models.Dicts;

namespace Smadot.Models.Entities.IngresoFormaValorada.Response
{
    public class IngresoFormaValoradaFormResponse
    {
        public long IdSolicitudFV { get; set; }
        public long IdIngresoFV { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaEntrega { get; set; }
        public long IdAlmacen { get; set; }
        public string NombreRecibio { get; set; }
        public string Almacen { get; set; }
        public string UrlDoc1 { get; set; }
        public string UrlDoc2 { get; set; }
        public string UrlDoc3 { get; set; }
        public List<CertificadoGrupo> certificados { get; set; }
        public List<CertificadosTotalData> certificadosTotalData { get; set; }

    }
    public class CertificadosTotalData
    {
        public int CantidadSolicitada { get; set; }
        public int CantidadRecibida { get; set; }
        public int IdCatTipoCertificado { get; set; }
        public string Clave
        {
            get
            {
                return TipoCertificado.DictNombreClave.FirstOrDefault(x => x.Key == IdCatTipoCertificado).Value ?? "";
            }
        }
        public string NombreCertificado
        {
            get
            {
                return TipoCertificado.DictNombreCertificado.FirstOrDefault(x => x.Key == IdCatTipoCertificado).Value ?? "";
            }
        }
    }
    public class CertificadoGrupo
    {
        public int IdCatTipoCertificado { get; set; }
        public string NombreCertificado
        {
            get
            {
                return TipoCertificado.DictNombreCertificado.FirstOrDefault(x => x.Key == IdCatTipoCertificado).Value ?? "";
            }
        }
        public string Clave
        {
            get
            {
                return TipoCertificado.DictNombreClave.FirstOrDefault(x => x.Key == IdCatTipoCertificado).Value ?? "";
            }
        }
        public List<CertificadoData> Certificados { get; set; }
    }
    public class CertificadoData
    {
        public long IdIngresoCertificado { get; set; }
        public int IdCatTipoCertificado { get; set; }
        public int FolioInicial { get; set; }
        public int FolioFinal { get; set; }
        public int CantidadRecibida { get; set; }
        public string Clave
        {
            get
            {
                return TipoCertificado.DictNombreClave.FirstOrDefault(x => x.Key == IdCatTipoCertificado).Value ?? "";
            }
        }
        public string NombreCertificado
        {
            get
            {
                return TipoCertificado.DictNombreCertificado.FirstOrDefault(x => x.Key == IdCatTipoCertificado).Value ?? "";
            }
        }
    }
}