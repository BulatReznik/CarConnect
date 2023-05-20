using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static System.Net.WebRequestMethods;

namespace CarConnect.Controllers
{
    class NumberCreate
    {
        public void CreateLicensePlateImage(string LicensePlate)
        {
            string region = "";
            string number = "";
            string country = "R U S";
            if (LicensePlate.Length >= 6)
            {
                number = LicensePlate[..6];

                if (LicensePlate.Length >= 8)
                {
                    region = LicensePlate[6..];
                }
            }
            // Создание нового объекта Bitmap с заданными размерами
            Bitmap numberBitmap = new Bitmap(235, 43);

            // Создание объекта Graphics на основе Bitmap
            using (Graphics graphics = Graphics.FromImage(numberBitmap))
            {
                graphics.FillRectangle(Brushes.White, 0, 0, 235, 43);
                // Настройка шрифтов и размеров
                Font plateFont = new Font(FontFamily.GenericSansSerif, 30, FontStyle.Bold, GraphicsUnit.Pixel);
                Font digitFont = new Font(FontFamily.GenericSansSerif, 40, FontStyle.Bold, GraphicsUnit.Pixel);
                Font regionFont = new Font(FontFamily.GenericSansSerif, 27, FontStyle.Bold, GraphicsUnit.Pixel);
                Font countryFont = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold, GraphicsUnit.Pixel);

                // Нарисовать номер на объекте Graphics
                float digitCount = 0;
                for (int i = 0; i < number.Length; i++)
                {
                    if (Char.IsDigit(number[i]))
                    {
                        graphics.DrawString(number[i].ToString(), digitFont, Brushes.Black, new PointF(10 + digitCount * 32, (44 - digitFont.Height) / 2));
                        digitCount += 0.75f;
                    }
                    else if (Char.IsLetter(number[i]))
                    {
                        graphics.DrawString(number[i].ToString(), plateFont, Brushes.Black, new PointF(10 + digitCount * 32, (46 - plateFont.Height) / 2));
                        digitCount += 0.7f;
                    }
                }

                graphics.DrawString(region, regionFont, Brushes.Black, new PointF(195 - region.Length * 7, (28 - regionFont.Height) / 2));
                graphics.DrawString(country, countryFont, Brushes.Black, new PointF(177, 26));

                Pen myPen;
                myPen = new Pen(Color.Black, 2);
                graphics.DrawLine(myPen, 175, 0, 175, 50);

                // Рисование обводки вокруг знака
                myPen = new Pen(Color.Black, 3);

                graphics.DrawRectangle(myPen, 1, 1, 232, 40);

                // Рисование флага России
                graphics.FillRectangle(Brushes.White, new Rectangle(208, 28, 17, 3));
                graphics.FillRectangle(Brushes.Blue, new Rectangle(208, 31, 17, 3));
                graphics.FillRectangle(Brushes.Red, new Rectangle(208, 34, 17, 3));

                myPen = new Pen(Color.Black, 0.5f);
                graphics.DrawRectangle(myPen, 208, 27, 17, 10);
            }

            // Сохранение Bitmap в файле с использованием значения LicensePlate в имени файла
            string folderPath = "C:/Users/73bul/source/repos/CarConnect/CarConnect/wwwroot/Files/";
            string fileName = "license_plate_" + LicensePlate + ".png";
            string filePath = Path.Combine(folderPath, fileName);

            numberBitmap.Save(filePath, ImageFormat.Png); // Можно выбрать другой формат, если нужно



            // Освобождение ресурсов
            numberBitmap.Dispose();
        }
    }
}
