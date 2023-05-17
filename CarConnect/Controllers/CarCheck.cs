using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectBusinessLogic.BusinessLogics
{
    public class CarCheck
    {
        private CarNumberRecognizer numberRecognizer;

        public List<string> ProcessImage(IInputOutputArray image)
        {
            numberRecognizer = new CarNumberRecognizer(@"C:\Users\73bul\source\repos\CarCheck\CarCheck\TestData", "eng");
            List<IInputOutputArray> licensePlateImageList = new();
            List<IInputOutputArray> filtredlicensePlateImageList = new();
            List<RotatedRect> licenseBoxList = new();

            List<string> recognizedPlates = numberRecognizer.DetectLicensePlates(image, licensePlateImageList, filtredlicensePlateImageList, licenseBoxList);

            List<string> result = new List<string>();
            for (int i = 0; i < recognizedPlates.Count; i++)
            {
                result.Add(recognizedPlates[i]);
            }
            return result;
        }
    }
}
