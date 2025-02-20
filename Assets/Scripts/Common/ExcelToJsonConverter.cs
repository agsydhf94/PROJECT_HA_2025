using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HA
{
    public class ExcelToJsonConverter : Singleton<ExcelToJsonConverter>
    {
        // public string excelFileName = "data.xlsx"; // 변환할 엑셀 파일명 (StreamingAssets 폴더 내)
        public string jsonOutputFolder = "JsonData"; // JSON을 저장할 폴더명
        public int sheetIndex = 0; // 변환할 시트의 번호 (기본값: 첫 번째 시트)


        public void ConvertExcelToJson(string excelFileName)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, excelFileName);
            string outputFolderPath = Path.Combine(Application.streamingAssetsPath, jsonOutputFolder);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(excelFileName);

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Excel 파일을 찾을 수 없습니다: {fullPath}");
                return;
            }

            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                int totalSheets = workbook.NumberOfSheets;

                // 시트 개수만큼 반복하여 JSON 변환
                for (int i = 0; i < totalSheets; i++)
                {
                    ISheet sheet = workbook.GetSheetAt(i);
                    string jsonFileName = $"{fileNameWithoutExtension}.json";
                    string jsonPath = Path.Combine(outputFolderPath, jsonFileName);

                    List<Dictionary<string, object>> sheetData = ConvertSheetToJson(sheet);

                    if (sheetData.Count > 0)
                    {
                        string jsonOutput = JsonConvert.SerializeObject(sheetData, Formatting.Indented);
                        File.WriteAllText(jsonPath, jsonOutput);
                        Debug.Log($"엑셀 시트 '{sheet.SheetName}' → JSON 변환 완료: {jsonPath}");
                    }
                    else
                    {
                        Debug.LogWarning($"엑셀 시트 '{sheet.SheetName}'에 변환할 데이터가 없습니다.");
                    }
                }
            }
        }

        private List<Dictionary<string, object>> ConvertSheetToJson(ISheet sheet)
        {
            List<Dictionary<string, object>> sheetData = new List<Dictionary<string, object>>();
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            for (int i = 1; i <= sheet.LastRowNum; i++) // 첫 행(헤더) 제외
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;

                var rowData = new Dictionary<string, object>();

                for (int j = 0; j < cellCount; j++)
                {
                    string columnName = headerRow.GetCell(j).ToString();
                    ICell cell = row.GetCell(j);
                    rowData[columnName] = GetValueFromCell(cell);
                }

                sheetData.Add(rowData);
            }

            return sheetData;
        }

        private object GetValueFromCell(ICell cell)
        {
            if (cell == null) return null;

            switch (cell.CellType)
            {
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Numeric:
                    return cell.NumericCellValue;
                case CellType.String:
                    return cell.StringCellValue;
                default:
                    return cell.ToString();
            }
        }
    }
}
