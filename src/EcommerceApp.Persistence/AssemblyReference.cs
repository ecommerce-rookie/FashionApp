using Persistence.Contexts;
using System.Reflection;

namespace Persistence;

public static class AssemblyReference
{
    public static readonly Assembly Executing = Assembly.GetExecutingAssembly();
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    public static readonly Assembly WriteDbContextAssembly = typeof(EcommerceWriteContext).Assembly;
    public static readonly Assembly ReadDbContextAssembly = typeof(EcommerceReadContext).Assembly;
}