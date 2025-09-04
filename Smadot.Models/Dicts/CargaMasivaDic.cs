using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smadot.Models.Entities.CargaMasiva.Response;

namespace Smadot.Models.Dicts
{
    public struct CargaMasivaDic
    {
        public static Dictionary<ColumnImportadorExcelCargaEnum, string> Dictionary = new()
        {
            {ColumnImportadorExcelCargaEnum.Nombre,"Nombre"},
            {ColumnImportadorExcelCargaEnum.RazonSocial,"Razón Social"},
            {ColumnImportadorExcelCargaEnum.Correo, "Correo"},
            {ColumnImportadorExcelCargaEnum.FechaCita, "Fecha Cita"},
            {ColumnImportadorExcelCargaEnum.IdCVV, "IdCVV"},
            {ColumnImportadorExcelCargaEnum.Placa, "Placa"},
            {ColumnImportadorExcelCargaEnum.IdMarca, "IdMarca"},
            {ColumnImportadorExcelCargaEnum.IdSubMarca, "IdSubMarca"},
            {ColumnImportadorExcelCargaEnum.Anio, "Anio"},
            {ColumnImportadorExcelCargaEnum.VIN, "VIN"},
            {ColumnImportadorExcelCargaEnum.Color, "Color"},
            {ColumnImportadorExcelCargaEnum.Estado, "Estado"},
            {ColumnImportadorExcelCargaEnum.Poblano, "Poblano"},
            {ColumnImportadorExcelCargaEnum.IdCatTipoCombustible, "IdCatTipoCombustible"},
            {ColumnImportadorExcelCargaEnum.ImporteActual, "Importe Actual"},
            {ColumnImportadorExcelCargaEnum.FechaRegistro, "Fecha Registro"},
            {ColumnImportadorExcelCargaEnum.ConsecutivoTramite, "Consecutivo Trámite"},
            {ColumnImportadorExcelCargaEnum.FechaEmisionRef, "FechaEmisionRef"},
            {ColumnImportadorExcelCargaEnum.FechaPago, "Fecha Pago"},
            {ColumnImportadorExcelCargaEnum.FolioCertificado, "Folio Certificado"},
            {ColumnImportadorExcelCargaEnum.Vigencia, "Vigencia"},
            {ColumnImportadorExcelCargaEnum.Marca, "Marca"},
            {ColumnImportadorExcelCargaEnum.Modelo, "Modelo"},
            {ColumnImportadorExcelCargaEnum.Combustible, "Combustible"},
            {ColumnImportadorExcelCargaEnum.TarjetaCirculacion, "TarjetaCirculacion"},
            {ColumnImportadorExcelCargaEnum.ClaveLinea, "ClaveLinea"},
            {ColumnImportadorExcelCargaEnum.NumeroReferencia, "NumeroReferencia"},
            {ColumnImportadorExcelCargaEnum.Semestre, "Semestre"},
            {ColumnImportadorExcelCargaEnum.AnioVerificacion, "Año Verificación"},
            {ColumnImportadorExcelCargaEnum.FechaVerificacion, "Fecha Verificación"},
            {ColumnImportadorExcelCargaEnum.IdCatTipoCertificado, "IdCatTipoCertificado"},
            {ColumnImportadorExcelCargaEnum.IdMotivoVerificacion, "IdMotivoVerificacion"}
        };
    }
}
