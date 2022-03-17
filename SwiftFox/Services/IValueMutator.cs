namespace SwiftFox.Services
{
    public interface IValueMutator<TValue>
    {
        ValueTask MutateAsync(TValue value);
    }
}