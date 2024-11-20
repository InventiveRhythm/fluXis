using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Auth.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Screens.Edit;

public partial class EditorDifficultyCreationPanel : Panel
{
    private FluXisTextBox textBox;
    private Toggle keepNotes;
    private Toggle linkEffects;

    public Action<CreateNewMapParameters> OnCreate { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 420;
        AutoSizeAxes = Axes.Y;
        Content.RelativeSizeAxes = Axes.X;
        Content.AutoSizeAxes = Axes.Y;

        Content.Child = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Padding = new MarginPadding(20),
            Spacing = new Vector2(10),
            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = "Create new...",
                    WebFontSize = 32,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre
                },
                textBox = new AuthOverlayTextBox
                {
                    RelativeSizeAxes = Axes.X,
                    PlaceholderText = "New difficulty name..."
                },
                keepNotes = new Toggle("Copy Notes", ""),
                linkEffects = new Toggle("Link Effects", "Uses the same file for effects, meaning changing the effects updates them in the other map."),
                new AuthOverlayButton("Create") { Action = create },
                new AuthOverlayButton("Cancel") { Action = Hide }
            }
        };
    }

    private void create() => OnCreate?.Invoke(new CreateNewMapParameters
    {
        DifficultyName = textBox.Text,
        CopyNotes = keepNotes.State.Value,
        LinkEffects = linkEffects.State.Value
    });

    private partial class Toggle : CompositeDrawable, IHasTooltip
    {
        public LocalisableString TooltipText { get; }

        public Bindable<bool> State { get; } = new();

        public Toggle(string text, string tooltip)
        {
            TooltipText = tooltip;

            RelativeSizeAxes = Axes.X;
            Height = 40;
            Anchor = Anchor.TopCentre;
            Origin = Anchor.TopCentre;

            InternalChildren = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = text,
                    WebFontSize = 14,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                },
                new FluXisToggleSwitch
                {
                    State = State,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight
                }
            };
        }
    }
}
