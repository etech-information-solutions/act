using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

public static class MvcHelper
{
    private static List<Type> GetSubClasses<T>()
    {
        return Assembly.Load( "ACT.UI" )
                       .GetTypes()
                       .Where( t => t.IsSubclassOf( typeof( T ) ) )
                       .ToList();
    }

    public static List<string> GetControllerNames()
    {
        List<string> controllerNames = new List<string>();

        GetSubClasses<Controller>().ForEach( t => controllerNames.Add( t.Name ) );

        return controllerNames;
    }
}
