using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CardWebSocks.Pages
{
    public class CCreatorModel : PageModel
    {
        private readonly ILogger<CCreatorModel> _logger;

        public CCreatorModel(ILogger<CCreatorModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
