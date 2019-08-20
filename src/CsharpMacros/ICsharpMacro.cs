using System.Collections.Generic;

namespace CsharpMacros
{
    public interface ICsharpMacro
    {
        IEnumerable<Dictionary<string, string>> ExecuteMacro(string param, ICsharpMacroContext context);
    }
}