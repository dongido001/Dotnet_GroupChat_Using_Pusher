
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GroupChat.Models;
using Microsoft.AspNetCore.Identity;
using PusherServer;

namespace GroupChat.Controllers
{
    public class AuthController : Controller
    {
        private readonly GroupChatContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public AuthController( GroupChatContext context, UserManager<ApplicationUser> userManager){
             _context = context;
             _userManager = userManager;
        }

        [HttpPost]
        public IActionResult ChannelAuth(string channel_name, string socket_id)
        {

               if( !User.Identity.IsAuthenticated ) {
                
                  return new ContentResult { Content = "Access forbidden", ContentType = "application/json" };
               }

                var pusher = new Pusher(
                    "PUSHER_APP_ID",
                    "PUSHER_APP_KEY",
                    "PUSHER_APP_SECRET"
                );
              
               var auth = pusher.Authenticate( channel_name, socket_id );
               return new ContentResult { Content = auth.ToJson(), ContentType = "application/json" };
        }

    }
}