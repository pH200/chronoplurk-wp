using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MetroPlurk.Helpers
{
    public static class VisualTree
    {
        public static T FindChildOfType<T>(this DependencyObject root) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);

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
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var controlName = child.GetValue(FrameworkElement.NameProperty) as string;
                if (controlName == name)
                {
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }
                }
                
                var result = FindVisualChildByName<T>(child, name);
                if (result != null) return result;
            }
            return null;
        }
    }
}
