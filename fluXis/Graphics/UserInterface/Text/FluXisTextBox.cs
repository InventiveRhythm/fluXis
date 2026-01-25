using System;
using System.Diagnostics;
using System.Linq;
using fluXis.Audio;
using fluXis.Graphics.Drawables;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Integration;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Graphics.UserInterface.Text;

public partial class FluXisTextBox : BasicTextBox
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private ISteamManager steam { get; set; }

    protected override Color4 SelectionColour => Theme.Background6;
    protected override Color4 InputErrorColour => Theme.ButtonRed;
    protected override float LeftRightPadding => SidePadding;

    public int SidePadding { get; init; } = 5;
    public float TextContainerHeight { get; set; } = .75f;
    public bool IsPassword { get; init; }
    public bool FixedWidth { get; init; }

    public Action OnTextChanged { get; set; }
    public Action OnCommitAction { get; set; }

    public Action OnFocusAction { get; set; }
    public Action OnFocusLostAction { get; set; }

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

    private KeyboardSamples samples { get; } = new();

    public FluXisTextBox()
    {
        CornerRadius = 5;
        Masking = true;
        LengthLimit = 256;
        BackgroundInactive = Theme.Background2;
        BackgroundActive = Theme.Background3;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        BackgroundCommit = Theme.Highlight;
        Placeholder.Font = FluXisSpriteText.GetFont();
        Placeholder.Colour = Theme.Foreground;
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
        if (textChanged)
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
    public void RemoveFocus() => KillFocus();

    protected override void NotifyInputError()
    {
        samples.Error();
        base.NotifyInputError();
    }

    protected override void OnFocus(FocusEvent e)
    {
        base.OnFocus(e);
        OnFocusAction?.Invoke();
        steam?.OpenKeyboard(this);
    }

    protected override void OnFocusLost(FocusLostEvent e)
    {
        base.OnFocusLost(e);
        OnFocusLostAction?.Invoke();
        steam?.CloseKeyboard();
    }

    protected override Drawable GetDrawableCharacter(char c)
    {
        var container = new FallingDownContainer
        {
            Anchor = Anchor.TopLeft,
            Origin = Anchor.TopLeft
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
                FontSize = FontSize,
                FixedWidth = FixedWidth
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
        [CanBeNull]
        [Resolved(CanBeNull = true)]
        private IBeatSyncProvider beatSync { get; set; }

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
                Colour = Theme.Text,
            };
        }

        protected override void LoadComplete()
        {
            // this sucks
            RelativeSizeAxes = box.FontSize != box.textContainer.DrawHeight ? Axes.None : Axes.Y;

            base.LoadComplete();

            if (beatSync != null)
                beatSync.OnBeat += onBeat;
        }

        private void onBeat(int beat, bool finish)
        {
            Debug.Assert(beatSync != null);
            if (!shouldPulse) return;

            this.FadeIn().FadeTo(.5f, beatSync.BeatTime);
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
                this.FadeColour(Theme.Text, 200, Easing.OutQuint);
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (beatSync != null)
                beatSync.OnBeat -= onBeat;
        }
    }
}
