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
    public class ImageData
    {
        [Required]
        [DataMember(IsRequired = true)]
        public List<ImageField> Data { get; set; }

        [Required]
        [DataMember(IsRequired = true)]
        public CoralTypes CoralType { get; set; }

        [Required]
        [DataMember(IsRequired = true)]
        public int TemplateNum { get; set; }
    }

    [DataContract(Namespace = "")]
    public class ImageField
    {
        [DataMember]
        public string Field { get; set; }

        [DataMember]
        public BarcodeType BarcodeType { get; set; }
    }

    [DataContract(Namespace = "")]
    public enum BarcodeType : int
    {
        [EnumMember]
        NONE = 0,

        [EnumMember]
        ONE_D = 1,

        [EnumMember]
        TWO_D = 2,

        [EnumMember]
        Unknown = 3
    }

    [DataContract(Namespace = "")]
    public class BarcodeList
    {
        [DataMember]
        public List<int> BarcodeTypes { get; set; }

        public List<BarcodeType> GetBarcodeTypes()
        {
            List<BarcodeType> types = new List<BarcodeType>();

            foreach (int type in BarcodeTypes)
            {
                types.Add((BarcodeType)type);
            }

            return types;
        }

        public void SetBarcodeTypes(IEnumerable<BarcodeType> barcodeTypes)
        {
            List<int> types = new List<int>();

            foreach (BarcodeType type in barcodeTypes)
            {
                types.Add((int)type);
            }

            BarcodeTypes = types;
        }
    }

    /// <summary>
    /// The possible types of corals.
    /// </summary>
    public enum CoralTypes
    {
        /// <summary>
        /// The EPOP50 coral type
        /// </summary>
        EPOP50 = 0,

        /// <summary>
        /// The EPOP55 coral type
        /// </summary>
        EPOP55 = 1,

        /// <summary>
        /// The EPOP500 coral type
        /// </summary>
        EPOP500 = 3,

        /// <summary>
        /// The EPOP900 coral type
        /// </summary>
        EPOP900 = 4,

        P3 = 5,

        E2 = 6,

        P4 = 7,

        /// <summary>
        /// The coral type is not known
        /// </summary>
        Unknown = 10
    }

}
