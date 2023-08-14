using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Audio;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Graphics;

public partial class FluXisTextBox : BasicTextBox
{
    protected override Color4 SelectionColour => FluXisColors.Background6;
    protected override Color4 InputErrorColour => FluXisColors.ButtonRed;

    public bool IsPassword { get; set; }
    public Action OnTextChanged;

    public Colour4 BackgroundInactive
    {
        get => BackgroundUnfocused;
        set => BackgroundUnfocused = value;
    }

    public Colour4 BackgroundActive
    {
        get => BackgroundFocused;
        set => BackgroundFocused = value;
    }

    private List<Sample> textAdded;
    private List<Sample> textAddedCaps;
    private Sample accept;
    private Sample delete;
    private Sample error;
    private Sample selectChar;
    private Sample selectWord;
    private Sample selectAll;

    public FluXisTextBox()
    {
        Colour = FluXisColors.Text;
        CornerRadius = 5;
        Masking = true;
        LengthLimit = 256;
        BackgroundInactive = FluXisColors.Background2;
        BackgroundActive = FluXisColors.Background3;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        BackgroundCommit = BorderColour = FluXisColors.Highlight;
        Placeholder.Font = FluXisSpriteText.GetFont();
        Placeholder.Colour = FluXisColors.Foreground;

        textAdded = new List<Sample>(3);
        textAddedCaps = new List<Sample>(3);

        for (int i = 0; i < textAdded.Capacity; i++)
            textAdded.Add(samples.Get($"UI/Keyboard/tap-{i + 1}"));

        for (int i = 0; i < textAddedCaps.Capacity; i++)
            textAddedCaps.Add(samples.Get($"UI/Keyboard/caps-{i + 1}"));

        accept = samples.Get("UI/Keyboard/confirm");
        delete = samples.Get("UI/Keyboard/delete");
        error = samples.Get("UI/Keyboard/error");
        selectChar = samples.Get("UI/Keyboard/select-char");
        selectWord = samples.Get("UI/Keyboard/select-word");
        selectAll = samples.Get("UI/Keyboard/select-all");
    }

    protected override Caret CreateCaret() => new FluXisCaret { SelectionColour = SelectionColour };

    protected override void OnUserTextAdded(string added)
    {
        if (added.Any(char.IsUpper)) textAddedCaps[RNG.Next(textAddedCaps.Count)]?.Play();
        else textAdded[RNG.Next(textAdded.Count)]?.Play();

        OnTextChanged?.Invoke();
    }

    protected override void OnUserTextRemoved(string removed)
    {
        delete?.Play();
        OnTextChanged?.Invoke();
    }

    protected override void OnTextCommitted(bool textChanged)
    {
        accept?.Play();
    }

    protected override void OnTextSelectionChanged(TextSelectionType selectionType)
    {
        switch (selectionType)
        {
            case TextSelectionType.Character:
                selectChar?.Play();
                break;

            case TextSelectionType.Word:
                selectWord?.Play();
                break;

            case TextSelectionType.All:
                selectAll?.Play();
                break;
        }
    }

    public void NotifyError() => NotifyInputError();

    protected override void NotifyInputError()
    {
        error?.Play();
        base.NotifyInputError();
    }

    protected override Drawable GetDrawableCharacter(char c)
    {
        var container = new FallingDownContainer
        {
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft
        };

        if (IsPassword)
        {
            container.Height = CalculatedTextSize;
            container.Width = CalculatedTextSize / 2;
            container.Child = new PasswordCharacter(CalculatedTextSize);
        }
        else
        {
            container.AutoSizeAxes = Axes.Both;
            container.Child = new FluXisSpriteText
            {
                Text = c.ToString(),
                FontSize = CalculatedTextSize
            };
        }

        return container;
    }

    private partial class PasswordCharacter : TicTac
    {
        public PasswordCharacter(float size)
            : base(size)
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Size = new Vector2(size / 2);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            this.ScaleTo(0).ScaleTo(1, 100);
        }
    }

    public partial class FluXisCaret : Caret
    {
        [Resolved]
        private AudioClock clock { get; set; }

        private bool shouldPulse = true;

        public FluXisCaret()
        {
            RelativeSizeAxes = Axes.Y;
            Size = new Vector2(4, 0.8f);
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.CentreLeft;
            CornerRadius = 2;
            Masking = true;

            InternalChild = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.White
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            clock.OnBeat += onBeat;
        }

        private void onBeat(int beat)
        {
            if (!shouldPulse) return;

            this.FadeIn().FadeTo(.5f, clock.BeatTime);
        }

        public override void Show()
        {
            base.Show();
            shouldPulse = true;
        }

        public override void Hide()
        {
            this.FadeOut(200);
            shouldPulse = false;
        }

        public Color4 SelectionColour { get; set; }

        public override void DisplayAt(Vector2 position, float? selectionWidth)
        {
            if (selectionWidth != null)
            {
                this.MoveTo(new Vector2(position.X, position.Y), 100, Easing.OutQuint);
                this.ResizeWidthTo(selectionWidth.Value, 100, Easing.OutQuint);
                this.FadeColour(SelectionColour, 200, Easing.OutQuint);
            }
            else
            {
                this.MoveTo(new Vector2(position.X, position.Y), 100, Easing.OutQuint);
                this.ResizeWidthTo(2, 100, Easing.OutQuint);
                this.FadeColour(Color4.White, 200, Easing.OutQuint);
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            clock.OnBeat -= onBeat;
        }
    }
}
