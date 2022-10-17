namespace SciMaterials.Contracts.Database.Initialization;

public interface IDbInitializer
{
    /// <summary> Асинхронно удаляет базу данных, если она существует. </summary>
    /// <param name="Cancel">Распространяет уведомление о том, что операции следует отменить. <see cref="CancellationToken"/></param>
    /// <returns>Создает Task с типом параметра <typeparam name="TResult">bool</typeparam>, которая завершается удачно с указанным результатом. См. <see cref="Task"/></returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<bool> DeleteDbAsync(CancellationToken Cancel = default);

    /// <summary> Асинхронно инициализирует новую базу данных. </summary>
    /// <param name="RemoveAtStart">Указывает, стоит ли удалять базу данных при инициализации. Значение по умолчанию: <value>false</value></param>
    /// <param name="UseDataSeeder">Указывает, нужно ли заполнять таблицы базы данных тестовыми данными. Значение по умолчанию: <value>false</value></param>
    /// <param name="Cancel">Распространяет уведомление о том, что операции следует отменить. <see cref="CancellationToken"/> Значение по умолчанию: <value>default</value></param>
    /// <returns>Задача, которая представляет работу в очереди на выполнение в ThreadPool. См. <see cref="Task"/></returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task InitializeDbAsync(bool RemoveAtStart = false, bool UseDataSeeder = false, CancellationToken Cancel = default);
}