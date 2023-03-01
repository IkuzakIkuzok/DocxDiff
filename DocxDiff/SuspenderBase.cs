
// (c) 2023 Kazuki KOHZUKI

namespace DocxDiff;

internal abstract class SuspenderBase : IDisposable
{
    private bool _disposed;

    public void Dispose()
        => Dispose(true);

    private void Dispose(bool disposing)
    {
        if (this._disposed) return;
        if (!disposing) return;

        Terminate();
        this._disposed = true;
    } // private void Dispose (bool disposing)

    protected abstract void Terminate();
} // internal abstract class SuspenderBase : IDisposable