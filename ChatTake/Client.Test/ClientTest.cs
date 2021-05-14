using NUnit.Framework;

namespace Client.Test
{
    public class Tests
    {
        [Test]
        public void ClientContructorNotReturnExcption()
        {
            var host = "127.0.0.1"; 
            var port = 2222; 
            Assert.DoesNotThrow(() => new Client(host,port));
        }
    }
}