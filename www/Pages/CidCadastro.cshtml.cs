using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SGFME.Application.Models;

namespace www.Pages
{
    public class CidCadastroModel : PageModel
    {
        public string ActivePage { get; private set; }
       
        public void OnGet()
        {
            ActivePage = "CidCadastro";
        }
    }
}
