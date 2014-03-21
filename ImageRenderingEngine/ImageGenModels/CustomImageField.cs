using BarcodeLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace ImageGenModels
{
    [DataContract(Namespace = "")]
    public class CustomImageField
    {
        [DataMember]
        public int BitmapHeight { get; set; }

        [DataMember]
        public int BitmapWidth { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public TYPE BarcodeType { get; set; }

        [DataMember]
        public ControlType CtrlType { get; set; }

        [DataMember]
        public float Height { get; set; }

        [DataMember]
        public float Width { get; set; }

        [DataMember]
        public float X { get; set; }

        [DataMember]
        public float Y { get; set; }

        [DataMember]
        public string FontFamily { get; set; }

        [DataMember]
        public float FontSize { get; set; }

        [DataMember]
        public FontType FontStyle { get; set; }

        [DataMember]
        public string StrokeColor { get; set; }

        [DataMember]
        public float StrokeThickness { get; set; }

        [DataMember]
        public float RotationAngle { get; set; }

        //For Line

        [DataMember]
        public float X1 { get; set; }

        [DataMember]
        public float Y1 { get; set; }

        [DataMember]
        public float X2 { get; set; }

        [DataMember]
        public float Y2 { get; set; }

        [DataMember]
        public string DataSourceName { get; set; }


        [DataMember]
        public int DiagramWidth { get; set; }


        [DataMember]
        public int DiagramHeight { get; set; }
    }

    public enum FontType
    {
        Regular, Bold
    }

    public enum ControlType
    {
        Barcode,
        TextBlock,
        Image,
        Rectangle,
        Line,
        Default
    }
}
