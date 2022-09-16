using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
using GateWashDataService.Models.GateWashContext.StoredProcedures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Repositories
{
    public class GraphicsRepository
    {
        private readonly GateWashDbContext _model;
        private readonly WashesRepository _washesRepository;

        public GraphicsRepository(GateWashDbContext model, WashesRepository washesRepository)
        {
            _model = model;
            _washesRepository = washesRepository;
        }

        /// <summary>
        /// Данные для графика по каждому внесению из хранимки GetCommulativeIncreasesSplitTerminals
        /// </summary>
        /// <param name="dtimeStart">Начало периода</param>
        /// <param name="dtimeEnd">Конец периода</param>
        /// <param name="washCodes">Коды моек</param>
        /// <param name="eventKindCode">Код типа внесения (optional)</param>
        /// <returns>Данные для отображения графика</returns>
        public async Task<GraphicsDataModel> GetCommulativeTotalSplitTerminalsGrapgicDataAsync(DateTime dtimeStart, DateTime dtimeEnd, IEnumerable<string> washCodes, string eventKindCode = null)
        {
            var dataToMakeGraphic = await GetCommulativeTotalSplitTerminalsAsync(dtimeStart, dtimeEnd, washCodes, eventKindCode);

            GraphicsDataModel graphic = new GraphicsDataModel();
            graphic.Labels = await dataToMakeGraphic.Select(o => o.DTime).Distinct().ToListAsync();
            List<string> datasetLabels = await dataToMakeGraphic.Select(o => o.Terminal).Distinct().ToListAsync();
            graphic.Datasets = new List<Dataset>();

            foreach (string label in datasetLabels)
            {
                graphic.Datasets.Add(new Dataset() { Data = new List<int>(), Label = label });
            }

            foreach (DateTime dt in graphic.Labels)
            {
                var pointXAxis = await dataToMakeGraphic.Where(o => (o.DTime - dt).Duration() <= TimeSpan.Parse("00:00:01")).ToListAsync();
                foreach (Dataset dataset in graphic.Datasets)
                {
                    var val = pointXAxis.Find(o => o.Terminal == dataset.Label);
                    if (val != null)
                    {
                        dataset.Data.Add(val.Amount);
                    }
                    else
                    {
                        if (dataset.Data.Count == 0)
                        {
                            dataset.Data.Add(0);
                        }
                        else
                        {
                            dataset.Data.Add(dataset.Data.Last());
                        }
                    }
                }
            }

            return graphic;
        }

        /// <summary>
        /// Получить результат хранимой процедуры GetCommulativeIncreasesSplitTerminals
        /// </summary>
        /// <param name="param"></param>
        /// <param name="washes"></param>
        /// <returns></returns>
        private async Task<IQueryable<IncreaseCommulativeTotalModel>> GetCommulativeTotalSplitTerminalsAsync(DateTime dtimeStart, DateTime dtimeEnd, IEnumerable<string> washes, string eventKindCode = null)
        {
            string terminalCodes = "";
            var terminals = await _washesRepository.GetTerminalsByWashesAsync(washes);
            terminalCodes = string.Join(", ", terminals);

            SqlParameter p_DTimeBegin = new SqlParameter("@p_DTimeBegin", dtimeStart);
            SqlParameter p_DTimeEnd = new SqlParameter("@p_DTimeEnd", dtimeEnd);
            SqlParameter p_TerminalCode = new SqlParameter("@p_TerminalCode", terminalCodes);
            SqlParameter p_IncreaseKind = new SqlParameter("@p_IncreaseKind", eventKindCode ?? "");

            var res = _model.Set<spGetCommulativeTotalSplitTerminals_Result>()
                .FromSqlRaw("GetCommulativeIncreasesSplitTerminals @p_DTimeBegin, @p_DTimeEnd, @p_TerminalCode, @p_IncreaseKind",
                p_DTimeBegin, p_DTimeEnd, p_TerminalCode, p_IncreaseKind).AsEnumerable()
                .Select(i => new IncreaseCommulativeTotalModel
                {
                    DTime = i.DTimeBegin,
                    Terminal = i.TerminalName,
                    TerminalCode = i.Code,
                    Amount = i.Total,
                });

            return res.AsQueryable();
        }
    }
}
