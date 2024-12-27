using Microsoft.AspNetCore.Mvc;

namespace MvcLaptop.Controllers.Components
{
    public class FooterViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult((IViewComponentResult)View("Default"));
        }
    }
}