namespace LuaDefinitionMaker.Docs;

public class MethodDocumentation
{
    public string? Summary { get; set; }
    public Dictionary<string, string> Parameters { get; } = new();
    public string? Returns { get; set; }

    public void AddParameter(string name, string description) => Parameters[name] = description;

    public string? GetParameterDescription(string name) =>
        Parameters.TryGetValue(name, out var description) ? description : null;
}
