namespace Tauron.Application.Shell
{
	public abstract class ShellApplication  : WpfApplication
	{
	    protected ShellApplication()
            : base(true)
	    {
	        CatalogList = "Catalogs.xaml";
	        CommandBinder.AutoRegister = true;
	    }
	}
}