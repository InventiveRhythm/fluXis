using fluXis.Game.Map;
using osu.Framework.Allocation;

namespace fluXis.Game.Screens.Edit.Tabs.Metadata;

public partial class MetadataSetupSection : SetupSection
{
    [Resolved]
    private EditorValues values { get; set; }

    private readonly SetupTextBox titleTextBox;
    private readonly SetupTextBox artistTextBox;
    private readonly SetupTextBox mapperTextBox;
    private readonly SetupTextBox difficultyTextBox;
    private readonly SetupTextBox sourceTextBox;
    private readonly SetupTextBox tagsTextBox;

    public MetadataSetupSection(MapMetadata metadata)
        : base("Metadata")
    {
        AddInternal(titleTextBox = new SetupTextBox("Title"));
        AddInternal(artistTextBox = new SetupTextBox("Artist"));
        AddInternal(mapperTextBox = new SetupTextBox("Mapper"));
        AddInternal(difficultyTextBox = new SetupTextBox("Difficulty"));
        AddInternal(sourceTextBox = new SetupTextBox("Source"));
        AddInternal(tagsTextBox = new SetupTextBox("Tags"));

        titleTextBox.Text = metadata.Title;
        artistTextBox.Text = metadata.Artist;
        mapperTextBox.Text = metadata.Mapper;
        difficultyTextBox.Text = metadata.Difficulty;
        sourceTextBox.Text = metadata.Source;
        tagsTextBox.Text = metadata.Tags;

        titleTextBox.OnTextChanged = updateMetadata;
        artistTextBox.OnTextChanged = updateMetadata;
        mapperTextBox.OnTextChanged = updateMetadata;
        difficultyTextBox.OnTextChanged = updateMetadata;
        sourceTextBox.OnTextChanged = updateMetadata;
        tagsTextBox.OnTextChanged = updateMetadata;
    }

    private void updateMetadata()
    {
        values.MapInfo.Metadata.Title = values.Editor.Map.Metadata.Title = titleTextBox.Text;
        values.MapInfo.Metadata.Artist = values.Editor.Map.Metadata.Artist = artistTextBox.Text;
        values.MapInfo.Metadata.Mapper = values.Editor.Map.Metadata.Mapper = mapperTextBox.Text;
        values.MapInfo.Metadata.Difficulty = values.Editor.Map.Difficulty = difficultyTextBox.Text;
        values.MapInfo.Metadata.Source = values.Editor.Map.Metadata.Source = sourceTextBox.Text;
        values.MapInfo.Metadata.Tags = values.Editor.Map.Metadata.Tags = tagsTextBox.Text;
    }
}
