using ChucksKitchenApi.DTOS;
using FluentValidation;
using System.Data;

namespace ChucksKitchenApi.Validators
{
    public class CartItemCreateDTOValidator : AbstractValidator<CartItemCreateDTO>
    {
        public CartItemCreateDTOValidator()
        {
            RuleFor(x => x.MenuId)
                .GreaterThan(0)
                .NotEmpty().WithMessage("MenuId is required.");
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .NotEmpty().WithMessage("Quantity is required.");
        }
    }
}
