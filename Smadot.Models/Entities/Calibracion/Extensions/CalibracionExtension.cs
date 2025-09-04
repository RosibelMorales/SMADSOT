using Smadot.Models.Dicts;

namespace Smadot.Models.Entities.Calibracion.Extensions
{
    public static class CalibracionExtension
    {
        public static DateTime ObtenerProximaFechaCalibracion(this DateTime fechaCalibracion, int idEquipoTipoCalibracion)
        {
            DateTime fechaProximaCalibracion = fechaCalibracion;
            switch (idEquipoTipoCalibracion)
            {
                case CatTipoCalibracionDic.Anual:
                    fechaProximaCalibracion = fechaCalibracion.AddYears(1);
                    break;
                case CatTipoCalibracionDic.Semestral:
                    fechaProximaCalibracion = fechaCalibracion.AddMonths(6);
                    break;
                case CatTipoCalibracionDic.Trimestral:
                    fechaProximaCalibracion = fechaCalibracion.AddMonths(3);
                    break;
                case CatTipoCalibracionDic.Mensual:
                    fechaProximaCalibracion = fechaCalibracion.AddMonths(1);
                    break;
                case CatTipoCalibracionDic.Quincenal:
                    fechaProximaCalibracion = fechaCalibracion.AddDays(15);
                    break;
                case CatTipoCalibracionDic.Semanal:
                    fechaProximaCalibracion = fechaCalibracion.AddDays(7);
                    break;
                case CatTipoCalibracionDic.Diario:
                    fechaProximaCalibracion = fechaCalibracion.AddDays(1);
                    break;
            }
            return fechaProximaCalibracion;
        }
    }
}
