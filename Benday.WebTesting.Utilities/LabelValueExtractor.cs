using Microsoft.VisualStudio.TestTools.WebTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benday.WebTesting.Utilities
{
    [DisplayName("Extract Label Value")]
    [Description("Extracts the value from an ASP.NET Web Forms Label control.")]
    public class LabelValueExtractor : ExtractionRule
    {
        [DisplayName("Label Id")]
        [Description("Id for the Label control.")]
        public string LabelId { get; set; }

        public override void Extract(object sender, ExtractionEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(LabelId) == true)
            {
                Fail(e, "The label id value is invalid.");
            }
            else
            {
                HtmlTagInnerTextParser parser = 
                    new HtmlTagInnerTextParser(e.Response.BodyString);

                var match = 
                    parser.GetInnerTextForHtmlTags(
                    "span", "id", LabelId, true, true, true).FirstOrDefault();

                if (match == null)
                {
                    Fail(e, 
                        String.Format(
                        "Could not find a label with the Id of '{0}'.", LabelId));
                }
                else
                {
                    e.WebTest.Context.Add(ContextParameterName, match.Value);
                    e.Success = true;
                }
            }
        }

        private void Fail(ExtractionEventArgs e, string message)
        {
            e.Success = false;
            e.Message = message;
        }
    }
}
