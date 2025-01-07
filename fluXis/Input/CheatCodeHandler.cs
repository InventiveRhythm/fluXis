using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Input;

public partial class CheatCodeHandler : Component
{
    private Action action { get; }
    private Key[] keys { get; }

    private List<Key> currentChain { get; } = new();

    public CheatCodeHandler(Action action, params Key[] keys)
    {
        this.action = action;
        this.keys = keys;
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (keys.Length == 0)
            return false;

        if (e.Key == keys[currentChain.Count])
            currentChain.Add(e.Key);
        else
            currentChain.Clear();

        if (currentChain.Count == keys.Length)
        {
            currentChain.Clear();
            action();
            return true;
        }

        return false;
    }
}
