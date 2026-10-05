namespace HealthPolicyManagementSystem.Models.TemplateLayout
{
    public class TemplateLayout
    {
        public TemplateElement Logo { get; set; } = new()
        {
            Name = "Ministry Logo",
            Type = "Image",
            X = 30,
            Y = 20,
            Width = 70,
            Height = 70
        };

        public TemplateElement SecondLogo { get; set; } = new()
        {
            Name = "Cluster Logo",
            Type = "Image",
            X = 470,
            Y = 20,
            Width = 70,
            Height = 70
        };

        public TemplateElement Header { get; set; } = new()
        {
            Name = "Header",
            Type = "Text",
            X = 120,
            Y = 40,
            Width = 300,
            Height = 40,
            FontSize = 20
        };

        public TemplateElement Title { get; set; } = new()
        {
            Name = "Policy Title",
            Type = "Text",
            X = 50,
            Y = 130,
            Width = 400,
            Height = 30,
            FontSize = 14
        };

        public TemplateElement Department { get; set; } = new()
        {
            Name = "Department",
            Type = "Text",
            X = 50,
            Y = 170,
            Width = 300,
            Height = 25,
            FontSize = 12
        };

        public TemplateElement Version { get; set; } = new()
        {
            Name = "Version",
            Type = "Text",
            X = 50,
            Y = 210,
            Width = 200,
            Height = 25,
            FontSize = 12
        };

        public TemplateElement Date { get; set; } = new()
        {
            Name = "Effective Date",
            Type = "Text",
            X = 50,
            Y = 250,
            Width = 250,
            Height = 25,
            FontSize = 12
        };

        public TemplateElement Description { get; set; } = new()
        {
            Name = "Description",
            Type = "Text",
            X = 50,
            Y = 310,
            Width = 450,
            Height = 300,
            FontSize = 12
        };

        public TemplateElement Signature { get; set; } = new()
        {
            Name = "Signature",
            Type = "Image",
            X = 380,
            Y = 700,
            Width = 120,
            Height = 70
        };

        public TemplateElement QR { get; set; } = new()
        {
            Name = "QR Code",
            Type = "QR",
            X = 470,
            Y = 700,
            Width = 70,
            Height = 70
        };

        public TemplateElement Footer { get; set; } = new()
        {
            Name = "Footer",
            Type = "Text",
            X = 50,
            Y = 790,
            Width = 450,
            Height = 25,
            FontSize = 10
        };
    }
}
