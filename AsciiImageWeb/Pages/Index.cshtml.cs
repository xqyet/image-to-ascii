using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;

namespace AsciiImageWeb.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        [BindProperty]
        public int TargetWidth { get; set; } = 120;   // default width in characters

        [BindProperty]
        public int FontSize { get; set; } = 6;       // default font size in px

        public string? AsciiArt { get; set; }
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ImageFile == null || ImageFile.Length == 0)
            {
                ErrorMessage = "Please choose an image file.";
                return Page();
            }

            if (TargetWidth <= 0) TargetWidth = 120;
            if (FontSize <= 0) FontSize = 6;

            try
            {
                using var stream = ImageFile.OpenReadStream();
                using var original = new Bitmap(stream);

                int targetWidth = TargetWidth;

                double aspectRatio = (double)original.Height / original.Width;

                // characters are taller than they are wide, so compress vertically
                double charAspect = 0.5; // tweak: 0.45–0.6 depending on font and taste

                int targetHeight = (int)(targetWidth * aspectRatio * charAspect);

                using var resized = new Bitmap(original, new Size(targetWidth, targetHeight));

                var sb = new System.Text.StringBuilder();

                for (int y = 0; y < resized.Height; y++)
                {
                    for (int x = 0; x < resized.Width; x++)
                    {
                        Color c = resized.GetPixel(x, y);

                        sb.Append("<span style=\"");
                        sb.AppendFormat("color: rgb({0},{1},{2});", c.R, c.G, c.B);
                        sb.Append("\">█</span>");
                    }

                    // line break so your htmlToStream() sees '\n'
                    sb.Append("<br/>");
                }

                AsciiArt = sb.ToString();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error processing image: " + ex.Message;
            }

            return Page();
        }


    }
}
