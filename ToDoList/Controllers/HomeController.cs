using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
      private readonly ToDoListContext _db;
      private readonly UserManager<IdentityUser> _userManager;

      public HomeController(UserManager<IdentityUser> userManager, ToDoListContext db)
      {
        _userManager = userManager;
        _db = db;
      }

      [HttpGet("/")]
      public async Task<ActionResult> Index()
      {
        string userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        IdentityUser currentUser = await _userManager.FindByIdAsync(userId);
        Dictionary<string,object[]> model = new Dictionary<string, object[]>();
        if (currentUser != null)
        {
        Category[] cats = _db.Categories
                      .Where(entry => entry.User.Id == currentUser.Id)
                      .ToArray();
        
        model.Add("categories", cats);
 
        
          Item[] items = _db.Items
                      .Where(entry => entry.User.Id == currentUser.Id)
                      .ToArray();
        model.Add("items", items);
        }
        return View(model);
      }
    }
}