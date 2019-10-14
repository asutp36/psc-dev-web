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

            object[,] someData = (worksheet.Range["A2", "A16"] as Excel.Range).Value;

            for(int r = 1; r < someData.GetLength(0); r++)
                for (int c = 1; c <= someData.GetLength(1); c++)
                    Console.WriteLine(someData[r, c].ToString());

            //worksheet = null;
            workbook.Close();
            excel.Quit();

            Console.ReadKey();
        }
    }
}
