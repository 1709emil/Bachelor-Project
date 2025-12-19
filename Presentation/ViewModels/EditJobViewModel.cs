using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VisualWorkflowBuilder.Core.Entities;
using VisualWorkflowBuilder.Presentation.Views;
using VisualWorkflowBuilder.UiImplementation.Commands;

namespace VisualWorkflowBuilder.Presentation.ViewModels;

internal class EditJobViewModel
{
    public Job Job { get; }

    public ObservableCollection<EnvEntry> Env { get; }
    public ObservableCollection<Step> Steps { get; }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand AddEnvEntryCommand { get; }
    public ICommand RemoveEnvEntryCommand { get; }
    public ICommand AddStepCommand { get; }
    public ICommand RemoveStepCommand { get; }
    public ICommand ShowEditStepWindowCommand { get; }

    public EditJobViewModel(Job job)
    {
        Job = job ?? throw new ArgumentNullException(nameof(job));

        Env = new ObservableCollection<EnvEntry>(
            (job.Env ?? new Dictionary<string, string>())
            .Select(kv => new EnvEntry { Key = kv.Key, Value = kv.Value }));

        Steps = new ObservableCollection<Step>(job.Steps ?? new List<Step>());

        SaveCommand = new RelayCommand(Save, _ => true);
        CancelCommand = new RelayCommand(Cancel, _ => true);
        AddEnvEntryCommand = new RelayCommand(AddEnvEntry, _ => true);
        RemoveEnvEntryCommand = new RelayCommand(RemoveEnvEntry, _ => true);
        AddStepCommand = new RelayCommand(AddStep, _ => true);
        RemoveStepCommand = new RelayCommand(RemoveStep, _ => true);
        ShowEditStepWindowCommand = new RelayCommand(ShowEditStepWindow, _ => true);
    }

    private void Save(object parameter)
    {

        Job.Env = Env
            .Where(e => !string.IsNullOrWhiteSpace(e.Key))
            .ToDictionary(e => e.Key!, e => e.Value ?? string.Empty);


        Job.Steps = Steps.ToList();


        if (parameter is Window w)
        {
            w.DialogResult = true;
            w.Close();
        }
    }

    private void Cancel(object parameter)
    {
        if (parameter is Window w) w.Close();
    }

    private void AddEnvEntry(object parameter)
    {
        Env.Add(new EnvEntry { Key = string.Empty, Value = string.Empty });
    }

    private void RemoveEnvEntry(object parameter)
    {
        if (parameter is EnvEntry entry && Env.Contains(entry))
            Env.Remove(entry);
    }

    private void AddStep(object parameter)
    {
        Steps.Add(new Step { Name = "New step" });
    }

    private void RemoveStep(object parameter)
    {
        if (parameter is Step step && Steps.Contains(step))
            Steps.Remove(step);
    }

    private void ShowEditStepWindow(object parameter)
    {
        if (parameter is not Step step)
        {
            return;
        }

        var editStepWindow = new EditStepWindow
        {
            Owner = System.Windows.Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            DataContext = new EditStepViewModel(step)
        };


        editStepWindow.ShowDialog();
    }

    internal class EnvEntry
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}
