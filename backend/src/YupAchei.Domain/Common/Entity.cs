namespace YupAchei.Domain.Common;

public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; protected init; }

    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    protected Entity(Guid id)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
    }

    public bool Equals(Entity? outro)
    {
        if (outro is null)
        {
            return false;
        }

        if (ReferenceEquals(this, outro))
        {
            return true;
        }

        return GetType() == outro.GetType() && Id == outro.Id;
    }

    public override bool Equals(object? obj) => obj is Entity entidade && Equals(entidade);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
