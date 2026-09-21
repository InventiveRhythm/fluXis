namespace fluXis.Map.Format;

#nullable enable

public interface IMapFormat
{
    bool IsChart(string path);

    PlayableMap? Parse(string path);
    void Save(PlayableMap map);
}
