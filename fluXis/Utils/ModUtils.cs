using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using fluXis.Mods;

namespace fluXis.Utils;

public static class ModUtils
{
    public static IMod Create(Type type)
    {
        if (type == null || !isMod(type))
            return null;

        return (IMod)Activator.CreateInstance(type);
    }

    public static IMod GetFromAcronym(string acronym)
    {
        if (string.IsNullOrEmpty(acronym))
            return null;

        if (acronym.EndsWith('x'))
        {
            var rate = acronym[..^1];
            return rate.TryParseFloatInvariant(out var rateValue) ? new RateMod { Rate = rateValue } : null;
        }

        var assembly = Assembly.GetAssembly(typeof(IMod));

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

    public static bool HasIncompatibleMods(IEnumerable<IMod> combo, out IEnumerable<IMod> incompatibleMods)
    {
        incompatibleMods = combo.Where(mod => combo.Any(otherMod => IsIncompatible(mod, otherMod)));
        return incompatibleMods.Any();
    }

    public static bool IsIncompatible(IMod mod, IMod otherMod) => mod.IncompatibleMods.Contains(otherMod.GetType());

    private static bool isMod(Type type)
    {
        return type?.GetInterface(nameof(IMod)) != null;
    }
}
