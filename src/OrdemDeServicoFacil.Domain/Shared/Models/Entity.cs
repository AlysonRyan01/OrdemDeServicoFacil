namespace OrdemDeServicoFacil.Domain.Shared.Models;

public abstract class Entity
{
    public readonly Guid Id;
    
    protected Entity() { }
    
    protected Entity(Guid id)
    {
        Id = id;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;
            
        if (ReferenceEquals(this, other))
            return true;
            
        if (GetType() != other.GetType())
            return false;
            
        if (Id == Guid.Empty || other.Id == Guid.Empty)
            return false;
            
        return Id == other.Id;
    }
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public static bool operator ==(Entity left, Entity right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;
            
        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;
            
        return left.Equals(right);
    }
    
    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }
}