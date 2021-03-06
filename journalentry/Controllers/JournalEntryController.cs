﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using journalentry.Models;
using System.IO;
using ExcelDataReader;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using PapeCore;
using DataTables;
using System.Data.Common;

namespace journalentry.Controllers
{
    [Authorize]
    public class JournalEntryController : Controller
    {
       
        //Get: JournalEntry
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ValidateTable()
        {
            /*var settings = Properties.Settings.Default;
            var formData = HttpContext.Request.Form;

                var response = new Editor()
                    .Model<EntryRow>()
                    .Field(new Field("journalNumber")
                        .Validator(Validation.MinNum(
                            20,
                            new ValidationOpts { Message = "Testing Validation" }
                        ))
                    )
                    .Process(formData)
                    .Data();

                return Json(response, JsonRequestBehavior.AllowGet);*/
            return View("test");
        }

        // POST: JournalEntry
        [HttpPost]
        public JsonResult CreateExcel()
        {
            HttpPostedFileBase excelFile = Request.Files[0];

            if (excelFile != null && excelFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(excelFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/uploads"), fileName);
                    excelFile.SaveAs(path);

                using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    ExcelCellAddress start = worksheet.Dimension.Start;
                    ExcelCellAddress end = worksheet.Dimension.End;

                    int startCol = start.Column;
                    int startRow = start.Row;
                    int endCol = end.Column;
                    int endRow = end.Row;
                    int rowId = 0;

                    List<EntryError> errorList = new List<EntryError>();

                    var entryList = new List<EntryRow>();
                    var columns = new List<ExcelMap>();
                    Dictionary<string, string> map = null;

                    var convertDateTime = new Func<double, DateTime>(excelDate =>
                    {
                        if (excelDate < 1)
                            throw new ArgumentException("Excel dates cannot be smaller than 0.");

                        var dateOfReference = new DateTime(1900, 1, 1);

                        if (excelDate > 60d)
                            excelDate = excelDate - 2;
                        else
                            excelDate = excelDate - 1;
                        return dateOfReference.AddDays(excelDate);
                    });

                    var props = typeof(EntryRow).GetProperties()
                        .Select(prop =>
                        {
                            var displayAttribute = (DisplayAttribute)prop.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
                            return new
                            {
                                Name = prop.Name,
                                DisplayName = displayAttribute == null ? prop.Name : displayAttribute.Name,
                                Order = displayAttribute == null || !displayAttribute.GetOrder().HasValue ? 999 : displayAttribute.Order,
                                PropertyInfo = prop,
                                PropertyType = prop.PropertyType,
                                HasDisplayName = displayAttribute != null
                            };
                        })
                    .Where(prop => !string.IsNullOrWhiteSpace(prop.DisplayName) && prop.Name != "rowId")
                    .ToList();

                    //Based on Template provided, the first row in Excel spreadsheet contains column names
                    for (int column = startCol; column <= endCol; column++)
                    {
                        string cellValue = (worksheet.Cells[startRow, column].Value ?? string.Empty).ToString().Trim();

                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            columns.Add(new ExcelMap()
                            {
                                Name = cellValue,
                                MappedTo = map == null || map.Count == 0 ?
                                    cellValue :
                                    map.ContainsKey(cellValue) ? map[cellValue] : string.Empty,
                                Index = column
                            });
                        }
                    }

                    //Loop through the remaining rows of the spreadsheet
                    for (int rowIndex = startRow + 1; rowIndex <= endRow; rowIndex++)
                    {
                        var item = new EntryRow();
                        columns.ForEach(column =>
                        {
                            var value = worksheet.Cells[rowIndex, column.Index].Value;
                            var valueStr = value == null ? string.Empty : value.ToString().Trim();
                            var prop = props.First(p => p.DisplayName.Contains(column.MappedTo));

                            //Excel stores all numbers as doubles, but we're relying on the object's property types
                            if (prop != null)
                            {
                                var propertyType = prop.PropertyType;
                                object parsedValue = null;

                                if (propertyType == typeof(int?) || propertyType == typeof(int))
                                {
                                    int val;
                                    if (!int.TryParse(valueStr, out val))
                                    {
                                        val = default(int);
                                    }

                                    parsedValue = val;
                                }
                                else if (propertyType == typeof(short?) || propertyType == typeof(short))
                                {
                                    short val;
                                    if (!short.TryParse(valueStr, out val))
                                        val = default(short);
                                    parsedValue = val;
                                }
                                else if (propertyType == typeof(long?) || propertyType == typeof(long))
                                {
                                    long val;
                                    if (!long.TryParse(valueStr, out val))
                                        val = default(long);
                                    parsedValue = val;
                                }
                                else if (propertyType == typeof(decimal?) || propertyType == typeof(decimal))
                                {
                                    decimal val;
                                    if (!decimal.TryParse(valueStr, out val))
                                        val = default(decimal);
                                    parsedValue = val;
                                }
                                else if (propertyType == typeof(double?) || propertyType == typeof(double))
                                {
                                    double val;
                                    if (!double.TryParse(valueStr, out val))
                                    {
                                        val = default(double);

                                        EntryError doubleError = new EntryError()
                                        {
                                            errorMessage = "Input not valid",
                                            errorRow = rowIndex,
                                            errorCol = column.Index
                                        };

                                        errorList.Add(doubleError);
                                    }
                                    else
                                    {
                                        parsedValue = val;
                                    }
                                    
                                }
                                else if (propertyType == typeof(DateTime?) || propertyType == typeof(DateTime))
                                {
                                    parsedValue = convertDateTime((double) value);
                                }
                                else if (propertyType.IsEnum)
                                {
                                    try
                                    {
                                        parsedValue = Enum.ToObject(propertyType, int.Parse(valueStr));
                                    }
                                    catch
                                    {
                                        parsedValue = Enum.ToObject(propertyType, 0);
                                    }
                                }
                                else if (propertyType == typeof(string))
                                {
                                    parsedValue = valueStr;
                                }
                                else
                                {
                                    try
                                    {
                                        parsedValue = Convert.ChangeType(value, propertyType);
                                    }
                                    catch
                                    {
                                        parsedValue = valueStr;
                                    }
                                }

                                try
                                {
                                    prop.PropertyInfo.SetValue(item, parsedValue);
                                }
                                catch (Exception ex)
                                {
                                    logger.log("Excel upload error : " + ex.Message);
                                }
                            }
                        });

                        entryList.Add(item);
                    }

                    //Now that we've added all of the spreadsheet fields to the list of EntryRows, add in a unique rowId
                    //The rowId is used for the checkbox feature in the datatables editor and as the key value in the HTML table
                    
                    foreach (var item in entryList)
                    {
                        item.DT_RowId = rowId;
                        rowId++;
                    }
                    
                    var json = new JavaScriptSerializer().Serialize(entryList);
                    Console.WriteLine(json);

                    //json = json.Replace("[{", "{\"data\" :[{").Replace("}]", "}]}");

                    json = json.Replace("[{", "{\"data\" :[{").Replace("}]", "}],\"fieldErrors\" :[{\"name\" :\"journalNumber\",\"status\" :\"This field is required\"},{\"name\":\"Source\",\"status\": \"This field is required\"}]}");

                    return Json(json, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(excelFile);
        }
    }
}