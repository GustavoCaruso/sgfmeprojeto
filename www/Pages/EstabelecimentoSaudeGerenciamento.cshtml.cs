using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace www.Pages
{
    public class EstabelecimentoSaudeGerenciamentoModel : PageModel
    {
        public string ActivePage { get; private set; }

        public void OnGet()
        {
            ActivePage = "EstabelecimentoSaudeGerenciamento";
        }
    }
}
