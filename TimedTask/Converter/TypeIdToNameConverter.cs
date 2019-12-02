// 版权所有：
// 文 件  名：TypeIdToNameConverter.cs
// 功能描述：
// 创建标识：Seven Song(m.sh.lin0328@163.com) 2014/1/19 13:28:35
// 修改描述：
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;

using System.Text;

namespace TimedTask.Converter
{
    public class TypeIdToNameConverter : System.Windows.Data.IValueConverter//此接口有以下两个方法
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null ||
                Model.PM.NoteTypeHt == null ||
                Model.PM.NoteTypeHt.Count == 0)
                return "";

            if (Model.PM.NoteTypeHt.ContainsKey(value))
                return Model.PM.NoteTypeHt[value];

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
