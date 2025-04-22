using Application.Messages;
using Domain.Exceptions;
using Domain.Models.Common;
using System.Net;

namespace Application.Middlewares
{
    public class GlobalException : IMiddleware
    {

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            } catch (NotFoundException ex) {
                await HandleNotFoundException(context, ex);
            } catch (Domain.Exceptions.ValidationException ex) 
            { 
                await HandleValidationExceptionInDomain(context, ex);
            } catch (FluentValidation.ValidationException ex)
            {
                await HandleValidationException(context, ex);
            } catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private Task HandleValidationExceptionInDomain(HttpContext context, Domain.Exceptions.ValidationException ex)
        {
            var errors = ex.Errors.Select(x => new ErrorValidation()
            {
                PropertyName = x.Key,
                ErrorMessage = x.Value
            }).ToArray();
            int statusCode = (int)HttpStatusCode.BadRequest;
            var errorResponse = new APIResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Message = MessageCommon.SomethingErrors,
                Data = errors
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(errorResponse.ToString()!);
        }

        private Task HandleNotFoundException(HttpContext context, NotFoundException ex)
        {
            var errorResponse = new APIResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Message = MessageCommon.SomethingErrors,
                Data = ex.Message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ((int)HttpStatusCode.BadRequest);

            return context.Response.WriteAsync(errorResponse.ToString());
        }

        private Task HandleValidationException(HttpContext context, FluentValidation.ValidationException ex)
        {
            var errors = ex.Errors.Select(x => new ErrorValidation()
            {
                PropertyName = x.PropertyName,
                ErrorMessage = x.ErrorMessage
            }).ToArray();
            int statusCode = (int)HttpStatusCode.BadRequest;
            var errorResponse = new APIResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Message = MessageCommon.SomethingErrors,
                Data = errors
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(errorResponse.ToString()!);
        }

        private Task HandleException(HttpContext context, Exception ex)
        {
            var methodError = ex.TargetSite?.DeclaringType?.FullName;
         
            var errorResponse = new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = ex.Message,
                Data = ex.GetType().ToString()
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)errorResponse.Status;

            return context.Response.WriteAsync(errorResponse.ToString());
        }
    }

    public static class ExceptionExtention
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalException>();
        }
    }
}
