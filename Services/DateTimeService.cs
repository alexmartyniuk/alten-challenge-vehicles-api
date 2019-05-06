using System;

namespace VehiclesAPI.Services
{
    public class DateTimeService
    {
        public virtual DateTime Now
        {
            get
            {
                return DateTime.UtcNow;
            }
        }
    }
}
