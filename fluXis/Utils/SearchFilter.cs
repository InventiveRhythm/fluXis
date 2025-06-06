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
            var words = Query.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                var matches = false;

                matches |= map.Metadata.Title.Contains(word, StringComparison.CurrentCultureIgnoreCase);
                matches |= map.Metadata.TitleRomanized?.Contains(word, StringComparison.CurrentCultureIgnoreCase) ?? false;
                matches |= map.Metadata.Artist.Contains(word, StringComparison.CurrentCultureIgnoreCase);
                matches |= map.Metadata.ArtistRomanized?.Contains(word, StringComparison.CurrentCultureIgnoreCase) ?? false;
                matches |= map.Metadata.Mapper.Contains(word, StringComparison.CurrentCultureIgnoreCase);
                matches |= map.Metadata.Source.Contains(word, StringComparison.CurrentCultureIgnoreCase);
                matches |= map.Metadata.Tags.Contains(word, StringComparison.CurrentCultureIgnoreCase);
                matches |= map.Difficulty.Contains(word, StringComparison.CurrentCultureIgnoreCase);

                if (!matches)
                    return false;
            }
        }

        if (KeyMode > 0)
        {
            if (map.KeyCount != KeyMode)
                return false;
        }

        return true;
    }

    public enum Type
    {
        Exact,
        Over,
        Under
    }
}
