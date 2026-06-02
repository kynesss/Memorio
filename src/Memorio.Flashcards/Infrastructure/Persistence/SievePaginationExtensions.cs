using Memorio.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace Memorio.Flashcards.Infrastructure.Persistence;

internal static class SievePaginationExtensions
{
    public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> source,
        SieveModel model,
        ISieveProcessor sieveProcessor,
        SieveOptions options,
        Func<TEntity, TDto> map,
        CancellationToken cancellationToken)
    {
        var filtered = sieveProcessor.Apply(model, source, applyPagination: false);
        var totalCount = await filtered.CountAsync(cancellationToken);

        var pageQuery = sieveProcessor.Apply(model, filtered, applyFiltering: false, applySorting: false);
        var entities = await pageQuery.ToListAsync(cancellationToken);

        var page = model.Page ?? 1;
        var pageSize = Math.Min(model.PageSize ?? options.DefaultPageSize, options.MaxPageSize);

        var items = entities.Select(map).ToList();
        return new PagedResult<TDto>(items, page, pageSize, totalCount);
    }
}
