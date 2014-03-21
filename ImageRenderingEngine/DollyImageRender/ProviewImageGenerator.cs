using BarcodeLib;
using ImageGenModels;
using ImageGenModels.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;



namespace ImageRender
{
    #region "Interface"

    public interface IProviewImageGenerator
    {
        byte[] GenerateLabelPreview(int labelTemplateID);

        byte[] GenerateLabelPreview(int labelTemplateID, int assetID);

        byte[] GenerateLabelPreviewTextOnly(int labelTemplateID, int? assetID);

        Dictionary<string, string> GetLabelParameters(int labelTemplateID);
    }

    #endregion

    #region "Class"

    public class ProviewImageGenerator : IProviewImageGenerator
    {
        #region "Private Methods"

        private bool ValidParameter(CustomImageField field)
        {
            if (field.CtrlType == ControlType.Barcode || field.CtrlType == ControlType.TextBlock || field.CtrlType == ControlType.Image)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private TYPE GetEquivalentBarcode(string telericBarcodeType)
        {
            string bcode = telericBarcodeType.Substring(telericBarcodeType.IndexOf(":") + 1).ToLower();
            TYPE bcodeType = TYPE.UNSPECIFIED;

            switch (bcode)
            {
                case "radbarcode128":
                    bcodeType = TYPE.CODE128;
                    break;
                case "radbarcode128a":
                    bcodeType = TYPE.CODE128A;
                    break;
                case "radbarcode128b":
                    bcodeType = TYPE.CODE128B;
                    break;
                case "radbarcode128c":
                    bcodeType = TYPE.CODE128C;
                    break;
                case "radbarcode39":
                    bcodeType = TYPE.CODE39;
                    break;
                case "radbarcode39extended":
                    bcodeType = TYPE.CODE39Extended;
                    break;
                case "radbarcodeean13":
                    bcodeType = TYPE.EAN13;
                    break;
                case "radbarcodeean8":
                    bcodeType = TYPE.EAN8;
                    break;
                case "radbarcodecodebar":
                    bcodeType = TYPE.Codabar;
                    break;
                case "radbarcode11":
                    bcodeType = TYPE.CODE11;
                    break;
                case "radbarcodemsi":
                    bcodeType = TYPE.MSI_Mod10;
                    break;
                case "radbarcodepostnet":
                    bcodeType = TYPE.PostNet;
                    break;
                case "radbarcode25standard":
                    bcodeType = TYPE.Standard2of5;
                    break;
                case "radbarcode25interleaved":
                    bcodeType = TYPE.Interleaved2of5;
                    break;
                case "radbarcode93":
                    bcodeType = TYPE.CODE93;
                    break;
                case "radbarcodeupca":
                    bcodeType = TYPE.UPCA;
                    break;
                case "radbarcodeupce":
                    bcodeType = TYPE.UPCE;
                    break;
                case "radbarcodeupcsupplement2":
                    bcodeType = TYPE.UPC_SUPPLEMENTAL_2DIGIT;
                    break;
                case "radbarcodeupcsupplement5":
                    bcodeType = TYPE.UPC_SUPPLEMENTAL_5DIGIT;
                    break;
                default:
                    throw new InvalidCastException("Invalid Barcode.");
            }
            return bcodeType;
        }

        private Bitmap DrawGrayScaleBitmapByCustomImageField(List<CustomImageField> imageInfo)
        {
            if (imageInfo == null || imageInfo.Count < 1)
            {
                throw new ArgumentNullException("imageInfo");
            }

            imageInfo = imageInfo.OrderBy(o => o.CtrlType).ToList();

            Bitmap bitmapImage = new Bitmap(imageInfo[0].DiagramWidth, imageInfo[0].DiagramHeight);
            using (Graphics graphic = Graphics.FromImage(bitmapImage))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                graphic.FillRectangle(new SolidBrush(Color.Transparent), new Rectangle(new Point(0, 0), new Size(bitmapImage.Width, bitmapImage.Height))); // fill with white pixel.
                graphic.DrawRectangle(new Pen(brushBlack), new Rectangle(new Point(0, 0), new Size(bitmapImage.Width, bitmapImage.Height)));
                foreach (CustomImageField imageField in imageInfo)
                {
                    FontStyle fs = (imageField.FontStyle == FontType.Regular) ? FontStyle.Regular : FontStyle.Bold;
                    switch (imageField.CtrlType)
                    {
                        case ControlType.TextBlock:
                            DrawText(graphic, imageField, fs);
                            break;
                        case ControlType.Barcode:
                            DrawBarcode(graphic, imageField, fs);
                            break;
                        case ControlType.Rectangle:
                            DrawlRectangle(graphic, brushBlack, imageField);
                            break;
                        case ControlType.Line:
                            DrawLine(graphic, brushBlack, imageField);
                            break;
                        case ControlType.Image:
                            DrawImage(graphic, imageField);
                            break;
                    }
                }
                graphic.Dispose();
            }
            return bitmapImage.ToGrayscaleBitmap();
        }

