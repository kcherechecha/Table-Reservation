using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebRestik.Models;

namespace WebRestik.Controllers
{
    public class MenusController : Controller
    {
        private readonly RestaurantWebContext _context;

        public MenusController(RestaurantWebContext context)
        {
            _context = context;
        }

        // GET: Menus
        public async Task<IActionResult> Index()
        {
              return View(await _context.Menus.ToListAsync());
        }

        // GET: Menus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Menus == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // GET: Menus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Menus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile Photo,[Bind("Id,DishName,Photo,Description")] Menu menu)
        {
            string token = await UpldoadFileToBlob(Photo);
            if (Photo != null && Photo.Length > 0)
            {
                menu.Photo = token;

                if (ModelState.IsValid)
                {
                    _context.Add(menu);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(menu);
        }

        // GET: Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Menus == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            return View(menu);
        }

        // POST: Menus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DishName,Photo,Description")] Menu menu)
        {
            if (id != menu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(menu);
        }

        // GET: Menus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Menus == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // POST: Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Menus == null)
            {
                return Problem("Entity set 'RestaurantWebContext.Menus'  is null.");
            }
            var menu = await _context.Menus.FindAsync(id);
            if (menu != null)
            {
                _context.Menus.Remove(menu);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(int id)
        {
          return _context.Menus.Any(e => e.Id == id);
        }

        public async Task<string> UpldoadFileToBlob(IFormFile file)
        {
            var config = new ConfigurationBuilder()
            .AddJsonFile("C:\\Users\\boda2\\source\\repos\\WebRestik\\key.json")
            .Build();

            string connectionString = config["AzureStorageConnectionString"];
            string containerName = "file-container";


            if (file != null)
            {
                string fileName = file.FileName;

                using (var stream = new MemoryStream())
                {

                    await file.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    try
                    {
                        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                        BlobClient blobClient = containerClient.GetBlobClient(fileName);

                        await blobClient.UploadAsync(stream, true);

                        var sasBuilder = new BlobSasBuilder
                        {
                            BlobContainerName = containerName,
                            BlobName = fileName,
                            Resource = "b",
                            ExpiresOn = DateTimeOffset.UtcNow.AddYears(1),
                            StartsOn = DateTimeOffset.UtcNow
                        };

                        string pattern = @"AccountName=([^;]+);AccountKey=([^;]+);";
                        Match match = Regex.Match(connectionString, pattern);
                        string accountName = match.Groups[1].Value;
                        string accountKey = match.Groups[2].Value;

                        var storageSharedKeyCredential = new StorageSharedKeyCredential(accountName, accountKey);

                        sasBuilder.SetPermissions(BlobSasPermissions.Read);
                        var sasToken = sasBuilder.ToSasQueryParameters(storageSharedKeyCredential).ToString();
                        var blobUrlWithSasToken = blobClient.Uri + "?" + sasToken;

                        return blobUrlWithSasToken;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Помилка завантаження файлу в Blob Storage: {ex.Message}");
                    }
                }
            }
            else
            {
                throw new Exception($"Помилка. Завантажений файл пустий");
            }
            return null;
        }
    }
}
