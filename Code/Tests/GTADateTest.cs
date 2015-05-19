#region Using

using AccountInBank;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Tests
{
    [TestClass]
    public class GTADateTest
    {
        [TestMethod]
        public void DateEquals()
        {
            var date1 = new GTADate( 2015, 12, 20 );
            var date2 = new GTADate( 2015, 12, 20 );
            Assert.AreEqual( date1, date2 );
        }

        [TestMethod]
        public void DateNotEquals()
        {
            var date1 = new GTADate( 2015, 12, 20 );
            var date2 = new GTADate( 2015, 12, 21 );
            Assert.AreNotEqual( date1, date2 );
        }

        [TestMethod]
        public void LeftMoreDay()
        {
            var date1 = new GTADate( 2015, 12, 21 );
            var date2 = new GTADate( 2015, 12, 20 );
            Assert.IsTrue( date1 > date2 );
        }

        [TestMethod]
        public void LeftMoreMonth()
        {
            var date1 = new GTADate( 2015, 12, 19 );
            var date2 = new GTADate( 2015, 11, 20 );
            Assert.IsTrue( date1 > date2 );
        }

        [TestMethod]
        public void LeftMoreYear()
        {
            var date1 = new GTADate( 2016, 12, 19 );
            var date2 = new GTADate( 2015, 12, 19 );
            Assert.IsTrue( date1 > date2 );
        }

        [TestMethod]
        public void RightMoreDay()
        {
            var date2 = new GTADate( 2015, 12, 21 );
            var date1 = new GTADate( 2015, 12, 20 );
            Assert.IsTrue( date1 < date2 );
        }

        [TestMethod]
        public void RightMoreMonth()
        {
            var date2 = new GTADate( 2015, 12, 19 );
            var date1 = new GTADate( 2015, 11, 20 );
            Assert.IsTrue( date1 < date2 );
        }

        [TestMethod]
        public void RightMoreYear()
        {
            var date2 = new GTADate( 2016, 12, 19 );
            var date1 = new GTADate( 2015, 12, 19 );
            Assert.IsTrue( date1 < date2 );
        }

        [TestMethod]
        public void RightMoreEqualsDay()
        {
            var date1 = new GTADate( 2015, 12, 21 );
            var date2 = new GTADate( 2015, 12, 20 );
            var date3 = new GTADate( 2015, 12, 20 );
            var date4 = new GTADate( 2015, 12, 20 );
            Assert.IsTrue( date1 >= date2 && date3 >= date4 );
        }

        [TestMethod]
        public void RightLessEqualsDay()
        {
            var date2 = new GTADate( 2015, 12, 21 );
            var date1 = new GTADate( 2015, 12, 20 );
            var date4 = new GTADate( 2015, 12, 20 );
            var date3 = new GTADate( 2015, 12, 20 );
            Assert.IsTrue( date1 <= date2 && date3 <= date4 );
        }
    }
}