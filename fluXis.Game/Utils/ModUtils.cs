using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using fluXis.Game.Mods;

namespace fluXis.Game.Utils;

public static class ModUtils
{
    public static IMod GetFromAcronym(string acronym)
    {
        if (string.IsNullOrEmpty(acronym))
            return null;

        if (acronym.EndsWith("x"))
        {
            var rate = acronym.Substring(0, acronym.Length - 1);
            return float.TryParse(rate, NumberStyles.Float, CultureInfo.InvariantCulture, out var rateValue) ? new RateMod { Rate = rateValue } : null;
        }

        Assembly assembly = Assembly.GetAssembly(typeof(IMod));

        if (assembly != null)
        {
            Type[] types = assembly.GetTypes();

            return (from type in types
                    where !type.IsInterface && !type.IsAbstract
                    where type.GetInterface(nameof(IMod)) != null
                    select (IMod)Activator.CreateInstance(type)).FirstOrDefault(mod => mod != null && mod.Acronym == acronym);
        }

        return null;
    }
}
