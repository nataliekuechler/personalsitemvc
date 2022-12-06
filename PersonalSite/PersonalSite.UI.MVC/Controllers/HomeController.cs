using Microsoft.AspNetCore.Mvc;
using PersonalSite.UI.MVC.Models;
using System.Diagnostics;
using MimeKit;
using MailKit.Net.Smtp;


namespace PersonalSite.UI.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Resume()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel cvm)
        {
            //When a class has validation attributes, that validation shoud be checked
            //BEFORE attempting to process any of the data they provided.

            if (!ModelState.IsValid)
            {
                //Send the user back to the form. We can pass the object to the View
                //so the form will contain the original information they provided.

                return View(cvm);
            }

            //To handle sending the email, we'll need to install a NuGet Package
            //and add a few using statements. We can do this with the following steps:

            #region Email Setup Steps & Email Info

            //1. Go to Tools > NuGet Package Manager > Manage NuGet Packages for Solution
            //2. Go to the Browse tab and search for NETCore.MailKit
            //3. Click NETCore.MailKit
            //4. On the right, check the box next to the CORE1 project, then click "Install"
            //5. Once installed, return here
            //6. Add the following using statements & comments:
            //      - using MimeKit; //Added for access to MimeMessage class
            //      - using MailKit.Net.Smtp; //Added for access to SmtpClient class
            //7. Once added, return here to continue coding email functionality

            //MIME - Multipurpose Internet Mail Extensions - Allows email to contain
            //information other than ASCII, including audio, video, images, and HTML

            //SMTP - Secure Mail Transfer Protocol - An internet protocol (similar to HTTP)
            //that specializes in the collection & transfer of email data

            #endregion

            //Create the format for the message content we will receive from the form
            string message = $"You have receieved a new email from your site's contact form!<br />" +
                $"Sender: {cvm.Name}<br />Email: {cvm.Email}<br />Subject: {cvm.Subject}<br />Message: {cvm.Message}";

            //Create a MimeMessage object to assist with storing/transporting the email
            //information from the contact form
            var mm = new MimeMessage();

            //We can access the credentials for our email user from the appsettings.json file:
            mm.From.Add(new MailboxAddress("Sender", _config.GetValue<string>("Credentials:Email:User")));

            //The recipient of this email will be our personal email address, also stored in appsettings.json
            mm.To.Add(new MailboxAddress("Personal", _config.GetValue<string>("Credentials:Email:Recipient")));

            //The subject of the message will be the one provided by the user, stored in the cvm object.
            mm.Subject = cvm.Subject;

            //The body of the message will be the string we created above
            mm.Body = new TextPart("HTML") { Text = message };

            //We can set the priority of the message as "Urgent" so it will be flagged in our email client
            mm.Priority = MessagePriority.Urgent;

            //We can also add the user's provided email address to the list of ReplyTo addresses
            //so our replies can be sent directly to them instead of to our email user
            mm.ReplyTo.Add(new MailboxAddress("User", cvm.Email));

            //The using directive will create the SmtpClient object used to send the email.
            //Once all of the code inside the using directive's scope has been executed, it
            //will close any open connections and dispose of the object for us.
            using (var client = new SmtpClient())
            {
                //Connect to the mail server using the credentials in our appsettings.json
                //client.Connect(_config.GetValue<string>("Credentials:Email:Client"));
                client.Connect(_config.GetValue<string>("Credentials:Email:Client"), 8889);

                //Log in to the mail server using the credentials for our email user
                client.Authenticate(

                    //Username
                    _config.GetValue<string>("Credentials:Email:User"),

                    //Password
                    _config.GetValue<string>("Credentials:Email:Password")

                    );

                //It's possible the mail server may be down when the user attemps to contact us,
                //or we may have issues in our code. So let's wrap our code to send the message
                //in a try/catch block.

                try
                {
                    //Try to send the email
                    client.Send(mm);
                }
                catch (Exception ex)
                {
                    //If there's an issue, we can store an error message in a ViewBag variable
                    //to be displayed in the View
                    ViewBag.ErrorMessage = $"There was an error processing your request." +
                        $"Please try again later.<br />Error Message: {ex.StackTrace}";

                    //Return the user to the View with their form information intact
                    return View(cvm);

                }

            }

            //If all goes well, return a View that displays a confirmation to the user that
            //their email was sent. 


            return View("EmailConfirmation", cvm);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}