using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodingSkill
{
 //   Foo foo = new Foo();
 //   foo.AddEvent();
 //           Register<Foo>.Registe(foo, Log);
 //           foo.RaiseEvents();

 //           Console.ReadLine();


 //public static void Log(object sender, EventArgs e)
 //   {
 //       Console.WriteLine("{0}: {1}", sender.GetType().Name, e.GetType().Name);
 //   }
    //对于两个具有相同声明的两个Delegate（A和B），如果B的所有输入（输入参数）类是A的子类或者类型相同，
    //而A的输出（返回值、输出参数）类型是B的子类或者类型相同，那么在B能够使用的地方A也能够使用。
    //我们在定义泛型Delegate的时候可以利用C#“协变”与“逆变”，使类型为A对象能够赋值给类型为B的变量。
    //具体来说，我们需要将输出定义为协变体（通过out关键字），而将输入定义为逆变体（通过in关键字）

    public delegate void EventHandler<in TEventArgs>(object sender, TEventArgs arg) where TEventArgs : EventArgs;
    public class Register<T>
    {
        public static void Registe(T target,EventHandler<EventArgs> handler)
        {
            foreach (EventInfo item in typeof(T).GetEvents())
            {
                item.AddEventHandler(target, handler);
            }
        }
    }

    public class BarEventArgs : EventArgs
    { }
    public class BazEventArgs : EventArgs
    { }
    public class QuxEventArgs : EventArgs
    { }

    public class Foo
    {
        public Foo()
        {

        }

        public void AddEvent()
        {
            Bar += new EventHandler<BarEventArgs>(Bar1);
            Baz += new EventHandler<BazEventArgs>(Baz1);
            Qux += new EventHandler<QuxEventArgs>(Qux1);
        }

        public event EventHandler<BarEventArgs> Bar;
        public event EventHandler<BazEventArgs> Baz;
        public event EventHandler<QuxEventArgs> Qux;

        public void RaiseEvents()
        {
            if (null != Bar) Bar(this, new BarEventArgs());
            if (null != Baz) Baz(this, new BazEventArgs());
            if (null != Qux) Qux(this, new QuxEventArgs());
        }

        public static void Bar1(object sender, BarEventArgs e)
        {
            Console.WriteLine("Bar1");
        }
        public static void Baz1(object sender, BazEventArgs e)
        {
            Console.WriteLine("Baz1");
        }
        public static void Qux1(object sender, QuxEventArgs e)
        {
            Console.WriteLine("Qux1");
        }
    }


    //run entry
    //static void Main(string[] args)
    //{
    //    Foo foo = new Foo();
    //    Register<Foo>.Registe(foo, Log);
    //    foo.RaiseEvents();

    //    Console.ReadLine();
    //}

    //public static void Log(object sender, EventArgs e)
    //{
    //    Console.WriteLine("{0}: {1}", sender.GetType().Name, e.GetType().Name);
    //}
}
