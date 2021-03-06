using Microsoft.AspNetCore.Mvc;
using RpgCompendium.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace RpgCompendium.Controllers
{
  [Authorize]
  public class MonstersController : Controller
  {
    private readonly RpgCompendiumContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public MonstersController(UserManager<ApplicationUser> userManager, RpgCompendiumContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public async Task<ActionResult> Index()
    {
      List<Monster> model = _db.Monsters.ToList();
      return View(model);
      // var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      // var currentUser = await _userManager.FindByIdAsync(userId);
      // var userMonsters = _db.Monsters.Where(entry => entry.User.Id == currentUser.Id).ToList();
      // return View(userMonsters);
    }

    public ActionResult Create()
    {
      ViewBag.MainTypeId = new SelectList(_db.MainTypes, "MainTypeId", "MainTypeName");
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Monster monster, int MainTypeId)
    {
      // _db.Monsters.Add(monster);
      // if (MainTypeId != 0)
      // {
      //   _db.MonsterMainTypes.Add(new MonsterMainType() { MainTypeId = MainTypeId, MonsterId = monster.MonsterId });
      // }
      // _db.SaveChanges();
      // return RedirectToAction("Index");
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      monster.User = currentUser;
      _db.Monsters.Add(monster);
      if (MainTypeId != 0)
      {
        _db.MonsterMainTypes.Add(new MonsterMainType() { MainTypeId = MainTypeId, MonsterId = monster.MonsterId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id, string postAlert)
    {
      var thisMonster = _db.Monsters
          // .Include(monster => monster.MainTypes)
          // .ThenInclude(join => join.MainType)
          // .Include(monster => monster.Behaviors)
          // .ThenInclude(join => join.Behavior)
          // .Include(monster => monster.Armors)
          // .ThenInclude(join => join.Armor)
          .FirstOrDefault(monster => monster.MonsterId == id);
      ViewBag.postAlert = postAlert;
      return View(thisMonster);
    }

    public ActionResult Edit(int id)
    {
      var thisMonster = _db.Monsters.FirstOrDefault(monster => monster.MonsterId == id);
      return View(thisMonster);
    }

    [HttpPost]
    public ActionResult Edit(Monster monster)
    {
      _db.Entry(monster).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = monster.MonsterId });
    }

    public ActionResult Delete(int id)
    {
      var thisMonster = _db.Monsters.FirstOrDefault(monster => monster.MonsterId == id);
      return View(thisMonster);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisMonster = _db.Monsters.FirstOrDefault(monster => monster.MonsterId == id);
      _db.Monsters.Remove(thisMonster);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    // MAIN TYPES
    public ActionResult AddMainType(int id)
    {
      var thisMonster = _db.Monsters.FirstOrDefault(monsters => monsters.MonsterId == id);
      ViewBag.MainTypeId = new SelectList(_db.MainTypes, "MainTypeId", "MainTypeName");
      return View(thisMonster);
    }
    [HttpPost]
    public ActionResult AddMainType(Monster monster, int MainTypeId)
    {
      if (MainTypeId != 0)
      {
        _db.MonsterMainTypes.Add(new MonsterMainType() { MainTypeId = MainTypeId, MonsterId = monster.MonsterId });
      }
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = monster.MonsterId });
    }
    [HttpPost]
    public ActionResult DeleteMainType(int monsterId, int joinId)
    {
      var joinEntry = _db.MonsterMainTypes.FirstOrDefault(entry => entry.MonsterMainTypeId == joinId);
      _db.MonsterMainTypes.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = monsterId });
    }
    // BEHAVIORS
    public ActionResult AddBehavior(int id)
    {
      var thisMonster = _db.Monsters.FirstOrDefault(monsters => monsters.MonsterId == id);
      ViewBag.BehaviorId = new SelectList(_db.Behaviors, "BehaviorId", "BehaviorName");
      return View(thisMonster);
    }
    [HttpPost]
    public ActionResult AddBehavior(Monster monster, int BehaviorId)
    {
      if (BehaviorId != 0)
      {
        _db.MonsterBehaviors.Add(new MonsterBehavior() { BehaviorId = BehaviorId, MonsterId = monster.MonsterId });
      }
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = monster.MonsterId });
    }
    [HttpPost]
    public ActionResult DeleteBehavior(int monsterId, int joinId)
    {
      var joinEntry = _db.MonsterBehaviors.FirstOrDefault(entry => entry.MonsterBehaviorId == joinId);
      _db.MonsterBehaviors.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = monsterId });
    }

    // ARMOR

    public ActionResult AddArmor(int id)
    {
      var thisMonster = _db.Monsters
      // .Include(monster => monster.Armors)
      // .ThenInclude(join => join.Armor)
      .FirstOrDefault(monsters => monsters.MonsterId == id);
      ViewBag.ArmorId = new SelectList(_db.Armors, "ArmorId", "ArmorName");
      @System.Console.WriteLine("addarmor1 test count: "+thisMonster.Armors.Count.ToString());
      return View(thisMonster);
    }
    [HttpPost]
    public ActionResult AddArmor(Monster monster, int ArmorId)
    {
      var thisMonster = _db.Monsters
      // .Include(monsterMonster => monsterMonster.Armors)
      // .ThenInclude(join => join.Armor)
      .FirstOrDefault(monsters => monsters.MonsterId == monster.MonsterId);
      if (ArmorId != 0)
      {
        bool canEquip = true;
        var thisArmor = _db.Armors.FirstOrDefault(armors => armors.ArmorId == ArmorId);
        foreach (MonsterArmor monsterArmor in thisMonster.Armors)
        {          
          if (thisArmor.ArmorSlot == monsterArmor.Armor.ArmorSlot)
          {
            System.Console.WriteLine("cant equip!");
            canEquip = false;
          }
        }
        if (canEquip)
        {          
          _db.MonsterArmor.Add(new MonsterArmor() { ArmorId = ArmorId, MonsterId = monster.MonsterId, ArmorSlot = thisArmor.ArmorSlot });
          _db.SaveChanges();
          return RedirectToAction("Details", new { id = monster.MonsterId, postAlert = "Alert: You successfully equipped something!"});
        }
        else
        {
          _db.SaveChanges();
          return RedirectToAction("Details", new { id = monster.MonsterId, postAlert = "Alert: There is already something equipped there!"});
        }
      }
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = monster.MonsterId});
    }

    [HttpPost]
    public ActionResult DeleteArmor(int monsterId, int joinId)
    {
      var joinEntry = _db.MonsterArmor.FirstOrDefault(entry => entry.MonsterArmorId == joinId);
      _db.MonsterArmor.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = monsterId });
    }
  }
}