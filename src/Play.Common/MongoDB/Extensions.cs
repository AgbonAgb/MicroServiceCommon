using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common;//.CataLog.Service.Entities;
using Play.Common.MongoDB;//.CataLog.Service.Repository;
using Play.Common.Settings;//.CataLog.Service.Settings;
using MongoDB.Bson;

namespace Play.Common.MongoDB
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)//extend IServiceCollection
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
            //get values of serviceName and others


            //get values for MongoDb Client
            services.AddSingleton(serviceprovider =>
            {
                var _configuration = serviceprovider.GetService<IConfiguration>();//request configuration service
                var _serviceSettings = _configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                //builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
                var mongoDbSettings = _configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();//Class and Appsettings
                                                                                                                // var mongoDbSettings = services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDbSettings"));
                                                                                                                //var monGoClient = new MongoClient("mongodb://localhost:27017");
                var monGoClient = new MongoClient(mongoDbSettings.conectionstrings);
                return monGoClient.GetDatabase(_serviceSettings.ServiceName);//Servicename=catalog
                // "ServiceName":"CataLog"

            });

            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName)
         where T : IEntity
        {
            services.AddSingleton<IRepository<T>>(serviceprovider =>
            {
                var database = serviceprovider.GetService<IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName);//pass items into the constructors
            });

            return services;
        }
    }
}