namespace Swiftfox.Values
{
    public interface IValueMutator<TValue>
    {
        ValueTask MutateAsync(TValue value);
    }
}