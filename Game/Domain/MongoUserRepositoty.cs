using System;
using System.Linq;
using MongoDB.Driver;

namespace Game.Domain
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> userCollection;
        public const string CollectionName = "users";

        public MongoUserRepository(IMongoDatabase database)
        {
            userCollection = database.GetCollection<UserEntity>(CollectionName);

            userCollection.Indexes.CreateOne(new CreateIndexModel<UserEntity>(
                Builders<UserEntity>.IndexKeys.Ascending(u => u.Login),
                new CreateIndexOptions { Unique = true }));
        }

        public UserEntity Insert(UserEntity user)
        {
            userCollection.InsertOne(user);
            return user;
        }

        public UserEntity FindById(Guid id)
        {
            var res = userCollection.Find(user => user.Id == id).FirstOrDefault();
            return res;
        }

        public UserEntity GetOrCreateByLogin(string login)
        {
            //TODO: Это Find или Insert
            var find = userCollection.Find(user => user.Login == login).SingleOrDefault();

            if (find == default(UserEntity))
            {
                var createdUser = new UserEntity
                {
                    Login = login
                };
                userCollection.InsertOne(createdUser);
                return createdUser;
            }
            else
                return find;
        }

        public void Update(UserEntity user)
        {
            //TODO: Ищи в документации ReplaceXXX
            userCollection.ReplaceOne(u => u.Id == user.Id, user);           
        }

        public void Delete(Guid id)
        {
            userCollection.DeleteOne(user => user.Id == id);
        }

        // Для вывода списка всех пользователей (упорядоченных по логину)
        // страницы нумеруются с единицы
        public PageList<UserEntity> GetPage(int pageNumber, int pageSize)
        {
            var total = userCollection.CountDocuments(u => true);
            //TODO: Тебе понадобятся SortBy, Skip и Limit
            var users = userCollection.Find(u => true)
                .SortBy(user => user.Login)
                .Skip(pageSize * (pageNumber - 1))
                .Limit(pageSize)
                .ToList();
            return new PageList<UserEntity>(users, total, pageNumber, pageSize);
        }

        // Не нужно реализовывать этот метод
        public void UpdateOrInsert(UserEntity user, out bool isInserted)
        {
            throw new NotImplementedException();
        }
    }
}