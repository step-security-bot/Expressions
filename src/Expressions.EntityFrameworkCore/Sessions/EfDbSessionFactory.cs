﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.EntityFrameworkCore.Options;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

public class EfDbSessionFactory<TContext>
    : IDbSessionFactory<TContext>, IDbQuerySessionFactory<TContext>, IDbSessionFactory, IDbQuerySessionFactory
    where TContext : DbContext
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IDbContextFactory<TContext> _contextFactory;
    private readonly ISqlProviderSelector _sqlProviderSelector;
    private readonly EntityOptionsSelector _optionsSelector;

    public EfDbSessionFactory(
        ILoggerFactory loggerFactory,
        IDbContextFactory<TContext> contextFactory,
        ISqlProviderSelector sqlProviderSelector,
        EntityOptionsSelector optionsSelector)
    {
        _loggerFactory = loggerFactory;
        _contextFactory = contextFactory;
        _sqlProviderSelector = sqlProviderSelector;
        _optionsSelector = optionsSelector;
    }

    public EfDbSession<TContext> Create(ChangeTracking? tracking = null) => new(
        _loggerFactory.CreateLogger<EfDbSession<TContext>>(),
        _contextFactory.CreateDbContext(),
        _sqlProviderSelector,
        _optionsSelector,
        tracking ?? ChangeTracking.Default);

    public EfDbSession<TContext> CreateForQuery() => Create(ChangeTracking.Disable);

    IDbSession IDbSessionFactory.Create(ChangeTracking? tracking) => Create(tracking);

    IDbSession<TContext> IDbSessionFactory<TContext>.Create(ChangeTracking? tracking) => Create(tracking);

    IDbQuerySession IDbQuerySessionFactory.Create() => CreateForQuery();

    IDbQuerySession<TContext> IDbQuerySessionFactory<TContext>.Create() => CreateForQuery();
}