        private void GetBitmapSize(List<CustomImageField> imageInfo, out int width, out int height)
        {
            width = imageInfo.FirstOrDefault().BitmapWidth;
            height = imageInfo.FirstOrDefault().BitmapHeight;

            if (width == 0) width = 525;
            if (height == 0) height = 360;
        }

        private void DrawText(Graphics graphic, CustomImageField imageField, FontStyle fs)
        {
            Font font = new Font(new FontFamily(imageField.FontFamily), imageField.FontSize, fs);
            if (imageField.RotationAngle != 0)
            {
                graphic.ResetTransform();
                graphic.TranslateTransform(imageField.X + (imageField.Width / 2.0f), imageField.Y + (imageField.Height / 2.0f));
                graphic.RotateTransform(imageField.RotationAngle);
                graphic.DrawString(imageField.Text, font, Brushes.Black, -imageField.Width / 2.0f, -imageField.Height / 2.0f, StringFormat.GenericDefault);
                graphic.ResetTransform();
            }
            else
            {
                graphic.DrawString(imageField.Text, font, Brushes.Black, imageField.X, imageField.Y, StringFormat.GenericDefault);
            }
        }

        private void DrawBarcode(Graphics graphic, CustomImageField imageField, FontStyle fs)
        {
            Font font = new Font(new FontFamily(imageField.FontFamily), imageField.FontSize, fs);
            Barcode bcode = new Barcode();
            bcode.IncludeLabel = true;
            bcode.LabelPosition = LabelPositions.BOTTOMCENTER;
            bcode.LabelFont = font;
            Image bcodeImage = null;
            try
            {
                bcodeImage = bcode.Encode(imageField.BarcodeType, imageField.Text);
            }
            catch
            {
                bcodeImage = Image.FromFile(@"Images\Error.jpg");
            }
            if (imageField.RotationAngle != 0)
            {
                graphic.ResetTransform();
                graphic.TranslateTransform(imageField.X + (imageField.Width / 2.0f), imageField.Y + (imageField.Height / 2.0f));
                graphic.RotateTransform(imageField.RotationAngle);
                graphic.DrawImage(bcodeImage, -imageField.Width / 2.0f, -imageField.Height / 2.0f, imageField.Width, imageField.Height);
                graphic.ResetTransform();
            }
            else
            {
                graphic.DrawImage(bcodeImage, imageField.X, imageField.Y, imageField.Width, imageField.Height);
            }
        }

        private void DrawlRectangle(Graphics graphic, SolidBrush brushBlack, CustomImageField imageField)
        {
            Pen pen = new Pen(brushBlack);
            pen.Width = imageField.StrokeThickness;
            if (imageField.RotationAngle != 0)
            {
                graphic.ResetTransform();
                graphic.TranslateTransform(imageField.X + (imageField.Width / 2.0f), imageField.Y + (imageField.Height / 2.0f));
                graphic.RotateTransform(imageField.RotationAngle);
                graphic.DrawRectangle(pen, -imageField.Width / 2.0f, -imageField.Height / 2.0f, imageField.Width, imageField.Height);
                graphic.ResetTransform();
            }
            else
            {
                graphic.DrawRectangle(pen, imageField.X, imageField.Y, imageField.Width, imageField.Height);
            }
        }

