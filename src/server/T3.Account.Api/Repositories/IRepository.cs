namespace T3.Account.Api.Repositories;

public interface IRepository<T>
{
    Task InsertOne(T entity);
    Task UpdateOne(T entity);
    Task DeleteOne(T entity);
}