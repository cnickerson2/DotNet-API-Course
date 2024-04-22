using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Helpers
{
    public class ReusableSQL
    {
        private readonly DataContextDapper _dapper;

        public ReusableSQL(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        public bool UpsertUser(UserComplete user)
        {
            string sql = @"EXEC TutorialAppSchema.spUser_Upsert
                         @FirstName = @FirstNameParam,
                         @LastName = @LastNameParam, 
                         @Email = @EmailParam,
                         @Gender = @GenderParam,
                         @Active = @ActiveParam,
                         @JobTitle = @JobTitleParam,
                         @Department = @DepartmentParam,
                         @Salary = @SalaryParam,
                         @UserId = @UserIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@FirstNameParam", user.FirstName, System.Data.DbType.String);
            sqlParameters.Add("@LastNameParam", user.LastName, System.Data.DbType.String);
            sqlParameters.Add("@EmailParam", user.Email, System.Data.DbType.String);
            sqlParameters.Add("@GenderParam", user.Gender, System.Data.DbType.String);
            sqlParameters.Add("@ActiveParam", user.Active, System.Data.DbType.Boolean);
            sqlParameters.Add("@JobTitleParam", user.JobTitle, System.Data.DbType.String);
            sqlParameters.Add("@DepartmentParam", user.Department, System.Data.DbType.String);
            sqlParameters.Add("@SalaryParam", user.Salary, System.Data.DbType.Decimal);
            sqlParameters.Add("@UserIdParam", user.UserId, System.Data.DbType.Int32);

            return _dapper.ExecuteSqlWithParameters(sql, sqlParameters);
        }
    }
}
