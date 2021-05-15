using NUnit.Framework;

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
