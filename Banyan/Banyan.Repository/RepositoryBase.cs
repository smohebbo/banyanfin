using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;
using URF.Core.Abstractions.Trackable;
using URF.Core.EF.Trackable;


namespace Banyan.Repository
{
    public interface IRepositoryBase<TEntity> : ITrackableRepository<TEntity> where TEntity : class, ITrackable
    {
        // Example: adding synchronous Find, scope: application wide for all repositories
        TEntity Find(object[] keyValues, CancellationToken cancellationToken);
        //Task<List<T>> CreateQueryAsync<T>(string sql, CancellationToken cancellationToken);
    }


    public class RepositoryBase<TEntity> : TrackableRepository<TEntity>, IRepositoryBase<TEntity> where TEntity : class, ITrackable
    {
        //private readonly DbContext _urfContext;
        public RepositoryBase(DbContext urfContext) : base(urfContext)
        {
            //Console.Out.WriteLine($"RepositoryBase: {GetType().Name}");
            //_urfContext = urfContext;//Don't use urfContext when inherit. URF has Own "Context" use that
        }
        // Example: adding synchronous Find, scope: application-wide
        public TEntity Find(object[] keyValues, CancellationToken cancellationToken)
        {
            return this.Context.Find<TEntity>(keyValues) as TEntity;
        }
        /*public async Task<List<T>> CreateQueryAsync<T>(string sql, CancellationToken cancellationToken)
        {
            List<T> items = new List<T>();
            using (var connection = new SqlConnection(Context.Database.GetDbConnection().ConnectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = sql;
                connection.Open();
                System.Reflection.PropertyInfo[] properties = typeof(T).GetProperties().Where(p => p.GetMethod.IsVirtual==false).ToArray(); ;//System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = Activator.CreateInstance<T>();
                        foreach (System.Reflection.PropertyInfo property in properties)
                        {
                            try
                            {
                                if (property.PropertyType.Name == "Nullable`1")
                                    property.SetValue(item, Convert.ChangeType(reader[property.Name], Nullable.GetUnderlyingType(property.PropertyType)), null);
                                else if(property.PropertyType?.BaseType?.FullName?.Contains("URF.Core.EF.Trackable") == false)
                                    property.SetValue(item, Convert.ChangeType(reader[property.Name], property.PropertyType), null);
                            }
                            catch (Exception ex)
                            {

                            }

                        }
                        items.Add(item);
                    }
                    reader.Close();
                }
            }

            return await Task.FromResult(items);
        }*/
    }
}
