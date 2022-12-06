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
    public class CardsController : ControllerBase
    {
        private readonly CardService _cardService;

        public CardsController(CardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet("refills")]
        public async Task<IActionResult> GetRefills([FromQuery]GetCardsRefillParameters parameters)
        {
            List<CardsRefill> refills = await _cardService.GetCardCountIncreaseAsync(parameters);

            string sortingRule;
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc,TerminalCode asc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction},Dtime desc,TerminalCode asc";

            refills = Sorting.Sort<CardsRefill>(refills.AsQueryable(), sortingRule).ToList();

            PagedList<CardsRefill> result = PagedList<CardsRefill>.ToPagedList(refills.AsQueryable(), parameters.Paging);

            return Ok(result);
        }

        [HttpGet("refills/total-count")]
        public async Task<IActionResult> GetRefillsTotalCount([FromQuery] GetCardsRefillParameters parameters)
        {
            List<CardsRefill> refills = await _cardService.GetCardCountIncreaseAsync(parameters);
            return Ok(refills.Count);
        }

        [HttpGet("issuances")]
        public async Task<IActionResult> GetIssuances([FromQuery] GetCardsRefillParameters parameters)
        {
            List<CardIssuance> refills = await _cardService.GetCardIssuanceAsync(parameters);

            string sortingRule;
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc,TerminalCode asc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction},Dtime desc,TerminalCode asc";

            refills = Sorting.Sort<CardIssuance>(refills.AsQueryable(), sortingRule).ToList();

            PagedList<CardIssuance> result = PagedList<CardIssuance>.ToPagedList(refills.AsQueryable(), parameters.Paging);

            return Ok(result);
        }

        [HttpGet("issuances/total-count")]
        public async Task<IActionResult> GetIssuancesTotalCount([FromQuery] GetCardsRefillParameters parameters)
        {
            List<CardIssuance> refills = await _cardService.GetCardIssuanceAsync(parameters);
            return Ok(refills.Count);
        }
    }
}
