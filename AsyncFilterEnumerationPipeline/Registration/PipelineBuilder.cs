using Microsoft.Extensions.DependencyInjection;

namespace AsyncFilterEnumerationPipeline.Registration;

/// <summary>
/// Класс для изоляции пайплайнов
/// </summary>
public class PipelineBuilder<T>(
    IServiceCollection services,
    string name,
    ServiceLifetime pipeLineLifetime = ServiceLifetime.Transient,
    ServiceLifetime filterPipeline = ServiceLifetime.Transient)
{
    /// <summary>
    /// Имя пайплайна
    /// </summary>
    public string Name { get; } = name;
    
    public ServiceLifetime PipelineLifetime { get; } = pipeLineLifetime;
    public ServiceLifetime DefaultFilterLifetime { get; } = filterPipeline;
    
    /// <summary>
    /// <see cref="IServiceCollection"/>
    /// </summary>
    public IServiceCollection Services { get; } = services;
    
    /// <summary>
    /// Список ключей фильтров
    /// </summary>
    public List<string> FilterKeys { get; } = [];
}