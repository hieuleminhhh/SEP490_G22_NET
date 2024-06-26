using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IWebHostEnvironment _hostEnvironment;

    public ImageController(IWebHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest("Không có tập tin nào được tải lên.");
        }

        var uploads = Path.Combine(_hostEnvironment.ContentRootPath, "uploads");
        if (!Directory.Exists(uploads))
        {
            Directory.CreateDirectory(uploads);
        }

        var fileName = Path.GetFileName(image.FileName);
        var filePath = Path.Combine(uploads, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }
        var imageUrl = $"{Request.Scheme}://{Request.Host}/api/Image/getImage/{fileName}";
        return Ok(new { imageUrl });
    }


    [HttpGet("getImage/{fileName}")]
    public IActionResult GetImage(string fileName)
    {
        var uploads = Path.Combine(_hostEnvironment.ContentRootPath, "uploads");
        var filePath = Path.Combine(uploads, fileName);

        if (System.IO.File.Exists(filePath))
        {
            var mimeType = GetMimeType(fileName);
            return PhysicalFile(filePath, mimeType);
        }
        else
        {
            return NotFound();
        }
    }
    private string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream",
        };
    }
}
