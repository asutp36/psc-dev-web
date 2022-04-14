﻿using GateWashDataService.Models;
using GateWashDataService.Models.Filters;
using GateWashDataService.Models.GateWashContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Helpers
{
    public class SqlHelper
    {
        private static readonly List<string> bankIncreaseEventkinds = new List<string>() { "cardincrease", "bankincrease", "qrincrease" };
        private static readonly List<string> cashIncreaseEventkinds = new List<string>() { "cashincrease" };

        public static List<IncreaseModel> GetIncreases(GateWashDbContext context, GetIncreaseParameters param)
        {
            return context.PaySessions.Where(s => s.DtimeBegin >= param.StartDate && s.DtimeBegin <= param.EndDate && (!param.OnlyNotes || s.Details != null)
                                            && (param.Terminal == null || s.IddeviceNavigation.Code == param.Terminal)
                                            && (param.Program == null || s.IdfunctionNavigation.Code == param.Program))
                                      .Select(s => new IncreaseModel
                                      {
                                          DTime = s.DtimeBegin,
                                          Terminal = s.IddeviceNavigation.Code,
                                          Program = s.IdfunctionNavigation.Code,
                                          Bank = s.PayEvents.Where(e => bankIncreaseEventkinds.Contains(e.IdeventKindNavigation.Code)).Sum(e => e.EventIncrease.Amount) ?? 0,
                                          Cash = s.PayEvents.Where(e => cashIncreaseEventkinds.Contains(e.IdeventKindNavigation.Code)).Sum(e => e.EventIncrease.Amount) ?? 0,
                                          Payout = s.PayEvents.Sum(e => e.EventPayout.Amount),
                                          Cheque = s.Qr != null && s.Qr != "",
                                          Note = s.Details
                                      })
                                      .Where(i => (!param.OnlyBank || i.Bank != 0) && (!param.OnlyCash || i.Cash != 0))
                                      .ToList();
        }

        public static async Task<List<RegionModel>> GetRegions(GateWashDbContext context)
        {
            return context.Regions.Select(r => new RegionModel
            {
                IdRegion = r.Idregion,
                Code = r.Code,
                Name = r.Name
            }).ToList();
        }

        public static async Task<List<WashModel>> GetWashes(GateWashDbContext context)
        {
            return context.Washes.Select(w => new WashModel
            {
                IdWash = w.Idwash,
                Code = w.Code,
                Name = w.Name,
                IdRegion = w.Idregion
            }).ToList();
        }

        public static async Task<List<PayTerminalModel>> GetPayTerminals(GateWashDbContext context)
        {
            return context.Posts.Where(p => p.IddeviceNavigation.IddeviceTypeNavigation.Code == "pay").Select(t => new PayTerminalModel()
            {
                IdDevice = t.Iddevice,
                Code = t.IddeviceNavigation.Code,
                Name = t.IddeviceNavigation.Name,
                IdWash = t.Idwash
            }).ToList();
        }
    }
}
