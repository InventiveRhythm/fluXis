using osu.Framework.Allocation;
using osu.Framework.Localisation;

namespace fluXis.Game.Screens.Edit.Tabs.Metadata;

public partial class MetadataSetupSection : SetupSection
{
    protected override LocalisableString Title => "Metadata";

    private SetupTextBox titleTextBox;
    private SetupTextBox artistTextBox;
    private SetupTextBox mapperTextBox;
    private SetupTextBox difficultyTextBox;
    private SetupTextBox sourceTextBox;
    private SetupTextBox tagsTextBox;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddInternal(titleTextBox = new SetupTextBox("Title"));
        AddInternal(artistTextBox = new SetupTextBox("Artist"));
        AddInternal(mapperTextBox = new SetupTextBox("Mapper"));
        AddInternal(difficultyTextBox = new SetupTextBox("Difficulty"));
        AddInternal(sourceTextBox = new SetupTextBox("Source"));
        AddInternal(tagsTextBox = new SetupTextBox("Tags"));

        titleTextBox.Text = Map.MapInfo.Metadata.Title;
        artistTextBox.Text = Map.MapInfo.Metadata.Artist;
        mapperTextBox.Text = Map.MapInfo.Metadata.Mapper;
        difficultyTextBox.Text = Map.MapInfo.Metadata.Difficulty;
        sourceTextBox.Text = Map.MapInfo.Metadata.Source;
        tagsTextBox.Text = Map.MapInfo.Metadata.Tags;

        titleTextBox.OnTextChanged = updateMetadata;
        artistTextBox.OnTextChanged = updateMetadata;
        mapperTextBox.OnTextChanged = updateMetadata;
        difficultyTextBox.OnTextChanged = updateMetadata;
        sourceTextBox.OnTextChanged = updateMetadata;
        tagsTextBox.OnTextChanged = updateMetadata;
    }

    private void updateMetadata()
    {
        Map.MapInfo.Metadata.Title = Map.RealmMap.Metadata.Title = titleTextBox.Text;
        Map.MapInfo.Metadata.Artist = Map.RealmMap.Metadata.Artist = artistTextBox.Text;
        Map.MapInfo.Metadata.Mapper = Map.RealmMap.Metadata.Mapper = mapperTextBox.Text;
        Map.MapInfo.Metadata.Difficulty = Map.RealmMap.Difficulty = difficultyTextBox.Text;
        Map.MapInfo.Metadata.Source = Map.RealmMap.Metadata.Source = sourceTextBox.Text;
        Map.MapInfo.Metadata.Tags = Map.RealmMap.Metadata.Tags = tagsTextBox.Text;
    }
}
