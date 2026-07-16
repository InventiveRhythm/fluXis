namespace fluXis.Map.Structures.Bases;

public interface IHasStartValue<T>
{
    T StartValue { get; set; }
    bool UseStartValue { get; set; }
}
