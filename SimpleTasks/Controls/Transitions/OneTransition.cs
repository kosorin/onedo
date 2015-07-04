using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Resources;
using Microsoft.Phone.Controls;

namespace SimpleTasks.Controls.Transitions
{
    public class OneTransition : TransitionElement
    {
        private static Dictionary<string, string> _storyboardXamlCache = null;

        public OneTransitionMode Mode
        {
            get
            {
                return (OneTransitionMode)GetValue(ModeProperty);
            }
            set
            {
                SetValue(ModeProperty, value);
            }
        }
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(OneTransitionMode), typeof(OneTransition), null);

        public override ITransition GetTransition(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (!Enum.IsDefined(typeof(OneTransitionMode), Mode))
            {
                throw new ArgumentOutOfRangeException("OneTransitionMode");
            }
            element.RenderTransform = new TranslateTransform();
            return GetEnumStoryboard<OneTransitionMode>(element, "One", Mode);
        }

        private static ITransition GetEnumStoryboard<T>(UIElement element, string name, T mode)
        {
            string key = name + Enum.GetName(typeof(T), mode);
            Storyboard storyboard = GetStoryboard(key);
            if (storyboard == null)
            {
                return null;
            }
            Storyboard.SetTarget(storyboard, element);
            return new Transition(element, storyboard);
        }

        private static Storyboard GetStoryboard(string name)
        {
            if (_storyboardXamlCache == null)
            {
                _storyboardXamlCache = new Dictionary<string, string>();
            }
            string xaml = null;
            if (_storyboardXamlCache.ContainsKey(name))
            {
                xaml = _storyboardXamlCache[name];
            }
            else
            {
                string path = "/SimpleTasks;component/Controls/Transitions/" + name + ".xaml";
                Uri uri = new Uri(path, UriKind.Relative);
                StreamResourceInfo streamResourceInfo = Application.GetResourceStream(uri);
                using (StreamReader streamReader = new StreamReader(streamResourceInfo.Stream))
                {
                    xaml = streamReader.ReadToEnd();
                    _storyboardXamlCache[name] = xaml;
                }
            }
            return XamlReader.Load(xaml) as Storyboard;
        }
    }
}
