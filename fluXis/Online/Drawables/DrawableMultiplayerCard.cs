﻿using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.Drawables.Images;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Online.Drawables;

public partial class DrawableMultiplayerCard : CompositeDrawable
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private FluXisGame game { get; set; }

    private MultiplayerRoom room { get; }

    public DrawableMultiplayerCard(MultiplayerRoom room)
    {
        this.room = room;

        Width = 360;
        Height = 80;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        CornerRadius = 12;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background4
            },
            new LoadWrapper<DrawableOnlineBackground>
            {
                RelativeSizeAxes = Axes.Both,
                LoadContent = () => new DrawableOnlineBackground(room.Map)
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2,
                Alpha = .5f
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding(16),
                Spacing = new Vector2(8),
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.X,
                        Height = 14,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(6),
                        Children = new Drawable[]
                        {
                            new FluXisSpriteIcon
                            {
                                Size = new Vector2(16),
                                Icon = room.Privacy switch
                                {
                                    MultiplayerPrivacy.Public => FontAwesome6.Solid.EarthAmericas,
                                    MultiplayerPrivacy.Club => FontAwesome6.Solid.CircleNodes,
                                    MultiplayerPrivacy.Private => FontAwesome6.Solid.Lock,
                                    _ => FontAwesome6.Solid.Question
                                },
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            },
                            new FluXisSpriteText
                            {
                                Text = room.Name,
                                WebFontSize = 16,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            }
                        }
                    },
                    createPlayers()
                }
            }
        };
    }

    private FillFlowContainer createPlayers()
    {
        const int max = 8;

        var flow = new FillFlowContainer
        {
            AutoSizeAxes = Axes.X,
            Height = 20,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(8),
            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(-8),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    ChildrenEnumerable = room.Participants.Take(max).Select(x => new LoadWrapper<DrawableAvatar>
                    {
                        Size = new Vector2(20),
                        CornerRadius = 10,
                        Masking = true,
                        LoadContent = () => new DrawableAvatar(x.Player)
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }
                    })
                }
            }
        };

        var leftover = room.Participants.Count - max;

        if (leftover > 0)
        {
            flow.Add(new ForcedHeightText
            {
                Text = $"+{leftover}",
                WebFontSize = 12,
                Height = 10,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            });
        }

        return flow;
    }

    protected override bool OnClick(ClickEvent e)
    {
        game?.CloseOverlays();
        game?.JoinMultiplayerRoom(room.RoomID, "");
        return false;
    }
}
