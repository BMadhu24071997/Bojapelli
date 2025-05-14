using Madhu.Models;


namespace Madhu.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;

        public UserService(ApplicationDbContext db)
        {
            _db = db;
        }

        public MyUsers GetUser(string username)
        {
            return _db.Users.FirstOrDefault(u => u.UserName == username);
        }

        public void UpdateUserBalance(string username, int newBalance)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserName == username);
            if (user != null)
            {
                user.AccountBalance = newBalance;
                _db.SaveChanges(); // Save to the database
            }
        }
    }

}

