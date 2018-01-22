
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GroupChat.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using PusherServer;

namespace GroupChat.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class GroupController : Controller
    {
        private readonly GroupChatContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public GroupController(GroupChatContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IEnumerable<UserGroupViewModel> GetAll()
        {

            var groups = _context.UserGroup.Where(
                            gp => gp.UserName == _userManager.GetUserName(User) ).Join(_context.Groups, 
                            ug => ug.GroupId, 
                            g =>g.ID, (ug,g) =>
                                    new UserGroupViewModel(){
                                        UserName = ug.UserName, 
                                        GroupId = g.ID,
                                        GroupName = g.GroupName
                                    }
                    ).ToList();

            return groups;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NewGroupViewModel group)
        {
            if (group == null || group.GroupName == "")
            {
                return new ObjectResult(new { status = "error", message = "incomplete request" });
            }

            if( (_context.Groups.Any(gp => gp.GroupName == group.GroupName)) == true ){
                return new ObjectResult(new { status = "error", message = "group name already exist" });
            }

            Group newGroup = new Group{ GroupName = group.GroupName };
            // Insert this new group to the database...
            _context.Groups.Add(newGroup);
            _context.SaveChanges();

            //Insert into the user group table, group_id and user_id in the user_groups table...
            foreach( string UserName in group.UserNames)
            {
                _context.UserGroup.Add( new UserGroup{ UserName = UserName, GroupId = newGroup.ID } );
                _context.SaveChanges();
            }


            var options = new PusherOptions
            {
                Cluster = "eu",
                Encrypted = true
            };
            var pusher = new Pusher(
                "PUSHER_APP_ID",
                "PUSHER_APP_KEY",
                "PUSHER_APP_SECRET",
            options);
            var result = await pusher.TriggerAsync(
                "group_chat", //channel name
                "new_group", // event name
            new { newGroup } );


            return new ObjectResult(new { status = "success", data = newGroup });
        }

    }
}