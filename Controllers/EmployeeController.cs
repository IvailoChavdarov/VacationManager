﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationManager.Data;
using VacationManager.Models;
using VacationManager.ViewModels;

namespace VacationManager.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;
        public EmployeeController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }

        //shows view with all users and their roles
        public async Task<IActionResult> Index()
        {
            List<AppUser> users = new List<AppUser>();
            //sets role full name
            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, "CEO"))
                {
                    user.MainRole = "Chief Executive Officer";
                }
                else if (await _userManager.IsInRoleAsync(user, "Team Lead"))
                {
                    user.MainRole = "Leader of a team";
                }
                else if (await _userManager.IsInRoleAsync(user, "Developer"))
                {
                    user.MainRole = "Developer in a team";
                }
                else
                {
                    user.MainRole = "";
                }
                users.Add(user);
            }
            return View(users);
        }

        //deletes user by their id
        [Authorize(Roles = "CEO")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            AppUser userToDelete = await _userManager.FindByIdAsync(id);
            if (userToDelete == null)
            {
                return NotFound();
            }
            await _userManager.DeleteAsync(userToDelete);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //shows view with details for user and related data
        public async Task<IActionResult> Details(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            EmployeeDetailsViewModel data = new EmployeeDetailsViewModel(); 

            //shows team if user is in one
            if (user.TeamId != null)
            {
                user.Team = _db.Teams
                    .Where(x => x.Id == user.TeamId)
                    .FirstOrDefault();
                if (user.Team == null)
                {
                    return NotFound();
                }
                data.TeamId = user.Team.Id;
            }
            else
            {
                //gets data for available teams for user to join
                data.Teams = _db.Teams.ToList();
            }

            data.User = user;
            data.UserId = id;

            return View(data);
        }

        //adds user to team
        [Authorize(Roles = "CEO")]
        [HttpPost]
        public async Task<IActionResult> AddToTeam(EmployeeDetailsViewModel data)
        {
            AppUser user = await _userManager.FindByIdAsync(data.UserId);
            if (user == null)
            {
                return NotFound();
            }

            Team team = await _db.Teams
                .Where(x => x.Id == data.TeamId)
                .FirstOrDefaultAsync();

            if (team == null)
            {
                return NotFound();
            }

            user.TeamId = data.TeamId;

            //sets user role to developer in a team
            if (await _userManager.IsInRoleAsync(user, "Unassigned"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Unassigned");
            }
            if (!await _userManager.IsInRoleAsync(user, "Developer"))
            {
                await _userManager.AddToRoleAsync(user, "Developer");
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new {id=data.UserId});
        }

        //removes user from team
        [Authorize(Roles = "CEO")]
        [HttpPost]
        public async Task<IActionResult> RemoveFromTeam(EmployeeDetailsViewModel data)
        {
            AppUser user = await _userManager.FindByIdAsync(data.UserId);
            if (user == null)
            {
                return NotFound();
            }

            Team team = await _db.Teams
                .Where(x => x.Id == data.TeamId)
                .FirstOrDefaultAsync();

            if (team == null)
            {
                return NotFound();
            }

            user.TeamId = null;

            //sets user role to unassigned
            if (!await _userManager.IsInRoleAsync(user, "Unassigned"))
            {
                await _userManager.AddToRoleAsync(user, "Unassigned");
            }
            if (await _userManager.IsInRoleAsync(user, "Developer"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Developer");
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new { id = data.UserId });
        }

        //shows view with form for user editing
        [Authorize(Roles = "CEO")]
        public async Task<IActionResult> Edit(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        //updates user data
        [Authorize(Roles = "CEO")]
        [HttpPost]
        public async Task<IActionResult> Edit(AppUser data)
        {
            AppUser oldUserData = await _userManager.FindByIdAsync(data.Id);
            if (oldUserData == null)
            {
                return NotFound();
            }

            //sets email and username
            if (oldUserData.Email != data.Email)
            {
                await _userManager.SetEmailAsync(oldUserData, data.Email);
                await _userManager.SetUserNameAsync(oldUserData, data.Email);
            }

            if (oldUserData.PhoneNumber != data.PhoneNumber)
            {
                await _userManager.SetPhoneNumberAsync(oldUserData, data.PhoneNumber);
            }

            if (data.FirstName != oldUserData.FirstName)
            {
                oldUserData.FirstName = data.FirstName;
            }

            if (data.LastName != oldUserData.LastName)
            {
                oldUserData.LastName = data.LastName;
            }

            await _userManager.UpdateAsync(oldUserData);

            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new { id = data.Id });
        }
    }
}
