﻿using GateWashDataService.Extentions;
using GateWashDataService.Models;
using GateWashDataService.Models.Filters;
using GateWashDataService.Services;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("filters")]
        [Authorize]
        public async Task<IActionResult> GetFilters()
        {
            PayoutFilters filters = await _payoutService.GetFiltersAsync(User.Claims.Where(c => c.Type == "GateWash" || c.Type == "RobotWash").Select(c => c.Value));

            return Ok(filters);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] GetPayoutsParameters parameters)
        {
            IEnumerable<string> washes = User.Claims.Where(c => c.Type == "GateWash" || c.Type == "RobotWash").Select(c => c.Value);

            List<Payout> payouts = new List<Payout>();
            if (parameters.IsSplitTerminals) 
            {
               payouts = await _payoutService.GetPayoutsSplitTerminalsAsync(washes, parameters);
            }
            else
            {
                payouts = await _payoutService.GetPayoutsAsync(washes, parameters);
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
            IEnumerable<string> washes = User.Claims.Where(c => c.Type == "GateWash" || c.Type == "RobotWash").Select(c => c.Value);

            List<Payout> payouts = new List<Payout>();
            if (parameters.IsSplitTerminals)
            {
                payouts = await _payoutService.GetPayoutsSplitTerminalsAsync(washes, parameters);
            }
            else
            {
                payouts = await _payoutService.GetPayoutsAsync(washes, parameters);
            }

            return Ok(payouts.Count);
        }

        [HttpGet("refills")]
        public async Task<IActionResult> GetRefills([FromQuery] GetPayoutsParameters parameters)
        {
            IEnumerable<string> washes = User.Claims.Where(c => c.Type == "GateWash" || c.Type == "RobotWash").Select(c => c.Value);

            List<PayoutInsertion> payouts = new List<PayoutInsertion>();
            if (parameters.IsSplitTerminals)
            {
                payouts = await _payoutService.GetPayoutInsertionsSplitTerminalAsync(washes, parameters);
            }
            else
            {
                payouts = await _payoutService.GetPayoutInsertionsAsync(washes, parameters);
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
        public async Task<IActionResult> GetRefillsTotalCount([FromQuery] GetPayoutsParameters parameters)
        {
            IEnumerable<string> washes = User.Claims.Where(c => c.Type == "GateWash" || c.Type == "RobotWash").Select(c => c.Value);

            List<PayoutInsertion> payouts = new List<PayoutInsertion>();
            if (parameters.IsSplitTerminals)
            {
                payouts = await _payoutService.GetPayoutInsertionsSplitTerminalAsync(washes, parameters);
            }
            else
            {
                payouts = await _payoutService.GetPayoutInsertionsAsync(washes,  parameters);
            }

            return Ok(payouts.Count);
        }
    }
}