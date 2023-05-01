using System.Runtime.Serialization;
using AutoMapper;
using AutoMapper.Internal;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using Realms;

namespace fluXis.Game.Database;

public static class RealmObjectUtils
{
    private static readonly IMapper mapper = new MapperConfiguration(c =>
    {
        setConfiguration(c);

        c.CreateMap<RealmMapSet, RealmMapSet>()
         .ConstructUsing(_ => new RealmMapSet())
         .MaxDepth(2)
         .AfterMap((_, d) =>
         {
             foreach (var map in d.Maps)
                 map.MapSet = d;
         });

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

    private static readonly IMapper set_mapper = new MapperConfiguration(c =>
    {
        setConfiguration(c);

        c.CreateMap<RealmMapSet, RealmMapSet>()
         .ConstructUsing(_ => new RealmMapSet())
         .MaxDepth(2)
         .AfterMap((_, d) =>
         {
             foreach (var map in d.Maps)
                 map.MapSet = d;
         });

        c.CreateMap<RealmMap, RealmMap>()
         .MaxDepth(1)
         .ForMember(m => m.MapSet, cc => cc.Ignore());
    }).CreateMapper();

    private static void setConfiguration(IMapperConfigurationExpression c)
    {
        c.ShouldMapField = _ => false;
        c.ShouldMapProperty = p => p.GetMethod?.IsPublic == true;

        c.Internal().ForAllMaps((_, ex) =>
        {
            ex.ForAllMembers(m =>
            {
                if (m.DestinationMember.Has<IgnoredAttribute>() || m.DestinationMember.Has<BacklinkAttribute>() || m.DestinationMember.Has<IgnoreDataMemberAttribute>())
                    m.Ignore();
            });
        });

        c.CreateMap<RealmMap, RealmMap>();
        c.CreateMap<RealmMapMetadata, RealmMapMetadata>();
        c.CreateMap<RealmMapSet, RealmMapSet>();
        c.CreateMap<RealmFile, RealmFile>();
        c.CreateMap<RealmScore, RealmScore>();
        c.CreateMap<RealmJudgements, RealmJudgements>();
        c.CreateMap<RealmMapFilters, RealmMapFilters>();
    }

    public static T Detach<T>(this T realmObject) where T : RealmObjectBase
    {
        if (realmObject is RealmMapSet set)
            return set_mapper.Map<T>(set);

        return mapper.Map<T>(realmObject);
    }
}
