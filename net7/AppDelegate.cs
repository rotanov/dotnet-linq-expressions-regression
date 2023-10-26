using System.Reflection;
using System.Linq.Expressions;

namespace net7;

public class Repro
{
	public Repro()
	{
		var checker1 = Repro.MakeChecker_NotCrashing(typeof(Repro), nameof(Bar));
		System.Console.WriteLine("CHECKPOINT_1");
		var checker2 = Repro.MakeChecker_Crashing(typeof(Repro).GetMethod(nameof(Foo)));
		System.Console.WriteLine("CHECKPOINT_2");
	}

	internal static Func<object, int, object, bool> MakeChecker_Crashing(MethodInfo m)
	{
		var pObj = Expression.Parameter(typeof(object));
		var pIndex = Expression.Parameter(typeof(int));
		var pItem = Expression.Parameter(typeof(object));
		var e = Expression.Call(Expression.Convert(pObj, m.DeclaringType), m, pIndex, pItem);
		return Expression.Lambda<Func<object, int, object, bool>>(
			e, pObj, pIndex, pItem).Compile();
	}

	public static Func<object, object, bool> MakeChecker_NotCrashing(Type tObj, string methodName)
	{
		var fn = tObj.GetMethod(methodName);
		var p = Expression.Parameter(typeof(object));
		var pf = Expression.Parameter(typeof(object));
		var e = Expression.Call(Expression.Convert(p, tObj), fn);
		return Expression.Lambda<Func<object, object, bool>>(e, p, pf).Compile();
	}

	public bool Foo(int index, object @object)
	{
		return true;
	}

	public bool Bar()
	{
		return true;
	}
}


[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
        var repro = new Repro();
		// create a new window instance based on the screen size
		Window = new UIWindow (UIScreen.MainScreen.Bounds);

		// create a UIViewController with a single UILabel
		var vc = new UIViewController ();
		vc.View!.AddSubview (new UILabel (Window!.Frame) {
			BackgroundColor = UIColor.SystemBackground,
			TextAlignment = UITextAlignment.Center,
			Text = "Hello, iOS!",
			AutoresizingMask = UIViewAutoresizing.All,
		});
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}
}
