namespace DataAccess
{
    public interface IRepository
    {
        public int GetTotalNumberOfUsers();

        public int GetTotalNumberOfScans();

        public bool CreateUser(UserDto user);

        public int UpdateUser(UserDto user);

        public bool CreateUserFilesInfo(UserFilesInfoDto userFilesInfo);

        public int UpdateUserFilesInfo(UserFilesInfoDto userFilesInfo);
    }
}