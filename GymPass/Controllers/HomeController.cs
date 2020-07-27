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
using System.Collections.Generic;
using System.IO;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;

namespace GymPass.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly FacilityContext _facilityContext;
        IAmazonS3 S3Client { get; set; }
        IAmazonRekognition AmazonRekognition { get; set; } // access amazon rekognition API ref

        public HomeController(
            UserManager<ApplicationUser> userManager,
            ILogger<HomeController> logger,
            FacilityContext facilityContext,
            IAmazonS3 s3Client,
            IAmazonRekognition amazonRekognition
            )
        {
            _userManager = userManager;
            _logger = logger;
            _facilityContext = facilityContext;
            S3Client = s3Client;
            AmazonRekognition = amazonRekognition;
        }

        [BindProperty]
        UsersInGymDetail UsersInGymDetail { get; set; }
       
        // GET: Home/Index/1
        // TODO: We begin using id = 1 for now, later will implement dynamically changing this ID number, if it is null then redirect to action choose gym
        [Authorize]
        public async Task<IActionResult> Index(int? id)
        {
            // Get the default gym for a user and set it to be the Id for the gym being edited
            var user = await _userManager.GetUserAsync(User);
            ViewBag.EstimatedNumberInGym = 0;
            ViewBag.IsOpenDoorRequested = false;

            // initiall set the access view bag to false, as this will prevent null exception

            if (user.Id == null)
            {
                return NotFound();
            }

            id = user.DefaultGym;

            if (id == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            var facility = await _facilityContext.Facilities.FindAsync(id);
            var facilityDetails = await _facilityContext.UsersInGymDetails.ToListAsync();
            UsersInGymDetail = await _facilityContext.UsersInGymDetails
                .Where(f => f.UniqueEntryID == user.Id).FirstOrDefaultAsync();

            // get facial recognition details to show
            ViewBag.IsSmiling = _facilityContext.UsersInGymDetails.Where(o => o.UniqueEntryID == user.Id).FirstOrDefault().IsSmiling;
            ViewBag.Gender = _facilityContext.UsersInGymDetails.Where(o => o.UniqueEntryID == user.Id).FirstOrDefault().Gender;
            ViewBag.AgeRangeLow = _facilityContext.UsersInGymDetails.Where(o => o.UniqueEntryID == user.Id).FirstOrDefault().AgeRangeLow;
            ViewBag.AgeRangeHigh = _facilityContext.UsersInGymDetails.Where(o => o.UniqueEntryID == user.Id).FirstOrDefault().AgeRangeHigh;


            DateTime estimatedExitTime = DateTime.Now;

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

            // Decide to increase or decrease the estimated numbers in gym
            // if there are entries get the estimated exit time
            if (facilityDetails.Count > 0)
            {
                estimatedExitTime = UsersInGymDetail.TimeAccessGranted.Add(UsersInGymDetail.EstimatedTrainingTime);

                // for each user logged into the gym, increment or decrement based on time entered
                foreach (var userInGym in facilityDetails)
                {
                    // if current time has a lesser value, training has not finished, so we add to the count of estimated users in the facilities table
                    if (DateTime.Now < estimatedExitTime)
                    {
                        // if the current user is still within his estimated training time, then add estimated number of gym users list
                        ViewBag.EstimatedNumberInGym++;
                    }
                    else if (DateTime.Now > estimatedExitTime)
                        ViewBag.EstimatedNumberInGym--;
                }
            }

            // if time since the date where user was denied, is more than 5 seconds, then access denied msg received is not received
            if (DateTime.Now <= (user.TimeAccessDenied.AddSeconds(10)))
                ViewBag.AccessDeniedMsgRecieved = false;

            return View(facility);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, [Bind("FacilityID,FacilityName,NumberOfClientsUsingWeightRoom,NumberOfClientsUsingCardioRoom," +
            "NumberOfClientsUsingStretchRoom,IsOpenDoorRequested,DoorOpened,DoorCloseTimer,IsCameraScanSuccessful, IsWithin10m")] Facility facilityView,
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
            bool enteredGym = false;

            if (id != facility.FacilityID || id != facilityView.FacilityID)
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
                    enteredGym = await DetermineEnterOrExitGym(facilityView, user, facility, facilityDetails, currentFacilityDetail, currentFacilityDetailDb, enteredGym);
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
                // if the user has logged in successfully and not logged workout, then send to questionnaire for now, later will use AJAX to post questionaire here, and have option to later log in workout
                return (user.AccessGrantedToFacility) && (!user.HasLoggedWorkoutToday) ? RedirectToAction("LogWorkout", "Facilities", new { id = user.DefaultGym }) : RedirectToAction(nameof(Index));
            }
            return View(facility);
        }

        private async Task<bool> DetermineEnterOrExitGym(Facility facilityView, ApplicationUser user, Facility facility, List<UsersInGymDetail> facilityDetails, UsersInGymDetail currentFacilityDetail,
            UsersInGymDetail currentFacilityDetailDb, bool enteredGym)
        {
            if (facilityView.IsOpenDoorRequested)
            {
                // ----------------- Begin Facial recognition----------------------
                float similarityThreshold = 70F;
                String photo = "business-atire.jpg";
                String targetImage = "fbPic.jpg";
                // String targetImage = "pris-face.jpg"; TODO: Try using this as a test if I cannot get user input
                String bucket = "gym-user-bucket-i";

                // ------------------------------ Recognition from image
                try
                {

                    Image imageSource = new Image()
                    {
                        S3Object = new S3Object()
                        {
                            Name = photo,
                            Bucket = bucket
                        },
                    };

                    Image imageTarget = new Image()
                    {
                        S3Object = new S3Object()
                        {
                            Name = targetImage,
                            Bucket = bucket
                        },
                    };

                    // TODO: Set the target to be user upload
                    //using (FileStream fs = new FileStream(targetImage, FileMode.Open, FileAccess.Read))
                    //{
                    //    byte[] data = new byte[fs.Length];
                    //    data = new byte[fs.Length];
                    //    fs.Read(data, 0, (int)fs.Length);
                    //    imageTarget.Bytes = new MemoryStream(data);
                    //}

                    CompareFacesRequest compareFacesRequest = new CompareFacesRequest()
                    {
                        SourceImage = imageSource,
                        TargetImage = imageTarget,
                        SimilarityThreshold = similarityThreshold
                    };

                    // detect face features of img scanned
                    //  DetectFacesResponse detectFacesResponse = await AmazonRekognition.DetectFacesAsync(detectFacesRequest);

                    // Call operation
                    CompareFacesResponse compareFacesResponse = await AmazonRekognition.CompareFacesAsync(compareFacesRequest);

                    // Display results
                    foreach (CompareFacesMatch match in compareFacesResponse.FaceMatches)
                    {
                        ComparedFace face = match.Face;
                        // if confidence for similarity is over 90 then grant access
                        if (match.Similarity > 90)
                        {
                            // if there is a match set scan success and display to the view the match
                            user.IsCameraScanSuccessful = true;
                        }
                        else
                        {
                            ViewBag.MatchResult = "Facial Match Failed!";
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e.Message);
                }

                // ------------------------------ Now add detect from video


                // ------------------------------ Now add get facial details to display in the view.
                DetectFacesRequest detectFacesRequest = new DetectFacesRequest()
                {
                    Image = new Image()
                    {
                        S3Object = new S3Object()
                        {
                            Name = targetImage,
                            Bucket = bucket
                        },
                    },
                    // Attributes can be "ALL" or "DEFAULT". 
                    // "DEFAULT": BoundingBox, Confidence, Landmarks, Pose, and Quality.
                    // "ALL": See https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/Rekognition/TFaceDetail.html
                    Attributes = new List<String>() { "ALL" }
                };

                try
                {
                    DetectFacesResponse detectFacesResponse = await AmazonRekognition.DetectFacesAsync(detectFacesRequest);
                    bool hasAll = detectFacesRequest.Attributes.Contains("ALL");
                    foreach (FaceDetail face in detectFacesResponse.FaceDetails)
                    {
                        if (hasAll) // consider removing of only certain features can be detected.
                        {
                            currentFacilityDetail.IsSmiling = face.Smile.Value;
                            currentFacilityDetail.Gender = face.Gender.Value.ToString();
                            currentFacilityDetail.AgeRangeLow = face.AgeRange.Low;
                            currentFacilityDetail.AgeRangeHigh = face.AgeRange.High;
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInformation(e.Message);
                }

                // --------------------------------------------------------end facial recognition-------------------------------------------------------------

                // gathers location scan results
                if (facilityView.IsWithin10m)
                    user.IsWithin10m = true;

                // if camera scan and location check is true, and user is not in the gym, then we open the door, and access granted is true
                if (user.IsWithin10m && user.IsCameraScanSuccessful && !user.IsInsideGym)
                {
                    user.AccessGrantedToFacility = true;
                    ViewBag.AccessGrantedToFacility = true;
                }
                // if camera scan is not successful
                else if (!facilityView.IsCameraScanSuccessful && !user.IsWithin10m)
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
                        if (facilityDetails.Count() > 0)
                        {
                            _facilityContext.UsersInGymDetails.Remove(currentFacilityDetailDb);
                        }

                        facilityView.IsCameraScanSuccessful = false;
                        user.IsWithin10m = false;
                        user.IsCameraScanSuccessful = false;
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
                    System.Threading.Thread.Sleep(200);
                }
                // delay 10s when entering
              if (!user.IsInsideGym) System.Threading.Thread.Sleep(200);

                // When 5 second timer finishes, we close the door again automatically
                 facility.IsOpenDoorRequested = false;
                ViewBag.IsOpenDoorRequested = false;
                facility.DoorOpened = false;
                _facilityContext.Update(facility);

                // if we are entering gym, use the new facility object, if we are leaving, use the facility detail using Db values.
                if (enteredGym) _facilityContext.Update(currentFacilityDetail);
                // else if (leftGym) _facilityContext.Update(currentFacilityDetailDb);

                // after a facility exist, then we can update facility to avoid foreign key constraint?
                await _facilityContext.SaveChangesAsync();
                await _userManager.UpdateAsync(user);
            }

            return enteredGym;
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
