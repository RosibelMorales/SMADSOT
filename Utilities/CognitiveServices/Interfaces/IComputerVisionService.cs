using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smadot.Utilities.Modelos;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Smadot.Utilities.CognitiveServices.Interfaces
{
    public interface IComputerVisionService
    {
        public Task<ResponseGeneric<Line>> AnalyzeImage(byte[] imageFile);
    }
}
