using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Projector.TestDomain.DDD.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace Projector.TestDomain.DDD.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DateRangeTests
    {
        [TestMethod]
        public void DetectsADateTimeInRange_ReturnsTrue_NoException()
        {
            DateTime testDate = DateTime.Now;
            DateTime startDate = testDate.AddMonths(months: -1);
            DateTime endDate = testDate.AddMonths(months: 1);

            DateRange range = new DateRange(startDate, endDate);

            var expected = true;
            var actual = range.Includes(testDate);

            Assert.IsTrue(actual == expected, message: "The date is in the defined range, should have returned true");
        }

        [TestMethod]
        public void DetectsADateTimeNotInRange_ReturnsFalse_NoException()
        {
            DateTime testDate = DateTime.Now;
            DateTime startDate = testDate.AddMonths(months: 1);
            DateTime endDate = testDate.AddMonths(months: 2);

            DateRange range = new DateRange(startDate, endDate);

            var expected = false;
            var actual = range.Includes(testDate);

            Assert.IsTrue(actual == expected, message: "The date is not in the defined range, should have returned false");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),"You cannot be allowed to define an end date earlier than the start date")]
        public void StartDateHaveToBeEarlierThanEndDate_ArgumentException()
        {
            DateTime startDate = DateTime.Now.AddMonths(months: 2);
            DateTime endDate = DateTime.Now.AddMonths(months: 1);

            DateRange range = new DateRange(startDate, endDate);
        }

        [TestMethod]
        public void DetectsARangeIsContainedWithinAnotherRange_ReturnsTrue_NoException()
        {
            DateTime startDate1 = DateTime.Now;
            DateTime endDate1 = DateTime.Now.AddMonths(months: 1);

            DateTime startDate2 = DateTime.Now.AddDays(value: 7);
            DateTime endDate2 = DateTime.Now.AddDays(value: 14);

            DateRange range1 = new DateRange(startDate1, endDate1);
            DateRange range2 = new DateRange(startDate2, endDate2);

            var expected = true;
            var actual = range1.Includes(range2);

            Assert.IsTrue(actual == expected, message: "The range is fully contained in the defined range, should have returned true");
        }

        [TestMethod]
        public void DetectsARangeStartDateIsNotContainedWithinAnotherRange_ReturnsFalse_NoException()
        {
            DateTime startDate1 = DateTime.Now;
            DateTime endDate1 = DateTime.Now.AddMonths(months: 1);

            DateTime startDate2 = DateTime.Now.AddDays(value: -7);
            DateTime endDate2 = DateTime.Now.AddDays(value: 2);

            DateRange range1 = new DateRange(startDate1, endDate1);
            DateRange range2 = new DateRange(startDate2, endDate2);

            var expected = false;
            var actual = range1.Includes(range2);

            Assert.IsTrue(actual == expected, message: "The range Start Date is not contained in the defined range, should have returned false");
        }

        [TestMethod]
        public void DetectsARangeEndDateIsNotContainedWithinAnotherRange_ReturnsFalse_NoException()
        {
            DateTime startDate1 = DateTime.Now;
            DateTime endDate1 = DateTime.Now.AddMonths(months: 1);

            DateTime startDate2 = DateTime.Now.AddDays(value: 7);
            DateTime endDate2 = DateTime.Now.AddMonths(months: 2);

            DateRange range1 = new DateRange(startDate1, endDate1);
            DateRange range2 = new DateRange(startDate2, endDate2);

            var expected = false;
            var actual = range1.Includes(range2);

            Assert.IsTrue(actual == expected, message: "The range End Date is not contained in the defined range, should have returned false");
        }
    }
}
