using LinqToDB.Expressions;
using LinqToDB.Linq;
using LinqToDB;
using System.Linq.Expressions;
using System.Reflection;

namespace TableRepo.linq2db
{
    internal static class Linq2dbExtension
    {
        public static async Task<int> AddAsync<T>(this ITable<T> source, object entity,
            CancellationToken cancellation = default)
        {
            return await source.DataContext.InsertAsync<T>((T)entity,
                null, null, null, null, TableOptions.NotSet, cancellation);
        }

        public static async Task<int> ModifyAsync<T>(this ITable<T> source, object entity,
            CancellationToken cancellation = default)
        {
            return await source.DataContext.UpdateAsync<T>((T)entity,
                null, null, null, null, TableOptions.NotSet, cancellation);
        }

        private static readonly MethodInfo SetMethodInfo = MemberHelper
            .MethodOf<object>(o => ((IUpdatable<object>)null).Set(null, 0))
            .GetGenericMethodDefinition();

        private static readonly MethodInfo SqlPropertyMethodInfo = typeof(Sql).GetMethod("Property")
            .GetGenericMethodDefinition();

        public enum FieldSource
        {
            Propety,
            Column
        }

        public static Func<ParameterExpression, KeyValuePair<string, object>, Expression> GetFieldFunc(FieldSource fieldSource)
        {
            switch (fieldSource)
            {
                case FieldSource.Propety:
                    return GetPropertyExpression;
                case FieldSource.Column:
                    return GetColumnExpression;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fieldSource), fieldSource, null);
            }
        }

        public static IQueryable<T> FilterByValues<T>(this IQueryable<T> source,
            IEnumerable<KeyValuePair<string, object>> values,
            Func<ParameterExpression, KeyValuePair<string, object>, Expression> fieldFunc)
        {
            var param = Expression.Parameter(typeof(T));

            foreach (var pair in values)
            {
                var fieldExpression = fieldFunc(param, pair);
                if (fieldExpression != null)
                {
                    var equality = Expression.Equal(fieldExpression, Expression.Constant(pair.Value, fieldExpression.Type));
                    var lambda = Expression.Lambda<Func<T, bool>>(equality, param);
                    source = source.Where(lambda);
                }
            }

            return source;
        }

        public static IQueryable<T> FilterByValues<T>(this IQueryable<T> source,
            IEnumerable<KeyValuePair<string, object>> values,
            FieldSource fieldSource = FieldSource.Propety)
        {
            return FilterByValues(source, values, GetFieldFunc(fieldSource));
        }

        public static IUpdatable<T> SetValues<T>(this IUpdatable<T> source,
            IEnumerable<KeyValuePair<string, object>> values,
            Func<ParameterExpression, KeyValuePair<string, object>, Expression> fieldFunc)
        {
            var param = Expression.Parameter(typeof(T));
            object current = source;
            foreach (var pair in values)
            {
                var fieldExpression = fieldFunc(param, pair);
                if (fieldExpression != null)
                {
                    var lambda = Expression.Lambda(fieldExpression, param);

                    var method = SetMethodInfo.MakeGenericMethod(typeof(T), fieldExpression.Type);
                    current = method.Invoke(null, new[] { current, lambda, pair.Value });
                }
            }

            return (IUpdatable<T>)current;
        }

        public static IUpdatable<T> SetValues<T>(this IQueryable<T> source,
            IEnumerable<KeyValuePair<string, object>> values,
            FieldSource fieldSource = FieldSource.Propety)
        {
            return source.AsUpdatable().SetValues(values, fieldSource);
        }

        public static IUpdatable<T> SetValues<T>(this IUpdatable<T> source,
            IEnumerable<KeyValuePair<string, object>> values,
            FieldSource fieldSource = FieldSource.Propety)
        {
            return SetValues(source, values, GetFieldFunc(fieldSource));
        }

        public static int UpdateDynamic<T>(this IQueryable<T> source,
            IEnumerable<KeyValuePair<string, object>> filterValues,
            IEnumerable<KeyValuePair<string, object>> setValues,
            FieldSource fieldSource = FieldSource.Propety)
        {
            return source
                .FilterByValues(filterValues, fieldSource)
                .SetValues(setValues, fieldSource)
                .Update();
        }

        public static Expression GetPropertyExpression(ParameterExpression instance, KeyValuePair<string, object> pair)
        {
            var propInfo = instance.Type.GetProperty(pair.Key);
            if (propInfo == null)
                return null;

            var propExpression = Expression.MakeMemberAccess(instance, propInfo);

            return propExpression;
        }

        public static Expression GetColumnExpression(ParameterExpression instance, KeyValuePair<string, object> pair)
        {
            var valueType = pair.Value != null ? pair.Value.GetType() : typeof(string);

            var method = SqlPropertyMethodInfo.MakeGenericMethod(valueType);
            var sqlPropertyCall = Expression.Call(null, method, instance, Expression.Constant(pair.Key, typeof(string)));
            var memberInfo = MemberHelper.GetMemberInfo(sqlPropertyCall);
            var memberAccess = Expression.MakeMemberAccess(instance, memberInfo);

            return memberAccess;
        }
    }
}
