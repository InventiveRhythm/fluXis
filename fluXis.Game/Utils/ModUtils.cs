using System;
using System.Linq;
using System.Reflection;
using fluXis.Game.Mods;

namespace fluXis.Game.Utils;

public static class ModUtils
{
    public static IMod GetFromAcronym(string acronym)
    {
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
