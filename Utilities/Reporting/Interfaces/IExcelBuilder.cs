using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;

namespace Smadot.Utilities.Reporting.Interfaces
{
    public interface IExcelBuilder
    {
        public Task<ResponseGeneric<DataExcelReport>> GetDocumentoFoliosUsadosEnVentanilla(List<FoliosUsadosEnVentanillaDocument> data);
        public Task<ResponseGeneric<DataExcelReport>> GetDocumentoFoliosRegresadosSPF(List<FoliosRegresadosSPFDocument> data);
        public Task<ResponseGeneric<DataExcelReport>> GetDocumentoFoliosVendidosCentroVerificacion(List<FoliosVendidosCentroVerificacionDocument> data);
        public Task<ResponseGeneric<DataExcelReport>> GetDocumentoDevolucionSPF(List<DevolucionSPFDocument> data);

        public Task<ResponseGeneric<DataExcelReport>> GetDocumentoVentaCVV(List<VentaCVVDocument> data);
        public Task<ResponseGeneric<DataExcelReport>> GetDocumentoConsultaStockDVRF(List<ConsultaStockDVRFDocument> data);
        public Task<ResponseGeneric<DataExcelReport>> GetDocumentoTablaMaestra(ConsultaTablaMaestraPDFUtilitiesResponse data);

        public Task<ResponseGeneric<DataExcelReport>> GetDocumentoDashboardGrid(List<EstadisticasDashboardGridDocumentResponse> data);
        public Task<ResponseGeneric<DataExcelReport>> GetDocumentoHistorialCitas(HistorialCitasUtilitiesResponse data);
        public Task<ResponseGeneric<DataExcelReport>> ExcelDocumentCitasCanceladas(List<PersonaCita> data);
    }
}
