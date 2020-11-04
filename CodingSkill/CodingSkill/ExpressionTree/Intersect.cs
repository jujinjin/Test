using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CodingSkill.ExpressionTree
{
   public class Intersect
    {
        public static void T()
        {
            Student s = new Student()
            {
                Id = 1,
                Name = "Sam",
                Subjects = new List<Subject>() {
                new Subject() {  Id = 1, SubjectName = "Math", Score = 20},
                new Subject() {  Id =2, SubjectName = "Chinese", Score = 50}
            }
            };

            IEnumerable<Subject> subjectList = new List<Subject>()
            {
                new Subject() {  Id =2, SubjectName = "Chinese", Score = 50}
            };

            IEnumerable<Subject> subStudent = s.Subjects;

            IQueryable<Subject> subjects = s.Subjects.AsQueryable();

            //var test = subjects.Intersect(subjectList, new GenericCompare());

       // var result = subStudent.Intersect(subjectList, new GenericCompare()).ToList();

       IQueryable <Subject> subjects2 = subjectList.AsQueryable();
            // ParameterExpression paraExp = Expression.Constant("Source", typeof(IEnumerable<Subject>));
            ParameterExpression source = Expression.Parameter(typeof(IQueryable<Subject>),"source");
            ParameterExpression source2 = Expression.Parameter(typeof(IEnumerable<Subject>),"source1");
            ParameterExpression comparer = Expression.Parameter(typeof(IEqualityComparer<Subject>),"comparer");

            Expression[] paras = new Expression[] { subjects.Expression, subjects2.Expression,comparer};

            var methodInfo = typeof(Queryable).GetMethods().Where(m => m.Name == "Intersect" && m.GetParameters().Length == 2).First();


            //var methodInfo = typeof(Queryable).GetMethod("Intersect",new Type[] { typeof(Subject)});
            MethodCallExpression intersectCall = Expression.Call(typeof(Queryable), "Intersect"
                                                 , new Type[] {subjects.ElementType}
                                                 , paras);

            LambdaExpression lambdaExp = LambdaExpression.Lambda(intersectCall, source,source2,comparer);

            var func = (Func<IQueryable<Subject>, IEnumerable<Subject>, IEqualityComparer<Subject>,IQueryable<Subject>>)lambdaExp.Compile();
            Console.WriteLine(func.ToString());
            IEqualityComparer<Subject> equality = new GenericCompare();
            var result = func(subjects2, subjectList,equality);
        }
    }
}
