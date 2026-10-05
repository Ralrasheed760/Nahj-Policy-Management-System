namespace HealthPolicyManagementSystem.Helpers
{
    public abstract class DocumentElement
    {
    }

    public class ParagraphElement : DocumentElement
    {
        public string Text { get; set; } = "";
    }

    public class TableElement : DocumentElement
    {
        public List<List<string>> Rows { get; set; } = new();
    }
}
