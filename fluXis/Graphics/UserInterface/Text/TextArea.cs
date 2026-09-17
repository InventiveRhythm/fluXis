using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Graphics.UserInterface.Text;

#nullable enable

public partial class TextArea : TabbableContainer
{
    protected Container TextContainer { get; }
    protected FillFlowContainer<Line> LinesContainer { get; }

    public TextArea()
    {
        Masking = true;

        InternalChild = TextContainer = new Container
        {
            AutoSizeAxes = Axes.Both,
            Children =
            [
                LinesContainer = new FillFlowContainer<Line>
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                }
            ]
        };
    }

    private Line createLine() => new Line { Height = FluXisSpriteText.GetWebFontSize(FontSize) };

    private int getLineIndex(int left)
    {
        var last = LinesContainer.LastOrDefault(x => x.StartPosition <= left);
        return last is null ? 0 : LinesContainer.IndexOf(last);
    }

    private Line getLine(int idx)
    {
        if (idx < LinesContainer.Count)
            return LinesContainer[idx];

        for (int i = LinesContainer.Count - 1; i < idx; i++)
            LinesContainer.Add(createLine());

        return LinesContainer[idx];
    }

    private void rebuildLinePositions()
    {
        var pos = 0;

        foreach (var line in LinesContainer)
        {
            line.StartPosition = pos;
            pos += line.Count;
            if (line.HasNewLine) pos++;
        }
    }

    #region Text Content

    private string text = string.Empty;
    private readonly BindableWithCurrent<string> current = new(string.Empty);

    public virtual string Text
    {
        get => text;
        set
        {
            if (Current.Disabled)
                return;

            if (value == text)
                return;

            // lastCommitText = value ??= string.Empty;

            /*if (value.Length == 0)
                Placeholder.Show();
            else
                Placeholder.Hide();*/

            setText(value);
        }
    }

    public Bindable<string> Current
    {
        get => current.Current;
        set => current.Current = value;
    }

    private void setText(string value)
    {
        // bool beganChange = beginTextChange();
        // if (IsLoaded) FinalizeImeComposition(false);

        selectionStart = selectionEnd = 0;

        LinesContainer.Clear();
        text = string.Empty;

        insertString(value, d => d.FinishTransforms());

        // endTextChange(beganChange);
        // cursorAndLayout.Invalidate();
    }

    public void InsertString(string value)
    {
        // FinalizeImeComposition(false);

        insertString(value);
    }

    private void insertString(string value, Action<Drawable>? drawableCreationParameters = null)
    {
        if (string.IsNullOrEmpty(value)) return;

        if (Current.Disabled)
        {
            // NotifyInputError();
            return;
        }

        value = value.Replace("\r", string.Empty);

        // bool beganChange = beginTextChange();

        foreach (var c in value)
        {
            /*if (!canAddCharacter(c))
            {
                NotifyInputError();
                continue;
            }*/

            /*if (hasSelection)
                removeSelection();*/

            /*if (text.Length + 1 > LengthLimit)
            {
                NotifyInputError();
                break;
            }*/

            if (c == '\n')
            {
                var line = getLine(getLineIndex(selectionLeft));
                line.HasNewLine = true;
                LinesContainer.Add(createLine());
            }
            else
            {
                Drawable drawable = AddCharacterToFlow(c);
                drawable.Show();
                drawableCreationParameters?.Invoke(drawable);
            }

            text = text.Insert(selectionLeft, c.ToString());
            rebuildLinePositions();

            selectionStart = selectionEnd = selectionLeft + 1;
            // ignoreOngoingDragSelection = true;

            // cursorAndLayout.Invalidate();
        }
    }

    #endregion

    #region Selection

    private int selectionStart;
    private int selectionEnd;

    private int selectionLength => Math.Abs(selectionEnd - selectionStart);
    private bool hasSelection => selectionLength > 0;

    private int selectionLeft => Math.Min(selectionStart, selectionEnd);
    private int selectionRight => Math.Max(selectionStart, selectionEnd);

    #endregion

    #region Drawable Characters

    public float FontSize { get; set; } = 16;

    protected virtual Drawable GetDrawableCharacter(char c) => new FluXisSpriteText { Text = c.ToString(), WebFontSize = FontSize };

    protected virtual Drawable AddCharacterToFlow(char c)
    {
        // if (InputProperties.Type.IsPassword())
        // c = MaskCharacter;

        var line = getLine(getLineIndex(selectionLeft));

        var charsRight = new List<Drawable>();

        foreach (var d in line.Children.Skip(selectionLeft - line.StartPosition))
            charsRight.Add(d);

        line.RemoveRange(charsRight, false);

        // int i = selectionLeft;
        // foreach (Drawable d in charsRight)
        // d.Depth = getDepthForCharacterIndex(i++);

        Drawable ch = GetDrawableCharacter(c);
        // ch.Depth = getDepthForCharacterIndex(selectionLeft);

        line.Add(ch);
        line.AddRange(charsRight);
        return ch;
    }

    #endregion
}
