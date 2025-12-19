using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VisualWorkflowBuilder.Core.Entities;
using VisualWorkflowBuilder.UiImplementation.Commands;

namespace VisualWorkflowBuilder.Presentation.ViewModels
{
    public class EditStepViewModel
    {

        public Step Step { get; }

        public ObservableCollection<KeyValueEntry> With { get; }
        public ObservableCollection<KeyValueEntry> Env { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddWithEntryCommand { get; }
        public ICommand RemoveWithEntryCommand { get; }
        public ICommand AddEnvEntryCommand { get; }
        public ICommand RemoveEnvEntryCommand { get; }

        public EditStepViewModel(Step step)
        {
            Step = step ?? throw new ArgumentNullException(nameof(step));

            With = new ObservableCollection<KeyValueEntry>(
                (Step.With ?? new System.Collections.Generic.Dictionary<string, string>())
                .Select(kv => new KeyValueEntry { Key = kv.Key, Value = kv.Value }));

            Env = new ObservableCollection<KeyValueEntry>(
                (Step.Env ?? new System.Collections.Generic.Dictionary<string, string>())
                .Select(kv => new KeyValueEntry { Key = kv.Key, Value = kv.Value }));

            SaveCommand = new RelayCommand(Save, _ => true);
            CancelCommand = new RelayCommand(Cancel, _ => true);
            AddWithEntryCommand = new RelayCommand(AddWithEntry, _ => true);
            RemoveWithEntryCommand = new RelayCommand(RemoveWithEntry, _ => true);
            AddEnvEntryCommand = new RelayCommand(AddEnvEntry, _ => true);
            RemoveEnvEntryCommand = new RelayCommand(RemoveEnvEntry, _ => true);
        }

        private void Save(object parameter)
        {
            Step.With = With
                .Where(e => !string.IsNullOrWhiteSpace(e.Key))
                .ToDictionary(e => e.Key!, e => e.Value ?? string.Empty);

            Step.Env = Env
                .Where(e => !string.IsNullOrWhiteSpace(e.Key))
                .ToDictionary(e => e.Key!, e => e.Value ?? string.Empty);

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

        private void AddWithEntry(object parameter)
        {
            With.Add(new KeyValueEntry { Key = string.Empty, Value = string.Empty });
        }

        private void RemoveWithEntry(object parameter)
        {
            if (parameter is KeyValueEntry entry && With.Contains(entry))
                With.Remove(entry);
        }

        private void AddEnvEntry(object parameter)
        {
            Env.Add(new KeyValueEntry { Key = string.Empty, Value = string.Empty });
        }

        private void RemoveEnvEntry(object parameter)
        {
            if (parameter is KeyValueEntry entry && Env.Contains(entry))
                Env.Remove(entry);
        }

        public class KeyValueEntry
        {
            public string? Key { get; set; }
            public string? Value { get; set; }
        }
    }
}
