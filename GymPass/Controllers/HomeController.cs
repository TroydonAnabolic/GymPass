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

        [BindProperty]
        public Facility Facility { get; set; }

        [Authorize]
        public async Task<IActionResult> Index()
        {

            var facilityData = await _facilityContext.Facilities.ToListAsync();

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
