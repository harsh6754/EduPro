using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace ASPNETCoreIdentityDemo.Services
{
    public static class CaptchaService
    {
        private static readonly char[] chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

        //If you want to inlcude Only Upper Case as part of Captcha, please use below
        //private static readonly char[] chars =
        //    "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

        // Generates a random captcha string of the specified length.
        // length: The length of the random string, default is 6
        // returns: Randomly generated string
        public static string GenerateCaptchaCode(int length = 6)
        {
            var random = new Random();
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }
            return sb.ToString();
        }

        // Creates a captcha image from the given captcha text.
        // captchaCode: The text to render in the captcha image.
        // returns: A byte array representing the captcha image in PNG format.
        public static byte[] GenerateCaptchaImage(string captchaCode)
        {
            // Defines the dimensions (150x50 pixels) and initializes a Bitmap and Graphics object with anti-aliasing and high-quality settings.
            int width = 150;
            int height = 50;

            using var bitmap = new Bitmap(width, height);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.Clear(Color.White);

            // Draws each character of the CAPTCHA string onto the image with slight random rotations and color variations to enhance security.
            var random = new Random();
            using var font = new Font("Arial", 20, FontStyle.Bold);
            using var brush = new SolidBrush(Color.Black);

            // Adds random lines to the image to make automated reading more difficult.
            float x = 10f;
            for (int i = 0; i < captchaCode.Length; i++)
            {
                // Slight rotation
                graphics.ResetTransform();
                var angle = random.Next(-10, 10);
                graphics.RotateTransform(angle);

                // Random color (optional)
                brush.Color = Color.FromArgb(
                    random.Next(50, 150),
                    random.Next(50, 150),
                    random.Next(50, 150));

                // Draw each character
                graphics.DrawString(captchaCode[i].ToString(), font, brush, x, 10);
                x += 20;  // shift x a bit for the next character
            }

            // Adds random lines to the image to make automated reading more difficult.
            for (int i = 0; i < 10; i++)
            {
                var pen = new Pen(Color.FromArgb(
                    random.Next(100, 255),
                    random.Next(100, 255),
                    random.Next(100, 255)));
                var startPoint = new Point(random.Next(width), random.Next(height));
                var endPoint = new Point(random.Next(width), random.Next(height));
                graphics.DrawLine(pen, startPoint, endPoint);
            }

            // Saves the bitmap as a PNG image and converts it to a byte array for transmission.
            using var ms = new System.IO.MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }
}