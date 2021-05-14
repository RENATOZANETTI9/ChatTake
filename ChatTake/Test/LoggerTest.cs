using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Test
{
    [TestFixture]
    class LoggerTest
    {

        [Test]
        public void LogFileIsNotNull()
        {
            Assert.DoesNotThrow(() => new Logger("Log.txt"));
        }

    }

}
