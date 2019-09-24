using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using tmg.equinox.domain.entities;
using tmg.equinox.repository.interfaces;
namespace tmg.equinox.repository.extensions
{
    public static partial class RepositoryQueryExtension
    {

        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains");
        private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

        #region Extension Methods for RepositoryQuery

        public static RepositoryQuery<TEntity> ApplyOrderBy<TEntity>(this RepositoryQuery<TEntity> entity, string sortPropertyName, string sortDirection) where TEntity : Entity
        {
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sorFunction = GetSortFunction<TEntity>(sortPropertyName, sortDirection);
            entity.OrderBy(sorFunction);
            return entity;
        }

        public static RepositoryQuery<TEntity> ApplySearchCriteria<TEntity>(this RepositoryQuery<TEntity> entity, SearchCriteria searchCriteria) where TEntity : Entity
        {
            Expression<Func<TEntity, bool>> searchFilter = null;
            if (searchCriteria != null)
            {
                searchFilter = BuildExpressionForEntityQuery<TEntity>(searchCriteria.rules);
            }
            return entity.Filter(searchFilter);
        }

        #endregion

        #region Extension Methods for List

        public static List<TEntity> ApplySearchCriteria<TEntity>(this List<TEntity> entity, SearchCriteria searchCriteria) where TEntity : class
        {
            Expression<Func<TEntity, bool>> searchFilter = null;
            if (searchCriteria != null)
            {
                searchFilter = BuildExpressionForClass<TEntity>(searchCriteria.rules);
                if (searchFilter != null)
                    return entity.AsQueryable().Where(searchFilter).ToList();
                else
                    return entity.ToList();
            }
            else
            {
                return entity;
            }

        }        

        public static List<TEntity> ApplyOrderBy<TEntity>(this List<TEntity> entity, string sortPropertyName, string sortDirection) where TEntity : class
        {
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sorFunction = GetSortFunction<TEntity>(sortPropertyName, sortDirection);
            return sorFunction(entity.AsQueryable()).ToList();
        }

        public static List<TEntity> GetPage<TEntity>(this List<TEntity> entity, int page, int pageSize, out int totalCount) where TEntity : class
        {
            totalCount = entity.Count();
            return entity.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        private static Expression<Func<TEntity, bool>> BuildExpressionForEntityQuery<TEntity>(IList<Filter> filters) where TEntity : Entity
        {
            if (filters.Count == 0)
                return null;
            ParameterExpression param = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "t");
            System.Linq.Expressions.Expression exp = null;

            if (filters.Count == 1)
                exp = GetExpressionForEntityQuery<TEntity>(param, filters[0]);
            else
            {
                foreach (Filter filter in filters)
                {
                    if (exp == null)
                    {
                        exp = GetExpressionForEntityQuery<TEntity>(param, filter);
                    }
                    else
                    {
                        exp = System.Linq.Expressions.Expression.AndAlso(exp, GetExpressionForEntityQuery<TEntity>(param, filter));
                    }
                }
            }
            if (exp != null)
                return System.Linq.Expressions.Expression.Lambda<Func<TEntity, bool>>(exp, param);
            return null;
        }

        private static Expression<Func<TEntity, bool>> BuildExpressionForClass<TEntity>(IList<Filter> filters) where TEntity : class
        {
            if (filters.Count == 0)
                return null;
            ParameterExpression param = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "t");
            System.Linq.Expressions.Expression exp = null;

            if (filters.Count == 1)
                exp = GetExpressionForClassQuery<TEntity>(param, filters[0]);
            else
            {
                foreach (Filter filter in filters)
                {
                    if (exp == null)
                    {
                        exp = GetExpressionForClassQuery<TEntity>(param, filter);
                    }
                    else
                    {
                        exp = System.Linq.Expressions.Expression.AndAlso(exp, GetExpressionForClassQuery<TEntity>(param, filter));
                    }
                }
            }
            if (exp != null)
                return System.Linq.Expressions.Expression.Lambda<Func<TEntity, bool>>(exp, param);
            return null;
        }

        #endregion

        #region Extension Methods for IQueryable
        public static IQueryable<TEntity> ApplyOrderBy<TEntity>(this IQueryable<TEntity> entity, string sortPropertyName, string sortDirection) where TEntity : class
        {
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sorFunction = GetSortFunction<TEntity>(sortPropertyName, sortDirection);
            return sorFunction(entity.AsQueryable());
        }

        public static IQueryable<TEntity> ApplySearchCriteria<TEntity>(this IQueryable<TEntity> entity, SearchCriteria searchCriteria) where TEntity : class
        {
            Expression<Func<TEntity, bool>> searchFilter = null;
            if (searchCriteria != null)
            {
                searchFilter = BuildExpressionForClass<TEntity>(searchCriteria.rules);
                if (searchFilter != null)
                    return entity.AsQueryable().Where(searchFilter);
                else
                    return entity.AsQueryable();
            }
            else
            {
                return entity;
            }

        }