        private void DrawLine(Graphics graphic, SolidBrush brushBlack, CustomImageField imageField)
        {
            Pen pen = new Pen(brushBlack);
            pen.Width = imageField.StrokeThickness;
            if (imageField.RotationAngle != 0)
            {
                graphic.ResetTransform();
                graphic.TranslateTransform(imageField.X + ((imageField.X2 - imageField.X1) / 2.0f), imageField.Y + ((imageField.Y2 - imageField.Y1) / 2.0f));
                graphic.RotateTransform(imageField.RotationAngle);

                graphic.DrawLine(pen, new PointF(-((imageField.X2 - imageField.X1) / 2.0f), -((imageField.Y2 - imageField.Y1) / 2.0f)),
                    new PointF(imageField.X2 - ((imageField.X2 - imageField.X1) / 2.0f), imageField.Y2 - ((imageField.Y2 - imageField.Y1) / 2.0f)));
                graphic.ResetTransform();
            }
            else
            {
                graphic.ResetTransform();
                graphic.TranslateTransform(imageField.X, imageField.Y);
                graphic.DrawLine(pen, new PointF(imageField.X1, imageField.Y1),
                    new PointF(imageField.X2, imageField.Y2));
                graphic.ResetTransform();
            }
        }

        private void DrawImage(Graphics graphic, CustomImageField imageField)
        {
            Image newImage = Image.FromFile(imageField.Text);
            float imageHeight = (newImage.Height > imageField.Height) ? imageField.Height : newImage.Height;
            float imageWidth = (newImage.Width > imageField.Width) ? imageField.Width : newImage.Width;
            if (imageField.RotationAngle != 0)
            {
                graphic.ResetTransform();
                graphic.TranslateTransform(imageField.X + (imageField.Width / 2.0f), imageField.Y + (imageField.Height / 2.0f));
                graphic.RotateTransform(imageField.RotationAngle);
                graphic.DrawImage(newImage, -imageField.Width / 2.0f, -imageField.Height / 2.0f, imageWidth, imageHeight);
                graphic.ResetTransform();
            }
            else
            {
                graphic.DrawImage(newImage, imageField.X, imageField.Y, imageWidth, imageHeight);
            }
        }

