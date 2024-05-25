using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Collections.Specialized.BitVector32;

namespace www.Pages
{
    public class MedicoGerenciamentoModel : PageModel
    {
        public string ActivePage { get; private set; }

        public void OnGet()
        {
            ActivePage = "MedicoGerenciamento";
        }

    }
}




