﻿namespace Cramming.SharedKernel
{
    public interface IRepository<T>
        : IReadRepository<T> where T : IAggregateRoot
    {
        Task<T> AddAsync(
            T entity,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            T entity,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            T entity,
            CancellationToken cancellationToken = default);
    }
}
