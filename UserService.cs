namespace Example.Services
{
    public class UserService : IUserService
    {
        private IAuthenticationService<int> _authenticationService;
        private IDataProvider _dataProvider = null;

        public UserService(IAuthenticationService<int> authSerice, IDataProvider dataProvider)
        {
            _authenticationService = authSerice;
            _dataProvider = dataProvider;
        }

        public void ConfirmAcct(Guid token)
        {
            string procName = "[dbo].[Users_ComfirmedUpdater]";
           
            _dataProvider.ExecuteNonQuery(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Guid", token);
            }
           );
           return;
        }        

        public int Create(UserAddRequest model)
        {
            string procName = "[dbo].[Users_Insert]";
            int id = 0;

            _dataProvider.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col);
                
                col.AddWithValue("@Password", getHash(model.Password));
                
                SqlParameter IdOut = new SqlParameter("@Id", SqlDbType.Int);
                IdOut.Direction = ParameterDirection.Output;

                col.Add(IdOut);
            }, returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@Id"].Value;
                int.TryParse(oId.ToString(), out id);
            }
            );
            return id;
        }

        private static void AddCommonParams(UserAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@Email", model.Email);
            col.AddWithValue("@RoleId", model.RoleId);
            col.AddWithValue("@IsConfirmed", model.IsConfirmed);
        }

        private string getHash(string password)
        {
            string salt = BCrypt.BCryptHelper.GenerateSalt();
            string hashedPassword = BCrypt.BCryptHelper.HashPassword(password, salt);

            return hashedPassword;
        }

        public int ForgotPassword(string email)
        {
            string procName = "[dbo].[Users_SelectByEmail]";
            int id = 0;


            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
              
                col.AddWithValue("@Email", email);
               
            }, delegate (IDataReader reader, short set)
            {
                id = reader.GetSafeInt32(0);
            }
            );

            return id;
        }
        
        public int ResetPassword(UpdatePassword model)
        {

            string procName = "[dbo].[Users_ResetPassword]";
            int id = 0;

            _dataProvider.ExecuteNonQuery(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Guid", model.Token);
                paramCollection.AddWithValue("@Password", getHash(model.Password));
            }
            );
            return id;
        }
    }
}
