using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace TryParseReports
{
    class Program
    {
        static void Main(string[] args)
        {
            ParseXls();

            Console.ReadKey();
        }

        private void ParseCsv()
        {

        }

        private static void ParseXls()
        {
            Excel.Application excel = new Excel.Application();
            Excel.Workbook workbook = excel.Workbooks.Open(@"D:\Programs\Работа\psc-dev-web\TryParseReports\m8-08.10.2019.xls");
            Excel.Worksheet worksheet = workbook.Worksheets["Данные"] as Excel.Worksheet;

            object[,] data = worksheet.UsedRange.Value;

            for (int r = 3; r < 10; r++)
            {
                DateTime startDate = DateTime.Parse(data[r, 1].ToString());
                DateTime stopDate = DateTime.Parse(data[r, 2].ToString());
                DateTime vupdatetime2 = DateTime.Parse(data[r, 9].ToString());
                Console.WriteLine("INSERT INTO %table% (startDate, stopDate, regionid, regionname, washname, wshaddress, washnum, postname, vupdatetime2, " +
                    "msgtotal, b10, b50, b100, b500, b1k, m10, s, linkcontext)" +
                                            $" VALUES({startDate.ToString("yyyyMMdd")}, " +
                                            $"{stopDate.ToString("yyyyMMdd")}, {data[r, 3]}, \'{data[r, 4]}\', {data[r, 5]}, {data[r, 6]}, {data[r, 7]}, {data[r, 8]}, " +
                                            $"{vupdatetime2.ToString("yyyyMMdd HH:mm:ss.fff")}, {data[r, 10]}, {data[r, 11]}, {data[r, 12]}, {data[r, 13]}, {data[r, 14]}, {data[r, 15]}, {data[r, 16]}, {data[r, 17]}, {data[r, 18]});" +
                                            " SELECT SCOPE_IDENTITY()");
                Console.WriteLine();
            }

            workbook.Close();
            excel.Quit();
        }
    }
}
