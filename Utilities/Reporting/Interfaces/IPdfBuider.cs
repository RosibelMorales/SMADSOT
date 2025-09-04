using Smadot.Utilities.Modelos;
using Smadot.Utilities.Modelos.Documentos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Reporting.Interfaces
{
    public interface IPdfBuider
    {

        public Task<ResponseGeneric<DataReport>> GetDocumentoSolicitudFormaValorada(List<SolicitudFormaValoradaDocument> solicitud);
        public Task<ResponseGeneric<DataReport>> GetDocumentoCertificadoReposicion(CertificadoReposicionResponse solicitud);
        public Task<ResponseGeneric<DataReport>> GetDocumentoVentaCertificados(VentaCertificadoDocument ventaCertificados);

        public Task<ResponseGeneric<DataReport>> GetRefrendoCertificado(RefrendoCertificadoResponse refrendo);
        public Task<ResponseGeneric<DataReport>> GetConstanciaUltimaVerificacion(ConstanciaUltimaVerificacionResponse refrendo);

        public Task<ResponseGeneric<DataReport>> GetDocumentoCertificadoAdministrativa(CertificadoAdministrativaResponse data);

        public Task<ResponseGeneric<DataReport>> GetDocumentoFoliosUsadosEnVentanilla(List<FoliosUsadosEnVentanillaDocument> data);
        public Task<ResponseGeneric<DataReport>> GetDocumentoFoliosRegresadosSPF(List<FoliosRegresadosSPFDocument> data);
        public Task<ResponseGeneric<DataReport>> GetDocumentoFoliosVendidosCentroVerificacion(List<FoliosVendidosCentroVerificacionDocument> data);

        public Task<ResponseGeneric<DataReport>> GetDocumentoPersonal(PersonalVistaPreviaResponse vm);

        public Task<ResponseGeneric<DataReport>> GetComprobanteCita(ComprobanteCitaResponse data);

        public Task<ResponseGeneric<DataReport>> GetDevolucionSPF(List<DevolucionSPFDocument> data);

        public Task<ResponseGeneric<DataReport>> GetDocumentoVentaCVV(List<VentaCVVDocument> data);
        public Task<ResponseGeneric<DataReport>> GetDocumentoConsultaStockDVRF(List<ConsultaStockDVRFDocument> data);
        public Task<ResponseGeneric<DataReport>> GetImpresion(ImpresionPDFResponse data); public Task<ResponseGeneric<DataReport>> GetImpresionCertificado(ImpresionPDFResponse data, int certificado = 1);
        public Task<ResponseGeneric<DataReport>> GetDocumentoTablaMaestra(ConsultaTablaMaestraPDFUtilitiesResponse obj);

        public Task<ResponseGeneric<DataReport>> GetDocumentoHistorialCitas(HistorialCitasUtilitiesResponse obj);
    }
}
