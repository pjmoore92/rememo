using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rememo
{
    public class DiaryConfig
    {
        private int weeks;
        private DateTime startDate;
        private String name;
        private TimeSpan morningTime;
        private TimeSpan afternoonTime;
        private TimeSpan eveningTime;
        private TimeSpan specialTime;

        public int Weeks
        {
            get { return weeks; }
            set { weeks = value; }
        }

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public TimeSpan MorningTime
        {
            get { return morningTime; }
            set { morningTime = value; }
        }

        public TimeSpan AfternoonTime
        {
            get { return afternoonTime; }
            set { afternoonTime = value; }
        }

        public TimeSpan EveningTime
        {
            get { return eveningTime; }
            set { eveningTime = value; }
        }

        public TimeSpan SpecialTime
        {
            get { return specialTime; }
            set { specialTime = value; }
        }

    }
}
