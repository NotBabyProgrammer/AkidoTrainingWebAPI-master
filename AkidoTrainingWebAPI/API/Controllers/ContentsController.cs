using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AkidoTrainingWebAPI.BusinessLogic.DTOs;
using AkidoTrainingWebAPI.BusinessLogic.Repositories;
using AkidoTrainingWebAPI.DataAccess.Data;
using AkidoTrainingWebAPI.DataAccess.Models;
using AkidoTrainingWebAPI.BusinessLogic.DTOs.ContentsDTO;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AkidoTrainingWebAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly AkidoTrainingWebAPIContext _context;
        private readonly string _videoStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "API", "Videos");
        private readonly string _imageStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "API", "Image");

        public ContentsController(AkidoTrainingWebAPIContext context)
        {
            _context = context;
        }

        // GET: api/Contents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contents>>> GetContents()
        {
            return await _context.Contents.ToListAsync();
        }

        // GET: api/Contents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contents>> GetContents(int id)
        {
            var contents = await _context.Contents.FindAsync(id);

            if (contents == null)
            {
                return NotFound();
            }

            return contents;
        }
        
        // GET: api/Contents/Post/5
        [HttpGet("Post/{postId}")]
        public async Task<ActionResult<IEnumerable<Contents>>> GetContentsFilledByPostId(int postId)
        {
            if (!_context.Posts.Any(p => postId == p.Id))
            {
                return NotFound("This post is not existing");
            }

            var contents = await _context.Contents
                                         .Where(c => c.PostId == postId)
                                         .OrderBy(c => c.Order)
                                         .ToListAsync();

            if (contents == null || !contents.Any())
            {
                return NotFound($"No contents found for PostId: {postId}");
            }

            return Ok(contents);
        }

        // PUT: api/Contents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContents(int id, Contents contents)
        {
            if (id != contents.Id)
            {
                return BadRequest();
            }

            _context.Entry(contents).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContentsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Contents/AddText
        [HttpPost("AddText")]
        public async Task<ActionResult<Contents>> AddText(ContentsDTOText contents)
        {
            if(!_context.Posts.Any(p => contents.PostId == p.Id))
            {
                return NotFound("This post is not existing");
            }

            int newOrder;
            if (_context.Contents.Any(c => c.PostId == contents.PostId))
            {
                newOrder = _context.Contents
                                   .Where(c => c.PostId == contents.PostId)
                                   .Max(c => c.Order) + 1;
            }
            else
            {
                newOrder = 1;
            }

            var newContent = new Contents
            {
                PostId = contents.PostId,
                Content = contents.Content,
                Order = newOrder,
                Type = "Text"
            };

            _context.Contents.Add(newContent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContents", new { id = newContent.Id }, newContent);
        }

        //POST: api/Contents/AddImage
        [HttpPost("AddImage")]
        public async Task<IActionResult> AddImage(int postId, IFormFile image)
        {
            // Validate Post existence
            if (!_context.Posts.Any(p => p.Id == postId))
            {
                return NotFound("This post does not exist.");
            }

            // Validate image file
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image file received.");
            }

            // Validate image type (Optional but recommended)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid image format. Allowed formats are: .jpg, .jpeg, .png, .gif");
            }

            // Ensure the image storage directory exists
            if (!Directory.Exists(_imageStoragePath))
            {
                Directory.CreateDirectory(_imageStoragePath);
            }

            // Determine the new order for the content
            int newOrder;
            if (_context.Contents.Any(c => c.PostId == postId))
            {
                newOrder = _context.Contents
                                   .Where(c => c.PostId == postId)
                                   .Max(c => c.Order) + 1;
            }
            else
            {
                newOrder = 1;
            }

            // Generate the file name as PostId + Order
            string newFileName = $"{postId}_{newOrder}{Path.GetExtension(image.FileName)}";
            string newFilePath = Path.Combine(_imageStoragePath, newFileName);

            // Save the image file
            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Create a new content entry
            var newContent = new Contents
            {
                PostId = postId,
                Content = newFileName, // Store the file name as PostId + Order
                Order = newOrder,
                Type = "Image" // Set the content type to "Image"
            };

            _context.Contents.Add(newContent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContents", new { id = newContent.Id }, new
            {
                Message = "Image uploaded and added to post successfully!",
                ContentId = newContent.Id,
                FileName = newFileName
            });
        }

        //POST: api/Contents/AddVideo
        [HttpPost("AddVideo")]
        public async Task<IActionResult> AddVideo(int postId,IFormFile video)
        {
            // Validate Post existence
            if (!_context.Posts.Any(p => postId == p.Id))
            {
                return NotFound("This post does not exist.");
            }

            // Validate video file
            if (video == null || video.Length == 0)
            {
                return BadRequest("No video file received.");
            }

            // Ensure the video storage directory exists
            if (!Directory.Exists(_videoStoragePath))
            {
                Directory.CreateDirectory(_videoStoragePath);
            }

            // Determine the new order for the content
            int newOrder;
            if (_context.Contents.Any(c => c.PostId == postId))
            {
                newOrder = _context.Contents
                                   .Where(c => c.PostId == postId)
                                   .Max(c => c.Order) + 1;
            }
            else
            {
                newOrder = 1;
            }

            string filePath = Path.Combine(_videoStoragePath, $"{newOrder}{postId}");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await video.CopyToAsync(stream);
            }

            // Create new content entry
            var newContent = new Contents
            {
                PostId = postId,
                Content = $"{newOrder}{postId}",
                Order = newOrder,
                Type = "Video"
            };

            _context.Contents.Add(newContent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContents", new { id = newContent.Id }, new
            {
                Message = "Video uploaded and content created successfully!",
                ContentId = newContent.Id,
                FileName = $"{newOrder}{postId}"
            });
        }

        //PUT: api/Contents/UpdateImage
        [HttpPut("UpdateImage/{contentId}")]
        public async Task<IActionResult> UpdateImage(int contentId, IFormFile newImage)
        {
            // Fetch the content entry by ContentId
            var content = _context.Contents.FirstOrDefault(c => c.Id == contentId && c.Type == "Image");
            if (content == null)
            {
                return NotFound("Image content not found.");
            }

            // Validate new image file
            if (newImage == null || newImage.Length == 0)
            {
                return BadRequest("No new image file received.");
            }

            // Validate image type (Optional but recommended)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(newImage.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid image format. Allowed formats are: .jpg, .jpeg, .png, .gif");
            }

            // Build the file path for the existing image
            string existingFilePath = Path.Combine(_imageStoragePath, content.Content);

            // Delete the existing image file if it exists
            if (System.IO.File.Exists(existingFilePath))
            {
                System.IO.File.Delete(existingFilePath);
            }

            // Generate a new file name based on PostId and Order
            string newFileName = $"{content.PostId}_{content.Order}{Path.GetExtension(newImage.FileName)}";
            string newFilePath = Path.Combine(_imageStoragePath, newFileName);

            // Save the new image file
            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await newImage.CopyToAsync(stream);
            }

            // Update the content entry with the new file name
            content.Content = newFileName;
            _context.Contents.Update(content);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Image updated successfully!",
                ContentId = content.Id,
                FileName = newFileName
            });
        }

        //PUT: api/Contents/UpdateVideo
        [HttpPut("UpdateVideo")]
        public async Task<IActionResult> UpdateVideo(int contentId, IFormFile video)
        {
            // Validate video file
            if (video == null || video.Length == 0)
            {
                return BadRequest("No video file received.");
            }

            // Ensure the video storage directory exists
            if (!Directory.Exists(_videoStoragePath))
            {
                Directory.CreateDirectory(_videoStoragePath);
            }

            // Find the existing video content
            var existingContent = _context.Contents.FirstOrDefault(c => c.Id == contentId && c.Type == "Video");
            if (existingContent == null)
            {
                return NotFound("The specified video content does not exist.");
            }

            // Delete the existing video file
            string existingFilePath = Path.Combine(_videoStoragePath, existingContent.Content);
            if (System.IO.File.Exists(existingFilePath))
            {
                System.IO.File.Delete(existingFilePath);
            }

            // Generate new file name and path
            string newFilePath = Path.Combine(_videoStoragePath, $"{existingContent.Order}{existingContent.PostId}");

            // Save the new video file
            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await video.CopyToAsync(stream);
            }

            // Update the database record with the new video details
            existingContent.Content = $"{existingContent.Order}{existingContent.PostId}";
            _context.Contents.Update(existingContent);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Video updated successfully!",
                ContentId = existingContent.Id,
                FileName = $"{existingContent.Order}{existingContent.PostId}"
            });
        }

        // DELETE: api/Contents/DeleteContent/5
        [HttpDelete("DeleteContent/{id}")]
        public async Task<IActionResult> DeleteContents(int id)
        {
            var contents = await _context.Contents.FindAsync(id);
            if (contents == null)
            {
                return NotFound();
            }
            if (contents.Type == "Video")
            {
                string existingFilePath = Path.Combine(_videoStoragePath, contents.Content);
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
            }
            else if (contents.Type == "Image")
            {
                string existingFilePath = Path.Combine(_imageStoragePath, contents.Content);
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
            }
            _context.Contents.Remove(contents);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("GetImage/{contentId}")]
        public IActionResult GetImage(int contentId)
        {
            // Fetch the content entry by ContentId
            var content = _context.Contents.FirstOrDefault(c => c.Id == contentId && c.Type == "Image");
            if (content == null)
            {
                return NotFound("Image content not found.");
            }

            // Build the file path for the image
            string filePath = Path.Combine(_imageStoragePath, content.Content);

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Image file not found.");
            }

            // Determine the MIME type of the image file
            string mimeType = GetMimeType(filePath);

            // Read the file and return it as a FileStreamResult
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, mimeType);
        }

        [HttpGet("Play/{fileName}")]
        public IActionResult PlayVideo(string fileName)
        {
            var filePath = Path.Combine(_videoStoragePath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Video not found.");
            }

            var videoStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(videoStream, "video/mp4", enableRangeProcessing: true);
        }

        private bool ContentsExists(int id)
        {
            return _context.Contents.Any(e => e.Id == id);
        }
        
        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream", // Default MIME type for unknown extensions
            };
        }

    }
}
