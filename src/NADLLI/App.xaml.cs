using NADLLI.Extensions;
using NADLLI.GUI.Views;
using System.Threading.Tasks;
using System.Windows;

namespace NADLLI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    internal partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            await Launch();
            base.OnStartup(e);
        }

        async Task Launch()
        {
            await CheckFiles();
            var pidView = new PIDView();
            pidView.Show();
        }

        Task CheckFiles()
        {
            "NADLLI.Injector.dll".CheckFile(NADLLI.Properties.Resources.NADLLI_Injector);
            return Task.CompletedTask;
        }
    }
}
