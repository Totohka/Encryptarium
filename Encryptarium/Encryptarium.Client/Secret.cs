using Encryptarium.Domain.DTO.Secrets;

namespace Encryptarium.Client;

public class Secret
{
    /// <summary>
    /// Секрет
    /// </summary>
    public SecretDto? SecretDto { get; set; }

    /// <summary>
    /// Статус получения секрета
    /// </summary>
    public VaultOperationStatuses Status { get; set; }
}
