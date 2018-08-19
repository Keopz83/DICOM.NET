using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace joselima.dicom.network._tests {

    [TestClass]
    public class CEchoTests {

        [TestMethod]
        public void CEchoRequest_can_parse() {

            var echoRqFilePath = "echo_rq.dat";
            byte[] rawData = System.IO.File.ReadAllBytes(echoRqFilePath);
            var associateRequest = CEchoParser.ParseRequest(rawData);
            Assert.AreEqual("1.2.840.10008.3.1.1.1", associateRequest.ApplicationContextName);
            Assert.AreEqual("1.2.840.10008.1.1", associateRequest.PresentationContexts.First().SyntaxItems.AbstractSyntax);
            Assert.AreEqual("1.2.840.10008.1.2", associateRequest.PresentationContexts.First().SyntaxItems.TransferSyntaxes.First());
            Assert.AreEqual((UInt32)16384, associateRequest.UserInformation.MaxLength);
            //TODO: test Extended Negotiation
        }


        /// <summary>
        /// Integration test: requires 3rd party C-Echo SCU.
        /// </summary>
        [TestMethod]
        public void CEchoScp_can_receive() {

            //CEchoScp.StartListening(104, "CEchoScp");
            Assert.Fail();
        }


        /// <summary>
        /// Integration test: requires 3rd party C-Echo SCU.
        /// </summary>
        [TestMethod]
        public void CEchoScp_can_respond() {

            Assert.Fail();
        }
    }
}
