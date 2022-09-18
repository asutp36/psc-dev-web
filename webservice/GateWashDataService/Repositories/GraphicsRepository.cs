using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
using GateWashDataService.Models.GateWashContext.StoredProcedures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GateWashDataService.Repositories
{
    public class GraphicsRepository
    {
        private readonly ILogger<GraphicsRepository> _logger;

        private readonly GateWashDbContext _model;
        private readonly WashesRepository _washesRepository;

        public GraphicsRepository(ILogger<GraphicsRepository> logger, GateWashDbContext model, WashesRepository washesRepository)
        {
            _logger = logger;
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
        public async Task<GraphicsDataModel> GetCommulativeTotalSplitTerminalsGraphicDataAsync(DateTime dtimeStart, DateTime dtimeEnd, IEnumerable<string> washCodes, string eventKindCode = null)
        {
            try
            {
                var data = await GetCommulativeTotalSplitTerminalsAsync(dtimeStart, dtimeEnd, washCodes, eventKindCode);

                return ConvertToGraphicDataModel(data);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Данные для графика из хранимки GetCommulativeIncreasesSplitTerminals, сгруппированные по дням
        /// </summary>
        /// <param name="dtimeStart">Начало периода</param>
        /// <param name="dtimeEnd">Конец периода</param>
        /// <param name="washCodes">Коды моек</param>
        /// <param name="eventKindCode">Код типа внесения (optional)</param>
        /// <returns>Данные для отображения графика</returns>
        public async Task<GraphicsDataModel> GetCommulativeTotalSplitTerminalsGraphicData_ByDayAsync(DateTime dtimeStart, DateTime dtimeEnd, IEnumerable<string> washCodes, string eventKindCode = null)
        {
            try
            {
                var data = await GetCommulativeTotalSplitTerminalsAsync(dtimeStart, dtimeEnd, washCodes, eventKindCode);

                var grouped = data.GroupBy(key => new { key.DTime.Date, key.TerminalCode, key.Terminal },
                             val => val.Amount,
                             (key, val) => new IncreaseCommulativeTotalModel
                                 {
                                     Terminal = key.Terminal,
                                     TerminalCode = key.TerminalCode,
                                     DTime = key.Date,
                                     Amount = val.Max() // сгруппировано по дням, тк нарастающий итог, то сумма за весь день = максимальное значение в этот день
                                 }
                             ).AsQueryable();

                return ConvertToGraphicDataModel(grouped);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + e.StackTrace);
                return null;
            }
        }
       
        /// <summary>
        /// Данные для графика из хранимки GetCommulativeIncreasesSplitTerminals, сгруппированные по месяцам
        /// </summary>
        /// <param name="dtimeStart">Начало периода</param>
        /// <param name="dtimeEnd">Конец периода</param>
        /// <param name="washCodes">Коды моек</param>
        /// <param name="eventKindCode">Код типа внесения (optional)</param>
        /// <returns>Данные для отображения графика</returns>
        public async Task<GraphicsDataModel> GetCommulativeTotalSplitTerminalsGraphicData_ByMonthAsync(DateTime dtimeStart, DateTime dtimeEnd, IEnumerable<string> washCodes, string eventKindCode = null)
        {
            try
            {
                var data = await GetCommulativeTotalSplitTerminalsAsync(dtimeStart, dtimeEnd, washCodes, eventKindCode);

                var grouped = data.GroupBy(key => new { key.DTime.Year, key.DTime.Month, key.TerminalCode, key.Terminal },
                             val => val.Amount,
                             (key, val) => new IncreaseCommulativeTotalModel
                                 {
                                     Terminal = key.Terminal,
                                     TerminalCode = key.TerminalCode,
                                     DTime = new DateTime(key.Year, key.Month, 1),
                                     Amount = val.Max() // сгруппировано по месяцам, тк нарастающий итог, то сумма за весь месяц = максимальное значение в этот месяц
                                 }
                             ).AsQueryable();

                return ConvertToGraphicDataModel(grouped);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Данные для графика из хранимки GetCommulativeIncreasesSplitTerminals, сгруппированные по по часам
        /// </summary>
        /// <param name="dtimeStart">Начало периода</param>
        /// <param name="dtimeEnd">Конец периода</param>
        /// <param name="washCodes">Коды моек</param>
        /// <param name="eventKindCode">Код типа внесения (optional)</param>
        /// <returns>Данные для отображения графика</returns>
        public async Task<GraphicsDataModel> GetCommulativeTotalSplitTerminalsGraphicData_ByHourAsync(DateTime dtimeStart, DateTime dtimeEnd, IEnumerable<string> washCodes, string eventKindCode = null)
        {
            try
            {
                var data = await GetCommulativeTotalSplitTerminalsAsync(dtimeStart, dtimeEnd, washCodes, eventKindCode);

                var grouped = data.GroupBy(key => new { key.DTime.Date, key.DTime.Hour, TerminalCode = key.TerminalCode, Terminal = key.Terminal },
                             val => val.Amount,
                             (key, val) => new IncreaseCommulativeTotalModel
                             {
                                 Terminal = key.Terminal,
                                 TerminalCode = key.TerminalCode,
                                 DTime = key.Date.AddHours(key.Hour),
                                 Amount = val.Max() // сгруппировано по часам, тк нарастающий итог, то сумма за весь час = максимальное значение в этот час
                             }
                             ).AsQueryable();

                return ConvertToGraphicDataModel(grouped);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Преобразовать данные из хранимой процедуры к формату для отображения на графике
        /// </summary>
        /// <param name="data">Данные из хранимой процедуры</param>
        /// <returns>Модель данных графика</returns>
        private GraphicsDataModel ConvertToGraphicDataModel(IQueryable<IncreaseCommulativeTotalModel> data)
        {
            GraphicsDataModel graphic = new GraphicsDataModel();
            graphic.Labels = data.Select(o => o.DTime).Distinct().ToList();
            List<string> datasetLabels = data.Select(o => o.Terminal).Distinct().ToList();
            graphic.Datasets = new List<Dataset>();

            foreach (string label in datasetLabels)
            {
                graphic.Datasets.Add(new Dataset() { Data = new List<int>(), Label = label });
            }

            foreach (DateTime dt in graphic.Labels)
            {
                var pointXAxis = data.Where(o => (o.DTime - dt).Duration() <= TimeSpan.Parse("00:00:01")).ToList();
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
            terminalCodes = string.Join(", ", terminals.Select(t => t.Code));

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
                }).ToList();

            return res.AsQueryable();
        }
    }
}
