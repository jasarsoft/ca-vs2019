﻿using LoanManager.Core.DataInterface;
using LoanManager.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoanManager.Core.Services
{
    public class LoanProcessingService : ILoanProcessingService
    {
        private readonly List<ILoanQualificationRule> _loanApprovalRules;
        private readonly List<LoanRate> _loanRates;

        public LoanProcessingService(ILoanRateRepository loanRateRepository, List<ILoanQualificationRule> rules)
            : this(loanRateRepository.GetLoanRates(), rules.ToArray())
        {

        }

        public LoanProcessingService(List<LoanRate> rates, params ILoanQualificationRule[] rules)
        {
            _loanRates = rates;
            _loanApprovalRules = rules.ToList();
        }

        public LoanApplicationResult ProcessLoan(LoanApplication application)
        {
            // Check loan qualification rules
            List<ILoanQualificationRule> failingRules = _loanApprovalRules.Where
                (rule => rule.CheckLoanApprovalRule(application) == false).ToList();

            if (failingRules.Count > 0)
            {
                LoanApplicationResult result = LoanApplicationResult.CreateDeniedResult(application, failingRules);
                return result;
            }

            // Determine interest rate
            double interestRate = DetermineInterestRate(application);

            // Determine monthly payment
            double monthlyPayment = CalculateLoanPayment(loanAmount: application.LoanAmount,
                termYears: application.Term.Years, interestRate: interestRate);

            return LoanApplicationResult.CreateApprovedResult(application, interestRate, monthlyPayment);
        }

        private double DetermineInterestRate(LoanApplication application)
        {
            double interestRate;
            double creditScore = application.CreditScore;
            LoanRate rate = _loanRates.FirstOrDefault(r =>
                creditScore >= r.LowerCreditScore
                && creditScore <= r.UpperCreditScore);

            interestRate = rate.InterestRate;

            if (application.ApplicantType.ToLower() == "premiere")
            {
                interestRate = rate.InterestRate - .01;
            }

            return interestRate;
        }

        internal double CalculateLoanPayment(double loanAmount, int termYears, double interestRate)
        {
            int totalPayments = termYears * 12;
            double monthlyInterest = interestRate / 12.0;
            double discountFactor = ((Math.Pow((1 + monthlyInterest), totalPayments)) - 1.0) /
                (monthlyInterest * Math.Pow((1 + monthlyInterest), totalPayments));

            double monthlyPayment = Math.Round(loanAmount / discountFactor, 2);
            return monthlyPayment;
        }
    }
}
