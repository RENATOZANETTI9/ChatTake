using NUnit.Framework;

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
