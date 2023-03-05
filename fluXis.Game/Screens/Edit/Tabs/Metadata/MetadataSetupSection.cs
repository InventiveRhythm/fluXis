using fluXis.Game.Map;

namespace fluXis.Game.Screens.Edit.Tabs.Metadata;

public partial class MetadataSetupSection : SetupSection
{
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
    }

    public MapMetadata GetMetadata()
    {
        return new MapMetadata
        {
            Title = titleTextBox.Text,
            Artist = artistTextBox.Text,
            Mapper = mapperTextBox.Text,
            Difficulty = difficultyTextBox.Text,
            Source = sourceTextBox.Text,
            Tags = tagsTextBox.Text
        };
    }
}