using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace DataAccess
{
    public class SqlRepository : IRepository
    {
        private readonly IConfiguration _configuration;

        public SqlRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // may use in future
        public int GetTotalNumberOfUsers()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var sqlCommand = new SqlCommand("SELECT COUNT(UserId) FROM [User] WITH (nolock)", connection);
            sqlCommand.Connection.Open();
            var noOfUsers = (int)sqlCommand.ExecuteScalar();

            return noOfUsers;
        }

        // may use in future
        public int GetTotalNumberOfScans()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            
            var sqlCommand = new SqlCommand("SELECT SUM(NumberOfScans) FROM [User] WITH (nolock)", connection);
            sqlCommand.Connection.Open();
            var noOfScans = (int)sqlCommand.ExecuteScalar();

            return noOfScans;
        }

        public bool CreateUser(UserDto user)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var cmdText = "INSERT INTO [User] (UserId, IpAddress, TotalFileCount, LastScan, OperatingSystem, NumberOfScans)" +
                              $" VALUES (@userId, @ipAddress, @totalFileCount, getdate(), @operatingSystem, @numberOfScans)";
                var sqlCommand = new SqlCommand(cmdText, connection);

                sqlCommand.Parameters.AddWithValue("@ipAddress", user.IpAddress);
                sqlCommand.Parameters.AddWithValue("@totalFileCount", user.TotalFileCount);
                sqlCommand.Parameters.AddWithValue("@operatingSystem", user.OperatingSystem);
                sqlCommand.Parameters.AddWithValue("@numberOfScans", user.NoOfScans);
                sqlCommand.Parameters.Add("@userId", SqlDbType.UniqueIdentifier, 16).Value = user.UserId;

                sqlCommand.Connection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Connection.Close();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int UpdateUser(UserDto user)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var cmdText = "UPDATE [User] SET IpAddress = @ipAddress," +
                              "TotalFileCount = @totalFileCount," +
                              "LastScan = getdate()," +
                              "OperatingSystem = @operatingSystem," +
                              "NumberOfScans = @numberOfScans" +
                              " Where UserId = @userId";

                var sqlCommand = new SqlCommand(cmdText, connection);

                sqlCommand.Parameters.AddWithValue("@ipAddress", user.IpAddress);
                sqlCommand.Parameters.AddWithValue("@totalFileCount", user.TotalFileCount);
                sqlCommand.Parameters.AddWithValue("@operatingSystem", user.OperatingSystem);
                sqlCommand.Parameters.AddWithValue("@numberOfScans", user.NoOfScans);
                sqlCommand.Parameters.AddWithValue("@userId", user.UserId);

                sqlCommand.Connection.Open();
                var userUpdated = sqlCommand.ExecuteNonQuery();

                sqlCommand.Connection.Close();
                return userUpdated;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool CreateUserFilesInfo(UserFilesInfoDto userFilesInfo)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var cmdText = "INSERT INTO [UserFiles] (UserId, FileType, FileCount) VALUES (@userId, @fileType, @fileCount)";
                var sqlCommand = new SqlCommand(cmdText, connection);

                sqlCommand.Parameters.Add("@userId", SqlDbType.UniqueIdentifier, 16).Value = userFilesInfo.UserId;
                sqlCommand.Parameters.AddWithValue("@fileType", userFilesInfo.FileType);
                sqlCommand.Parameters.AddWithValue("@fileCount", userFilesInfo.FileCount);

                sqlCommand.Connection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Connection.Close();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int UpdateUserFilesInfo(UserFilesInfoDto userFilesInfo)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var cmdText = "UPDATE [UserFiles] SET FileCount = @fileCount Where UserId = @userId and FileType = @fileType";

                var sqlCommand = new SqlCommand(cmdText, connection);

                sqlCommand.Parameters.Add("@userId", SqlDbType.UniqueIdentifier, 16).Value = userFilesInfo.UserId;
                sqlCommand.Parameters.AddWithValue("@fileType", userFilesInfo.FileType);
                sqlCommand.Parameters.AddWithValue("@fileCount", userFilesInfo.FileCount);

                sqlCommand.Connection.Open();
                var noOfRowsUpdated = sqlCommand.ExecuteNonQuery();
                sqlCommand.Connection.Close();

                return noOfRowsUpdated;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public List<VulnerabilitySummary> GetVulnerabilitySummary(string osVersion)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var cmdText = @"SELECT Isnull(VulnerabilitySummary.range, 'Total') AS [ScoreRange],
       Count(*) AS [NoOfVulnerabilities],
       Cast(( Count(*) / Cast(Sum(Count(*)) OVER () AS FLOAT) * 100 ) * 2 AS DECIMAL(10, 2)) AS 'PercentageOfTotal',
       (SELECT cast(max(updateDate) as datetime) FROM CVEDetails WHERE WindowsVersion = @osVersion) AS LastUpdateDate
