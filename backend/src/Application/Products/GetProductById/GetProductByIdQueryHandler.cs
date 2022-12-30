using Application.Abstractions.Messaging;
using Domain.Products;
using Domain.Shared;
using Marten;
using Marten.Linq;

namespace Application.Products.GetProductById;

internal sealed class GetProductsQueryHandler
    : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IQuerySession _session;

    public GetProductsQueryHandler(IQuerySession session)
    {
        _session = session;
    }

    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _session
            .Query<Product>()
            .Where(p => p.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (product != null)
        {
            return new ProductResponse(product.Id, product.Name, product.Price, product.Tags);
        }

        return default(ProductResponse);
    }
}
