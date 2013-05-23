using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Aaron.Core.Data
{
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Inserts the specified obj.
        /// </summary>
        /// <param name="obj">The object to insert.</param>
        void Insert(T obj);

        /// <summary>
        /// Updates the specified obj.
        /// </summary>
        /// <param name="obj">The object to delete.</param>
        void Update(T obj);

        /// <summary>
        /// Deletes the specified id.
        /// </summary>
        /// <param name="id">The identity of object to delete.</param>
        void Delete(object id);

        /// <summary>
        /// Deletes the specified obj.
        /// </summary>
        /// <param name="obj">The object to delete.</param>
        void Delete(T obj);

        /// <summary>
        /// Deletes the specified data.
        /// </summary>
        /// <param name="data">The data is a specified list from database.</param>
        void Delete(IQueryable<T> data);

        /// <summary>
        /// Gets the specified filter.
        /// </summary>
        /// <param name="filter">The filter for specified list.</param>
        /// <param name="skip">The skip of specified row.</param>
        /// <param name="take">The records want be taken.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties to join many tables.</param>
        /// <returns></returns>
        IQueryable<T> Get(Expression<Func<T, bool>> filter = null,
            int skip = 0,
            int take = 0,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null);

        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        T GetById(object id);

        /// <summary>
        /// Gets the table.
        /// </summary>
        IQueryable<T> Table { get; }
        
    }
}
