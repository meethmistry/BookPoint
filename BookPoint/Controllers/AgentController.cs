using BookPoint.Models;
using BookPoint.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookPoint.Controllers
{
    public class AgentController : Controller
    {
        private readonly ApplicationDBContext _dbContext;

        public AgentController(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public IActionResult DeliveryAgents()
        {
            var agents = _dbContext.Agents.ToList();
            var users = _dbContext.Users.ToList();

            var viewModel = new UserAgentViewModel
            {
                Agents = agents,
                Users = users
            };

            return View(viewModel);
        }

        // Return partial view for AJAX refresh
        public IActionResult Agents()
        {
            var agents = _dbContext.Agents.ToList();
            var users = _dbContext.Users.ToList();

            var viewModel = new UserAgentViewModel
            {
                Agents = agents,
                Users = users
            };

            return PartialView(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEditAgent(UserAgentViewModel vm)
        {
            if (vm.Agent.Id == 0)
            {
                // Check for duplicate email
                if (_dbContext.Users.Any(u => u.Email.ToLower() == vm.User.Email.ToLower()))
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Email already exists. Please use a different email." });
                    }
                    TempData["AgentError"] = "Email already exists. Please use a different email.";
                    TempData["CurrentTab"] = "Delivery Agents";
                    return RedirectToAction("Dashboard", "Dashboard");
                }

                // Create User
                var user = new UserModel
                {
                    Email = vm.User.Email,
                    Password = vm.Password,
                    Role = "Agent",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();

                // Create Agent
                var agent = new AgentModel
                {
                    UID = user.Id,
                    UserName = vm.Agent.UserName,
                    Phone = vm.Agent.Phone,
                    User = user
                };

                _dbContext.Agents.Add(agent);
                _dbContext.SaveChanges();
            }
            else
            {
                var agent = _dbContext.Agents.FirstOrDefault(a => a.Id == vm.Agent.Id);
                var user = _dbContext.Users.FirstOrDefault(u => u.Id == agent!.UID);

                // Check for duplicate email (excluding current user)
                if (_dbContext.Users.Any(u => u.Email.ToLower() == vm.User.Email.ToLower() && u.Id != user!.Id))
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Email already exists. Please use a different email." });
                    }
                    TempData["AgentError"] = "Email already exists. Please use a different email.";
                    TempData["CurrentTab"] = "Delivery Agents";
                    return RedirectToAction("Dashboard", "Dashboard");
                }

                if (user != null && agent != null)
                {
                    // Update User
                    user.Email = vm.User.Email;
                    user.Password = vm.Password;

                    // Update Agent
                    agent.UserName = vm.Agent.UserName;
                    agent.Phone = vm.Agent.Phone;

                    _dbContext.Users.Update(user);
                    _dbContext.Agents.Update(agent);
                    _dbContext.SaveChanges();
                }
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            TempData["CurrentTab"] = "Delivery Agents";
            return RedirectToAction("Dashboard", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var agent = _dbContext.Agents.FirstOrDefault(a => a.Id == id);
            if (agent == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Agent not found." });
                }
                return NotFound();
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Id == agent.UID);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                _dbContext.Users.Update(user);
                _dbContext.SaveChanges();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, isActive = user.IsActive });
            }

            TempData["CurrentTab"] = "Delivery Agents";
            return RedirectToAction("Dashboard", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUserAndAgent(int id)
        {
            var agent = _dbContext.Agents.FirstOrDefault(a => a.Id == id);
            if (agent == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Agent not found." });
                }
                return NotFound();
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Id == agent.UID);

            _dbContext.Agents.Remove(agent);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
            }

            _dbContext.SaveChanges();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            TempData["CurrentTab"] = "Delivery Agents";
            return RedirectToAction("Dashboard", "Dashboard");
        }
    }
}