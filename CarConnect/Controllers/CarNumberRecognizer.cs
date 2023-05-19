using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.Util;


namespace CarConnectBusinessLogic.BusinessLogics
{
    class CarNumberRecognizer : DisposableObject
    {
        private readonly Tesseract ocr;

        public CarNumberRecognizer(string testDataPath, string lang)
        {
            ocr = new Tesseract(testDataPath, lang, OcrEngineMode.TesseractLstmCombined);
        }

        /// <summary>
        /// Detects the license plates.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <param name="licensePlateImageList">The list of detected license plates images.</param>
        /// <param name="filtredLicensesPlateImageList">The list of filtered detected license plates images.</param>
        /// <param name="detectedLicensePlateRegionList">The list of detected license plates regions.</param>
        /// <returns>List of detected license plates numbers.</returns>

        public List<string> DetectLicensePlates(IInputArray image,
             List<IInputOutputArray> licensePlateImageList,
             List<IInputOutputArray> filtredLicensesPlateImageList,
             List<RotatedRect> detectedLicensePlateRegionList)
        {
            // Создаем пустой список строк для хранения номерных знаков.
            List<string> licenses = new List<string>();

            // Используем оператор using для автоматического освобождения памяти после работы с объектами Mat,
            // которые содержат изображение в формате матрицы пикселей.
            // Оператор using гарантирует, что объекты Mat будут удалены из памяти после выполнения блока кода внутри фигурных скобок.
            using (Mat gray = new Mat())
            {
                using (Mat canny = new Mat())
                {
                    // Создаем вектор векторов точек (контуров) для хранения контуров объектов на изображении.
                    using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                    {
                        // Преобразуем изображение из цветного в оттенки серого.
                        CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);

                        // Находим границы объектов на изображении с помощью алгоритма Canny.
                        // Алгоритм Canny используется для поиска резких изменений яркости на изображении, что может указывать на границы объектов.
                        CvInvoke.Canny(gray, canny, 100, 50, 3, false);

                        // Находим контуры объектов на изображении.
                        // Каждый контур представлен вектором точек, которые лежат на границе объекта.
                        // Массив hierachy содержит информацию о иерархии контуров.
                        int[,] hierachy = CvInvoke.FindContourTree(canny, contours, ChainApproxMethod.ChainApproxSimple);
                        // Находим номерные знаки на изображении и добавляем их в список licenses.
                        FindPlate(contours, hierachy, 0, gray, canny, licensePlateImageList, filtredLicensesPlateImageList, detectedLicensePlateRegionList, licenses);

                    }
                }
            }
            return licenses;
        }
        /// <summary>
        /// Recursively finds license plates.
        /// </summary>
        /// <param name="contours">The contours.</param>
        /// <param name="hierarhy">The hierarchy.</param>
        /// <param name="index">The current index.</param>
        /// <param name="gray">The gray image.</param>
        /// <param name="canny">The canny image.</param>
        /// <param name="licensePlateImageList">The list of detected license plates images.</param>
        /// <param name="filtredLicensePlateImageList">The list of filtered detected license plates images.</param>
        /// <param name="detectedLicensePlatRegionList">The list of detected license plates regions.</param>
        /// <param name="licenses">The list of detected license plates numbers.</param>

