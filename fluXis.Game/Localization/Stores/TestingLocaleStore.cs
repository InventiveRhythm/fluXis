using osu.Framework.IO.Stores;

namespace fluXis.Game.Localization.Stores;

public class TestingLocaleStore : ResourceLocaleStore
{
    protected override bool ForceFallback => false;

    public TestingLocaleStore(string code, ResourceStore<byte[]> store)
        : base(code, store)
    {
    }

    public override string Get(string name) => base.Get(name) ?? $"[[{name}]]";
}