        public static IQueryable<TEntity> GetPage<TEntity>(this IQueryable<TEntity> entity, int page, int pageSize, out int totalCount) where TEntity : class
        {
            totalCount = entity.Count();
            return entity.Skip((page - 1) * pageSize).Take(pageSize);
        }

        #endregion

        #region Helper Methods
        private static System.Linq.Expressions.Expression GetExpressionForEntityQuery<T>(ParameterExpression param, Filter filter)
        {
            try
            {
                MemberExpression member = System.Linq.Expressions.Expression.Property(param, filter.field);
                ConstantExpression constant = System.Linq.Expressions.Expression.Constant(filter.data);

                switch (filter.op)
                {
                    case "cn":
                        return System.Linq.Expressions.Expression.Call(member, containsMethod, constant);

                    //case Op.Equals:
                    //    return System.Linq.Expressions.Expression.Equal(member, constant);

                    //case Op.GreaterThan:
                    //    return System.Linq.Expressions.Expression.GreaterThan(member, constant);

                    //case Op.GreaterThanOrEqual:
                    //    return System.Linq.Expressions.Expression.GreaterThanOrEqual(member, constant);

                    //case Op.LessThan:
                    //    return System.Linq.Expressions.Expression.LessThan(member, constant);

                    //case Op.LessThanOrEqual:
                    //    return System.Linq.Expressions.Expression.LessThanOrEqual(member, constant);

                    //case Op.Contains:
                    //    return System.Linq.Expressions.Expression.Call(member, containsMethod, constant);

                    //case Op.StartsWith:
                    //    return System.Linq.Expressions.Expression.Call(member, startsWithMethod, constant);

                    //case Op.EndsWith:
                    //    return System.Linq.Expressions.Expression.Call(member, endsWithMethod, constant);
                }
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is ArgumentException)
                    return null;
                else throw ex;
            }
            return null;
        }
   
        private static System.Linq.Expressions.Expression GetExpressionForClassQuery<T>(ParameterExpression param, Filter filter)
        {
            try
            {
                MemberExpression member = System.Linq.Expressions.Expression.Property(param, filter.field);
                ConstantExpression constant = System.Linq.Expressions.Expression.Constant(string.IsNullOrEmpty(filter.data) ? "" : filter.data.ToLower());
                Expression dateFormat = Expression.Constant("MM/dd/yyyy hh:mm tt");
                Expression shortDateFormat = Expression.Constant("MM/dd/yyyy");
                ConstantExpression Blank = Expression.Constant("");
                ConstantExpression NullDate = Expression.Constant(DateTime.MinValue);
                var ToLower_Method = typeof(string).GetMethod("ToLower", System.Type.EmptyTypes);
                var ToString_Method = typeof(object).GetMethod("ToString");

                switch (filter.op)
                {
                    case "cn":
                        MethodCallExpression callExpression = null;
                        Expression leftOperandExpression = null;

                        if (member.Type.FullName.Contains("Nullable"))
                        {
                            if (member.Type.FullName.Contains("DateTime"))
                            {
                                leftOperandExpression = Expression.Condition(Expression.Property(member, "HasValue"), Expression.Call(Expression.Property(member, "Value"), "ToString", null, dateFormat), Blank);
                            }
                            else
                            {
                                leftOperandExpression = Expression.Condition(Expression.Property(member, "HasValue"), Expression.Call(member, ToString_Method), Blank);
                            }
                        }
                        else
                        {
                            //  leftOperandExpression = Expression.Call(typeof(Convert), "ToString", null, member, member.Type.FullName.Contains("DateTime") ? dateFormat : null);
                            if (member.Type.FullName.Contains("DateTime"))
                            {
                                leftOperandExpression = Expression.Condition(Expression.Equal(member, NullDate), Blank, Expression.Call(member, "ToString", null, dateFormat));
                            }
                            else if (member.Type.FullName.Contains("Boolean"))
	                            {
                                    leftOperandExpression = Expression.Condition(Expression.Equal(member, Expression.Constant(true, typeof(bool))), Expression.Constant("true"), Expression.Constant("false"));
	                            }
                            else if (member.Type.FullName.Contains("String"))
                            {
                                leftOperandExpression = Expression.Condition(Expression.Equal(member, Expression.Constant(null, typeof(object))), Blank, Expression.Call(member, ToString_Method));
                            }
                            else
                                leftOperandExpression = Expression.Call(member, ToString_Method);
                        }
                        callExpression = System.Linq.Expressions.Expression.Call(Expression.Call(leftOperandExpression, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes)), containsMethod, constant);

                        return callExpression;


                    case "eq":
                        if (member.Type.FullName.Contains("Nullable"))
                        {
                            if (member.Type.FullName.Contains("DateTime"))
                            {
                                leftOperandExpression = Expression.Condition(Expression.Property(member, "HasValue"), Expression.Call((Expression.Property(member, "Value")), "ToString", null, shortDateFormat), Blank);
                            }
                            else
                                leftOperandExpression = Expression.Condition(Expression.Property(member, "HasValue"), Expression.Call(Expression.Property(member, "Value"), ToLower_Method), Blank);
                        }
                        else
                        {
                            if (member.Type.FullName.Contains("DateTime"))
                            {
                                leftOperandExpression = Expression.Call(member, "ToString", null, shortDateFormat);
                            }
                            else if (member.Type.FullName.Contains("Boolean"))
                            {
                                leftOperandExpression = Expression.Condition(Expression.Equal(member, Expression.Constant(true, typeof(bool))), Expression.Constant("true"), Expression.Constant("false"));
                            }
                            else if (member.Type.FullName.Contains("String"))
                                leftOperandExpression = Expression.Call((Expression.Condition(Expression.Equal(member, Expression.Constant(null, typeof(object))), Blank, member)), "ToLower", null, null);
                            else
                                leftOperandExpression = Expression.Call(member, ToString_Method);

                        }

                        return System.Linq.Expressions.Expression.Equal(leftOperandExpression, constant);

                    //case Op.GreaterThan:
                    //    return System.Linq.Expressions.Expression.GreaterThan(member, constant);

                    //case Op.GreaterThanOrEqual:
                    //    return System.Linq.Expressions.Expression.GreaterThanOrEqual(member, constant);

                    //case Op.LessThan:
                    //    return System.Linq.Expressions.Expression.LessThan(member, constant);

                    //case Op.LessThanOrEqual:
                    //    return System.Linq.Expressions.Expression.LessThanOrEqual(member, constant);

                    //case Op.Contains:
                    //    return System.Linq.Expressions.Expression.Call(member, containsMethod, constant);

                    //case Op.StartsWith:
                    //    return System.Linq.Expressions.Expression.Call(member, startsWithMethod, constant);

                    //case Op.EndsWith:
                    //    return System.Linq.Expressions.Expression.Call(member, endsWithMethod, constant);                        
                }
            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is ArgumentException)
                    return null;
                else throw ex;
            }            
            return null;
        }

        private static Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> GetSortFunction<TEntity>(string sortPropertyName, string sortDirection) where TEntity : class
        {
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sorFunction = null;

            Type type = typeof(TEntity);
            ParameterExpression arg = System.Linq.Expressions.Expression.Parameter(type, "x");
            System.Linq.Expressions.Expression expr = arg;
            // use reflection (not ComponentModel) to mirror LINQ
            PropertyInfo pi = type.GetProperty(sortPropertyName);
            expr = System.Linq.Expressions.Expression.Property(expr, pi);
            type = pi.PropertyType;

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(TEntity), type);
            LambdaExpression lambda = System.Linq.Expressions.Expression.Lambda(delegateType, expr, arg);

            var orderBy = typeof(Queryable).GetMethods().First(
                    method => method.Name == "OrderBy"
                            && method.GetParameters().Count() == 2)
                    .MakeGenericMethod(typeof(TEntity), type);

            var orderByDescending = typeof(Queryable).GetMethods().First(
                    method => method.Name == "OrderByDescending"
                            && method.GetParameters().Count() == 2)
                    .MakeGenericMethod(typeof(TEntity), type);


            if (sortDirection == "asc")
                sorFunction = query => (IOrderedQueryable<TEntity>)
                      orderBy.Invoke(null, new object[] { query, lambda });
            else
                sorFunction = query => (IOrderedQueryable<TEntity>)
                  orderByDescending.Invoke(null, new object[] { query, lambda });
            return sorFunction;
        }
        #endregion
    }

    public class Filter
    {
        public string field { get; set; }
        public string op { get; set; }
        public string data { get; set; }
    }

    public class SearchCriteria
    {
        public string groupOp { get; set; }
        public List<Filter> rules { get; set; }

        public SearchCriteria()
        {
            rules = new List<Filter>();
        }
    }


    //public class Filter
    //{
    //    public string PropertyName { get; set; }
    //    public Op Operation { get; set; }

    //    public object Value { get; set; }
    //}
    //public enum Op
    //{
    //    Equals,
    //    GreaterThan,
    //    LessThan,
    //    GreaterThanOrEqual,
    //    LessThanOrEqual,
    //    Contains,
    //    StartsWith,
    //    EndsWith
    //}


}
