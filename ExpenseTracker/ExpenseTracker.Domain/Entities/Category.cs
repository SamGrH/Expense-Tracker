namespace ExpenseTracker.Domain.Entities;

public sealed class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;



    private Category() { }

    public Category(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name.Length > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(name), "Category name cannot exceed 100 characters.");
        }
        Name = name;
    }
    public Category(int id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name.Length > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(name), "Category name cannot exceed 100 characters.");
        }
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        Id = id;
        Name = name;
    }


}



