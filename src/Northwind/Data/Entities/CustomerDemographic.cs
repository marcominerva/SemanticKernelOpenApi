namespace Northwind.Data.Entities;

public class CustomerDemographic
{
    public string CustomerTypeId { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = [];
}
