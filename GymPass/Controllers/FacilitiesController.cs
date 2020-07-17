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
        public async Task<IActionResult> Create([Bind("FacilityID,FacilityName,NumberOfClientsInGym,NumberOfClientsUsingWeightRoom,NumberOfClientsUsingCardioRoom,NumberOfClientsUsingStretchRoom,IsOpenDoorRequested,DoorOpened,DoorCloseTimer")] Facility facility)
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
        public async Task<IActionResult> EditWorkoutLog(int? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWorkoutLog(int id, [Bind("FacilityID,FacilityName,NumberOfClientsInGym,NumberOfClientsUsingWeightRoom,NumberOfClientsUsingCardioRoom,NumberOfClientsUsingStretchRoom,IsOpenDoorRequested,DoorOpened,DoorCloseTimer")] Facility facilityView)
        {
            var facility = await _facilityContext.Facilities.FindAsync(id);


            if (id != facility.FacilityID && id != facilityView.FacilityID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);

                    if (user.Id == null)
                    {
                        return NotFound();
                    }

                    // if the user chooses to skip then we go to the home page TODO: Change viewbag for skipworkout in the view if possible otherwise use facility
                    if (ViewBag.SkipWorkoutLog) return RedirectToAction(nameof(Index), "Home");

                    // If the user is inside gym, ad does not skip, then save the data and go to the home page
                    else if (user.IsInsideGym)
                    {
                        // TODO: create logic for working out avg training time and inputted expected time find out expected amount of ppl
                        facility.TotalTrainingDuration += facilityView.UserTrainingDuration;

                        _facilityContext.Update(facilityView); // check if updaing facilityview instead of facility will work
                        await _facilityContext.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index), "Home");
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
