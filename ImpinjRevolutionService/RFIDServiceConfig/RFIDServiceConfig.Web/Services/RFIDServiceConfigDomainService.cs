
namespace RFIDServiceConfig.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.EntityFramework;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using RFIDServiceConfig.Web.Models;


    // Implements application logic using the RFIDServiceConfigEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public partial class RFIDServiceConfigDomainService : LinqToEntitiesDomainService<RFIDServiceConfigEntities>
    {

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'Antennae' query.
        public IQueryable<Antenna> GetAntennae()
        {
            return this.ObjectContext.Antennae;
        }

        public void InsertAntenna(Antenna antenna)
        {
            if ((antenna.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(antenna, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Antennae.AddObject(antenna);
            }
        }

        public void UpdateAntenna(Antenna currentAntenna)
        {
            this.ObjectContext.Antennae.AttachAsModified(currentAntenna, this.ChangeSet.GetOriginal(currentAntenna));
        }

        public void DeleteAntenna(Antenna antenna)
        {
            if ((antenna.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(antenna, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.Antennae.Attach(antenna);
                this.ObjectContext.Antennae.DeleteObject(antenna);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'ConfigItems' query.
        public IQueryable<ConfigItem> GetConfigItems()
        {
            return this.ObjectContext.ConfigItems;
        }

        public void InsertConfigItem(ConfigItem configItem)
        {
            if ((configItem.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(configItem, EntityState.Added);
            }
            else
            {
                this.ObjectContext.ConfigItems.AddObject(configItem);
            }
        }

        public void UpdateConfigItem(ConfigItem currentConfigItem)
        {
            this.ObjectContext.ConfigItems.AttachAsModified(currentConfigItem, this.ChangeSet.GetOriginal(currentConfigItem));
        }

        public void DeleteConfigItem(ConfigItem configItem)
        {
            if ((configItem.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(configItem, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.ConfigItems.Attach(configItem);
                this.ObjectContext.ConfigItems.DeleteObject(configItem);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'ConfigItemGroups' query.
        public IQueryable<ConfigItemGroup> GetConfigItemGroups()
        {
            return this.ObjectContext.ConfigItemGroups;
        }

        public void InsertConfigItemGroup(ConfigItemGroup configItemGroup)
        {
            if ((configItemGroup.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(configItemGroup, EntityState.Added);
            }
            else
            {
                this.ObjectContext.ConfigItemGroups.AddObject(configItemGroup);
            }
        }

        public void UpdateConfigItemGroup(ConfigItemGroup currentConfigItemGroup)
        {
            this.ObjectContext.ConfigItemGroups.AttachAsModified(currentConfigItemGroup, this.ChangeSet.GetOriginal(currentConfigItemGroup));
        }

        public void DeleteConfigItemGroup(ConfigItemGroup configItemGroup)
        {
            if ((configItemGroup.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(configItemGroup, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.ConfigItemGroups.Attach(configItemGroup);
                this.ObjectContext.ConfigItemGroups.DeleteObject(configItemGroup);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'ConfigItemTypes' query.
        public IQueryable<ConfigItemType> GetConfigItemTypes()
        {
            return this.ObjectContext.ConfigItemTypes;
        }

        public void InsertConfigItemType(ConfigItemType configItemType)
        {
            if ((configItemType.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(configItemType, EntityState.Added);
            }
            else
            {
                this.ObjectContext.ConfigItemTypes.AddObject(configItemType);
            }
        }

        public void UpdateConfigItemType(ConfigItemType currentConfigItemType)
        {
            this.ObjectContext.ConfigItemTypes.AttachAsModified(currentConfigItemType, this.ChangeSet.GetOriginal(currentConfigItemType));
        }

        public void DeleteConfigItemType(ConfigItemType configItemType)
        {
            if ((configItemType.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(configItemType, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.ConfigItemTypes.Attach(configItemType);
                this.ObjectContext.ConfigItemTypes.DeleteObject(configItemType);
            }
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'Readers' query.
        public IQueryable<Reader> GetReaders()
        {
            return this.ObjectContext.Readers;
        }

        public void InsertReader(Reader reader)
        {
            if ((reader.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(reader, EntityState.Added);
            }
            else
            {
                this.ObjectContext.Readers.AddObject(reader);
            }
        }

        public void UpdateReader(Reader currentReader)
        {
            this.ObjectContext.Readers.AttachAsModified(currentReader, this.ChangeSet.GetOriginal(currentReader));
        }

        public void DeleteReader(Reader reader)
        {
            if ((reader.EntityState != EntityState.Detached))
            {
                this.ObjectContext.ObjectStateManager.ChangeObjectState(reader, EntityState.Deleted);
            }
            else
            {
                this.ObjectContext.Readers.Attach(reader);
                this.ObjectContext.Readers.DeleteObject(reader);
            }
        }
    }
}


