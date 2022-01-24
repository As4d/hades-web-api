using System;

// Data transfer object

namespace DataAccess
{
    public class UserFilesInfoDto
    {
        public Guid UserId { get; set; }

        public string FileType { get; set; }

        public int FileCount { get; set; }
    }
}