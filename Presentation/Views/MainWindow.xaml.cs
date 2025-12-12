using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VisualWorkflowBuilder.UiImplementation.ViewModels;

namespace VisualWorkflowBuilder;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Maximized;

        if (Vm != null)
        {
            Dispatcher.BeginInvoke(new System.Action(RefreshConnectors), System.Windows.Threading.DispatcherPriority.Loaded);

            Vm.Nodes.CollectionChanged += Nodes_CollectionChanged;

            foreach (var n in Vm.Nodes)
            {
                AttachNodePropertyChanged(n);
            }
        }

        if (WorkspaceScrollViewer != null)
        {
            WorkspaceScrollViewer.ScrollChanged += (_, __) => RefreshConnectors();
        }
    }

    private MainViewModel? Vm => DataContext as MainViewModel;

    private void Nodes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (JobNodeViewModel n in e.NewItems)
            {
                AttachNodePropertyChanged(n);
            }
        }

        if (e.OldItems != null)
        {
            foreach (JobNodeViewModel n in e.OldItems)
            {
                n.PropertyChanged -= Node_PropertyChanged;
            }
        }


        RefreshConnectors();
    }

    private void AttachNodePropertyChanged(JobNodeViewModel node)
    {
        node.PropertyChanged -= Node_PropertyChanged;
        node.PropertyChanged += Node_PropertyChanged;
    }

    private void Node_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {

        if (e.PropertyName == nameof(JobNodeViewModel.CanvasLeft) || e.PropertyName == nameof(JobNodeViewModel.CanvasTop))
        {
            Dispatcher.BeginInvoke(new System.Action(RefreshConnectors), System.Windows.Threading.DispatcherPriority.Render);
        }
    }


    private void RefreshConnectors()
    {
        if (ConnectorCanvas == null || JobsItemsControl == null) return;


        ConnectorCanvas.Width = JobsItemsControl.ActualWidth > 0 ? JobsItemsControl.ActualWidth : WorkspaceGrid.ActualWidth;
        ConnectorCanvas.Height = JobsItemsControl.ActualHeight > 0 ? JobsItemsControl.ActualHeight : WorkspaceGrid.ActualHeight;

        ConnectorCanvas.Children.Clear();
        if (Vm == null) return;


        var elementMap = new Dictionary<JobNodeViewModel, FrameworkElement>();
        foreach (var node in Vm.Nodes)
        {
            var container = JobsItemsControl.ItemContainerGenerator.ContainerFromItem(node) as FrameworkElement;
            if (container == null) continue;
            var border = FindVisualChild<Border>(container);
            if (border != null)
            {
                elementMap[node] = border;
            }
        }

        foreach (var child in Vm.Nodes)
        {
            var needName = child.Job.Needs;
            if (string.IsNullOrWhiteSpace(needName)) continue;

            var parent = Vm.Nodes.FirstOrDefault(n => n.Job.Name == needName);
            if (parent == null) continue;
            if (!elementMap.TryGetValue(parent, out var parentElem)) continue;
            if (!elementMap.TryGetValue(child, out var childElem)) continue;


            var parentPos = parentElem.TransformToVisual(ConnectorCanvas).Transform(new Point(0, 0));
            var childPos  = childElem.TransformToVisual(ConnectorCanvas).Transform(new Point(0, 0));

            double parentRight = parentPos.X + parentElem.ActualWidth;
            double parentCenterY = parentPos.Y + parentElem.ActualHeight / 2.0;

            double childLeft = childPos.X;
            double childCenterY = childPos.Y + childElem.ActualHeight / 2.0;


            Point startPoint = new Point(parentRight, parentCenterY);
            Point endPoint = new Point(childLeft, childCenterY);


            var midPoint = new Point((startPoint.X + endPoint.X) / 2.0, startPoint.Y);
            var midPoint2 = new Point((startPoint.X + endPoint.X) / 2.0, endPoint.Y);

            var path = new Path
            {
                Stroke = (Brush)FindResource("Brush.TextMuted") ?? Brushes.Gray,
                StrokeThickness = 2,
                Data = new PathGeometry(new[]
                {
                    new PathFigure(startPoint, new PathSegment[]
                    {
                        new PolyLineSegment(new[] { midPoint, midPoint2, endPoint }, true)
                    }, false)
                })
            };

            ConnectorCanvas.Children.Add(path);


            DrawArrowHead(endPoint, midPoint2, ConnectorCanvas);
        }
    }

    private static void DrawArrowHead(Point tip, Point from, Canvas canvas)
    {

        const double size = 8;
        var direction = new Vector(tip.X - from.X, tip.Y - from.Y);
        if (direction.Length == 0) return;
        direction.Normalize();


        var perp = new Vector(-direction.Y, direction.X);
        var p1 = tip - direction * size + perp * (size * 0.6);
        var p2 = tip - direction * size - perp * (size * 0.6);

        var tri = new Polygon
        {
            Points = new PointCollection { tip, p1, p2 },
            Fill = (Brush)System.Windows.Application.Current.FindResource("Brush.TextMuted") ?? Brushes.Gray,
            IsHitTestVisible = false
        };

        canvas.Children.Add(tri);
    }

    private static T? FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj == null) return null;
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);
            if (child is T t) return t;
            var childItem = FindVisualChild<T>(child);
            if (childItem != null) return childItem;
        }
        return null;
    }
}