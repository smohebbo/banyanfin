
using Banyan.Entities.Models;
using Microsoft.EntityFrameworkCore;


namespace Banyan.Repository.Models
{
    public interface IJobGradeRepository : IRepositoryBase<JobGrade>
    {

    }
    public class JobGradeRepository : RepositoryBase<JobGrade>, IJobGradeRepository
    {
        private readonly DbContext _context;

        public JobGradeRepository(DbContext context) : base(context)
        {
            this._context = context;
        }

    }
}