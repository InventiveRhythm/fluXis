using System.Runtime.Serialization;
using AutoMapper;
using AutoMapper.Internal;
using fluXis.Game.Database.Input;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using osu.Framework.Logging;
using Realms;

namespace fluXis.Game.Database;

public static class RealmObjectUtils
{
    private static readonly IMapper write_mapper = new MapperConfiguration(c =>
    {
        c.ShouldMapField = _ => false;
        c.ShouldMapProperty = pi => pi.SetMethod?.IsPublic == true;
        Logger.LogPrint("Creating write mapper", LoggingTarget.Database, LogLevel.Debug);

        c.CreateMap<RealmScore, RealmScore>();
        c.CreateMap<RealmMapMetadata, RealmMapMetadata>();
        c.CreateMap<RealmMapFilters, RealmMapFilters>();
        c.CreateMap<RealmMap, RealmMap>()
         .ForMember(s => s.Metadata, cc => cc.Ignore())
         .ForMember(s => s.Filters, cc => cc.Ignore())
         .ForMember(s => s.MapSet, cc => cc.Ignore())
         .AfterMap((s, d) =>
         {
             copyChanges(s.Filters, d.Filters);
             copyChanges(s.Metadata, d.Metadata);
         });
        c.CreateMap<RealmMapSet, RealmMapSet>()
         .ConstructUsing(_ => new RealmMapSet(null))
         .ForMember(s => s.Maps, cc => cc.Ignore())
         .AfterMap((s, d) =>
         {
             foreach (var map in s.Maps)
             {
                 var existing = d.Realm!.Find<RealmMap>(map.ID);

                 if (existing != null)
                 {
                     if (!d.Maps.Contains(existing))
                         d.Maps.Add(existing);

                     copyChanges(map, existing);
                 }
                 else
                 {
                     var newMap = new RealmMap()
                     {
                         ID = map.ID,
                         Metadata = new RealmMapMetadata(),
                         Filters = new RealmMapFilters(),
                         MapSet = d
                     };

                     d.Maps.Add(newMap);
                     copyChanges(map, newMap);
                 }
             }
         });

        c.Internal().ForAllMaps((_, expression) =>
        {
            expression.ForAllMembers(m =>
            {
                if (m.DestinationMember.Has<IgnoredAttribute>()
                    || m.DestinationMember.Has<BacklinkAttribute>()
                    || m.DestinationMember.Has<IgnoreDataMemberAttribute>())
                    m.Ignore();
            });
        });
    }).CreateMapper();

    private static readonly IMapper mapper = new MapperConfiguration(c =>
    {
        setConfiguration(c);

        c.CreateMap<RealmMapSet, RealmMapSet>()
         .ConstructUsing(_ => new RealmMapSet(null))
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
         .ConstructUsing(_ => new RealmMapSet(null))
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
        c.CreateMap<RealmScore, RealmScore>();
        c.CreateMap<RealmMapFilters, RealmMapFilters>();
        c.CreateMap<RealmKeybind, RealmKeybind>();
    }

    public static T Detach<T>(this T realmObject) where T : RealmObjectBase
    {
        if (realmObject is RealmMapSet set)
            return set_mapper.Map<T>(set);

        return mapper.Map<T>(realmObject);
    }

    public static void CopyChanges(this RealmMapSet source, RealmMapSet dest)
        => copyChanges(source, dest);

    private static void copyChanges<T>(T source, T dest) where T : RealmObjectBase
        => write_mapper.Map(source, dest);
}
