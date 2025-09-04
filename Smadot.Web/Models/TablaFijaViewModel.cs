namespace Smadot.Web.Models
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
                        TipoCertificado = "Doble Cero \"00\"",
                        Cantidad = random ? rdm.Next(1,10) : 0,
                        FolioInicial = random ? 100: 0,
                        FolioFinal =  random ? 200: 0,
                        ClaveCerfiticado =  "DOBLE CERO"
                    },
                    new TablaFijaRow
                    {
                        TipoCertificado = "Cero \"0\"",
                       Cantidad = random ? rdm.Next(1,10) : 0,
                        FolioInicial = random ? 100: 0,
                        FolioFinal =  random ? 200: 0,
                        ClaveCerfiticado =  "CERO"
                    },
                    new TablaFijaRow
                    {
                        TipoCertificado = "Uno \"1\"",
                        Cantidad = random ? rdm.Next(1,10) : 0,
                        FolioInicial = random ? 100: 0,
                        FolioFinal =  random ? 200: 0,
                        ClaveCerfiticado =  "UNO"
                    },
                    new TablaFijaRow
                    {
                        TipoCertificado = "Dos \"2\"",
                        Cantidad = random ? rdm.Next(1,10) : 0,
                        FolioInicial = random ? 100: 0,
                        FolioFinal =  random ? 200: 0,
                        ClaveCerfiticado =  "DOS"
                    },
                    new TablaFijaRow
                    {
                        TipoCertificado = "Constancias de No Aprobado",
                        Cantidad = random ? rdm.Next(1,10) : 0,
                        FolioInicial = random ? 100: 0,
                        FolioFinal =  random ? 200: 0,
                        ClaveCerfiticado =  "CONSTANCIAS"
                    },
                    new TablaFijaRow
                    {
                        TipoCertificado = "Exentos",
                        Cantidad = random ? rdm.Next(1,10) : 0,
                        FolioInicial = random ? 100: 0,
                        FolioFinal =  random ? 200: 0,
                        ClaveCerfiticado =  "EXCENTOS"
                    }
                };
            }
        }
        public class TablaFijaRow
        {
            public string TipoCertificado { get; set; }
            public int Cantidad { get; set; }
            public int FolioInicial { get; set; }
            public int FolioFinal { get; set; }
            public string ClaveCerfiticado { get; set; }
        }
    }
}
