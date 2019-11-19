using System.IO;
using System.Reflection;
using Tauron.Application.CQRS.Common;

namespace ServiceManager.Core.Core
{
    public static class Helper
    {
        //public string GetApplicationRoot()
        //{
        //    var exePath = Path.GetDirectoryName(System.Reflection
        //                                           .Assembly.GetExecutingAssembly().CodeBase);
        //    Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
        //    var appRoot = appPathMatcher.Match(exePath).Value;
        //    return appRoot;
        //}


        public static string ToApplicationPath(this string fileName)
        {
            return Path.Combine(Guard.CheckNull(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)), fileName);

            //var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            //var appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            //var appRoot = appPathMatcher.Match(exePath).Value;
            //return Path.Combine(appRoot, fileName);
        }
    }
}