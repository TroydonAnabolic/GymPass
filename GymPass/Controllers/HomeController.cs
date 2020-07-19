﻿using System;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FacilityContext _facilityContext;


        public HomeController(
            UserManager<ApplicationUser> userManager,
            ILogger<HomeController> logger,
            FacilityContext facilityContext

            )
        {
            _userManager = userManager;
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

            // default to false for access
            ViewBag.AccessGrantedToFacility = false;
            // door open status depends on database value
            ViewBag.DoorOpened = facility.DoorOpened;
            // access denied message is normally true
            ViewBag.AccessDeniedMsgRecieved = true;

            //// if the time since access is granted, we redirect the user that only displays a page to submit data for the training intentions
            /// it will have the option to skip, which will just navigate the user back to home page
            /// possibly will have the page look like a replica of the dashboard(or just a random gym image), but without server side code, only have it with a reduced opacity, by making the modal load on page load.
            /// The modal will have server side code just for posting data for intended training durations and equipment to train with.
            /// TODO: Create a controller action, in facility controller, that allows this above mentioned functionality.
            if (DateTime.Now <= (user.TimeAccessGranted.AddSeconds(15)))
            {
                //return RedirectToAction("ViewWithModal");
            }

            // if time since the date where user was denied, is more than 5 seconds, then access denied msg received is not received
            if (DateTime.Now <= (user.TimeAccessDenied.AddSeconds(10)))
            ViewBag.AccessDeniedMsgRecieved = false;

            return View(facility);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, [Bind("FacilityID,FacilityName,NumberOfClientsUsingWeightRoom,NumberOfClientsUsingCardioRoom," +
            "NumberOfClientsUsingStretchRoom,IsOpenDoorRequested,DoorOpened,DoorCloseTimer")] Facility facilityView,
            [Bind("FirstName,TimeAccessGranted,EstimatedTrainingTime,UniqueEntryID")] UsersInGymDetail usersInGymDetailView) // 
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
            var facilityDetails = await _facilityContext.UsersInGymDetails.ToListAsync();
            UsersInGymDetail currentFacilityDetail = new UsersInGymDetail();
            var currentFacilityDetailDb = await _facilityContext.UsersInGymDetails.Where(f => f.UniqueEntryID == user.Id).FirstOrDefaultAsync();

            bool enteredGym = false, leftGym = false;
            // currentFacilityDetail = await _facilityContext.UsersInGymDetails.Where(f => f.UniqueEntryID == user.Id).FirstOrDefaultAsync();
            // foreach facility details count, check for user id match, then remove
            //currentFacilityDetail = await _facilityContext.UsersInGymDetails.FirstOrDefaultAsync();


            if (id != facility.FacilityID ||  id != facilityView.FacilityID)
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
                        ViewBag.IsExerciseLogComplete = false;

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
                                user.TimeAccessGranted = DateTime.Now;
                                // fill in facility details table, TODO: Exchange user time access with facility list
                                currentFacilityDetail.TimeAccessGranted = DateTime.Now;
                                currentFacilityDetail.FirstName = user.FirstName;
                                currentFacilityDetail.UniqueEntryID = user.Id;
                                currentFacilityDetail.FacilityID = facility.FacilityID;
                                enteredGym = true;
                                // TODO: Use AJAX to async send to and from the client at the same time
                            }
                            // if the user is already in the gym, when button is pushed then make reset all access to false, and decrement the number of ppl in the gym by 1
                            else if (user.IsInsideGym)
                            {
                                leftGym = true;
                                user.IsInsideGym = false;
                                facility.NumberOfClientsInGym--;

                                if (user.WillUseWeightsRoom)
                                {
                                    facility.NumberOfClientsUsingWeightRoom--;
                                    user.WillUseWeightsRoom = false;
                                }
                                if (user.WillUseCardioRoom)
                                {
                                    facility.NumberOfClientsUsingCardioRoom--;
                                    user.WillUseCardioRoom = false;
                                }
                                if (user.WillUseStretchRoom)
                                {
                                    facility.NumberOfClientsUsingStretchRoom--;
                                    user.WillUseWeightsRoom = false;
                                }

                                // if there are entries for facilities, loop through all the facilities, remove the entry which is stamped with the current user entry
                                if (facilityDetails.Count() > 0 )
                                {
                                     _facilityContext.UsersInGymDetails.Remove(currentFacilityDetailDb);
                                    //var gymDetailToRemove = new UsersInGymDetail();
                                    //gymDetailToRemove.FacilityID = currentFacilityDetailId;
                                    ////_facilityContext.UsersInGymDetails.Remove(gymDetailToRemove);
                                    //_facilityContext.Entry(gymDetailToRemove).State = EntityState.Deleted;

                                }

                                user.IsCameraScanSuccessful = false;
                                user.IsWithin10m = false;
                                user.AccessGrantedToFacility = false;
                            }

                        } // end access granted
                        else if (!user.AccessGrantedToFacility)
                        {
                            ViewBag.AccessDeniedMsgRecieved = false;
                            user.TimeAccessDenied = DateTime.Now;
                        }

                        // if door has been opened and user is authorised
                        if (facility.DoorOpened && user.AccessGrantedToFacility)
                        {
                            // log the time granted, and wait 5 seconds.
                            System.Threading.Thread.Sleep(facility.DoorCloseTimer);
                        }
                        else if (!user.AccessGrantedToFacility) System.Threading.Thread.Sleep(facility.DoorCloseTimer);

                        // When 5 second timer finishes, we close the door again automatically
                        // facility.IsOpenDoorRequested = false;
                        facility.DoorOpened = false;
                        _facilityContext.Update(facility);

                        // if we are entering gym, use the new facility object, if we are leaving, use the facility detail using Db values.
                        if (enteredGym) _facilityContext.Update(currentFacilityDetail);
                       // else if (leftGym) _facilityContext.Update(currentFacilityDetailDb);

                        // after a facility exist, then we can update facility to avoid foreign key constraint?
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
                // if the user has logged in successfully, then send to questionnaire for now, later will use AJAX to post questionaire here, and have option to later log in workout
                return (user.AccessGrantedToFacility) ? RedirectToAction("LogWorkout", "Facilities", new { id = user.DefaultGym }) : RedirectToAction(nameof(Index));
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
