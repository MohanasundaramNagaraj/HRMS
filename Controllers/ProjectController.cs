using Microsoft.AspNetCore.Mvc;
using SparkHRMS.Data;
using SparkHRMS.Dtos;

namespace SparkHRMS.Controllers
{
	public class ProjectController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ProjectController(ApplicationDbContext context)
        {
			_context = context;
        }

		[HttpGet]
        public IActionResult AllProjectView()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> GetAllProjects(int PageNumber, int PageSize)
		{
			return Ok();
		}

		[HttpGet]
		public async Task<IActionResult> ProjectCreateForm()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> GetProjectDetail()
		{
			return Ok();
		}


		[HttpPost]
		public async Task<IActionResult> CreateProject(CreateProjectDto project)
		{
			return Ok();
		}
	}

}
