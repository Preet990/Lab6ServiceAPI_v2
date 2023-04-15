using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Xml.Serialization;
using Lab6ServiceAPI.Models;
using System.Xml.Schema;
using System.Xml;
using Lab6ServiceAPI.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.VisualBasic;
using System.IO;
//using AutoMapper;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Lab6ServiceAPI.Controllers
{
    [EnableCors]
    [Route("[controller]")]
    [ApiController]
    public class RestaurantReviewController : ControllerBase
    {
        //private readonly IHostingEnvironment _hostingEnvironment;
        //public RestaurantReviewController(IHostingEnvironment hostingEnvironment)
        //{
        //    _hostingEnvironment = hostingEnvironment;
        //}          



        //[HttpGet]
        [HttpGet("GetAllRestaurantReviews")]        
        public List<RestaurantInfo> GetAllRestaurantReviews()
        {
            var restaurant_reviews = GetRestaurantReviewsFromXml();
            List<RestaurantInfo> list=new List<RestaurantInfo>();
            int id = 0;
            foreach (var restaurant_review in restaurant_reviews.restaurant) 
            {

                id++;
                list.Add(new RestaurantInfo {
                    id = id, name = restaurant_review.name,
                    address = new Address
                    {
                        street = restaurant_review.address.street_address,
                        city = restaurant_review.address.city,
                        provstate = restaurant_review.address.state_province.ToString(),
                        postalzipcode = restaurant_review.address.zip_postal_code
                    }     
                    //address = string.Format("{0} {1} {2} {3}", restaurant_review.address.street_address, restaurant_review.address.city, restaurant_review.address.state_province, restaurant_review.address.street_address.Zip)
                    ,cost =new Cost
                    {
                        maxCost = restaurant_review.cost.max,
                        minCost = restaurant_review.cost.min,
                        currentCost = restaurant_review.cost.Value
                    }
                    ,
                    rating = new Rating
                    {
                        currentRating = restaurant_review.rating.Value,
                        maxRating = restaurant_review.rating.max,
                        minRating = restaurant_review.rating.min
                    }                                        
                    ,
                    foodType = restaurant_review.food_type,
                    summary = restaurant_review.summary,
                });
            }
            return list;         

        }
        [HttpGet("GetRestaurantReviewbyId/{id1}")]
        public RestaurantInfo GetRestaurantReviewbyId(int id1) 
        {
            if (id1 <= 0) return null;
            var restaurant_reviews = GetRestaurantReviewsFromXml();
            List<RestaurantInfo> list = new List<RestaurantInfo>();
            int id = 0;
            foreach (var restaurant_review in restaurant_reviews.restaurant)
            {

                id++;
                list.Add(new RestaurantInfo
                {
                    id = id,
                    name = restaurant_review.name,
                    address = new Address
                    {
                        street = restaurant_review.address.street_address,
                        city = restaurant_review.address.city,
                        provstate = restaurant_review.address.state_province.ToString(),
                        postalzipcode = restaurant_review.address.zip_postal_code
                    }
                    //address = string.Format("{0} {1} {2} {3}", restaurant_review.address.street_address, restaurant_review.address.city, restaurant_review.address.state_province, restaurant_review.address.street_address.Zip)
                    ,
                    cost = new Cost
                    {
                        maxCost = restaurant_review.cost.max,
                        minCost = restaurant_review.cost.min,
                        currentCost = restaurant_review.cost.Value
                    }
                    ,
                    rating = new Rating
                    {
                        currentRating = restaurant_review.rating.Value,
                        maxRating = restaurant_review.rating.max,
                        minRating = restaurant_review.rating.min
                    }
                    ,
                    foodType = restaurant_review.food_type,
                    summary = restaurant_review.summary,
                });
            }
            if (id1 > list.Count) 
            {
                return null;
            }

            return list[id1-1];

        }

        [HttpGet("GetRestaurantNames")]
        public string[] GetRestaurantNames()
        {            
            var restaurant_reviews = GetRestaurantReviewsFromXml();
            var arr= new List<string>();            
            foreach (var restaurant_review in restaurant_reviews.restaurant)
            {
                arr.Add(restaurant_review.name);

            }
            return arr.ToArray();
        }

        [HttpPut("UpdateaRestaurantReview")]
        public IActionResult UpdateaRestaurantReview([FromBody] RestaurantInfo updatedInfo)
        {
            var restaurant_reviews = GetRestaurantReviewsFromXml();

            
            //get row to edit
            var restaurantReviewToUpdate =restaurant_reviews.restaurant[updatedInfo.id-1];

            restaurantReviewToUpdate.name = updatedInfo.name;


            restaurantReviewToUpdate.address.street_address = updatedInfo.address.street;
            restaurantReviewToUpdate.address.city = updatedInfo.address.city;
            restaurantReviewToUpdate.address.state_province = (StateProvinceType)Enum.Parse(typeof(StateProvinceType), updatedInfo.address.provstate);
            restaurantReviewToUpdate.address.zip_postal_code = updatedInfo.address.postalzipcode;



            restaurantReviewToUpdate.cost.min = Convert.ToByte(updatedInfo.cost.minCost);            
            restaurantReviewToUpdate.cost.max = Convert.ToByte(updatedInfo.cost.maxCost);
            restaurantReviewToUpdate.cost.Value = Convert.ToByte(updatedInfo.cost.currentCost);

            restaurantReviewToUpdate.rating.Value = Convert.ToByte(updatedInfo.rating.currentRating);
            restaurantReviewToUpdate.rating.max = Convert.ToByte(updatedInfo.rating.maxRating);
            restaurantReviewToUpdate.rating.min = Convert.ToByte(updatedInfo.rating.minRating);
                                
            restaurantReviewToUpdate.food_type=updatedInfo.foodType;
            restaurantReviewToUpdate.summary=updatedInfo.summary;


            SaveRestaurantReviewsToXml(restaurant_reviews);

            return StatusCode(200, "Restaurant review updated successfully.");


            //var restaurantReviewToUpdate = restaurant_reviews.restaurant.FirstOrDefault(r => r.Id == updatedInfo.id);

            //using var streamWriter = new StreamWriter("restaurant_reviews.xml");
            //serializer.Serialize(streamWriter, restaurantReviews);
            
        }
        [HttpPut("SaveaRestaurantReview")]
        public IActionResult SaveaRestaurantReview([FromBody] RestaurantInfo updatedInfo)
        {
            try
            {
                var state_province = (StateProvinceType)Enum.Parse(typeof(StateProvinceType), updatedInfo.address.provstate);
            }
            catch (Exception)
            {

                return StatusCode(400, "State is Incorrect");
            }


            var restaurant_reviews = GetRestaurantReviewsFromXml();
            var list = restaurant_reviews.restaurant.ToList();
            list.Add(new restaurant_reviewsRestaurant {
                name = updatedInfo.name,
                address = new address
                {                    
                    street_address= updatedInfo.address.street,
                    city = updatedInfo.address.city,
                    state_province = (StateProvinceType)Enum.Parse(typeof(StateProvinceType), updatedInfo.address.provstate),
                    zip_postal_code= updatedInfo.address.postalzipcode
                },
                cost = new RangeType
                {
                    max = Convert.ToByte(updatedInfo.cost.maxCost),
                    min = Convert.ToByte(updatedInfo.cost.minCost),
                    Value = Convert.ToByte(updatedInfo.cost.currentCost),                    
                },
                 rating = new RangeType
                 {

                     max = Convert.ToByte(updatedInfo.rating.maxRating),
                     min = Convert.ToByte(updatedInfo.rating.minRating),
                     Value = Convert.ToByte(updatedInfo.rating.currentRating),
                 },
                 food_type= updatedInfo.foodType,
                 summary= updatedInfo.summary,

            });
            restaurant_reviews.restaurant = list.ToArray();
            SaveRestaurantReviewsToXml(restaurant_reviews);
            //list.ToArray();

            return StatusCode(200, "Restaurant Review has been added new");
        }

        [HttpDelete("{id}")]

        public RestaurantInfo Delete(int id)
        {
            
            var restaurant_reviews = GetRestaurantReviewsFromXml();
            var list = restaurant_reviews.restaurant.ToList();

            if (restaurant_reviews.restaurant.Length >= id) {

                var restaurant1 = restaurant_reviews.restaurant[id - 1];
                var res = new RestaurantInfo
                {
                    id = id,
                    name = restaurant1.name,
                    address = new Address
                    {
                        street = restaurant1.address.street_address,
                        city = restaurant1.address.city,
                        provstate = restaurant1.address.state_province.ToString(),
                        postalzipcode = restaurant1.address.zip_postal_code
                    }
                        //address = string.Format("{0} {1} {2} {3}", restaurant_review.address.street_address, restaurant_review.address.city, restaurant_review.address.state_province, restaurant_review.address.street_address.Zip)
                        ,
                    cost = new Cost
                    {
                        maxCost = restaurant1.cost.max,
                        minCost = restaurant1.cost.min,
                        currentCost = restaurant1.cost.Value
                    }
                        ,
                    rating = new Rating
                    {
                        currentRating = restaurant1.rating.Value,
                        maxRating = restaurant1.rating.max,
                        minRating = restaurant1.rating.min
                    }
                        ,
                    foodType = restaurant1.food_type,
                    summary = restaurant1.summary,
                };                       
                //var tam = list[id - 1];
                list.RemoveAt(id - 1);
                restaurant_reviews.restaurant = list.ToArray();
                SaveRestaurantReviewsToXml(restaurant_reviews);
                return res;
                //return StatusCode(200, "Restaurant Review has been removed");
            }
            else 
            {
                return new RestaurantInfo();
            }
            

            
        }



            //// GET api/<RestaurantReviewController>/5
            //[HttpGet("{id}")]
            //public RestaurantInfo Get(int id)
            //{
            //   throw new NotImplementedException("Replace this line with your code");
            //}

            //[Route("[action]")]
            //[HttpGet]
            //public List<string> GetRestaurantNames()
            //{
            //    throw new NotImplementedException("Replace this line with your code");

            //}

            //// POST <RestaurantReviewController>
            //[HttpPost]
            //public void Post([FromBody] RestaurantInfo restInfo)
            //{


            //    throw new NotImplementedException("Replace this line with your code");

            //}

            //// PUT api/<RestaurantReviewController>
            //[HttpPut]
            //[EnableCors]
            //public void Put([FromBody] RestaurantInfo restInfo)
            //{
            //   throw new NotImplementedException("Replace this line with your code");
            //}

            //// DELETE api/<RestaurantReviewController>/5
            //[HttpDelete("{id}")]
            //public void Delete(int id)
            //{
            //    throw new NotImplementedException("Replace this line with your code");
            //}

            private restaurant_reviews GetRestaurantReviewsFromXml()
        {
            restaurant_reviews reviews = null;

            string xmlPath = Path.GetFullPath("Data/restaurant_review.xml");

            using (FileStream xs = new FileStream(xmlPath, FileMode.Open))
            {
                XmlSerializer serializor = new XmlSerializer(typeof(restaurant_reviews));
                reviews = serializor.Deserialize(xs) as restaurant_reviews;
            }
            return reviews;
        }


        private void SaveRestaurantReviewsToXml(restaurant_reviews reviews)
        {
            string xmlFilePath = Path.GetFullPath("Data/restaurant_review.xml");
            using (FileStream xs = new FileStream(xmlFilePath, FileMode.Create))
            {
                XmlSerializer serializor = new XmlSerializer(typeof(restaurant_reviews));
                serializor.Serialize(xs, reviews);
            }

        }

        //private RestaurantInfo GetRestaurantInfo(restaurant_reviewsRestaurant rs)
        //{
        //    RestaurantInfo rsInfo = new RestaurantInfo();
        //    rsInfo.address = new Address();
        //    rsInfo.rating = new Rating();
        //    rsInfo.cost = new Cost();

        //    rsInfo.name = rs.name;
        //    rsInfo.address.street = rs.address.street_address;
        //    rsInfo.address.city = rs.address.city;
        //    rsInfo.address.provstate = rs.address.state_province.ToString();
        //    rsInfo.address.postalzipcode = rs.address.zip_postal_code;
        //    rsInfo.summary = rs.summary;
        //    rsInfo.foodType = rs.food_type;
        //    rsInfo.rating.currentRating = rs.rating.Value;
        //    rsInfo.rating.minRating = rs.rating.min;
        //    rsInfo.rating.maxRating = rs.rating.max;
        //    rsInfo.cost.currentCost = rs.cost.Value;
        //    rsInfo.cost.minCost = rs.cost.min;
        //    rsInfo.cost.maxCost = rs.cost.max;

        //    return rsInfo;
        //}

        //private void UpdateRestaurantWithRestaurantInfo(restaurant_reviewsRestaurant rest, RestaurantInfo restInfo)
        //{
        //    if (!string.IsNullOrEmpty(restInfo.name)) rest.name = restInfo.name;

        //    if (restInfo.address != null)
        //    {
        //        if (!string.IsNullOrEmpty(restInfo.address.street))
        //            rest.address.street_address = restInfo.address.street;

        //        if (!string.IsNullOrEmpty(restInfo.address.city))
        //            rest.address.city = restInfo.address.city;

        //        if (!string.IsNullOrEmpty(restInfo.address.provstate))
        //            rest.address.state_province = Enum.Parse<StateProvinceType>(restInfo.address.provstate);

        //        if (!string.IsNullOrEmpty(restInfo.address.postalzipcode))
        //            rest.address.zip_postal_code = restInfo.address.postalzipcode;
        //    }

        //    if (!string.IsNullOrEmpty(restInfo.summary)) rest.summary = restInfo.summary;

        //    if (!string.IsNullOrEmpty(restInfo.foodType)) rest.food_type = restInfo.foodType;

        //    if (restInfo.rating != null)
        //    {
        //        rest.rating.Value = (byte)restInfo.rating.currentRating;
        //    }

        //    if (restInfo.cost != null)
        //    {
        //        rest.cost.Value = (byte)restInfo.cost.currentCost;
        //    }
        //}
        //private restaurant_reviewsRestaurant GetNewRestaurantWithRestaurantInfo(RestaurantInfo restInfo)
        //{
        //    restaurant_reviewsRestaurant rest = new restaurant_reviewsRestaurant();

        //    rest.name = restInfo.name;

        //    if (restInfo.address != null)
        //    {
        //        rest.address = new address();
        //        rest.address.street_address = restInfo.address.street;
        //        rest.address.city = restInfo.address.city;
        //        rest.address.state_province = Enum.Parse<StateProvinceType>(restInfo.address.provstate);
        //        rest.address.zip_postal_code = restInfo.address.postalzipcode;
        //    }
        //    rest.summary = restInfo.summary;
        //    rest.food_type = restInfo.foodType;

        //    if (restInfo.rating != null)
        //    {
        //        rest.rating = new RangeType();
        //        rest.rating.Value = (byte)restInfo.rating.currentRating;
        //        rest.rating.min = (byte)restInfo.rating.minRating;
        //        rest.rating.max = (byte)restInfo.rating.maxRating;
        //    }

        //    if (restInfo.cost != null)
        //    {
        //        rest.cost = new RangeType();
        //        rest.cost.Value = (byte)restInfo.cost.currentCost;
        //        rest.cost.min = (byte)restInfo.cost.minCost;
        //        rest.cost.max = (byte)restInfo.cost.maxCost;
        //    }
        //    return rest;
        //}


    }
}
