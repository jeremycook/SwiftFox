namespace SwiftFox.Services
{
    /// <summary>
    /// Resolve a <typeparamref name="TValue"/>
    /// with mutations from implementations of <see cref="IValueMutator{TValue}"/>.
    /// The lifetime of <typeparamref name="TValue"/> is scoped because it is tied to the
    /// lifetime of the instance of the <see cref="ValueProvider{TValue}"/> that
    /// resolved it, which is scoped.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Service]
    public class ValueProvider<TValue>
        where TValue : new()
    {
        private readonly IEnumerable<IValueMutator<TValue>> valueMutators;

        private TValue? value = default;

        public ValueProvider(IEnumerable<IValueMutator<TValue>> valueMutators)
        {
            this.valueMutators = valueMutators;
        }

        public async Task<TValue> ResolveAsync()
        {
            if (value is null)
            {
                value = new();

                foreach (var mutator in valueMutators)
                {
                    await mutator.MutateAsync(value);
                }
            }

            return value;
        }
    }
}
