using FluentValidation;
using SGFME.Domain.Entidades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SGFME.Domain.Interfaces
{
    public interface IBaseService<TEntity> where TEntity : BaseEntity
    {
        TModel Add<TModel, TValidator>(TModel inputModel)
            where TValidator : AbstractValidator<TEntity>
            where TModel : class;
        void Delete(long id);
        IEnumerable<TModel> Get<TModel>() where TModel : class;
        TModel GetById<TModel>(long id) where TModel : class;
        TModel Update<TModel, TValidator>(TModel inputModel)
            where TValidator : AbstractValidator<TEntity>
            where TModel : class;

        public IEnumerable<TModel> GetFiltro<TModel>(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        string includeProperties = null,
        long? take = null)
        where TModel : class;
    }
}
