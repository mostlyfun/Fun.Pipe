namespace Fun;

/// <summary>
/// Immutable option type which can either be Some or None.
/// When the state <see cref="IsSome"/>, the option holds the valid value which can be extracted by <see cref="Unwrap()"/> (or <see cref="Unwrap(T)"/>) methods.
/// </summary>
public readonly struct Opt<T> : IEquatable<Opt<T>>
{
    // Data
    internal readonly T? value;
    /// <summary>
    /// True if the option is None.
    /// </summary>
    public bool IsNone
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !IsSome;
    }
    // Propcd 
    /// <summary>
    /// True if the option is Some value, which can be obtained by <see cref="Unwrap()"/> or <see cref="Unwrap(T)"/>.
    /// </summary>
    public readonly bool IsSome;


    // Ctor
    internal Opt(T value)
    {
        if (typeof(T).IsClass)
        {
            if (value == null)
            {
                IsSome = false;
                this.value = default;
            }
            else
            {
                IsSome = true;
                this.value = value;
            }
        }
        else
        {
            IsSome = true;
            this.value = value;
        }
    }
    /// <summary>
    /// Option type of <typeparamref name="T"/>: either None or Some value.
    /// Parameterless ctor returns None; use 'Fun.Extensions.Some' or `Fun.Extensions.None` to construct options.
    /// Better to add `using static Fun.Extensions` and use `Some` and `None` directly.
    /// </summary>
    public Opt()
    {
        IsSome = false;
        value = default;
    }
    /// <summary>
    /// Implicitly converts to <paramref name="value"/> into <see cref="Opt{T}"/>.Some(<paramref name="value"/>).
    /// </summary>
    public static implicit operator Opt<T>(T value)
        => new(value);


    // Method
    /// <summary>
    /// Returns the value when <see cref="IsSome"/>; or throws when <see cref="IsNone"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Unwrap()
    {
        if (IsNone)
            throw new ArgumentException("tried to unwrap None");
        return value!;
    }
    /// <summary>
    /// Returns the value when <see cref="IsSome"/>; or returns the <paramref name="fallbackValue"/> when <see cref="IsNone"/>.
    /// </summary>
    /// <param name="fallbackValue"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Unwrap(T fallbackValue)
        => IsNone ? fallbackValue : value!;
    /// <summary>
    /// Returns the value when <see cref="IsSome"/>; or returns <paramref name="lazyFallbackValue"/>() when <see cref="IsNone"/>.
    /// </summary>
    /// <param name="lazyFallbackValue"></param>
    public T Unwrap(Func<T> lazyFallbackValue)
        => IsNone ? lazyFallbackValue() : value!;
    /// <summary>
    /// Returns the value when <see cref="IsSome"/>; or returns <paramref name="lazyFallbackValue"/>() when <see cref="IsNone"/>.
    /// </summary>
    /// <param name="lazyFallbackValue"></param>
    public Task<T> Unwrap(Func<Task<T>> lazyFallbackValue)
        => IsNone ? lazyFallbackValue() : Task.FromResult(value!);
    /// <summary>
    /// Returns the value when <see cref="IsSome"/>; throws with the given <paramref name="errorMessage"/> when <see cref="IsNone"/>.
    /// </summary>
    public T UnwrapOrThrow(string errorMessage)
    {
        if (IsNone)
            throw new ArgumentException(errorMessage);
        return value!;
    }


    // Common
    /// <summary>
    /// Returns the text representation of the option.
    /// /// </summary>
    public override string ToString()
        => IsNone ? "None" : string.Format("Some({0})", value);
    /// <summary>
    /// Returns the text representation of the option; value will be <paramref name="format"/>ted when <see cref="IsSome"/>.
    /// </summary>
    public string ToString(string format)
    {
        if (IsNone)
            return "None";
        var method = typeof(T).GetMethod(nameof(ToString), new[] { typeof(string) });
        if (method != null)
            return string.Format("Some({0})", (string?)method.Invoke(value, new[] { format })); 
        else
            return string.Format("Some({0})", value);
    }
    /// <summary>
    /// Returns true if both values are <see cref="IsSome"/> and their unwrapped values are equal; false otherwise.
    /// </summary>
    public static bool operator ==(Opt<T> first, Opt<T> second)
        => first.IsSome && second.IsSome && first.value != null && second.value != null && first.value.Equals(second.value);
    /// <summary>
    /// Returns true if either value <see cref="IsNone"/> or their unwrapped values are not equal; false otherwise.
    /// </summary>
    public static bool operator !=(Opt<T> first, Opt<T> second)
        => !(first == second);
    /// <summary>
    /// Returns whether this option is equal to the <paramref name="obj"/>.
    /// </summary>
    public override bool Equals(object? obj)
        => obj != null && (obj is Opt<T>) && (this == (Opt<T>)obj);
    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    public override int GetHashCode()
        => value == null ? int.MinValue : value.GetHashCode();
    /// <summary>
    /// <inheritdoc cref="operator ==(Opt{T}, Opt{T})"/>
    /// </summary>
    public bool Equals(Opt<T> other)
        => this == other;
}
