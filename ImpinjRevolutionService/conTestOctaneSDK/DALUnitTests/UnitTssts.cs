using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ReaderConfigDALProto;

namespace DALUnitTests
{
    [TestFixture]
    public class UnitTssts
    {
        [Test]
        public void TestCreateConfigEntities()
        {
            try
            {
                ImpinjReadersConfigurationEntities entities = new ImpinjReadersConfigurationEntities();


            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

    }
}
