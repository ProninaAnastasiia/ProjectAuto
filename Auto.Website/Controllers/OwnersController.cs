using System;
using Auto.Data;
using Microsoft.AspNetCore.Mvc;

namespace Auto.WebSite.Controllers;

public class OwnersController : Controller
{
    private readonly IAutoDatabase db;

    public OwnersController(IAutoDatabase db) {
        this.db = db;
    }
    
    public IActionResult Owner(string id)
    {
        var owner = db.FindOwner(id);
        Console.WriteLine(owner.Email);
        return View(owner);
    }
    
}