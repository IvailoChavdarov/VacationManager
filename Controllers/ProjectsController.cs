using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using VacationManager.Data;
using VacationManager.Models;
using VacationManager.ViewModels;

namespace VacationManager.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _db;
        public ProjectsController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }

        //shows view with all projects
        [Authorize]
        public IActionResult Index()
        {
            List<Project> data = new List<Project>();
            data = _db.Projects
                .ToList();

            return View(data);
        }

        //deletes project
        [Authorize(Roles = "CEO")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Project projectToDelete = await _db.Projects
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (projectToDelete == null)
            {
                return NotFound();
            }

            _db.Projects.Remove(projectToDelete);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        //shows view with form for creating project
        [Authorize(Roles = "CEO")]
        public IActionResult Create()
        {
            return View();
        }


        //creates project
        [Authorize(Roles = "CEO")]
        [HttpPost]
        public async Task<IActionResult> Create(Project project)
        {
            try {
                await _db.Projects.AddAsync(project);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(project);
            }
        }


        //shows view with form for editing project
        [Authorize(Roles = "CEO")]
        public IActionResult Edit(int id)
        {
            Project project = _db.Projects
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (project == null) {
                return NotFound();
            }

            return View(project);
        }


        //updates project data
        [HttpPost]
        [Authorize(Roles = "CEO")]
        public async Task<IActionResult> Edit(Project project)
        {
            try
            {
                _db.Projects.Update(project);
                await _db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = project.Id });
            }
            catch
            {
                return View(project);
            }
        }


        //shows view with details of project and teams working on the project
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            Project project = _db.Projects
                .Where(x => x.Id == id)
                .Include(x=>x.TeamsAtWork)
                .FirstOrDefault();

            if (project == null)
            {
                return NotFound();
            }

            ProjectDetailsViewModel data = new ProjectDetailsViewModel();

            data.Project = project;

            data.TeamsWithoutProject = await _db.Teams
                .Where(x=>x.ProjectId==null)
                .ToListAsync();

            data.ProjectId = id;

            return View(data);
        }

        //removes team from working on the project
        [HttpPost]
        [Authorize(Roles = "CEO")]
        public async Task<IActionResult> RemoveTeamFromProject(int teamId, int projectId)
        {
            Project project = _db.Projects
                .Where(x => x.Id == projectId)
                .FirstOrDefault();

            if (project == null)
            {
                return NotFound();
            }

            Team team = _db.Teams
                .Where(x => x.Id == teamId)
                .FirstOrDefault();

            if (team == null)
            {
                return NotFound();
            }
            if (team.ProjectId == projectId)
            {
                team.ProjectId = null;
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", new { id = projectId });
        }


        //adds team to work on the project
        [HttpPost]
        [Authorize(Roles = "CEO")]
        public async Task<IActionResult> AddTeamToProject(ProjectDetailsViewModel data)
        {
            Project project = _db.Projects
                .Where(x => x.Id == data.ProjectId)
                .FirstOrDefault();

            if (project == null)
            {
                return NotFound();
            }

            Team team = _db.Teams
                .Where(x => x.Id == data.TeamToAddId)
                .FirstOrDefault();

            if (team == null)
            {
                return NotFound();
            }
            team.ProjectId = data.ProjectId;
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new {id=data.ProjectId});
        }
    }
}
