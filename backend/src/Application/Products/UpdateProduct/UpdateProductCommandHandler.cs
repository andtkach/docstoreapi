using Application.Abstractions.Messaging;
using Domain.Products;
using Domain.Shared;
using Marten;

namespace Application.Products.UpdateProduct;

internal sealed class UpdateProductCommandHandler
    : ICommandHandler<UpdateProductCommand>
{
    private readonly IDocumentSession _session;

    public UpdateProductCommandHandler(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _session
            .Query<Product>()
            .Where(p => p.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
        {
            return Result.Failure(new Error("404", "Not found"));
        }

        product.Name = request.Name;
        product.Price = request.Price;
        product.Tags = request.Tags;

        _session.Store(product);

        await _session.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
