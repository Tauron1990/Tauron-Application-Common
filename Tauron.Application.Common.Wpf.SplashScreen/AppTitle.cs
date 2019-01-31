namespace Tauron.Application.Common.Wpf.SplashScreen
{
    public class AppTitle
    {
        public string First { get; set; }

        public char Middle { get; set; }

        public string Last { get; set; }

        public AppTitle(string first, char middle, string last)
        {
            First = first;
            Middle = middle;
            Last = last;
        }

        public AppTitle()
        {
            
        }
    }
}