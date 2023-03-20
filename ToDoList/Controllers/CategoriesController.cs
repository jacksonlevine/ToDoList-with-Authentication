using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ToDoList.Controllers
{
  [Authorize]
  public class CategoriesController : Controller
  {
    private readonly ToDoListContext _db;

    private readonly UserManager<IdentityUser> _userManager;

    public CategoriesController(UserManager<IdentityUser> userManager, ToDoListContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public async Task<ActionResult> Index()
    {
      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      IdentityUser currentUser = await _userManager.FindByIdAsync(userId);

      List<Category> model = _db.Categories
                                .Where(category => category.User.Id == currentUser.Id)
                                .Include(category => category.Items)
                                .ToList();
      return View(model);
    }

    public ActionResult Create()
    {

      ViewBag.ItemId = new SelectList(_db.Items, "ItemId", "Name");
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Category category)
    {
      // if (!ModelState.IsValid)
      // {applicationuser
      //   ViewBag.ItemId = new SelectList(_db.Items, "ItemId", "Name");
      //   return View();
      // }
      // else
      // {
     
      //   string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      //   IdentityUser currentUser = await _userManager.FindByIdAsync(userId);
      //   category.User = currentUser;
      //   _db.Categories.Add(category);
      //   _db.SaveChanges();
      //   return RedirectToAction("Index");    
      // }

      string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      IdentityUser currentUser = await _userManager.FindByIdAsync(userId);
      category.User = currentUser;
      _db.Categories.Add(category);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      Category thisCategory = _db.Categories
                                .Include(cat => cat.Items)
                                .ThenInclude(item => item.JoinEntities)
                                .ThenInclude(join => join.Tag)
                                .FirstOrDefault(category => category.CategoryId == id);
      return View(thisCategory);
    }

    public ActionResult Edit(int id)
    {
      Category thisCategory = _db.Categories.FirstOrDefault(category => category.CategoryId == id);
      return View(thisCategory);
    }

    [HttpPost]
    public ActionResult Edit(Category category)
    {
      _db.Categories.Update(category);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      Category thisCategory = _db.Categories.FirstOrDefault(category => category.CategoryId == id);
      return View(thisCategory);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      Category thisCategory = _db.Categories.FirstOrDefault(category => category.CategoryId == id);
      _db.Categories.Remove(thisCategory);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}