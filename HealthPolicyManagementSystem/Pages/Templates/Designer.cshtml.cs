using HealthPolicyManagementSystem.Data;
using HealthPolicyManagementSystem.Models;
using HealthPolicyManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace HealthPolicyManagementSystem.Pages.Templates
{
    [Authorize(Roles = "Admin")]
    public class DesignerModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DesignerModel(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public TemplateDesignerViewModel Template { get; set; } = new();

        [BindProperty]
        public IFormFile? LogoFile { get; set; }

  


        public IActionResult OnGet()
        {
            var template = _context.Templates.FirstOrDefault();

            if (template == null)
                return Page();

            Template = JsonSerializer.Deserialize<TemplateDesignerViewModel>(
                template.TemplateJson) ?? new();

            return Page();
        }


        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();


            Template.LogoPath = SaveImage(
                LogoFile,
                Template.LogoPath);


            



            Template.Layout.Logo = new()
            {
                Name = "Logo",
                X = Template.LogoX,
                Y = Template.LogoY,
                Width = Template.LogoWidth,
                Height = Template.LogoHeight
            };




            Template.Layout.Header = new()
            {
                Name = "Header",
                X = Template.HeaderX,
                Y = Template.HeaderY
            };


            Template.Layout.Title = new()
            {
                Name = "Title",
                X = Template.TitleX,
                Y = Template.TitleY
            };


            Template.Layout.Department = new()
            {
                Name = "Department",
                X = Template.DepartmentX,
                Y = Template.DepartmentY
            };


            Template.Layout.Version = new()
            {
                Name = "Version",
                X = Template.VersionX,
                Y = Template.VersionY
            };


            Template.Layout.Date = new()
            {
                Name = "Date",
                X = Template.DateX,
                Y = Template.DateY
            };


            Template.Layout.Description = new()
            {
                Name = "Description",
                X = Template.DescriptionX,
                Y = Template.DescriptionY
            };


          


            Template.Layout.QR = new()
            {
                Name = "QR",
                X = Template.QRX,
                Y = Template.QRY,
                Width = Template.QRSize,
                Height = Template.QRSize
            };


            Template.Layout.Footer = new()
            {
                Name = "Footer",
                X = Template.FooterX,
                Y = Template.FooterY
            };



            string json = JsonSerializer.Serialize(
                Template,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });



            var template = _context.Templates.FirstOrDefault();


            if (template == null)
            {
                template = new Template
                {
                    TemplateName = Template.TemplateName,
                    TemplateJson = json
                };

                _context.Templates.Add(template);
            }
            else
            {
                template.TemplateName = Template.TemplateName;
                template.TemplateJson = json;
                template.UpdatedAt = DateTime.Now;
            }


            _context.SaveChanges();


            TempData["Success"] =
                "Template saved successfully.";


            return RedirectToPage();
        }



        private string SaveImage(
            IFormFile? file,
            string currentPath)
        {
            if (file == null)
                return currentPath;


            string folder = Path.Combine(
                _environment.WebRootPath,
                "Uploads",
                "Templates");


            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);



            string fileName =
                Guid.NewGuid()
                + Path.GetExtension(file.FileName);



            string fullPath =
                Path.Combine(folder, fileName);



            using (var stream = new FileStream(
                fullPath,
                FileMode.Create))
            {
                file.CopyTo(stream);
            }



            return "/Uploads/Templates/" + fileName;
        }
    }
}
