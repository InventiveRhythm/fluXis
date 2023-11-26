using System;
using System.Collections.Generic;
using fluXis.Game.Database.Maps;

namespace fluXis.Game.Utils;

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
                    if (map.Filters.BPMMin <= BPM && map.Filters.BPMMax >= BPM)
                        matches = true;
                    else
                        return false;

                    break;
                }

                case Type.Over:
                {
                    if (map.Filters.BPMMin >= BPM)
                        matches = true;
                    else
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
            if (!Status.Contains(map.Status))
                return false;
        }

        if (Query.Length > 0)
        {
            bool termMatches = false;

            string title = map.Metadata.Title.ToLower();
            string artist = map.Metadata.Artist.ToLower();
            string mapper = map.Metadata.Mapper.ToLower();
            string source = map.Metadata.Source.ToLower();
            string tags = map.Metadata.Tags.ToLower();
            string difficulty = map.Difficulty.ToLower();

            termMatches |= title.Contains(Query.ToLower());
            termMatches |= artist.Contains(Query.ToLower());
            termMatches |= mapper.Contains(Query.ToLower());
            termMatches |= source.Contains(Query.ToLower());
            termMatches |= tags.Contains(Query.ToLower());
            termMatches |= difficulty.Contains(Query.ToLower());

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
