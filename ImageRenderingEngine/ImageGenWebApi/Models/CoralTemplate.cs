using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ImageGenModels;
using ImageRender;

namespace ImageGenWebApi.Models
{
    public class CoralTemplate
    {
        /// <summary>
        /// Generate image for the given elabel type.
        /// </summary>
        /// <param name="data">The information contained in the image template.</param>
        /// <param name="elabelType">The type of elabel.</param>
        /// <returns>A bitmap image that is formatted to be displayed on the supplied type of elabel.</returns>
        public static Bitmap GenerateImage(List<ImageField> data, CoralTypes coralType, int templateNum)
        {
            switch (coralType)
            {
                case CoralTypes.EPOP900:
                    return CoralImage.CreateImageForEP900(data);
                case CoralTypes.EPOP500:
                    return CoralImage.CreateImageForEP500(data);
                case CoralTypes.EPOP55:
                    return CoralImage.CreateImageForEP55(data);
                case CoralTypes.EPOP50:
                    return CoralImage.CreateImageForEP50(data);
                case CoralTypes.E2:
                    return CoralImage.CreateImageForE2(data);
                case CoralTypes.P3:
                    return CoralImage.CreateImageForP3(data);
                case CoralTypes.P4:
                    switch (templateNum)
                    {
                        case 1:
                            return CoralImage.CreateImageForBridgeStoneP4_1(data);
                        case 2:
                            return CoralImage.CreateImageForBridgeStoneP4_2(data);
                        case 3:
                            return CoralImage.CreateImageForBridgeStoneP4_3(data);
                        default:
                            return CoralImage.CreateImageForBridgeStoneP4_1(data);
                    }
            }

            return null;
        }

        /// <summary>
        /// Returns a list that can be used to determine the number of fields supported in a template for a given coral type 
        /// and the type of fields. Each element in the list corresponds to a field. A field can support either
        /// text only (the element is <see cref="ImageRender.BarcodeType.None"/>), 
        /// 1-D barcodes and text (the element is <see cref="ImageRender.BarcodeType.ONE_D"/>), 
        /// or 2-D barcodes, 1-D barcodes and text (the element is <see cref="ImageRender.BarcodeType.TWO_D"/>). 
        /// The number of fields is determined from the Count property of the returned list.
        /// </summary>
        /// <param name="coralType">the type of coral</param>
        /// <returns>A list which is meta data describing the format of data that can be send to 
        /// <see cref="Controller.BounceMan.UpdateImageByData"/> and <see cref="Controller.BounceMan.SilentImageDownloadByData"/></returns>
        public static BarcodeList GetTemplateFields(CoralTypes coralType)
        {
            IList<BarcodeType> retList = new List<BarcodeType>();
            switch (coralType)
            {
                case CoralTypes.EPOP900:
                    retList.Add(BarcodeType.TWO_D);
                    retList.Add(BarcodeType.TWO_D);
                    retList.Add(BarcodeType.TWO_D);
                    retList.Add(BarcodeType.TWO_D);
                    break;
                case CoralTypes.EPOP500:
                    retList.Add(BarcodeType.TWO_D);
                    retList.Add(BarcodeType.TWO_D);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    break;
                case CoralTypes.EPOP55:
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    break;
                case CoralTypes.EPOP50:
                    retList.Add(BarcodeType.TWO_D);
                    retList.Add(BarcodeType.TWO_D);
                    retList.Add(BarcodeType.NONE);
                    break;
                case CoralTypes.E2:
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    break;
                case CoralTypes.P3:
                    retList.Add(BarcodeType.ONE_D);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    break;
                case CoralTypes.P4:
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.ONE_D);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    retList.Add(BarcodeType.NONE);
                    break;
                default:
                    return null;
            }

            BarcodeList barcodeList = new BarcodeList();
            barcodeList.SetBarcodeTypes(retList);

            return barcodeList;
        }
    }
}
