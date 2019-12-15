using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR.Pipeline;

namespace WebShop.Api.Pipelines
{
    public class CommandValidationBehavior<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly IList<IValidator<TRequest>> _validators;

        public CommandValidationBehavior(IList<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var validationResults = _validators
                .Select(v => v.ValidateAsync(request, cancellationToken))
                .ToList();

            await Task.WhenAll(validationResults);

            var errors = validationResults.Where(x => x != null).SelectMany(x => x.Result.Errors);

            if (errors?.Any() == true)
            {
                var errorBuilder = new StringBuilder();

                errorBuilder.AppendLine("Invalid command, reason: ");

                foreach (var error in errors)
                {
                    errorBuilder.AppendLine(error.ErrorMessage);
                }

                throw new Exception(errorBuilder.ToString());
            }
        }
    }
}