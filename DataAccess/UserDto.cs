using System;

namespace DataAccess
{
    public class UserDto
    {
        public Guid UserId { get; set; }

        public string IpAddress { get; set; }

        public int TotalFileCount { get; set; }

        public DateTime LastScan { get; set; }

        public string OperatingSystem { get; set; }

        public int NoOfScans { get; set; }
    }
}
