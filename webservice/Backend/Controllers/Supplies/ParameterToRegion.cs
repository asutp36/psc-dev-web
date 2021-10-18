using Backend.Controllers.Supplies.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies
{
    public static class ParameterToRegion<T>
    {
        public static List<RegionParameter<T>> WashesToRegion(List<WashParameter<T>> washes)
        {
            List<RegionParameter<T>> result = new List<RegionParameter<T>>();

            foreach (WashParameter<T> w in washes)
            {
                w.washName = SqlHelper.GetWashByCode(w.washCode).name;

                RegionViewModel region = SqlHelper.GetRegionByWash(w.washCode);
                RegionParameter<T> rrm = result.Find(x => x.regionCode == region.code);

                if (rrm == null)
                {
                    rrm = new RegionParameter<T>
                    {
                        regionCode = region.code,
                        regionName = region.name,
                        washes = new List<WashParameter<T>>()
                    };

                    rrm.washes.Add(w);
                }
                else
                {
                    result.Remove(rrm);
                    rrm.washes.Add(w);
                }

                result.Add(rrm);
            }
            return result;
        }

        public static RegionParameter<T> WashToRegion(WashParameter<T> wash)
        {
            RegionViewModel region = SqlHelper.GetRegionByWash(wash.washCode);
            RegionParameter<T> result = new RegionParameter<T>
            {
                regionCode = region.code,
                regionName = region.name,
                washes = new List<WashParameter<T>>()
            };

            result.washes.Add(wash);

            return result;
        }
    }
}