        private List<CustomImageField> XMLToCustomImageFieldConvertor(string xmlData)
        {
            if (string.IsNullOrEmpty(xmlData))
            {
                throw new ArgumentNullException("xmlData");
            }
            var toReplace = "xmlns=&quot;http://schemas.microsoft.com/client/2007&quot;";
            xmlData = xmlData.Replace(toReplace, "");
            toReplace = "xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot;";
            xmlData = xmlData.Replace(toReplace, "");
            xmlData = (xmlData ?? string.Empty).Trim();

            var diagramContents = XElement.Parse(xmlData);

            var lstRadDiagramFields = (
                                         from a in diagramContents.Element("Shapes").Elements("RadDiagramShape")
                                         select new
                                         {
                                             Content = a.Attribute("Content").Value,
                                             Position = a.Attribute("Position").Value,
                                             RotationAngle = a.Attribute("RotationAngle").Value,
                                         }
                                   ).ToList();

            if (lstRadDiagramFields == null || lstRadDiagramFields.Count < 1)
            {
                // Log - No data found in xml data
                return null;
            }
            List<CustomImageField> imageInfo = new List<CustomImageField>();
            foreach (var field in lstRadDiagramFields)
            {
                XElement control = XElement.Parse(field.Content).Element("ContentControl").Elements().FirstOrDefault();
                XElement customEntity = control.Elements().Where(o => o.Name.LocalName.Contains("DataContext")).FirstOrDefault().Elements().FirstOrDefault();
                CustomImageField objCustomImageField = new CustomImageField()
                {
                    FontFamily = (customEntity.Attribute("FontFamily").ToValue().Equals("Portable User Interface", StringComparison.CurrentCultureIgnoreCase)) ? "Microsoft Sans Serif" : customEntity.Attribute("FontFamily").ToValue(),
                    FontSize = customEntity.Attribute("FontSize").ToValue().ToFloat(),
                    FontStyle = customEntity.Attribute("FontStyle").ToValue().ToFontType(),
                    Height = customEntity.Attribute("Height").ToValue().ToFloat(),
                    RotationAngle = field.RotationAngle.ToFloat(),
                    Text = customEntity.Attribute("Text").ToValue(),
                    Width = customEntity.Attribute("Width").ToValue().ToFloat(),
                    DataSourceName = customEntity.Attribute("DataSourceName").ToValue(),
                    DiagramWidth = customEntity.Attribute("DiagramWidth").ToValue().ToInt(),
                    DiagramHeight = customEntity.Attribute("DiagramHeight").ToValue().ToInt(),
                };
                string[] position = field.Position.Split(';');
                if (position.Length > 1)
                {
                    objCustomImageField.X = position[0].ToFloat();
                    objCustomImageField.Y = position[1].ToFloat();
                }
                switch (control.Name.LocalName)
                {
                    case "TextBlock":
                        objCustomImageField.CtrlType = ControlType.TextBlock;
                        break;
                    case "Line":
                        objCustomImageField.CtrlType = ControlType.Line;
                        objCustomImageField.X1 = customEntity.Attribute("X1").Value.ToFloat();
                        objCustomImageField.X2 = customEntity.Attribute("X2").Value.ToFloat();
                        objCustomImageField.Y1 = customEntity.Attribute("Y1").Value.ToFloat();
                        objCustomImageField.Y2 = customEntity.Attribute("Y2").Value.ToFloat();
                        objCustomImageField.Height = customEntity.Attribute("LineHeight").Value.ToFloat();
                        objCustomImageField.Width = customEntity.Attribute("LineWidth").Value.ToFloat();
                        objCustomImageField.StrokeThickness = customEntity.Attribute("StrokeThickness").Value.ToFloat();
                        break;
                    case "Rectangle":
                        objCustomImageField.CtrlType = ControlType.Rectangle;
                        objCustomImageField.StrokeThickness = customEntity.Attribute("StrokeThickness").Value.ToFloat();
                        break;
                    case "Image":
                        objCustomImageField.CtrlType = ControlType.Image;
                        break;
                    default:
                        objCustomImageField.CtrlType = ControlType.Barcode;
                        objCustomImageField.BarcodeType = GetEquivalentBarcode(control.Name.LocalName);
                        break;
                }
                imageInfo.Add(objCustomImageField);
            }
            return imageInfo;
        }

        private Dictionary<string, string> ExtractLabelParameters(string xmlData)
        {
            List<CustomImageField> lstCustomImageField = XMLToCustomImageFieldConvertor(xmlData);
            Dictionary<string, string> parameters = FetchParameters(lstCustomImageField);
            return parameters;
        }

        private Dictionary<string, string> FetchParameters(List<CustomImageField> lstCustomImageField)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            foreach (var customField in lstCustomImageField)
            {
                if ((customField.CtrlType == ControlType.Barcode || customField.CtrlType == ControlType.Image || customField.CtrlType == ControlType.TextBlock) && !string.IsNullOrEmpty(customField.DataSourceName))
                    parameters.Add(customField.DataSourceName, customField.CtrlType.ToString());
            }
            return parameters;
        }

        private string GetXMLByLabelTemplateID(int labelTemplateID)
        {
            using (AssetTrackingEntities context = new ImageGenModels.Models.AssetTrackingEntities())
            {
                var template = context.LabelTemplates.Where(o => o.FK_ID_LabelTemplateType == 2 && o.ID == labelTemplateID).FirstOrDefault();
                string proviewXML = string.Empty;
                if (template != null)
                {
                    proviewXML = template.ProviewXML;
                }
                return proviewXML;
            }
        }

