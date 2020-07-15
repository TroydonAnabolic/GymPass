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
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            // initiall set the access view bag to false, as this will prevent null exception

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

            // door open status depends on database value
            ViewBag.DoorOpened = facility.DoorOpened;
            // access denied message is normally true
            ViewBag.AccessDeniedMsgRecieved = true;

            //// if the time since access is granted, we redirect the user that only displays a page to submit data for the training intentions
            /// it will have the option to skip, which will just navigate the user back to home page
            /// possibly will have the page look like a replica of the dashboard, only have it with a reduced opacity, by making the modal load on page load.
            /// TODO: Create a controller action, in facility controller, that allows this above mentioned functionality.
            if (DateTime.Now <= (user.TimeAccessGranted.AddSeconds(15)))
            {
                return RedirectToAction("ViewWithModal");
            }

            // if time since the date where user was denied, is more than 5 seconds, then access denied msg received is not received
            if (DateTime.Now <= (user.TimeAccessDenied.AddSeconds(10)))
            ViewBag.AccessDeniedMsgRecieved = false;

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

            // note that variable facility is the database values, facilityView, binds data from the view
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
                { // maybe make is open door requested a user property
                    // if door open is requested from the view by clicking the button, then run the below logic to test if user is authorized and also apply crowdsensing functions
                    if (facilityView.IsOpenDoorRequested == true)
                    {
                        // temp viewBag data showing true - to be used for testing, unless I can get real data using webcam with facial recognition
                        // TODO: Add facial recognition scan and geo location detection

                        user.IsCameraScanSuccessful = true;
                        user.IsWithin10m = true;

                        // if camera scan and location check is true, and user is not in the gym, then we open the door, and access granted is true
                        if (user.IsCameraScanSuccessful && user.IsWithin10m && !user.IsInsideGym)
                        {
                            user.AccessGrantedToFacility = true;
                            ViewBag.AccessGrantedToFacility = true;
                        }
                        // if camera scan is not successful
                        else if (!user.IsCameraScanSuccessful && !user.IsWithin10m)
                        {
                            user.AccessGrantedToFacility = false;
                        }

                        if (user.AccessGrantedToFacility)
                        {
                            facility.DoorOpened = true;
                            // if the user is not in the gym, then say this user is not in the gym, and increase the number of ppl in the gym by 1
                            if (!user.IsInsideGym)
                            {
                                facility.NumberOfClientsInGym++;
                                user.IsInsideGym = true;
                                // TODO: await modal being filled, possibly use view bag to pass data, this will prevent the door from closing
                                _facilityContext.Update(facility);
                                await _facilityContext.SaveChangesAsync();
                            }
                            // if the user is already in the gym, when button is pushed then make reset all access to false, and decrement the number of ppl in the gym by 1
                            else if (user.IsInsideGym)
                            {
                                user.IsInsideGym = false;
                                facility.NumberOfClientsInGym--;
                                user.IsCameraScanSuccessful = false;
                                user.IsWithin10m = false;
                                user.AccessGrantedToFacility = false;
                                _facilityContext.Update(facility);
                                await _facilityContext.SaveChangesAsync();
                                // TODO: if statements, if viewbag, user selected if they are using cardio equip, then increment cardio etc.
                            }

                        } // end access granted
                        else if (!user.AccessGrantedToFacility)
                        {
                            ViewBag.AccessDeniedMsgRecieved = false;
                            user.TimeAccessDenied = DateTime.Now;
                        }
                        // make sure door open is no longer requested

                            //// TODO: when we reach midnight and, set everyones as out of the gym
                            //if (!facility.is24SevenGym user.IsInsideGym && DateTime.Today)
                            //{
                            //    facility.NumberOfClientsInGym = 0;
                            //    user.IsInsideGym = false;
                            //}

                            // save the opened door and user
                        _facilityContext.Update(facility);
                        await _facilityContext.SaveChangesAsync();


                        // if door has been opened and user is authorised
                        if (facility.DoorOpened && user.AccessGrantedToFacility)
                        {
                            // log the time granted, and wait 8 seconds.
                            user.TimeAccessGranted = DateTime.Now;
                            System.Threading.Thread.Sleep(8000);
                        }
                        else if (!user.AccessGrantedToFacility) System.Threading.Thread.Sleep(facility.DoorCloseTimer);

                        // When 5 second timer finishes, we close the door again automatically
                        // facility.IsOpenDoorRequested = false;
                        facility.DoorOpened = false;
                        _facilityContext.Update(facility);
                        await _facilityContext.SaveChangesAsync();
                        await _userManager.UpdateAsync(user);
                    }
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
