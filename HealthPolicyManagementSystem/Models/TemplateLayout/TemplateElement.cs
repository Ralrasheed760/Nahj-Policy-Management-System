namespace HealthPolicyManagementSystem.Models.TemplateLayout
{
    public class TemplateElement
    {
        public string Name { get; set; } = "";

        public string Type { get; set; } = "";

        public float X { get; set; }

        public float Y { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public bool Visible { get; set; } = true;

        public int FontSize { get; set; } = 12;

        public string FontColor { get; set; } = "#000000";
    }
}
