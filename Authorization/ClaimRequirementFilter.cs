using MvcLaptop.Utils.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace MvcLaptop.Authorization
{
    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        private readonly FunctionCode _functionCode;
        private readonly CommandCode _commandCode;
        private readonly ILogger<ClaimRequirementFilter> _logger;

        public ClaimRequirementFilter(FunctionCode functionCode, CommandCode commandCode, ILogger<ClaimRequirementFilter> logger)
        {
            _functionCode = functionCode;
            _commandCode = commandCode;
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var permissionsClaim = context.HttpContext.User.Claims
                .SingleOrDefault(c => c.Type == SystemConstants.Claims.Permissions);

            if (permissionsClaim == null)
            {
                _logger.LogWarning("Claim Permissions not found.");
                context.Result = new ForbidResult();
                return;
            }

            try
            {
                var permissions = JsonConvert.DeserializeObject<List<string>>(permissionsClaim.Value);
                if (permissions != null)
                {
                    if (!permissions.Contains($"{_functionCode}_{_commandCode}"))
                    {
                        _logger.LogWarning($"Permissions: {string.Join(", ", permissions)}");
                        _logger.LogWarning($"Required: {_functionCode}_{_commandCode}");
                        context.Result = new ForbidResult();
                    }
                }
                else
                {
                    _logger.LogError("Deserialization failed.");
                    context.Result = new ForbidResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deserializing claim: {ex.Message}");
                context.Result = new ForbidResult();
            }
        }
    }
}