FROM   (SELECT CASE
                 WHEN score BETWEEN 0 AND 1 THEN '0-1'
                 WHEN score BETWEEN 1 AND 2 THEN '1-2'
                 WHEN score BETWEEN 2 AND 3 THEN '2-3'
                 WHEN score BETWEEN 3 AND 4 THEN '3-4'
                 WHEN score BETWEEN 4 AND 5 THEN '4-5'
                 WHEN score BETWEEN 5 AND 6 THEN '5-6'
                 WHEN score BETWEEN 6 AND 7 THEN '6-7'
                 WHEN score BETWEEN 7 AND 8 THEN '7-8'
                 WHEN score BETWEEN 8 AND 9 THEN '8-9'
                 WHEN score BETWEEN 9 AND 10 THEN '9-10'
                 ELSE 'total'
               END AS [range]
        FROM  CVEDetails
        WHERE WindowsVersion = @osVersion) AS VulnerabilitySummary
GROUP BY VulnerabilitySummary.range WITH rollup
ORDER BY VulnerabilitySummary.range ASC";

                var sqlCommand = new SqlCommand(cmdText, connection);

                sqlCommand.Parameters.AddWithValue("@osVersion", osVersion);

                sqlCommand.Connection.Open();
                var dataReader = sqlCommand.ExecuteReader();

                var result = new List<VulnerabilitySummary>();

                if (!dataReader.HasRows) return result;

                while (dataReader.Read())
                {
                    result.Add(new VulnerabilitySummary()
                    {
                        OsVersion = osVersion,
                        ScoreRange = (string)dataReader["ScoreRange"],
                        NoOfVulnerabilities = (int)dataReader["NoOfVulnerabilities"],
                        PercentageOfTotal = (decimal)dataReader["PercentageOfTotal"],
                        LastUpdateDate = (DateTime)dataReader["LastUpdateDate"]
                    });
                }

                return result;
            }
            catch (Exception)
            {
                return new List<VulnerabilitySummary>();
            }
        }

        public List<VulnerabilityMetric> GetVulnerabilityMetric(string osVersion)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var cmdText = @"select * from (
		SELECT top 1 'Access' as 'Name', access as [Value], count(access) as 'Total', 'Base' as 'Type'
		FROM cvedetails
		where WindowsVersion = @osVersion
		group by access
		order by count(access) desc) as a
union
	select * from (
		SELECT top 1 'Complexity' as 'Name', Complexity as [Value], count(Complexity) as 'Total', 'Base' as 'Type'
		FROM cvedetails
		where WindowsVersion = @osVersion
		group by (Complexity)
		order by count(Complexity) desc) as b
union 
	select * from (
		SELECT top 1 'Authentication' as 'Name', [Authentication] as [Value], count([Authentication]) as 'Total', 'Base' as 'Type'
		FROM cvedetails
		where WindowsVersion = @osVersion
		group by [Authentication]
		order by count([Authentication]) desc) as c
union
		select * from (
		SELECT top 1 'Confidentiality' as 'Name', Confidentiality as [Value], count(Confidentiality) as 'Total', 'Impact' as 'Type'
		FROM cvedetails
		where WindowsVersion = @osVersion
		group by Confidentiality
		order by count(Confidentiality) desc) as d
union
		select * from (
		SELECT top 1 'Integrity' as 'Name', Integrity as [Value], count(Integrity) as 'Total', 'Impact' as 'Type'
		FROM cvedetails
		where WindowsVersion = @osVersion
		group by (Integrity)
		order by count(Integrity) desc) as e
union 
		select * from (
		SELECT top 1 'Availability' as 'Name', [Availability] as [Value], count([Availability]) as 'Total', 'Impact' as 'Type'
		FROM cvedetails
		where WindowsVersion = @osVersion
		group by [Availability]
		order by count([Availability]) desc) as f";

                var sqlCommand = new SqlCommand(cmdText, connection);

                sqlCommand.Parameters.AddWithValue("@osVersion", osVersion);

                sqlCommand.Connection.Open();
                var dataReader = sqlCommand.ExecuteReader();

                var result = new List<VulnerabilityMetric>();

                if (!dataReader.HasRows) return result;

                while (dataReader.Read())
                {
                    result.Add(new VulnerabilityMetric
                    {
                        Name = (string)dataReader["Name"],
                        Value = (string)dataReader["Value"],
                        Total = (int)dataReader["Total"],
                        Type = (string)dataReader["Type"]
                    });
                }

                return result;
            }
            catch (Exception)
            {
                return new List<VulnerabilityMetric>();
            }
        }
    }
}
