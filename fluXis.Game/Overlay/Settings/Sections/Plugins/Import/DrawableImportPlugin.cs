using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Graphics;
using fluXis.Game.Import;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Overlay.Settings.Sections.Plugins.Import;

public partial class DrawableImportPlugin : Container
{
    public ImportPlugin Plugin { get; init; }

    [BackgroundDependencyLoader]
    private void load(FluXisRealm realm)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        CornerRadius = 5;
        Masking = true;

        FillFlowContainer flow;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background
            },
            flow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding(10),
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Both,
                                Direction = FillDirection.Vertical,
                                Children = new[]
                                {
                                    new FluXisSpriteText
                                    {
                                        Text = Plugin.Name,
                                        FontSize = 24
                                    },
                                    new FluXisSpriteText
                                    {
                                        Text = $"by {Plugin.Author}",
                                        Colour = FluXisColors.Text2,
                                        FontSize = 16
                                    }
                                }
                            },
                            new FluXisSpriteText
                            {
                                Text = Plugin.Version.ToString(),
                                FontSize = 16,
                                Colour = FluXisColors.Text2,
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight
                            }
                        }
                    }
                }
            }
        };

        if (Plugin.HasAutoImport)
        {
            Bindable<bool> autoImport = new Bindable<bool>(realm.Run(r =>
                r.All<ImporterInfo>().FirstOrDefault(i => i.Id == Plugin.ImporterId)?.AutoImport ?? false));

            flow.Add(new SettingsDivider());
            flow.Add(new SettingsToggle
            {
                Label = "Auto Import Maps",
                Description = "Requires a restart to take effect.",
                Bindable = autoImport
            });

            autoImport.BindValueChanged(e =>
            {
                realm.RunWrite(r =>
                {
                    var info = r.All<ImporterInfo>().FirstOrDefault(i => i.Id == Plugin.ImporterId);

                    if (info is not null)
                        info.AutoImport = e.NewValue;
                });
            });
        }
    }
}
