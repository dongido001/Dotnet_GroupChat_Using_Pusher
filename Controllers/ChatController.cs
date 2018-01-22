
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GroupChat.Models;

namespace GroupSChat.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GroupChatContext _GroupContext;
        public ChatController(
          UserManager<ApplicationUser> userManager,
          GroupChatContext context
          )
        {
            _userManager = userManager;
            _GroupContext = context;
        }
        public IActionResult Index()
        {

            ViewData["UserGroups"] = _GroupContext.UserGroup.Where(
                                    gp => gp.UserName == _userManager.GetUserName(User) ).Join(_GroupContext.Groups, 
                                    ug => ug.GroupId, 
                                    g =>g.ID, (ug,g) =>
                                            new UserGroupViewModel{
                                                UserName = ug.UserName, 
                                                GroupId = g.ID,
                                                GroupName = g.GroupName
                                            }
                                ).ToList();
                                
            ViewData["Users"] = _userManager.Users;

            return View();
        }
    }
}