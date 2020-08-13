﻿using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using GymPass.Data;
using GymPass.Helpers;
using GymPass.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GymPass.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string bucketName = "gym-user-bucket-i";
        private readonly IAmazonS3 S3Client;
        private readonly ILogger<IndexModel> _logger;
        private readonly FacilityContext _facilityContext;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment webHostEnvironment,
            IAmazonS3 s3Client,
            FacilityContext facilityContext,
            ILogger<IndexModel> logger
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            S3Client = s3Client;
            _facilityContext = facilityContext;
            _logger = logger;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Test Latitude")]
            public string TestLat { get; set; }
            [Display(Name = "Test Longitude")]
            public string TestLong { get; set; }

            [Display(Name = "User Image")]
            public byte[] UserImage { get; set; } = new byte[1024];
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var userImage = user.UserImage;
            var testLat = user.TestLat;
            var tesLong = user.TestLong;

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                UserImage = userImage,
                TestLat = testLat,
                TestLong = tesLong,
            };
        }

        [BindProperty]
        public byte[] Image { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            string keyName = $"{user.FirstName}_{user.Id}_Target.jpg";

            await ReadImageAsync(keyName);

            await LoadAsync(user);
            return Page();
        }

        // method to read image of the user logged in
        private async Task ReadImageAsync(string keyName)
        {
           // string responseBody = "";
            try
            {
                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };
                using (GetObjectResponse response = await S3Client.GetObjectAsync(request))
                using (Stream responseStream = response.ResponseStream)
                

                using (StreamReader reader = new StreamReader(responseStream))
                {
                    var FileSize = responseStream.Length;
                    Image = new byte[(int)FileSize];
                    //string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
                    //string contentType = response.Headers["Content-Type"];

                    //Console.WriteLine("Object metadata, Title: {0}", title);
                    //Console.WriteLine("Content type: {0}", contentType);

                    //responseBody = reader.ReadToEnd(); // Now you process the response body.
                }

            }
            catch (AmazonS3Exception e)
            {
                // If bucket or object does not exist
                _logger.LogInformation("Error encountered ***. Message:'{0}' when reading object", e.Message);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Unknown encountered on server. Message:'{0}' when reading object", e.Message);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // save phone number
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await AddTargetImageToS3Bucket(user);

            var testLat = user.TestLat;
            var tesLong = user.TestLong;

            if (Input.TestLat != testLat)
            {
                user.TestLat = Input.TestLat;
                await _userManager.UpdateAsync(user);
            }
            if (Input.TestLong != tesLong)
            {
                user.TestLong = Input.TestLong;
                await _userManager.UpdateAsync(user);
            }


            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }


        private async Task AddTargetImageToS3Bucket(ApplicationUser user)
        {
            String keyName = $"{user.FirstName}_{user.Id}_Target.jpg";
            var files = HttpContext.Request.Form.Files; // this retrieves file from post request
            //IFormFile file = Request.Form.Files.FirstOrDefault();

            StoreImageHelper storeImageHelper = new StoreImageHelper(_facilityContext) { };
            var fileTransferUtility = new TransferUtility(S3Client);

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
                    var newFileName = $"{myUniqueFileName}_{keyName}{fileExtension}";
                    //  Generating Path to store photo   
                    var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "CameraPhotos") + $@"\{newFileName}";

                    if (!string.IsNullOrEmpty(filepath))
                        storeImageHelper.StoreInFolder(file, filepath);

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

                    // now delete the file to avoid cluttering (in real world, can possibly keep for logs)
                    if (!string.IsNullOrEmpty(filepath))
                        storeImageHelper.DeleteFromFolder(filepath);
                }
            }
        }
    }
}
