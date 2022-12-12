using GateWashDataService.Extentions;
using GateWashDataService.Models;
using GateWashDataService.Repositories;
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
    public class CardsController : ControllerBase
    {
        private readonly CardService _cardService;
        private readonly WashesRepository _washesRepository;

        public CardsController(CardService cardService, WashesRepository washesRepository)
        {
            _cardService = cardService;
            _washesRepository = washesRepository;
        }

        [HttpGet("refills")]
        [Authorize]
        public async Task<IActionResult> GetRefills([FromQuery]GetCardsRefillParameters parameters)
        {
            IEnumerable<string> terminals = await _washesRepository.GetTerminalCodesByWashesAsync(User.Claims.Where(c => c.Type == "GateWash").Select(c => c.Value));
            List<CardsRefill> refills = await _cardService.GetCardCountIncreaseAsync(parameters, terminals);

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
        [Authorize]
        public async Task<IActionResult> GetRefillsTotalCount([FromQuery] GetCardsRefillParameters parameters)
        {
            IEnumerable<string> terminals = await _washesRepository.GetTerminalCodesByWashesAsync(User.Claims.Where(c => c.Type == "GateWash").Select(c => c.Value));
            List<CardsRefill> refills = await _cardService.GetCardCountIncreaseAsync(parameters, terminals);
            return Ok(refills.Count);
        }

        [HttpGet("issuances")]
        [Authorize]
        public async Task<IActionResult> GetIssuances([FromQuery] GetCardsRefillParameters parameters)
        {
            IEnumerable<string> terminals = await _washesRepository.GetTerminalCodesByWashesAsync(User.Claims.Where(c => c.Type == "GateWash").Select(c => c.Value));
            List<CardIssuance> refills = await _cardService.GetCardIssuanceAsync(parameters, terminals);

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
        [Authorize]
        public async Task<IActionResult> GetIssuancesTotalCount([FromQuery] GetCardsRefillParameters parameters)
        {
            IEnumerable<string> terminals = await _washesRepository.GetTerminalCodesByWashesAsync(User.Claims.Where(c => c.Type == "GateWash").Select(c => c.Value));
            List<CardIssuance> refills = await _cardService.GetCardIssuanceAsync(parameters, terminals);
            return Ok(refills.Count);
        }
    }
}
