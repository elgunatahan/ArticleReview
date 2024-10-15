using AuthApi.Documents;
using MongoDB.Driver;

namespace AuthApi.Repositories
{
    public interface IUserRepository
    {
        public Task<User> GetUserByUsernameAsync(string username);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;

        public UserRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("AuthDB");
            _collection = database.GetCollection<User>("User");
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _collection.Find(user => user.Username == username).FirstOrDefaultAsync();
        }
    }
}
