using MathNet.Numerics.Distributions;
using NPOI.SS.Formula.Functions;
using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NPOI.HSSF.Util.HSSFColor;

namespace Smadot.Models.Dicts.ProveedorDicts
{
    public struct Etapa
    {
        public const string HUMO = "HUMO";
        public const string PAS5024 = "PAS5024";
        public const string PAS2540 = "PAS2540";
        public const string CRUCERO = "CRUCERO";
        public const string RALENTI = "RALENTI";
    }
    public struct OBD
    {
        public const int SinCodigoError = 0;
        public const int CodigoError = 1;
        public const int FallaConexion = 2;
        public const int MonitorNoDisponible = 9;

        public static Dictionary<int, string> DictOBD = new Dictionary<int, string>
                        {
                            {SinCodigoError, "No se detecta código de error." },
                            {CodigoError, "Código de error detectado" },
                            {FallaConexion, "Monitor no disponible" },
                            {MonitorNoDisponible, "Monitor no disponible" },
        };



    }
    public struct PBV
    {
        public static Dictionary<int, string> DictPBV = new Dictionary<int, string>
                        {
                            {1, "300 - 2000 kgs" },
                            {2, "2001 - 3000 kgs" },
                            {3, "3001 + kgs" },
        };
    }
    public struct Combustible
    {
        public static Dictionary<int, string> DictCombustible = new Dictionary<int, string>
                        {
                            {1, "Gasolina" },
                            {2, "Diesel" },
                            {3, "Gas Lp" },
                            {4, "Gas Natural" },
                        };
        public const int Gasolina = 1;
        public const int Diesel = 2;
        public const int GasLp = 3;
        public const int GasNat = 4;
    }
    public struct Protocolos
    {
        public const int DESCONOCIDO = 0;
        public const int ESTATICO = 2;
        public const int DIESELSINDATOS = 5;
        public const int ACELERACIONDIESEL = 6;
        public const int DINAMICO = 7;
        public static Dictionary<int, string> DictProtocolo = new Dictionary<int, string>
                        {
                            {DESCONOCIDO, "Desconocido" },
                            {ESTATICO, "Estático (TSI)" },
                            {ACELERACIONDIESEL, "Aceleración libre (Diesel)" },
                            {DINAMICO, "dinámico (ASM)" },
                            {DIESELSINDATOS, "Aceleración libre (Diesel)" },

        };
    }
    public struct Resultados
    {
        public static Dictionary<int, string> DictResultados = new Dictionary<int, string>
                        {
                            {Escapado, "Resultado Sin Procesar" },
                            {Cero, "Tipo 0" },
                            {Uno, "Tipo 1" },
                            {Dos, "Tipo 2" },
                            {DobleCero, "Tipo 00" },
                            {Rechazo, "Rechazo" },
                        };
        public static Dictionary<int, int> DictCertificadoEquivalencia = new Dictionary<int, int>
                        {
                            {Escapado,Escapado},
                            {TipoCertificado.Cero,Cero},
                            {TipoCertificado.Uno,Uno},
                            {TipoCertificado.Dos,Dos},
                            {TipoCertificado.DobleCero,DobleCero},
                            {TipoCertificado.ConstanciasNoAprobado,Rechazo},
                        };


        public const int Escapado = 0;
        public const int Cero = 1;
        public const int Uno = 2;
        public const int Dos = 3;
        public const int DobleCero = 4;
        public const int Rechazo = 5;
    }
    public struct CausaRechazo
    {
        public static Dictionary<int, string> DictClaveRechazo = new Dictionary<int, string>
                        {
                            {NoAplica, "No Aplica" },
                            {CertificadoAnteriorRobado, "Certificado Anterior Robado" },
                            {Visual, "Falla Prueba Visual" },
                            {HumoCisible, "Humo visible" },
                            {Rpm, "RPM" },
                            {VehículoFueraEspecificacionesMecanicas, "Vehículo Fuera de especificaciones mecánicas" },
                            {Dilucion, "Dilución" },
                            {Emisiones, "Emisiones" },
                            {LamdaDurantePrueba, "Lamda durante la prueba" },
                            {FallaEquipoAnalizador, "Falla del equipo analizador" },
                            {OBD, "OBD" },
                            {AbortadaTecnico, "Prueba Abortada por el Técnico Verificador" },
                        };
        public static Dictionary<int, string> DictPruebaAbortada = new Dictionary<int, string>
                        {
                            {CertificadoAnteriorRobado, "Certificado Anterior Robado" },
                            {Visual, "Falla Prueba Visual" },
                            {HumoCisible, "Humo visible" },
                            {Rpm, "rpm" },
                            {VehículoFueraEspecificacionesMecanicas, "Vehículo Fuera de especificaciones mecánicas" },
                            {Dilucion, "Dilución" },
                            {LamdaDurantePrueba, "Lamda durante la prueba." },
                            {FallaEquipoAnalizador, "Falla del equipo analizador." },
                            {AbortadaTecnico, "Prueba Abortada por el Técnico Verificador" },
                        };
        public static Dictionary<int, string> DictPruebaAbortadaApp = new Dictionary<int, string>
                        {
                            {AbortadaTecnico, "Prueba Abortada por el Técnico Verificador" },
                            {VehículoFueraEspecificacionesMecanicas, "Vehículo Fuera de especificaciones mecánicas" },
                            {Visual, "Falla Prueba Visual" },
                            {FallaEquipoAnalizador, "Falla del equipo analizador." },
                            {LamdaDurantePrueba, "Lamda durante la prueba." },
                            {Dilucion, "Dilución" },
                            // {HumoCisible, "Humo visible" },
                            // {Rpm, "rpm" },
                            {CertificadoAnteriorRobado, "Certificado Anterior Robado" },
                        };
        public const int NoAplica = 0;
        public const int CertificadoAnteriorRobado = 1;
        public const int Visual = 2;
        public const int HumoCisible = 3;
        public const int Rpm = 4;
        public const int VehículoFueraEspecificacionesMecanicas = 5;
        public const int Dilucion = 6;
        public const int Emisiones = 7;
        public const int LamdaDurantePrueba = 8;
        public const int FallaEquipoAnalizador = 10;
        public const int OBD = 11;
        public const int AbortadaTecnico = 12;

    }
}
