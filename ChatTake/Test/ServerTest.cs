using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;

namespace Server.Test
{
    public class ServerTest
    {
        [Test]
        public void ServerContructorNotReturnExcption()
        {
            Assert.DoesNotThrow(() => new Server());
        }

        [Test]
        public void RunMethodNotReturnExcption()
        {
            Assert.DoesNotThrow(() => new Server().Run());
        }

    }
}
