using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MetroPlurk.Helpers
{
    public static class VisualTree
    {
        public static T FindChildOfType<T>(this DependencyObject parent) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(parent);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }
                    queue.Enqueue(child);
                }
            }
            return null;
        }

        public static T FindVisualChildByName<T>(this DependencyObject parent, string name) where T : DependencyObject
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(parent);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var childrenCount = VisualTreeHelper.GetChildrenCount(current);

                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var controlName = child.GetValue(FrameworkElement.NameProperty) as string;
                    if (controlName == name)
                    {
                        var typedChild = child as T;
                        if (typedChild != null)
                        {
                            return typedChild;
                        }
                    }
                    queue.Enqueue(child);
                }
            }
            return null;
        }
    }
}
