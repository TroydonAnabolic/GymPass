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
using static GymPass.Areas.Identity.Pages.Account.Manage.IndexModel;

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
            // Get the default gym for a user and set it to be the Id for the gym being edited
            var user = await _userManager.GetUserAsync(User);

            if (user.Id == null)
            {
                return NotFound();
            }

            id = user.DefaultGym;

            if (id == null)
            {
                return RedirectToAction("Index", "Facilities");
            }

            var facility = await _facilityContext.Facilities.FindAsync(id);
            if (facility == null)
            {
                return NotFound();
            }
            
            ViewBag.DoorOpened = facility.DoorOpened;

            return View(facility);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, [Bind("FacilityID,FacilityName,NumberOfClientsUsingWeightRoom,NumberOfClientsUsingCardioRoom,NumberOfClientsUsingStretchRoom,IsOpenDoorRequested,DoorOpened,DoorCloseTimer")] Facility facilityView) // 
        {
            // Get the default gym for a user and set it to be the Id for the gym being edited
            var user = await _userManager.GetUserAsync(User);

            if (user.Id == null)
            {
                return NotFound();
            }

            id = user.DefaultGym;

            var facility = await _facilityContext.Facilities.FindAsync(id);


            if (id != facility.FacilityID && id != facilityView.FacilityID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // if door open is requested from the view then open the door
                    if (facilityView.IsOpenDoorRequested == true)
                    {
                        facility.DoorOpened = true;

                        // if the user is already in the gym, then say this user is not in the gym, and decrease the number of ppl in the gym
                        if (user.IsInsideGym)
                        {
                            user.IsInsideGym = false;
                            facility.NumberOfClientsInGym--;
                            _facilityContext.Update(facility);
                            await _facilityContext.SaveChangesAsync();
                            // TODO: if statements, if viewbag, user selected if they are using cardio equip, then increment cardio etc.
                        }
                        // if the user is not in the gym, then say this user is not in the gym, and increase the number of ppl in the gym
                        else if (!user.IsInsideGym)
                        {
                            facility.NumberOfClientsInGym++;
                            user.IsInsideGym = true;
                            _facilityContext.Update(facility);
                            await _facilityContext.SaveChangesAsync();
                        }
                    }
                    // when we are leaving we set open door and door opened to false
                    else if (facility.IsOpenDoorRequested == false)
                    {
                        facility.DoorOpened = false;
                        user.IsInsideGym = false;
                    }

                    // save the opened door and user
                    _facilityContext.Update(facility);
                    await _facilityContext.SaveChangesAsync();
                    await _userManager.UpdateAsync(user);

                    // Now jQuery will show the icon unlocked image, which only changes view to show unlocked icon not submit value, jquery will also be used to set the checkbox value based on click

                    // await statement 5 second timer before closing only if door is open, if the door is closed, it automatically closes
                    if (facility.DoorOpened) System.Threading.Thread.Sleep(facility.DoorCloseTimer);
                    // When 5 second timer finishes, we close the door again automatically
                    // facility.IsOpenDoorRequested = false;
                    facility.DoorOpened = false;
                    _facilityContext.Update(facility);
                    await _facilityContext.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilityExists(facility.FacilityID))
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
            return View(facility);
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

        private bool FacilityExists(int id)
        {
            return _facilityContext.Facilities.Any(e => e.FacilityID == id);
        }
    }
}
