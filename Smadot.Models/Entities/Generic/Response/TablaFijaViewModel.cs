using Smadot.Models.Dicts;

namespace Smadot.Models.Entities.Generic.Response
{
    public class TablaFijaViewModel
    {
        public class TablaFijaMain
        {
            public List<TablaFijaRow> Consultar(bool random = false)
            {
                var rdm = new Random();
                return new List<TablaFijaRow>
                {
                    new TablaFijaRow
                    {
                        IdTipoCertificado=TipoCertificado.DobleCero,
                        TipoCertificado = "Doble Cero \"00\"",
                        Cantidad = random ? rdm.Next(1,10) : 0,
                        FolioInicial = random ? 100: 0,
                        FolioFinal =  random ? 200: 0,
                        ClaveCerfiticado =  "1701005"
                    },
                    new TablaFijaRow
                    {
                        IdTipoCertificado=TipoCertificado.Cero,
                        TipoCertificado = "Cero \"0\"",
                        Cantidad = random ? rdm.Next(1,10) : 0,
                        FolioInicial = random ? 100: 0,
                        FolioFinal =  random ? 200: 0,
                        ClaveCerfiticado =  "1701004"
                    },
                    new TablaFijaRow
                    {
                        IdTipoCertificado=TipoCertificado.Uno,
                        TipoCertificado = "Uno \"1\"",
                        Cantidad = random ? rdm.Next(1,10) : 0,
                        FolioInicial = random ? 100: 0,
                        FolioFinal =  random ? 200: 0,
                        ClaveCerfiticado =  "1701002"
                    },
                    new TablaFijaRow
                    {
                        IdTipoCertificado=TipoCertificado.Dos,
                        TipoCertificado = "Dos \"2\"",
                        Cantidad = random ? rdm.Next(1,10) : 0,
                        FolioInicial = random ? 100: 0,
                        FolioFinal =  random ? 200: 0,
                        ClaveCerfiticado =  "1701002"
                    },
                    new TablaFijaRow
                    {
                        IdTipoCertificado=TipoCertificado.ConstanciasNoAprobado,
                        TipoCertificado = "Constancias de No Aprobado",
                        Cantidad = random ? rdm.Next(1,10) : 0,
                        FolioInicial = random ? 100: 0,
                        FolioFinal =  random ? 200: 0,
                        ClaveCerfiticado =  "1701006"
                    },
                    new TablaFijaRow
                    {
                        IdTipoCertificado=TipoCertificado.Exentos,
                        TipoCertificado = "Exentos",
                        Cantidad = random ? rdm.Next(1,10) : 0,
                        FolioInicial = random ? 100: 0,
                        FolioFinal =  random ? 200: 0,
                        ClaveCerfiticado =  "1701001"
                    },
                    new TablaFijaRow
                    {
                        IdTipoCertificado=TipoCertificado.Testificacion,
                        TipoCertificado = "\"T\" (TESTIFICACIÓN)",
                        Cantidad = random ? rdm.Next(1,10) : 0,
                        FolioInicial = random ? 100: 0,
                        FolioFinal =  random ? 200: 0,
                        ClaveCerfiticado =  "1701014"
                    }
                };
            }
        }
        public class TablaFijaRow
        {
            public string? TipoCertificado { get; set; }
            public int IdTipoCertificado { get; set; }
            public int Cantidad { get; set; }
            public int FolioInicial { get; set; }
            public int FolioFinal { get; set; }
            public string? ClaveCerfiticado { get; set; }
        }
        public class VentaCertificados
        {
            public int IdTipoCertificado { get; set; }
            public int Cantidad { get; set; }
            public int FolioInicial { get; set; }
            public int FolioFinal { get; set; }
        }
    }
}
