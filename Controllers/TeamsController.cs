﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationManager.Data;
using VacationManager.Models;
using VacationManager.ViewModels;

namespace VacationManager.Controllers
{
    public class TeamsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;
        public TeamsController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }
        //public IActionResult Index(int? page = 1, char? pageSize = 's')
        //{
        //    List<Team> data = new List<Team>();
        //    if (page.Value!=1)
        //    {


        //    }
        //    return View(_db.Teams);
        //}
        public async Task<IActionResult> Index()
        {
            List<Team> data = new List<Team>();
            foreach (var item in _db.Teams
                .Include(x => x.Project)
                .ToList())
            {
                item.Leader = await _db.Users
                    .Where(x => x.Id == item.LeaderId)
                    .FirstOrDefaultAsync();

                if (item.Leader == null)
                {
                    return NotFound();
                }

                data.Add(item);
            }
            
            return View(data);
        }

        public async Task<IActionResult> Details(int id) {
            TeamDetailsViewModel data = new TeamDetailsViewModel();

            data.TeamData = _db.Teams
                .Where(x => x.Id == id)
                .Include(x => x.Members)
                .FirstOrDefault();

            if (data.TeamData == null)
            {
                return NotFound();
            }

            data.UsersNotInTeam = _db.Users
                .Where(x=>x.TeamId==null)
                .ToList();

            data.TeamId = id;

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> AddToTeam(TeamDetailsViewModel data)
        {
            Team team = _db.Teams
            .Where(x => x.Id == data.TeamId)
            .FirstOrDefault();

            if (team == null)
            {
                return NotFound();
            }

            AppUser userToAdd = _db.Users
                .Where(x=>x.Id == data.UserToAddId)
                .FirstOrDefault();

            if (userToAdd == null)
            {
                return NotFound();
            }

            userToAdd.TeamId = data.TeamId;

            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new {id=data.TeamId});
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromTeam(string userId, int teamId)
        {
            AppUser userToRemove = _db.Users
               .Where(x => x.Id == userId)
               .FirstOrDefault();

            if (userToRemove == null)
            {
                return NotFound();
            }

            userToRemove.TeamId = null;
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new { id = teamId });
        }

        public async Task<IActionResult> Create()
        {
            TeamCreateViewModel data = new TeamCreateViewModel();
            data.UsersWithNoTeam = await _db.Users
                .Where(x => x.TeamId == null)
                .ToListAsync();

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TeamCreateViewModel data)
        {
            AppUser teamLeader = await _db.Users
                .Where(x => x.Id == data.TeamToCreate.LeaderId)
                .FirstOrDefaultAsync();
            
            if (teamLeader == null)
            {
                return NotFound();
            }

            await _db.Teams.AddAsync(data.TeamToCreate);

            await _db.SaveChangesAsync();

            teamLeader.TeamId = data.TeamToCreate.Id;


            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new {id= data.TeamToCreate.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Team teamToDelete = await _db.Teams
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (teamToDelete == null)
            {
                return NotFound();
            }

            _db.Teams.Remove(teamToDelete);
            await _db.SaveChangesAsync();

            return RedirectToAction("index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            Team team = await _db.Teams
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();

            if (team == null)
            {
                return NotFound();
            }
            TeamEditViewModel data = new TeamEditViewModel();
            data.Team = team;
            data.Team.Leader = await _userManager.FindByIdAsync(team.LeaderId);
            data.UserNotLeading = new List<AppUser>();
            data.OldTeamLeadId = team.LeaderId;
            foreach (var user in _userManager.Users)
            {
                if (!await _userManager.IsInRoleAsync(user, "Team Lead"))
                {
                    data.UserNotLeading.Add(user);
                }
            }

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TeamEditViewModel data)
        {
            if (data.OldTeamLeadId != data.Team.LeaderId)
            {
                AppUser newTeamLead = await _userManager.FindByIdAsync(data.Team.LeaderId);
                if (!await _userManager.IsInRoleAsync(newTeamLead, "Team Lead"))
                {
                    await _userManager.AddToRoleAsync(newTeamLead, "Team Lead");
                }
                if (newTeamLead.TeamId != data.Team.Id)
                {
                    newTeamLead.TeamId = data.Team.Id;
                }

                AppUser oldTeamLead = await _userManager.FindByIdAsync(data.OldTeamLeadId);
                if (await _userManager.IsInRoleAsync(oldTeamLead, "Team Lead"))
                {
                    await _userManager.RemoveFromRoleAsync(oldTeamLead, "Team Lead");
                }
            }

            _db.Teams.Update(data.Team);

            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new {id=data.Team.Id});
        }
    }
}
