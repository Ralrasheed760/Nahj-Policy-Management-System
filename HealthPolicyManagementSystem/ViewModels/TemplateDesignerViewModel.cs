using System.ComponentModel.DataAnnotations;
using HealthPolicyManagementSystem.Models.TemplateLayout;

namespace HealthPolicyManagementSystem.ViewModels
{
    public class TemplateDesignerViewModel
    {
        public int TemplateID { get; set; }

        [Required]
        public string TemplateName { get; set; } = "";

        public string HeaderText { get; set; } = "";

        public string FooterText { get; set; } = "";

        public string LogoPath { get; set; } = "";

        public bool ShowQR { get; set; } = true;

        public bool ShowVersion { get; set; } = true;

        public bool ShowDate { get; set; } = true;

        public string PrimaryColor { get; set; } = "#0F6E5B";

        public string FontName { get; set; } = "Arial";

        public int FontSize { get; set; } = 12;

        public float LogoX { get; set; } = 30;
        public float LogoY { get; set; } = 20;
        public float LogoWidth { get; set; } = 100;
        public float LogoHeight { get; set; } = 70;

        public float HeaderX { get; set; } = 120;
        public float HeaderY { get; set; } = 40;

        public float TitleX { get; set; } = 50;
        public float TitleY { get; set; } = 130;

        public float DepartmentX { get; set; } = 50;
        public float DepartmentY { get; set; } = 170;

        public float VersionX { get; set; } = 50;
        public float VersionY { get; set; } = 210;

        public float DateX { get; set; } = 50;
        public float DateY { get; set; } = 250;

        public float DescriptionX { get; set; } = 50;
        public float DescriptionY { get; set; } = 310;

        public float QRX { get; set; } = 470;
        public float QRY { get; set; } = 700;
        public float QRSize { get; set; } = 70;

        public float FooterX { get; set; } = 50;
        public float FooterY { get; set; } = 790;

        public TemplateLayout Layout { get; set; } = new();
    }
}