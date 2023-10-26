using System.Reflection;
using System.Linq.Expressions;
using System;

using Foundation;
using UIKit;

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

namespace xamarin_ios
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register ("AppDelegate")]
    public class AppDelegate : UIResponder, IUIApplicationDelegate {
    
        [Export("window")]
        public UIWindow Window { get; set; }

        [Export ("application:didFinishLaunchingWithOptions:")]
        public bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
        {
            var repro = new Repro();
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method
            return true;
        }

        // UISceneSession Lifecycle

        [Export ("application:configurationForConnectingSceneSession:options:")]
        public UISceneConfiguration GetConfiguration (UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
        {
            // Called when a new scene session is being created.
            // Use this method to select a configuration to create the new scene with.
            return UISceneConfiguration.Create ("Default Configuration", connectingSceneSession.Role);
        }

        [Export ("application:didDiscardSceneSessions:")]
        public void DidDiscardSceneSessions (UIApplication application, NSSet<UISceneSession> sceneSessions)
        {
            // Called when the user discards a scene session.
            // If any sessions were discarded while the application was not running, this will be called shortly after `FinishedLaunching`.
            // Use this method to release any resources that were specific to the discarded scenes, as they will not return.
        }
    }
}


