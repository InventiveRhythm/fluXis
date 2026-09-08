using System;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;

namespace fluXis.Graphics.UserInterface.Form;

#nullable enable

public abstract partial class BaseFormComponent<T, C> : BaseFormComponent
    where C : BaseFormComponent<T, C>
{
    public new T Value
    {
        get => Bindable.Value;
        set => Bindable.Value = value;
    }

    public new Bindable<T> Bindable { get; }
    public Action<C, T>? OnValueChanged { get; set; }

    protected bool FinishedLoading { get; private set; }

    protected BaseFormComponent(LocalisableString label, Bindable<T> bind)
        : base(label, bind.GetBoundCopy(), bind.Value!)
    {
        Bindable = (Bindable<T>)base.Bindable;

        Bindable.BindValueChanged(v =>
        {
            OnValueChanged?.Invoke((C)this, v.NewValue);
            base.Value = v.NewValue!;
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        FinishTransforms(true);
        FinishedLoading = true;
    }
}

public abstract partial class BaseFormComponent : CompositeDrawable
{
    protected const float HEIGHT = 64;
    protected const float RADIUS = 8;
    protected const float PADDING = 12;
    protected const float INNER_GAP = 2;

    public LocalisableString Label { get; }
    protected FluXisSpriteText? LabelSprite { get; set; }

    public LocalisableString? Description { get; init; }

    public object Value { get; set; }
    public IBindable Bindable { get; protected set; }

    protected virtual Colour4 BackgroundColor => Theme.Background3;

    protected HoverLayer? Hover { get; private set; }
    protected FlashLayer? Flash { get; private set; }

    private Container? head;

    protected BaseFormComponent(LocalisableString label, IBindable bind, object value)
    {
        RelativeSizeAxes = Axes.X;

        Label = label;
        Bindable = bind;
        Value = value;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Y;

        InternalChild = CreateWrapper();
    }

    protected virtual Drawable CreateWrapper() => head = new Container
    {
        RelativeSizeAxes = Axes.X,
        Height = HEIGHT,
        CornerRadius = RADIUS,
        Masking = true,
        Children =
        [
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = BackgroundColor,
            },
            Hover = new HoverLayer(),
            Flash = new FlashLayer(),
            CreateContent()
        ]
    };

    protected abstract Drawable CreateContent();

    protected FluXisSpriteText CreateDefaultLabel() => new()
    {
        Text = Label,
        WebFontSize = 14,
        Colour = Theme.TextVariant
    };

    protected void StartHighlight()
    {
        head?.BorderColorTo(Theme.Highlight, 50).BorderTo(3);
        LabelSprite?.FadeColour(Theme.Highlight, 50);
    }

    protected void StopHighlight()
    {
        head?.BorderColorTo(BackgroundColor, 50);
        LabelSprite?.FadeColour(Theme.TextVariant, 50);
    }
}
