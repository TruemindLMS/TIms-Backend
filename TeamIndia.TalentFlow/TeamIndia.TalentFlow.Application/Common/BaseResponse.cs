using System;
using System.Collections.Generic;

namespace TeamIndia.TalentFlow.Application.Common;

public class BaseResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public static BaseResponse Ok(string? message = null, int statusCode = 200)
        => new() { Success = true, Message = message, StatusCode = statusCode };

    public static BaseResponse Fail(string? message = null, IEnumerable<string>? errors = null, int statusCode = 400)
        => new() { Success = false, Message = message, Errors = errors, StatusCode = statusCode };
}

public class BaseResponse<T> : BaseResponse
{
    public T? Data { get; set; }

    public static BaseResponse<T> Ok(T data, string? message = null, int statusCode = 200)
        => new() { Success = true, Data = data, Message = message, StatusCode = statusCode };

    public static new BaseResponse<T> Fail(string? message = null, IEnumerable<string>? errors = null, int statusCode = 400)
        => new() { Success = false, Message = message, Errors = errors, StatusCode = statusCode };
}
