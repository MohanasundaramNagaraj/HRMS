using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities.ProjectTransactions;
using SparkHRMS.Data.Entities.ProjectTransactions.StatusTransaction;
using SparkHRMS.Dtos;
using SparkHRMS.Services;
using SparkHRMS.ViewModels;
using Utility = SparkHRMS.Utilities.Utility;

namespace SparkHRMS.Controllers
{
    public class ProjectController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly DefaultModuleStatus _moduleStatus;
		private readonly UserResolverService _userResolver;
		private readonly ILogger<ProjectController> _logger;

		public ProjectController(ApplicationDbContext context, IOptions<DefaultModuleStatus> moduleStatus, UserResolverService userResolver, ILogger<ProjectController> logger)
        {
			_context = context;
			_moduleStatus = moduleStatus.Value;
			_userResolver = userResolver;
			_logger = logger;
        }

		[HttpGet]
        public IActionResult AllProjectView()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> GetAllProjects(int PageNumber, int PageSize, string SearchText)
		{
			int skipCount = (PageNumber - 1) * PageSize;

			
			var query = from p in _context.Projects
						join c in _context.Clients on p.ClientId equals c.Id into clients
						from client in clients.DefaultIfEmpty() 
						join module_status in _context.ModuleStatuses on p.StatusId equals module_status.Id into module_statuses
						from status in module_statuses.DefaultIfEmpty() 
						where p.IsDeleted == false && p.IsActive
						orderby p.CreatedTime descending
						select new
						{
							id = p.Id,
							name = p.Name,
							clientId = client.Id,  
							clientName = client.Name ?? "No Client",
							statusId = p.StatusId,
							status = status.Status ?? "No Status",
							plannedEndData = p.PlannedEndDate,
							members = _context.ProjectMembers
											.Where(pm => pm.ProjectId == p.Id)
											.Select(pm => new {
												memberId = pm.EmployeeId,
												memberName = pm.Member.Name 
											}).ToList() 
						};

			if (!string.IsNullOrWhiteSpace(SearchText))
			{
				query = query.Where(p => p.name.Contains(SearchText));
			}

			int totalCount = query.Count();
			int totalNumberPages = (int)Math.Ceiling((double)totalCount / PageSize);

			var pageQuery = await query
				.Skip(skipCount).Take(PageSize).Select(p => new
				{
					ProjectId = p.id,
					ProjectName = p.name,
					ClientName = p.clientName,
					PlannedEndDate = p.plannedEndData,
					Status = p.status,
					StatusId = p.statusId,
					Members = p.members.Select(m => new Member
					{
						Id = m.memberId,
						Name = m.memberName
					}).ToList()
				}).ToListAsync();

			List<ProjectVM> projects = pageQuery
				.Select(p => new ProjectVM
				{
					ProjectId = p.ProjectId,
					ProjectName = p.ProjectName,
					ClientName = p.ClientName,
					PlannedEndDate = p.PlannedEndDate,
					Status = p.Status,
					ProgressPercentage = getProjectStatusPercentage(p.ProjectId, p.StatusId.Value),
					Members = p.Members
				}).ToList();

			int previousPageNumber = PageNumber - 1 == 0 ? PageNumber : PageNumber - 1;
			int nextPageNumber = PageNumber >= totalNumberPages ? totalNumberPages : PageNumber + 1;

			return Json(new AllProjectVM
			{
				Projects = projects,
				TotalNumberOfPage = totalNumberPages,
				CurrentPageNumber = PageNumber,
				PreviousPageNumber = previousPageNumber,
				NextPageNumber = nextPageNumber,
				DataCount = PageSize
			});
		}

		private int getProjectStatusPercentage(int ProjectId, int StatusId)
		{

			var query = _context.GetProjectStatus(ProjectId).Result;

			int total = query.Count();

			int part = query.First(x => x.ModuleStatusId == StatusId).DisplaySequence;

			double percentage = Utility.GetPercentage(Convert.ToDouble(part), Convert.ToDouble(total));

			return Convert.ToInt32(percentage);
		}

