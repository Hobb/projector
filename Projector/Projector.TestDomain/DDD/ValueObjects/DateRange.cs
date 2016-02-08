using Projector.TestDomain.DDD.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.TestDomain.DDD.ValueObjects
{
    public struct DateRange : IRange<DateTime>
    {
        public DateRange(DateTime start, DateTime end)
            : this()
        {
            if (start > end)
            {
                throw new ArgumentException(message: "Start date cannot be later than end date");
            }

            this.Start = start;
            this.End = end;
        }

        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public bool Includes(DateTime value)
        {
            return (Start <= value) && (value <= End);
        }

        public bool Includes(IRange<DateTime> range)
        {
            return (Start <= range.Start) && (range.End <= End);
        }
    }
}
