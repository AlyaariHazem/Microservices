using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Behaviors
{
    internal class LoggingBehavior<TRequest, TResponse>
        (ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation("[START] Handle request: {Request} - response= {Response}",
                typeof(TRequest).Name, typeof(TResponse).Name);
            var timer = new Stopwatch();
            timer.Start();
            var response = next();
            timer.Stop();
            var timerTicks = timer.Elapsed;
            logger.LogInformation("[END] Handle request: {Request} - response= {Response} - elapsed time: {ElapsedTime}",
                typeof(TRequest).Name, typeof(TResponse).Name, timerTicks);
            return response;
        }
    }
}
