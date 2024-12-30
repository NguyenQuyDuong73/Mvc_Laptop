using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MockHttpSession : ISession
{
    private readonly Dictionary<string, object> _sessionStorage = new Dictionary<string, object>();

    public IEnumerable<string> Keys => _sessionStorage.Keys;

    public bool IsAvailable => true;

    public string Id => "MockSession";

    public void Clear()
    {
        _sessionStorage.Clear();
    }

    public Task CommitAsync(System.Threading.CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task LoadAsync(System.Threading.CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        _sessionStorage.Remove(key);
    }

    public void Set(string key, byte[] value)
    {
        _sessionStorage[key] = value;
    }

    public bool TryGetValue(string key, out byte[] value)
    {
        if (_sessionStorage.TryGetValue(key, out var objValue) && objValue is byte[] byteValue)
        {
            value = byteValue;
            return true;
        }

        value = null;
        return false;
    }

    public string GetString(string key)
    {
        return _sessionStorage.TryGetValue(key, out var value) && value is string strValue ? strValue : null;
    }

    public void SetString(string key, string value)
    {
        _sessionStorage[key] = value;
    }
}
