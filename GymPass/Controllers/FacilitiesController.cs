using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymPass.Data;
using GymPass.Models;
using Microsoft.AspNetCore.Identity;

namespace GymPass.Controllers
{
    public class FacilitiesController : Controller
    {
        private readonly FacilityContext _facilityContext;
        private readonly UserManager<ApplicationUser> _userManager;


        public FacilitiesController(
            FacilityContext facilityContext,
            UserManager<ApplicationUser> userManager
            )
        {
            _facilityContext = facilityContext;
            _userManager = userManager;
        }

        // GET: Facilities
        public async Task<IActionResult> Index()
        {
            return View(await _facilityContext.Facilities.ToListAsync());
        }

        // GET: Facilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facility = await _facilityContext.Facilities
                .FirstOrDefaultAsync(m => m.FacilityID == id);
            if (facility == null)
            {
                return NotFound();
            }

            return View(facility);
        }

        // GET: Facilities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Facilities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("FacilityID,FacilityName,NumberOfClientsInGym,NumberOfClientsUsingWeightRoom," +
            "NumberOfClientsUsingCardioRoom,NumberOfClientsUsingStretchRoom,IsOpenDoorRequested,DoorOpened,DoorCloseTimer")] Facility facility)
        {
            if (ModelState.IsValid)
            {
                _facilityContext.Add(facility);
                await _facilityContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(facility);
        }

        // EDIT Workoutlog
        public async Task<IActionResult> LogWorkout(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facility = await _facilityContext.Facilities.FindAsync(id);


            if (facility == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user.Id == null)
            {
                return NotFound();
            }

            if (user.TimeLoggedWorkout.Date < DateTime.Today.Date) // checks midnight date time
            {
                user.HasLoggedWorkoutToday = false;
            }
            else if (user.TimeLoggedWorkout.Date >= DateTime.Today.Date)
            {
                user.HasLoggedWorkoutToday = true;
            }

            return View(facility);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogWorkout(int id,
            [Bind("FacilityID,FacilityName,NumberOfClientsInGym,NumberOfClientsUsingWeightRoom," +
            "NumberOfClientsUsingCardioRoom,NumberOfClientsUsingStretchRoom,IsOpenDoorRequested,DoorOpened,DoorCloseTimer," +
            "UserTrainingDuration, TotalTrainingDuration, WillUseWeightsRoom, WillUseCardioRoom, WillUseStretchRoom," +
            "HasLoggedWorkoutToday")] Facility facilityView)
        {
            var facility = await _facilityContext.Facilities.FindAsync(id);


            if (id != facility.FacilityID && id != facilityView.FacilityID)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var currentFacilityDetail = await _facilityContext.UsersInGymDetails.Where(f => f.UniqueEntryID == user.Id).FirstOrDefaultAsync();


            if (ModelState.IsValid)
            {
                try
                {

                    if (user.Id == null)
                    {
                        return NotFound();
                    }

                    // If the user is inside gym, ad does not skip, then save the data and go to the home page
                    else if (user.IsInsideGym)
                    {
                        // if the user logged workout before today, then we can log it
                        if (user.TimeLoggedWorkout.Date <= DateTime.Today.Date)
                        {
                            user.HasLoggedWorkoutToday = false;
                        }

                        // if the user has not logged workout today, then add the estimated training duration
                        if (!user.HasLoggedWorkoutToday)
                        {
                            facility.TotalTrainingDuration += facilityView.UserTrainingDuration;
                            user.HasLoggedWorkoutToday = true;
                            user.TimeLoggedWorkout = DateTime.Now;
                            currentFacilityDetail.EstimatedTrainingTime = facilityView.UserTrainingDuration;
                            // if the user will use any of these facilities of the gym, it will increase that counter
                            if (facilityView.WillUseWeightsRoom)
                            {
                                facility.NumberOfClientsUsingWeightRoom++;
                                user.WillUseWeightsRoom = true;
                            }
                            if (facilityView.WillUseCardioRoom)
                            {
                                facility.NumberOfClientsUsingCardioRoom++;
                                user.WillUseCardioRoom = true;
                            }
                            if (facilityView.WillUseStretchRoom)
                            {
                                facility.NumberOfClientsUsingStretchRoom++;
                                user.WillUseStretchRoom = true;
                            }
                        }

                        // redirect to home page after 3 seconds if user tries to submit form
                        else if (user.HasLoggedWorkoutToday)
                        {
                            System.Threading.Thread.Sleep(500);
                            return RedirectToAction("Index", "Home", new { id = user.DefaultGym });
                        }

                        _facilityContext.Update(facility); // check if updaing facilityview instead of facility will work
                        _facilityContext.Update(currentFacilityDetail); // check if updaing facilityview instead of facility will work
                        await _facilityContext.SaveChangesAsync();
                        await _userManager.UpdateAsync(user);
                    }
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilityExists(facilityView.FacilityID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home", new { id = user.DefaultGym });
            }
            return View(facilityView);
        }


        // GET: Facilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facility = await _facilityContext.Facilities.FindAsync(id);
            if (facility == null)
            {
                return NotFound();
            }
            return View(facility);
        }

        // POST: Facilities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FacilityID,FacilityName,NumberOfClientsInGym,NumberOfClientsUsingWeightRoom,NumberOfClientsUsingCardioRoom,NumberOfClientsUsingStretchRoom,IsOpenDoorRequested,DoorOpened,DoorCloseTimer")] Facility facility)
        {
            if (id != facility.FacilityID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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

        // GET: Facilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facility = await _facilityContext.Facilities
                .FirstOrDefaultAsync(m => m.FacilityID == id);
            if (facility == null)
            {
                return NotFound();
            }

            return View(facility);
        }

        // POST: Facilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facility = await _facilityContext.Facilities.FindAsync(id);

            _facilityContext.Facilities.Remove(facility);
            await _facilityContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacilityExists(int id)
        {
            return _facilityContext.Facilities.Any(e => e.FacilityID == id);
        }
    }
}
