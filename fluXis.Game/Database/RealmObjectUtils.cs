using System.Collections.Generic;
using System.Runtime.Serialization;
using AutoMapper;
using AutoMapper.Internal;
using fluXis.Game.Database.Maps;
using Realms;

namespace fluXis.Game.Database;

public static class RealmObjectUtils
{
    private static readonly IMapper mapper = new MapperConfiguration(c =>
    {
        setConfiguration(c);

        c.CreateMap<RealmMapSet, RealmMapSet>()
         .ConstructUsing(_ => new RealmMapSet(null, null))
         .MaxDepth(2)
         .AfterMap((_, d) =>
         {
             foreach (var map in d.Maps)
                 map.MapSet = d;
         });

        // This can be further optimised to reduce cyclic retrievals, similar to the optimised set mapper below.
        // Only hasn't been done yet as we detach at the point of BeatmapInfo less often.
        c.CreateMap<RealmMap, RealmMap>()
         .MaxDepth(2)
         .AfterMap((_, d) =>
         {
             for (int i = 0; i < d.MapSet?.Maps.Count; i++)
             {
                 if (d.MapSet.Maps[i].Equals(d))
                 {
                     d.MapSet.Maps[i] = d;
                     break;
                 }
             }
         });
    }).CreateMapper();

    private static void setConfiguration(IMapperConfigurationExpression c)
    {
        c.ShouldMapField = _ => false;
        c.ShouldMapProperty = p => p.GetMethod?.IsPublic == true;

        c.Internal().ForAllMaps((_, ex) =>
        {
            ex.ForAllMembers(m =>
            {
                if (m.DestinationMember.Has<IgnoredAttribute>() || m.DestinationMember.Has<BacklinkAttribute>() || m.DestinationMember.Has<PrimaryKeyAttribute>() || m.DestinationMember.Has<IgnoreDataMemberAttribute>())
                    m.Ignore();
            });
        });

        c.CreateMap<RealmMap, RealmMap>();
        c.CreateMap<RealmMapMetadata, RealmMapMetadata>();
        c.CreateMap<RealmMapSet, RealmMapSet>();
        c.CreateMap<RealmFile, RealmFile>();
    }

    public static T Detach<T>(this T realmObject) where T : RealmObjectBase
    {
        return mapper.Map<T>(realmObject);
    }

    public static List<T> Detach<T>(this IEnumerable<T> realmObjects) where T : RealmObjectBase
    {
        var detached = new List<T>();

        foreach (var realmObject in realmObjects)
            detached.Add(mapper.Map<T>(realmObject));

        return detached;
    }
}
