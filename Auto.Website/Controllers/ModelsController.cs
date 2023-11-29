using Auto.Data;
using Microsoft.AspNetCore.Mvc;

namespace Auto.WebSite.Controllers;

public class ModelsController : Controller {
    private readonly IAutoDatabase db;

    public ModelsController(IAutoDatabase db) {
        this.db = db;
    }

    public IActionResult Index() {
        var models = db.ListModels();
        return View(models);
    }
}