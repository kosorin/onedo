using SimpleTasks.Core.Helpers;
using SimpleTasks.Core.Models;
using SimpleTasks.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SimpleTasks.Controls
{
    public class ListPickerItem : Microsoft.Phone.Controls.ListPickerItem
    {
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(ListPickerItem), new PropertyMetadata(""));


        public ListPickerItem(string label)
        {
            DefaultStyleKey = typeof(ListPickerItem);
            Label = label;
        }

        public override string ToString()
        {
            return Label;
        }
    }

    public class ListPickerItem<T> : ListPickerItem
    {
        public T Value
        {
            get { return (T)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(T), typeof(ListPickerItem<T>), new PropertyMetadata(default(T)));


        public ListPickerItem(string label, T value) :
            base(label)
        {
            DefaultStyleKey = typeof(ListPickerItem);
            Value = value;
        }
    }

    public class ListPickerItem<T1, T2> : ListPickerItem
    {
        public T1 Value1
        {
            get { return (T1)GetValue(Value1Property); }
            set { SetValue(Value1Property, value); }
        }
        public static readonly DependencyProperty Value1Property =
            DependencyProperty.Register("Value1", typeof(T1), typeof(ListPickerItem<T1, T2>), new PropertyMetadata(default(T1)));


        public T2 Value2
        {
            get { return (T2)GetValue(Value2Property); }
            set { SetValue(Value2Property, value); }
        }
        public static readonly DependencyProperty Value2Property =
            DependencyProperty.Register("Value2", typeof(T2), typeof(ListPickerItem<T1, T2>), new PropertyMetadata(default(T2)));


        public ListPickerItem(string label, T1 value1, T2 value2) :
            base(label)
        {
            DefaultStyleKey = typeof(ListPickerItem);
            Value1 = value1;
            Value2 = value2;
        }
    }

    public class ColorListPickerItem : ListPickerItem
    {
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorListPickerItem), new PropertyMetadata(Colors.Transparent, ColorProperty_Changed));
        private static void ColorProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColorListPickerItem)d).ColorBrush = new SolidColorBrush((Color)e.NewValue);
        }


        public SolidColorBrush ColorBrush
        {
            get { return (SolidColorBrush)GetValue(ColorBrushProperty); }
            private set { SetValue(ColorBrushProperty, value); }
        }
        public static readonly DependencyProperty ColorBrushProperty =
            DependencyProperty.Register("ColorBrush", typeof(SolidColorBrush), typeof(ColorListPickerItem), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));


        public ColorListPickerItem(string label, Color color)
            : base(label)
        {
            DefaultStyleKey = typeof(ColorListPickerItem);
            Color = color;
        }
    }

    public class GestureActionListPickerItem : ListPickerItem
    {
        public GestureAction Action
        {
            get { return (GestureAction)GetValue(ActionProperty); }
            private set { SetValue(ActionProperty, value); }
        }
        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.Register("Action", typeof(GestureAction), typeof(GestureActionListPickerItem), new PropertyMetadata(GestureAction.None));


        public Style Icon
        {
            get { return (Style)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Style), typeof(GestureActionListPickerItem), new PropertyMetadata(null));


        public GestureActionListPickerItem(GestureAction action)
            : base(GestureActionHelper.Text(action))
        {
            DefaultStyleKey = typeof(GestureActionListPickerItem);
            Action = action;
            Icon = GestureActionHelper.IconStyle(action);
        }
    }
}
