using Invoice_Generator.Data;
using Invoice_Generator.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace Invoice_Generator.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly InvoiceDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(InvoiceDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync(); ;
        }

        public async Task<T?> GetByIdAsync(int? id)
        {
            if(id == null)
            {
                throw new ArgumentNullException(nameof(id), "ID cannot be null");
            }
            return await _dbSet.FindAsync(id);
        }

        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }


    }
}
