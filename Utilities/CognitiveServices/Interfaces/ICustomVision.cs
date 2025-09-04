using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.CognitiveServices.Interfaces
{
    public interface ICustomVision
    {
        public ResponseGeneric<byte[]> ProcessLicensePlate(byte[] licensePlate);
    }
}
