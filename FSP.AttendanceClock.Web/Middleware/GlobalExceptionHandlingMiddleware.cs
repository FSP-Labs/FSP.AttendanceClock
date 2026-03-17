using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FSP.AttendanceClock.Web.Middleware
{
    /// <summary>
    /// Middleware para manejar excepciones globales y prevenir exposición de errores internos.
    /// </summary>
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {ExceptionMessage}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                Message = "An error occurred on the server.",
                StatusCode = context.Response.StatusCode
            };

            switch (exception)
            {
                case ArgumentNullException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid request.";
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "Unauthorized.";
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "Resource not found.";
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "An internal server error occurred. Please contact the administrator.";
                    break;
            }

            response.StatusCode = context.Response.StatusCode;
            return context.Response.WriteAsJsonAsync(response);
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
