using Microsoft.AspNetCore.Http;
using System.Text.Json;

public static class SessionExtensions
{
    // Lưu đối tượng vào session dưới dạng chuỗi JSON
    public static void SetObject(this ISession session, string key, object value)
    {
        var json = JsonSerializer.Serialize(value);  // Serialize đối tượng thành chuỗi JSON
        session.SetString(key, json);  // Lưu chuỗi JSON vào session
    }

    // Lấy đối tượng từ session và deserialize từ chuỗi JSON
    public static T? GetObject<T>(this ISession session, string key)
    {
        var json = session.GetString(key);  // Lấy chuỗi JSON từ session
        return json == null ? default(T) : JsonSerializer.Deserialize<T>(json);  // Deserialize chuỗi JSON thành đối tượng
    }
}
