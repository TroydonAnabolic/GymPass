using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymPass.Data;
using GymPass.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using GymPass.Helpers;
using Amazon.S3;
using Amazon;
using Amazon.Rekognition;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using System.Text;

namespace GymPass.Controllers
{
    public class FacilitiesController : Controller
    {
        private readonly FacilityContext _facilityContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string bucketName = "gym-user-bucket-i";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSoutheast2;
        private readonly IAmazonS3 S3Client;
        private readonly ILogger<FacilitiesController> _logger;

        IAmazonRekognition AmazonRekognition { get; set; }

        public FacilitiesController(
            FacilityContext facilityContext,
            ILogger<FacilitiesController> logger,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment,
            IAmazonS3 s3Client,
            IAmazonRekognition amazonRekognition
            )
        {
            _facilityContext = facilityContext;
            _userManager = userManager;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            S3Client = s3Client;
            AmazonRekognition = amazonRekognition;
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
        [HttpGet]
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
            "HasLoggedWorkoutToday, IsCameraScanSuccessful, IsWithin10m")] Facility facilityView)
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

        // This post sends selected data from the drop down list to the server
        [HttpPost]
        [Route("Home/Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectTimeToEstimate(int idForUser, // TODO look into assigning the value for this id on argument
           [Bind("UserOutOfGymDetailsID, FacilityID, EstimatedTimeToCheck, UniqueEntryID")] UsersOutOfGymDetails usersOutOfGymDetails, [Bind] DateTime userDetails)
        {

            // get the user
            var user = await _userManager.GetUserAsync(User);

            if (user.Id == null)
            {
                return NotFound();
            }

            // set the PK entry to be the same as the one the usere created
            idForUser = _facilityContext.UsersOutofGymDetails.Where(o => o.UniqueEntryID == user.Id).FirstOrDefault().UsersOutOfGymDetailsID;
            // ensure we are only updating for the user that entered the drop down list value


            var currentUserDetail = _facilityContext.UsersOutofGymDetails.Where(o => o.UniqueEntryID == user.Id).FirstOrDefault();

            if (idForUser != currentUserDetail.UsersOutOfGymDetailsID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    currentUserDetail.EstimatedTimeToCheck = userDetails;

                    _facilityContext.Update(currentUserDetail);
                    await _facilityContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilityExists(usersOutOfGymDetails.FacilityID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction("Index", "Home", new { id = user.DefaultGym });
        }


        // POST: Home/Index/10

        [HttpPost]
        public async Task<IActionResult> CaptureAsync(string webcam)
        {
            var files = HttpContext.Request.Form.Files;
            StoreImageHelper storeImageHelper = new StoreImageHelper(_facilityContext)
            {

            };

            if (files != null)
            {
                await SaveImageAsync(files, storeImageHelper);

                return Json(true);
            }
            else
            {
                return Json(false);
            }

        }

        private async Task SaveImageAsync(IFormFileCollection files, StoreImageHelper storeImageHelper)
        {
            var fileTransferUtility = new TransferUtility(S3Client);
            var user = await _userManager.GetUserAsync(User);
            // set keyname to be user id, this way we can associate the one being scanned to the user ID to identify which item in the S3 bucket to check
            string keyName = $"{user.Id}.jpg";

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // Getting Filename  
                    var fileName = file.FileName;
                    // Unique filename "Guid"  
                    var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                    // Getting Extension  
                    var fileExtension = Path.GetExtension(fileName);
                    // Concating filename + fileExtension + a key ID based on userID (unique filename)  
                    var newFileName = $"{ myUniqueFileName}_{keyName}{fileExtension}";
                    //  Generating Path to store photo   
                    var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "CameraPhotos") + $@"\{newFileName}";

                    if (!string.IsNullOrEmpty(filepath))
                    {
                        // Storing Image in Folder  
                        storeImageHelper.StoreInFolder(file, filepath);
                    }

                    var imageBytes = System.IO.File.ReadAllBytes(filepath);
                    if (imageBytes != null)
                    {
                        // Storing Image in Folder  
                        await StoreInDatabaseAsync(imageBytes);
                    }
                    //TODO: delete image associated to the user once he leaves gym from db and folder

                    // Now save this in the S3 bucket to use for facial recognition
                    try
                    {
                        // grab the filePath from the image captured from the camera
                        using (var fileToUpload =
                            new FileStream(filepath, FileMode.Open, FileAccess.Read))
                        {
                            // upload it (if multiple are taken, they are overriden with the unique userID)
                            await fileTransferUtility.UploadAsync(fileToUpload,
                                                       bucketName, keyName);
                        }

                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation(e.Message);
                    }
                }
            }
        }

        /// <summary>  
        /// Saving captured image into database.  
        /// </summary>  
        /// <param name="imageBytes"></param>  
        private async Task StoreInDatabaseAsync(byte[] imageBytes)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (imageBytes != null)
                {
                    string base64String = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                    string imageUrl = string.Concat("data:image/jpg;base64,", base64String);

                    ImageStore imageStore = new ImageStore()
                    {
                        CreateDate = DateTime.Now,
                        ImageBase64String = imageUrl,
                        ImageId = 0,
                        UniqueID = user.Id
                    };

                    _facilityContext.ImageStore.Add(imageStore);
                    _facilityContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
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
        public async Task<IActionResult> Edit(int id,
           [Bind("FacilityID,FacilityName,NumberOfClientsInGym,NumberOfClientsUsingWeightRoom," +
            "NumberOfClientsUsingCardioRoom,NumberOfClientsUsingStretchRoom,IsOpenDoorRequested,DoorOpened,DoorCloseTimer," +
            "UserTrainingDuration, TotalTrainingDuration, WillUseWeightsRoom, WillUseCardioRoom, WillUseStretchRoom," +
            "HasLoggedWorkoutToday, IsCameraScanSuccessful, IsWithin10m")] Facility facility)
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
