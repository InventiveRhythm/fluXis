using System;
using System.Reflection;
using fluXis.Utils.Attributes;

namespace fluXis.Utils.Inspect;

public class ObjectInspectInvalidComboException : InvalidOperationException
{
    public ObjectInspectInvalidComboException(TypeOverrideAttribute.Type attr, PropertyInfo prop)
        : base($"Custom type '{attr}' can not be represented with '{prop.PropertyType}'.")
    {
    }
}
