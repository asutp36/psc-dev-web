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
            Excel.Application excel = new Excel.Application();
            Excel.Workbook workbook = excel.Workbooks.Open(@"D:\Programs\Работа\psc-dev-web\TryParseReports\m8-08.10.2019.xls");
            Excel.Worksheet worksheet = workbook.Worksheets["Данные"] as Excel.Worksheet;

            object[,] data = worksheet.UsedRange.Value;

            for (int r = 3; r < data.GetLength(0); r++)
            {
                for (int c = 1; c <= data.GetLength(1); c++)
                    Console.Write(data[r, c].ToString() + ' ');
                Console.WriteLine();
            }


            //worksheet = null;
            workbook.Close();
            excel.Quit();

            Console.ReadKey();
        }
    }
}
