using DAM.Models.Salt_MH;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DAMTest
{
    [TestClass]
    public class MH_GM_test
    {
        private MH_GM mh_gm = new MH_GM();
        [TestMethod]
        public void MH_Test()
        {
            // Arrange
            string input = "Admin";

            // Act
            string result = mh_gm.MH(input);

            // Assert

            Assert.AreEqual("133 77 221 82 223 ", result);
        }
        [TestMethod]
        public void GM_Test()
        {
            // Arrange
            string input = "133 77 221 82 223 ";

            // Act
            string result = mh_gm.GM(input);

            // Assert

            Assert.AreEqual("Admin", result);
        }
    }
}
