using System;
using System.Data;
using System.Data.SqlClient;
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
    }
}
