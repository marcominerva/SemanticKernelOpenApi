using System.ComponentModel;

namespace Northwind.Shared.Models;

public class NewCategory
{
    [Description("The name of the category. It must be 15 characters max")]
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }
}
