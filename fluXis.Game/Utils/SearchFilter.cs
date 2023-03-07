using System;
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

    private int bpm;
    private Type bpmType;
    private string query;

    public bool Matches(RealmMap map)
    {
        bool matches = false;

        if (BPM > 0)
        {
            switch (BPMType)
            {
                case Type.Exact:
                {
                    if (map.BPMMin <= BPM && map.BPMMax >= BPM)
                        matches = true;
                    else
                        return false;

                    break;
                }

                case Type.Over:
                {
                    if (map.BPMMin >= BPM)
                        matches = true;
                    else
                        return false;

                    break;
                }

                case Type.Under:
                {
                    if (map.BPMMax <= BPM)
                        matches = true;
                    else
                        return false;

                    break;
                }
            }
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

            termMatches |= title.Contains(Query);
            termMatches |= artist.Contains(Query);
            termMatches |= mapper.Contains(Query);
            termMatches |= source.Contains(Query);
            termMatches |= tags.Contains(Query);
            termMatches |= difficulty.Contains(Query);

            if (!termMatches)
                return false;

            if (!matches)
                matches = true;
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
