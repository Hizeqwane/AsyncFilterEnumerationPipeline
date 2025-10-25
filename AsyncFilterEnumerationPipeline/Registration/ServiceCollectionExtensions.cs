using Microsoft.Extensions.DependencyInjection;

namespace AsyncFilterEnumerationPipeline.Registration;

/// <summary>
/// <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    // Начало регистрации пайплайна с именем
    public static PipelineBuilder<T> AddPipeline<T>(
        this IServiceCollection services,
        string pipelineName = "default",
        ServiceLifetime pipelineLifetime = ServiceLifetime.Transient,
        ServiceLifetime defaultFilterLifetime = ServiceLifetime.Transient) => 
        string.IsNullOrEmpty(pipelineName)
            ? throw new ArgumentException("Pipeline name cannot be null or empty", nameof(pipelineName))
            : new PipelineBuilder<T>(services, pipelineName, pipelineLifetime, defaultFilterLifetime);

    // Добавление фильтра с возвратом к PipelineBuilder<T, TFilter> для типобезопасной цепочки
    public static PipelineBuilder<T> AddFilter<T, TFilter>(
        this PipelineBuilder<T> builder,
        ServiceLifetime filterLifetime = ServiceLifetime.Transient)
        where TFilter : class, IFilter<T>
    {
        var uniqueServiceName = builder.Name.GetUniqueFilterName(typeof(TFilter));
        builder.FilterKeys.Add(uniqueServiceName);
        
        switch (filterLifetime)
        {
            case ServiceLifetime.Singleton:
                builder.Services.AddKeyedSingleton<IFilter<T>, TFilter>(uniqueServiceName);
                break;
            case ServiceLifetime.Scoped:
                builder.Services.AddKeyedScoped<IFilter<T>, TFilter>(uniqueServiceName);
                break;
            case ServiceLifetime.Transient:
            default:
                builder.Services.AddKeyedTransient<IFilter<T>, TFilter>(uniqueServiceName);
                break;
        }

        return builder;
    }

    // Завершение регистрации пайплайна
    public static IServiceCollection Build<T>(this PipelineBuilder<T> builder)
    {
        Func<IServiceProvider, object, Pipeline<T>> implementationFactory = (serviceProvider, _) =>
        {
            var keys = builder.FilterKeys;
            var filters = GetPipelineFilters<T>(
                serviceProvider,
                keys.ToArray());
            
            return new Pipeline<T>(filters);
        };
        
        switch (builder.PipelineLifetime)
        {
            case ServiceLifetime.Singleton:
                builder.Services.AddKeyedSingleton(builder.Name, implementationFactory);
                break;
            case ServiceLifetime.Scoped:
                builder.Services.AddKeyedScoped(builder.Name, implementationFactory);
                break;
            case ServiceLifetime.Transient:
            default:
                builder.Services.AddKeyedTransient(builder.Name, implementationFactory);
                break;
        }

        return builder.Services;
    }

    private static IEnumerable<IFilter<T>> GetPipelineFilters<T>(
        IServiceProvider serviceProvider, params string[] filterKeys) =>
        filterKeys.Select(serviceProvider.GetRequiredKeyedService<IFilter<T>>);

    private static string GetUniqueFilterName(this string pipelineName, Type filterType)
        => $"{pipelineName}_{filterType.FullName}";
}