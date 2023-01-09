using DemoApplication.Areas.Admin.ViewModels.Slider;
using DemoApplication.Contracts.File;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/slider")]
    [Authorize(Roles = "admin")]
    public class SliderController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IFileService _fileService;

        public SliderController(DataContext dataContext, IFileService fileService)
        {
            _dataContext = dataContext;
            _fileService = fileService;
        }


        #region List

        [HttpGet("list", Name = "admin-slider-list")]
        public async Task<IActionResult> ListAsync()
        {
            var model = await _dataContext.Sliders
                .Select(s => new ListItemViewModel(
                        s.Id,
                        s.Title,
                        s.Content,
                        _fileService.GetFileUrl(s.ImageNameInFileSystem, UploadDirectory.Slider),
                        s.CreatedAt,
                        s.UpdatedAt)).ToListAsync();


            return View(model);
        }

        #endregion

        #region Add

        [HttpGet("add", Name = "admin-slider-add")]
        public IActionResult AddAsync()
        {
            return View();
        }

        [HttpPost("add", Name = "admin-slider-add")]
        public async Task<IActionResult> Add(AddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var imageNameInSystem = await _fileService.UploadAsync(model!.Image, UploadDirectory.Slider);

            await AddSliderAsync(model.Image!.FileName, imageNameInSystem);


            async Task AddSliderAsync(string imageName, string imageNameInSystem)
            {
                var slider = new Slider
                {
                    Title = model.Title,
                    Content = model.Content,
                    ButtonName = model.ButtonName,
                    ButtonRedirectUrl = model.ButtonRedirectUrl,
                    Order = model.Order,
                    ImageName = imageName,
                    ImageNameInFileSystem = imageNameInSystem
                };


                await _dataContext.Sliders.AddAsync(slider);
                await _dataContext.SaveChangesAsync();
            }

            return RedirectToRoute("admin-slider-list");
        }




        #endregion


        #region Update

        [HttpGet("update/{id}", Name = "admin-slider-update")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id)
        {
            var slider = await _dataContext.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (slider is null)
            {
                return NotFound();
            }

            var model = new AddViewModel
            {
                Id = slider.Id,
                Title = slider.Title,
                Content = slider.Content,
                ButtonName = slider.ButtonName,
                ButtonRedirectUrl = slider.ButtonRedirectUrl,
                Order = slider.Order,

                ImageUrl = _fileService.GetFileUrl(slider.ImageNameInFileSystem, UploadDirectory.Slider),
            };

            return View(model);
        }

        [HttpPost("update/{id}", Name = "admin-slider-update")]
        public async Task<IActionResult> UpdateAsync(AddViewModel model)
        {
            var slider = await _dataContext.Sliders.FirstOrDefaultAsync(s => s.Id == model.Id);
            if (slider is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            await _fileService.DeleteAsync(slider.ImageNameInFileSystem, UploadDirectory.Slider);
            var imageFileNameInSystem = await _fileService.UploadAsync(model.Image, UploadDirectory.Slider);

            await UpdateSliderAsync(model.Image.FileName, imageFileNameInSystem);

            


            async Task UpdateSliderAsync(string imageName, string imageNameInFileSystem)
            {
                slider.Title = model.Title;
                slider.Content = model.Content;
                slider.ButtonName = model.ButtonName;
                slider.ButtonRedirectUrl = model.ButtonRedirectUrl;
                slider.ImageName = imageName;
                slider.ImageNameInFileSystem = imageNameInFileSystem;


                await _dataContext.SaveChangesAsync();
            }

            return RedirectToRoute("admin-slider-list");
        }

        #endregion

        #region Delete

        [HttpPost("delete/{id}", Name = "admin-slider-delete")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var slider = await _dataContext.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (slider is null)
            {
                return NotFound();
            }

            await _fileService.DeleteAsync(slider.ImageNameInFileSystem, UploadDirectory.Slider);

            _dataContext.Sliders.Remove(slider);
            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("admin-slider-list");
        }

        #endregion
    }
}