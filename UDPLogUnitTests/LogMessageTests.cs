using Microsoft.VisualStudio.TestTools.UnitTesting;
using UPDLog;

namespace UDPLogUnitTests
{
    [TestClass]
    public class LogMessageTests
    {
        [TestMethod]
        public void TestLogMessageParsing()
        {
            var message = "<11>1 2015-06-13T14:44:30.0430-7:00 TS-JH2.dev.local AVSMProxySvc.exe 1192 - " +
                "[AventuraMetaData@41909 file=\"MessageClientImpl.cpp\" thread=\"1364\" line=\"456\" project=\"ThreadPool\" " +
                "SocketId=\"000000001BB29A40\"] Closing socket";
            var lm = new LogMessage(message);

            //Verify Header Data
            Assert.IsTrue(lm.HostName == "TS-JH2.dev.local");
            Assert.IsTrue(lm.Process == "AVSMProxySvc.exe");
            Assert.IsTrue(lm.Pid == "1192");

            //Verify Metadata
            Assert.IsTrue(lm.Thread == "1364");
            Assert.IsTrue(lm.Project == "ThreadPool");
            Assert.IsTrue(lm.SocketId == "000000001BB29A40");
            Assert.IsTrue(lm.File == "MessageClientImpl.cpp(456)");

            //Verify Message
            Assert.IsTrue(lm.Message == "Closing socket");
        }
    }
}
