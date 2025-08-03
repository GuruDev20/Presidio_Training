using Microsoft.AspNetCore.Mvc;
using MigrationApp.DTOs.Model;
using MigrationApp.Interfaces.Services;

namespace MigrationApp.Controllers
{
    // [ApiController]
    // [Route("api/models")]
    // public class ModelController : ControllerBase
    // {
    //     private readonly IModelService _modelService;
    //     public ModelController(IModelService modelService)
    //     {
    //         _modelService = modelService;
    //     }

    //     [HttpPost("add")]
    //     public async Task<IActionResult> AddModelAsync([FromBody] AddModelDto modelDto)
    //     {
    //         if (modelDto == null)
    //         {
    //             return BadRequest("Model data cannot be null.");
    //         }

    //         var result = await _modelService.AddModelAsync(modelDto);
    //         return Ok(result);
    //     }

    //     [HttpDelete("delete/{modelId}")]
    //     public async Task<IActionResult> DeleteModelAsync(Guid modelId)
    //     {
    //         if (modelId == Guid.Empty)
    //         {
    //             return BadRequest("Model ID cannot be empty.");
    //         }

    //         var result = await _modelService.DeleteModelAsync(modelId);
    //         return Ok(result);
    //     }

    //     [HttpGet("all")]
    //     public async Task<IActionResult> GetAllModelsAsync()
    //     {
    //         var models = await _modelService.GetAllModelsAsync();
    //         return Ok(models);
    //     }

    //     [HttpGet("{modelId}")]
    //     public async Task<IActionResult> GetModelByIdAsync(Guid modelId)
    //     {
    //         if (modelId == Guid.Empty)
    //         {
    //             return BadRequest("Model ID cannot be empty.");
    //         }

    //         var model = await _modelService.GetModelByIdAsync(modelId);
    //         if (model == null)
    //         {
    //             return NotFound("Model not found.");
    //         }

    //         return Ok(model);
    //     }

    //     [HttpPut("update")]
    //     public async Task<IActionResult> UpdateModelAsync([FromBody] UpdateModelDto modelDto)
    //     {
    //         if (modelDto == null)
    //         {
    //             return BadRequest("Model data cannot be null.");
    //         }

    //         var result = await _modelService.UpdateModelAsync(modelDto);
    //         return Ok(result);
    //     }

    //     // [HttpGet("search/{modelName}")]
    //     // public async Task<IActionResult> SearchModelsAsync(Guid id)
    //     // {
    //     //     if (Guid.Empty == id)
    //     //     {
    //     //         return BadRequest("Model ID cannot be empty.");
    //     //     }
    //     //     var products = await _modelService.GetProductsByModelIdAsync(id);
    //     //     return Ok(products);
    //     // }
    // }
}