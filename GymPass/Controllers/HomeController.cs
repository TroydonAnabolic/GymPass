using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GymPass.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using GymPass.Data;
using Microsoft.EntityFrameworkCore;

namespace GymPass.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FacilityContext _facilityContext;


        public HomeController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<HomeController> logger,
            FacilityContext facilityContext

            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _facilityContext = facilityContext;
        }


        // GET: Home/Index/1
        // TODO: We begin using id = 1 for now, later will implement dynamically changing this ID number, if it is null then redirect to action choose gym
        [Authorize]
        public async Task<IActionResult> Index(int? id)
        {
            // Get the default gym for a user and set it to be the Id
            var user = await _userManager.GetUserAsync(User);

            if (user.Id == null)
            {
                return NotFound();
            }



            if (id == null)
            {
                return RedirectToAction("Index", "Facilities");
            }

            var facility = await _facilityContext.Facilities.FindAsync(id);
            if (facility == null)
            {
                return NotFound();
            }

            ViewBag.DoorOpened = false;

            //Facility.DoorOpened = true;

            return View();
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
}
