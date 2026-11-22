using System.Security.Cryptography;
using Api.Contracts;
using Core.Abstractions;
using Core.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Endpoints;

public static class BookEndpoints
{
    public static void MapBooks(this WebApplication app)
    {
        var group = app.MapGroup("/api/books").WithTags("Books");

        group.MapGet("/", (string? author, string? genre, IBookStore store) =>
        {
            var items = store.List().Select(ToDto);

            if (!string.IsNullOrWhiteSpace(author))
            {
                var authorTerm = author.Trim();
                items = items.Where(b => b.Author.Contains(authorTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(genre))
            {
                var genreTerm = genre.Trim();
                items = items.Where(b => b.Genre.Contains(genreTerm, StringComparison.OrdinalIgnoreCase));
            }

            return TypedResults.Ok(items.ToList());
        });

        group.MapGet("/{id}", Results<Ok<BookDto>, NotFound> (string id, IBookStore store) =>
            store.GetById(id) is { } found
                ? TypedResults.Ok(ToDto(found))
                : TypedResults.NotFound());

        group.MapPost("/", Results<Created<BookDto>, ValidationProblem> (
            SaveBookRequest request,
            IBookStore store) =>
        {
            if (!request.TryValidate(out var errors))
                return TypedResults.ValidationProblem(errors);

            var id = Convert.ToHexString(RandomNumberGenerator.GetBytes(4)).ToLowerInvariant();
            var created = BookCopy.Create(
                id: id,
                isbn: request.Isbn,
                title: request.Title,
                isIssued: false,
                author: request.Author,
                year: request.Year,
                copies: request.Copies,
                genre: request.Genre);

            store.Add(created);
            var dto = ToDto(created);
            return TypedResults.Created($"/api/books/{dto.Id}", dto);
        });

        group.MapPut("/{id}", Results<Ok<BookDto>, NotFound, ValidationProblem> (
            string id,
            SaveBookRequest request,
            IBookStore store) =>
        {
            if (!request.TryValidate(out var errors))
                return TypedResults.ValidationProblem(errors);

            var existing = store.GetById(id);
            if (existing is null)
                return TypedResults.NotFound();

            var replacement = BookCopy.Create(
                id: id,
                isbn: request.Isbn,
                title: request.Title,
                isIssued: existing.IsIssued,
                author: request.Author,
                year: request.Year,
                copies: request.Copies,
                genre: request.Genre);

            store.Update(replacement);
            return TypedResults.Ok(ToDto(replacement));
        });

        group.MapDelete("/{id}", Results<NoContent, NotFound> (string id, IBookStore store) =>
            store.Remove(id)
                ? TypedResults.NoContent()
                : TypedResults.NotFound());
    }

    private static BookDto ToDto(BookCopy copy) =>
        new(copy.Id, copy.Isbn, copy.Title, copy.Author, copy.Year, copy.Copies, copy.Genre);
}