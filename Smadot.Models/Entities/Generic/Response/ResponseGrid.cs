using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Generic.Response
{
    /// <summary>
    /// ViewModel de regresar una respuesta al frontend
    /// </summary>
    public class ResponseGrid<T>
    {
        #region Propiedades
        [JsonProperty("recordsTotal")]
        public int? RecordsTotal { get; set; }
        [JsonProperty("recordsFiltered")]
        public int? RecordsFiltered { get; set; }
        [JsonProperty("draw")]
        public string? Draw { get; set; }

        [JsonProperty("data")]
        public List<T> Data { get; set; }
        #endregion

        #region Constructores
        /// <summary>
        /// Constructor vacio.
        /// </summary>
        public ResponseGrid()
        {
            RecordsTotal = 0;
            RecordsFiltered = 0;
            Data = new List<T>();
        }


        #endregion
    }

}
