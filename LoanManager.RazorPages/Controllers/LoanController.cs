using LoanManager.Core.DataInterface;
using LoanManager.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace LoanManager.RazorPages.Controllers
{
    public class LoanController : Controller
    {
        private readonly ILoanApplicationResultRepository repo;

        public LoanController(ILoanApplicationResultRepository repo)
        {
            this.repo = repo;
        }

        [HttpGet("loans")]
        public IActionResult Index(int start, int length = 2)
        {

            List<LoanApplicationResult> loanResults = this.repo.GetLoanApplicationResults();
            int totalRecords = loanResults.Count;

            List<LoanApplicationResult> filteredLoanResults = loanResults.Skip(start).Take(length).ToList();

            var response = new
            {
                recordsFiltered = totalRecords,
                recordsTotal = totalRecords,
                data = filteredLoanResults
            };

            return Ok(response);
        }

        [HttpGet("loans/{id}")]
        public IActionResult Index(int id)
        {
            LoanApplicationResult loan = this.repo.GetLoan(id);
            return View(loan);
        }
    }
}
