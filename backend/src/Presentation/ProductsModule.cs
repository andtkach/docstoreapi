using Application.Products.CreateProduct;
using Application.Products.DeleteProduct;
using Application.Products.GetProductById;
using Application.Products.GetProducts;
using Application.Products.UpdateProduct;
using Carter;
using Domain.Shared;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using ListProductResponse = Application.Products.GetProducts.ProductResponse;
using OneProductResponse = Application.Products.GetProductById.ProductResponse;

namespace Presentation;

public class ProductsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/products",
            async (ISender sender) =>
            {
                Result<List<ListProductResponse>> result = await sender.Send(new GetProductsQuery());

                return Results.Ok(result.Value);
            });

        app.MapGet(
            "/products/{id}",
            async (int id, ISender sender) =>
            {
                Result<OneProductResponse> result = await sender.Send(new GetProductByIdQuery(id));
                if (result.IsFailure)
                {
                    return Results.NotFound();
                }

                return Results.Ok(result.Value);
            });

        app.MapPost(
            "/products",
            async (CreateProductRequest request, ISender sender) =>
            {
                CreateProductCommand command = request.Adapt<CreateProductCommand>();

                await sender.Send(command);

                return Results.Ok();
            });

        app.MapPut(
            "/products/{id}",
            async (long id, UpdateProductRequest request, ISender sender) =>
            {
                if (id != request.Id)
                {
                    return Results.BadRequest();
                }

                UpdateProductCommand command = request.Adapt<UpdateProductCommand>();
                var result = await sender.Send(command);

                if (result.IsSuccess)
                {
                    return Results.Ok();
                }

                return Results.BadRequest(result.Error);
            });

        app.MapDelete(
            "/products/{id}",
            async (long id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteProductCommand(id));

                if (result.IsSuccess)
                {
                    return Results.Ok();
                }

                return Results.BadRequest(result.Error);
            });
    }
}

public sealed record CreateProductRequest(
    string Name,
    decimal Price,
    List<string> Tags);

public sealed record UpdateProductRequest(
    long Id,
    string Name,
    decimal Price,
    List<string> Tags);
