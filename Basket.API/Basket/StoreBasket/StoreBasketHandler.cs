using Discount.Grpc;

namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
    public record StoreBasketResult(string UserName);

    public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
    {
        public StoreBasketCommandValidator()
        {
            RuleFor(x => x.Cart)
                .NotNull()
                .WithMessage("Cart can not be null");

            RuleFor(x => x.Cart.UserName)
                .NotEmpty()
                .WithMessage("UserName is required");
        }
    }

    public class StoreBasketCommandHandler
        (IBasketRepository repository,DiscountProtoService.DiscountProtoServiceClient discountProto)
        : ICommandHandler<StoreBasketCommand, StoreBasketResult>
    {
        private readonly IBasketRepository _repository;


        public async Task<StoreBasketResult> Handle(
            StoreBasketCommand command,
            CancellationToken cancellationToken)
        {
            //TODO: communicate with Discount.Grpc and Calculate latest prices of items in the basket

            // Stroe the basket in database (use Marten upsert - if exist = update, if not exist = insert)

            await _repository.StoreBasket(command.Cart, cancellationToken);

            return new StoreBasketResult(command.Cart.UserName);
        }
    }
}
