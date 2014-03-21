using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ConfigDatabaseDAL;

namespace ConfigDatabaseDALTests
{
    [TestFixture]
    public class BasicTests
    {
        [Test]
        public void InitialConnectionTest()
        {
            using (ConfigDatabaseEntities context = ConfigDatabaseContext.GetContext())
            {
                int configGroupCount = context.ConfigItemGroup.Count();
            }
        }
    }
}
