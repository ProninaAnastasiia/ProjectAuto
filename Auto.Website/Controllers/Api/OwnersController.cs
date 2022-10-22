using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Auto.Data;
using Auto.Data.Entities;
using Auto.Messages;
using Auto.Website.Models;
using Castle.Core.Internal;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;


namespace Auto.Website.Controllers.Api {
    [Route("api/[controller]")]
	[ApiController]
	public class OwnersController : ControllerBase {
		private readonly IAutoDatabase db;
		private readonly IBus bus;
		public OwnersController(IAutoDatabase db, IBus bus) {
			this.db = db;
			this.bus = bus;
		}

		private dynamic Paginate(string url, int index, int count, int total) {
			dynamic links = new ExpandoObject();
			links.self = new { href = url };
			links.final = new { href = $"{url}?index={total - (total % count)}&count={count}" };
			links.first = new { href = $"{url}?index=0&count={count}" };
			if (index > 0) links.previous = new { href = $"{url}?index={index - count}&count={count}" };
			if (index + count < total) links.next = new { href = $"{url}?index={index + count}&count={count}" };
			return links;
		}

		// GET: api/owners
		[HttpGet]
		[Produces("application/hal+json")]
		public IActionResult Get(int index = 0, int count = 10) {
			var items = db.ListOwners().Skip(index).Take(count);
			var total = db.CountOwners();
			var _links = Paginate("/api/owners", index, count, total);
			var _actions = new {
				create = new {
					method = "POST",
					type = "application/json",
					name = "Create a new owner",
					href = "/api/owners"
				},
				delete = new {
					method = "DELETE",
					name = "Delete a owner",
					href = "/api/owners/{id}"
				}
			};
			var result = new {
				_links, _actions, index, count, total, items
			};
			return Ok(result);
		}

		// GET api/owners/email
		[HttpGet("{id}")]
		public IActionResult Get(string id) {
			var owner = db.FindOwner(id);
			if (owner == default) return NotFound();
			var json = owner.ToDynamic();
			json._links = new {
				self = new { href = $"/api/owners/{id}" },
				vehicle = new { href = $"/api/vehicles/{owner.VehicleCode}" }
			};
			json._actions = new {
				update = new {
					method = "PUT",
					href = $"/api/owners/{id}",
					accept = "application/json"
				},
				delete = new {
					method = "DELETE",
					href = $"/api/owners/{id}"
				}
			};
			return Ok(json);
		}

		// POST api/owners
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] OwnerDto dto) {
			var vehicle = db.FindVehicle(dto.VehicleCode);
			var owner = new Owner {
				Email = dto.Email,
				FirstName = dto.FirstName,
				LastName = dto.LastName,
				PhoneNumber = dto.PhoneNumber,
				VehicleCode = dto.VehicleCode,
				Vehicle = vehicle,
			};
			db.CreateOwner(owner);
			PublishNewOwnerMessage(owner);
			return Ok(dto);
		}
		
		private void PublishNewOwnerMessage(Owner owner) {
			var message = new NewOwnerMessage() {
					FirstName = owner.FirstName,
					LastName = owner.LastName,
					VehicleName = owner.Vehicle?.Registration,
					Email = owner.Email,
					ListedAtUtc = DateTime.UtcNow
				};
				bus.PubSub.Publish(message);
	    }

		// PUT api/owners/email
		[HttpPut("{id}")]
		public IActionResult Put(string id, [FromBody] dynamic dto) {
			var vehicleHref = dto._links.vehicle.href;
			var vehicleCode = VehiclesController.ParseVehicleId(vehicleHref);
			var vehicle = db.FindVehicle(vehicleCode);
			var owner = new Owner {
				Email = id,
				FirstName = dto.FirstName,
				LastName = dto.LastName,
				PhoneNumber = dto.PhoneNumber,
				Vehicle = vehicle
			};
			db.UpdateOwner(owner);
			return Get(id);
		}

		// DELETE api/owners/email
		[HttpDelete("{id}")]
		public IActionResult Delete(string id) {
			var owner = db.FindOwner(id);
			if (owner == default) return NotFound();
			db.DeleteOwner(owner);
			return NoContent();
		}
	}
}
