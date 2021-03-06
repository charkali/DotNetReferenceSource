

/***************************************************************************\
*
* File: DateTimeConverter2.cs
*
\***************************************************************************/

using System;
using System.ComponentModel;

#if PBTCOMPILER
namespace MS.Internal.Markup
#else
namespace System.Windows.Markup
#endif
{
    /// <summary>
    /// TypeConverter for System.Type
    /// </summary>
    internal class TypeTypeConverter : TypeConverter
    {
#if !PBTCOMPILER
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string typeName = value as string;

            if (null != context && typeName != null)
            {
                IXamlTypeResolver xamlTypeResolver = (IXamlTypeResolver)context.GetService(typeof(IXamlTypeResolver));

                if (null != xamlTypeResolver)
                {
                    return xamlTypeResolver.Resolve(typeName);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
#endif
    }
}
