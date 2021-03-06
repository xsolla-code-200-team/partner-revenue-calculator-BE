﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using xsolla_revenue_calculator.Controllers.Requests;
using xsolla_revenue_calculator.DTO;
using xsolla_revenue_calculator.Exceptions;
using xsolla_revenue_calculator.Models;
using xsolla_revenue_calculator.Models.ForecastModels;
using xsolla_revenue_calculator.Models.UserInfoModels;
using xsolla_revenue_calculator.Services;
using xsolla_revenue_calculator.Services.ForecastExportService;

namespace xsolla_revenue_calculator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RevenueForecastController : Controller
    {
        private readonly IDatabaseAccessService _databaseAccessService;
        private readonly IRevenueForecastService _revenueForecastService;
        private readonly IForecastExportService _exportService;
        private readonly IMapper _mapper;

        public RevenueForecastController(IDatabaseAccessService databaseAccessService, IRevenueForecastService revenueForecastService, IMapper mapper, IForecastExportService exportService)
        {
            _databaseAccessService = databaseAccessService;
            _revenueForecastService = revenueForecastService;
            _mapper = mapper;
            _exportService = exportService;
        }

        /// <summary>
        /// Posting user information from the simple form to the service
        /// </summary>
        /// <response code="200">Returns in case of success</response>
        [HttpPost("Simple")]
        [SwaggerResponse(200, type: typeof(RevenueForecastViewModel))]
        [SwaggerResponse(400, type: typeof(ExceptionDetails))]
        [SwaggerRequestExample(typeof(UserInfoBaseRequestBody), typeof(UserInfoBaseRequestBodyExample))]
        public async Task<IActionResult> PostUserSimpleAsync([FromBody] UserInfoBaseRequestBody userInfoDto)
        {
            if (!ModelState.IsValid) throw new ValidationException(ModelState);
            var userInfo = await _databaseAccessService.LogUserAsync(userInfoDto);
            var forecast = await _revenueForecastService.StartCalculationAsync(userInfo);
            await _databaseAccessService.AttachForecastToUserAsync(userInfo.Id, forecast.Id);
            return Ok(_mapper.Map<RevenueForecastViewModel>(forecast));
        }
        
        
        /// <summary>
        /// Posting user information from the complex form to the service
        /// </summary>
        /// <response code="200">Returns in case of success</response>
        [HttpPost("Complex")]
        [SwaggerResponse(200, type: typeof(RevenueForecastViewModel))]
        [SwaggerResponse(400, type: typeof(ExceptionDetails))]
        [SwaggerRequestExample(typeof(UserInfoFullRequestBody), typeof(UserInfoFullRequestBodyExample))]
        public async Task<IActionResult> PostUserComplexAsync([FromBody] UserInfoFullRequestBody userInfoDto)
        {
            if (!ModelState.IsValid) throw new ValidationException(ModelState);
            var userInfo = await _databaseAccessService.LogUserAsync(userInfoDto);
            var forecast = await _revenueForecastService.StartCalculationAsync(userInfo);
            await _databaseAccessService.AttachForecastToUserAsync(userInfo.Id, forecast.Id);
            return Ok(_mapper.Map<RevenueForecastViewModel>(forecast));
        }
        
        /// <summary>
        /// Getting information about the forecast with given id
        /// </summary>
        /// <param name="id">id of the forecast</param>
        [HttpGet("{id}")]
        [SwaggerResponse(200, type: typeof(RevenueForecastViewModel))]
        [SwaggerResponse(404, type: typeof(ExceptionDetails))]
        public async Task<IActionResult> GetForecast(string id)
        {
            var forecast = await _databaseAccessService.GetForecastAsync(id);
            return Ok(_mapper.Map<RevenueForecastViewModel>(forecast));
        }
        
        /// <summary>
        /// Sending email with content as a body
        /// </summary>
        [HttpPost("Export")]
        public async Task<IActionResult> ExportHtml([FromBody] ExportRequestBody requestBody)
        {
            await _exportService.ExportForecast(requestBody);
            return Ok();
        }
    }
}