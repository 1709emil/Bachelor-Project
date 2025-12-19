using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VisualWorkflowBuilder.Application.Ports;
using VisualWorkflowBuilder.Core.Entities;
using VisualWorkflowBuilder.Presentation.ViewModels;
using VisualWorkflowBuilder.Presentation.Views;
using VisualWorkflowBuilder.UiImplementation.Commands;


namespace VisualWorkflowBuilder.UiImplementation.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private IWorkFlowConstructor WorkFlowConstructor;
    private IObjectToYamlTranslator Translator;
    private IYamlToObjectTranslator YamlToObjectTranslator;
    private IEnumerable<IBuildJobConstructor> BuildJobConstructors;
    private ILintingJobConstructor LintingJobConstructor;
    private IEnumerable<ITestingJobConstructor> TestingJobConstructors;
    private IDelpoyJobConstructor DelpoyJobConstructor;

    private Workflow _currentWorkflow = new();
    public Workflow CurrentWorkflow
    {
        get => _currentWorkflow;
        set
        {
            if (ReferenceEquals(_currentWorkflow, value)) return;
            _currentWorkflow = value;
            OnPropertyChanged(nameof(CurrentWorkflow));
        }
    }

    public ObservableCollection<JobNodeViewModel> Nodes { get; } = new();

    public ICommand AddJobToWorkspaceCommand { get; }
    public ICommand AddBuildJobMavenToWorkspaceCommand { get; }
    public ICommand AddBuildJobDotNetToWorkspaceCommand { get; }
    public ICommand AddLintingJobToWorkspaceCommand { get; }
    public ICommand AddTestingJobDotNetToWorkspaceCommand { get; }
    public ICommand AddTestingJobJUnitToWorkspaceCommand { get; }
    public ICommand AddDeployJobToWorkspaceCommand { get; }



    public ICommand RemoveJobFromWorkspaceCommand { get; }
    public ICommand ShowEditWindowCommand { get; }
    public ICommand SaveWorkflowCommand { get; }
    public ICommand LoadWorkflowCommand { get; }

    public ICommand ShowEditWorkFlowConfigSettingsWindowCommand { get; }

    private const double LeftMargin = 20.0;
    private const double TopMargin = 20.0;
    private const double HorizontalSpacing = 250.0;
    private const double VerticalSpacing = 40.0;
    private const double DefaultNodeWidth = 160.0;
    private const double DefaultNodeHeight = 100.0;

    public MainViewModel(IWorkFlowConstructor workFlowConstructor, IObjectToYamlTranslator objectToYamlTranslator,
        IYamlToObjectTranslator yamlToObjectTranslator,
        IEnumerable<IBuildJobConstructor> buildJobConstructors, ILintingJobConstructor lintingJobConstructor,
        IEnumerable<ITestingJobConstructor> testingJobConstructors, IDelpoyJobConstructor delpoyJobConstructor)
    {
        WorkFlowConstructor = workFlowConstructor;
        Translator = objectToYamlTranslator;
        YamlToObjectTranslator = yamlToObjectTranslator;
        BuildJobConstructors = buildJobConstructors;
        LintingJobConstructor = lintingJobConstructor;
        TestingJobConstructors = testingJobConstructors;
        DelpoyJobConstructor = delpoyJobConstructor;

        AddJobToWorkspaceCommand = new RelayCommand(AddJobToWorkspace, _ => true);
        AddBuildJobMavenToWorkspaceCommand = new RelayCommand(AddBuildJobMavenToWorkspace, _ => true);
        AddBuildJobDotNetToWorkspaceCommand = new RelayCommand(AddBuildJobDotNetToWorkspace, _ => true);
        AddLintingJobToWorkspaceCommand = new RelayCommand(AddLintingJobToWorkspace, _ => true);
        AddTestingJobDotNetToWorkspaceCommand = new RelayCommand(AddTestingJobDotNetToWorkspace, _ => true);
        AddTestingJobJUnitToWorkspaceCommand = new RelayCommand(AddTestingJobJUnitToWorkspace, _ => true);
        AddDeployJobToWorkspaceCommand = new RelayCommand(AddDeployJobToWorkspace, _ => true);


        ShowEditWindowCommand = new RelayCommand(ShowEditWindow, _ => true);
        SaveWorkflowCommand = new RelayCommand(SaveWorkflow, _ => true);
        LoadWorkflowCommand = new RelayCommand(LoadWorkflow, _ => true);
        ShowEditWorkFlowConfigSettingsWindowCommand = new RelayCommand(ShowEditWorkFlowConfigSettingsWindow, _ => true);
        RemoveJobFromWorkspaceCommand = new RelayCommand(RemoveJobFromWorkspace, _ => true);
    }

    private void AddJobToWorkspace(object? parameter)
    {
        Job job = new Job() { Name = $"Job {Nodes.Count + 1}" };

        EditNewNodePositions(job);

        RecalculateLayout();
    }

    private void AddBuildJobMavenToWorkspace(object? parameter)
    {
        IBuildJobConstructor? mavenConstructor = BuildJobConstructors
                                                .FirstOrDefault(c => c.GetType().Name.Contains("Maven", StringComparison.OrdinalIgnoreCase));
        if (mavenConstructor == null)
            return;

        Job job = mavenConstructor.ConstructBuildJobWithName($"Build With Maven {Nodes.Count + 1}");

        EditNewNodePositions(job);
        RecalculateLayout();
    }

    private void AddBuildJobDotNetToWorkspace(object? parameter)
    {
        IBuildJobConstructor? dotNetConstructor = BuildJobConstructors
                                                .FirstOrDefault(c => c.GetType().Name.Contains("DotNet", StringComparison.OrdinalIgnoreCase));
        if (dotNetConstructor == null)
            return;

        Job job = dotNetConstructor.ConstructBuildJobWithName($"Build With .NET {Nodes.Count + 1}");
        EditNewNodePositions(job);
        RecalculateLayout();
    }

    private void AddLintingJobToWorkspace(object? parameter)
    {
        Job job = LintingJobConstructor.ConstructLintingJobWithName($"Linting Job {Nodes.Count + 1}");
        EditNewNodePositions(job);
        RecalculateLayout();
    }
    private void AddTestingJobDotNetToWorkspace(object? parameter)
    {
        ITestingJobConstructor? testingConstructor = TestingJobConstructors
                                                .FirstOrDefault(c => c.GetType().Name.Contains("DotNet", StringComparison.OrdinalIgnoreCase));
        if (testingConstructor == null)
            return;

        Job job = testingConstructor.ConstructTestingJobWithName($"Testing With .NET Job {Nodes.Count + 1}");
        EditNewNodePositions(job);
        RecalculateLayout();
    }

    private void AddTestingJobJUnitToWorkspace(object? parameter)
    {
        ITestingJobConstructor? junitConstructor = TestingJobConstructors
                                                .FirstOrDefault(c => c.GetType().Name.Contains("JUnit", StringComparison.OrdinalIgnoreCase));
        if (junitConstructor == null)
            return;

        Job job = junitConstructor.ConstructTestingJobWithName($"Testing With JUnit Job {Nodes.Count + 1}");
        EditNewNodePositions(job);
        RecalculateLayout();
    }
    private void AddDeployJobToWorkspace(object? parameter)
    {
        Job job = DelpoyJobConstructor.ConstructDeployJobWithName($"Deploy Job {Nodes.Count + 1}");
        EditNewNodePositions(job);
        RecalculateLayout();
    }
    private void RemoveJobFromWorkspace(object? parameter)
    {
        if (parameter is not JobNodeViewModel nodeVm) return;

        var result = MessageBox.Show(
        $"Are you sure you want to remove '{nodeVm.Job.Name}'?",
        "Confirm Removal",
        MessageBoxButton.YesNo,
        MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        Nodes.Remove(nodeVm);
        RecalculateLayout();
    }


    private void ShowEditWindow(object? parameter)
    {
        if (parameter is not JobNodeViewModel nodeVm) return;
        string originalNeeds = nodeVm.Job.Needs;
        string originalName = nodeVm.Job.Name;

        EditJobWindow editWindow = new EditJobWindow
        {
            Owner = System.Windows.Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            DataContext = new EditJobViewModel(nodeVm.Job)
        };

        editWindow.ShowDialog();

        if (originalNeeds != nodeVm.Job.Needs || originalName != nodeVm.Job.Name)
        {
            RecalculateLayout();
        }
    }

    private void ShowEditWorkFlowConfigSettingsWindow(object? parameter)
    {
        EditWorkFlowConfigSettingsWindow editWindow = new EditWorkFlowConfigSettingsWindow
        {
            Owner = System.Windows.Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            DataContext = new EditWorkFlowConfigSettingsViewModel(CurrentWorkflow)
        };


        bool? result = editWindow.ShowDialog();
        if (result == true)
        {

            OnPropertyChanged(nameof(CurrentWorkflow));


        }
    }

    private void LoadWorkflow(object? parameter)
    {
        OpenFileDialog dlg = new OpenFileDialog
        {
            Title = "Open workflow file",
            Filter = "YAML files (*.yaml;*.yml)|*.yaml;*.yml|All files (*.*)|*.*",
            DefaultExt = ".yaml",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        bool? result = dlg.ShowDialog();
        if (result != true || string.IsNullOrWhiteSpace(dlg.FileName))
            return;

        try
        {
            using FileStream stream = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);
            Workflow workflow = YamlToObjectTranslator.TranslateYamlToObject(stream);

            if (workflow == null)
            {
                MessageBox.Show("Failed to load workflow: Invalid YAML format.", "Load error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            Nodes.Clear();


            CurrentWorkflow = workflow;


            if (workflow.Jobs != null)
            {
                foreach (var jobEntry in workflow.Jobs)
                {
                    Job job = jobEntry.Value;
                    JobNodeViewModel nodeVm = new JobNodeViewModel(job);
                    Nodes.Add(nodeVm);
                }
            }


            RecalculateLayout();

            MessageBox.Show("Workflow loaded successfully.", "Load", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load workflow: {ex.Message}", "Load error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SaveWorkflow(object? parameter)
    {

        if (!IsWorkflowConfigValid())
        {
            MessageBoxResult validationResult = MessageBox.Show(
                "Workflow configuration is incomplete. Please fill out the workflow name and triggers before generating the YAML file.\n\nWould you like to edit the workflow config settings now?",
                "Incomplete Configuration",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (validationResult == MessageBoxResult.Yes)
            {
                ShowEditWorkFlowConfigSettingsWindow(null);
            }
            return;
        }

        Workflow workflow = WorkFlowConstructor.ConstructWorkFlowWithParameters(CurrentWorkflow.Name, CurrentWorkflow.On,
            Nodes.ToDictionary(n => n.Job.Name, n => n.Job));

        SaveFileDialog dlg = new SaveFileDialog
        {
            Title = "Save workflow as",
            Filter = "YAML files (*.yaml;*.yml)|*.yaml;*.yml|All files (*.*)|*.*",
            DefaultExt = ".yaml",
            FileName = string.IsNullOrWhiteSpace(workflow?.Name) ? "workflow.yaml" : $"{workflow.Name}.yaml",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        bool? result = dlg.ShowDialog();
        if (result != true || string.IsNullOrWhiteSpace(dlg.FileName))
            return;

        try
        {
            Translator.TranslateObjectToYaml(workflow, dlg.FileName);
            MessageBox.Show("Workflow saved.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save workflow: {ex.Message}", "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool IsWorkflowConfigValid()
    {

        if (string.IsNullOrWhiteSpace(CurrentWorkflow.Name))
        {
            return false;
        }


        if (CurrentWorkflow.On == null)
        {
            return false;
        }

        return true;
    }

    public void RecalculateLayout()
    {
        var nodeByName = Nodes.ToDictionary(n => n.Job.Name);

        var topLevel = Nodes
            .Where(n => string.IsNullOrWhiteSpace(n.Job.Needs) || !nodeByName.ContainsKey(n.Job.Needs))
            .ToList();

        for (int i = 0; i < topLevel.Count; i++)
        {
            var n = topLevel[i];
            n.CanvasLeft = LeftMargin;
            n.CanvasTop = TopMargin + i * (DefaultNodeHeight + VerticalSpacing);
        }

        var handled = new HashSet<JobNodeViewModel>(topLevel);
        var queue = new Queue<JobNodeViewModel>(topLevel);

        while (queue.Count > 0)
        {
            var parent = queue.Dequeue();

            var children = Nodes.Where(n => n != parent && n.Job.Needs == parent.Job.Name).ToList();
            for (int ci = 0; ci < children.Count; ci++)
            {
                var child = children[ci];

                child.CanvasLeft = parent.CanvasLeft + HorizontalSpacing;
                child.CanvasTop = parent.CanvasTop + ci * (DefaultNodeHeight + VerticalSpacing);
                if (!handled.Contains(child))
                {
                    handled.Add(child);
                    queue.Enqueue(child);
                }
            }
        }

        var remaining = Nodes.Except(handled).ToList();
        for (int i = 0; i < remaining.Count; i++)
        {
            remaining[i].CanvasLeft = LeftMargin;
            remaining[i].CanvasTop = TopMargin + (topLevel.Count + i) * (DefaultNodeHeight + VerticalSpacing);
        }
    }

    private void EditNewNodePositions(Job job)
    {
        JobNodeViewModel node = new JobNodeViewModel(job);
        List<JobNodeViewModel> topNodes = Nodes.Where(n => string.IsNullOrWhiteSpace(n.Job.Needs)
                                        || !Nodes.Any(x => x.Job.Name == n.Job.Needs)).ToList();
        node.CanvasLeft = LeftMargin;
        node.CanvasTop = TopMargin + topNodes.Count * (DefaultNodeHeight + VerticalSpacing);
        Nodes.Add(node);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}