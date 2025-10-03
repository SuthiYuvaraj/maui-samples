using System.Windows.Input;
using DeveloperBalance.Models;

namespace DeveloperBalance.Pages.Controls;

public partial class TaskView
{
    public TaskView()
    {
        InitializeComponent();
#if WINDOWS
        TaskViewBorder.Loaded += OnTaskViewLoaded;
#endif
    }

#if WINDOWS

    private void OnTaskViewLoaded(object sender, EventArgs e)
    {
        if (TaskViewBorder.Handler?.PlatformView is Microsoft.UI.Xaml.FrameworkElement platformView)
        {
            platformView.IsTabStop = true;
			platformView.UseSystemFocusVisuals = true;
            platformView.KeyDown += OnKeyDown;
        }
    }
 
    private void OnKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Space || e.Key == Windows.System.VirtualKey.Enter)
        {
            if (BindingContext is ProjectTask task)
            {
                Element? current = this.Parent;
                while (current != null)
                {
                    if (current.BindingContext is IProjectTaskPageModel pageModel)
                    {
                        pageModel.NavigateToTaskCommand?.Execute(task);
                        break;
                    }

                    current = current.Parent;
                }
            }
            e.Handled = true;
        }
    }

#endif

    public static readonly BindableProperty TaskCompletedCommandProperty = BindableProperty.Create(
        nameof(TaskCompletedCommand),
        typeof(ICommand),
        typeof(TaskView),
        null);

    public ICommand TaskCompletedCommand
    {
        get => (ICommand)GetValue(TaskCompletedCommandProperty);
        set => SetValue(TaskCompletedCommandProperty, value);
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var checkbox = (CheckBox)sender;

        if (checkbox.BindingContext is not ProjectTask task)
            return;

        if (task.IsCompleted == e.Value)
            return;

        task.IsCompleted = e.Value;
        TaskCompletedCommand?.Execute(task);
    }
}