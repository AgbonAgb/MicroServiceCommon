using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Play.Common;//.CataLog.Service.Entities;

namespace Play.Common.MongoDB
{

    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        //private const string collectionName = "items";//this stand for group of items in MobgoDb
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;
        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            //var monGoClient = new MongoClient("mongodb://localhost:27017");
            //var database = monGoClient.GetDatabase("CataLog");
            dbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();//return all db Objects
        }
        //Filter Added
        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).ToListAsync();//return all db Objects
        }
        public async Task<T> GetAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();//return all db Objects
        }
        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
             return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await dbCollection.InsertOneAsync(entity);

        }

        public async Task UpdateDb(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            FilterDefinition<T> filter = filterBuilder.Eq(Existingentity => Existingentity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }

    }
}