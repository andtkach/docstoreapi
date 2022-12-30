using Application.Abstractions.Messaging;

namespace Application.Products.GetProductById;

public sealed record GetProductByIdQuery(int Id) : IQuery<ProductResponse>;
