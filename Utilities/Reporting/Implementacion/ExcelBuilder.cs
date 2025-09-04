using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Smadot.Utilities.Reporting.Interfaces;
using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using System.Globalization;
using System.Drawing;
using NPOI.Util;
using NPOI.SS;
using iTextSharp.text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Smadot.Utilities.Reporting.Implementacion
{
    public class ExcelBuilder : IExcelBuilder
    {
        private readonly ILogger<ExcelBuilder> _logger;
        public ExcelBuilder(ILogger<ExcelBuilder> logger)
        {
            _logger = logger;
        }

        public async Task<ResponseGeneric<DataExcelReport>> GetDocumentoFoliosUsadosEnVentanilla(List<FoliosUsadosEnVentanillaDocument> data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.xlsx";
            try
            {
                await using (MemoryStream ms = new())
                {
                    List<string> columnas = new()
                    {
                        "#",
                        "Folio de trámite",
                        "Tipo de trámite",
                        "Datos del vehículo",
                        "Folio de certificado o constancia",
                        "Razón",
                        "Referencia bancaria",
                        "Fecha"
                    };
                    var reporte = InicializarReporte(columnas, "FoliosUsadosVentanilla", "Folios usados en ventanilla");

                    //Creación de filas
                    int index = 1;
                    foreach (var item in data)
                    {
                        IRow fila = reporte.Hoja.CreateRow(index);
                        fila.CreateCell(0).SetCellValue(index - 1);
                        fila.CreateCell(1).SetCellValue(item.FolioTramite);
                        fila.CreateCell(2).SetCellValue(item.TipoTramite);
                        fila.CreateCell(3).SetCellValue(item.DatosVehiculo);
                        fila.CreateCell(4).SetCellValue(item.FolioCertificado);
                        fila.CreateCell(5).SetCellValue(item.Razon);
                        fila.CreateCell(6).SetCellValue(item.ReferenciaBancaria);
                        fila.CreateCell(7).SetCellValue(item.Fecha.ToString("dd/MM/yyyy"));
                        index++;
                    }
                    resp = ms.ToArray();
                    reporte.Libro.Write(ms, true);
                    resp = ms.ToArray();
                }
                return new ResponseGeneric<DataExcelReport>(new DataExcelReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoExcel = resp
                });
            }
            catch (Exception ex)
            {

                return new ResponseGeneric<DataExcelReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataExcelReport>> GetDocumentoFoliosRegresadosSPF(List<FoliosRegresadosSPFDocument> data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.xlsx";
            try
            {
                await using (MemoryStream ms = new())
                {
                    List<string> columnas = new()
                    {
                        "#",
                        "Tipo de certificado",
                        "Clave de certificado",
                        "Folio inicial",
                        "Folio final",
                        "Responsable de la entrega",
                        "Persona que validó",
                        "Fecha"
                    };
                    var reporte = InicializarReporte(columnas, "FoliosRegresadosSPF", "Folios regresados a spf");

                    //Creación de filas
                    int index = 2;
                    foreach (var item in data)
                    {
                        IRow fila = reporte.Hoja.CreateRow(index);
                        fila.CreateCell(0).SetCellValue(index - 1);
                        fila.CreateCell(1).SetCellValue(item.TipoCertificado);
                        fila.CreateCell(2).SetCellValue(item.ClaveCertificado);
                        fila.CreateCell(3).SetCellValue(item.FolioInicial);
                        fila.CreateCell(4).SetCellValue(item.FolioFinal);
                        fila.CreateCell(5).SetCellValue(item.ResponsableEntrega);
                        fila.CreateCell(6).SetCellValue(item.PersonaValido);
                        fila.CreateCell(7).SetCellValue(item.Fecha.ToString("dd/MM/yyyy"));
                        index++;
                    }
                    resp = ms.ToArray();
                    reporte.Libro.Write(ms, true);
                    resp = ms.ToArray();
                }
                return new ResponseGeneric<DataExcelReport>(new DataExcelReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoExcel = resp
                });
            }
            catch (Exception ex)
            {

                return new ResponseGeneric<DataExcelReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataExcelReport>> GetDocumentoFoliosVendidosCentroVerificacion(List<FoliosVendidosCentroVerificacionDocument> data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.xlsx";
            try
            {
                await using (MemoryStream ms = new())
                {
                    List<string> columnas = new()
                    {
                        "#",
                        "Folio de venta",
                        "Folios de FV",
                        "Clave de venta",
                        "Folios en stock",
                        "Referencia bancaria",
                        "Monto de cada venta",
                        "Fecha"
                    };
                    var reporte = InicializarReporte(columnas, "FoliosVendidosCVV", "Folios vendidos a centros de verificación");

                    //Creación de filas                    
                    int index = 2;
                    foreach (var item in data)
                    {
                        IRow fila = reporte.Hoja.CreateRow(index);
                        fila.CreateCell(0).SetCellValue(index - 1);
                        fila.CreateCell(1).SetCellValue(item.FolioVenta?.ToString());
                        fila.CreateCell(2).SetCellValue(item.FolioFV?.ToString());
                        fila.CreateCell(3).SetCellValue(item.ClaveVenta);
                        fila.CreateCell(4).SetCellValue(item.FoliosStock?.ToString());
                        fila.CreateCell(5).SetCellValue(item.ReferenciaBancaria);
                        fila.CreateCell(6).SetCellValue(item.MontoCadaVenta?.ToString("C2", new CultureInfo("es-MX")));
                        fila.CreateCell(7).SetCellValue(item.Fecha.ToString("dd/MM/yyyy"));
                        index++;
                    }
                    resp = ms.ToArray();
                    reporte.Libro.Write(ms, true);
                    resp = ms.ToArray();
                }
                return new ResponseGeneric<DataExcelReport>(new DataExcelReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoExcel = resp
                });
            }
            catch (Exception ex)
            {

                return new ResponseGeneric<DataExcelReport>(ex);
            }
        }
        public async Task<ResponseGeneric<DataExcelReport>> ExcelDocumentCitasCanceladas(List<PersonaCita> data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.xlsx";

            try
            {
                await using (MemoryStream ms = new())
                {
                    List<string> columnas = new()
                    {
                        "Nombre",
                        "Correo",
                        "Fecha Hora",
                    };

                    var reporte = InicializarReporte(columnas, "CitasCanceladas", "Lista de Citas Canceladas");

                    //Creación de filas
                    int index = 2;
                    var autoSize = 1;
                    foreach (var item in data)
                    {
                        IRow fila = reporte.Hoja.CreateRow(index);

                        fila.CreateCell(0).SetCellValue(item.Nombre);
                        fila.CreateCell(1).SetCellValue(item.Correo);
                        fila.CreateCell(2).SetCellValue(item.Fecha.ToString("dd/MM/yyyy hh:mm tt"));

                        index++;
                        autoSize++;
                    }
                    resp = ms.ToArray();
                    reporte.Libro.Write(ms, true);
                    ms.Seek(0, SeekOrigin.Begin);
                    resp = ms.ToArray();
                }
                return new ResponseGeneric<DataExcelReport>(new DataExcelReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoExcel = resp
                });
            }
            catch (Exception ex)
            {

                return new ResponseGeneric<DataExcelReport>(ex);
            }
        }
        public async Task<ResponseGeneric<DataExcelReport>> GetDocumentoDevolucionSPF(List<DevolucionSPFDocument> data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.xlsx";

            try
            {
                await using (MemoryStream ms = new())
                {
                    List<string> columnas = new()
                    {
                        "#",
                        "Numero de Devolucion",
                        "Fecha de Registro",
                        "Fecha de Entrega",
                        "Usuario Aprobó",
                        "Responsable de la entrega",
                        "Recibio en SPF",
                        "Numero de Solicitud",
                        "Cantidad"
                    };

                    var reporte = InicializarReporte(columnas, "DevolucionSPF", "");

                    //Creación de filas
                    int index = 2;
                    var autoSize = 1;
                    foreach (var item in data)
                    {
                        IRow fila = reporte.Hoja.CreateRow(index);
                        fila.CreateCell(0).SetCellValue(index - 1);

                        fila.CreateCell(1).SetCellValue(item.NumeroDevolucion);
                        fila.CreateCell(2).SetCellValue(item.FechaRegistro.ToString("dd/MM/yyyy"));
                        fila.CreateCell(3).SetCellValue(item.FechaEntrega.ToString("dd/MM/yyyy"));
                        fila.CreateCell(4).SetCellValue(item.UsuarioAprobo);
                        fila.CreateCell(5).SetCellValue(item.ResponsableEntrega);
                        fila.CreateCell(6).SetCellValue(item.RecibioSPF);
                        fila.CreateCell(7).SetCellValue(item.NumeroDevolucion);
                        fila.CreateCell(8).SetCellValue(item.Cantidad.ToString());
                        reporte.Hoja.AutoSizeColumn(autoSize);

                        index++;
                        autoSize++;
                    }

                    resp = ms.ToArray();
                    reporte.Libro.Write(ms, true);
                    resp = ms.ToArray();
                }
                return new ResponseGeneric<DataExcelReport>(new DataExcelReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoExcel = resp
                });
            }
            catch (Exception ex)
            {

                return new ResponseGeneric<DataExcelReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataExcelReport>> GetDocumentoVentaCVV(List<VentaCVVDocument> data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.xlsx";
            try
            {
                await using (MemoryStream ms = new())
                {
                    List<string> columnas = new()
                    {
                        "Numero de Venta",
                        "Verificentro",
                        "Cantidad de Hologramas",
                        "Usuario Registró",
                        "Fecha de Venta"
                    };

                    var reporte = InicializarReporte(columnas, "VentaFormaValoradaCVV", "Venta de Formas Valoradas a CVV");

                    //Creación de filas
                    int index = 2;
                    var autoSize = 1;
                    foreach (var item in data)
                    {
                        IRow fila = reporte.Hoja.CreateRow(index);
                        fila.CreateCell(0).SetCellValue(item.NumeroVenta);
                        fila.CreateCell(1).SetCellValue(item.Verificentro);
                        fila.CreateCell(2).SetCellValue(item.CantidadHologramas.ToString());
                        fila.CreateCell(3).SetCellValue(item.UserRegistro);
                        fila.CreateCell(4).SetCellValue(item.FechaVenta.ToString("dd/MM/yyyy"));
                        index++;
                    }
                    reporte.Libro.Write(ms, true);
                    resp = ms.ToArray();
                }
                return new ResponseGeneric<DataExcelReport>(new DataExcelReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoExcel = resp
                });
            }
            catch (Exception ex)
            {

                return new ResponseGeneric<DataExcelReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataExcelReport>> GetDocumentoConsultaStockDVRF(List<ConsultaStockDVRFDocument> data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.xlsx";
            try
            {
                await using (MemoryStream ms = new())
                {
                    List<string> columnas = new()
                    {
                        "Tipo de Certificado",
                        "Cantidad",
                        "Caja",
                        "Folio inicial",
                        "Folio final",
                        "Clave del Certificado"
                    };
                    var reporte = InicializarReporte(columnas, "ConsultaStockDVRF", "Consulta Stock DVRF");

                    //Creación de filas
                    int index = 1;
                    foreach (var item in data)
                    {
                        IRow fila = reporte.Hoja.CreateRow(index);
                        fila.CreateCell(0).SetCellValue(index - 1);

                        fila.CreateCell(1).SetCellValue(item.TipoCertificado);
                        fila.CreateCell(2).SetCellValue(item.Cantidad.ToString());
                        fila.CreateCell(3).SetCellValue(item.Caja.ToString());
                        fila.CreateCell(4).SetCellValue(item.FolioInicial.ToString());
                        fila.CreateCell(5).SetCellValue(item.FolioFinal.ToString());
                        fila.CreateCell(6).SetCellValue(item.ClaveCertificado);
                        index++;
                    }
                    resp = ms.ToArray();
                    reporte.Libro.Write(ms, true);
                    resp = ms.ToArray();
                }
                return new ResponseGeneric<DataExcelReport>(new DataExcelReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoExcel = resp
                });
            }
            catch (Exception ex)
            {

                return new ResponseGeneric<DataExcelReport>(ex);
            }
        }

        public async Task<ResponseGeneric<DataExcelReport>> GetDocumentoTablaMaestra(ConsultaTablaMaestraPDFUtilitiesResponse data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.xlsx";
            try
            {
                await using (MemoryStream ms = new())
                {
                    List<string> columnas = new()
                    {
                        "Marca",
                        "Submarca",
                        "Protocolo",
                        "Desde",
                        "Hasta",
                        "Cilindros",
                        "Cilindrada",
                        "Combustible",
                        "Doble Cero",
                    };
                    var reporte = InicializarReporte(columnas, "Consulta Tabla Maestra", "Consulta Tabla Maestra");

                    //Creación de filas
                    int index = 2;
                    foreach (var item in data.Rows)
                    {
                        IRow fila = reporte.Hoja.CreateRow(index);
                        fila.CreateCell(0).SetCellValue(item.Marca);
                        fila.CreateCell(1).SetCellValue(item.SubMarca);
                        fila.CreateCell(2).SetCellValue(item.ProtocoloStr);
                        fila.CreateCell(3).SetCellValue(item.ANO_DESDE ?? 0);
                        fila.CreateCell(4).SetCellValue(item.ANO_HASTA ?? 0);
                        fila.CreateCell(5).SetCellValue(item.CILINDROS ?? 0);
                        fila.CreateCell(6).SetCellValue(item.CILINDRADA ?? 0);
                        fila.CreateCell(7).SetCellValue(item.CombustibleStr);
                        fila.CreateCell(8).SetCellValue(item.AplicaDobleCero);
                        index++;
                    }
                    reporte.Hoja.SetColumnWidth(0, 6000);
                    reporte.Hoja.SetColumnWidth(1, 6000);
                    reporte.Hoja.SetColumnWidth(2, 6000);
                    reporte.Hoja.SetColumnWidth(3, 6000);
                    reporte.Hoja.SetColumnWidth(4, 3000);
                    //var i = 0;
                    //foreach (var item in columnas)
                    //{
                    //    reporte.Hoja.AutoSizeColumn(i);
                    //    i++;
                    //}
                    resp = ms.ToArray();
                    reporte.Libro.Write(ms, true);
                    resp = ms.ToArray();
                }
                return new ResponseGeneric<DataExcelReport>(new DataExcelReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoExcel = resp
                });
            }
            catch (Exception ex)
            {

                return new ResponseGeneric<DataExcelReport>(ex);
            }
        }
        #region Documentos Citas

        public async Task<ResponseGeneric<DataExcelReport>> GetDocumentoHistorialCitas(HistorialCitasUtilitiesResponse data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.xlsx";
            try
            {
                await using (MemoryStream ms = new())
                {
                    List<string> columnas = new()
                    {
                        "Folio Cita",
                        "Nombre Propietario",
                        "Placa",
                        "VIN",
                        "Marca",
                        "Submarca",
                        "Centro de verificación",
                        "Progreso",
                        "Fecha",
                        "Resultado",
                        "Tipo Ingreso",
                    };
                    var reporte = InicializarReporte(columnas, "Historial de Citas", "Historial de Citas");

                    //Creación de filas
                    int index = 2;
                    foreach (var item in data.Rows ?? new())
                    {
                        IRow fila = reporte.Hoja.CreateRow(index);
                        fila.CreateCell(0).SetCellValue(item.Folio);
                        fila.CreateCell(0).SetCellValue(item.Folio);
                        // fila.CreateCell(1).SetCellValue(item.FolioAsignado.HasValue ? item.FolioAsignado.Value.ToString("00000000") : "");
                        fila.CreateCell(1).SetCellValue(item.Placa);
                        fila.CreateCell(2).SetCellValue(item.Serie);
                        fila.CreateCell(3).SetCellValue(item.Marca);
                        fila.CreateCell(4).SetCellValue(item.Modelo ?? "-");
                        fila.CreateCell(5).SetCellValue(item.NombreCentro);
                        fila.CreateCell(6).SetCellValue(item.Progreso);
                        fila.CreateCell(7).SetCellValue(item.FechaStr);
                        fila.CreateCell(8).SetCellValue(item.ResultadoStr ?? "-");
                        fila.CreateCell(9).SetCellValue(item.IngresoManualStr);
                        index++;
                    }
                    reporte.Hoja.SetColumnWidth(0, 3000);
                    reporte.Hoja.SetColumnWidth(1, 3000);
                    reporte.Hoja.SetColumnWidth(2, 6000);
                    reporte.Hoja.SetColumnWidth(3, 4000);
                    reporte.Hoja.SetColumnWidth(4, 6000);
                    reporte.Hoja.SetColumnWidth(5, 6000);
                    reporte.Hoja.SetColumnWidth(6, 4000);
                    reporte.Hoja.SetColumnWidth(7, 4500);
                    reporte.Hoja.SetColumnWidth(8, 4000);
                    reporte.Hoja.SetColumnWidth(9, 4000);
                    //var i = 0;
                    //foreach (var item in columnas)
                    //{
                    //    reporte.Hoja.AutoSizeColumn(i);
                    //    i++;
                    //}
                    resp = ms.ToArray();
                    reporte.Libro.Write(ms, true);
                    resp = ms.ToArray();
                }
                return new ResponseGeneric<DataExcelReport>(new DataExcelReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoExcel = resp
                });
            }
            catch (Exception ex)
            {

                return new ResponseGeneric<DataExcelReport>(ex);
            }
        }
        #endregion

        public async Task<ResponseGeneric<DataExcelReport>> GetDocumentoDashboardGrid(List<EstadisticasDashboardGridDocumentResponse> data)
        {
            byte[] resp = null;
            string archivoPDF = $"{Guid.NewGuid()}.xlsx";
            string nombreHoja = "";
            IWorkbook workbook = new XSSFWorkbook();
            ICellStyle style = workbook.CreateCellStyle();
            style.WrapText = true;

            try
            {
                await using (MemoryStream ms = new())
                {
                    List<string> columnas = new()
                    {
                        "Id",
                        "Combustible",
                        "Serie",
                        "Placa",
                        "Modelo",
                        "Marca",
                        "SubMarca",
                        "NombrePropietario",
                        "TarjetaCirculacion",
                        "NombreVerificentro",
                        "FechaVerificacion",
                        "Vigencia",
                        "MotivoVerificacion",
                        "IdMotivoVerificacion.ToString",
                        "TipoServicio",
                        "CausaRechazo",
                        "CambioPlacas",
                        "FechaFacturacion ",
                        "NoIntentos",
                        "(SinMulta",
                        "IdVerificacion",
                        "IdCatMarcaVehiculo",
                        "IdCatSubmarcaVehiculo",
                        "Motor_DSL",
                        "COMB_ORIG",
                        "CARROCERIA",
                        "ALIM_COMB",
                        "CILINDROS",
                        "CILINDRADA",
                        "PBV",
                        "PBV_EQUIV",
                        "PBV_ASM",
                        "CONV_CATAL",
                        "OBD",
                        "C_ABS",
                        "T_TRACC",
                        "C_TRACC",
                        "T_PRUEBA",
                        "PROTOCOLO",
                        "POTMAX_RPM",
                        "ANO_DESDE",
                        "ANO_HASTA",
                        "O2_MAX.ToString",
                        "LAMDA_MAX.ToString",
                        "POT_5024.ToString",
                        "POT_2540.ToString",
                        "DOBLECERO",
                        "CERO_GASOL",
                        "CERO_GASLP",
                        "CERO_GASNC",
                        "CERO_DSL",
                        "REF_00",
                        "InicioPruebas ",
                        "FinalizacionPruebas ",
                        "EstatusPrueba.ToString",
                        "FugasSistemaEscape ",
                        "PortafiltroAire ",
                        "FiltroAire ",
                        "TaponCombustible ",
                        "Bayoneta ",
                        "FugaLiquidoRefrigerante ",
                        "DesperfectosNeumaticos ",
                        "ControlEmisionAlterados ",
                        "GobernadorBuenEstado ",
                        "NumeroEscapes.ToString",
                        "Etapa",
                        "SPS_Humo.ToString",
                        "SPS_5024.ToString",
                        "SPS_2540.ToString",
                        "HC.ToString",
                        "CO ",
                        "CO2 ",
                        "O2 ",
                        "NO ",
                        "LAMDA ",
                        "FCNOX ",
                        "FCDIL ",
                        "RPM ",
                        "KPH ",
                        "VEL_LIN ",
                        "VEL_ANG ",
                        "BHP ",
                        "PAR_TOR ",
                        "FUERZA ",
                        "POT_FRENO ",
                        "TEMP.ToString",
                        "PRESION.ToString",
                        "HUMREL.ToString",
                        "OBD_TIPO_SDB",
                        "OBD_MIL.ToString",
                        "OBD_CATAL.ToString",
                        "OBD_CILIN.ToString",
                        "OBD_COMBU.ToString",
                        "OBD_INTEG.ToString",
                        "OBD_OXIGE.ToString",
                        "LAMDA_5024 ",
                        "TEMP_5024 ",
                        "HR_5024 ",
                        "PSI_5024 ",
                        "FCNOX_5024 ",
                        "FCDIL_5024 ",
                        "RPM_5024 ",
                        "KPH_5024 ",
                        "THP_5024 ",
                        "VOLTS_5024 ",
                        "HC_5024.ToString",
                        "CO_5024 ",
                        "CO2_5024 ",
                        "COCO2_5024 ",
                        "O2_5024 ",
                        "NO_5024.ToString",
                        "LAMDA_2540 ",
                        "TEMP_2540 ",
                        "HR_2540",
                        "PSI_2540.ToString",
                        "FCNOX_2540 ",
                        "FCDIL_2540 ",
                        "RPM_2540.ToString",
                        "KPH_2540 ",
                        "THP_2540 ",
                        "VOLTS_2540 ",
                        "HC_2540.ToString",
                        "CO_2540 ",
                        "CO2_2540 ",
                        "COCO2_2540 ",
                        "O2_2540 ",
                        "NO_2540.ToString",
                        "OPACIDADP ",
                        "OPACIDADK ",
                        "TEMP_MOT.ToString",
                        "VEL_GOB.ToString",
                        "TEMP_GAS.ToString",
                        "TEMP_CAM.ToString",
                        "RESULTADO.ToString",
                        "C_RECHAZO.ToString",
                        "C_RECHAZO_OBD.ToString",
                        "(PruebaObd",
                        "(PruebaEmisiones",
                        "(PruebaOpacidad",
                        "EntidadProcedencia",
                        "FolioCertificadoActual",
                        "FolioCertificadoAnterior",
                        "ClaveTramite",
                        "TipoCertificadoString",
                        "VerificacionActiva",
                    };

                    var reporte = InicializarReporte(columnas, "Verificaciones", "Verificaciones");

                    //Creación de filas
                    int index = 2;
                    var autoSize = 0;
                    foreach (var item in data)
                    {
                        IRow fila = reporte.Hoja.CreateRow(index);
                        fila.CreateCell(0).SetCellValue(item.Id);
                        fila.CreateCell(1).SetCellValue(item.Combustible);
                        fila.CreateCell(2).SetCellValue(item.Serie);
                        fila.CreateCell(3).SetCellValue(item.Placa);
                        fila.CreateCell(4).SetCellValue(item.Modelo);
                        fila.CreateCell(5).SetCellValue(item.Marca);
                        fila.CreateCell(6).SetCellValue(item.SubMarca);
                        fila.CreateCell(7).SetCellValue(item.NombrePropietario);
                        fila.CreateCell(8).SetCellValue(item.TarjetaCirculacion);
                        fila.CreateCell(9).SetCellValue(item.NombreVerificentro);
                        fila.CreateCell(10).SetCellValue(item.FechaVerificacion.ToString("d", new CultureInfo("es-MX")));
                        fila.CreateCell(11).SetCellValue(item.Vigencia.ToString("d", new CultureInfo("es-MX")));
                        fila.CreateCell(12).SetCellValue(item.MotivoVerificacion);
                        fila.CreateCell(13).SetCellValue(item.IdMotivoVerificacion.ToString("00"));
                        fila.CreateCell(14).SetCellValue(item.TipoServicio);
                        fila.CreateCell(15).SetCellValue(item.CausaRechazo);
                        fila.CreateCell(16).SetCellValue(item.CambioPlacas ? "SÍ" : "NO");
                        fila.CreateCell(17).SetCellValue(item.FechaFacturacion == null ? "" : item.FechaFacturacion.Value.ToString("d", new CultureInfo("es-MX")));
                        fila.CreateCell(18).SetCellValue(item.NoIntentos);
                        fila.CreateCell(19).SetCellValue((item.SinMulta ?? false) ? "SÍ" : "NO");
                        fila.CreateCell(20).SetCellValue(item.IdVerificacion);
                        fila.CreateCell(21).SetCellValue(item.IdCatMarcaVehiculo);
                        fila.CreateCell(22).SetCellValue(item.IdCatSubmarcaVehiculo);
                        fila.CreateCell(23).SetCellValue(item.Motor_DSL);
                        fila.CreateCell(24).SetCellValue(item.COMB_ORIG);
                        fila.CreateCell(25).SetCellValue(item.CARROCERIA);
                        fila.CreateCell(26).SetCellValue(item.ALIM_COMB);
                        fila.CreateCell(27).SetCellValue(item.CILINDROS);
                        fila.CreateCell(28).SetCellValue(item.CILINDRADA);
                        fila.CreateCell(29).SetCellValue(item.PBV);
                        fila.CreateCell(30).SetCellValue(item.PBV_EQUIV);
                        fila.CreateCell(31).SetCellValue(item.PBV_ASM);
                        fila.CreateCell(32).SetCellValue(item.CONV_CATAL);
                        fila.CreateCell(33).SetCellValue(item.OBD);
                        fila.CreateCell(34).SetCellValue(item.C_ABS);
                        fila.CreateCell(35).SetCellValue(item.T_TRACC);
                        fila.CreateCell(36).SetCellValue(item.C_TRACC);
                        fila.CreateCell(37).SetCellValue(item.T_PRUEBA);
                        fila.CreateCell(38).SetCellValue(item.PROTOCOLO);
                        fila.CreateCell(39).SetCellValue(item.POTMAX_RPM);
                        fila.CreateCell(40).SetCellValue(item.ANO_DESDE);
                        fila.CreateCell(41).SetCellValue(item.ANO_HASTA);
                        fila.CreateCell(42).SetCellValue(item.O2_MAX.ToString("0.####"));
                        fila.CreateCell(43).SetCellValue(item.LAMDA_MAX.ToString("0.####"));
                        fila.CreateCell(44).SetCellValue(item.POT_5024.ToString("0.####"));
                        fila.CreateCell(45).SetCellValue(item.POT_2540.ToString("0.####"));
                        fila.CreateCell(46).SetCellValue(item.DOBLECERO);
                        fila.CreateCell(47).SetCellValue(item.CERO_GASOL);
                        fila.CreateCell(48).SetCellValue(item.CERO_GASLP);
                        fila.CreateCell(49).SetCellValue(item.CERO_GASNC);
                        fila.CreateCell(50).SetCellValue(item.CERO_DSL);
                        fila.CreateCell(51).SetCellValue(item.REF_00);
                        fila.CreateCell(52).SetCellValue(item.InicioPruebas == null ? "" : item.InicioPruebas.Value.ToString("g", new CultureInfo("es-MX")));
                        fila.CreateCell(53).SetCellValue(item.FinalizacionPruebas == null ? "" : item.FinalizacionPruebas.Value.ToString("g", new CultureInfo("es-MX")));
                        fila.CreateCell(54).SetCellValue(item.EstatusPrueba.ToString());
                        fila.CreateCell(55).SetCellValue(item.FugasSistemaEscape == null ? "N/A" : (item.FugasSistemaEscape ?? false) ? "SÍ" : "NO");
                        fila.CreateCell(56).SetCellValue(item.PortafiltroAire == null ? "N/A" : (item.PortafiltroAire ?? false) ? "SÍ" : "NO");
                        fila.CreateCell(57).SetCellValue(item.FiltroAire == null ? "N/A" : (item.FiltroAire ?? false) ? "SÍ" : "NO");
                        fila.CreateCell(58).SetCellValue(item.TaponCombustible == null ? "N/A" : (item.TaponCombustible ?? false) ? "SÍ" : "NO");
                        fila.CreateCell(59).SetCellValue(item.Bayoneta == null ? "N/A" : (item.Bayoneta ?? false) ? "SÍ" : "NO");
                        fila.CreateCell(60).SetCellValue(item.FugaLiquidoRefrigerante == null ? "N/A" : (item.FugaLiquidoRefrigerante ?? false) ? "SÍ" : "NO");
                        fila.CreateCell(61).SetCellValue(item.DesperfectosNeumaticos == null ? "N/A" : (item.DesperfectosNeumaticos ?? false) ? "SÍ" : "NO");
                        fila.CreateCell(62).SetCellValue(item.ControlEmisionAlterados == null ? "N/A" : (item.ControlEmisionAlterados ?? false) ? "SÍ" : "NO");
                        fila.CreateCell(63).SetCellValue(item.GobernadorBuenEstado == null ? "N/A" : (item.GobernadorBuenEstado ?? false) ? "SÍ" : "NO");
                        fila.CreateCell(64).SetCellValue(item.NumeroEscapes.ToString());
                        fila.CreateCell(65).SetCellValue(item.Etapa);
                        fila.CreateCell(66).SetCellValue(item.SPS_Humo.ToString());
                        fila.CreateCell(67).SetCellValue(item.SPS_5024.ToString());
                        fila.CreateCell(68).SetCellValue(item.SPS_2540.ToString());
                        fila.CreateCell(69).SetCellValue(item.HC.ToString());
                        fila.CreateCell(70).SetCellValue(item.CO == null ? "0.0" : item.CO.Value.ToString("0.####"));
                        fila.CreateCell(71).SetCellValue(item.CO2 == null ? "0.0" : item.CO2.Value.ToString("0.####"));
                        fila.CreateCell(72).SetCellValue(item.O2 == null ? "0.0" : item.O2.Value.ToString("0.####"));
                        fila.CreateCell(73).SetCellValue(item.NO == null ? "0.0" : item.NO.Value.ToString("0.####"));
                        fila.CreateCell(74).SetCellValue(item.LAMDA == null ? "0.0" : item.LAMDA.Value.ToString("0.####"));
                        fila.CreateCell(75).SetCellValue(item.FCNOX == null ? "0.0" : item.FCNOX.Value.ToString("0.####"));
                        fila.CreateCell(76).SetCellValue(item.FCDIL == null ? "0.0" : item.FCDIL.Value.ToString("0.####"));
                        fila.CreateCell(77).SetCellValue(item.RPM == null ? "0.0" : item.RPM.Value.ToString("0.####"));
                        fila.CreateCell(78).SetCellValue(item.KPH == null ? "0.0" : item.KPH.Value.ToString("0.####"));
                        fila.CreateCell(79).SetCellValue(item.VEL_LIN == null ? "0.0" : item.VEL_LIN.Value.ToString("0.####"));
                        fila.CreateCell(80).SetCellValue(item.VEL_ANG == null ? "0.0" : item.VEL_ANG.Value.ToString("0.####"));
                        fila.CreateCell(81).SetCellValue(item.BHP == null ? "0.0" : item.BHP.Value.ToString("0.####"));
                        fila.CreateCell(82).SetCellValue(item.PAR_TOR == null ? "0.0" : item.PAR_TOR.Value.ToString("0.####"));
                        fila.CreateCell(83).SetCellValue(item.FUERZA == null ? "0.0" : item.FUERZA.Value.ToString("0.####"));
                        fila.CreateCell(84).SetCellValue(item.POT_FRENO == null ? "0.0" : item.POT_FRENO.Value.ToString("0.####"));
                        fila.CreateCell(85).SetCellValue((item.TEMP ?? 0).ToString());
                        fila.CreateCell(86).SetCellValue((item.PRESION ?? 0).ToString());
                        fila.CreateCell(87).SetCellValue((item.HUMREL ?? 0).ToString());
                        fila.CreateCell(88).SetCellValue(item.OBD_TIPO_SDB);
                        fila.CreateCell(89).SetCellValue((item.OBD_MIL ?? 0).ToString());
                        fila.CreateCell(90).SetCellValue((item.OBD_CATAL ?? 0).ToString());
                        fila.CreateCell(91).SetCellValue((item.OBD_CILIN ?? 0).ToString());
                        fila.CreateCell(92).SetCellValue((item.OBD_COMBU ?? 0).ToString());
                        fila.CreateCell(93).SetCellValue((item.OBD_INTEG ?? 0).ToString());
                        fila.CreateCell(94).SetCellValue((item.OBD_OXIGE ?? 0).ToString());
                        fila.CreateCell(95).SetCellValue(item.LAMDA_5024 == null ? "0.0" : item.LAMDA_5024.Value.ToString("0.####"));
                        fila.CreateCell(96).SetCellValue(item.TEMP_5024 == null ? "0.0" : item.TEMP_5024.Value.ToString("0.####"));
                        fila.CreateCell(97).SetCellValue(item.HR_5024 == null ? "0.0" : item.HR_5024.Value.ToString("0.####"));
                        fila.CreateCell(98).SetCellValue(item.PSI_5024 == null ? "0.0" : item.PSI_5024.Value.ToString("0.####"));
                        fila.CreateCell(99).SetCellValue(item.FCNOX_5024 == null ? "0.0" : item.FCNOX_5024.Value.ToString("0.####"));
                        fila.CreateCell(100).SetCellValue(item.FCDIL_5024 == null ? "0.0" : item.FCDIL_5024.Value.ToString("0.####"));
                        fila.CreateCell(101).SetCellValue(item.RPM_5024 == null ? "0.0" : item.RPM_5024.Value.ToString("0.####"));
                        fila.CreateCell(102).SetCellValue(item.KPH_5024 == null ? "0.0" : item.KPH_5024.Value.ToString("0.####"));
                        fila.CreateCell(103).SetCellValue(item.THP_5024 == null ? "0.0" : item.THP_5024.Value.ToString("0.####"));
                        fila.CreateCell(104).SetCellValue(item.VOLTS_5024 == null ? "0.0" : item.VOLTS_5024.Value.ToString("0.####"));
                        fila.CreateCell(105).SetCellValue(item.HC_5024.ToString());
                        fila.CreateCell(106).SetCellValue(item.CO_5024 == null ? "0.0" : item.CO_5024.Value.ToString("0.####"));
                        fila.CreateCell(107).SetCellValue(item.CO2_5024 == null ? "0.0" : item.CO2_5024.Value.ToString("0.####"));
                        fila.CreateCell(108).SetCellValue(item.COCO2_5024 == null ? "0.0" : item.COCO2_5024.Value.ToString("0.####"));
                        fila.CreateCell(109).SetCellValue(item.O2_5024 == null ? "0.0" : item.O2_5024.Value.ToString("0.####"));
                        fila.CreateCell(110).SetCellValue(item.NO_5024.ToString());
                        fila.CreateCell(111).SetCellValue(item.LAMDA_2540 == null ? "0.0" : item.LAMDA_2540.Value.ToString("0.####"));
                        fila.CreateCell(112).SetCellValue(item.TEMP_2540 == null ? "0.0" : item.TEMP_2540.Value.ToString("0.####"));
                        fila.CreateCell(113).SetCellValue(item.HR_2540 == null ? "0.0" : item.HR_2540.Value.ToString("0.####"));
                        fila.CreateCell(114).SetCellValue(item.PSI_2540.ToString());
                        fila.CreateCell(115).SetCellValue(item.FCNOX_2540 == null ? "0.0" : item.FCNOX_2540.Value.ToString("0.####"));
                        fila.CreateCell(116).SetCellValue(item.FCDIL_2540 == null ? "0.0" : item.FCDIL_2540.Value.ToString("0.####"));
                        fila.CreateCell(117).SetCellValue(item.RPM_2540.ToString());
                        fila.CreateCell(118).SetCellValue(item.KPH_2540 == null ? "0.0" : item.KPH_2540.Value.ToString("0.####"));
                        fila.CreateCell(119).SetCellValue(item.THP_2540 == null ? "0.0" : item.THP_2540.Value.ToString("0.####"));
                        fila.CreateCell(120).SetCellValue(item.VOLTS_2540 == null ? "0.0" : item.VOLTS_2540.Value.ToString("0.####"));
                        fila.CreateCell(121).SetCellValue(item.HC_2540.ToString());
                        fila.CreateCell(122).SetCellValue(item.CO_2540 == null ? "0.0" : item.CO_2540.Value.ToString("0.####"));
                        fila.CreateCell(123).SetCellValue(item.CO2_2540 == null ? "0.0" : item.CO2_2540.Value.ToString("0.####"));
                        fila.CreateCell(124).SetCellValue(item.COCO2_2540 == null ? "0.0" : item.COCO2_2540.Value.ToString("0.####"));
                        fila.CreateCell(125).SetCellValue(item.O2_2540 == null ? "0.0" : item.O2_2540.Value.ToString("0.####"));
                        fila.CreateCell(126).SetCellValue(item.NO_2540.ToString());
                        fila.CreateCell(127).SetCellValue(item.OPACIDADP == null ? "0.0" : item.OPACIDADP.Value.ToString("0.####"));
                        fila.CreateCell(128).SetCellValue(item.OPACIDADK == null ? "0.0" : item.OPACIDADK.Value.ToString("0.####"));
                        fila.CreateCell(129).SetCellValue(item.TEMP_MOT.ToString());
                        fila.CreateCell(130).SetCellValue(item.VEL_GOB.ToString());
                        fila.CreateCell(131).SetCellValue(item.TEMP_GAS.ToString());
                        fila.CreateCell(132).SetCellValue(item.TEMP_CAM.ToString());
                        fila.CreateCell(133).SetCellValue(item.RESULTADO.ToString());
                        fila.CreateCell(134).SetCellValue(item.C_RECHAZO.ToString());
                        fila.CreateCell(135).SetCellValue(item.C_RECHAZO_OBD.ToString());
                        fila.CreateCell(136).SetCellValue((item.PruebaObd ?? false) ? "SÍ" : "NO");
                        fila.CreateCell(137).SetCellValue((item.PruebaEmisiones ?? false) ? "SÍ" : "NO");
                        fila.CreateCell(138).SetCellValue((item.PruebaOpacidad ?? false) ? "SÍ" : "NO");
                        fila.CreateCell(139).SetCellValue(item.EntidadProcedencia);
                        fila.CreateCell(140).SetCellValue(item.FolioCertificadoActual);
                        fila.CreateCell(141).SetCellValue(item.FolioCertificado);
                        fila.CreateCell(142).SetCellValue(item.ClaveTramite);
                        fila.CreateCell(143).SetCellValue(item.TipoCertificadoString);
                        fila.CreateCell(144).SetCellValue(item.VerificacionActiva);
                        //reporte.Hoja.AutoSizeColumn(autoSize);

                        index++;
                        autoSize++;
                    }
                    //reporte.Hoja.IsAutosizeColumn = false;
                    //var col = 0;
                    //foreach (var c in columnas)
                    //{
                    //    reporte.Hoja.AutoSizeColumn(col);
                    //    col++;
                    //}

                    //resp = ms.ToArray();
                    reporte.Libro.Write(ms, true);
                    ms.Seek(0, SeekOrigin.Begin);
                    resp = ms.ToArray();
                }
                return new ResponseGeneric<DataExcelReport>(new DataExcelReport
                {
                    NombreDocumento = archivoPDF,
                    DocumentoExcel = resp
                });
            }
            catch (Exception ex)
            {

                return new ResponseGeneric<DataExcelReport>(ex) { mensaje=ex.StackTrace};
            }
        }

        #region Inicializar Excel
        private (IWorkbook Libro, ISheet Hoja) InicializarReporte(List<string> columnas, string nombreHoja, string titulo)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ICellStyle style = workbook.CreateCellStyle();
            ICellStyle styleHeader = workbook.CreateCellStyle();
            ISheet reporte = workbook.CreateSheet(nombreHoja);
            IFont font = workbook.CreateFont();
            IRow header_title = reporte.CreateRow(0);
            IRow header = reporte.CreateRow(1);
            int i = 0;
            foreach (var item in columnas)
            {
                header.CreateCell(i).SetCellValue(item);
                i++;
            }
            style.FillForegroundColor = IndexedColors.Grey40Percent.Index;
            style.FillPattern = FillPattern.SolidForeground;
            font.Color = IndexedColors.White.Index;
            style.SetFont(font);
            style.BorderRight = BorderStyle.Double;


            header_title.CreateCell(0).SetCellValue(titulo.ToUpper());
            header_title.GetCell(0).CellStyle = style;
            header_title.GetCell(0).CellStyle.Alignment = HorizontalAlignment.Center;
            reporte.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, columnas.Count - 1));

            styleHeader.VerticalAlignment = VerticalAlignment.Center;
            styleHeader.FillForegroundColor = IndexedColors.Black.Index;
            styleHeader.FillPattern = FillPattern.SolidForeground;
            styleHeader.SetFont(font);
            styleHeader.BorderRight = BorderStyle.Double;
            //styleHeader.WrapText = true;
            //styleHeader.ShrinkToFit = true;

            var t = 0;
            foreach (var item in columnas)
            {
                header.GetCell(t).CellStyle = styleHeader;
                t++;
            }



            return (workbook, reporte);


        }

        #endregion



    }
}