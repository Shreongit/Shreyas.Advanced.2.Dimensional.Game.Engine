using SA2DGE.Engine.Math;

namespace SA2DGE.Engine.Graphics.Shaders;

public abstract class ShaderProgram : IDisposable
{
    private readonly Dictionary<string, int> _uniformLocations = new();

    private bool _linked;
    private bool _disposed;

    public bool IsLinked => _linked;

    public void Attach(Shader shader)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(shader);

        if (!shader.IsCompiled)
        {
            throw new InvalidOperationException(
                "Shader must be compiled before it can be attached.");
        }

        OnAttach(shader);
    }

    public void Link()
    {
        ThrowIfDisposed();

        if (_linked)
        {
            return;
        }

        OnLink();

        _linked = true;
        _uniformLocations.Clear();
    }

    public void Use()
    {
        ThrowIfDisposed();
        EnsureLinked();

        OnUse();
    }

    public void StopUsing()
    {
        ThrowIfDisposed();

        OnStopUsing();
    }

    public void SetInt(
        string name,
        int value)
    {
        SetUniform(
            name,
            location => OnSetInt(location, value));
    }

    public void SetFloat(
        string name,
        float value)
    {
        SetUniform(
            name,
            location => OnSetFloat(location, value));
    }

    public void SetVector2(
        string name,
        Vector2 value)
    {
        SetUniform(
            name,
            location => OnSetVector2(location, value));
    }

    public void SetVector3(
        string name,
        Vector3 value)
    {
        SetUniform(
            name,
            location => OnSetVector3(location, value));
    }

    public void SetColor(
        string name,
        Color value)
    {
        SetUniform(
            name,
            location => OnSetColor(location, value));
    }

    public void SetMatrix3(
        string name,
        Matrix3 value)
    {
        SetUniform(
            name,
            location => OnSetMatrix3(location, value));
    }

    public void SetMatrix4(
        string name,
        Matrix4 value)
    {
        SetUniform(
            name,
            location => OnSetMatrix4(location, value));
    }

    protected int GetUniformLocation(
        string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        EnsureLinked();

        if (_uniformLocations.TryGetValue(
                name,
                out int location))
        {
            return location;
        }

        location = OnGetUniformLocation(name);

        _uniformLocations[name] = location;

        return location;
    }

    private void SetUniform(
        string name,
        Action<int> setter)
    {
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(setter);

        EnsureLinked();

        int location =
            GetUniformLocation(name);

        if (location < 0)
        {
            return;
        }

        setter(location);
    }

    protected abstract void OnAttach(
        Shader shader);

    protected abstract void OnLink();

    protected abstract void OnUse();

    protected abstract void OnStopUsing();

    protected abstract int OnGetUniformLocation(
        string name);

    protected abstract void OnSetInt(
        int location,
        int value);

    protected abstract void OnSetFloat(
        int location,
        float value);

    protected abstract void OnSetVector2(
        int location,
        Vector2 value);

    protected abstract void OnSetVector3(
        int location,
        Vector3 value);

    protected abstract void OnSetColor(
        int location,
        Color value);

    protected abstract void OnSetMatrix3(
        int location,
        Matrix3 value);

    protected abstract void OnSetMatrix4(
        int location,
        Matrix4 value);

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        OnDispose();

        _uniformLocations.Clear();

        _linked = false;
        _disposed = true;
    }

    protected virtual void OnDispose()
    {
    }

    protected void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }

    protected void EnsureLinked()
    {
        if (!_linked)
        {
            throw new InvalidOperationException(
                "Shader program has not been linked.");
        }
    }
}