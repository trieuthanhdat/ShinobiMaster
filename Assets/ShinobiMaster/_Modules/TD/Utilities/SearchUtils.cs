using System;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;


namespace TD.Utilities
{
    public static class SearchUtils
    {
        //Dynamic text search (case-insensitivity, partial match)
        public static IQueryable<T> SearchByText<T>
        (
            IQueryable<T> source,
            Expression<Func<T, string>> propertySelector,
            string searchTerm
        )
        {
            if(string.IsNullOrEmpty(searchTerm)) return source;
            var lowerTerm = searchTerm.ToLower();

            var parameter = propertySelector.Parameters[0];
            var propertyExpression = propertySelector.Body;
            var toLowerExpression = Expression.Call(propertyExpression, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
            var containExpression = Expression.Call(toLowerExpression, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(lowerTerm));
            var lamda = Expression.Lambda<Func<T, bool>>(containExpression, parameter);

            return source.Where(lamda);
        }
        //Dynamic search by number (Equality)
        public static IQueryable<T> SearchByNumber<T>
        (
            IQueryable<T> source,
            Expression<Func<T, int>> propertySelector,
            int? searchVaue
        )
        {
            if(!searchVaue.HasValue) return source;

            return source.Where(item => propertySelector.Compile()(item) == searchVaue.Value);
        }

        //Combine multiple search conditionas with AND or OR
        public static IQueryable<T> CombineSearchCondition<T>
        (
            IQueryable<T> source,
            Func<IQueryable<T>, IQueryable<T>> condition_a,
            Func<IQueryable<T>, IQueryable<T>> condition_b,
            bool useAnd = true
        )
        { 
            if(useAnd)
            { 
                return condition_a(source).Intersect(condition_b(source));
            }else
            {
                return condition_a(source).Union(condition_b(source));
            }
        }

        //Combine Text search and Num Search
        public static IQueryable<T> ApplySearchByTextAndNumber<T>
        (
            IQueryable<T> source,
            string textTerm,
            Expression<Func<T, string>> textSelector,
            int? numTerm,
            Expression<Func<T, int>> numberSelector,
            bool useAnd = true
        )
        {
            var textSearch = SearchByText(source, textSelector, textTerm);
            var numSearch = SearchByNumber(source, numberSelector, numTerm);

            return useAnd ? textSearch.Intersect(numSearch) : textSearch.Union(numSearch);
        }
    }

}
