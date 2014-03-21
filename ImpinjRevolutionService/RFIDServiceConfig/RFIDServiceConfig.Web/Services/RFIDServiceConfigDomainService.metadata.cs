
namespace RFIDServiceConfig.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // The MetadataTypeAttribute identifies AntennaMetadata as the class
    // that carries additional metadata for the Antenna class.
    [MetadataTypeAttribute(typeof(Antenna.AntennaMetadata))]
    public partial class Antenna
    {

        // This class allows you to attach custom attributes to properties
        // of the Antenna class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class AntennaMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private AntennaMetadata()
            {
            }

            public long FK_ID_Reader { get; set; }

            public long ID { get; set; }

            public Nullable<bool> IsDefault { get; set; }

            public long Port { get; set; }

            public Reader Reader { get; set; }

            public long TxPowerIndBm { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies ConfigItemMetadata as the class
    // that carries additional metadata for the ConfigItem class.
    [MetadataTypeAttribute(typeof(ConfigItem.ConfigItemMetadata))]
    public partial class ConfigItem
    {

        // This class allows you to attach custom attributes to properties
        // of the ConfigItem class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ConfigItemMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ConfigItemMetadata()
            {
            }

            [Include]
            public ConfigItemGroup ConfigItemGroup { get; set; }

            [Include]
            public ConfigItemType ConfigItemType { get; set; }

            public long FK_ID_CONFIG_ITEM_GROUP { get; set; }

            public long FK_ID_CONFIG_ITEM_TYPE { get; set; }

            public long FK_ID_Reader { get; set; }

            public long ID { get; set; }

            public string Name { get; set; }

            public Reader Reader { get; set; }

            public string Value { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies ConfigItemGroupMetadata as the class
    // that carries additional metadata for the ConfigItemGroup class.
    [MetadataTypeAttribute(typeof(ConfigItemGroup.ConfigItemGroupMetadata))]
    public partial class ConfigItemGroup
    {

        // This class allows you to attach custom attributes to properties
        // of the ConfigItemGroup class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ConfigItemGroupMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ConfigItemGroupMetadata()
            {
            }

            public EntityCollection<ConfigItem> ConfigItems { get; set; }

            public string Description { get; set; }

            public long ID { get; set; }

            public string Name { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies ConfigItemTypeMetadata as the class
    // that carries additional metadata for the ConfigItemType class.
    [MetadataTypeAttribute(typeof(ConfigItemType.ConfigItemTypeMetadata))]
    public partial class ConfigItemType
    {

        // This class allows you to attach custom attributes to properties
        // of the ConfigItemType class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ConfigItemTypeMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ConfigItemTypeMetadata()
            {
            }

            public EntityCollection<ConfigItem> ConfigItems { get; set; }

            public string Description { get; set; }

            public long ID { get; set; }

            public string Name { get; set; }

            public string RegEx { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies ReaderMetadata as the class
    // that carries additional metadata for the Reader class.
    [MetadataTypeAttribute(typeof(Reader.ReaderMetadata))]
    public partial class Reader
    {

        // This class allows you to attach custom attributes to properties
        // of the Reader class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ReaderMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ReaderMetadata()
            {
            }

            [Include]
            public EntityCollection<Antenna> Antennae { get; set; }

            [Include]
            public EntityCollection<ConfigItem> ConfigItems { get; set; }

            public string CurrentStatus { get; set; }

            public string HostName { get; set; }

            public long ID { get; set; }

            public Nullable<bool> IsDefault { get; set; }

            public Nullable<DateTime> LastPing { get; set; }

            public string ReaderID { get; set; }
        }
    }
}
