using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using VisualWorkflowBuilder.Core.Entities;
using VisualWorkflowBuilder.UiImplementation.Commands;

namespace VisualWorkflowBuilder.Presentation.ViewModels
{
    public class EditWorkFlowConfigSettingsViewModel
    {
        public Workflow Workflow { get; }

        public string Name { get; set; }

        public ObservableCollection<BranchEntry> PushBranches { get; }
        public ObservableCollection<BranchEntry> PullRequestBranches { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddPushBranchCommand { get; }
        public ICommand RemovePushBranchCommand { get; }
        public ICommand AddPullBranchCommand { get; }
        public ICommand RemovePullBranchCommand { get; }

        public EditWorkFlowConfigSettingsViewModel(Workflow inputWorkflow)
        {
            Workflow = inputWorkflow ?? new Workflow();

            Name = Workflow.Name ?? string.Empty;

            PushBranches = new ObservableCollection<BranchEntry>(
                (Workflow.On?.Push?.Branches ?? Array.Empty<string>())
                .Select(s => new BranchEntry { Value = s }));

            PullRequestBranches = new ObservableCollection<BranchEntry>(
                (Workflow.On?.PullRequest?.Branches ?? Array.Empty<string>())
                .Select(s => new BranchEntry { Value = s }));

            SaveCommand = new RelayCommand(Save, _ => true);
            CancelCommand = new RelayCommand(Cancel, _ => true);
            AddPushBranchCommand = new RelayCommand(AddPushBranch, _ => true);
            RemovePushBranchCommand = new RelayCommand(RemovePushBranch, _ => true);
            AddPullBranchCommand = new RelayCommand(AddPullBranch, _ => true);
            RemovePullBranchCommand = new RelayCommand(RemovePullBranch, _ => true);
        }

        private void Save(object parameter)
        {
            Workflow.Name = Name;

            if (Workflow.On == null) Workflow.On = new Triggers();

            var push = PushBranches
                .Select(be => be.Value)
                .Where(b => !string.IsNullOrWhiteSpace(b))
                .Select(b => b!.Trim())
                .ToArray();
            Workflow.On.Push = push.Length > 0 ? new BranchTrigger { Branches = push } : null;

            var pr = PullRequestBranches
                .Select(be => be.Value)
                .Where(b => !string.IsNullOrWhiteSpace(b))
                .Select(b => b!.Trim())
                .ToArray();
            Workflow.On.PullRequest = pr.Length > 0 ? new BranchTrigger { Branches = pr } : null;

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

        private void AddPushBranch(object parameter)
        {
            PushBranches.Add(new BranchEntry { Value = string.Empty });
        }

        private void RemovePushBranch(object parameter)
        {
            if (parameter is BranchEntry entry && PushBranches.Contains(entry))
                PushBranches.Remove(entry);
        }

        private void AddPullBranch(object parameter)
        {
            PullRequestBranches.Add(new BranchEntry { Value = string.Empty });
        }

        private void RemovePullBranch(object parameter)
        {
            if (parameter is BranchEntry entry && PullRequestBranches.Contains(entry))
                PullRequestBranches.Remove(entry);
        }

        
        public class BranchEntry : INotifyPropertyChanged
        {
            private string? _value;
            public string? Value
            {
                get => _value;
                set
                {
                    if (value == _value) return;
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }
    }
}
