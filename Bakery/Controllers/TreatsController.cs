using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Bakery.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Bakery.Controllers
{

  [AllowAnonymous] // added for Identity
  //AllowAnonymous allows read but not write by non users
  //Authorize forbids read or write to non users
  public class TreatsController : Controller
  {
    private readonly BakeryContext _db;
    private readonly UserManager<ApplicationUser> _userManager; // added for Identity

    public TreatsController(UserManager<ApplicationUser> userManager, BakeryContext db) // added for Identity
    {
      _userManager = userManager; // added for Identity
      _db = db;
    }
// update this for Identity by using the userId to get a specific part of the database
    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; //? checks for a null value and wont pass if its not there
      var currentUser = await _userManager.FindByIdAsync(userId);
      var userTreats = _db.Treats.Where(entry => entry.User.Id == currentUser.Id).ToList();
      return View(userTreats);
    }
// all of this code must be updated

    public ActionResult Create()
    {
      ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Name");
      return View();
    }

// update the code below with userId for posting a create
    [HttpPost] 
    public async Task<ActionResult> Create(Treat Treat, int FlavorId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      Treat.User = currentUser;
      _db.Treats.Add(Treat);
      _db.SaveChanges();
      if (FlavorId != 0)
      {
          _db.FlavorTreat.Add(new FlavorTreat() { FlavorId = FlavorId, TreatId = Treat.TreatId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisTreat = _db.Treats
          .Include(Treat => Treat.JoinEntities)
          .ThenInclude(join => join.Flavor)
          .FirstOrDefault(Treat => Treat.TreatId == id);
      return View(thisTreat);
    }

    public ActionResult Edit(int id)
    {
      var thisTreat = _db.Treats.FirstOrDefault(Treat => Treat.TreatId == id);
      ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Name");
      return View(thisTreat);
    }

    [HttpPost]
    public ActionResult Edit(Treat Treat, int FlavorId)
    {
      if (FlavorId != 0)
      {
        _db.FlavorTreat.Add(new FlavorTreat() { FlavorId = FlavorId, TreatId = Treat.TreatId });
      }
      _db.Entry(Treat).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    // this is the getter, but the getter is auto taken care of
    public ActionResult AddFlavor(int id)
    {
      var thisTreat = _db.Treats.FirstOrDefault(Treat => Treat.TreatId == id);
      ViewBag.FlavorId = new SelectList(_db.Flavors, "FlavorId", "Name");
      return View(thisTreat);
    }

    [HttpPost]
    public ActionResult AddFlavor(Treat Treat, int FlavorId)
    {
      if (FlavorId != 0)
      {
      _db.FlavorTreat.Add(new FlavorTreat() { FlavorId = FlavorId, TreatId = Treat.TreatId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisTreat = _db.Treats.FirstOrDefault(Treat => Treat.TreatId == id);
      return View(thisTreat);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisTreat = _db.Treats.FirstOrDefault(Treat => Treat.TreatId == id);
      _db.Treats.Remove(thisTreat);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
// submit to this action, and all I need is the joined table ID
    [HttpPost]
    public ActionResult DeleteFlavor(int joinId)
    {
      var joinEntry = _db.FlavorTreat.FirstOrDefault(entry => entry.FlavorTreatId == joinId);
      _db.FlavorTreat.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}