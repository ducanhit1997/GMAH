using System.Collections.Generic;
using System;
using System.Configuration;

namespace GMAH.Web.Helpers
{
    public class MailMergeHelper
    {
        public static void TextToWord(string pWordDoc, Dictionary<string, string> pDictionaryMerge)
        {
            Object oMissing = System.Reflection.Missing.Value;
            Object oTrue = true;
            Object oFalse = false;
            Microsoft.Office.Interop.Word.Application oWord = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document oWordDoc = new Microsoft.Office.Interop.Word.Document();

            // Debug thì để tru
            bool.TryParse(ConfigurationManager.AppSettings["DEBUG_MAILMERGE"], out bool debugMailMerge);
            oWord.Visible = debugMailMerge;

            Object oTemplatePath = pWordDoc;
            oWordDoc = oWord.Documents.Add(ref oTemplatePath, ref oMissing, ref oMissing, ref oMissing);

            foreach (Microsoft.Office.Interop.Word.Field myMergeField in oWordDoc.Fields)
            {
                Microsoft.Office.Interop.Word.Range rngFieldCode = myMergeField.Code;
                String fieldText = rngFieldCode.Text;
                if (fieldText.StartsWith(" MERGEFIELD"))
                {
                    Int32 endMerge = fieldText.IndexOf("\\");
                    if (endMerge == -1) endMerge = fieldText.Length;
                    Int32 fieldNameLength = fieldText.Length - endMerge;
                    String fieldName = fieldText.Substring(11, endMerge - 11);
                    fieldName = fieldName.Trim();
                    foreach (var item in pDictionaryMerge)
                    {
                        if (fieldName == item.Key)
                        {
                            myMergeField.Select();
                            oWord.Selection.TypeText(item.Value);
                        }
                    }
                }
            }

            oWordDoc.SaveAs(pWordDoc.Replace(".docx", ".pdf"), Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
            oWordDoc.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges);
            oWord.Application.Quit(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges);
        }
    }
}