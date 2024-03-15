using System;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;

namespace fluXis.Game.Screens.Edit.Tabs.Metadata;

public partial class DifficultySetupSection : SetupSection
{
    protected override LocalisableString Title => "Difficulty";

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(new SetupSlider
        {
            Title = "Accuracy",
            Default = Map.MapInfo.AccuracyDifficulty,
            OnValueChanged = value => Map.MapInfo.AccuracyDifficulty = Map.RealmMap.AccuracyDifficulty = value
        });
    }

    public partial class SetupSlider : Container
    {
        public string Title { get; init; }
        public float Default { get; init; }
        public Action<float> OnValueChanged { get; init; }

        private FluXisSpriteText valueText;
        private Bindable<float> bindable;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = 40;

            bindable = new BindableFloat(8)
            {
                MinValue = 1,
                MaxValue = 10,
                Precision = 0.1f
            };

            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = Title,
                    FontSize = 24,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreRight,
                    X = 90
                },
                valueText = new FluXisSpriteText
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    X = -420,
                    Text = Default.ToString("0.0")
                },
                new FluXisSlider<float>
                {
                    Width = 400,
                    Height = 40,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Bindable = bindable,
                    Step = 0.1f
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            bindable.Value = Default;
            bindable.BindValueChanged(e =>
            {
                OnValueChanged?.Invoke(e.NewValue);
                valueText.Text = e.NewValue.ToStringInvariant("0.0");
            }, true);
        }
    }
}
