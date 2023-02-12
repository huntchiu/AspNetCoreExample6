using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreExample6.Models;
using AspNetCoreExample6.Data;

namespace AspNetCoreExample4.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _context = context;
        _hostEnvironment = hostEnvironment;
    }

    public IActionResult Index()
    {
        var products = _context.Product.ToList();

        return View(products);

    }

    public IActionResult Upsert(int? Id)
    {

        if (Id != null)
        {
            var product = _context.Product.Find(Id);
            if (product == null)
            {
                return NotFound();
            }
            // 編輯Product 視圖
            return View(product);
        }
        // 創建Product 視圖
        return View();

    }

    [HttpPost]
    public IActionResult Upsert(Product product, IFormFile? file)
    {

        if (ModelState.IsValid)
        {

            string wwwRootPath = _hostEnvironment.WebRootPath;

            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images/products");
                var extension = Path.GetExtension(file.FileName);

                // 如果有更新圖片，則刪除原來的圖片文件,product.ImageUrl為原來的圖片地址，需要在Form提交上來
                if (product.ImageUrl != null)
                {
                    var oldImageUrl = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImageUrl))
                    {
                        System.IO.File.Delete(oldImageUrl);
                    }
                }

                // 複製新圖片至指定路徑，並更新product.ImageUrl 的值
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                product.ImageUrl = @"/images/products/" + fileName + extension;
            }

            // 更新資料庫
            _context.Product.Update(product);
            _context.SaveChanges();
            // 重定向至Index
            return RedirectToAction("Index");
        }

        //資料驗證錯誤，返回 Upsert View
        return View();
    }

    // 刪除資料
    [HttpPost]
    public IActionResult Delete(int? Id)
    {
        var product = _context.Product.Find(Id);
        if (product == null)
        {
            return NotFound();
        }
        // 刪除資料對應的圖片
        if (product.ImageUrl != null)
        {
            var oldImageUrl = Path.Combine(_hostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldImageUrl))
            {
                System.IO.File.Delete(oldImageUrl);
            }
        }
        _context.Product.Remove(product);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

