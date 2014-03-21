using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;     //for DataContract
using System.Drawing;
using System.Windows.Forms;
using ImageGenModels;
using Zen.Barcode;
using Barcode = BarcodeLib.Barcode;       // from bin/debug/BarcodeLib.dll, for various barcode specs (Code 39, Code 128, etc)
using AlignmentPositions = BarcodeLib.AlignmentPositions;

namespace ImageRender
{
    public static class CoralImage
    {
        private static readonly int S5_HEIGHT = 240;
        private static readonly int S5_WIDTH = 320;
        private static readonly int S3_HEIGHT = 160;
        private static readonly int S3_WIDTH = 400;
        private static readonly int S1_HEIGHT = 90;
        private static readonly int S1_WIDTH = 224;

        public struct TextDrawingInfo
        {
            public string text;
            public Point position;
            public Size size;
            public Font font;
            public bool blackground;
            public bool isbarcode;  // deprecated
            public BarcodeType barcodeType;
        }

        public static Bitmap CreateImageForP33(List<ImageField> imageInfo)
        {
            if (imageInfo == null) throw new ArgumentNullException();
            // Requires 3 elements in imageInfo. If they are not supplied is filled with empty data
            while (imageInfo.Count < 3)
            {
                imageInfo.Add(new ImageField()
                {
                    Field = " ",
                    BarcodeType = BarcodeType.NONE
                });
            }

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            // NOT USED
            // const int line2_Y = 10;
            // const int line3_Y = 50;
            // const int line4_Y = 70;

            TextDrawingInfo SortPlan_Info = new TextDrawingInfo();
            SortPlan_Info.text = imageInfo[0].Field;
            SortPlan_Info.position = new Point(5, 5);
            SortPlan_Info.size = new Size(90, 50);
            SortPlan_Info.font = new Font("Arial", 15, FontStyle.Bold);
            SortPlan_Info.blackground = false;
            SortPlan_Info.barcodeType = imageInfo[0].BarcodeType;
            textinfo_list.Add(SortPlan_Info);

            TextDrawingInfo DeliveryDate_Info = new TextDrawingInfo();
            DeliveryDate_Info.text = imageInfo[1].Field;
            int xStartPoint = GetDollyDayStartPoint(imageInfo[1].Field);
            DeliveryDate_Info.position = new Point(5, 50);
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            // DeliveryDate_Info.size = new Size((224 - xStartPoint), (line3_Y - line2_Y) + 10);
            DeliveryDate_Info.font = new Font("Arial", 25, FontStyle.Bold);
            DeliveryDate_Info.blackground = false;
            DeliveryDate_Info.barcodeType = imageInfo[1].BarcodeType;
            textinfo_list.Add(DeliveryDate_Info);

            TextDrawingInfo Zipcodes_Info = new TextDrawingInfo();
            Zipcodes_Info.text = GetZipCodeTextFormatted(imageInfo[2].Field);
            Zipcodes_Info.position = new Point(20, 120);
            Zipcodes_Info.size = new Size(220, 50);
            Zipcodes_Info.font = new Font("Arial", 11, FontStyle.Regular);
            Zipcodes_Info.blackground = false;
            Zipcodes_Info.barcodeType = imageInfo[2].BarcodeType;
            textinfo_list.Add(Zipcodes_Info);

            //    // Brush string to the bitmap
            //    // initialize a bitmap with 240 x 96 in pixel, EP55 pixel size, and the format is 1 bit for 1 pixel.            
            Bitmap tmp = new Bitmap(264, 176);
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);
                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.

                for (int i = 0; i < textinfo_list.Count; i++)
                {
                    TextDrawingInfo ei = textinfo_list[i];
                    if (ei.barcodeType == BarcodeType.ONE_D)
                    {  // add a 1D Code39 barcode
                        Barcode bcode = new Barcode();
                        try
                        {
                            if (!ei.blackground)
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, ei.size.Width, ei.size.Height);
                            }
                            else
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                            }
                            graphic.DrawImage(bcode.EncodedImage, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }

                    }
                    else if (ei.barcodeType == BarcodeType.TWO_D)
                    {
                        try
                        {
                            Image bm = CreateQRCodeBitmap(ei.text, ei.size.Height);
                            graphic.DrawImage(bm, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    if (!ei.isbarcode)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                }
                graphic.Dispose();
            }

            return ConvertBitmapToMonochrome(tmp);
        }

        public static Bitmap CreateImageForE2(List<ImageField> imageInfo)
        {
            if (imageInfo == null) throw new ArgumentNullException();
            // Requires 3 elements in imageInfo. If they are not supplied is filled with empty data
            while (imageInfo.Count < 3)
            {
                imageInfo.Add(new ImageField()
                {
                    Field = " ",
                    BarcodeType = BarcodeType.NONE
                });
            }

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            // NOT USED
            // const int line2_Y = 10;
            // const int line3_Y = 50;
            // const int line4_Y = 70;

            TextDrawingInfo SortPlan_Info = new TextDrawingInfo();
            SortPlan_Info.text = imageInfo[0].Field;
            SortPlan_Info.position = new Point(5, 5);
            SortPlan_Info.size = new Size(45, 25);
            SortPlan_Info.font = new Font("Arial", 8, FontStyle.Bold);
            SortPlan_Info.blackground = false;
            SortPlan_Info.barcodeType = imageInfo[0].BarcodeType;
            textinfo_list.Add(SortPlan_Info);

            TextDrawingInfo DeliveryDate_Info = new TextDrawingInfo();
            DeliveryDate_Info.text = imageInfo[1].Field;
            int xStartPoint = GetDollyDayStartPoint(imageInfo[1].Field);
            DeliveryDate_Info.position = new Point(5, 20);
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            // DeliveryDate_Info.size = new Size((224 - xStartPoint), (line3_Y - line2_Y) + 10);
            DeliveryDate_Info.font = new Font("Arial", 15, FontStyle.Bold);
            DeliveryDate_Info.blackground = false;
            DeliveryDate_Info.barcodeType = imageInfo[1].BarcodeType;
            textinfo_list.Add(DeliveryDate_Info);

            TextDrawingInfo Zipcodes_Info = new TextDrawingInfo();
            Zipcodes_Info.text = GetZipCodeTextFormatted(imageInfo[2].Field);
            Zipcodes_Info.position = new Point(10, 45);
            Zipcodes_Info.size = new Size(130, 25);
            Zipcodes_Info.font = new Font("Arial", 10, FontStyle.Bold);
            Zipcodes_Info.blackground = false;
            Zipcodes_Info.barcodeType = imageInfo[2].BarcodeType;
            textinfo_list.Add(Zipcodes_Info);

            //    // Brush string to the bitmap
            //    // initialize a bitmap with 240 x 96 in pixel, EP55 pixel size, and the format is 1 bit for 1 pixel.            
            Bitmap tmp = new Bitmap(172, 72);
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);
                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.

