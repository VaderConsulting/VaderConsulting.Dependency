using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    public class Disk
    {
        private int _TotalAvailableGB = 0;
        private int _TotalFreeGB = 0;
        private int _TotalUsedGB = 0;
        private int _TotalAllocatedGB = 0;
        private string _Name = "";
        private string _StorageName = "";
        private TimeSpan _RestoreTime = TimeSpan.Zero;

        public int TotalAvailableGB
        {
            get
            {
                return _TotalAvailableGB;
            }
            set
            {
                _TotalAvailableGB = value;
            }
        }

        public int TotalFreeGB
        {
            get
            {
                return _TotalFreeGB;
            }
            set
            {
                _TotalFreeGB = value;
            }
        }

        public int TotalUsedGB
        {
            get
            {
                return _TotalUsedGB;
            }
            set
            {
                _TotalUsedGB = value;
            }
        }

        public int TotalAllocatedGB
        {
            get
            {
                return _TotalAllocatedGB;
            }
            set
            {
                _TotalAllocatedGB = value;
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public string StorageName
        {
            get
            {
                return _StorageName;
            }
            set
            {
                _StorageName = value;
            }
        }

        public TimeSpan RestoreTime
        {
            get
            {
                return _RestoreTime;
            }
        }

        public void CalculateRestoreTime(TimeSpan Time1, TimeSpan Time2, int Speed1, int Speed2, TimeSpan Interval, Dependency.Global.AvailabilityPredictionMethod Method)
        {
            TimeSpan Result = new TimeSpan();

            // Get the per-disk times
            if (Method == Global.AvailabilityPredictionMethod.Time)
                Result += Time1;

            if (Method == Global.AvailabilityPredictionMethod.TimeAndStorage)
                Result += Time2;

            // Now get the per-storage times
            switch (Method)
            {
                case Global.AvailabilityPredictionMethod.Storage:
                    // Rates are stored as MB/minute
                    double Time3 = (double)(_TotalAllocatedGB * 1024) / Speed1; // minutes and decimal seconds
                    Result += TimeSpan.FromMinutes(Time3);
                    break;
                case Global.AvailabilityPredictionMethod.TimeAndStorage:
                    // Rates are stored as MB/minute
                    double Time4 = (double)(_TotalAllocatedGB * 1024) / Speed2; // minutes and decimal seconds
                    Result += TimeSpan.FromMinutes(Time4);
                    break;
            }

            // The final result is now...
            // Time:            Time1 + Interval
            // TimeAndStorage:  Time2 + Time4 + Interval
            // Storage:         Time3 + Interval

            _RestoreTime = Result + Interval;
        }
    }
}
