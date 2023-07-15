namespace BlockTimer;

internal static class TypeExtensions
{
    internal static string GetTypeName(this Type type)
    {
        var typeName = string.Empty;

        if (type.IsGenericType)
        {
            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.GetTypeName()).ToArray());
            typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            typeName = type.Name;
        }

        return typeName;
    }

    internal static string GetTypeName(this object @object)
    {
        return @object.GetType().GetTypeName();
    }
}
