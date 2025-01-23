using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NSE.Identity.API.Controllers;

[ApiController]
public abstract class MainController : ControllerBase
{
    protected ICollection<string> Errors = [];

    protected ActionResult CustomResponse(object? result = null)
    {
        if (ValidOperation())
        {
            return Ok(result);
        }

        return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Messages", Errors.ToArray() }
        }));
    }

    protected ActionResult CustomResponse(ModelStateDictionary modelState)
    {
        var errors = modelState.Values.SelectMany(e => e.Errors);

        foreach (var error in errors)
        {
            AddProcessingError(error.ErrorMessage);
        }

        return CustomResponse();
    }

    protected bool ValidOperation()
    {
        return Errors.Count == 0;
    }

    protected void AddProcessingError(string error)
    {
        Errors.Add(error);
    }

    protected void ClearProcessingErrors()
    {
        Errors.Clear();
    }
}
