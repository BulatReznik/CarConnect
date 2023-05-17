using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace CarConnectContracts.BusinessLogicsContracts
{
    public interface IRecognationLogic
    {
        void AddRecognations(UMat mat);
        List<string> ReadRecognitions();
    }
}
