
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SparkHRMS.Data;
using SparkHRMS.Data.Entities.ProjectTransactions;
using SparkHRMS.Dtos;
using SparkHRMS.Services;
using SparkHRMS.ViewModels;


namespace SparkHRMS.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserResolverService _userResolver;

        public ClientController(ApplicationDbContext context, UserResolverService userResolver)
        {
            _context = context;
            _userResolver = userResolver;
        }

        [HttpGet]
        public ActionResult AddClient()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateClient(CreateClientDto client)
        {

            if (client is null) return BadRequest("client object null");

            if(client.Name.IsNullOrEmpty() || client.Email.IsNullOrEmpty() || client.Address.IsNullOrEmpty() || client.Description.IsNullOrEmpty())
            {
                return BadRequest("client object not valid");
            }

            if(
                (client.CountryCode.IsNullOrEmpty() && !client.PhoneNumber.IsNullOrEmpty())
                || 
                (!client.CountryCode.IsNullOrEmpty() && client.PhoneNumber.IsNullOrEmpty())
                )
            {
                return BadRequest("country code and phone number both are should have value");
            }

            bool isClientNameExist = _context.Clients.Any(x => x.Name.Equals(client.Name));

            if (isClientNameExist) return BadRequest("Client Name aready exists. Please try to different one.");

            Client newClient = new Client();
            newClient.Name = client.Name;
            newClient.Email = client.Email;
            newClient.CountryCode = client.CountryCode;
            newClient.PhoneNumber = client.PhoneNumber;
            newClient.Description = client.Description;
            newClient.Address = client.Address;
            newClient.CreatedBy = _userResolver.GetUserId().Value;
            newClient.ModifiedBy = _userResolver.GetUserId().Value;


            _context.Clients.Add(newClient);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllClientsView));

        }


        [HttpGet]
        public IActionResult AllClientsView()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AllClients(int PageNo, int Count)
        {

            int skipCount = (PageNo - 1) * Count;

            var query = from c in _context.Clients
                        orderby c.ModifiedTime ?? c.CreatedTime descending
                        where c.IsDeleted == false
                        select new
                        {
                            id = c.Id,
                            name = c.Name,
                            email = c.Email,
                            countryCode = c.CountryCode,
                            phoneNumber = c.PhoneNumber,
                            description = c.Description,
                            address = c.Address,
                            isActive = c.IsActive,
                            createdTime = c.CreatedTime,
                            modifiedTime = c.ModifiedTime
                        };

            int totalCount = query.Count();
            int totalNumberPages = (int)Math.Ceiling((double)totalCount / Count);

            List<ClientVM> clients = await query
                .Skip(skipCount).Take(Count).Select(client => new ClientVM
            {
                ClientId = client.id,
                Name = client.name,
                Email = client.email,
                CountryCode = client.countryCode ?? string.Empty,
                PhoneNumber = client.phoneNumber ?? string.Empty,
                Description = client.description,
                Address = client.address,
                IsActive = client.isActive
            }).ToListAsync();

            int previousPageNumber = PageNo - 1 == 0 ? PageNo : PageNo - 1;
            int nextPageNumber = PageNo >= totalNumberPages ? totalNumberPages : PageNo + 1;

			return Json(new AllClientVM
			{
				Clients = clients,
				TotalNumberOfPage = totalNumberPages,
				CurrentPageNumber = PageNo,
                PreviousPageNumber = previousPageNumber,
                NextPageNumber = nextPageNumber
			});
        }


        public async Task<IActionResult> EditClientView(int ClientId)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(x => x.Id == ClientId);

            if (client is null) return NotFound("Client Not Found");

			ClientVM clientDetail = new ClientVM
			{
                ClientId = client.Id,
                Name = client.Name,
                Email = client.Email ?? string.Empty,
                CountryCode = client.CountryCode ?? string.Empty,
                PhoneNumber = client.PhoneNumber ?? string.Empty,
                Description = client.Description ?? string.Empty,
                Address = client.Address ?? string.Empty,
			};


			return View(clientDetail);
        }


		[HttpPost("/edit-client/{id}")]
		public async Task<IActionResult> EditClient(int id, CreateClientDto client)
		{
            if(client is null)
            {
                return BadRequest("client object null");
            }

            Client? existClient = await _context.Clients.FirstOrDefaultAsync(client => client.Id == id);

            if (existClient is null) return NotFound("client not found");

            existClient.Name = client.Name;
            existClient.Email = client.Email;
            existClient.CountryCode = client.CountryCode;
            existClient.Address = client.Address;
            existClient.PhoneNumber = client.PhoneNumber;
            existClient.Description = client.Description;
            existClient.ModifiedTime = DateTime.Now;
            existClient.ModifiedBy = _userResolver.GetUserId().Value;

            _context.Entry(existClient).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllClientsView));
        }


        public async Task<IActionResult> DeleteClient(int ClientId)
        {
            Client? existClient = await _context.Clients.FirstOrDefaultAsync(client => client.Id == ClientId);

            if (existClient is null) return NotFound("client not found");

            existClient.IsDeleted = true;
            existClient.DeletedDateTime = DateTime.Now;
            existClient.DeletedBy = _userResolver.GetUserId().Value;

            _context.Entry(existClient).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllClientsView));
        }


        [HttpGet]
        public async Task<List<string>> GetAllClientNames()
        {

            var query = _context.Clients.Where(client => client.IsDeleted == false && client.IsActive == true)
                .Select(c => c.Name);

            return await query.ToListAsync() ?? new List<string>();
        }
    }
}
