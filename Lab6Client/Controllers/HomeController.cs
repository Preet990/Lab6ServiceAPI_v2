using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Lab6Client.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Net;
using Newtonsoft.Json;

namespace Lab6.Controllers
{
    public class HomeController : Controller
    {

        private IConfiguration _configuration;
        private string serviceUrl;
        public HomeController(IConfiguration config)
        {
            _configuration = config;
            serviceUrl = _configuration.GetValue<string>("ServerURL");
        }
        public async Task<IActionResult> Index()
        {
            
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(serviceUrl + "/GetAllRestaurantReviews");
            var json = await response.Content.ReadAsStringAsync();
            var restaurant = System.Text.Json.JsonSerializer.Deserialize<List<RestaurantInfo>>(json);
            if (restaurant == null)
            {
                return View(new List<RestaurantInfo>());
            }
            return View(restaurant);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound( );
            }

            // Make GET request to API
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(serviceUrl+ "/GetRestaurantReviewbyId/"+ id);
            var json = await response.Content.ReadAsStringAsync();

            // Deserialize JSON response into C# object
            var restaurant = System.Text.Json.JsonSerializer.Deserialize<RestaurantInfo>(json);

            // Map C# object to view model
            //var viewModel = Mapper.Map<RestaurantInfo, RestaurantFormViewModel>(restaurant);

            // Render form using HTML helpers
            return View(restaurant);
            


        }

        [HttpPost]
        public async Task<IActionResult> Edit(RestaurantInfo restInfo)
        {

            var restaurantInfoJson = System.Text.Json.JsonSerializer.Serialize(restInfo);
            var content = new StringContent(restaurantInfoJson, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            var response = await httpClient.PutAsync(serviceUrl + "/UpdateaRestaurantReview", content);

            if (response.IsSuccessStatusCode)
            {
                // Handle success
                return RedirectToAction("Index");
            }
            else
            {
                // Handle error
                return View();
            }

            
            
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> New(RestaurantInfo restInfo)
        {
            var restaurantInfoJson = System.Text.Json.JsonSerializer.Serialize(restInfo);
            var content = new StringContent(restaurantInfoJson, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            var response = await httpClient.PutAsync(serviceUrl + "/SaveaRestaurantReview", content);

            if (response.IsSuccessStatusCode)
            {
                // Handle success
                return RedirectToAction("Index");
            }
            else
            {
                string message = await response.Content.ReadAsStringAsync();
                ViewBag.Confirmation = message;
                // Handle error
                return View();
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var data = new { id = id };
            //var jsonData = JsonConvert.SerializeObject(data);
            //var content = new StringContent(jsonData, Encoding.UTF8, "application/json");


            //var httpClient = new HttpClient();
            //var response = await httpClient.PostAsync(serviceUrl + "/Delete", content);
            var httpClient = new HttpClient();
            var response = await httpClient.DeleteAsync(serviceUrl+ "/" + id);

            if (response.IsSuccessStatusCode)
            {
                // Handle success
                return RedirectToAction("Index");
            }
            else
            {
                string message = await response.Content.ReadAsStringAsync();
                ViewBag.Confirmation = message;
                // Handle error
                return View();
            }
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}

    }

}
