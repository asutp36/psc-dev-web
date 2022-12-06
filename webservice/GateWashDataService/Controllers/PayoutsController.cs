using GateWashDataService.Extentions;
using GateWashDataService.Models;
using GateWashDataService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayoutsController : ControllerBase
    {
        private readonly PayoutService _payoutService;

        public PayoutsController(PayoutService payoutService)
        {
            _payoutService = payoutService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetPayoutsParameters parameters)
        {
            List<Payout> payouts = new List<Payout>();
            if (parameters.IsSplitTerminals) 
            {
               payouts = await _payoutService.GetPayoutsSplitTerminalsAsync(parameters);
            }
            else
            {
                payouts = await _payoutService.GetPayoutsAsync(parameters);
            }

            string sortingRule;
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc,TerminalCode asc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction},Dtime desc,TerminalCode asc";

            payouts = Sorting.Sort<Payout>(payouts.AsQueryable(), sortingRule).ToList();

            PagedList<Payout> result = PagedList<Payout>.ToPagedList(payouts.AsQueryable(), parameters.Paging);

            return Ok(result);
        }

        [HttpGet("total_count")]
        public async Task<IActionResult> GetTotalCount([FromQuery] GetPayoutsParameters parameters)
        {
            List<Payout> payouts = new List<Payout>();
            if (parameters.IsSplitTerminals)
            {
                payouts = await _payoutService.GetPayoutsSplitTerminalsAsync(parameters);
            }
            else
            {
                payouts = await _payoutService.GetPayoutsAsync(parameters);
            }

            return Ok(payouts.Count);
        }

        [HttpGet("refills")]
        public async Task<IActionResult> GetInsertions([FromQuery] GetPayoutsParameters parameters)
        {
            List<PayoutInsertion> payouts = new List<PayoutInsertion>();
            if (parameters.IsSplitTerminals)
            {
                payouts = await _payoutService.GetPayoutInsertionsSplitTerminalAsync(parameters);
            }
            else
            {
                payouts = await _payoutService.GetPayoutInsertionsAsync(parameters);
            }

            string sortingRule;
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc,TerminalCode asc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction},Dtime desc,TerminalCode asc";

            payouts = Sorting.Sort<PayoutInsertion>(payouts.AsQueryable(), sortingRule).ToList();

            PagedList<PayoutInsertion> result = PagedList<PayoutInsertion>.ToPagedList(payouts.AsQueryable(), parameters.Paging);

            return Ok(result);
        }

        [HttpGet("refills/total_count")]
        public async Task<IActionResult> GetInsertionsTotalCount([FromQuery] GetPayoutsParameters parameters)
        {
            List<PayoutInsertion> payouts = new List<PayoutInsertion>();
            if (parameters.IsSplitTerminals)
            {
                payouts = await _payoutService.GetPayoutInsertionsSplitTerminalAsync(parameters);
            }
            else
            {
                payouts = await _payoutService.GetPayoutInsertionsAsync(parameters);
            }

            return Ok(payouts.Count);
        }
    }
}
