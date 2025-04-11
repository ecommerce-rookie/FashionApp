namespace Infrastructure.Shared.Helpers
{
    public static class NameHelper
    {
        public static string GenerateName(object handler, object[] arguments)
        {
            string handlerName = handler.GetType().Name;
            if (arguments.Length > 0)
            {
                var firstArg = arguments[0];
                var properties = firstArg.GetType().GetProperties();
                if (properties.Any())
                {
                    var queryProperties = properties
                        .Select(prop => $"{prop.GetValue(firstArg) ?? "null"}");
                    return $"{handlerName}_{string.Join("_", queryProperties)}";
                } else
                {
                    return $"{handlerName}_{firstArg.ToString()}";
                }
            }
            return handlerName;
        }
    }
}
