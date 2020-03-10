using UserReporting.Shared.Events;

namespace UserReporting.ReportGenerator.Library
{
    public static class PDFGenerator
    {
        public static byte[] GeneratePDF(CreateReportRequested request)
        {
            var content = $@"
                            First Name: {request.FirstName}<br/>
                            Middle Name: {request.MiddleName}<br/>
                            Last Name: {request.LastName}<br/>
                            Date of Birth: {request.DateOfBirth:dd/mm/yyyy}<br/>
                            Joined us on: {request.JoinedOn:dd/mm/yyyy}";

            var Renderer = new IronPdf.HtmlToPdf();
            var pdfDoc = Renderer.RenderHtmlAsPdf(content);
            byte[] result = pdfDoc.BinaryData;

            return result;
        }
    }
}
