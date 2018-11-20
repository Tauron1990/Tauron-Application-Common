using System.Windows;
using JetBrains.Annotations;
using Tauron.Application.Help.View;
using Tauron.Application.Help.ViewModels;

namespace Tauron.Application.Help
{
    [PublicAPI]
    public static class HelpViewFactory
    {
        private static Window _windowRef;

        [NotNull]
        public static Window GetHelpView() => new HelpView();

        [NotNull]
        public static HelpViewModel GetHelpViewModel([NotNull] string filePath, [CanBeNull] string topic, [CanBeNull] string group)
        {
            var temp = new HelpViewModel(Argument.NotNull(filePath, nameof(filePath)));
            temp.Activate(topic, group);
            return temp;
        }

        public static void ShowHelp([NotNull] string filePath, [CanBeNull] string topic, [CanBeNull] string group)
        {
            var window = _windowRef;

            window?.Activate();

            window = GetHelpView();
            window.Closed += (sender, e) => _windowRef = null;
            window.DataContext = GetHelpViewModel(Argument.NotNull(filePath, nameof(filePath)), topic, group);
            window.Show();
            _windowRef = window;
        }

        public static void ShowHelpLock([NotNull] string filePath, [CanBeNull] string topic, [CanBeNull] string group)
        {
            var window = GetHelpView();
            window.DataContext = GetHelpViewModel(Argument.NotNull(filePath, nameof(filePath)), topic, group);
            window.ShowDialog();
        }
    }
}