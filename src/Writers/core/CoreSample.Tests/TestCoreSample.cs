using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CoreSample.Tests
{
    [TestClass]
    public class TestCoreSample
    {
        [TestMethod]
        [Ignore("Expected to fail if data collector not setup")]
        public void TestEnv()
        {
            var result = Environment.GetEnvironmentVariable("CORECLR_ENABLE_PROFILING");

            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void TestMd5()
        {
            byte[] message = new byte[] { 0, 1, 2 };

            byte[] result = Program.Md5(message);

            CollectionAssert.AreEqual(
                new byte[] {
                    0xb9, 0x5f, 0x67, 0xf6, 0x1e, 0xbb, 0x03, 0x61,
                    0x96, 0x22, 0xd7, 0x98, 0xf4, 0x5f, 0xc2, 0xd3,
                },
                result
            );
        }

        [TestMethod]
        public void TestSha1()
        {
            byte[] message = new byte[] { 0, 1, 2 };

            byte[] result = Program.Sha1(message);

            CollectionAssert.AreEqual(
                new byte[] {
                    0x0c, 0x7a, 0x62, 0x3f, 0xd2, 0xbb, 0xc0, 0x5b,
                    0x06, 0x42, 0x3b, 0xe3, 0x59, 0xe4, 0x02, 0x1d,
                    0x36, 0xe7, 0x21, 0xad
                },
                result
            );
        }

        [TestMethod]
        public void TestAesCbc()
        {
            byte[] message = new byte[] { 0, 1, 2 };

            byte[] result = Program.AesCbc(message);

            CollectionAssert.AreEqual(
                new byte[] {
                    0x87, 0x88, 0x42, 0x78, 0x22, 0x01, 0x90, 0x22,
                    0x9c, 0x7d, 0x8f, 0xb3, 0xab, 0x11, 0x34, 0xc3,
                },
                result
            );
        }
    }
}
