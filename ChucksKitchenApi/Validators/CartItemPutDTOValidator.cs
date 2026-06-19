using ChucksKitchenApi.DTOS;
using FluentValidation;

namespace ChucksKitchenApi.Validators
{
    public class CartItemPutDTOValidator : AbstractValidator<CartItemPutDTO>
    {
        public CartItemPutDTOValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .NotEmpty().WithMessage("Quantity is required");
        }
    }
}