                for (int i = 0; i < textinfo_list.Count; i++)
                {
                    TextDrawingInfo ei = textinfo_list[i];
                    if (ei.barcodeType == BarcodeType.ONE_D)
                    {  // add a 1D Code39 barcode
                        Barcode bcode = new Barcode();
                        try
                        {
                            if (!ei.blackground)
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = false;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, ei.size.Width, ei.size.Height);
                            }
                            else
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                            }
                            graphic.DrawImage(bcode.EncodedImage, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }

                    }
                    else if (ei.barcodeType == BarcodeType.TWO_D)
                    {
                        try
                        {
                            Image bm = CreateQRCodeBitmap(ei.text, ei.size.Height);
                            graphic.DrawImage(bm, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    if (!ei.isbarcode)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                }
                graphic.Dispose();
            }

            // E2 is in portrait mode so must rotate 90 degrees
            tmp.RotateFlip(RotateFlipType.Rotate90FlipNone);

            return ConvertBitmapToMonochrome(tmp);
        }

        /// <summary>
        /// The supplied data is rendered as a Bitmap to fit on an EP55.
        /// </summary>
        /// <param name="imageInfo">Up to 3 elements can be shown on the screen. If less are supplied the 
        /// fields are left blank.</param>
        /// <returns></returns>
        public static Bitmap CreateImageForEP55(List<ImageField> imageInfo)
        {
            if (imageInfo == null)
                throw new ArgumentNullException();
            // Requires 3 elements in imageInfo. If they are not supplied is filled with empty data
            while (imageInfo.Count < 3)
            {
                imageInfo.Add(new ImageField()
                {
                    Field = " ",
                    BarcodeType = BarcodeType.NONE
                });
            }

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            // separate the first element of imageInfo by the last 4 characters
            string field0 = imageInfo[0].Field;
            if (field0.Length < 4)
            {
                field0 = field0 + "     ";
            }
            string field0a = field0.Substring(0, field0.Length - 4);
            string field0b = field0.Substring(field0.Length - 4, 4);

            TextDrawingInfo SortPlan_Info = new TextDrawingInfo();
            SortPlan_Info.text = field0a;
            SortPlan_Info.position = new Point(0, 0);
            //SortPlan_Info.size = new Size(50, line2_Y);
            SortPlan_Info.font = new Font("Arial", 10, FontStyle.Regular);
            SortPlan_Info.blackground = false;
            textinfo_list.Add(SortPlan_Info);

            TextDrawingInfo DeliveryDate_Info = new TextDrawingInfo();
            DeliveryDate_Info.text = field0b;
            DeliveryDate_Info.position = new Point(24, -17);
            //DeliveryDate_Info.size = new Size(200, line3_Y);
            DeliveryDate_Info.font = new Font("Arial", 65, FontStyle.Bold);
            DeliveryDate_Info.blackground = false;
            textinfo_list.Add(DeliveryDate_Info);

            TextDrawingInfo Zipcodes_Info = new TextDrawingInfo();
            Zipcodes_Info.text = imageInfo[1].Field;
            Zipcodes_Info.position = new Point(0, 70);
            //Zipcodes_Info.size = new Size(150, line4_Y - line3_Y);
            Zipcodes_Info.font = new Font("Arial", 15, FontStyle.Regular);
            Zipcodes_Info.blackground = false;
            textinfo_list.Add(Zipcodes_Info);

            TextDrawingInfo field3 = new TextDrawingInfo();
            field3.text = imageInfo[2].Field;
            field3.position = new Point(150, 70);
            //Zipcodes_Info.size = new Size(150, line4_Y - line3_Y);
            field3.font = new Font("Arial", 15, FontStyle.Regular);
            field3.blackground = false;
            textinfo_list.Add(field3);

            // Brush string to the bitmap
            // initialize a bitmap with 240 x 96 in pixel, EP55 pixel size, and the format is 1 bit for 1 pixel.            
            Bitmap tmp = new Bitmap(240, 96);
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);
                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.

                for (int i = 0; i < textinfo_list.Count; i++)
                {
                    TextDrawingInfo ei = textinfo_list[i];
                    if (!ei.blackground)
                    {
                        graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                    }
                    else
                    {
                        graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                        graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                    }
                }
                graphic.Dispose();
            }

            return ConvertBitmapToMonochrome(tmp);
        }

        //public static Bitmap CreateImageForEP55(List<ImageField> imageInfo)
        //{
        //    if (imageInfo == null) throw new ArgumentNullException();
        //    // Requires 3 elements in imageInfo. If they are not supplied is filled with empty data
        //    while (imageInfo.Count < 3)
        //    {
        //        imageInfo.Add(new ImageField()
        //        {
        //            Field = " ",
        //            BarcodeType = BarcodeType.NONE
        //        });
        //    }

        //    List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

        //    const int line2_Y = 10;
        //    const int line3_Y = 50;
        //    const int line4_Y = 70;

        //    TextDrawingInfo SortPlan_Info = new TextDrawingInfo();
        //    SortPlan_Info.text = imageInfo[0].Field;
        //    SortPlan_Info.position = new Point(0, 0);
        //    SortPlan_Info.size = new Size(90, line2_Y);
        //    SortPlan_Info.font = new Font("Arial", 17, FontStyle.Bold);
        //    SortPlan_Info.blackground = false;
        //    SortPlan_Info.BarcodeType = imageInfo[0].BarcodeType;
        //    textinfo_list.Add(SortPlan_Info);

        //    TextDrawingInfo DeliveryDate_Info = new TextDrawingInfo();
        //    DeliveryDate_Info.text = imageInfo[1].Field;
        //    int xStartPoint = GetDollyDayStartPoint(imageInfo[1].Field);
        //    DeliveryDate_Info.position = new Point(xStartPoint, line2_Y + 15);
        //    //DeliveryDate_Info.size = new Size((244-20), (144-80));
        //    DeliveryDate_Info.size = new Size((224 - xStartPoint), (line3_Y - line2_Y) + 10);
        //    DeliveryDate_Info.font = new Font("Arial", 35, FontStyle.Bold);
        //    DeliveryDate_Info.blackground = false;
        //    DeliveryDate_Info.BarcodeType = imageInfo[1].BarcodeType;
        //    textinfo_list.Add(DeliveryDate_Info);

        //    TextDrawingInfo Zipcodes_Info = new TextDrawingInfo();
        //    Zipcodes_Info.text = GetZipCodeTextFormatted(imageInfo[2].Field);
        //    Zipcodes_Info.position = new Point(115, line3_Y + 23);
        //    Zipcodes_Info.size = new Size(150, line4_Y - line3_Y);
        //    Zipcodes_Info.font = new Font("Arial", 11, FontStyle.Regular);
        //    Zipcodes_Info.blackground = false;
        //    Zipcodes_Info.BarcodeType = imageInfo[2].BarcodeType;
        //    textinfo_list.Add(Zipcodes_Info);

        //    // Brush string to the bitmap
        //    // initialize a bitmap with 240 x 96 in pixel, EP55 pixel size, and the format is 1 bit for 1 pixel.            
        //    Bitmap tmp = new Bitmap(240, 96);
        //    using (Graphics graphic = Graphics.FromImage(tmp))
        //    {
        //        SolidBrush brushBlack = new SolidBrush(Color.Black);
        //        SolidBrush brushWhite = new SolidBrush(Color.White);

        //        Pen pen = new Pen(brushBlack);
        //        graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.

        //        for (int i = 0; i < textinfo_list.Count; i++)
        //        {
        //            TextDrawingInfo ei = textinfo_list[i];
        //            if (ei.BarcodeType == BarcodeType.ONE_D)
        //            {  // add a 1D Code39 barcode
        //                Barcode bcode = new Barcode();
        //                try
        //                {
        //                    if (!ei.blackground)
        //                    {
        //                        bcode.Alignment = AlignmentPositions.CENTER;
        //                        bcode.IncludeLabel = true;
        //                        bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, ei.size.Width, ei.size.Height);
        //                    }
        //                    else
        //                    {
        //                        bcode.Alignment = AlignmentPositions.CENTER;
        //                        bcode.IncludeLabel = true;
        //                        bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
        //                    }
        //                    graphic.DrawImage(bcode.EncodedImage, ei.position);
        //                    ei.isbarcode = true;
        //                }
        //                catch (Exception)
        //                {
        //                    ei.isbarcode = false;
        //                }

        //            }
        //            else if (ei.BarcodeType == BarcodeType.TWO_D)
        //            {
        //                try
        //                {
        //                    BarcodePDF417 pd = new BarcodePDF417();
        //                    pd.SetText(ei.text);
        //                    Image bm = pd.CreateDrawingImage(Color.Black, Color.White);
        //                    graphic.DrawImage(bm, ei.position);
        //                    ei.isbarcode = true;
        //                }
        //                catch (Exception)
        //                {
        //                    ei.isbarcode = false;
        //                }
        //            }
        //            if (!ei.isbarcode)
        //            {
        //                if (!ei.blackground)
        //                {
        //                    graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
        //                }
        //                else
        //                {
        //                    graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
        //                    graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
        //                }
        //            }
        //        }
        //        graphic.Dispose();
        //    }

        //    return ConvertBitmapToMonochrome(tmp);
        //}

        /// <summary>
        /// The supplied data is rendered as a Bitmap to fit on an EP50.
        /// </summary>
        /// <param name="imageInfo">Up to 3 elements can be shown on the screen. If less are supplied the 
        /// fields are left blank.</param>
        /// <returns></returns>
        public static Bitmap CreateImageForEP50(List<ImageField> imageInfo)
        {
            if (imageInfo == null)
                throw new ArgumentNullException();
            // Requires 3 elements in imageInfo. If they are not supplied is filled with empty data
            while (imageInfo.Count < 3)
            {
                imageInfo.Add(new ImageField()
                {
                    Field = " ",
                    BarcodeType = BarcodeType.NONE
                });
            }

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            const int line2_Y = 10;
            const int line3_Y = 50;
            const int line4_Y = 70;

            TextDrawingInfo SortPlan_Info = new TextDrawingInfo();
            SortPlan_Info.text = imageInfo[0].Field;
            SortPlan_Info.position = new Point(0, 0);
            SortPlan_Info.size = new Size(90, line2_Y);
            SortPlan_Info.font = new Font("Arial", 17, FontStyle.Bold);
            SortPlan_Info.blackground = false;
            SortPlan_Info.barcodeType = imageInfo[0].BarcodeType;
            textinfo_list.Add(SortPlan_Info);

            TextDrawingInfo DeliveryDate_Info = new TextDrawingInfo();
            DeliveryDate_Info.text = imageInfo[1].Field;
            int xStartPoint = GetDollyDayStartPoint(imageInfo[1].Field);
            DeliveryDate_Info.position = new Point(xStartPoint, line2_Y + 15);
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            DeliveryDate_Info.size = new Size((224 - xStartPoint), (line3_Y - line2_Y) + 10);
            DeliveryDate_Info.font = new Font("Arial", 35, FontStyle.Bold);
            DeliveryDate_Info.blackground = false;
            DeliveryDate_Info.barcodeType = imageInfo[1].BarcodeType;
            textinfo_list.Add(DeliveryDate_Info);

            TextDrawingInfo Zipcodes_Info = new TextDrawingInfo();
            Zipcodes_Info.text = GetZipCodeTextFormatted(imageInfo[2].Field);
            Zipcodes_Info.position = new Point(115, line3_Y + 23);
            Zipcodes_Info.size = new Size(150, line4_Y - line3_Y);
            Zipcodes_Info.font = new Font("Arial", 11, FontStyle.Regular);
            Zipcodes_Info.blackground = false;
            Zipcodes_Info.barcodeType = imageInfo[2].BarcodeType;
            textinfo_list.Add(Zipcodes_Info);

            // Brush string to the bitmap
            // initialize a bitmap with 224 x 90 in pixel, EP50 pixel size, and the format is 1 bit for 1 pixel.            
            Bitmap tmp = new Bitmap(224, 90);
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);
                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.

                for (int i = 0; i < textinfo_list.Count; i++)
                {
                    TextDrawingInfo ei = textinfo_list[i];
                    if (ei.barcodeType == BarcodeType.ONE_D)
                    { // add a 1D Code39 barcode
                        Barcode bcode = new Barcode();
                        try
                        {
                            if (!ei.blackground)
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, ei.size.Width, ei.size.Height);
                            }
                            else
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                            }
                            graphic.DrawImage(bcode.EncodedImage, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    else if (ei.barcodeType == BarcodeType.TWO_D)
                    {
                        try
                        {
                            Image bm = CreateQRCodeBitmap(ei.text, ei.size.Height);
                            graphic.DrawImage(bm, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    if (!ei.isbarcode)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                }
                graphic.Dispose();
            }

            return ConvertBitmapToMonochrome(tmp);
        }

        public static Bitmap CreateImageForEP900ForUSPS(List<ImageField> imageInfo)
        {
            if (imageInfo == null)
                return null;

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            const int line2_Y = 150;
            const int line3_Y = 215;
            const int line4_Y = 350;
            const int bottomOfDisplay_Y = 480;

            TextDrawingInfo SortPlan_Info = new TextDrawingInfo();
            SortPlan_Info.text = imageInfo[0].Field;
            SortPlan_Info.position = new Point(0, 0);
            SortPlan_Info.size = new Size(360, line2_Y);
            SortPlan_Info.font = new Font("Arial", 47, FontStyle.Bold);
            SortPlan_Info.blackground = false;
            SortPlan_Info.barcodeType = imageInfo[0].BarcodeType;
            textinfo_list.Add(SortPlan_Info);

            TextDrawingInfo DeliveryDate_Info = new TextDrawingInfo();
            DeliveryDate_Info.text = imageInfo[1].Field;
            int xStartPoint = GetDollyDayStartPoint(imageInfo[1].Field);
            DeliveryDate_Info.position = new Point(xStartPoint, line2_Y + GetDollyDayHeightStartPointAdjust(imageInfo[1].Field));
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            DeliveryDate_Info.size = new Size((360 - xStartPoint), (line3_Y - line2_Y));
            DeliveryDate_Info.font = new Font("Arial", 35, FontStyle.Bold);
            DeliveryDate_Info.blackground = false;
            DeliveryDate_Info.barcodeType = imageInfo[1].BarcodeType;
            textinfo_list.Add(DeliveryDate_Info);

            TextDrawingInfo Txt_Info1 = new TextDrawingInfo();
            Txt_Info1.text = imageInfo[2].Field;
            Txt_Info1.position = new Point(0, line3_Y);
            Txt_Info1.size = new Size(360, line4_Y);
            Txt_Info1.font = new Font("Arial", 12, FontStyle.Bold);
            Txt_Info1.blackground = false;
            Txt_Info1.barcodeType = imageInfo[2].BarcodeType;
            textinfo_list.Add(Txt_Info1);

            TextDrawingInfo Txt_Info2 = new TextDrawingInfo();
            Txt_Info2.text = imageInfo[3].Field;
            Txt_Info2.position = new Point(0, line3_Y + 30);
            Txt_Info2.size = new Size(360, line4_Y - line3_Y - 50);
            Txt_Info2.font = new Font("Arial", 12, FontStyle.Bold);
            Txt_Info2.blackground = false;
            Txt_Info2.barcodeType = imageInfo[3].BarcodeType;
            textinfo_list.Add(Txt_Info2);

            TextDrawingInfo DateCreated_Info = new TextDrawingInfo();
            DateCreated_Info.text = imageInfo[4].Field;
            DateCreated_Info.position = new Point(0 + 60, line4_Y);
            DateCreated_Info.size = new Size(250, bottomOfDisplay_Y - line4_Y);
            DateCreated_Info.font = new Font("Arial", 18, FontStyle.Regular);
            DateCreated_Info.blackground = false;
            DateCreated_Info.barcodeType = imageInfo[4].BarcodeType;
            textinfo_list.Add(DateCreated_Info);

            // Brush string to the bitmap
            // initialize a bitmap with 360X480 in pixel, EP900 pixel size, and the format is 1 bit for 1 pixel.            
            Bitmap tmp = new Bitmap(360, 480);
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);
                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.

                for (int i = 0; i < textinfo_list.Count; i++)
                {
                    TextDrawingInfo ei = textinfo_list[i];
                    if (ei.barcodeType == BarcodeType.ONE_D)
                    { // add a 1D Code39 barcode
                        Barcode bcode = new Barcode();
                        try
                        {
                            if (!ei.blackground)
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, ei.size.Width, ei.size.Height);
                            }
                            else
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                            }
                            graphic.DrawImage(bcode.EncodedImage, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    else if (ei.barcodeType == BarcodeType.TWO_D)
                    {
                        try
                        {
                            Image bm = CreateQRCodeBitmap(ei.text, ei.size.Height);
                            graphic.DrawImage(bm, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    if (!ei.isbarcode)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                }
                graphic.Dispose();
            }

            return ConvertBitmapToMonochrome(tmp);
        }

        public static Bitmap CreateImageForEP900(List<ImageField> imageInfo)
        {
            if (imageInfo == null)
                throw new ArgumentNullException();
            // Requires 4 elements in imageInfo. If they are not supplied is filled with empty data
            while (imageInfo.Count < 4)
            {
                imageInfo.Add(new ImageField()
                {
                    Field = " ",
                    BarcodeType = BarcodeType.NONE
                });
            }

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            const int line2_Y = 150;
            const int line3_Y = 215;
            const int line4_Y = 350;
            const int bottomOfDisplay_Y = 480;

            TextDrawingInfo SortPlan_Info = new TextDrawingInfo();
            SortPlan_Info.text = imageInfo[0].Field;
            SortPlan_Info.position = new Point(0, 0);
            SortPlan_Info.size = new Size(360, line2_Y);
            SortPlan_Info.font = new Font("Arial", 47, FontStyle.Bold);
            SortPlan_Info.blackground = false;
            SortPlan_Info.barcodeType = imageInfo[0].BarcodeType;
            textinfo_list.Add(SortPlan_Info);

            TextDrawingInfo DeliveryDate_Info = new TextDrawingInfo();
            DeliveryDate_Info.text = imageInfo[1].Field;
            int xStartPoint = GetDollyDayStartPoint(imageInfo[1].Field);
            DeliveryDate_Info.position = new Point(xStartPoint, line2_Y + GetDollyDayHeightStartPointAdjust(imageInfo[1].Field));
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            DeliveryDate_Info.size = new Size((360 - xStartPoint), (line3_Y - line2_Y));
            DeliveryDate_Info.font = new Font("Arial", 35, FontStyle.Bold);
            DeliveryDate_Info.blackground = false;
            DeliveryDate_Info.barcodeType = imageInfo[1].BarcodeType;
            textinfo_list.Add(DeliveryDate_Info);

            TextDrawingInfo Zipcodes_Info = new TextDrawingInfo();
            Zipcodes_Info.text = GetZipCodeTextFormatted(imageInfo[2].Field);
            Zipcodes_Info.position = new Point(0, line3_Y);
            Zipcodes_Info.size = new Size(360, line4_Y - line3_Y - 50);
            Zipcodes_Info.font = new Font("Arial", 25, FontStyle.Regular);
            Zipcodes_Info.blackground = false;
            Zipcodes_Info.barcodeType = imageInfo[2].BarcodeType;
            textinfo_list.Add(Zipcodes_Info);

            TextDrawingInfo DateCreated_Info = new TextDrawingInfo();
            DateCreated_Info.text = imageInfo[3].Field;
            DateCreated_Info.position = new Point(0, line4_Y);
            DateCreated_Info.size = new Size(200, bottomOfDisplay_Y - line4_Y);
            DateCreated_Info.font = new Font("Arial", 18, FontStyle.Regular);
            DateCreated_Info.blackground = false;
            DateCreated_Info.barcodeType = imageInfo[3].BarcodeType;
            textinfo_list.Add(DateCreated_Info);

            // Brush string to the bitmap
            // initialize a bitmap with 360X480 in pixel, EP900 pixel size, and the format is 1 bit for 1 pixel.            
            Bitmap tmp = new Bitmap(360, 480);
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);
                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.

                for (int i = 0; i < textinfo_list.Count; i++)
                {
                    TextDrawingInfo ei = textinfo_list[i];
                    if (ei.barcodeType == BarcodeType.ONE_D)
                    { // add a 1D Code39 barcode
                        Barcode bcode = new Barcode();
                        try
                        {
                            if (!ei.blackground)
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, ei.size.Width, ei.size.Height);
                            }
                            else
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                            }
                            graphic.DrawImage(bcode.EncodedImage, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    else if (ei.barcodeType == BarcodeType.TWO_D)
                    {
                        try
                        {
                            Image bm = CreateQRCodeBitmap(ei.text, ei.size.Height);
                            graphic.DrawImage(bm, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    if (!ei.isbarcode)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                }
                graphic.Dispose();
            }

            return ConvertBitmapToMonochrome(tmp);
        }

        public static Bitmap CreateImageForEP500(List<ImageField> imageInfo)
        {
            if (imageInfo == null)
                throw new ArgumentNullException();
            // Requires 7 elements in imageInfo. If they are not supplied is filled with empty data
            while (imageInfo.Count < 7)
            {
                imageInfo.Add(new ImageField()
                {
                    Field = " ",
                    BarcodeType = BarcodeType.NONE
                });
            }

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            const int line2_Y = 83;
            const int line3_Y = 165;  //144
            const int line4_Y = 210;
            const int bottomOfDisplay_Y = 240;

            TextDrawingInfo SortPlan_Info = new TextDrawingInfo();
            SortPlan_Info.text = imageInfo[0].Field;
            SortPlan_Info.position = new Point(0, 0);
            SortPlan_Info.size = new Size(320, line2_Y);
            SortPlan_Info.font = new Font("Arial", 47, FontStyle.Bold);
            SortPlan_Info.blackground = false;
            SortPlan_Info.barcodeType = imageInfo[0].BarcodeType;
            textinfo_list.Add(SortPlan_Info);

            TextDrawingInfo DeliveryDate_Info = new TextDrawingInfo();
            DeliveryDate_Info.text = imageInfo[1].Field;
            int xStartPoint = GetDollyDayStartPoint(imageInfo[1].Field);
            DeliveryDate_Info.position = new Point(xStartPoint, line2_Y + GetDollyDayHeightStartPointAdjust(imageInfo[1].Field));
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            DeliveryDate_Info.size = new Size((320 - xStartPoint), (line3_Y - line2_Y));
            DeliveryDate_Info.font = new Font("Arial", GetDollyDayFontSize(imageInfo[1].Field), FontStyle.Bold);
            DeliveryDate_Info.blackground = false;
            DeliveryDate_Info.barcodeType = imageInfo[1].BarcodeType;
            textinfo_list.Add(DeliveryDate_Info);

            TextDrawingInfo Zipcodes_Info = new TextDrawingInfo();
            Zipcodes_Info.text = GetZipCodeTextFormatted(imageInfo[2].Field);
            Zipcodes_Info.position = new Point(0, line3_Y);
            Zipcodes_Info.size = new Size(320, line4_Y - line3_Y);
            Zipcodes_Info.font = new Font("Arial", 25, FontStyle.Regular);
            Zipcodes_Info.blackground = false;
            Zipcodes_Info.barcodeType = imageInfo[2].BarcodeType;
            textinfo_list.Add(Zipcodes_Info);

            TextDrawingInfo DateCreated_Info = new TextDrawingInfo();
            DateCreated_Info.text = imageInfo[3].Field;
            DateCreated_Info.position = new Point(0, line4_Y);
            DateCreated_Info.size = new Size(130, bottomOfDisplay_Y - line4_Y);
            DateCreated_Info.font = new Font("Arial", 18, FontStyle.Regular);
            DateCreated_Info.blackground = false;
            DateCreated_Info.barcodeType = imageInfo[3].BarcodeType;
            textinfo_list.Add(DateCreated_Info);

            TextDrawingInfo TimeCreated_Info = new TextDrawingInfo();
            TimeCreated_Info.text = imageInfo[4].Field;
            TimeCreated_Info.position = new Point(130, line4_Y);
            TimeCreated_Info.size = new Size(100, bottomOfDisplay_Y - line4_Y);
            TimeCreated_Info.font = new Font("Arial", 18, FontStyle.Regular);
            TimeCreated_Info.blackground = false;
            TimeCreated_Info.barcodeType = imageInfo[4].BarcodeType;
            textinfo_list.Add(TimeCreated_Info);

            TextDrawingInfo FullACTS_Info = new TextDrawingInfo();
            FullACTS_Info.text = imageInfo[5].Field;
            FullACTS_Info.position = new Point(230, line4_Y);
            FullACTS_Info.size = new Size(45, bottomOfDisplay_Y - line4_Y);
            FullACTS_Info.font = new Font("Arial", 18, FontStyle.Regular);
            FullACTS_Info.blackground = false;
            FullACTS_Info.barcodeType = imageInfo[5].BarcodeType;
            textinfo_list.Add(FullACTS_Info);

            TextDrawingInfo EmptyACTS_Info = new TextDrawingInfo();
            EmptyACTS_Info.text = imageInfo[6].Field;
            EmptyACTS_Info.position = new Point(275, line4_Y);
            EmptyACTS_Info.size = new Size(45, bottomOfDisplay_Y - line4_Y);
            EmptyACTS_Info.font = new Font("Arial", 18, FontStyle.Regular);
            EmptyACTS_Info.blackground = false;
            EmptyACTS_Info.barcodeType = imageInfo[6].BarcodeType;
            textinfo_list.Add(EmptyACTS_Info);

            // Brush string to the bitmap
            // initialize a bitmap with 320X240 in pixel, EP500 pixel size, and the format is 1 bit for 1 pixel.
            //Bitmap tmp = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format1bppIndexed );
            Bitmap tmp = new Bitmap(320, 240);
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);
                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.
                graphic.DrawRectangle(pen, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height)));
                graphic.DrawLine(pen, new Point(0, line2_Y), new Point(320, line2_Y));
                graphic.DrawLine(pen, new Point(0, line3_Y), new Point(320, line3_Y));
                graphic.DrawLine(pen, new Point(0, line4_Y), new Point(320, line4_Y));
                graphic.DrawLine(pen, new Point(130, line4_Y), new Point(130, bottomOfDisplay_Y));
                graphic.DrawLine(pen, new Point(230, line4_Y), new Point(230, bottomOfDisplay_Y));
                //   graphic.DrawLine(pen, new Point(275, line4_Y), new Point(275, bottomOfDisplay_Y));    // for full/empty ACT separation
                //graphic.DrawImage();
                //graphic.DrawIcon();

                for (int i = 0; i < textinfo_list.Count; i++)
                {
                    TextDrawingInfo ei = textinfo_list[i];
                    if (ei.barcodeType == BarcodeType.ONE_D)
                    { // add a 1D Code39 barcode
                        Barcode bcode = new Barcode();
                        try
                        {
                            if (!ei.blackground)
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, ei.size.Width, ei.size.Height);
                            }
                            else
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                            }
                            graphic.DrawImage(bcode.EncodedImage, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    else if (ei.barcodeType == BarcodeType.TWO_D)
                    {
                        try
                        {
                            Image bm = CreateQRCodeBitmap(ei.text, ei.size.Height);
                            graphic.DrawImage(bm, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    if (!ei.isbarcode)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                }
                graphic.Dispose();
            }

            return ConvertBitmapToMonochrome(tmp);
        }

        //-----
        public static Bitmap CreateImageForP3(List<ImageField> imageInfo)
        {
            if (imageInfo == null)
                throw new ArgumentNullException();
            // Requires 7 elements in imageInfo. If they are not supplied is filled with empty data
            while (imageInfo.Count < 7)
            {
                imageInfo.Add(new ImageField()
                {
                    Field = " ",
                    BarcodeType = BarcodeType.NONE
                });
            }

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            const int line2_Y = 83;
            const int line3_Y = 165;  //144
            const int line4_Y = 210;
            const int bottomOfDisplay_Y = 176;

            TextDrawingInfo SortPlan_Info = new TextDrawingInfo();
            SortPlan_Info.text = imageInfo[0].Field;
            SortPlan_Info.position = new Point(0, 5);
            SortPlan_Info.size = new Size(264, 50);
            SortPlan_Info.font = new Font("Arial", 10, FontStyle.Bold);
            SortPlan_Info.blackground = false;
            SortPlan_Info.barcodeType = imageInfo[0].BarcodeType;
            textinfo_list.Add(SortPlan_Info);

            TextDrawingInfo DeliveryDate_Info = new TextDrawingInfo();
            DeliveryDate_Info.text = imageInfo[1].Field;
            int xStartPoint = GetDollyDayStartPoint(imageInfo[1].Field);
            DeliveryDate_Info.position = new Point(0, 58);
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            DeliveryDate_Info.size = new Size((400 - xStartPoint), (line3_Y - line2_Y));
            DeliveryDate_Info.font = new Font("Arial", 28, FontStyle.Bold);
            DeliveryDate_Info.blackground = false;
            DeliveryDate_Info.barcodeType = imageInfo[1].BarcodeType;
            textinfo_list.Add(DeliveryDate_Info);

            TextDrawingInfo Zipcodes_Info = new TextDrawingInfo();
            Zipcodes_Info.text = GetZipCodeTextFormatted(imageInfo[2].Field);
            Zipcodes_Info.position = new Point(0, 105);
            Zipcodes_Info.size = new Size(320, line4_Y - line3_Y);
            Zipcodes_Info.font = new Font("Arial", 24, FontStyle.Regular);
            Zipcodes_Info.blackground = false;
            Zipcodes_Info.barcodeType = imageInfo[2].BarcodeType;
            textinfo_list.Add(Zipcodes_Info);

            TextDrawingInfo DateCreated_Info = new TextDrawingInfo();
            DateCreated_Info.text = imageInfo[3].Field;
            DateCreated_Info.position = new Point(2, 145);
            DateCreated_Info.size = new Size(130, bottomOfDisplay_Y - line4_Y + 10);
            DateCreated_Info.font = new Font("Arial", 22, FontStyle.Regular);
            DateCreated_Info.blackground = false;
            DateCreated_Info.barcodeType = imageInfo[3].BarcodeType;
            textinfo_list.Add(DateCreated_Info);

            TextDrawingInfo TimeCreated_Info = new TextDrawingInfo();
            TimeCreated_Info.text = imageInfo[4].Field;
            TimeCreated_Info.position = new Point(140, 145);
            TimeCreated_Info.size = new Size(100, bottomOfDisplay_Y - line4_Y);
            TimeCreated_Info.font = new Font("Arial", 20, FontStyle.Regular);
            TimeCreated_Info.blackground = false;
            TimeCreated_Info.barcodeType = imageInfo[4].BarcodeType;
            textinfo_list.Add(TimeCreated_Info);

            TextDrawingInfo FullACTS_Info = new TextDrawingInfo();
            FullACTS_Info.text = imageInfo[5].Field;
            FullACTS_Info.position = new Point(210, 145);
            FullACTS_Info.size = new Size(45, bottomOfDisplay_Y - line4_Y);
            FullACTS_Info.font = new Font("Arial", 20, FontStyle.Regular);
            FullACTS_Info.blackground = false;
            FullACTS_Info.barcodeType = imageInfo[5].BarcodeType;
            textinfo_list.Add(FullACTS_Info);

            TextDrawingInfo EmptyACTS_Info = new TextDrawingInfo();
            EmptyACTS_Info.text = imageInfo[6].Field;
            EmptyACTS_Info.position = new Point(220, 165);
            EmptyACTS_Info.size = new Size(45, bottomOfDisplay_Y - line4_Y);
            EmptyACTS_Info.font = new Font("Arial", 18, FontStyle.Bold);
            EmptyACTS_Info.blackground = false;
            EmptyACTS_Info.barcodeType = imageInfo[6].BarcodeType;
            textinfo_list.Add(EmptyACTS_Info);

            // Brush string to the bitmap
            // initialize a bitmap with 400X300 in pixel, P4 pixel size, and the format is 1 bit for 1 pixel.
            //Bitmap tmp = new Bitmap(400, 300, System.Drawing.Imaging.PixelFormat.Format1bppIndexed );
            Bitmap tmp = new Bitmap(264, 176);
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);
                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.
                graphic.DrawRectangle(pen, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height)));
                graphic.DrawLine(pen, new Point(0, 55), new Point(264, 55));
                graphic.DrawLine(pen, new Point(0, 100), new Point(264, 100));
                graphic.DrawLine(pen, new Point(0, 140), new Point(264, 140));
                graphic.DrawLine(pen, new Point(135, 140), new Point(135, bottomOfDisplay_Y));
                graphic.DrawLine(pen, new Point(210, 140), new Point(210, bottomOfDisplay_Y));
                //   graphic.DrawLine(pen, new Point(275, line4_Y), new Point(275, bottomOfDisplay_Y));    // for full/empty ACT separation
                //graphic.DrawImage();
                //graphic.DrawIcon();

                for (int i = 0; i < textinfo_list.Count; i++)
                {
                    TextDrawingInfo ei = textinfo_list[i];
                    if (ei.barcodeType == BarcodeType.ONE_D)
                    { // add a 1D Code39 barcode
                        Barcode bcode = new Barcode();
                        try
                        {
                            if (!ei.blackground)
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128, ei.text, ei.size.Width, ei.size.Height);
                            }
                            else
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                            }
                            graphic.DrawImage(bcode.EncodedImage, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    else if (ei.barcodeType == BarcodeType.TWO_D)
                    {
                        try
                        {
                            Image bm = CreateQRCodeBitmap(ei.text, ei.size.Height);
                            graphic.DrawImage(bm, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    if (!ei.isbarcode)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                }
                graphic.Dispose();
            }

            return ConvertBitmapToMonochrome(tmp);
        }
        /// <summary>
        /// <para>Creates a bitmap formatted for an EP500 using the supplied data.</para>
        /// <param name="imageInfo">Should contain 7 elements. If isBarcode is true
        /// an attempt will be made to format the data as a barcode. If it is not possible
        /// the string will be displayed instead.</param>
        /// <returns>A Bitmap formatted for the EP500</returns>
        /// <remarks>Section 4.2 of the spec (ISO 16002) says that white-on-dark decoding is optional for Datamatrix code. 
        /// This means some fraction of readers can read the code, versus 100%. For this reason our image rendering
        /// will not render Datamatrix as white-on-dark (i.e.Datamaxtrix is white and the background is dark).</remarks>
        /// </summary>
        public static Bitmap CreateDollyImageForEP500(List<ImageField> imageInfo)
        {
            if (imageInfo == null)
                throw new ArgumentNullException();
            // Requires 7 elements in imageInfo. If they are not supplied is filled with empty data
            while (imageInfo.Count < 7)
            {
                imageInfo.Add(new ImageField()
                {
                    Field = " ",
                    BarcodeType = BarcodeType.NONE
                });
            }

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            const int line2_Y = 83;
            const int line3_Y = 165;  //144
            const int line4_Y = 210;
            const int bottomOfDisplay_Y = 240;

            TextDrawingInfo SortPlan_Info = new TextDrawingInfo();
            SortPlan_Info.text = imageInfo[0].Field;
            SortPlan_Info.position = new Point(0, 0);
            SortPlan_Info.size = new Size(320, line2_Y);
            SortPlan_Info.font = new Font("Arial", 47, FontStyle.Bold);
            SortPlan_Info.blackground = false;
            SortPlan_Info.barcodeType = imageInfo[0].BarcodeType;
            textinfo_list.Add(SortPlan_Info);

            TextDrawingInfo DeliveryDate_Info = new TextDrawingInfo();
            DeliveryDate_Info.text = imageInfo[1].Field;
            int xStartPoint = GetDollyDayStartPoint(imageInfo[1].Field);
            DeliveryDate_Info.position = new Point(xStartPoint, line2_Y + GetDollyDayHeightStartPointAdjust(imageInfo[1].Field));
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            DeliveryDate_Info.size = new Size((320 - xStartPoint), (line3_Y - line2_Y));
            DeliveryDate_Info.font = new Font("Arial", GetDollyDayFontSize(imageInfo[1].Field), FontStyle.Bold);
            DeliveryDate_Info.blackground = false;
            DeliveryDate_Info.barcodeType = imageInfo[1].BarcodeType;
            textinfo_list.Add(DeliveryDate_Info);

            TextDrawingInfo Zipcodes_Info = new TextDrawingInfo();
            Zipcodes_Info.text = GetZipCodeTextFormatted(imageInfo[2].Field);
            Zipcodes_Info.position = new Point(0, line3_Y);
            Zipcodes_Info.size = new Size(320, line4_Y - line3_Y);
            Zipcodes_Info.font = new Font("Arial", 25, FontStyle.Regular);
            Zipcodes_Info.blackground = false;
            Zipcodes_Info.barcodeType = imageInfo[2].BarcodeType;
            textinfo_list.Add(Zipcodes_Info);

            TextDrawingInfo DateCreated_Info = new TextDrawingInfo();
            DateCreated_Info.text = imageInfo[3].Field;
            DateCreated_Info.position = new Point(0, line4_Y);
            DateCreated_Info.size = new Size(130, bottomOfDisplay_Y - line4_Y);
            DateCreated_Info.font = new Font("Arial", 18, FontStyle.Regular);
            DateCreated_Info.blackground = false;
            DateCreated_Info.barcodeType = imageInfo[3].BarcodeType;
            textinfo_list.Add(DateCreated_Info);

            TextDrawingInfo TimeCreated_Info = new TextDrawingInfo();
            TimeCreated_Info.text = imageInfo[4].Field;
            TimeCreated_Info.position = new Point(130, line4_Y);
            TimeCreated_Info.size = new Size(100, bottomOfDisplay_Y - line4_Y);
            TimeCreated_Info.font = new Font("Arial", 18, FontStyle.Regular);
            TimeCreated_Info.blackground = false;
            TimeCreated_Info.barcodeType = imageInfo[4].BarcodeType;
            textinfo_list.Add(TimeCreated_Info);

            TextDrawingInfo FullACTS_Info = new TextDrawingInfo();
            FullACTS_Info.text = imageInfo[5].Field;
            FullACTS_Info.position = new Point(230, line4_Y);
            FullACTS_Info.size = new Size(45, bottomOfDisplay_Y - line4_Y);
            FullACTS_Info.font = new Font("Arial", 18, FontStyle.Regular);
            FullACTS_Info.blackground = false;
            FullACTS_Info.barcodeType = imageInfo[5].BarcodeType;
            textinfo_list.Add(FullACTS_Info);

            TextDrawingInfo EmptyACTS_Info = new TextDrawingInfo();
            EmptyACTS_Info.text = imageInfo[6].Field;
            EmptyACTS_Info.position = new Point(275, line4_Y);
            EmptyACTS_Info.size = new Size(45, bottomOfDisplay_Y - line4_Y);
            EmptyACTS_Info.font = new Font("Arial", 18, FontStyle.Regular);
            EmptyACTS_Info.blackground = false;
            EmptyACTS_Info.barcodeType = imageInfo[6].BarcodeType;
            textinfo_list.Add(EmptyACTS_Info);

            // Brush string to the bitmap
            // initialize a bitmap with 320X240 in pixel, EP500 pixel size, and the format is 1 bit for 1 pixel.
            //Bitmap tmp = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format1bppIndexed );
            Bitmap tmp = new Bitmap(320, 240);
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);
                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.
                graphic.DrawRectangle(pen, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height)));
                graphic.DrawLine(pen, new Point(0, line2_Y), new Point(320, line2_Y));
                graphic.DrawLine(pen, new Point(0, line3_Y), new Point(320, line3_Y));
                graphic.DrawLine(pen, new Point(0, line4_Y), new Point(320, line4_Y));
                graphic.DrawLine(pen, new Point(130, line4_Y), new Point(130, bottomOfDisplay_Y));
                graphic.DrawLine(pen, new Point(230, line4_Y), new Point(230, bottomOfDisplay_Y));
                //   graphic.DrawLine(pen, new Point(275, line4_Y), new Point(275, bottomOfDisplay_Y));    // for full/empty ACT separation
                //graphic.DrawImage();
                //graphic.DrawIcon();

                for (int i = 0; i < textinfo_list.Count; i++)
                {
                    TextDrawingInfo ei = textinfo_list[i];
                    if (ei.barcodeType == BarcodeType.ONE_D)
                    { // add a 1D Code39 barcode
                        Barcode bcode = new Barcode();
                        try
                        {
                            if (!ei.blackground)
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, ei.size.Width, ei.size.Height);
                            }
                            else
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                            }
                            graphic.DrawImage(bcode.EncodedImage, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    else if (ei.barcodeType == BarcodeType.TWO_D)
                    {
                        try
                        {
                            Image bm = CreateQRCodeBitmap(ei.text, ei.size.Height);
                            graphic.DrawImage(bm, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    if (!ei.isbarcode)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                }
                graphic.Dispose();
            }

            /* // the tmp's bitdepth is 32 and cannot be accepted by ZBD bounce system, it need to convert to bitmap with bitdepth 1, or monochrome bitmap.
            // convert it to monochrome bitmap
            // The effective luminance of a pixel is calculated with the following formula:
            // Y=0.3RED+0.59GREEN+0.11Blue
            // this luminance value can then be turned into a grayscale pixel using Color.FromArgb(Y,Y,Y).
            Bitmap bm = new Bitmap(tmp.Width, tmp.Height);
            for (int y = 0; y < bm.Height; y++) {
            for (int x = 0; x < bm.Width; x++) {
            Color c = tmp.GetPixel(x, y);
            int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
            bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma));
            }
            }
            
            Point start = new Point(0,0);
            Size size= new Size( tmp.Width, tmp.Height);
            Rectangle r = new Rectangle(start, size);
            Bitmap bm2 = bm.Clone(r, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            //return bm.Clone(r, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            //return bm.Clone(new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height)), System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            // bm2;
            //return tmp.Clone(new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height)), System.Drawing.Imaging.PixelFormat.Format1bppIndexed); */

            return ConvertBitmapToMonochrome(tmp);
        }

        public static Bitmap CreateDollyImageForEP500(DollyImageInfo imageInfo)
        {
            if (imageInfo == null)
                return null;

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            int line2_Y = 83;
            int line3_Y = 165;  //144
            int line4_Y = 210;
            int bottomOfDisplay_Y = 240;

            TextDrawingInfo SortPlan_Info = new TextDrawingInfo();
            SortPlan_Info.text = imageInfo.SortPlan;
            SortPlan_Info.position = new Point(0, 0);
            SortPlan_Info.size = new Size(320, line2_Y);
            SortPlan_Info.font = new Font("Arial", 47, FontStyle.Bold);
            SortPlan_Info.blackground = false;
            SortPlan_Info.isbarcode = true;
            textinfo_list.Add(SortPlan_Info);

            TextDrawingInfo DeliveryDate_Info = new TextDrawingInfo();
            DeliveryDate_Info.text = imageInfo.DeliveryDate;
            int xStartPoint = GetDollyDayStartPoint(imageInfo.DeliveryDate);
            DeliveryDate_Info.position = new Point(xStartPoint, line2_Y + GetDollyDayHeightStartPointAdjust(imageInfo.DeliveryDate));
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            DeliveryDate_Info.size = new Size((320 - xStartPoint), (line3_Y - line2_Y));
            DeliveryDate_Info.font = new Font("Arial", GetDollyDayFontSize(imageInfo.DeliveryDate), FontStyle.Bold);
            DeliveryDate_Info.blackground = false;
            textinfo_list.Add(DeliveryDate_Info);

            TextDrawingInfo Zipcodes_Info = new TextDrawingInfo();
            Zipcodes_Info.text = GetZipCodeTextFormatted(imageInfo.ZipCodes);
            Zipcodes_Info.position = new Point(0, line3_Y);
            Zipcodes_Info.size = new Size(320, line4_Y - line3_Y);
            Zipcodes_Info.font = new Font("Arial", 25, FontStyle.Regular);
            Zipcodes_Info.blackground = false;
            textinfo_list.Add(Zipcodes_Info);

            TextDrawingInfo DateCreated_Info = new TextDrawingInfo();
            DateCreated_Info.text = imageInfo.DateCreated;
            DateCreated_Info.position = new Point(0, line4_Y);
            DateCreated_Info.size = new Size(130, bottomOfDisplay_Y - line4_Y);
            DateCreated_Info.font = new Font("Arial", 18, FontStyle.Regular);
            DateCreated_Info.blackground = false;
            textinfo_list.Add(DateCreated_Info);

            TextDrawingInfo TimeCreated_Info = new TextDrawingInfo();
            TimeCreated_Info.text = imageInfo.TimeCreated;
            TimeCreated_Info.position = new Point(130, line4_Y);
            TimeCreated_Info.size = new Size(100, bottomOfDisplay_Y - line4_Y);
            TimeCreated_Info.font = new Font("Arial", 18, FontStyle.Regular);
            TimeCreated_Info.blackground = false;
            textinfo_list.Add(TimeCreated_Info);

            TextDrawingInfo Barcode_Info = new TextDrawingInfo();
            Barcode_Info.text = imageInfo.GetShortBarcode();
            Barcode_Info.position = new Point(230, line4_Y);
            Barcode_Info.size = new Size(45, bottomOfDisplay_Y - line4_Y);
            Barcode_Info.font = new Font("Arial", 18, FontStyle.Regular);
            Barcode_Info.blackground = false;
            textinfo_list.Add(Barcode_Info);
            /*
            TextDrawingInfo FullACTS_Info = new TextDrawingInfo();
            FullACTS_Info.text = imageInfo.FullACTS;
            FullACTS_Info.position = new Point(230, line4_Y);
            FullACTS_Info.size = new Size(45, bottomOfDisplay_Y - line4_Y);
            FullACTS_Info.font = new Font("Arial", 18, FontStyle.Regular);
            FullACTS_Info.blackground = false;
            textinfo_list.Add(FullACTS_Info);

            TextDrawingInfo EmptyACTS_Info = new TextDrawingInfo();
            EmptyACTS_Info.text = imageInfo.EmptyACTS;
            EmptyACTS_Info.position = new Point(275, line4_Y);
            EmptyACTS_Info.size = new Size(45, bottomOfDisplay_Y - line4_Y);
            EmptyACTS_Info.font = new Font("Arial", 18, FontStyle.Regular);
            EmptyACTS_Info.blackground = false;
            textinfo_list.Add(EmptyACTS_Info);
            */
            // Brush string to the bitmap
            // initialize a bitmap with 320X240 in pixel, EP500 pixel size, and the format is 1 bit for 1 pixel.
            //Bitmap tmp = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format1bppIndexed );
            Bitmap tmp = new Bitmap(320, 240);
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);
                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.
                graphic.DrawRectangle(pen, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height)));
                graphic.DrawLine(pen, new Point(0, line2_Y), new Point(320, line2_Y));
                graphic.DrawLine(pen, new Point(0, line3_Y), new Point(320, line3_Y));
                graphic.DrawLine(pen, new Point(0, line4_Y), new Point(320, line4_Y));
                graphic.DrawLine(pen, new Point(130, line4_Y), new Point(130, bottomOfDisplay_Y));
                graphic.DrawLine(pen, new Point(230, line4_Y), new Point(230, bottomOfDisplay_Y));
                //   graphic.DrawLine(pen, new Point(275, line4_Y), new Point(275, bottomOfDisplay_Y));    // for full/empty ACT separation
                //graphic.DrawImage();
                //graphic.DrawIcon();

                foreach (TextDrawingInfo ei in textinfo_list)
                {
                    if (ei.isbarcode == false)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                    else
                    { // add a 1D Code39 barcode
                        Barcode bcode = new Barcode();
                        if (!ei.blackground)
                        {
                            bcode.Alignment = AlignmentPositions.CENTER;
                            bcode.IncludeLabel = true;
                            bcode.Encode(BarcodeLib.TYPE.CODE39, ei.text, ei.size.Width, ei.size.Height);
                        }
                        else
                        {
                            bcode.Alignment = AlignmentPositions.CENTER;
                            bcode.IncludeLabel = true;
                            bcode.Encode(BarcodeLib.TYPE.CODE39, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                        }
                        graphic.DrawImage(bcode.EncodedImage, ei.position);
                    }
                }
                graphic.Dispose();
            }

            /* // the tmp's bitdepth is 32 and cannot be accepted by ZBD bounce system, it need to convert to bitmap with bitdepth 1, or monochrome bitmap.
            // convert it to monochrome bitmap
            // The effective luminance of a pixel is calculated with the following formula:
            // Y=0.3RED+0.59GREEN+0.11Blue
            // this luminance value can then be turned into a grayscale pixel using Color.FromArgb(Y,Y,Y).
            Bitmap bm = new Bitmap(tmp.Width, tmp.Height);
            for (int y = 0; y < bm.Height; y++) {
            for (int x = 0; x < bm.Width; x++) {
            Color c = tmp.GetPixel(x, y);
            int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
            bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma));
            }
            }
            
            Point start = new Point(0,0);
            Size size= new Size( tmp.Width, tmp.Height);
            Rectangle r = new Rectangle(start, size);
            Bitmap bm2 = bm.Clone(r, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            //return bm.Clone(r, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            //return bm.Clone(new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height)), System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            // bm2;
            //return tmp.Clone(new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height)), System.Drawing.Imaging.PixelFormat.Format1bppIndexed); */

            return ConvertBitmapToMonochrome(tmp);
        }

        public static Bitmap CreateLargeTextDollyImageForEP500(DollyImageInfo imageInfo)
        {
            if (imageInfo == null)
                return null;

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            int line2_Y = 110;
            int line3_Y = 210;
            int bottomOfDisplay_Y = 240;

            TextDrawingInfo SortPlan_Info = new TextDrawingInfo();
            SortPlan_Info.text = GetAbbrevSortPlan(imageInfo.SortPlan);
            SortPlan_Info.position = new Point(0, 0);
            SortPlan_Info.size = new Size(320, line2_Y);
            SortPlan_Info.font = new Font("Arial", 78, FontStyle.Bold);
            SortPlan_Info.blackground = false;
            SortPlan_Info.isbarcode = false;
            textinfo_list.Add(SortPlan_Info);

            string abbrevDay = GetAbbrevDeliveryDay(imageInfo.DeliveryDate);

            TextDrawingInfo DeliveryDate_Info = new TextDrawingInfo();
            DeliveryDate_Info.text = abbrevDay;
            int xStartPoint = GetDollyDayLargeTextStartPoint(abbrevDay);
            DeliveryDate_Info.position = new Point(xStartPoint, line2_Y + GetAbbrevDayHeightAdjust(abbrevDay));
            DeliveryDate_Info.size = new Size((320 - xStartPoint), (line3_Y - line2_Y));
            int fontSize = GetAbbrevDayFontSize(abbrevDay);
            DeliveryDate_Info.font = new Font("Arial", fontSize, FontStyle.Bold);
            DeliveryDate_Info.blackground = false;
            textinfo_list.Add(DeliveryDate_Info);

            TextDrawingInfo DateCreated_Info = new TextDrawingInfo();
            DateCreated_Info.text = imageInfo.DateCreated;
            DateCreated_Info.position = new Point(0, line3_Y);
            DateCreated_Info.size = new Size(130, bottomOfDisplay_Y - line3_Y);
            DateCreated_Info.font = new Font("Arial", 18, FontStyle.Regular);
            DateCreated_Info.blackground = false;
            textinfo_list.Add(DateCreated_Info);

            TextDrawingInfo TimeCreated_Info = new TextDrawingInfo();
            TimeCreated_Info.text = imageInfo.TimeCreated;
            TimeCreated_Info.position = new Point(130, line3_Y);
            TimeCreated_Info.size = new Size(100, bottomOfDisplay_Y - line3_Y);
            TimeCreated_Info.font = new Font("Arial", 18, FontStyle.Regular);
            TimeCreated_Info.blackground = false;
            textinfo_list.Add(TimeCreated_Info);

            TextDrawingInfo Barcode_Info = new TextDrawingInfo();
            Barcode_Info.text = imageInfo.GetShortBarcode();
            Barcode_Info.position = new Point(230 + 10, line3_Y);
            Barcode_Info.size = new Size(90 - 10, bottomOfDisplay_Y - line3_Y);
            Barcode_Info.font = new Font("Arial", 18, FontStyle.Regular);
            Barcode_Info.blackground = false;
            textinfo_list.Add(Barcode_Info);

            // Brush string to the bitmap
            // initialize a bitmap with 320X240 in pixel, EP500 pixel size, and the format is 1 bit for 1 pixel.
            //Bitmap tmp = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format1bppIndexed );
            Bitmap tmp = new Bitmap(320, 240);
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);
                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.
                graphic.DrawRectangle(pen, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height)));
                graphic.DrawLine(pen, new Point(0, line2_Y), new Point(320, line2_Y));
                graphic.DrawLine(pen, new Point(0, line3_Y), new Point(320, line3_Y));
                graphic.DrawLine(pen, new Point(130, line3_Y), new Point(130, bottomOfDisplay_Y));
                graphic.DrawLine(pen, new Point(230, line3_Y), new Point(230, bottomOfDisplay_Y));

                foreach (TextDrawingInfo ei in textinfo_list)
                {
                    if (ei.isbarcode == false)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                    else
                    { // add a 1D Code39 barcode
                        Barcode bcode = new Barcode();
                        if (!ei.blackground)
                        {
                            bcode.Alignment = AlignmentPositions.CENTER;
                            bcode.IncludeLabel = true;
                            bcode.Encode(BarcodeLib.TYPE.CODE39, ei.text, ei.size.Width, ei.size.Height);
                        }
                        else
                        {
                            bcode.Alignment = AlignmentPositions.CENTER;
                            bcode.IncludeLabel = true;
                            bcode.Encode(BarcodeLib.TYPE.CODE39, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                        }
                        graphic.DrawImage(bcode.EncodedImage, ei.position);
                    }
                }
                graphic.Dispose();
            }
            return ConvertBitmapToMonochrome(tmp);
        }

        private static string GetAbbrevSortPlan(string fullSortPlan)
        {
            if (fullSortPlan.Length < 8)
            {
                return fullSortPlan;
            }
            string abbrevSortPlan = fullSortPlan.Substring(2, 5);
            return abbrevSortPlan;
        }

        private static int GetAbbrevDayHeightAdjust(string abbrevDay)
        {
            int numChars = abbrevDay.Length;
            switch (numChars)
            {
                case 3:
                    return -5;
                case 4:
                    return -5;
                case 5:
                    return 0;
                default:
                    return 0;
            }
        }

        private static int GetAbbrevDayFontSize(string abbrevDay)
        {
            int numChars = abbrevDay.Length;
            switch (numChars)
            {
                case 3:
                    return 74;
                case 4:
                    return 74;
                case 5:
                    return 65;
                default:
                    return 0;
            }
        }

        private static string GetAbbrevDeliveryDay(string day)
        {
            switch (day.ToUpper())
            {
                case "SUNDAY":
                    return "SUN";
                case "MONDAY":
                    return "MON";
                case "TUESDAY":
                    return "TUES";
                case "WEDNESDAY":
                    return "WED";
                case "THURSDAY":
                    return "THURS";
                case "FRIDAY":
                    return "FRI";
                case "SATURDAY":
                    return "SAT";
                default:
                    return "";
            }
        }

        private static int GetDollyDayLargeTextStartPoint(string abbrevDay)
        {
            int numChars = abbrevDay.Length;
            switch (numChars)
            {
                case 3:
                    return 30;
                case 4:
                    return 10;
                case 5:
                    return 0;
                default:
                    return 0;
            }
        }

        private static int GetDollyDayHeightStartPointAdjust(string day)
        {
            /*int numChars = day.Length;
            int spacePerChar = 18;
            int maxChars = 9;
            int initialStartPos = 0;
            if (numChars > maxChars) {
            return initialStartPos;
            }
            int startPos = initialStartPos + (maxChars - numChars) * spacePerChar;
            return startPos;*/
            int numChars = day.Length;
            switch (day)
            {
                case "Wednesday":
                    return 7;
                case "Thursday":
                    return -1;
                case "Friday":
                    return -5;
                default:
                    return -5;
            }
        }

        // char estimate: 9 chars (WEDESDAY) across 320 pixel width = 35.5 pixels per char
        // if centering, comes to 17.75 per side ~= 18
        private static int GetDollyDayStartPoint(string day)
        {
            int numChars = day.Length;
            switch (numChars)
            {
                case 6:
                    if (day.Equals("Friday"))
                    {
                        return 33;
                    }
                    return 20;
                case 7:
                    return 5;
                case 8:
                    return 5;
                default:
                    return 0;
            }
        }

        private static int GetCastrDeliverToZipcodeStartPoint(string zipcdoes)
        {
            if (zipcdoes.Length == 5)
            {
                return 50;
            }
            else
            {
                return 0;
            }
        }

        // original size was 42
        private static int GetDollyDayFontSize(string day)
        {
            int numChars = day.Length;
            switch (numChars)
            {
                case 6:
                    if (day.Equals("Friday"))
                    {
                        return 56;
                    }
                    return 54;
                case 7:
                    return 54;
                case 8:
                    return 48;
                default:
                    return 42;
            }
        }

        private static string GetZipCodeTextFormatted(string zipcodeString)
        {
            if (zipcodeString.Length > 17)
            {
                string firstThreeZipcodes = zipcodeString.Substring(0, 17) + "..";
                return firstThreeZipcodes;
            }
            return zipcodeString;
        }

        private static int GetDisplayHeight(int displayType)
        {
            switch (displayType)
            {
                case CastrImageInfo.S5:
                    return CoralImage.S5_HEIGHT;
                case CastrImageInfo.S3:
                    return CoralImage.S3_HEIGHT;
                case CastrImageInfo.S1:
                    return CoralImage.S1_HEIGHT;
                default:
                    throw new Exception("Unknown Display Type! ImageRender cannot generate image for type: " + displayType.ToString());
            }
        }

        private static int GetDisplayWidth(int displayType)
        {
            switch (displayType)
            {
                case CastrImageInfo.S5:
                    return CoralImage.S5_WIDTH;
                case CastrImageInfo.S3:
                    return CoralImage.S3_WIDTH;
                case CastrImageInfo.S1:
                    return CoralImage.S1_WIDTH;
                default:
                    throw new Exception("Unknown Display Type! ImageRender cannot generate image for type: " + displayType.ToString());
            }
        }

        public static Bitmap CreateCastrImageForEP500(CastrImageInfo imageInfo)
        {
            if (imageInfo == null)
                return null;

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            int line2_X_2 = 200;
            int middle_X = 320 / 2;
            int line2_Y = 70;
            int line3a_Y = 125;
            int line3b_Y = 168;
            int line4_Y = 210;
            int bottomOfDisplay_Y = 240;

            // Line 1: "TO: 90210" [deliver to: zipcode]
            TextDrawingInfo DeliverToZipcode_Info = new TextDrawingInfo();
            DeliverToZipcode_Info.text = imageInfo.Zipcodes;  //"TO: " + imageInfo.Zipcodes;
            DeliverToZipcode_Info.position = new Point(0, 0);
            int xStartPoint = GetCastrDeliverToZipcodeStartPoint(DeliverToZipcode_Info.text);
            DeliverToZipcode_Info.size = new Size((320 - xStartPoint), line2_Y);    //+ GetDollyDayHeightStartPointAdjust(imageInfo.DeliveryDate)
            DeliverToZipcode_Info.font = new Font("Arial", GetCastrDeliverToZipcodeFontSize(DeliverToZipcode_Info.text), FontStyle.Bold);
            DeliverToZipcode_Info.blackground = false;
            DeliverToZipcode_Info.isbarcode = false;
            textinfo_list.Add(DeliverToZipcode_Info);

            // Line 2 - left: "03/21/11" [dispatch date]
            TextDrawingInfo DispatchDate_Info = new TextDrawingInfo();
            DispatchDate_Info.text = imageInfo.DispatchDate;
            DispatchDate_Info.position = new Point(0, line2_Y);
            DispatchDate_Info.size = new Size(line2_X_2, (line3a_Y - line2_Y));
            DispatchDate_Info.font = new Font("Arial", 35, FontStyle.Bold);
            DispatchDate_Info.blackground = false;
            textinfo_list.Add(DispatchDate_Info);

            // Line 2 - Right: "FRI" [dispatch day]
            TextDrawingInfo DispatchDay_Info = new TextDrawingInfo();
            DispatchDay_Info.text = GetZipCodeTextFormatted(imageInfo.DispatchDay);
            DispatchDay_Info.position = new Point(line2_X_2, line2_Y);
            DispatchDay_Info.size = new Size((320 - line2_X_2), line3a_Y - line2_Y);
            DispatchDay_Info.font = new Font("Arial", 35, FontStyle.Bold);
            DispatchDay_Info.blackground = false;
            textinfo_list.Add(DispatchDay_Info);

            // Line 3a: "[20194 DPS] 20194" [delivery unit]
            TextDrawingInfo DeliveryUnit_Info1 = new TextDrawingInfo();
            //Zipcodes_Info.text = GetZipCodeTextFormatted(imageInfo.ZipCodes);
            string du1 = GetDeliveryUnitText(imageInfo.DeliveryUnit, 1);
            DeliveryUnit_Info1.text = du1;
            DeliveryUnit_Info1.position = new Point(0, line3a_Y);
            DeliveryUnit_Info1.size = new Size(320, line3b_Y - line3a_Y);
            DeliveryUnit_Info1.font = new Font("Arial", GetDeliveryUnitFontSize(du1), FontStyle.Regular);
            DeliveryUnit_Info1.blackground = false;
            textinfo_list.Add(DeliveryUnit_Info1);

            // Line 3b: "20194 DPS [20194]" [delivery unit]
            TextDrawingInfo DeliveryUnit_Info2 = new TextDrawingInfo();
            //Zipcodes_Info.text = GetZipCodeTextFormatted(imageInfo.ZipCodes);
            string du2 = GetDeliveryUnitText(imageInfo.DeliveryUnit, 2);
            DeliveryUnit_Info2.text = du2;
            DeliveryUnit_Info2.position = new Point(0, line3b_Y);
            DeliveryUnit_Info2.size = new Size(320, line4_Y - line3b_Y);
            DeliveryUnit_Info2.font = new Font("Arial", GetDeliveryUnitFontSize(du2), FontStyle.Regular);
            DeliveryUnit_Info2.blackground = false;
            textinfo_list.Add(DeliveryUnit_Info2);

            // Line 4 - left: "03/20/2011" [date created]
            TextDrawingInfo DateCreated_Info = new TextDrawingInfo();
            DateCreated_Info.text = imageInfo.DateCreated;
            DateCreated_Info.position = new Point(14, line4_Y);
            DateCreated_Info.size = new Size((middle_X - 14), bottomOfDisplay_Y - line4_Y);
            DateCreated_Info.font = new Font("Arial", 19, FontStyle.Bold);
            DateCreated_Info.blackground = false;
            textinfo_list.Add(DateCreated_Info);

            // Line 4 - right: "13:21:59" [time created]
            TextDrawingInfo TimeCreated_Info = new TextDrawingInfo();
            TimeCreated_Info.text = imageInfo.TimeCreated;
            TimeCreated_Info.position = new Point((middle_X + 20), line4_Y);
            TimeCreated_Info.size = new Size((middle_X - 24), bottomOfDisplay_Y - line4_Y);
            TimeCreated_Info.font = new Font("Arial", 19, FontStyle.Bold);
            TimeCreated_Info.blackground = false;
            textinfo_list.Add(TimeCreated_Info);

            // Brush string to the bitmap
            // initialize a bitmap with 320X240 in pixel, EP500 pixel size, and the format is 1 bit for 1 pixel.
            //Bitmap tmp = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format1bppIndexed );
            Bitmap tmp = new Bitmap(320, 240);
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);
                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.
                graphic.DrawRectangle(pen, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height)));
                graphic.DrawLine(pen, new Point(0, line2_Y), new Point(320, line2_Y));
                graphic.DrawLine(pen, new Point(0, line3a_Y), new Point(320, line3a_Y));
                graphic.DrawLine(pen, new Point(0, line4_Y), new Point(320, line4_Y));
                graphic.DrawLine(pen, new Point(line2_X_2, line2_Y), new Point(line2_X_2, line3a_Y));            // line 2-3 vertical separator
                graphic.DrawLine(pen, new Point(middle_X, line4_Y), new Point(middle_X, bottomOfDisplay_Y));    // bottom section veritcal separator

                foreach (TextDrawingInfo ei in textinfo_list)
                {
                    if (ei.isbarcode == false)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                    else
                    { // add a 1D Code39 barcode
                        Barcode bcode = new Barcode();
                        if (!ei.blackground)
                        {
                            bcode.Alignment = AlignmentPositions.CENTER;
                            bcode.IncludeLabel = true;
                            bcode.Encode(BarcodeLib.TYPE.CODE39, ei.text, ei.size.Width, ei.size.Height);
                        }
                        else
                        {
                            bcode.Alignment = AlignmentPositions.CENTER;
                            bcode.IncludeLabel = true;
                            bcode.Encode(BarcodeLib.TYPE.CODE39, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                        }
                        graphic.DrawImage(bcode.EncodedImage, ei.position);
                    }
                }
                // graphic.Dispose();
            }
            return ConvertBitmapToMonochrome(tmp);
        }

        // lineNum: should be Line 1 or Line 2
        private static string GetDeliveryUnitText(string text, int lineNum)
        {
            string[] words = GetTrimmedWordArray(text);
            if (words.Length < 1)
            {
                return "";
            }
            else if (words.Length == 1)
            {
                if (lineNum == 1)
                {
                    return words[0].Trim();
                }
                return "";
            }
            else if (words.Length == 2)
            {
                if (lineNum == 1)
                {
                    return words[0].Trim();
                }
                return words[1].Trim();
            }
            else if (words.Length >= 3)
            {
                if (lineNum == 1)
                {
                    return words[0].Trim() + " " + words[1].Trim();
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 2; i < words.Length; i++)
                    {
                        if (i != 2)
                        {
                            sb.Append(" ");
                        }
                        sb.Append(words[i].Trim());
                    }
                    return sb.ToString();
                }
            }
            return "";
        }

        private static string[] GetTrimmedWordArray(string text)
        {
            string[] splitText = text.Split(' ');
            List<string> words = new List<string>(text.Length);
            for (int i = 0; i < splitText.Length; i++)
            {
                string curWord = splitText[i].Trim();
                if (!curWord.Equals(""))
                {
                    words.Add(curWord);
                }
            }
            return words.ToArray();
        }

        private static int GetCastrDeliverToZipcodeFontSize(string zipcodes)
        {
            if (zipcodes.Length == 5)
            {
                return 50;
            }
            else
            {
                return 39;
            }
        }

        private static int GetDeliveryUnitFontSize(string deliveryUnit)
        {
            /*int numChars = day.Length;
            int spacePerChar = 18;
            int maxChars = 9;
            int initialStartPos = 0;
            if (numChars > maxChars) {
            return initialStartPos;
            }
            int startPos = initialStartPos + (maxChars - numChars) * spacePerChar;
            return startPos;*/
            return 26;  // fix this
        }

        private static Bitmap ConvertBitmapToMonochrome(Bitmap bitmap)
        {
            int threshold = 127;
            int index = 0;
            int dimensions = bitmap.Height * bitmap.Width;

            System.Collections.BitArray bits = new System.Collections.BitArray(dimensions);
            Bitmap bm = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
            System.Drawing.Imaging.BitmapData bmapdata = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);

            //Vertically 
            for (int y = 0; y < bitmap.Height; y++)
            {
                //Horizontally 
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    int luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    if (luminance > threshold)
                    {
                        bits[index] = true;
                        SetIndexedPixel(x, y, bmapdata, true);
                    }
                    else
                    {
                        bits[index] = false;
                        SetIndexedPixel(x, y, bmapdata, false);
                    }
                    index++;
                }
            }
            bm.UnlockBits(bmapdata);
            byte[] data = new byte[dimensions / 8];
            bits.CopyTo(data, 0);
            //return data; 
            return bm;
        }

        private static void SetIndexedPixel(int x, int y, System.Drawing.Imaging.BitmapData bmd, Boolean pixel)
        {
            int index = y * bmd.Stride + (x >> 3);
            Byte p = System.Runtime.InteropServices.Marshal.ReadByte(bmd.Scan0, index);
            Byte mask = Convert.ToByte(0x80 >> (x & 0x7));

            if (pixel)
            {
                p = Convert.ToByte(p | mask);
            }
            else
            {
                p = Convert.ToByte(p & Convert.ToByte(mask ^ 0xFF));
            }
            System.Runtime.InteropServices.Marshal.WriteByte(bmd.Scan0, index, p);
        }

        public static Bitmap CreateImageForEP300(List<ImageField> imageInfo)
        {
            if (imageInfo == null)
                throw new ArgumentNullException();
            // Requires 7 elements in imageInfo. If they are not supplied is filled with empty data
            while (imageInfo.Count < 7)
            {
                imageInfo.Add(new ImageField()
                {
                    Field = " ",
                    BarcodeType = BarcodeType.NONE
                });
            }

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            // Field 1A (routeNum -- ex. "TO: 22030")
            TextDrawingInfo field1A = new TextDrawingInfo();
            field1A.text = "TO: " + imageInfo[0].Field;
            field1A.position = new Point(0, 0);
            field1A.size = new Size(310, 27);
            field1A.font = new Font("Arial", 18, FontStyle.Bold);
            field1A.blackground = false;
            field1A.barcodeType = imageInfo[0].BarcodeType;
            textinfo_list.Add(field1A);

            // Field 1B (pNum -- ex. "P2")
            TextDrawingInfo field1B = new TextDrawingInfo();
            field1B.text = imageInfo[1].Field;
            field1B.position = new Point(310, 0);
            field1B.size = new Size(90, 33);
            field1B.font = new Font("Arial", 18, FontStyle.Bold);
            field1B.blackground = false;
            field1B.barcodeType = imageInfo[1].BarcodeType;
            textinfo_list.Add(field1B);

            // Field 2A (date -- ex. "11/10/10")
            TextDrawingInfo field2A = new TextDrawingInfo();
            field2A.text = imageInfo[2].Field;
            field2A.position = new Point(0, 27);
            field2A.size = new Size(155, 26);
            field2A.font = new Font("Arial", 18, FontStyle.Bold);
            field2A.blackground = false;
            field2A.barcodeType = imageInfo[2].BarcodeType;
            textinfo_list.Add(field2A);

            // Field 2B (day -- ex. "WED")
            TextDrawingInfo field2B = new TextDrawingInfo();
            field2B.text = imageInfo[3].Field;
            field2B.position = new Point(155, 27);
            field2B.size = new Size(155, 26);
            field2B.font = new Font("Arial", 18, FontStyle.Bold);
            field2B.blackground = false;
            field2B.barcodeType = imageInfo[3].BarcodeType;
            textinfo_list.Add(field2B);

            // Field 3A (routeInfo -- ex. "22030  DPS Rural 22030")
            TextDrawingInfo field3A = new TextDrawingInfo();
            field3A.text = imageInfo[4].Field;
            field3A.position = new Point(0, 53);
            field3A.size = new Size(310, 43);
            field3A.font = new Font("Arial", 18, FontStyle.Regular);
            field3A.blackground = false;
            field3A.barcodeType = imageInfo[4].BarcodeType;
            textinfo_list.Add(field3A);

            // Field 3B (sortNum -- ex. "2-8")
            TextDrawingInfo field3B = new TextDrawingInfo();
            field3B.text = imageInfo[5].Field;
            field3B.position = new Point(310, 27);
            field3B.size = new Size(155, 69);
            field3B.font = new Font("Arial", 20, FontStyle.Bold);
            field3B.blackground = false;
            field3B.barcodeType = imageInfo[5].BarcodeType;
            textinfo_list.Add(field3B);

            // Field 4 [BARCODE] (barCode -- ex. "99P22030ED0313---170000017")
            TextDrawingInfo field4 = new TextDrawingInfo();
            field4.text = imageInfo[6].Field;
            field4.position = new Point(0, 96);
            field4.size = new Size(400, 60);
            field4.font = new Font("Arial", 20, FontStyle.Regular);
            field4.blackground = false;
            field4.barcodeType = imageInfo[6].BarcodeType;
            textinfo_list.Add(field4);

            // Brush string to the bitmap
            // initialize a bitmap with 400x160 in pixel, S3 pixel size, and the format is 1 bit for 1 pixel.
            //Bitmap tmp = new Bitmap(400, 160, System.Drawing.Imaging.PixelFormat.Format1bppIndexed );
            Bitmap tmp = new Bitmap(400, 160);  // width,height
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);

                try
                {
                    graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill entire background with white pixel.

                    graphic.DrawLine(pen, new Point(155, 27), new Point(155, 53));      // VERT line between F2.A and F2.B (column division 1)
                    graphic.DrawLine(pen, new Point(310, 0), new Point(310, 96));      // VERT line for column division 2
                    graphic.DrawLine(pen, new Point(0, 27), new Point(310, 27));        // HORZ line between F1 and F2
                    graphic.DrawLine(pen, new Point(0, 53), new Point(310, 53));        // HORZ line between F2 and F3
                    graphic.DrawLine(pen, new Point(0, 96), new Point(400, 96));      // HORZ line between F3 and F4                

                    for (int i = 0; i < textinfo_list.Count; i++)
                    {
                        TextDrawingInfo ei = textinfo_list[i];
                        if (ei.barcodeType == BarcodeType.ONE_D)
                        {
                            try
                            {
                                Barcode bcode = new Barcode();
                                if (!ei.blackground)
                                {
                                    bcode.Alignment = AlignmentPositions.CENTER;
                                    bcode.IncludeLabel = true;
                                    bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, ei.size.Width, ei.size.Height);
                                }
                                else
                                {
                                    bcode.Alignment = AlignmentPositions.CENTER;
                                    bcode.IncludeLabel = true;
                                    bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                                }
                                graphic.DrawImage(bcode.EncodedImage, ei.position);
                                ei.isbarcode = true;
                            }
                            catch (Exception)
                            {
                                ei.isbarcode = false;
                            }
                        }
                        else if (ei.barcodeType == BarcodeType.TWO_D)
                        {
                            try
                            {
                                Image bm = CreateQRCodeBitmap(ei.text, ei.size.Height);
                                graphic.DrawImage(bm, ei.position);
                                ei.isbarcode = true;
                            }
                            catch (Exception)
                            {
                                ei.isbarcode = false;
                            }
                        }
                        if (!ei.isbarcode)
                        {
                            if (!ei.blackground)
                            {
                                graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                            }
                            else
                            {
                                graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                                graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                            }
                        }
                    }
                    graphic.DrawRectangle(pen, new Rectangle(new Point(0, 0), new Size(tmp.Width - 1, tmp.Height - 1)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EXCEPTION trying to Generate 400x160 [S3] image! " + ex.Message);
                }
                finally
                {
                    graphic.Dispose();
                }
            }
            return ConvertBitmapToMonochrome(tmp);
        }

        public static Bitmap CreateImage_TNT1_S3(string routeNum, string pNum, string date, string day, string routeInfo, string sortNum, string barCode)
        {
            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            // Field 1A (routeNum -- ex. "TO: 22030")
            TextDrawingInfo field1A = new TextDrawingInfo();
            field1A.text = "TO: " + routeNum;
            field1A.position = new Point(0, 0);
            field1A.size = new Size(310, 27);
            field1A.font = new Font("Arial", 18, FontStyle.Bold);
            field1A.blackground = false;
            textinfo_list.Add(field1A);

            // Field 1B (pNum -- ex. "P2")
            TextDrawingInfo field1B = new TextDrawingInfo();
            field1B.text = pNum;
            field1B.position = new Point(310, 0);
            field1B.size = new Size(90, 33);
            field1B.font = new Font("Arial", 18, FontStyle.Bold);
            field1B.blackground = false;
            textinfo_list.Add(field1B);

            // Field 2A (date -- ex. "11/10/10")
            TextDrawingInfo field2A = new TextDrawingInfo();
            field2A.text = date;
            field2A.position = new Point(0, 27);
            field2A.size = new Size(155, 26);
            field2A.font = new Font("Arial", 18, FontStyle.Bold);
            field2A.blackground = false;
            textinfo_list.Add(field2A);

            // Field 2B (day -- ex. "WED")
            TextDrawingInfo field2B = new TextDrawingInfo();
            field2B.text = day;
            field2B.position = new Point(155, 27);
            field2B.size = new Size(155, 26);
            field2B.font = new Font("Arial", 18, FontStyle.Bold);
            field2B.blackground = false;
            textinfo_list.Add(field2B);

            // Field 3A (routeInfo -- ex. "22030  DPS Rural 22030")
            TextDrawingInfo field3A = new TextDrawingInfo();
            field3A.text = routeInfo;
            field3A.position = new Point(0, 53);
            field3A.size = new Size(310, 43);
            field3A.font = new Font("Arial", 18, FontStyle.Regular);
            field3A.blackground = false;
            textinfo_list.Add(field3A);

            // Field 3B (sortNum -- ex. "2-8")
            TextDrawingInfo field3B = new TextDrawingInfo();
            field3B.text = sortNum;
            field3B.position = new Point(310, 27);
            field3B.size = new Size(155, 69);
            field3B.font = new Font("Arial", 40, FontStyle.Bold);
            field3B.blackground = false;
            textinfo_list.Add(field3B);

            // Field 4 [BARCODE] (barCode -- ex. "99P22030ED0313---170000017")
            TextDrawingInfo field4 = new TextDrawingInfo();
            field4.text = barCode;
            field4.position = new Point(0, 96);
            field4.size = new Size(400, 64);
            field4.font = new Font("Arial", 20, FontStyle.Regular);
            field4.blackground = false;
            field4.isbarcode = true;
            textinfo_list.Add(field4);

            // Brush string to the bitmap
            // initialize a bitmap with 400x160 in pixel, S3 pixel size, and the format is 1 bit for 1 pixel.
            //Bitmap tmp = new Bitmap(400, 160, System.Drawing.Imaging.PixelFormat.Format1bppIndexed );
            Bitmap tmp = new Bitmap(400, 160);  // width,height
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);

                try
                {
                    graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill entire background with white pixel.

                    graphic.DrawLine(pen, new Point(155, 27), new Point(155, 53));      // VERT line between F2.A and F2.B (column division 1)
                    graphic.DrawLine(pen, new Point(310, 0), new Point(310, 96));      // VERT line for column division 2
                    graphic.DrawLine(pen, new Point(0, 27), new Point(310, 27));        // HORZ line between F1 and F2
                    graphic.DrawLine(pen, new Point(0, 53), new Point(310, 53));        // HORZ line between F2 and F3
                    graphic.DrawLine(pen, new Point(0, 107), new Point(400, 96));      // HORZ line between F3 and F4                

                    foreach (TextDrawingInfo ei in textinfo_list)
                    {
                        if (ei.isbarcode == false)
                        {
                            if (!ei.blackground)
                            {
                                graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                            }
                            else
                            {
                                graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                                graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                            }
                        }
                        else
                        { // add a 1D Code39 barcode
                            Barcode bcode = new Barcode();
                            if (!ei.blackground)
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE39, ei.text, ei.size.Width, ei.size.Height);
                            }
                            else
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE39, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                            }
                            graphic.DrawImage(bcode.EncodedImage, ei.position);
                        }
                    }
                    graphic.DrawRectangle(pen, new Rectangle(new Point(0, 0), new Size(tmp.Width - 1, tmp.Height - 1)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("EXCEPTION trying to Generate 400x160 [S3] image! " + ex.Message);
                }
                finally
                {
                    graphic.Dispose();
                }
            }
            return ConvertBitmapToMonochrome(tmp);
        }

        public static Bitmap CreateImageForP4(List<ImageField> imageInfo)
        {
            if (imageInfo == null)
                throw new ArgumentNullException();
            // Requires 7 elements in imageInfo. If they are not supplied is filled with empty data
            while (imageInfo.Count < 6)
            {
                imageInfo.Add(new ImageField()
                {
                    Field = " ",
                    BarcodeType = BarcodeType.NONE
                });
            }

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            const int line2_Y = 83;
            const int line3_Y = 165;  //144
            const int line4_Y = 210;
            const int bottomOfDisplay_Y = 300;

            TextDrawingInfo SortPlan_Info = new TextDrawingInfo();
            SortPlan_Info.text = imageInfo[0].Field;
            SortPlan_Info.position = new Point(28, 40);
            SortPlan_Info.size = new Size(350, 70);
            SortPlan_Info.font = new Font("Arial", 20, FontStyle.Bold);
            SortPlan_Info.blackground = false;
            SortPlan_Info.barcodeType = BarcodeType.ONE_D;
            textinfo_list.Add(SortPlan_Info);

            TextDrawingInfo DeliveryDate_Info = new TextDrawingInfo();
            DeliveryDate_Info.text = "MODULE NO.";
            DeliveryDate_Info.position = new Point(2, 3);
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            DeliveryDate_Info.size = new Size((400 - 100), (line3_Y - 50));
            DeliveryDate_Info.font = new Font("Arial", 22, FontStyle.Bold);
            DeliveryDate_Info.blackground = false;
            textinfo_list.Add(DeliveryDate_Info);

            TextDrawingInfo DeliveryDate1_Info = new TextDrawingInfo();
            DeliveryDate1_Info.text = imageInfo[0].Field;
            DeliveryDate1_Info.position = new Point(220, 3);
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            DeliveryDate1_Info.size = new Size((400 - 100), (line3_Y - 50));
            DeliveryDate1_Info.font = new Font("Arial", 22, FontStyle.Bold);
            DeliveryDate1_Info.blackground = false;
            textinfo_list.Add(DeliveryDate1_Info);

            TextDrawingInfo car1_Info = new TextDrawingInfo();
            car1_Info.text = "PART NO.";
            car1_Info.position = new Point(1, 123);
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            car1_Info.size = new Size((400 - 100), (line3_Y - 50));
            car1_Info.font = new Font("Arial", 16, FontStyle.Bold);
            car1_Info.blackground = false;
            textinfo_list.Add(car1_Info);

            TextDrawingInfo car2_Info = new TextDrawingInfo();
            car2_Info.text = imageInfo[1].Field;
            car2_Info.position = new Point(-2, 138);
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            car2_Info.size = new Size((400 - 100), (line3_Y - 50));
            car2_Info.font = new Font("Arial", 43, FontStyle.Bold);
            car2_Info.blackground = false;
            textinfo_list.Add(car2_Info);

            TextDrawingInfo car3_Info = new TextDrawingInfo();
            car3_Info.text = "DESCRIPTION";
            car3_Info.position = new Point(1, 207);
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            car3_Info.size = new Size((400 - 100), (line3_Y - 50));
            car3_Info.font = new Font("Arial", 15, FontStyle.Bold);
            car3_Info.blackground = false;
            textinfo_list.Add(car3_Info);



            TextDrawingInfo car_Info = new TextDrawingInfo();
            // calculate the width so we can change the font size so the text fits within 340 pixels
            Font font = null;
            int fontSize = 19;
            while (fontSize > 10)
            {
                font = new Font("Arial", fontSize, FontStyle.Bold);
                Bitmap tmp1 = new Bitmap(400, 300);
                using (Graphics graphic = Graphics.FromImage(tmp1))
                {
                    SizeF stringSize = graphic.MeasureString(imageInfo[2].Field, font);
                    if (stringSize.Width > 340)
                    {
                        fontSize -= 2;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            car_Info.text = imageInfo[2].Field;
            car_Info.position = new Point(0, 234);
            //DeliveryDate_Info.size = new Size((244-20), (144-80));
            car_Info.size = new Size((400 - 100), (line3_Y - line2_Y));
            car_Info.font = font;
            car_Info.blackground = false;
            textinfo_list.Add(car_Info);

            TextDrawingInfo DateCreated_Info = new TextDrawingInfo();
            DateCreated_Info.text = imageInfo[3].Field;
            DateCreated_Info.position = new Point(342, 211);
            DateCreated_Info.size = new Size(130, bottomOfDisplay_Y - line4_Y + 10);
            DateCreated_Info.font = new Font("Arial", 34, FontStyle.Bold);
            DateCreated_Info.blackground = false;
            textinfo_list.Add(DateCreated_Info);

            // Brush string to the bitmap
            // initialize a bitmap with 264X176 in pixel, P3 pixel size, and the format is 1 bit for 1 pixel.
            //Bitmap tmp = new Bitmap(264, 176, System.Drawing.Imaging.PixelFormat.Format1bppIndexed );
            Bitmap tmp = new Bitmap(400, 300);
            using (Graphics graphic = Graphics.FromImage(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                Pen pen = new Pen(brushBlack);
                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.
                graphic.DrawRectangle(pen, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height)));

                graphic.DrawLine(pen, new Point(0, 119), new Point(400, 119));
                graphic.DrawLine(pen, new Point(0, 203), new Point(400, 203));

                graphic.DrawLine(pen, new Point(347, 203), new Point(347, bottomOfDisplay_Y));

                for (int i = 0; i < textinfo_list.Count; i++)
                {
                    TextDrawingInfo ei = textinfo_list[i];
                    if (ei.barcodeType == BarcodeType.ONE_D)
                    { // add a 1D Code39 barcode
                        Barcode bcode = new Barcode();
                        try
                        {
                            if (!ei.blackground)
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = false;
                                bcode.Encode(BarcodeLib.TYPE.CODE39, ei.text, ei.size.Width, ei.size.Height);
                            }
                            else
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = false;
                                bcode.Encode(BarcodeLib.TYPE.CODE39, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                            }
                            graphic.DrawImage(bcode.EncodedImage, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    else if (ei.barcodeType == BarcodeType.TWO_D)
                    {
                        try
                        {
                            Image bm = CreateQRCodeBitmap(ei.text, ei.size.Height);
                            graphic.DrawImage(bm, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    if (!ei.isbarcode)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                }
                graphic.Dispose();
            }

            return ConvertBitmapToMonochrome(tmp);
        }

        public static Bitmap CreateImageForBridgeStoneP4_1(List<ImageField> imageInfo)
        {
            if (imageInfo == null)
                throw new ArgumentNullException();
            // Requires 10 elements in imageInfo. If they are not supplied is filled with empty data
            while (imageInfo.Count < 10)
            {
                imageInfo.Add(new ImageField()
                {
                    Field = " ",
                    BarcodeType = BarcodeType.NONE
                });
            }

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            const int line1_Y = 15;
            const int line2_Y = 52;
            const int line3_Y = 175;
            const int line4_Y = 214;
            const int line5_Y = 250;
            const int column1_X = 0;
            const int column2_X = 250;
            const int line1_X1 = 170;
            const int line1_X2 = 270;

            TextDrawingInfo label1 = new TextDrawingInfo();
            label1.text = "Producing M/C";
            label1.position = new Point(0, 0);
            label1.font = new Font("Courier New", 12, FontStyle.Regular);
            textinfo_list.Add(label1);

            TextDrawingInfo field1 = new TextDrawingInfo();
            field1.text = imageInfo[0].Field;
            field1.position = new Point(0, line1_Y);
            field1.font = new Font("Courier New", 12, FontStyle.Bold);
            textinfo_list.Add(field1);

            TextDrawingInfo label2 = new TextDrawingInfo();
            label2.text = "BKT";
            label2.position = new Point(line1_X1, 0);
            label2.font = new Font("Courier New", 12, FontStyle.Regular);
            textinfo_list.Add(label2);

            TextDrawingInfo field2 = new TextDrawingInfo();
            field2.text = imageInfo[1].Field;
            field2.position = new Point(line1_X1, line1_Y);
            field2.font = new Font("Courier New", 12, FontStyle.Bold);
            textinfo_list.Add(field2);

            TextDrawingInfo label3 = new TextDrawingInfo();
            label3.text = "Location";
            label3.position = new Point(line1_X2, 0);
            label3.font = new Font("Courier New", 12, FontStyle.Regular);
            textinfo_list.Add(label3);

            TextDrawingInfo field3 = new TextDrawingInfo();
            field3.text = imageInfo[2].Field;
            field3.position = new Point(line1_X2, line1_Y);
            field3.font = new Font("Courier New", 12, FontStyle.Bold);
            textinfo_list.Add(field3);

            TextDrawingInfo label4 = new TextDrawingInfo();
            label4.text = "Material ID";
            label4.position = new Point(0, line2_Y - 17);
            label4.font = new Font("Courier New", 12, FontStyle.Regular);
            textinfo_list.Add(label4);

            TextDrawingInfo field4 = new TextDrawingInfo();
            field4.text = imageInfo[3].Field;
            field4.position = new Point(2, line2_Y);
            field4.size = new Size(398, 100);
            field4.font = new Font("Courier New", 12, FontStyle.Bold);
            field4.blackground = false;
            field4.isbarcode = true;
            field4.barcodeType = BarcodeType.ONE_D;
            textinfo_list.Add(field4);

            TextDrawingInfo label5 = new TextDrawingInfo();
            label5.text = "Production Date";
            label5.position = new Point(0, line3_Y - 15);
            label5.font = new Font("Courier New", 12, FontStyle.Regular);
            textinfo_list.Add(label5);

            TextDrawingInfo field5 = new TextDrawingInfo();
            field5.text = imageInfo[4].Field;
            field5.position = new Point(0, line3_Y);
            field5.font = new Font("Courier New", 12, FontStyle.Bold);
            textinfo_list.Add(field5);

            TextDrawingInfo label6 = new TextDrawingInfo();
            label6.text = "Production Qty";
            label6.position = new Point(column2_X, line3_Y - 15);
            label6.font = new Font("Courier New", 12, FontStyle.Regular);
            textinfo_list.Add(label6);

            TextDrawingInfo field6 = new TextDrawingInfo();
            field6.text = imageInfo[5].Field;
            field6.position = new Point(column2_X, line3_Y);
            field6.font = new Font("Courier New", 12, FontStyle.Bold);
            field6.blackground = false;
            textinfo_list.Add(field6);

            TextDrawingInfo label7 = new TextDrawingInfo();
            label7.text = "Expiration Date";
            label7.position = new Point(0, line4_Y - 15);
            label7.font = new Font("Courier New", 12, FontStyle.Regular);
            textinfo_list.Add(label7);

            TextDrawingInfo field7 = new TextDrawingInfo();
            field7.text = imageInfo[6].Field;
            field7.position = new Point(0, line4_Y);
            field7.font = new Font("Courier New", 12, FontStyle.Bold);
            field7.blackground = false;
            textinfo_list.Add(field7);

            TextDrawingInfo label8 = new TextDrawingInfo();
            label8.text = "Qty";
            label8.position = new Point(column2_X, line4_Y - 15);
            label8.font = new Font("Courier New", 12, FontStyle.Regular);
            textinfo_list.Add(label8);

            TextDrawingInfo field8 = new TextDrawingInfo();
            field8.text = imageInfo[7].Field;
            field8.position = new Point(column2_X, line4_Y);
            field8.font = new Font("Courier New", 12, FontStyle.Bold);
            field8.blackground = false;
            textinfo_list.Add(field8);

            TextDrawingInfo label9 = new TextDrawingInfo();
            label9.text = "Lot No.";
            label9.position = new Point(0, line5_Y - 15);
            label9.font = new Font("Courier New", 12, FontStyle.Regular);
            textinfo_list.Add(label9);

            TextDrawingInfo field9 = new TextDrawingInfo();
            field9.text = imageInfo[8].Field;
            field9.position = new Point(0, line5_Y);
            field9.font = new Font("Courier New", 12, FontStyle.Bold);
            field9.blackground = false;
            textinfo_list.Add(field9);

            TextDrawingInfo label10 = new TextDrawingInfo();
            label10.text = "Cart No.";
            label10.position = new Point(column2_X, line5_Y - 15);
            label10.font = new Font("Courier New", 12, FontStyle.Regular);
            textinfo_list.Add(label10);

            TextDrawingInfo field10 = new TextDrawingInfo();
            field10.text = imageInfo[9].Field;
            field10.position = new Point(column2_X, line5_Y);
            field10.font = new Font("Courier New", 12, FontStyle.Bold);
            field10.blackground = false;
            textinfo_list.Add(field10);

            // Brush string to the bitmap
            // initialize a bitmap with 264X176 in pixel, P3 pixel size, and the format is 1 bit for 1 pixel.
            //Bitmap tmp = new Bitmap(264, 176, System.Drawing.Imaging.PixelFormat.Format1bppIndexed );
            Bitmap tmp = new Bitmap(400, 300);
            using (Graphics graphic = GetGraphic(tmp))
            {                                
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.

                Pen pen = new Pen(brushBlack);
                graphic.DrawRectangle(pen, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // border
                graphic.DrawLine(pen, new Point(0, line1_Y + 18), new Point(tmp.Width, line1_Y + 18));
                graphic.DrawLine(pen, new Point(line1_X1 - 20, line1_Y + 18), new Point(line1_X1 - 20, 0));
                graphic.DrawLine(pen, new Point(line1_X2 - 20, line1_Y + 18), new Point(line1_X2 - 20, 0));
                graphic.DrawLine(pen, new Point(0, line3_Y - 18), new Point(tmp.Width, line3_Y - 18));
                graphic.DrawLine(pen, new Point(0, line4_Y - 18), new Point(tmp.Width, line4_Y - 18));
                graphic.DrawLine(pen, new Point(0, line5_Y - 16), new Point(tmp.Width, line5_Y - 16));
                graphic.DrawLine(pen, new Point(column2_X - 50, tmp.Height), new Point(column2_X - 50, line3_Y - 18));

                for (int i = 0; i < textinfo_list.Count; i++)
                {
                    TextDrawingInfo ei = textinfo_list[i];
                    if (ei.barcodeType == BarcodeType.ONE_D)
                    { // add a 1D Code128 barcode
                        Barcode bcode = new Barcode();
                        try
                        {
                            if (!ei.blackground)
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, ei.size.Width, ei.size.Height);
                            }
                            else
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                            }
                            graphic.DrawImage(bcode.EncodedImage, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    else if (ei.barcodeType == BarcodeType.TWO_D)
                    {
                        try
                        {
                            Image bm = CreateQRCodeBitmap(ei.text, ei.size.Height);
                            graphic.DrawImage(bm, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    if (!ei.isbarcode)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                }
                graphic.Dispose();
            }

            return ConvertBitmapToMonochrome(tmp);
        }
        
        public static Bitmap CreateImageForBridgeStoneP4_2(List<ImageField> imageInfo)
        {
            if (imageInfo == null)
                throw new ArgumentNullException();
            // Requires 1 element in imageInfo. If they are not supplied is filled with empty data
            while (imageInfo.Count < 1)
            {
                imageInfo.Add(new ImageField()
                {
                    Field = " ",
                    BarcodeType = BarcodeType.NONE
                });
            }

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            TextDrawingInfo label1 = new TextDrawingInfo();
            label1.text = "Scheduling No.";
            label1.position = new Point(0, 0);
            label1.font = new Font("Courier New", 15, FontStyle.Regular);
            textinfo_list.Add(label1);

            int fontSize = CalculateFontSize("Courier New", imageInfo[0].Field, 400, 300, 390);

            TextDrawingInfo field1 = new TextDrawingInfo();
            field1.text = imageInfo[0].Field;
            field1.position = new Point(0, 25);
            field1.font = new Font("Courier New", fontSize, FontStyle.Bold);
            textinfo_list.Add(field1);

            // Brush string to the bitmap
            // initialize a bitmap with 264X176 in pixel, P3 pixel size, and the format is 1 bit for 1 pixel.
            //Bitmap tmp = new Bitmap(264, 176, System.Drawing.Imaging.PixelFormat.Format1bppIndexed );
            Bitmap tmp = new Bitmap(400, 300);
            using (Graphics graphic = GetGraphic(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.

                //Pen pen = new Pen(brushBlack);
                //graphic.DrawRectangle(pen, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // border

                for (int i = 0; i < textinfo_list.Count; i++)
                {
                    TextDrawingInfo ei = textinfo_list[i];
                    if (ei.barcodeType == BarcodeType.ONE_D)
                    { // add a 1D Code128 barcode
                        Barcode bcode = new Barcode();
                        try
                        {
                            if (!ei.blackground)
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, ei.size.Width, ei.size.Height);
                            }
                            else
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                            }
                            graphic.DrawImage(bcode.EncodedImage, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    else if (ei.barcodeType == BarcodeType.TWO_D)
                    {
                        try
                        {
                            Image bm = CreateQRCodeBitmap(ei.text, ei.size.Height);
                            graphic.DrawImage(bm, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    if (!ei.isbarcode)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                }
                graphic.Dispose();
            }

            return ConvertBitmapToMonochrome(tmp);
        }

        public static Bitmap CreateImageForBridgeStoneP4_3(List<ImageField> imageInfo)
        {
            if (imageInfo == null)
                throw new ArgumentNullException();
            // Requires 1 element in imageInfo. If they are not supplied is filled with empty data
            while (imageInfo.Count < 1)
            {
                imageInfo.Add(new ImageField()
                {
                    Field = " ",
                    BarcodeType = BarcodeType.NONE
                });
            }

            List<TextDrawingInfo> textinfo_list = new List<TextDrawingInfo>();

            TextDrawingInfo label1 = new TextDrawingInfo();
            label1.text = "Quality/Trouble Message";
            label1.position = new Point(0, 0);
            label1.font = new Font("Courier New", 15, FontStyle.Regular);
            textinfo_list.Add(label1);

            int fontSize = 12;
            float lineHeight;
            List<string> lines = CalculateLines("Courier New", fontSize, imageInfo[0].Field, 400, 275, out lineHeight);

            int lineCnt = 0;
            foreach (string line in lines)
            {
                TextDrawingInfo field1 = new TextDrawingInfo();
                field1.text = line;
                field1.position = new Point(0, 25 + (int)(lineCnt * lineHeight));
                field1.font = new Font("Courier New", fontSize, FontStyle.Bold);
                textinfo_list.Add(field1);
                lineCnt++;
            }

            // Brush string to the bitmap
            // initialize a bitmap with 264X176 in pixel, P3 pixel size, and the format is 1 bit for 1 pixel.
            //Bitmap tmp = new Bitmap(264, 176, System.Drawing.Imaging.PixelFormat.Format1bppIndexed );
            Bitmap tmp = new Bitmap(400, 300);
            using (Graphics graphic = GetGraphic(tmp))
            {
                SolidBrush brushBlack = new SolidBrush(Color.Black);
                SolidBrush brushWhite = new SolidBrush(Color.White);

                graphic.FillRectangle(brushWhite, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // fill with white pixel.

                //Pen pen = new Pen(brushBlack);
                //graphic.DrawRectangle(pen, new Rectangle(new Point(0, 0), new Size(tmp.Width, tmp.Height))); // border

                for (int i = 0; i < textinfo_list.Count; i++)
                {
                    TextDrawingInfo ei = textinfo_list[i];
                    if (ei.barcodeType == BarcodeType.ONE_D)
                    { // add a 1D Code128 barcode
                        Barcode bcode = new Barcode();
                        try
                        {
                            if (!ei.blackground)
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, ei.size.Width, ei.size.Height);
                            }
                            else
                            {
                                bcode.Alignment = AlignmentPositions.CENTER;
                                bcode.IncludeLabel = true;
                                bcode.Encode(BarcodeLib.TYPE.CODE128B, ei.text, Color.White, Color.Black, ei.size.Width, ei.size.Height);
                            }
                            graphic.DrawImage(bcode.EncodedImage, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    else if (ei.barcodeType == BarcodeType.TWO_D)
                    {
                        try
                        {
                            Image bm = CreateQRCodeBitmap(ei.text, ei.size.Height);
                            graphic.DrawImage(bm, ei.position);
                            ei.isbarcode = true;
                        }
                        catch (Exception)
                        {
                            ei.isbarcode = false;
                        }
                    }
                    if (!ei.isbarcode)
                    {
                        if (!ei.blackground)
                        {
                            graphic.DrawString(ei.text, ei.font, brushBlack, ei.position);
                        }
                        else
                        {
                            graphic.FillRectangle(brushBlack, new Rectangle(ei.position, ei.size));
                            graphic.DrawString(ei.text, ei.font, brushWhite, ei.position);
                        }
                    }
                }
                graphic.Dispose();
            }

            return ConvertBitmapToMonochrome(tmp);
        }

        public static Bitmap CreateClearedDollyImage(DollyImageInfo imageInfo)
        {
            return null;
        }

        public static Bitmap CreateClearedCastrImage(CastrImageInfo imageInfo)
        {
            return null;
        }

        public static Image CreateQRCodeBitmap(string text, int maxHeight)
        {
            CodeQrBarcodeDraw qr = BarcodeDrawFactory.CodeQr;
            return qr.Draw(text, maxHeight);
        }

        /// <summary>
        /// Maximizes the font size to fix the supplied text into the supplied width.
        /// </summary>
        /// <param name="fontName">the family name of the windows font</param>
        /// <param name="text">the text to fit in the desired width</param>
        /// <param name="targetImageWidth">the width of the image that the text will go in. The parameter targetWidth must be less than this parameter.</param>
        /// <param name="targetImageHeight">the height of the image that the text will go in.</param>
        /// <param name="targetWidth">The desired maximum width</param>
        /// <returns></returns>
        public static int CalculateFontSize(string fontName, string text, int targetImageWidth, int targetImageHeight, int targetWidth)
        {
            Font font = null;
            int fontSize = 60;
            while (fontSize > 10)
            {
                font = new Font(fontName, fontSize, FontStyle.Bold);
                Bitmap tmp1 = new Bitmap(targetImageWidth, targetImageHeight);
                using (Graphics graphic = Graphics.FromImage(tmp1))
                {
                    SizeF stringSize = graphic.MeasureString(text, font);
                    if (stringSize.Width > targetWidth)
                    {
                        fontSize -= 2;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return fontSize;
        }

        /// <summary>
        /// Given the supplied constraints of the image the supplied text is fit into a number of lines that are returned.
        /// </summary>
        /// <param name="fontName"></param>
        /// <param name="fontSize"></param>
        /// <param name="text"></param>
        /// <param name="targetImageWidth"></param>
        /// <param name="targetImageHeight"></param>
        /// <returns></returns>
        public static List<string> CalculateLines(string fontName, int fontSize, string text, int targetImageWidth, int targetImageHeight, out float lineHeight)
        {
            List<string> lines = new List<string>();
            Font font = new Font(fontName, fontSize, FontStyle.Bold);
            Bitmap tmp1 = new Bitmap(targetImageWidth, targetImageHeight);
            bool allLinesFit = false;
            using (Graphics graphic = Graphics.FromImage(tmp1))
            {
                SizeF stringSize = graphic.MeasureString(text, font);
                lineHeight = stringSize.Height;
                int curLine = 2;    // for some reason this algorithm is adding an additional line that does not fit. So start at line 2 to remove this line.
                string curText = text;
                string prevText = text;
                while (curLine * lineHeight < targetImageHeight)
                {
                    bool gotALine = false;
                    if (stringSize.Width > targetImageWidth)
                    {
                        // remove one word and remeasure
                        int lastIndex = curText.LastIndexOf(' ');
                        if (lastIndex < 0)
                        {
                            // we reduced the line to only one word and that one word is really long. We need to simply fit the one word and allow it to go off the page
                            gotALine = true;
                        }
                        else
                        {
                            curText = curText.Substring(0, lastIndex);
                        }
                    }
                    else
                    {
                        gotALine = true;
                    }

                    if (gotALine)
                    {
                        lines.Add(curText);
                        string remainingText = prevText.Substring(prevText.IndexOf(curText) + curText.Length).TrimStart();
                        curText = remainingText;
                        prevText = remainingText;
                        curLine++;

                        if (string.Empty == curText)
                        {
                            // all lines are able to fit on the screen
                            allLinesFit = true;
                        }
                    }

                    stringSize = graphic.MeasureString(curText, font);
                }
            }

            if (!allLinesFit)
            {
                // remove the last word on the last line and replace it with "..."
                string lastLine = lines.Last();
                lastLine = lastLine.Substring(0, lastLine.LastIndexOf(' ')) + "...";
                lines[lines.Count() - 1] = lastLine;
            }

            return lines;
        }

        private static Graphics GetGraphic(Bitmap tmp)
        {
            Graphics graphic = Graphics.FromImage(tmp);
            graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            return graphic;
        }
    }

    public enum ImageGenSettings
    {
        Blackground = 1,
        Bold = 1,
        Barcode = 1,
        LineLeft = 1,
        LineRight = 1
    }

    public class ImageRow
    {
        public int HeightVal { get; set; }
        public bool IsLineBelow { get; set; }

        public List<ImageRowItem> RowItemList { get; set; }

        public ImageRow(int heightVal, bool isLineBelow, List<ImageRowItem> rowItemList)
        {
            this.HeightVal = heightVal >= 1 ? heightVal : 1;
            this.IsLineBelow = isLineBelow;
            this.RowItemList = rowItemList;
        }
    }

    public class ImageRowItem
    {
        public string Text { get; set; }
        public int WidthVal { get; set; }
        public bool IsBold { get; set; }
        public bool IsBarcode { get; set; }
        public bool IsBlackground { get; set; }
        public bool IsLineLeft { get; set; }
        public bool IsLineRight { get; set; }

        public ImageRow ChildImageRow { get; set; }

        public ImageRowItem(string text, int widthVal, bool isBold, bool IsBarcode, bool isBlackground, bool isLineLeft, bool isLineRight, ImageRow childImageRow)
        {
            this.Text = text;
            this.WidthVal = widthVal >= 1 ? widthVal : 1;
            this.IsBold = isBold;
            this.IsBarcode = IsBarcode;
            this.IsBlackground = isBlackground;
            this.IsLineLeft = isLineLeft;
            this.IsLineRight = isLineRight;
            this.ChildImageRow = childImageRow;
        }
    }

    [DataContract]
    public class DollyImageInfo
    {
        public DollyImageInfo(string sortPlan, string deliveryDate, string zipCodes, string dateCreated, string timeCreated, string fullACTS, string emptyACTS)
        {
            this.SortPlan = sortPlan;
            this.DeliveryDate = deliveryDate;
            this.ZipCodes = zipCodes;
            this.DateCreated = dateCreated;
            this.TimeCreated = timeCreated;
            this.FullACTS = fullACTS;
            this.EmptyACTS = emptyACTS;
        }

        [DataMember]
        public string SortPlan { get; set; }
        [DataMember]
        public string DeliveryDate { get; set; }
        [DataMember]
        public string ZipCodes { get; set; }
        [DataMember]
        public string DateCreated { get; set; }
        [DataMember]
        public string TimeCreated { get; set; }
        [DataMember]
        public string FullACTS { get; set; }
        [DataMember]
        public string EmptyACTS { get; set; }
        [DataMember]
        public string Barcode { get; set; }

        public string GetShortBarcode()
        {
            if (this.Barcode == null || this.Barcode.Equals("") || this.Barcode.Length != 19)
            {
                return "";
            }
            if (this.Barcode[11] == '0')
            {
                string shortBarcode = this.Barcode.Substring(12, 4);
                return shortBarcode;
            }
            else
            {
                string shortBarcode = this.Barcode.Substring(11, 5);
                return shortBarcode;
            }
        }
    }

    [DataContract]
    public class CastrImageInfo
    {
        public const int S5 = 30;
        public const int S3 = 20;
        public const int S1 = 10;

        public CastrImageInfo(string zipcodes, string dispatchDate, string dispatchDay, string deliveryUnit, string pass, string itcIdentity,
            string dateCreated, string timeCreated, string barcode)
        {
            this.Zipcodes = zipcodes;
            this.DispatchDate = dispatchDate;
            this.DispatchDay = dispatchDay;
            this.DeliveryUnit = deliveryUnit;
            this.Pass = pass;
            this.ItcIdentity = itcIdentity;
            this.DateCreated = dateCreated;
            this.TimeCreated = timeCreated;
            this.Barcode = barcode;
        }

        [DataMember]
        public string Zipcodes { get; set; }
        [DataMember]
        public string DispatchDate { get; set; }
        [DataMember]
        public string DispatchDay { get; set; }
        [DataMember]
        public string DeliveryUnit { get; set; }
        [DataMember]
        public string Pass { get; set; }
        [DataMember]
        public string ItcIdentity { get; set; }
        [DataMember]
        public string DateCreated { get; set; }
        [DataMember]
        public string TimeCreated { get; set; }
        [DataMember]
        public string Barcode { get; set; }
    }
}