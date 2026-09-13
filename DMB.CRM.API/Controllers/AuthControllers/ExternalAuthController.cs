using Dmb.Crm.Model.Dtos.Auth;
using Dmb.Crm.Service.Interface.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dmb.Crm.Api.Controllers.AuthControllers;

[ApiController]
[Route("api/auth/external")]
public class ExternalAuthController : ControllerBase
{
    private readonly IExternalAuthService _externalAuthService;
    private readonly IConfiguration _configuration;

    public ExternalAuthController(IExternalAuthService externalAuthService, IConfiguration configuration)
    {
        _externalAuthService = externalAuthService;
        _configuration = configuration;
    }

    [HttpGet("{provider}/start")]
    [AllowAnonymous]
    public IActionResult Start(string provider, [FromQuery] string? redirect)
    {
        var result = _externalAuthService.Start(provider, redirect, BuildCallbackUrl(provider));
        if (!string.IsNullOrWhiteSpace(result.RedirectUrl))
        {
            return Redirect(result.RedirectUrl);
        }

        return StatusCode(StatusCodes.Status503ServiceUnavailable, new
        {
            message = result.ErrorMessage ?? "Social sign-in is not available."
        });
    }

    [HttpGet("{provider}/callback")]
    [AllowAnonymous]
    public async Task<IActionResult> Callback(
        string provider,
        [FromQuery] string? code,
        [FromQuery] string? state,
        [FromQuery] string? error,
        [FromQuery(Name = "error_description")] string? errorDescription,
        CancellationToken cancellationToken)
    {
        var redirectUrl = await _externalAuthService.HandleCallbackAsync(
            provider,
            code,
            state,
            error,
            errorDescription,
            BuildCallbackUrl(provider),
            cancellationToken);
        return Redirect(redirectUrl);
    }

    [HttpPost("complete")]
    [AllowAnonymous]
    public async Task<IActionResult> Complete(
        [FromBody] ExternalAuthCompleteRequest request,
        CancellationToken cancellationToken)
    {
        var (ok, message, statusCode) = await _externalAuthService.CompleteAsync(request, cancellationToken);
        return StatusCode(statusCode, new { message });
    }

    [HttpPost("verify")]
    [AllowAnonymous]
    public async Task<IActionResult> Verify(
        [FromBody] ExternalAuthVerifyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _externalAuthService.VerifyAsync(request, cancellationToken);
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        }

        return Ok(new
        {
            token = result.AccessToken,
            locations = result.Locations,
            currentLocationId = result.CurrentLocationId
        });
    }

    private string BuildCallbackUrl(string provider)
    {
        var providerKey = provider.Trim().ToLowerInvariant();
        // Same domain as the marketing site, different path so Google / LinkedIn / Facebook
        // treat this as its own app callback and do not collide with /api/auth/external/...
        return $"https://www.dmbwebsolutions.com/crm/api/auth/external/{providerKey}/callback";
    }
}
