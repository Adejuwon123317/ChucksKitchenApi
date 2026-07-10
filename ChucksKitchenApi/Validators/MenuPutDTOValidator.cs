using ChucksKitchenApi.DTOS;
using FluentValidation;

namespace ChucksKitchenApi.Validators
{
    public class MenuPutDTOValidator : AbstractValidator<MenuPutDTO>
    {
        public MenuPutDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100)
                .WithMessage("maximum length of 100 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .NotEmpty().WithMessage("Price is required");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("maximum length of 500 characters ");

            RuleFor(x => x.ImageUrl)
    .NotEmpty().WithMessage("ImageUrl is required.")
    .MaximumLength(300).WithMessage("maximum length of 300 characters")
    .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute)).WithMessage("valid url is required");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .NotEmpty().WithMessage("CategoryId is required");
        }
    }
}
