using NADLLI.GUI.Utilities;
using NADLLI.GUI.Utilities.Interfaces;
using NADLLI.GUI.ViewModels.Abstractions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NADLLI.GUI.ViewModels
{
    internal class PIDViewModel : BaseViewModel
    {
        [DllImport("NADLLI.Injector.dll")]
        static extern bool Inject(int pid, string dll);

        public IAsyncCommand RefreshAsyncCommand { get; }
        public IAsyncCommand ChooseDllAsyncCommand { get; }
        public IAsyncCommand InjectAsyncCommand { get; }

        public PIDViewModel()
        {
            PIDs = PIDs;
            RefreshAsyncCommand = new AsyncCommand(ExecuteRefreshAsyncCommand);
            ChooseDllAsyncCommand = new AsyncCommand(ExecuteChooseDllAsyncCommand);
            InjectAsyncCommand = new AsyncCommand(ExecuteInjectAsyncCommand, CanExecuteInjectAsyncCommand);
        }

        string search;
        public string Search
        {
            get => search;
            set
            {
                search = value;
                OnPropertyChanged();
                PIDs = PIDs;
            }
        }

        ObservableCollection<Process> pids;
        public ObservableCollection<Process> PIDs
        {
            get => pids;
            set
            {
                var tmp = new ObservableCollection<Process>();
                Process[] processes;
                if (string.IsNullOrWhiteSpace(Search))
                    processes = Process.GetProcesses();
                else
                    processes = Process.GetProcesses().Where(x => x.ProcessName.ToLower().Contains(Search.ToLower())).ToArray();
                foreach (var process in processes)
                    tmp.Add(process);
                pids = tmp;
                OnPropertyChanged();
            }
        }

        Process selectedPID;
        public Process SelectedPID
        {
            get => selectedPID;
            set
            {
                selectedPID = value;
                InjectAsyncCommand.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        string name = "Choose DLL";
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        string dll;
        public string DLL
        {
            get => dll;
            set
            {
                dll = value;
                InjectAsyncCommand.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        Task ExecuteRefreshAsyncCommand()
        {
            PIDs = PIDs;
            return Task.CompletedTask;
        }

        Task ExecuteChooseDllAsyncCommand()
        {
            var ofd = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "library/executable (*.dll, *.exe)|*.dll; *.exe",
                FilterIndex = 1,
                Title = "choose a .dll or .exe"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Name = ofd.SafeFileName;
                DLL = ofd.FileName;
            }
            return Task.CompletedTask;
        }

        bool CanExecuteInjectAsyncCommand() => SelectedPID != null && !string.IsNullOrWhiteSpace(DLL);

        Task ExecuteInjectAsyncCommand()
        {
            Inject(SelectedPID.Id, DLL);
            return Task.CompletedTask;
        }
    }
}
