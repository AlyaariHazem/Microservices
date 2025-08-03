//using Microsoft.AspNetCore.Diagnostics;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;

//namespace BuildingBlocks.Exceptions.Handler
//{
//    public class CustomExceptionHandler
//        (ILogger<CustomExceptionHandler> logger)
//        : IExceptionHandler
//    {
//        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
//        {
//            logger.LogError(
//                "Error Message: {exceptionMessage}, Time of occurrence {time}",
//                 exception.Message, DateTime.UtcNow);
//            (string Detail, string Title, int StatusCode) details = exception switch
//            {
//                InternalServerException =>
//                (
//                 exception.Message,
//                 exception.GetType().Name,
//                 context.Response.StatusCode = StatusCodes.Status400BadRequest
//                ),
//                BadRequestException =>
//                (
//                exception.Message,
//                exception.GetType().Name,
//                context.Response.StatusCode = StatusCodes.Status400BadRequest
//                )

//            };
//        }
//    }
//}
