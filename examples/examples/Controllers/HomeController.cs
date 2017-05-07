using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace examples.Controllers
{
    public class HomeController : Controller
    {
        private IMemoryCache _cache;

        public HomeController(IMemoryCache cache)
        {
            this._cache = cache;
        }

        public IActionResult Index()
        {
            

            // Check whenever key is already present in cache
            //if (string.IsNullOrEmpty(_cache.Get<string>("timestamp")))
            //{
            //    _cache.Set<string>("timestamp", DateTime.Now.ToString());
            //}
            // More elegant way of the same
            //if (!_cache.TryGetValue<string>("timestamp", out string timestamp))
            //{
            //    _cache.Set<string>("timestamp", DateTime.Now.ToString());
            //}

            //_cache.Set<string>("timestamp", DateTime.Now.ToString());
            return View();
        }

        public IActionResult About()
        {
            // Set expiration options of cached item
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTime.Now.AddSeconds(10); // removed on specified date and time 
                                                                      // (in here 20 seconds from the moment of setting this option)
            options.SlidingExpiration = TimeSpan.FromSeconds(10);     // removed if object remains idle for specified amount of time
            options.RegisterPostEvictionCallback(MyCallback, this);   // register a callback function when object is removed from cache

            // Get existing item, if it doesnt exist create one
            //string timestamp = _cache.GetOrCreate<string>("timestamp", entry =>
            //{
            //    return DateTime.Now.ToString();
            //});

            if (!_cache.TryGetValue<string>("timestamp", out string timestamp))
            {
                _cache.Set<string>("timestamp", DateTime.Now.ToString(), options);
            }

            ViewData["callback"] = _cache.Get<string>("callback");
            //string timestamp = _cache.Get<string>("timestamp");

            return View("About", timestamp);
        }

        private void MyCallback(object key, object value, EvictionReason reason, object state)
        {
            var message = $"Cache entry was removed: {reason} on {DateTime.Now.ToString()}";
            ((HomeController)state)._cache.Set("callback", message);
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
