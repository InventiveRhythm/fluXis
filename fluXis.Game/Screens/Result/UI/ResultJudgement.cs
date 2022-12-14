using fluXis.Game.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Result.UI
{
    public class ResultJudgement : CompositeDrawable
    {
        private const int box_height = 54;
        private const int box_margin = 5;

        private readonly Judgement judgement;
        private readonly int count;
        private readonly int id;
        private readonly int max;

        private Box line;
        private SpriteText judgename;
        private SpriteText judgecount;

        public ResultJudgement(Judgement judgement, int count, int id, int max)
        {
            this.judgement = judgement;
            this.count = count;
            this.id = id;
            this.max = max;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.BottomLeft;
            Origin = Anchor.BottomLeft;
            Height = box_height;
            Y = (max - 1 - id) * (-box_height - box_margin);

            AddRangeInternal(new Drawable[]
            {
                line = new Box
                {
                    Width = 2,
                    Height = box_height,
                    Colour = judgement.Color
                },
                judgename = new SpriteText
                {
                    Text = $"{judgement.Key}",
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    X = 10,
                    Colour = judgement.Color,
                    Font = new FontUsage("Quicksand", 32f, "SemiBold")
                },
                judgecount = new SpriteText
                {
                    Text = $"{count}",
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    X = 10,
                    Font = new FontUsage("Quicksand", 32f, "SemiBold")
                }
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            this.FadeTo(0)
                .MoveToX(200)
                .Then(id * 100)
                .MoveToX(0, 250, Easing.OutQuint)
                .FadeInFromZero(250);
        }
    }
}
