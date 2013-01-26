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

    }
}
