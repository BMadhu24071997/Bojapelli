using Madhu.Models;
using System.Runtime.CompilerServices;

namespace Madhu.Services
{
    public interface IUserService
    {
        MyUsers GetUser(string username);
        void UpdateUserBalance(string username, int newBalance);
    }
}
