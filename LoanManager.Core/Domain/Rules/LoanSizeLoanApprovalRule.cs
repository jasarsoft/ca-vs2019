using LoanManager.Core.DataInterface;
using System;

namespace LoanManager.Core.Domain
{
    public class LoanSizeLoanApprovalRule : ILoanQualificationRule
    {
        public const String RULE_NAME = "Loan Size";

        public string RuleName { get => RULE_NAME; }

        public bool CheckLoanApprovalRule(LoanApplication application)
        {
            double loanAmount = application.LoanAmount;

            if (loanAmount is double n1 && (n1 <= 10_000))
            // We don't issue loans less than $50,000
                return false;
            else if (loanAmount is double n2 && (n2 > 10_000 && n2 < 1_000_000))
            // Loans from $50,000 to $1,000,000 are OK
                return true;
            else if (loanAmount is double n3 && (n3 > 1_000_000))
            // Do not issue loans over $1,000,000
                return false;
            else
                return false;
        }


    }
}
