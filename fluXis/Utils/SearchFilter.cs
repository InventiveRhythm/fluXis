using System;
using System.Collections.Generic;
using fluXis.Database.Maps;

namespace fluXis.Utils;

public class SearchFilters
{
    public Action OnChange { get; set; } = () => { };

    public string Query
    {
        get => query;
        set
        {
            query = value;
            OnChange.Invoke();
        }
    }

    public int BPM
    {
        get => bpm;
        set
        {
            bpm = value;
            OnChange.Invoke();
        }
    }

    public Type BPMType
    {
        get => bpmType;
        set
        {
            bpmType = value;
            OnChange.Invoke();
        }
    }

    public int KeyMode
    {
        get => keymode;
        set
        {
            keymode = value;
            OnChange.Invoke();
        }
    }

    public List<int> Status { get; set; } = new();

    private int bpm;
    private Type bpmType;
    private string query = string.Empty;
    private int keymode;

    public bool Matches(RealmMap map)
    {
        bool matches = true;

        if (BPM > 0)
        {
            switch (BPMType)
            {
                case Type.Exact:
                {
                    if (!(map.Filters.BPMMin <= BPM) || !(map.Filters.BPMMax >= BPM))
                        return false;

                    break;
                }

                case Type.Over:
                {
                    if (!(map.Filters.BPMMin >= BPM))
                        return false;

                    break;
                }

                case Type.Under:
                {
                    if (!(map.Filters.BPMMax <= BPM))
                        return false;

                    break;
                }
            }
        }

        if (Status.Count > 0)
        {
            if (!Status.Contains(map.StatusInt))
                return false;
        }

        if (Query.Length > 0)
        {
            bool termMatches = false;

            termMatches |= map.Metadata.Title.Contains(Query, StringComparison.CurrentCultureIgnoreCase);
            termMatches |= map.Metadata.TitleRomanized?.Contains(Query, StringComparison.CurrentCultureIgnoreCase) ?? false;
            termMatches |= map.Metadata.Artist.Contains(Query, StringComparison.CurrentCultureIgnoreCase);
            termMatches |= map.Metadata.ArtistRomanized?.Contains(Query, StringComparison.CurrentCultureIgnoreCase) ?? false;
            termMatches |= map.Metadata.Mapper.Contains(Query, StringComparison.CurrentCultureIgnoreCase);
            termMatches |= map.Metadata.Source.Contains(Query, StringComparison.CurrentCultureIgnoreCase);
            termMatches |= map.Metadata.Tags.Contains(Query, StringComparison.CurrentCultureIgnoreCase);
            termMatches |= map.Difficulty.Contains(Query, StringComparison.CurrentCultureIgnoreCase);

            if (!termMatches)
                matches = false;
        }

        if (KeyMode > 0)
        {
            if (map.KeyCount != KeyMode)
                matches = false;
        }

        return matches;
    }

    public enum Type
    {
        Exact,
        Over,
        Under
    }
}
