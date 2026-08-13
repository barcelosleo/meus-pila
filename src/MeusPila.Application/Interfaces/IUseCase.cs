namespace MeusPila.Application.Interfaces;

/// <summary>
/// Represents a use case interface for executing operations with input and output data transfer objects (DTOs).
/// </summary>
/// <typeparam name="TInputDto">The type of the input data transfer object (DTO).</typeparam>
/// <typeparam name="TOutputDto">The type of the output data transfer object (DTO).</typeparam>
public interface IUseCase<TInputDto, TOutputDto>
    where TInputDto : class
    where TOutputDto : class
{
    /// <summary>
    /// Executes the use case with the specified input data transfer object (DTO) asynchronously.
    /// </summary>
    /// <param name="input">The input data transfer object (DTO).</param>
    /// <returns>The output data transfer object (DTO) resulting from the execution.</returns>
    public Task<TOutputDto> ExecuteAsync(TInputDto input);
}
