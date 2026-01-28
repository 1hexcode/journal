using System.Text;
using System.Text.RegularExpressions;

namespace Journal.Services;

public interface IPdfExportService
{
    string ExportJournalTable(IEnumerable<Models.Journal> rows);
}

public class PdfExportService : IPdfExportService
{
    public string ExportJournalTable(IEnumerable<Models.Journal> journals)
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("=================================");
            sb.AppendLine("    JOURNAL EXPORT");
            sb.AppendLine($"    {DateTime.Now:dd MMM yyyy}");
            sb.AppendLine("=================================");
            sb.AppendLine();
        
            foreach (var journal in journals)
            {
                sb.AppendLine($"Date: {journal.Date:dd MMM yyyy}");
                sb.AppendLine($"Mood: {journal.PrimaryMood ?? "N/A"}");
                if (!string.IsNullOrEmpty(journal.SecondaryMoods))
                    sb.AppendLine($"Secondary Moods: {journal.SecondaryMoods}");
                sb.AppendLine();
                sb.AppendLine(StripHtml(journal.Notes));
                sb.AppendLine();
                sb.AppendLine("---");
                sb.AppendLine();
            }
        
            return sb.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Export error: {ex.Message}");
            throw;
        }
    }

    private string StripHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return "";
        return Regex.Replace(html, "<.*?>", string.Empty);
    }
}