		[HttpGet]
		public async Task<IActionResult> ProjectCreateForm()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateProject(CreateProjectDto project)
		{

			if(project is null) return BadRequest("does not accept null value");

			Dictionary<string, string> errorMessages = new Dictionary<string, string>();

			if (string.IsNullOrWhiteSpace(project.Name)) 
				errorMessages.Add(nameof(project.Name), "Project name is required.");

			if (string.IsNullOrWhiteSpace(project.ProjectClientName)) 
				errorMessages.Add(nameof(project.ProjectClientName), "Client name is required.");

			if (string.IsNullOrWhiteSpace(project.Description))
				errorMessages.Add(nameof(project.Description), "Description is required.");

			if (project.PlannedStartDate == default) 
				errorMessages.Add(nameof(project.PlannedStartDate), "Planned start date is required.");

			if (project.PlannedEndDate == default) 
				errorMessages.Add(nameof(project.PlannedEndDate), "Planned end date is required.");

			if (errorMessages is not null && errorMessages.Any()) return BadRequest(errorMessages);

			if (project.PlannedStartDate < DateTime.Now) 
				errorMessages.Add(nameof(project.PlannedStartDate), "Planned start date should be in the future.");

			if (project.PlannedStartDate > project.PlannedEndDate)
				errorMessages.Add(nameof(project.PlannedEndDate), "Planned end date should be later than both the current time and the planned start date.");

			if (errorMessages is not null && errorMessages.Any()) return BadRequest(errorMessages);

			Client? client = await _context.Clients.FirstOrDefaultAsync(x => x.Name == project.ProjectClientName);

			if (client == null)
				return NotFound("client not fouond");

			using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				Project newProject = new Project
				{
					Code = generateProjectCode(),
					Name = project.Name,
					Description = project.Description,
					ClientId = client.Id,
					CreatedBy = _userResolver.GetUserId().Value,
					PlannedStartDate = project.PlannedStartDate,
					PlannedEndDate = project.PlannedEndDate
				};

				_context.Projects.Add(newProject);
				await _context.SaveChangesAsync();

				List<ModuleStatus> moduleStatuses = new List<ModuleStatus>();

				foreach (var statusDetail in _moduleStatus.Status)
				{
					ModuleStatus moduleStatus = new ModuleStatus
					{
						Status = statusDetail.Name,
						DisplaySequence = statusDetail.DisplaySequence,
						ModuleId = _context.GetModule(Constants.ModuleConstants.ModuleCode.Project)?.Id ?? null
					};

					ProjectStatus projectStatus = new ProjectStatus
					{
						ProjectId = newProject.Id
					};

					List<ProjectStatus> projectStatuses = new List<ProjectStatus>();
					projectStatuses.Add(projectStatus);

					moduleStatus.ProjectStatuses = projectStatuses;

					moduleStatuses.Add(moduleStatus);
				}

				_context.ModuleStatuses.AddRange(moduleStatuses);

				await _context.SaveChangesAsync();

				await _context.UpdateStatus(_moduleStatus.Status.First()?.Name, Constants.ModuleConstants.ModuleCode.Project, newProject.Id);

				await transaction.CommitAsync();

			}
			catch(Exception ex)
			{
				_logger.LogWarning("Database Transaction Failed", ex);

				try
				{
					await transaction.RollbackAsync();
				}
				catch(Exception ex_ro)
				{
					_logger.LogWarning("Database transaction rolback failed", ex_ro);
				}

				return BadRequest("Project Creation Failed");
			}

			return RedirectToAction(nameof(AllProjectView));
		}

		private string generateProjectCode()
		{
			return $"{Constants.ModuleConstants.ModuleCode.Project}-{new Random().Next(1, 99999)}-{DateTime.Now.Year}";
		}

		public async Task<IActionResult> DeleteProject(int ProjectId)
		{
			var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == ProjectId);

			if (project is null) return NotFound("Project not Found");

			_context.Entry(project).State = EntityState.Deleted;

			await _context.SaveChangesAsync();

			return Ok();
		}

		[HttpGet]
		public IActionResult GetProjectDetail(int ProjectId)
		{
			return View();
		}
	}
}
