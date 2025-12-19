using System.ComponentModel;
using VisualWorkflowBuilder.Core.Entities;

namespace VisualWorkflowBuilder.Presentation.ViewModels;

public sealed class JobNodeViewModel : INotifyPropertyChanged
{
    private double _canvasLeft;
    private double _canvasTop;

    public Job Job { get; }

    public JobNodeViewModel(Job job)
    {
        Job = job;
    }

    public string Name => Job?.Name ?? string.Empty;
    public double CanvasLeft
    {
        get => _canvasLeft;
        set
        {
            if (_canvasLeft == value) return;
            _canvasLeft = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanvasLeft)));
        }
    }

    public double CanvasTop
    {
        get => _canvasTop;
        set
        {
            if (_canvasTop == value) return;
            _canvasTop = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanvasTop)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}