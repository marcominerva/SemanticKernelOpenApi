using FluentValidation;
using Northwind.Shared.Models;

namespace Northwind.Validations;

public class NewProductValidator : AbstractValidator<NewProduct>
{
    public NewProductValidator()
    {
        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(40);
        RuleFor(x => x.QuantityPerUnit).MaximumLength(20);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.UnitsInStock).Must(units => units is null or >= 0).WithMessage($"{nameof(NewProduct.UnitsInStock)} must be greater than or equal to 0.");
        RuleFor(x => x.UnitsOnOrder).Must(units => units is null or >= 0).WithMessage($"{nameof(NewProduct.UnitsOnOrder)} must be greater than or equal to 0.");
        RuleFor(x => x.ReorderLevel).Must(units => units is null or >= 0).WithMessage($"{nameof(NewProduct.ReorderLevel)} must be greater than or equal to 0.");
    }
}
