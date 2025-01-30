namespace NSE.Core.DomainObjects;

public abstract class Entity
{
    public Guid Id { get; private set; }

    public static bool operator ==(Entity a, Entity b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b)
    {
        return !(a == b);
    }

    public override bool Equals(object? obj)
    {
        var compareTo = obj as Entity;

        if (compareTo is null) return false;

        if (ReferenceEquals(this, compareTo)) return true;

        return Id.Equals(compareTo.Id);
    }

    public override int GetHashCode()
    {
        return (GetType().GetHashCode() * 907) + Id.GetHashCode();
    }

    public override string ToString()
    {
        return $"{GetType().Name} [Id={Id}]";
    }
}
