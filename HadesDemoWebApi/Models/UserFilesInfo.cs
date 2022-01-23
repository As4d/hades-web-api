using System;

namespace HadesDemoWebApi.Models
{
    public class UserFilesInfo
    {
        public Guid UserId { get; set; }

        public string FileType { get; set; }

        public int FileCount { get; set; }
    }
}
