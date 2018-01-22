
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
        public JsonResult ChannelAuth(string channel_name, string socket_id)
        {
            int group_id;

            if( !User.Identity.IsAuthenticated) {
            
                return Json( new { Content = "Access forbidden" } );
            }

            try
            {
                 group_id = Int32.Parse(channel_name.Replace("private-", ""));
            }
            catch (FormatException e)
            {
                return Json( new  { Content = e.Message } );
            }

            var IsInChannel = _context.UserGroup.Where(
                                            gb => gb.GroupId == group_id 
                                            && gb.UserName == _userManager.GetUserName(User) 
                                       ).Count();

            if( IsInChannel > 0){

                var pusher = new Pusher(
                    "PUSHER_APP_ID",
                    "PUSHER_APP_KEY",
                    "PUSHER_APP_SECRET"
                );
                
                var auth = pusher.Authenticate( channel_name, socket_id ).ToJson();
                return Json( new { Content = auth } );
            }

           return Json ( new { Content = "Access forbidden" } );

        }

    }
}