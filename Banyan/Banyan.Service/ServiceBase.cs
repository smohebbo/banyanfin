using Banyan.Repository;
using System;
using System.Threading;
using TrackableEntities.Common.Core;
using URF.Core.Abstractions.Services;
using URF.Core.Services;

namespace Hoxro.Service
{
    public interface IServiceBase<TEntity> : IService<TEntity>, IRepositoryBase<TEntity> where TEntity : class, ITrackable
    {
        //System.Threading.Tasks.Task<System.Collections.Generic.List<T>> CreateQueryAsync<T>(string sql, CancellationToken cancellationToken);
        //TEntity Find(object[] keyValues, CancellationToken cancellationToken);
    }
    public class ServiceBase<TEntity> : Service<TEntity>, IServiceBase<TEntity> where TEntity : class, ITrackable
    {
        private readonly IRepositoryBase<TEntity> repository;

        protected ServiceBase(IRepositoryBase<TEntity> repository) : base(repository)
        {
            //Console.Out.WriteLine($"ServiceBase: {GetType().Name}");
            this.repository = repository;
        }

        /*public TEntity Find(object[] keyValues, CancellationToken cancellationToken)
        {
            return this.repository.Find(keyValues, cancellationToken);
        }*/
        /*public async System.Threading.Tasks.Task<System.Collections.Generic.List<T>> CreateQueryAsync<T>(string sql, CancellationToken cancellationToken)
        {
            return await this.repository.CreateQueryAsync<T>(sql, cancellationToken);
        }*/
    }
}
