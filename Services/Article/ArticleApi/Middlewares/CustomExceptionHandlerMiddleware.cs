using Application.Exceptions;
using ArticleApi.Common;
using System.Net;
using System.Text.Json;

namespace ArticleApi.Middlewares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ErrorCodeMaps _errorCodeMaps;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ErrorCodeMaps errorCodeMaps)
        {
            _next = next;
            _errorCodeMaps = errorCodeMaps;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var knownEx = GetKnownException(ex);

                await HandleKnownException(context, knownEx.Result, knownEx.Code);
            }
        }

        private async ValueTask HandleKnownException(HttpContext context, string result, HttpStatusCode code)
        {
            if (result != string.Empty)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)code;
                await context.Response.WriteAsync(result);
            }
        }

        private (string Result, HttpStatusCode Code) GetKnownException(Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            string result;
            string exceptionType;

            switch (exception)
            {
                case ValidationException validationException:
                    code = HttpStatusCode.BadRequest;
                    exceptionType = validationException.GetType().Name;
                    result = JsonSerializer.Serialize(new DefaultExceptionDto
                    {
                        Errors = validationException.Failures,
                        ErrorCode = _errorCodeMaps.MapDic.ContainsKey(exceptionType) ? _errorCodeMaps.MapDic[exceptionType] : 0
                    });
                    break;
                case BaseNotFoundException baseNotFoundException:
                    code = HttpStatusCode.NotFound;
                    exceptionType = baseNotFoundException.GetType().Name;

                    result = JsonSerializer.Serialize(new DefaultExceptionDto
                    {
                        Errors = new List<string>
                        {
                            baseNotFoundException.Message
                        },
                        ErrorCode = _errorCodeMaps.MapDic.ContainsKey(exceptionType) ? _errorCodeMaps.MapDic[exceptionType] : 0
                    });
                    break;
                case WrongExpectedVersionException wrongExpectedVersionException:
                    code = HttpStatusCode.BadRequest;
                    exceptionType = wrongExpectedVersionException.GetType().Name;

                    result = JsonSerializer.Serialize(new DefaultExceptionDto
                    {
                        Errors = new List<string>
                        {
                            wrongExpectedVersionException.Message
                        },
                        ErrorCode = _errorCodeMaps.MapDic.ContainsKey(exceptionType) ? _errorCodeMaps.MapDic[exceptionType] : 0
                    });
                    break;
                default:
                    result = JsonSerializer.Serialize(new DefaultExceptionDto
                    {
                        Errors = new List<string>
                        {
                            exception.Message
                        },
                        ErrorCode = _errorCodeMaps.MapDic.ContainsKey("UnknownException") ? _errorCodeMaps.MapDic["UnknownException"] : 0
                    });

                    break;
            }

            return (result, code);
        }
    }
}
