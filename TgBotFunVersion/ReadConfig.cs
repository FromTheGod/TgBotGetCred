
using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;
namespace TgBotFunVersion

{
    internal class ReadConfig
    {
        
        private string filePath { get; } = @"C:\Users\User\source\repos\TgBotFunVersion\TgBotFunVersion\Data\Config.xlsx";//путь к конфиг файлу
        
        Dictionary<string, string> settingDictionary = new Dictionary<string, string>();
        public ReadConfig()//Конструктор сразу считывает конфиг и заносит его в словарь
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.End.Row;
                int colCount = worksheet.Dimension.End.Column;
                for (int row = 1; row <= rowCount; row++)
                {
                    string key = worksheet.Cells[row, 1].Value?.ToString();
                    string value = worksheet.Cells[row, 2].Value?.ToString();

                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        settingDictionary[key] = value;
                    }
                }
            }
        }

        public string returnValue(string key) //получение значения по ключу
        {
            return settingDictionary.ContainsKey(key) ? settingDictionary[key].ToString() : "Error key doesnot exist";
        }
    }
}
