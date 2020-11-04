using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;

namespace CodingSkill
{
  
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Subject> Subjects { get; set; }
    }

    public class Subject //: BaseEntity
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public double Score { get; set; }
    }

    public class ExpressionPractise
    {
       public  static string[] companies = { "Consolidated Messenger", "Alpine Ski House", "Southridge Video", "City Power & Light",
                       "Coho Winery", "Wide World Importers", "Graphic Design Institute", "Adventure Works",
                       "Humongous Insurance", "Woodgrove Bank", "Margie's Travel", "Northwind Traders",
                       "Blue Yonder Airlines", "Trey Research", "The Phone Company",
                       "Wingtip Toys", "Lucerne Publishing", "Fourth Coffee" };


        public static void Test1()
        {
            ParameterExpression para1 = Expression.Parameter(typeof(int), "a");
            ParameterExpression para2 = Expression.Parameter(typeof(int), "b");
            BinaryExpression binary = Expression.Add(para1, para2);
            ConstantExpression para3 = Expression.Constant(2, typeof(int));
            BinaryExpression multyBinary = Expression.Multiply(binary, para3);
            LambdaExpression lambda = Expression.Lambda<Func<int, int, int>>(multyBinary, para1, para2);

            Console.WriteLine(lambda.ToString());
        }

        public static string TrueOrFalse(object obj)
        {
            return (obj == null).ToString(); ;
        }

        public static Func<T, string> IfElseConditon<T>()
        {
            ParameterExpression console = Expression.Parameter(typeof(Console));

            ParameterExpression paraExp = Expression.Parameter(typeof(T), "x");
            //ConstantExpression isNull = Expression.Constant(null, typeof(object));
            //BinaryExpression equalExp = Expression.Equal(paraExp, isNull);
            //MethodCallExpression isNullCall = Expression.Call(null,typeof(Console).GetMethod("WriteLine",new Type[] { typeof(string)}), Expression.Constant("null"));
            //MethodCallExpression isNotNullCall = Expression.Call(null, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }), Expression.Constant("Not null"));
            MethodCallExpression body = Expression.Call(null, typeof(ExpressionPractise).GetMethod("TrueOrFalse"), paraExp);
            //ConditionalExpression body = Expression.IfThenElse(equalExp, isNullCall, isNotNullCall);
            var lamdba = Expression.Lambda<Func<T, string>>(body, paraExp);

            Console.WriteLine(lamdba.ToString());

            return lamdba.Compile();
        }
        public static Func<T, object> GetFuncByPropertyName<T>(string name)
        {
            var property = typeof(T).GetProperties().Where(s => s.Name == name).FirstOrDefault();
            MethodInfo methodInfo = property.GetGetMethod();
            ParameterExpression paraExp = Expression.Parameter(typeof(T));
            MethodCallExpression methodCall = Expression.Call(paraExp, methodInfo);
            UnaryExpression objectCall = Expression.Convert(methodCall, typeof(object));
            LambdaExpression lambda = Expression.Lambda(objectCall, paraExp);
            return (Func<T, object>)lambda.Compile();
        }


        //companies.Where(company => (company.ToLower() == "coho winery" || company.Length > 16))
        //.OrderBy(company => company)
        public static IEnumerable<string> WhereClause()
        {
            Expression<Func<string, bool>> exp = company => (company.ToLower() == "coho winery" || company.Length > 16);
            Expression body = Expression.Quote(exp);
             
            IQueryable<string> queryData = companies.AsQueryable();
            ParameterExpression parameterCompany = Expression.Parameter(typeof(string), "company");
            ConstantExpression constantExp = Expression.Constant("coho winery");
            MethodCallExpression toLowerCall = Expression.Call(parameterCompany, typeof(string)
                                               .GetMethod("ToLower", Type.EmptyTypes));
            BinaryExpression equalExp = Expression.Equal(constantExp, toLowerCall);

            ConstantExpression length = Expression.Constant(16, typeof(int));
            MemberExpression lengthCall = Expression.Property(parameterCompany, typeof(string).GetProperty("Length"));
            BinaryExpression greaterThanCall = Expression.GreaterThan(lengthCall, length);

            BinaryExpression whereSetence = Expression.Or(equalExp, greaterThanCall);

            var t = Expression.Lambda<Func<string, bool>>(whereSetence, new ParameterExpression[] { parameterCompany });

            MethodCallExpression whereCall = Expression.Call(typeof(Queryable), "Where", new Type[] { queryData.ElementType },
                                             queryData.Expression,
                                             body);//Expression.Lambda<Func<string, bool>>(whereSetence, new ParameterExpression[] { parameterCompany }));

            Expression<Func<string, string>> orderByExp = s => s;
            Expression orderByBody = Expression.Quote(orderByExp);
            MethodCallExpression orderByCall = Expression.Call(typeof(Queryable), "Orderby", new Type[] { queryData.ElementType, typeof(string) },
                                               whereCall,
                                               orderByBody);//Expression.Lambda<Func<string, string>>(parameterCompany, new ParameterExpression[] { parameterCompany }));
            //{System.String[].Any(s => s.Contains("ab"))}
            var result = queryData.Provider.CreateQuery<string>(whereCall);
            // IEnumerable<string> companyName = result(queryData);

            return Enumerable.Empty<string>();
        }



    }
}