        private void FindPlate(
            VectorOfVectorOfPoint contours,
            int[,] hierarhy,
            int index,
            IInputArray gray,
            IInputArray canny,
            List<IInputOutputArray> licensePlateImageList,
            List<IInputOutputArray> filtredLicensePlateImageList,
            List<RotatedRect> detectedLicensePlatRegionList,
            List<string> licenses)
        {
            // Итерируемся по дереву иерархии контуров, начиная с индекса index,
            // который является индексом текущего контура.
            for (; index >= 0; index = hierarhy[index, 0])
            {
                // Получаем количество дочерних контуров у текущего контура.
                int numberOfChildren = GetNumberOfChildren(hierarhy, index);
                // Если количество дочерних контуров равно нулю, переходим к следующему контуру.
                if (numberOfChildren == 0)
                {
                    continue;
                }
                // Используем оператор using для автоматического освобождения памяти после работы с объектом VectorOfPoint.
                // Оператор using гарантирует, что объект VectorOfPoint будет удален из памяти после выполнения блока кода внутри фигурных скобок.
                using (VectorOfPoint contour = contours[index])
                {
                    // Если площадь текущего контура больше 400, продолжаем выполнение кода.
                    if (CvInvoke.ContourArea(contour) > 400)
                    {
                        // Если количество дочерних контуров меньше 3, находим номерной знак в дочерних контурах.
                        if (numberOfChildren < 3)
                        {
                            FindPlate(contours, hierarhy, hierarhy[index, 2], gray, canny,
                                licensePlateImageList, filtredLicensePlateImageList, detectedLicensePlatRegionList, licenses);
                            continue;
                        }
                        // Находим минимальный ограничивающий прямоугольник текущего контура.
                        RotatedRect box = CvInvoke.MinAreaRect(contour);
                        // Если угол наклона прямоугольника меньше -45.0 градусов, меняем местами длину и ширину прямоугольника.
                        if (box.Angle < -45.0)
                        {
                            float tmp = box.Size.Width;
                            box.Size.Width = tmp;

                            box.Angle += 90.0f;
                        }
                        // Если угол больше 45 градусов, мы меняем высоту и ширину box, а также угол поворота на -90 градусов, чтобы прямоугольник имел вертикальную ориентацию.
                        else if (box.Angle > 45.0)
                        {
                            (box.Size.Height, box.Size.Width) = (box.Size.Width, box.Size.Height);
                            box.Angle -= 90.0f;
                        }
                        double whRatio = (double)box.Size.Width / box.Size.Height;
                        //Проверяем, находится ли отношение ширины и высоты box вне диапазона от 3 до 10. Если да, мы проверяем, есть ли у текущего контура дочерние контуры.
                        //Если есть, мы вызываем метод FindPlate для каждого дочернего контура и продолжаем обработку.
                        //Если дочерних контуров нет, мы продолжаем поиск следующих контуров на этом уровне.
                        if (!(3.0 < whRatio && whRatio < 13.0))
                        {
                            if (hierarhy[index, 2] > 0)
                            {
                                FindPlate(contours, hierarhy, hierarhy[index, 2], gray, canny,
                                 licensePlateImageList, filtredLicensePlateImageList, detectedLicensePlatRegionList, licenses);
                                continue;
                            }
                        }
                        using (UMat tmp1 = new UMat())
                        {
                            using (UMat tmp2 = new UMat())
                            {
                                // Получение координат углов прямоугольника вокруг номера
                                PointF[] srcCorners = box.GetVertices();
                                // Задание координат углов для результирующего изображения
                                PointF[] destCorners = new PointF[]
                                {
                                    new PointF(0,box.Size.Height -1),
                                    new Point(0,0),
                                    new PointF(box.Size.Width - 1, 0),
                                    new PointF(box.Size.Width -1, box.Size.Height -1)
                                };
                                // Применяем аффинное преобразование, чтобы скорректировать искажения перспективы изображения
                                using (Mat rot = CvInvoke.GetAffineTransform(srcCorners, destCorners))
                                {
                                    CvInvoke.WarpAffine(gray, tmp1, rot, Size.Round(box.Size));
                                }
                                // Устанавливаем размер, который мы хотим получить после изменения масштаба изображения
                                Size approxSize = new Size(320, 240);
                                // Определяем масштаб для изменения размера изображения
                                double scale = Math.Min(approxSize.Width / box.Size.Width, approxSize.Height / box.Size.Height);

                                // Вычисляем новый размер изображения на основе масштаба
                                Size newSize = new Size((int)Math.Round(box.Size.Width * scale),
                                    (int)Math.Round(box.Size.Height * scale));

                                // Изменение размера изображения tmp1 до newSize с использованием кубической интерполяции и сохранение результата в tmp2.
                                CvInvoke.Resize(tmp1, tmp2, newSize, 0, 0, Inter.Cubic);

                                // Определение размера краевых пикселей и создание нового прямоугольника,
                                // который будет использоваться для выделения области на изображении tmp2.
                                int edgePixelSize = 2;
                                Rectangle newRoi = new Rectangle(new Point(edgePixelSize, edgePixelSize),
                                    tmp2.Size - new Size(2 * edgePixelSize, 2 * edgePixelSize));
                                // Создание UMat plate, который представляет собой область изображения tmp2, определенную новым прямоугольником newRoi.
                                UMat plate = new UMat(tmp2, newRoi);
                                // Применение фильтра к изображению plate и сохранение результата в filtredPlate.
                                UMat filtredPlate = FilterPlate(plate);
                                // Создание экземпляра StringBuilder для хранения текстовой информации о распознанных символах.
                                StringBuilder stringBuilder = new StringBuilder();

                                using (UMat tmp = filtredPlate.Clone())
                                {
                                    ocr.SetImage(tmp);
                                    ocr.Recognize();
                                    stringBuilder.Append(ocr.GetUTF8Text());
                                }
                                string str = stringBuilder.ToString().ToUpper();
                                // шаблон для замены символов, не входящих в список
                                string pattern = "[^ABCDEHKMOPTXY1234567890]";
                                // замена символов, не входящих в список, на пустую строку
                                string result = Regex.Replace(str, pattern, "");

                                if (!string.IsNullOrEmpty(result))
                                {
                                    licenses.Add(result);
                                }

                                licensePlateImageList.Add(plate);
                                filtredLicensePlateImageList.Add(filtredPlate);
                                detectedLicensePlatRegionList.Add(box);
                            }
                        }
                    }
                }
            }
        }
        private int GetNumberOfChildren(int[,] hierarchy, int index)
        {
            index = hierarchy[index, 2];
            if (index < 0)
            {
                return 0;
            }
            int count = 1;
            while (hierarchy[index, 0] > 0)
            {
                count++;
                index = hierarchy[index, 0];
            }
            return count;
        }
        private static UMat FilterPlate(UMat plate)
        {
            UMat thresh = new UMat();
            // Применяем пороговую фильтрацию к изображению "plate"
            // с пороговым значением 120. Пиксели с яркостью выше 120
            // становятся белыми, а пиксели с яркостью ниже 120 - черными.
            // Результат сохраняем в объекте thresh.
            CvInvoke.Threshold(plate, thresh, 130, 255, ThresholdType.BinaryInv);
            // Получаем размеры изображения "plate"
            Size plateSize = plate.Size;

            // Создаем матрицу - маску, которая будет содержать области,
            // не содержащие номерной знак. Изначально она заполнена белым цветом.
            using (Mat plateMask = new Mat(plateSize.Height, plateSize.Width, DepthType.Cv8U, 1))
            {
                using Mat plateCanny = new Mat();
                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                {
                    // Заполняем маску белым цветом
                    plateMask.SetTo(new MCvScalar(255.0));

                    CvInvoke.Canny(plate, plateCanny, 100, 50);

                    CvInvoke.FindContours(plateCanny, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                    int count = contours.Size;

                    for (int i = 0; i < count; i++)
                    {
                        using (VectorOfPoint contour = contours[i])
                        {
                            Rectangle rect = CvInvoke.BoundingRectangle(contour);
                            if (rect.Height > (plateSize.Height >> 1))
                            {
                                rect.X -= 1;
                                rect.Y -= 1;

                                rect.Width += 2;
                                rect.Height += 2;
                                Rectangle roi = new Rectangle(Point.Empty, plate.Size);

                                rect.Intersect(roi);

                                CvInvoke.Rectangle(plateMask, rect, new MCvScalar(), -1);
                            }
                        }
                    }
                    thresh.SetTo(new MCvScalar(), plateMask);
                }
            }
            CvInvoke.Erode(thresh, thresh, null, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);
            CvInvoke.Dilate(thresh, thresh, null, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);

            return thresh;
        }
        protected override void DisposeObject()
        {
            ocr.Dispose();
        }
    }
}
