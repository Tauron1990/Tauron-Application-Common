using System.Windows.Controls;
using Tauron.Application.Views;

namespace Tauron.Application.Common.Updater.UI
{
    /// <summary>
    /// Interaktionslogik für AutoUpdaterView.xaml
    /// </summary>
    [ExportView(UpdaterConststands.UpdateView)]
    public partial class AutoUpdaterView : UserControl
    {
        public AutoUpdaterView()
        {
            InitializeComponent();
        }
    }
}
