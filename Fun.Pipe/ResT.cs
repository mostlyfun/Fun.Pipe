namespace Fun;

/// <summary>
/// Immutable result type which can either be Ok or Err.
/// When the state <see cref="IsOk"/>, the result holds the valid value which can be extracted by <see cref="Unwrap()"/> (or <see cref="Unwrap(T)"/>) methods.
/// When the state <see cref="IsErr"/>, the result further holds Some <see cref="ErrorMessage"/>.
/// </summary>
public readonly struct Res<T> : IEquatable<Res<T>>
{
    // Data
    internal readonly T? value;
    readonly string? errorMessage;
    // Prop
    /// <summary>
    /// True if the result is Ok; false otherwise.
    /// </summary>
    public bool IsOk
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => errorMessage == null;
    }
    /// <summary>
    /// True if the result is Err; false otherwise.
    /// </summary>
    public bool IsErr
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => errorMessage != null;
    }
    /// <summary>
    /// Returns the underlying error message if <see cref="IsErr"/>; None if <see cref="IsOk"/>;
    /// </summary>
    public Opt<string> ErrorMessage
        => errorMessage == null ? new() : new(errorMessage);


    // Ctor
    /// <summary>
    /// Parameterless ctor returns Err("not-initialized"), and hence, is not useful!
    /// Use 'Fun.Extensions.Ok' or `Fun.Extensions.Err` to construct options.
    /// Better to add `using static Fun.Extensions` and use `Ok` and `Err` directly.
    /// </summary>
    public Res()
    {
        errorMessage = "constructed with Res<T>(); rather than Ok(value) or Err(...)";
        value = default;
    }
    internal Res(T value)
    {
        if (typeof(T).IsClass)
        {
            if (value == null)
            {
                errorMessage = $"null-is-passed-in-Res<{typeof(T).Name}>";
                this.value = default;
            }
            else
            {
                this.value = value;
                errorMessage = null;
            }
        }
        else
        {
            this.value = value;
            errorMessage = null;
        }
    }
    internal Res(string? errorMessage, string? when)
    {
        value = default;
        this.errorMessage = Res.GetErrorMessage(errorMessage, when);
    }
    internal Res(Exception exception, string? when)
    {
        value = default;
        errorMessage = Res.GetExceptionMessage(exception, when);
    }
    /// <summary>
    /// Implicitly converts to <paramref name="value"/> into <see cref="Res{T}"/>.Ok(<paramref name="value"/>).
    /// </summary>
    public static implicit operator Res<T>(T value) => new(value);


    // Method
    /// <summary>
    /// Returns the value when <see cref="IsOk"/>; or throws when <see cref="IsErr"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Unwrap()
    {
        if (errorMessage != null)
            throw new ArgumentException("tried to unwrap None");
        return value!;
    }
    /// <summary>
    /// Returns the value when <see cref="IsOk"/>; or returns the <paramref name="fallbackValue"/> when <see cref="IsErr"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Unwrap(T fallbackValue)
        => errorMessage == null ? value! : fallbackValue;
    /// <summary>
    /// Returns the value when <see cref="IsOk"/>; or returns <paramref name="lazyFallbackValue"/>() when <see cref="IsErr"/>.
    /// </summary>
    /// <param name="lazyFallbackValue"></param>
    public T Unwrap(Func<T> lazyFallbackValue)
        => IsErr ? lazyFallbackValue() : value!;
    /// <summary>
    /// Returns the value when <see cref="IsOk"/>; or returns <paramref name="lazyFallbackValue"/>() when <see cref="IsErr"/>.
    /// </summary>
    /// <param name="lazyFallbackValue"></param>
    public Task<T> Unwrap(Func<Task<T>> lazyFallbackValue)
        => IsErr ? lazyFallbackValue() : Task.FromResult(value!);
    /// <summary>
    /// <inheritdoc cref="Res.MsgIfErr(string)"/>
    /// </summary>
    public Res<T> MsgIfErr(string errorMessage)
    {
        if (this.errorMessage == null)
            return this;
        string msg = this.errorMessage + " " + Res.GetErrorMessage(errorMessage, null);
        return new(msg, null);
    }
    /// <summary>
    /// <inheritdoc cref="Res.MsgIfErr(string, string)"/>
    /// </summary>
    public Res<T> MsgIfErr(string errorMessage, string when)
    {
        if (this.errorMessage == null)
            return this;
        string msg = this.errorMessage + " " + Res.GetErrorMessage(errorMessage, when);
        return new(msg, null);
    }


    // Common
    /// <summary>
    /// Returns the text representation of the option.
    /// </summary>
    public override string ToString()
        => IsOk ? $"Ok({value})" : $"Err({errorMessage})";
    /// <summary>
    /// Returns the text representation of the result; value will be <paramref name="format"/>ted when <see cref="IsOk"/>.
    /// </summary>
    /// <param name="format">Determines whether the error log wil be detailed or not.</param>
    public string ToString(string format)
    {
        if (IsErr)
            return $"Err({errorMessage})";
        var method = typeof(T).GetMethod(nameof(ToString), new[] { typeof(string) });
        if (method != null)
            return string.Format("Ok({0})", (string?)method.Invoke(value, new[] { format }));
        else
            return string.Format("Ok({0})", value);
    }
    /// <summary>
    /// Returns true if both values are <see cref="IsOk"/> and their unwrapped values are equal; false otherwise.
    /// </summary>
    public static bool operator ==(Res<T> first, Res<T> second)
        => first.value != null && second.value != null && first.value.Equals(second.value);
    /// <summary>
    /// Returns true if either value <see cref="IsErr"/> or their unwrapped values are not equal; false otherwise.
    /// </summary>
    public static bool operator !=(Res<T> first, Res<T> second)
        => first.value == null || second.value == null || !first.value.Equals(second.value);
    /// <summary>
    /// Returns whether this result is equal to the <paramref name="obj"/>.
    /// </summary>
    public override bool Equals(object? obj)
        => obj != null && (obj is Res<T> res) && (this == res);
    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    public override int GetHashCode()
        => errorMessage != null ? errorMessage.GetHashCode() : (value != null ? value.GetHashCode() : int.MaxValue);
    /// <summary>
    /// <inheritdoc cref="operator ==(Res{T}, Res{T})"/>
    /// </summary>
    public bool Equals(Res<T> other)
        => this == other;
}