        private void SetParametersValue(List<CustomImageField> lstCustomImageField, Dictionary<string, string> parameters)
        {
            foreach (KeyValuePair<string, string> entry in parameters)
            {
                var field = lstCustomImageField.Where(o => o.DataSourceName == entry.Key).FirstOrDefault();
                if (field != null && ValidParameter(field))
                {
                    field.Text = entry.Value;
                }
            }
        }

        #endregion

        #region "Public Methods"

        public byte[] GenerateLabelPreview(int labelTemplateID)
        {
            byte[] data = null;
            string xmlData = GetXMLByLabelTemplateID(labelTemplateID);
            if (!string.IsNullOrEmpty(xmlData))
            {
                List<CustomImageField> lstCustomImageField = XMLToCustomImageFieldConvertor(xmlData);
                Bitmap bitmap = DrawGrayScaleBitmapByCustomImageField(lstCustomImageField);
                data = bitmap.ToByteArray();
            }
            return data;
        }

        public byte[] GenerateLabelPreview(int labelTemplateID, int assetID)
        {
            byte[] data = null;
            string xmlData = GetXMLByLabelTemplateID(labelTemplateID);
            if (!string.IsNullOrEmpty(xmlData))
            {
                List<CustomImageField> lstCustomImageField = XMLToCustomImageFieldConvertor(xmlData);
                Dictionary<string, string> parameters = FetchParameters(lstCustomImageField);
                SetParametersValue(lstCustomImageField, parameters);
                Bitmap bitmap = DrawGrayScaleBitmapByCustomImageField(lstCustomImageField);
                data = bitmap.ToByteArray();
            }
            return data;
        }

        public Dictionary<string, string> GetLabelParameters(int labelTemplateID)
        {
            string xmlData = GetXMLByLabelTemplateID(labelTemplateID);
            Dictionary<string, string> parameters = null;
            if (!string.IsNullOrEmpty(xmlData))
            {
                parameters = ExtractLabelParameters(xmlData);
            }
            return parameters;
        }

        public byte[] GenerateLabelPreviewTextOnly(int labelTemplateID, int? assetID = null)
        {
            byte[] data = null;
            string xmlData = GetXMLByLabelTemplateID(labelTemplateID);
            if (!string.IsNullOrEmpty(xmlData))
            {
                List<CustomImageField> lstCustomImageField = XMLToCustomImageFieldConvertor(xmlData).Where(o => o.CtrlType == ControlType.TextBlock).ToList();
                Bitmap bitMap = null;

                if (lstCustomImageField != null && lstCustomImageField.Count() > 0)
                {
                    if (assetID != null)
                    {
                        Dictionary<string, string> parameters = FetchParameters(lstCustomImageField);
                        SetParametersValue(lstCustomImageField, parameters);
                    }
                    bitMap = DrawGrayScaleBitmapByCustomImageField(lstCustomImageField);
                }
                data = bitMap.ToByteArray();
            }
            return data;
        }

        #endregion
    }

    #endregion

    #region "Extension Class"

    public static class Extension
    {
        public static string ToValue(this XAttribute data)
        {
            string result = string.Empty;
            if (data != null)
            {
                result = data.Value;
            }
            return result;
        }

        public static FontType ToFontType(this string data)
        {
            switch (data)
            {
                case "Bold":
                case "ExtraBlack":
                case "ExtraBold":
                case "SemiBold":
                    return FontType.Bold;
                default:
                    return FontType.Regular;
            }
        }

        public static float ToFloat(this string data)
        {
            float result = 0.0f;

            float.TryParse(data, out result);
            return result;
        }

        public static int ToInt(this string data)
        {
            int result = 0;

            int.TryParse(data, out result);
            return result;
        }

        public static Bitmap ToGrayscaleBitmap(this Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);
            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);
            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
              {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
              });
            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();
            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);
            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }

        public static byte[] ToByteArray(this Bitmap image)
        {
            ImageConverter imageConverter = new ImageConverter();
            byte[] imageByte = (byte[])imageConverter.ConvertTo(image, typeof(byte[]));
            return imageByte;
        }
    }

    #endregion
}
