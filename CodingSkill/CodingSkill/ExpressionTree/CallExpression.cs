using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodingSkill.ExpressionTree
{
    public class CallExpression
    {
        public static void WhereCall()
        {
            IQueryable<char> dataQuery = "abcdefg".AsQueryable();
            Expression<Func<char,bool>> p = c => c == 97;

            Expression[] arguments = new Expression[] { dataQuery.Expression, Expression.Quote(p) };
            MethodInfo mInfo = GetMethodInfo(
                                    new Func<IQueryable<char>, Expression<Func<char, bool>>, IQueryable<char>>(Queryable.Where<char>)
                                    ,dataQuery,p);

            Expression call = Expression.Call(null, mInfo, arguments);
        }

        public static void IntersectWithComparer()
        {

        }

        public static void Intersect()
        {
            IQueryable<int> q1Query = new int[] { 1, 2, 3, 4, 57 }.AsQueryable();
            IEnumerable<int> q2Data = new int[] { 1, 2, 5, 6, 0 };
            Expression q2Exp = q2Data.AsQueryable().Expression;
            Expression[] arguments = new Expression[] { q1Query.Expression, q2Exp };
            ParameterExpression parameter1 = Expression.Parameter(typeof(IQueryable<int>));
            ParameterExpression parameter2 = Expression.Parameter(typeof(IEnumerable<int>));
            MethodInfo mInfo = GetMethodInfo(
                                    new Func<IQueryable<int>, IEnumerable<int>, IQueryable<int>>(Queryable.Intersect<int>));

            MethodInfo mInfo2 = typeof(Queryable).GetMethods().Where(m => m.Name == "Intersect" && m.GetParameters().Length == 2).First();

            Type[] genericParams = mInfo2.GetType().GetGenericArguments();
            Type[] genericParams1 = mInfo.GetType().GetGenericArguments();
            Type t = q1Query.ElementType;

            //Type[] typeParas = mInfo2.GetParameters().Select(s => s.ParameterType).ToArray();
            MethodCallExpression intersectCall = Expression.Call(typeof(Queryable), "Intersect",new Type[] { q1Query.ElementType },
                             arguments);

            Expression call = Expression.Call(null, mInfo, arguments);

            var compile = (Func<IQueryable<int>, IEnumerable<int>, IQueryable<int>>)Expression.Lambda(intersectCall, parameter1, parameter2).Compile();
            Func<IQueryable<int>,IEnumerable<int>,IQueryable<int>> lambda = (Func<IQueryable<int>, IEnumerable<int>, IQueryable<int>>)Expression.Lambda(call, parameter1, parameter2).Compile();

            var result2 = compile(q1Query, q2Data);
            var result1 = lambda(q1Query, q2Data);
        }

        private static MethodInfo GetMethodInfo<T1, T2, T3>(Func<T1, T2, T3> f)
        {
            return f.GetMethodInfo();
        }
        private static MethodInfo GetMethodInfo<T1, T2, T3>(Func<T1, T2, T3> f, T1 t1, T2 t2)
        {
            return f.GetMethodInfo();
        }
    }
}
