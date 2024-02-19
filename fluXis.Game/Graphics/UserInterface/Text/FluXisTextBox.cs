using System;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Graphics.UserInterface.Text;

public partial class FluXisTextBox : BasicTextBox
{
    protected override Color4 SelectionColour => FluXisColors.Background6;
    protected override Color4 InputErrorColour => FluXisColors.ButtonRed;
    protected override float LeftRightPadding => SidePadding;

    public int SidePadding { get; set; } = 5;
    public float TextContainerHeight { get; set; } = .75f;
    public bool IsPassword { get; set; }
    public Action OnTextChanged { get; set; }
    public Action OnCommitAction { get; set; }

    private Container textContainer => TextContainer;

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

    private KeyboardSamples samples { get; set; } = new();

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
    private void load()
    {
        BackgroundCommit = BorderColour = FluXisColors.Highlight;
        Placeholder.Font = FluXisSpriteText.GetFont();
        Placeholder.Colour = FluXisColors.Foreground;
        TextContainer.Height = TextContainerHeight;

        Add(samples);
    }

    protected override Caret CreateCaret() => new FluXisCaret(this) { SelectionColour = SelectionColour };

    protected override void OnUserTextAdded(string added)
    {
        samples.Tap(added.Any(char.IsUpper) && !IsPassword);
        OnTextChanged?.Invoke();
    }

    protected override void OnUserTextRemoved(string removed)
    {
        samples.Delete();
        OnTextChanged?.Invoke();
    }

    protected override void OnTextCommitted(bool textChanged)
    {
        samples.Accept();
        OnCommitAction?.Invoke();
    }

    private double lastSelectionTime;

    protected override void OnTextSelectionChanged(TextSelectionType selectionType)
    {
        switch (selectionType)
        {
            case TextSelectionType.Character:
                if (Time.Current - lastSelectionTime > 50)
                {
                    samples.SelectChar();
                    lastSelectionTime = Time.Current;
                }

                break;

            case TextSelectionType.Word:
                samples.SelectWord();
                break;

            case TextSelectionType.All:
                samples.SelectAll();
                break;
        }
    }

    public void NotifyError() => NotifyInputError();

    protected override void NotifyInputError()
    {
        samples.Error();
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
            container.Height = FontSize;
            container.Width = FontSize / 2;
            container.Child = new PasswordCharacter(FontSize);
        }
        else
        {
            container.AutoSizeAxes = Axes.Both;
            container.Child = new FluXisSpriteText
            {
                Text = c.ToString(),
                FontSize = FontSize
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
        private GlobalClock clock { get; set; }

        private bool shouldPulse = true;

        private FluXisTextBox box { get; }

        public FluXisCaret(FluXisTextBox box)
        {
            this.box = box;

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
            // this sucks
            RelativeSizeAxes = box.FontSize != box.textContainer.DrawHeight ? Axes.None : Axes.Y;

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
