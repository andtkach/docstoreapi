using Application.Abstractions.Messaging;
using Domain.Products;
using Domain.Shared;
using Marten;

namespace Application.Products.DeleteProduct;

internal sealed class DeleteProductCommandHandler
    : ICommandHandler<DeleteProductCommand>
{
    private readonly IDocumentSession _session;

    public DeleteProductCommandHandler(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _session
            .Query<Product>()
            .Where(p => p.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
        {
            return Result.Failure(new Error("404", "Not found"));
        }

        _session.Delete(product);

        await _session.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
