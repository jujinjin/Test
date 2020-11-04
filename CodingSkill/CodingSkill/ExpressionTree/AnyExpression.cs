using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodingSkill.ExpressionTree
{
    public class AnyExpression
    {
        static string[] strs = { "abc", "abf", "ert", "qwe", "abg" };
        public static void AnyTest<T>(IQueryable<T> queryable)
        {
            IQueryable<T> dataQuery = queryable;
            Expression<Func<T, bool>> func = s => true;//s.Contains("ab");
            ParameterExpression param1 = Expression.Parameter(typeof(IQueryable<T>), "s");
            ParameterExpression param2 = Expression.Parameter(typeof(Expression<Func<T, bool>>));
            Expression[] arguments = new Expression[] { dataQuery.Expression,Expression.Quote(func)};

            MethodInfo mInfo = getMethodInfo(
                              new Func<IQueryable<T>, Expression<Func<T, bool>>, bool>(Queryable.Any<T>));

            MethodCallExpression callExp = Expression.Call(null, mInfo, arguments);

            LambdaExpression lambdaExp = Expression.Lambda(callExp, param1,param2);

            var result = (Func<IQueryable<T>, Expression<Func<T, bool>>, bool>)lambdaExp.Compile();
        }

        public static void AnyTest2()
        {
            IQueryable<string> dataQuery = strs.AsQueryable();
            ParameterExpression param1 = Expression.Parameter(typeof(IQueryable<string>), "s");
            Expression<Func<string, bool>> func = s => s.Contains("ab");
            ParameterExpression param3 = Expression.Parameter(typeof(string),"s");
            ParameterExpression param4 = Expression.Parameter(typeof(bool));
            Expression param5 = Expression.Constant("ab");
            ParameterExpression param2 = Expression.Parameter(typeof(Expression<Func<string, bool>>));
            Expression body = Expression.Quote(func);

            var t = Expression.Lambda(body, new ParameterExpression[] {});
            MethodCallExpression callExp1 = Expression.Call(typeof(Queryable), "All", new[] { typeof(string) }
                            , dataQuery.Expression
                            ,body);
            
            LambdaExpression lambda2 = Expression.Lambda(callExp1, param1);         
            LambdaExpression lambdaExp = Expression.Lambda(body, param1, param2);
           var resu = dataQuery.Provider.Execute<bool>(callExp1);
            var result = (Func<IQueryable<string>,Expression<Func<string,bool>>,bool>)lambdaExp.Compile();

        }

        public static void Test3()
        {
            IQueryable<string> dataQuery = strs.AsQueryable();
            MethodInfo methodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            Expression abConstant = Expression.Constant("ab", typeof(string));
            ParameterExpression param1 = Expression.Parameter(typeof(IQueryable<string>));
            var sConstant = Expression.Parameter(typeof(string),"s");
            MethodCallExpression callExp = Expression.Call(sConstant, methodInfo, abConstant);

            var result = Expression.Lambda<Func<string, bool>>(callExp, new[] { sConstant });

            var methodCallExp = Expression.Call(typeof(Queryable), "Any", new[] { typeof(string) }
                            , dataQuery.Expression,
                            result);

            var abc = LambdaExpression.Lambda(methodCallExp, param1);
            var compile = ((Func<IQueryable<string>,bool>)abc.Compile())(dataQuery);
          // IQueryable s = dataQuery.Provider.CreateQuery<string>(methodCallExp);
        }

        private static MethodInfo getMethodInfo<T1, T2, T3>(Func<T1, T2, T3> f)
        {
            return f.GetMethodInfo();
        }
    }
}
