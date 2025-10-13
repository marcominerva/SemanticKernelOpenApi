using FluentValidation;
using Northwind.Shared.Models;

namespace Northwind.Validations;

public class NewCategoryValidator : AbstractValidator<NewCategory>
{
    public NewCategoryValidator()
    {
        RuleFor(x => x.CategoryName).NotEmpty().MaximumLength(15);
    }
}
