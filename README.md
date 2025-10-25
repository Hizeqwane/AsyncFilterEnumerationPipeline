# Асинхронный пайплайн фильтров списков
Данная библиотека призвана облегчить работу с фильтрацией коллекций. Актуально для случаев, когда для отдельных фильтров пайплайна возможно выполнить какую-то
асинхронную операцию сразу для всей коллекции, например, при получении списка через итератор выполнить асинхронный запрос на коллекции (с возможным батчеванием, пагинацией и пр.).

# Основные возможности
- Поддерживает асинхронную фильтрацию при том, что каждый фильтр может выполнять асинхронные операции как на отдельном элементе, так и на батче (нескольких значениях) - построен на _IAsyncEnumerable_
- Поддерживает регистрацию нескольких пайплайнов с одинаковыми типами - используется механизм ключей (KeyedService)
- Содержит удобные методы регистрации в **DI** через builder
- Позволяет гибко настраивать жизненные циклы как для всего пайплайна, так и для отдельных фильтров в частности

# Регистрация в **DI**

```
_services.AddPipeline<string>("first")             // Регистрируем пайплайн; ключ опционален (если подразумевается несколько разных пайплайнов для string)
         .AddFilter<string, EvenFilter>()          // Регистрация отдельных фильтров в пайплайне (порядок важен)
         .AddFilter<string, FirstLetterAFilter>()  
         .Build();                                 // Завершение построения пайплайна
```

# Работа с пайплайном
## Объект пайплайна

Сам объект пайплайна представлен классом _Pipeline\<T\>_ с методом _Apply_:

```
Task<List<T>> Apply(IEnumerable<T> source, CancellationToken cancellationToken = default)
```

Таким образом пайплайн позволяет асинхронно получить список отфильтрованных значений.

## Получение экземпляра пайплайна

Для получения по ключу требуется вызвать метод GetKeyedService у _IServiceProvider_.
```
var firstPipeline = provider.GetKeyedService<Pipeline<string>>("first"); // provider - IServiceProvider
```

В случае, когда пайплайн с параметром единственный - можем получить пайплайн через конструктор (**DI**).

# Тесты
Проект содержит демо в виде тестов для двух строковых пайплайнов

```
_services = new ServiceCollection();

_services.AddPipeline<string>("first")
    .AddFilter<string, EvenFilter>()
    .AddFilter<string, FirstLetterAFilter>()
    .Build();

_services.AddPipeline<string>("second")
    .AddFilter<string, NotEvenFilter>()
    .AddFilter<string, FirstLetterIFilter>()
    .Build();

---

var firstPipeline = provider.GetKeyedService<Pipeline<string>>("first")!;
var secondPipeline = provider.GetKeyedService<Pipeline<string>>("second")!;

var firstResult = await firstPipeline.Apply(_strList);
Assert.That(firstResult, Is.EqualTo(_firstRightAnswer));

var secondResult = await secondPipeline.Apply(_strList);
Assert.That(secondResult, Is.EqualTo(_secondRightAnswer));
```
