using MvcLaptop.Models;
using Microsoft.AspNetCore.Mvc;

namespace MvcLaptop.Controllers.Components;

public class PagerViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(PaginationBase result)
    {
        return Task.FromResult((IViewComponentResult)View("Default", result));
    }
}