using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Binding = System.Windows.Data.Binding;

namespace Rx.Net.Plus
{
    public static class ExtensionsVisual
    {
        /// <summary>
        /// Perform a specific action on all elements' binding
        /// </summary>
        /// <param name="targetElement"> the root framework element </param>
        /// <param name="action"> an action to perform on a binding </param>
        public static void DoOnAllBindings(FrameworkElement targetElement, Action<Binding> action)
        {
            WalkElementTree(targetElement,  frameworkElement => ActionBindings(frameworkElement, action));
        }

        /// <summary>
        /// Perform an action on a element tree
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        public static void WalkElementTree(object obj, Action<FrameworkElement> action)
        {
            if (obj is FrameworkElement dependencyObject)
            {
                action (dependencyObject);
                
                foreach (var child in LogicalTreeHelper.GetChildren(dependencyObject))
                {
                    WalkElementTree(child, action);
                }
            }
        }

        /// <summary>
        /// Perform the action on the binding
        /// </summary>
        /// <param name="target"></param>
        /// <param name="action"></param>
        private static void ActionBindings(DependencyObject target, Action<Binding> action)
        {
            var localValueEnumerator = target.GetLocalValueEnumerator();
            
            while (localValueEnumerator.MoveNext())
            {
                var current = localValueEnumerator.Current;
                var binding = BindingOperations.GetBinding(target, current.Property);

                if (binding != null)
                {
                    action(binding);
                }
            }
        }

        /// <summary>
        /// Binds RxProperties to View Model
        /// Note that the view model shall implements IPropertyChangedProxy
        /// Refer to doc for details
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="view"></param>
        public static void BindRxPropertiesToView (this IPropertyChangedProxy viewModel, object view)
        {
            var rxProperties = new List<string>();          // Holds all RxProperties names

            var typeOfThis = viewModel.GetType();
            var allProp = typeOfThis.GetProperties();

            // Bind all properties to view model
            allProp.ToList().ForEach(p =>
            {
                if (p.GetValue(viewModel) is IPropertyToBind propertyToBind)       // Get only IPropertyToBind (RxProperty)
                {
                    propertyToBind.BindToView (viewModel, p.Name);
                    rxProperties.Add(p.Name);
                }
            });

            // Update the Binding path from RxProperty name to name.Value (allows TwoWay binding mode)
            DoOnAllBindings(view as FrameworkElement, binding =>
            {
                var path = binding.Path?.Path;
                
                if (rxProperties.Contains(path))
                {
                    binding.Path.Path += ".Value";
                }
            });
        }
    }
}