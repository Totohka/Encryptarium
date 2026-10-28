namespace Encryptarium.Client;

/// <summary>
/// Статусы получения секрета
/// </summary>
public enum VaultOperationStatuses
{
    /// <summary>
    /// Успешно
    /// </summary>
    Success,

    /// <summary>
    /// Неавторизован
    /// </summary>
    Unauthorize,

    /// <summary>
    /// Нет доступа
    /// </summary>
    Forbidden,

    /// <summary>
    /// Ошибка клиента
    /// </summary>
    Error,

    /// <summary>
    /// Ошибка хранилища
    /// </summary>
    ErrorVault
}
